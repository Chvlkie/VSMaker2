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
        private List<string> UnfilteredClasses = [];
        private bool UnsavedClassChanges => ClassNameEdited || ClassPropertyEdited;

        public void RefreshTrainerClasses()
        {
            try
            {
                mainEditorModel.Classes = classEditorMethods.GetTrainerClasses(mainEditorModel.Trainers, mainEditorModel.ClassNames, mainEditorModel.ClassDescriptions);

                Console.WriteLine("Trainer classes refreshed successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing trainer classes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void PopulateEyeContactMusic(ComboBox comboBox, GameFamily gameFamily)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            IEnumerable<string> musicList = gameFamily switch
            {
                GameFamily.DiamondPearl => EyeContactMusics.DiamondPearl.Select(x => x.ListName),
                GameFamily.HeartGoldSoulSilver => EyeContactMusics.HeartGoldSoulSilver.Select(x => x.ListName),
                GameFamily.Platinum => EyeContactMusics.Platinum.Select(x => x.ListName),
                _ => []
            };
            comboBox.Items.AddRange(musicList.ToArray());
            comboBox.EndUpdate();
        }

        private void AddNewTrainerClass()
        {
            isLoadingData = true;
            class_FilterTextBox.Text = "";
            class_ClassListBox.SelectedIndex = -1;
            int newTrainerClassId = RomFile.TotalNumberOfTrainerClasses;
            mainEditorModel.ClassNames.Add("-");
            mainEditorModel.ClassDescriptions.Add("-");
            mainEditorModel.Classes.Add(new TrainerClass(newTrainerClassId));

            fileSystemMethods.WriteClassName(mainEditorModel.ClassNames, newTrainerClassId, "-", RomFile.ClassNamesTextNumber);
            fileSystemMethods.WriteClassDescription(mainEditorModel.ClassDescriptions, newTrainerClassId, "-", RomFile.ClassDescriptionMessageNumber);
            fileSystemMethods.AddNewTrainerClassSprite();
            UnfilteredClasses.Add(mainEditorModel.Classes.Single(x => x.TrainerClassId == newTrainerClassId).ListName);
            class_ClassListBox.Items.Add(mainEditorModel.Classes.Single(x => x.TrainerClassId == newTrainerClassId).ListName);
            RomFile.TotalNumberOfTrainerClasses++;

            isLoadingData = false;
            class_ClassListBox.SelectedIndex = newTrainerClassId - 2;
            EditedTrainerClassName(true);
            EditedTrainerClassProperties(true);
        }

        private void class_AddClassBtn_Click(object sender, EventArgs e)
        {
            if (UnsavedClassChanges && ConfirmUnsavedChanges())
            {
                ClearUnsavedTrainerChanges();
                ValidateAddClass();
            }
            else if (!UnsavedClassChanges)
            {
                ValidateAddClass();
            }
        }

        private void class_ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData && class_ClassListBox.SelectedIndex > -1)
            {
                string selectedClass = class_ClassListBox.SelectedItem!.ToString()!;

                if (selectedClass != mainEditorModel.SelectedTrainerClass.ListName)
                {
                    if (UnsavedClassChanges && !InhibitClassChange && !ConfirmUnsavedChanges())
                    {
                        InhibitClassChange = true;
                        class_ClassListBox.SelectedIndex = class_ClassListBox.Items.IndexOf(mainEditorModel.SelectedTrainerClass.ListName);
                        return;
                    }

                    if (!InhibitClassChange)
                    {
                        ClearUnsavedClassChanges();
                        ClearUnsavedClassPropertiesChanges();
                        mainEditorModel.SelectedTrainerClass = classEditorMethods.GetTrainerClass(
                            mainEditorModel.Classes,
                            TrainerClass.ListNameToTrainerClassId(selectedClass!)
                        );
                        class_ViewTrainerBtn.Enabled = false;
                        PopulateTrainerClassData(mainEditorModel.SelectedTrainerClass);
                        PopulateUsedByTrainers(mainEditorModel.SelectedTrainerClass.UsedByTrainers);
                        PopulateTrainerClassSprite(class_SpritePicBox, class_SpriteFrameNum, mainEditorModel.SelectedTrainerClass.TrainerClassId);
                        EnableClassEditor();
                    }
                    InhibitClassChange = false;
                }
            }
        }

        private void class_ClearFilterBtn_Click(object sender, EventArgs e) => class_FilterTextBox.Text = "";

        private void class_CopyBtn_Click(object sender, EventArgs e)
        {
            mainEditorModel.ClipboardTrainerClass = new TrainerClass(mainEditorModel.SelectedTrainerClass);
            class_PasteBtn.Enabled = true;
        }

        private void class_DescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_EyeContactDayComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_EyeContactNightComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                if (string.IsNullOrEmpty(class_FilterTextBox.Text))
                {
                    PopulateClassList(mainEditorModel.Classes);
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
            if (!isLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private void class_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                class_NameTextBox.BackColor = Color.White;
                EditedTrainerClassName(true);
            }
        }

        private void class_NewClassInfoBtn_Click(object sender, EventArgs e) => MessageBox.Show("Trainer Classes rely on data from specific tables:\n" +
                "Class Gender Table and Prize Money table.\n\n" +
                "In order to create a new Trainer Class, these required tables must first be expanded. " +
                "It is possible to apply patches to expand these tables from the ROM Patcher Menu.\n" +
                "To open these patches select Tools > ROM Patcher.\n\n" +
                "IMPORTANT: Applying patches is an advanced feature and has the potential of corrupting your ROM. " +
                "Before applying any ROM patch, make a back up of your project to avoid any potential loss of work.",
                "Creating a New Class");

        private void class_PasteBtn_Click(object sender, EventArgs e)
        {
            int selectedTrainerClassId = mainEditorModel.SelectedTrainerClass.TrainerClassId;
            var pasteTrainerClass = new TrainerClass(selectedTrainerClassId, mainEditorModel.ClipboardTrainerClass!);

            class_ViewTrainerBtn.Enabled = false;
            PopulateTrainerClassData(pasteTrainerClass);
            PopulateUsedByTrainers(mainEditorModel.SelectedTrainerClass.UsedByTrainers);
            PopulateTrainerClassSprite(class_SpritePicBox, class_SpriteFrameNum, pasteTrainerClass.TrainerClassId);
            EnableClassEditor();
            EditedTrainerClassName(true);
            EditedTrainerClassProperties(true);
        }
        private void class_PrizeMoneyNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerClassProperties(true);
            }
        }

        private async void class_SaveClassBtn_Click(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                isLoadingData = true;
                if (ValidateClassName() && SaveClassName(mainEditorModel.SelectedTrainerClass.TrainerClassId))
                {
                    bool saveProperties = await SaveTrainerClassPropertiesAsync(mainEditorModel.SelectedTrainerClass.TrainerClassId);
                    if (saveProperties)
                    {
                        MessageBox.Show("Class data updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                isLoadingData = false;
            }
        }

        private async void class_SavePropertyBtn_Click(object sender, EventArgs e)
        {
            if (await SaveTrainerClassPropertiesAsync(mainEditorModel.SelectedTrainerClass.TrainerClassId))
            {
                EditedTrainerClassProperties(false);
                MessageBox.Show("Class Properties updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void class_SpriteFrameNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                int trainerClassId = TrainerClass.ListNameToTrainerClassId(class_ClassListBox.SelectedItem.ToString());
                UpdateTrainerClassSprite(class_SpritePicBox, class_SpriteFrameNum, trainerClassId);
            }
        }

        private void class_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e) => class_ViewTrainerBtn.Enabled = !isLoadingData && class_TrainersListBox.SelectedIndex > -1;

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
            if (!isLoadingData && class_TrainersListBox.SelectedIndex > -1)
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

        private void ClearUnsavedClassPropertiesChanges() => EditedTrainerClassProperties(false);

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
            class_EyeContactNightComboBox.Enabled = RomFile.IsHeartGoldSoulSilver;
            class_EyeContactNightComboBox.Visible = RomFile.IsHeartGoldSoulSilver;
            class_PrizeMoneyNum.Enabled = true;
            class_TrainersListBox.Enabled = true;
            class_NameTextBox.Enabled = true;
            class_EyeContactDayComboBox.Enabled = true;
            class_SaveClassBtn.Enabled = true;
            class_DescriptionTextBox.Enabled = true;
            class_CopyBtn.Enabled = true;
            class_SavePropertyBtn.Enabled = true;
            class_GenderComboBox.Enabled = true;
            class_AddClassBtn.Enabled = RomFile.PrizeMoneyExpanded && RomFile.ClassGenderExpanded && RomFile.EyeContactExpanded;
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
            class_EyeContactDaySoundBtn.Enabled = false;
            class_EyeContactNightPlayBtn.Enabled = false;
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
            class_ClassListBox.BeginUpdate();
            class_ClassListBox.Items.Clear();
            UnfilteredClasses = classes.Select(item => item.ListName).ToList();
            class_ClassListBox.Items.AddRange(UnfilteredClasses.ToArray());
            class_ClassListBox.EndUpdate();
        }
        private void PopulateTrainerClassData(TrainerClass trainerClass)
        {
            isLoadingData = true;
            SetClassName(trainerClass);
            SetClassProperties(trainerClass);
            isLoadingData = false;
        }

        private void PopulateUsedByTrainers(List<Trainer> usedByTrainers)
        {
            class_TrainersListBox.BeginUpdate();
            class_TrainersListBox.Items.Clear();
            foreach (var trainer in usedByTrainers)
            {
                class_TrainersListBox.Items.Add(trainer.ListName);
            }
            class_TrainersListBox.EndUpdate();
        }

        private bool SaveClassGender(int classId, int classGender)
        {
            var index = RomFile.ClassGenderData.FindIndex(x => x.TrainerClassId == classId);
            if (index > -1)
            {
                var classGenderData = new ClassGenderData(RomFile.ClassGenderData[index].Offset, (byte)classGender, classId);
                var writeClassGenderData = fileSystemMethods.WriteClassGenderData(classGenderData);
                if (writeClassGenderData.Success)
                {
                    mainEditorModel.SelectedTrainerClass.ClassProperties.Gender = classGenderData.Gender;
                    mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.Gender = classGenderData.Gender;
                    RomFile.ClassGenderData[index].Gender = classGenderData.Gender;
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
            var saveClass = fileSystemMethods.WriteClassName(mainEditorModel.ClassNames, classId, class_NameTextBox.Text, RomFile.ClassNamesTextNumber);
            if (saveClass.Success)
            {
                class_NameTextBox.BackColor = Color.White;
                mainEditorModel.ClassNames[classId] = class_NameTextBox.Text;
                mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).TrainerClassName = class_NameTextBox.Text;
                var index = class_ClassListBox.FindString(UnfilteredClasses[classId - 2]);
                if (index > -1)
                {
                    class_ClassListBox.Items[index] = mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ListName;
                    class_ClassListBox.SelectedIndex = index;
                }
                UnfilteredClasses[classId - 2] = mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ListName;
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
            var index = RomFile.EyeContactMusicData.FindIndex(x => x.TrainerClassId == classId);
            if (index > -1)
            {
                var eyeContactMusicData = new EyeContactMusicData(RomFile.EyeContactMusicData[index].Offset, (ushort)classId, (ushort)eyeContactDay, (ushort?)eyeContactNight);

                var (Success, ErrorMessage) = fileSystemMethods.WriteEyeContactMusicData(eyeContactMusicData);
                if (Success)
                {
                    mainEditorModel.SelectedTrainerClass.ClassProperties.EyeContactMusicDay = eyeContactDay;
                    mainEditorModel.SelectedTrainerClass.ClassProperties.EyeContactMusicNight = eyeContactNight;
                    mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.EyeContactMusicDay = eyeContactDay;
                    mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.EyeContactMusicNight = eyeContactNight;
                    RomFile.EyeContactMusicData[index] = eyeContactMusicData;
                    return true;
                }
                else
                {
                    MessageBox.Show(ErrorMessage, "Unable to Save Eye Contact Music Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> SavePrizeMoneyDataAsync(int classId, int prizeMoneyMultiplier)
        {
            var index = RomFile.PrizeMoneyData.FindIndex(x => x.TrainerClassId == classId);

            if (index > -1)
            {
                var prizeMoneyData = new PrizeMoneyData(RomFile.PrizeMoneyData[index].Offset, (ushort)classId, (ushort)prizeMoneyMultiplier);

                var (Success, ErrorMessage) = await fileSystemMethods.WritePrizeMoneyDataAsync(prizeMoneyData);

                if (Success)
                {
                    mainEditorModel.SelectedTrainerClass.ClassProperties.PrizeMoneyMultiplier = prizeMoneyMultiplier;
                    mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties.PrizeMoneyMultiplier = prizeMoneyMultiplier;
                    RomFile.PrizeMoneyData[index] = prizeMoneyData;

                    return true;
                }
                else
                {
                    MessageBox.Show(ErrorMessage, "Unable to Save Prize Money Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        private bool SaveTrainerClassDescription(int classId, string newDescription)
        {
            var (Success, ErrorMessage) = fileSystemMethods.WriteClassDescription(mainEditorModel.ClassDescriptions, classId, newDescription, RomFile.ClassDescriptionMessageNumber);
            if (Success)
            {
                mainEditorModel.ClassDescriptions[classId] = newDescription;
                return true;
            }
            else { return false; }
        }

        private async Task<bool> SaveTrainerClassPropertiesAsync(int classId)
        {
            int gender = class_GenderComboBox.SelectedIndex;
            int prizeMoneyMultiplier = (int)class_PrizeMoneyNum.Value;
            string newDescription = class_DescriptionTextBox.Text;
            int eyeContactMusicDay = class_EyeContactDayComboBox.Enabled ? EyeContactMusic.ListNameToId(class_EyeContactDayComboBox.SelectedItem.ToString()) : -1;
            int? eyeContactMusicNight = RomFile.IsHeartGoldSoulSilver && class_EyeContactNightComboBox.Enabled ? EyeContactMusic.ListNameToId(class_EyeContactNightComboBox.SelectedItem.ToString()) : null;

            bool prizeMoneySaved = await SavePrizeMoneyDataAsync(classId, prizeMoneyMultiplier);
            bool eyeContactSaved = SaveEyeContactData(classId, eyeContactMusicDay, eyeContactMusicNight); // Assuming this is sync
            bool genderSaved = SaveClassGender(classId, gender); // Assuming this is sync
            bool descriptionSaved = SaveTrainerClassDescription(classId, newDescription); // Assuming this is sync

            if (prizeMoneySaved && eyeContactSaved && genderSaved && descriptionSaved)
            {
                mainEditorModel.Classes.Single(x => x.TrainerClassId == classId).ClassProperties = new TrainerClassProperty(gender, prizeMoneyMultiplier, newDescription, eyeContactMusicDay, eyeContactMusicNight);
                EditedTrainerClassProperties(false);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetClassName(TrainerClass trainerClass) => class_NameTextBox.Text = trainerClass.TrainerClassName;

        private void SetClassProperties(TrainerClass trainerClass)
        {
            class_GenderComboBox.SelectedIndex = trainerClass.ClassProperties.Gender ?? -1;
            class_DescriptionTextBox.Text = trainerClass.ClassProperties.Description;
            class_PrizeMoneyNum.Value = trainerClass.ClassProperties.PrizeMoneyMultiplier;
            class_EyeContactDayComboBox.SelectedIndex = SetEyeContactMusicDay(RomFile.GameFamily, trainerClass);
            class_EyeContactNightComboBox.SelectedIndex = RomFile.IsHeartGoldSoulSilver ?
                EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == trainerClass.ClassProperties.EyeContactMusicNight)
                : -1;
        }

        private int SetEyeContactMusicDay(GameFamily gameFamily, TrainerClass trainerClass) => gameFamily switch
        {
            GameFamily.DiamondPearl => EyeContactMusics.DiamondPearl.FindIndex(x => x.MusicId == trainerClass.ClassProperties.EyeContactMusicDay),
            GameFamily.Platinum => EyeContactMusics.Platinum.FindIndex(x => x.MusicId == trainerClass.ClassProperties.EyeContactMusicDay),
            GameFamily.HeartGoldSoulSilver => EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == trainerClass.ClassProperties.EyeContactMusicDay),
            GameFamily.HgEngine => EyeContactMusics.HeartGoldSoulSilver.FindIndex(x => x.MusicId == trainerClass.ClassProperties.EyeContactMusicDay),
            _ => -1
        };

        private void SetupClassEditor()
        {
            isLoadingData = true;
            if (class_ClassListBox.Items.Count == 0)
            {
                PopulateClassList(mainEditorModel.Classes);
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
            isLoadingData = false;
        }

        private void UndoAllClassChanges()
        {
            UndoClassNameChange();
            UndoClassPropertiesChanges();
        }

        private void UndoClassNameChange()
        {
            isLoadingData = true;
            SetClassName(mainEditorModel.SelectedTrainerClass);
            isLoadingData = false;
        }

        private void UndoClassPropertiesChanges()
        {
            isLoadingData = true;
            SetClassProperties(mainEditorModel.SelectedTrainerClass);
            EditedTrainerClassProperties(false);
            isLoadingData = false;
        }

        private void ValidateAddClass()
        {
            if (class_ClassListBox.Items.Count >= 150)
            {
                MessageBox.Show("Unable to add another class. VS Maker 2 is optimized to only allow up to 150 Trainer Classes.", "Trainer Class Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (RomFile.ClassGenderExpanded && RomFile.PrizeMoneyExpanded && RomFile.EyeContactExpanded)
            {
                AddNewTrainerClass();
            }
            else
            {
                MessageBox.Show("Trainer Class Expansion not applied. Please patch using the ROM Tool Box", "Unable to Add New Class", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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