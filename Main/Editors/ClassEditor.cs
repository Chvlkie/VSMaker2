using Main.Models;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace Main
{
    // CLASS EDITOR
    public partial class Mainform : Form
    {
        private bool ClassNameEdited;
        private bool ClassPropertyEdited;
        private bool InhibitClassChange = false;
        private TrainerClass SelectedClass = new();
        private List<string> UnfilteredClasses = [];
        private bool UnsavedClassChanges => ClassNameEdited || ClassPropertyEdited;

        private void class_ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData && class_ClassListBox.SelectedIndex > -1)
            {
                string selectedClass = class_ClassListBox.SelectedItem.ToString();

                if (selectedClass != SelectedClass.ListName)
                {
                    if (UnsavedClassChanges && !InhibitClassChange && !ConfirmUnsavedChanges())
                    {
                        InhibitClassChange = true;
                        class_ClassListBox.SelectedIndex = class_ClassListBox.Items.IndexOf(SelectedClass.ListName);
                        return;
                    }

                    if (!InhibitClassChange)
                    {
                        SelectedClass = classEditorMethods.GetTrainerClass(
                            MainEditorModel.Classes,
                            TrainerClass.ListNameToTrainerClassId(selectedClass)
                        );
                        class_ViewTrainerBtn.Enabled = false;
                        PopulateTrainerClassData();
                        PopulateUsedByTrainers(SelectedClass.UsedByTrainers);
                        EnableClassEditor();
                    }
                    InhibitClassChange = false;
                }
            }
        }

        private void class_ClearFilterBtn_Click(object sender, EventArgs e)
        {
            class_FilterTextBox.Text = "";
        }

        private void class_DescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_EyeContactDayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_EyeContactNightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
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

        private void class_GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                class_NameTextBox.BackColor = Color.White;
                EditedTrainerClassName(true);
            }
        }

        private void class_NewClassInfoBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Trainer Classes rely on data from specific tables:\n" +
                "Class Gender Table and Prize Money table.\n\n" +
                "In order to create a new Trainer Class, these required tables must first be expanded. " +
                "It is possible to apply patches to expand these tables from the ROM Patcher Menu.\n" +
                "To open these patches select Tools > ROM Patcher.\n\n" +
                "IMPORTANT: Applying patches is an advanced feature and has the potential of corrupting your ROM. " +
                "Before applying any ROM patch, make a back up of your project to avoid any potential loss of work.",
                "Creating a New Class");
        }

        private void class_PrizeMoneyNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_SaveClassBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                IsLoadingData = true;
                if (ValidateClassName() && SaveClassName(SelectedClass.TrainerClassId) && SaveTrainerClassProperties(SelectedClass.TrainerClassId))
                {
                    MessageBox.Show("Class data updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                IsLoadingData = false;
            }
        }

        private void class_SavePropertyBtn_Click(object sender, EventArgs e)
        {
            if (SaveTrainerClassProperties(SelectedClass.TrainerClassId))
            {
                EditedTrainerClassProperties(false);
                MessageBox.Show("Class Properties updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void class_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            class_ViewTrainerBtn.Enabled = !IsLoadingData && class_TrainersListBox.SelectedIndex > -1;
        }

        private void class_UndoAllBtn_Click(object sender, EventArgs e)
        {
            ClearUnsavedClassChanges();
            UndoAllClassChanges();
        }

        private void class_UndoPropertiesBtn_Click(object sender, EventArgs e)
        {
            ClearUnsavedClassPropertiesChanges();
            UndoClassPropertiesChanges();
        }

        private void class_ViewTrainerBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData && class_TrainersListBox.SelectedIndex > -1)
            {
                if (UnsavedTrainerEditorChanges && ConfirmUnsavedChanges())
                {
                    ClearUnsavedTrainerChanges();
                    int trainerId = Trainer.ListNameToTrainerId(class_TrainersListBox.SelectedItem.ToString());
                    GoToSelectedTrainer(trainerId);
                }
                else if (!UnsavedTrainerEditorChanges)
                {
                    int trainerId = Trainer.ListNameToTrainerId(class_TrainersListBox.SelectedItem.ToString());
                    GoToSelectedTrainer(trainerId);
                }
            }
        }

        private void ClearUnsavedClassChanges()
        {
            EditedTrainerClassName(false);
            EditedTrainerClassProperties(false);
        }

        private void ClearUnsavedClassPropertiesChanges()
        {
            EditedTrainerClassProperties(false);
        }

        private void EditedTrainerClassName(bool hasChanges)
        {
            ClassNameEdited = hasChanges;
            main_MainTab_ClassTab.Text = UnsavedClassChanges ? "Classes *" : "Classes";
            class_UndoAllBtn.Enabled = UnsavedClassChanges;
        }

        private void EditedTrainerClassProperties(bool hasChanges)
        {
            ClassPropertyEdited = hasChanges;
            main_MainTab_ClassTab.Text = UnsavedClassChanges ? "Classes *" : "Classes";
            class_UndoAllBtn.Enabled = UnsavedClassChanges;
            class_UndoPropertiesBtn.Enabled = ClassPropertyEdited;
        }

        private void EnableClassEditor()
        {
            class_EyeContactNightComboBox.Enabled = LoadedRom.IsHeartGoldSoulSilver;
            class_EyeContactNightComboBox.Visible = LoadedRom.IsHeartGoldSoulSilver;
            class_PrizeMoneyNum.Enabled = true;
            class_TrainersListBox.Enabled = true;
            class_NameTextBox.Enabled = true;
            class_EyeContactDayComboBox.Enabled = true;
            class_SaveClassBtn.Enabled = true;
            class_DescriptionTextBox.Enabled = true;
            class_SavePropertyBtn.Enabled = true;
            class_GenderComboBox.Enabled = true;

            if (class_EyeContactDayComboBox.SelectedIndex < 0)
            {
                class_EyeContactDayComboBox.Enabled = false;
            }
            if (class_EyeContactNightComboBox.SelectedIndex < 0)
            {
                class_EyeContactNightComboBox.Enabled = false;
            }
        }

        private void GoToSelectedTrainer(int trainerId)
        {
            trainer_FilterBox.Text = "";
            main_MainTab.SelectedTab = main_MainTab_TrainerTab;
            trainer_TrainersListBox.SelectedIndex = trainerId - 1;
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
            class_SavePropertyBtn.Enabled = false;
            class_UndoPropertiesBtn.Enabled = false;
            class_PropertyCopyBtn.Enabled = false;
            class_PropertyExportBtn.Enabled = false;
            class_PropertyImportBtn.Enabled = false;
            class_PropertyPasteBtn.Enabled = false;
            class_EyeContactDaySoundBtn.Enabled = false;
            class_EyeContactNightPlayBtn.Enabled = false;
            class_EyeContactHelpBtn.Enabled = false;
            class_VSEffectsListBox.Enabled = false;
            class_InBattleMusicPlayBtn.Enabled = false;
        }

        private void PopulateClassGenderList()
        {
            class_GenderComboBox.Items.Clear();
            class_GenderComboBox.Items.AddRange(Gender.ClassGenders.ToArray());
        }

        private void PopulateClassList(List<TrainerClass> classes)
        {
            class_ClassListBox.Items.Clear();
            UnfilteredClasses = classes.Select(item => item.ListName).ToList();
            class_ClassListBox.Items.AddRange(UnfilteredClasses.ToArray());
        }

        private void PopulateEyeContactMusic(ComboBox comboBox, GameFamily gameFamily)
        {
            comboBox.Items.Clear();
            IEnumerable<string> musicList = gameFamily switch
            {
                GameFamily.DiamondPearl => EyeContactMusics.DiamondPearl.Select(x => x.ListName),
                GameFamily.HeartGoldSoulSilver => EyeContactMusics.HeartGoldSoulSilver.Select(x => x.ListName),
                GameFamily.Platinum => EyeContactMusics.Platinum.Select(x => x.ListName),
                _ => Enumerable.Empty<string>()
            };
            comboBox.Items.AddRange(musicList.ToArray());
        }

        private void PopulateTrainerClassData()
        {
            IsLoadingData = true;
            SetClassName();
            SetClassProperties();
            IsLoadingData = false;
        }

        private void PopulateUsedByTrainers(List<Trainer> usedByTrainers)
        {
            class_TrainersListBox.Items.Clear();
            foreach (var trainer in usedByTrainers)
            {
                class_TrainersListBox.Items.Add(trainer.ListName);
            }
        }

        private bool SaveClassGender(int classId, int classGender)
        {
            var index = LoadedRom.ClassGenderData.FindIndex(x => x.TrainerClassId == classId);
            if (index > -1)
            {
                var classGenderData = new ClassGenderData(LoadedRom.ClassGenderData[index].Offset, (byte)classGender, classId);
                var writeClassGenderData = fileSystemMethods.WriteClassGenderData(classGenderData);
                if (writeClassGenderData.Success)
                {
                    SelectedClass.ClassProperties.Gender = classGenderData.Gender;
                    MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.Gender = classGenderData.Gender;
                    LoadedRom.ClassGenderData[index].Gender = classGenderData.Gender;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool SaveClassName(int classId)
        {
            var saveClass = fileSystemMethods.WriteClassName(MainEditorModel.ClassNames, classId, class_NameTextBox.Text, LoadedRom.ClassNamesTextNumber);
            if (saveClass.Success)
            {
                class_NameTextBox.BackColor = Color.White;
                MainEditorModel.ClassNames[classId] = class_NameTextBox.Text;
                MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).TrainerClassName = class_NameTextBox.Text;
                var index = class_ClassListBox.FindString(UnfilteredClasses[classId - 2]);
                if (index > -1)
                {
                    class_ClassListBox.Items[index] = MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ListName;
                    class_ClassListBox.SelectedIndex = index;
                }
                UnfilteredClasses[classId - 2] = MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ListName;
                EditedTrainerClassName(false);
            }
            else
            {
                MessageBox.Show(saveClass.ErrorMessage, "Unable to Save Class", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return saveClass.Success;
        }

        private bool SaveEyeContactData(int classId, int eyeContactDay, int? eyeContactNight = null)
        {
            var index = LoadedRom.EyeContactMusicData.FindIndex(x => x.TrainerClassId == classId);
            if (index > -1)
            {
                var eyeContactMusicData = new EyeContactMusicData(LoadedRom.EyeContactMusicData[index].Offset, (ushort)classId, (ushort)eyeContactDay, (ushort?)eyeContactNight);

                var writeEyeContactMusic = fileSystemMethods.WriteEyeContactMusicData(eyeContactMusicData, LoadedRom);
                if (writeEyeContactMusic.Success)
                {
                    SelectedClass.ClassProperties.EyeContactMusicDay = eyeContactDay;
                    SelectedClass.ClassProperties.EyeContactMusicNight = eyeContactNight;
                    MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.EyeContactMusicDay = eyeContactDay;
                    MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.EyeContactMusicNight = eyeContactNight;
                    LoadedRom.EyeContactMusicData[index] = eyeContactMusicData;
                    return true;
                }
                else
                {
                    MessageBox.Show(writeEyeContactMusic.ErrorMessage, "Unable to Save Eye Contact Music Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private bool SavePrizeMoneyData(int classId, int prizeMoneyMultiplier)
        {
            var index = LoadedRom.PrizeMoneyData.FindIndex(x => x.TrainerClassId == classId);
            if (index > -1)
            {
                var prizeMoneyData = new PrizeMoneyData(LoadedRom.PrizeMoneyData[index].Offset, (ushort)classId, (ushort)prizeMoneyMultiplier);

                var writePrizeMoney = fileSystemMethods.WritePrizeMoneyData(prizeMoneyData, LoadedRom);
                if (writePrizeMoney.Success)
                {
                    SelectedClass.ClassProperties.PrizeMoneyMultiplier = prizeMoneyMultiplier;
                    MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.PrizeMoneyMultiplier = prizeMoneyMultiplier;
                    LoadedRom.PrizeMoneyData[index] = prizeMoneyData;
                    return true;
                }
                else
                {
                    MessageBox.Show(writePrizeMoney.ErrorMessage, "Unable to Save Prize Money Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private bool SaveTrainerClassDescription(int classId, string newDescription)
        {
            var saveDescription = fileSystemMethods.WriteClassDescription(MainEditorModel.ClassDescriptions, classId, newDescription, LoadedRom.ClassDescriptionMessageNumber);
            if (saveDescription.Success)
            {
                MainEditorModel.ClassDescriptions[classId] = newDescription;
                return true;
            }
            else { return false; }
        }

        private bool SaveTrainerClassProperties(int classId)
        {
            int gender = class_GenderComboBox.SelectedIndex;
            int prizeMoneyMultiplier = (int)class_PrizeMoneyNum.Value;
            string newDescription = class_DescriptionTextBox.Text;
            int eyeContactMusicDay = class_EyeContactDayComboBox.Enabled ? EyeContactMusic.ListNameToId(class_EyeContactDayComboBox.SelectedItem.ToString()) : -1;
            int? eyeContactMusicNight = LoadedRom.IsHeartGoldSoulSilver && class_EyeContactNightComboBox.Enabled ? EyeContactMusic.ListNameToId(class_EyeContactNightComboBox.SelectedItem.ToString()) : null;

            if (SavePrizeMoneyData(classId, prizeMoneyMultiplier)
                && SaveEyeContactData(classId, eyeContactMusicDay, eyeContactMusicNight)
                && SaveClassGender(classId, gender)
                && SaveTrainerClassDescription(classId, newDescription))
            {
                MainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties = new TrainerClassProperty(gender, prizeMoneyMultiplier, newDescription, eyeContactMusicDay, eyeContactMusicNight);
                EditedTrainerClassProperties(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetClassName()
        {
            class_NameTextBox.Text = SelectedClass.TrainerClassName;
        }

        private void SetClassProperties()
        {
            class_GenderComboBox.SelectedIndex = SelectedClass.ClassProperties.Gender ?? -1;
            class_DescriptionTextBox.Text = SelectedClass.ClassProperties.Description;
            class_PrizeMoneyNum.Value = SelectedClass.ClassProperties.PrizeMoneyMultiplier;
            class_EyeContactDayComboBox.SelectedIndex = SetEyeContactMusicDay(RomFile.GameFamily);
            class_EyeContactNightComboBox.SelectedIndex = LoadedRom.IsHeartGoldSoulSilver ?
                EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == SelectedClass.ClassProperties.EyeContactMusicNight)
                : -1;
        }

        private int SetEyeContactMusicDay(GameFamily gameFamily) => gameFamily switch
        {
            GameFamily.DiamondPearl => EyeContactMusics.DiamondPearl.FindIndex(x => x.MusicId == SelectedClass.ClassProperties.EyeContactMusicDay),
            GameFamily.Platinum => EyeContactMusics.Platinum.FindIndex(x => x.MusicId == SelectedClass.ClassProperties.EyeContactMusicDay),
            GameFamily.HeartGoldSoulSilver => EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == SelectedClass.ClassProperties.EyeContactMusicDay),
            GameFamily.HgEngine => EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == SelectedClass.ClassProperties.EyeContactMusicDay),
            _ => -1
        };

        private void SetupClassEditor()
        {
            IsLoadingData = true;
            if (class_ClassListBox.Items.Count == 0)
            {
                PopulateClassList(MainEditorModel.Classes);
            }
            if (class_GenderComboBox.Items.Count == 0)
            {
                PopulateClassGenderList();
            }
            if (class_EyeContactDayComboBox.Items.Count == 0)
            {
                PopulateEyeContactMusic(class_EyeContactDayComboBox, RomFile.GameFamily);
            }
            if (class_EyeContactNightComboBox.Items.Count == 0)
            {
                PopulateEyeContactMusic(class_EyeContactNightComboBox, RomFile.GameFamily);
            }
            class_ClassListBox.Enabled = true;
            class_FilterTextBox.Enabled = true;
            IsLoadingData = false;
        }

        private void UndoAllClassChanges()
        {
            UndoClassNameChange();
            UndoClassPropertiesChanges();
        }

        private void UndoClassNameChange()
        {
            IsLoadingData = true;
            SetClassName();
            IsLoadingData = false;
        }

        private void UndoClassPropertiesChanges()
        {
            IsLoadingData = true;
            SetClassProperties();
            EditedTrainerClassProperties(false);
            IsLoadingData = false;
        }

        private bool ValidateClassName()
        {
            string className = class_NameTextBox.Text;
            string cleanClassName = className.Replace("[PKMN]", "").Replace("[M]", "");

            if (string.IsNullOrEmpty(cleanClassName))
            {
                MessageBox.Show("You must enter a class name", "Unable to Save Class", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (cleanClassName.Length > 16)
            {
                MessageBox.Show("Class name exceeds 16 characters. This might cause issues.", "Long Class Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return true;
        }
    }
}