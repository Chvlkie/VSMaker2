using Main.Forms;
using Main.Models;
using VsMaker2Core.DataModels;
using VsMaker2Core.Methods;
using static VsMaker2Core.Enums;

namespace Main
{
    // CLASS EDITOR
    public partial class Mainform : Form
    {
        private bool InhibitClassChange = false;
        private TrainerClass SelectedClass = new();
        private List<string> UnfilteredClasses = [];
        private bool UnsavedClassChanges;
        private void class_ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData && class_ClassListBox.SelectedIndex > -1)
            {
                string selectedClass = class_ClassListBox.SelectedItem.ToString();

                if (selectedClass != SelectedClass.ListName)
                {
                    if (UnsavedClassChanges && !InhibitClassChange)
                    {
                        if (ConfirmUnsavedChanges())
                        {
                            ClearUnsavedClassChanges();
                        }
                        else
                        {
                            InhibitClassChange = true;
                            class_ClassListBox.SelectedIndex = class_ClassListBox.Items.IndexOf(SelectedClass.ListName);
                        }
                    }

                    if (!InhibitClassChange)
                    {
                        selectedClass = class_ClassListBox.SelectedItem.ToString();
                        SelectedClass = classEditorMethods.GetTrainerClass(MainEditorModel.Classes, TrainerClass.ListNameToTrainerClassId(selectedClass));

                        if (SelectedClass.TrainerClassId > 0)
                        {
                            PopulateTrainerClassData();
                            EnableClassEditor();
                        }
                    }
                    else
                    {
                        InhibitClassChange = false;
                    }
                }
            }
        }
        private void PopulateTrainerClassData()
        {
            if (!IsLoadingData)
            {
                class_NameTextBox.Text = SelectedClass.TrainerClassName;
                class_PrizeMoneyNum.Value = SelectedClass.PrizeMoneyMultiplier;
               // class_EyeContactDayComboBox.SelectedIndex = (int)SelectedClass.EyeContactMusic;
                if (LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver)
                {
                //    class_EyeContactNightComboBox.SelectedIndex = (int)SelectedClass.EyeContactMusicNight.Value;
                }
               // class_GenderComboBox.SelectedIndex = SelectedClass.Gender;
            }
        }
        private void EnableClassEditor()
        {
            class_EyeContactNightComboBox.Enabled = LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver;
            class_EyeContactNightComboBox.Visible = LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver;
            class_PrizeMoneyNum.Enabled = true;
            class_NameTextBox.Enabled = true;
            class_EyeContactDayComboBox.Enabled = true;
            class_SaveClassBtn.Enabled = true;
        }
        private void class_ClearFilterBtn_Click(object sender, EventArgs e)
        {
            class_FilterTextBox.Text = "";
        }

        private void class_FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (string.IsNullOrEmpty(class_FilterTextBox.Text))
                {
                    PopulateClassList(MainEditorModel.Classes);
                    class_ClearFilterBtn.Enabled = false;
                }
                else
                {
                    FilterListBox(class_ClassListBox, class_FilterTextBox.Text, UnfilteredClasses);
                    class_ClearFilterBtn.Enabled = true;
                }
            }
        }

        private void ClearUnsavedClassChanges()
        {
            EditedTrainerClass(false);
        }

        private void EditedTrainerClass(bool hasChanges)
        {
            UnsavedClassChanges = hasChanges;
            main_MainTab_ClassTab.Text = UnsavedTrainerEditorChanges ? "Classes *" : "Classes";
            class_UndoAllBtn.Enabled = UnsavedClassChanges;
        }

        private void InitializeClassEditor()
        {
            class_UndoAllBtn.Enabled = false;
            class_SaveClassBtn.Enabled = false;
            class_AddClassBtn.Enabled = false;
            class_RemoveBtn.Enabled = false;
            class_ImportAllBtn.Enabled = false;
            class_ExportAllBtn.Enabled = false;
            class_FilterTextBox.Enabled = false;
            class_ClearFilterBtn.Enabled = false;
            class_TrainersListBox.Enabled = false;
            class_SpriteExportBtn.Enabled = false;
            class_SpriteFrameNum.Enabled = false;
            class_SpriteImportBtn.Enabled = false;
            class_CopyBtn.Enabled = false;
            class_PasteBtn.Enabled = false;
            class_ImportBtn.Enabled = false;
            class_ExportBtn.Enabled = false;
            class_ClassListBox.Enabled = false;
            class_ViewTrainerBtn.Enabled = false;
            class_NameTextBox.Enabled = false;
        }

        private void PopulateClassList(List<TrainerClass> classes)
        {
            class_ClassListBox.Items.Clear();
            UnfilteredClasses = [];
            foreach (var item in classes)
            {
                class_ClassListBox.Items.Add(item.ListName);
                UnfilteredClasses.Add(item.ListName);
            }
        }
        private void SetupClassEditor()
        {
            IsLoadingData = true;
            if (class_ClassListBox.Items.Count == 0)
            {
                PopulateClassList(MainEditorModel.Classes);
            }
            class_ClassListBox.Enabled = true;
            class_FilterTextBox.Enabled = true;
            IsLoadingData = false;
        }
    }
}