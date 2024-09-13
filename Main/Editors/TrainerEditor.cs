﻿using Main.CustomUi.Extensions;
using Main.Forms;
using Main.Models;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace Main
{
    // TRAINER EDITOR
    public partial class Mainform : Form
    {
        public ushort[] poke1Moves;
        public ushort[] poke2Moves;
        public ushort[] poke3Moves;
        public ushort[] poke4Moves;
        public ushort[] poke5Moves;
        public ushort[] poke6Moves;
        private bool InhibitTrainerChange = false;
        private List<ComboBox> pokeAbilityComboBoxes;
        private List<ComboBox> pokeBallCapsuleComboBoxes;
        private List<ComboBox> pokeComboBoxes;
        private List<NumericUpDown> pokeDVNums;
        private List<ComboBox> pokeFormsComboBoxes;
        private List<ComboBox> pokeGenderComboBoxes;
        private List<ComboBox> pokeHeldItemComboBoxes;
        private List<PictureBox> pokeIconsPictureBoxes;
        private List<NumericUpDown> pokeLevelNums;
        private List<Button> pokeMoveButtons;
        private List<ushort[]> pokeMoves;
        private bool TrainerBattleMessagesChange;
        private bool TrainerDataChange;
        private List<ComboBox> trainerItemsComboBoxes;
        private bool TrainerPartyChange;
        private bool TrainerPropertyChange;
        private List<string> UnfilteredTrainers = [];
        private bool UnsavedTrainerEditorChanges => TrainerDataChange || TrainerPartyChange || TrainerPropertyChange || TrainerBattleMessagesChange;

        private void AddNewTrainer()
        {
            IsLoadingData = true;
            trainer_FilterBox.Text = "";
            trainer_TrainersListBox.SelectedIndex = -1;
            int newTrainerId = LoadedRom.TotalNumberOfTrainers;
            // Add new name to trainers
            MainEditorModel.TrainerNames.Add("-");
            // Add new trainer
            MainEditorModel.Trainers.Add(new Trainer(newTrainerId));

            // New TrainerProperties
            fileSystemMethods.WriteTrainerName(MainEditorModel.TrainerNames, newTrainerId, "-", LoadedRom.TrainerNamesTextNumber);
            fileSystemMethods.WriteTrainerData(new TrainerData(), newTrainerId);
            fileSystemMethods.WriteTrainerPartyData(new TrainerPartyData(), newTrainerId, false, false, RomFile.GameFamily != GameFamily.DiamondPearl);
            UnfilteredTrainers.Add(MainEditorModel.Trainers.Single(x => x.TrainerId == newTrainerId).ListName);
            trainer_TrainersListBox.Items.Add(MainEditorModel.Trainers.Single(x => x.TrainerId == newTrainerId).ListName);
            LoadedRom.TotalNumberOfTrainers++;

            // Create new Trainer Script
            var updateScripts = fileSystemMethods.UpdateTrainerScripts(LoadedRom.TotalNumberOfTrainers);
            if (!updateScripts.Success)
            {
                MessageBox.Show(updateScripts.ErrorMessage, "Couldn't update Trainer Scripts");
            }
            IsLoadingData = false;
            trainer_TrainersListBox.SelectedIndex = newTrainerId - 1;
            EditedTrainerData(true);
            EditedTrainerParty(true);
            EditedTrainerProperty(true);
        }

        private void ClearTrainerEditorData()
        {
            trainer_TrainersListBox.Items.Clear();
            trainer_TrainersListBox.SelectedIndex = -1;

            trainer_ClassListBox.Items.Clear();
            trainer_ClassListBox.SelectedIndex = -1;

            trainer_ItemComboBox1.Items.Clear();
            trainer_ItemComboBox1.SelectedIndex = -1;
            trainer_ItemComboBox1.ResetText();
            trainer_ItemComboBox2.Items.Clear();
            trainer_ItemComboBox2.SelectedIndex = -1;
            trainer_ItemComboBox2.ResetText();
            trainer_ItemComboBox3.Items.Clear();
            trainer_ItemComboBox3.SelectedIndex = -1;
            trainer_ItemComboBox3.ResetText();
            trainer_ItemComboBox4.Items.Clear();
            trainer_ItemComboBox4.SelectedIndex = -1;
            trainer_ItemComboBox4.ResetText();

            trainer_TeamSizeNum.Value = 1;
            trainer_DblBattleCheckBox.Checked = false;
            trainer_HeldItemsCheckbox.Checked = false;
            trainer_ChooseMovesCheckbox.Checked = false;
            trainer_NameTextBox.Text = "";
            trainer_AiFlags_listbox.Items.Clear();

            pokeComboBoxes?.ForEach(x => x.Items.Clear());
            pokeComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeComboBoxes?.ForEach(x => x.ResetText());
            pokeLevelNums?.ForEach(x => x.Value = 1);
            pokeDVNums?.ForEach(x => x.Value = 1);
            pokeGenderComboBoxes?.ForEach(x => x.Items.Clear());
            pokeGenderComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeGenderComboBoxes?.ForEach(x => x.ResetText());
            pokeFormsComboBoxes?.ForEach(x => x.Items.Clear());
            pokeFormsComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeFormsComboBoxes?.ForEach(x => x.ResetText());
            pokeAbilityComboBoxes?.ForEach(x => x.Items.Clear());
            pokeAbilityComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeAbilityComboBoxes?.ForEach(x => x.ResetText());
            pokeBallCapsuleComboBoxes?.ForEach(x => x.Items.Clear());
            pokeBallCapsuleComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeBallCapsuleComboBoxes?.ForEach(x => x.ResetText());
            pokeHeldItemComboBoxes?.ForEach(x => x.Items.Clear());
            pokeHeldItemComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            pokeHeldItemComboBoxes?.ForEach(x => x.ResetText());
            pokeMoves?.ForEach(x => x = null);
            MainEditorModel.SelectedTrainer = new Trainer();
        }

        private void ClearUnsavedTrainerChanges()
        {
            EditedTrainerBattleMessages(false);
            EditedTrainerData(false);
            EditedTrainerParty(false);
            EditedTrainerProperty(false);
        }

        private void EditedTrainerBattleMessages(bool hasChanges)
        {
            TrainerBattleMessagesChange = hasChanges;
            main_MainTab_TrainerTab.Text = hasChanges ? "Trainers *" : "Trainers";
            trainer_BattleMessageTab.Text = hasChanges ? "Battle Messages *" : "Battle Messages";
        }

        private void EditedTrainerData(bool hasChanges)
        {
            TrainerDataChange = hasChanges;
            main_MainTab_TrainerTab.Text = UnsavedTrainerEditorChanges ? "Trainers *" : "Trainers";
            trainer_UndoAll_Btn.Enabled = UnsavedTrainerEditorChanges;
        }

        private void EditedTrainerParty(bool hasChanges)
        {
            TrainerPartyChange = hasChanges;
            main_MainTab_TrainerTab.Text = UnsavedTrainerEditorChanges ? "Trainers *" : "Trainers";
            trainer_TrainerData_tab.Text = TrainerPartyChange || TrainerPropertyChange ? "Trainer Data *" : "Trainer Data";
            trainer_UndoParty_btn.Enabled = hasChanges;
            trainer_UndoAll_Btn.Enabled = UnsavedTrainerEditorChanges;
        }

        private void EditedTrainerProperty(bool hasChanges)
        {
            TrainerPropertyChange = hasChanges;
            main_MainTab_TrainerTab.Text = UnsavedTrainerEditorChanges ? "Trainers *" : "Trainers";
            trainer_TrainerData_tab.Text = TrainerPropertyChange || TrainerPartyChange ? "Trainer Data *" : "Trainer Data";
            trainer_UndoProperties_btn.Enabled = hasChanges;
            trainer_UndoAll_Btn.Enabled = UnsavedTrainerEditorChanges;
        }

        private void EnableDisableParty(int partySize, bool chooseItems, bool chooseMoves)
        {
            // Firstly Disable All
            pokeComboBoxes.ForEach(x => x.Enabled = false);
            pokeIconsPictureBoxes.ForEach(x => x.Enabled = false);
            pokeLevelNums.ForEach(x => x.Enabled = false);
            pokeDVNums.ForEach(x => x.Enabled = false);
            pokeBallCapsuleComboBoxes.ForEach(x => x.Enabled = false);
            pokeFormsComboBoxes.ForEach(x => x.Enabled = false);
            pokeHeldItemComboBoxes.ForEach(x => x.Enabled = false);
            pokeMoveButtons.ForEach(x => x.Enabled = false);
            pokeGenderComboBoxes.ForEach(x => x.Enabled = false);
            pokeAbilityComboBoxes.ForEach(x => x.Enabled = false);
            // Enable by Party Size
            for (int i = 0; i < partySize; i++)
            {
                var species = GetSpeciesBySpeciesId(pokeComboBoxes[i].SelectedIndex);
                pokeComboBoxes[i].Enabled = true;
                pokeIconsPictureBoxes[i].Enabled = true;
                pokeLevelNums[i].Enabled = true;
                pokeDVNums[i].Enabled = true;
                pokeBallCapsuleComboBoxes[i].Enabled = RomFile.GameFamily != GameFamily.DiamondPearl;
                pokeAbilityComboBoxes[i].Enabled = RomFile.GameFamily != GameFamily.DiamondPearl && species.HasMoreThanOneAbility;
                pokeFormsComboBoxes[i].Enabled = RomFile.GameFamily != GameFamily.DiamondPearl && Species.HasMoreThanOneForm(pokeComboBoxes[i].SelectedIndex);
                pokeHeldItemComboBoxes[i].Enabled = chooseItems;
                pokeMoveButtons[i].Enabled = chooseMoves;
                pokeGenderComboBoxes[i].Enabled = LoadedRom.IsHeartGoldSoulSilver
                    && species.HasMoreThanOneGender;
            }
        }

        private void EnableTrainerEditor()
        {
            trainer_RemoveBtn.Enabled = true;
            trainer_SpriteExportBtn.Enabled = true;
            trainer_SpriteFrameNum.Enabled = true;
            trainer_SpriteImportBtn.Enabled = true;
            trainer_Copy_Btn.Enabled = true;
            trainer_Paste_Btn.Enabled = MainEditorModel.ClipboardTrainer != null;
            trainer_Import_Btn.Enabled = true;
            trainer_Export_Btn.Enabled = true;
            trainer_ClassListBox.Enabled = true;
            trainer_ViewClassBtn.Enabled = trainer_ClassListBox.SelectedIndex > -1;
            trainer_NameTextBox.Enabled = true;
            trainer_PropertiesTabControl.Enabled = true;
            trainer_SaveBtn.Enabled = true;
        }

        #region Get

        private string GetAbilityNameByAbilityId(int abilityId)
        {
            return MainEditorModel.AbilityNames[abilityId];
        }

        private Species GetSpeciesBySpeciesId(int speciesId)
        {
            return MainEditorModel.PokemonSpecies.Find(x => x.SpeciesId == speciesId);
        }

        private TrainerClass GetTrainerClassByTrainerClassId(int trainerClassId)
        {
            return MainEditorModel.Classes.Find(x => x.TrainerClassId == trainerClassId);
        }

        #endregion Get

        private void GoToSelectedClass(int classId)
        {
            class_FilterTextBox.Text = "";
            main_MainTab.SelectedTab = main_MainTab_ClassTab;
            class_ClassListBox.SelectedIndex = classId - 2;
        }

        #region Initialize

        private void InitializeAiFlags()
        {
            foreach (var flag in AiFlags.AiFlagNames)
            {
                trainer_AiFlags_listbox.Items.Add(flag);
            }
        }

        private void InitializeBallCapsules()
        {
            foreach (var ballCapsule in BallCapsules.BallCapsuleNames)
            {
                pokeBallCapsuleComboBoxes.ForEach(x => x.Items.Add(ballCapsule));
            }
        }

        private void InitializePartyEditor()
        {
            pokeComboBoxes.ForEach(x => x.SelectedIndex = 0);
            pokeLevelNums.ForEach(x => x.Value = 1);
            pokeAbilityComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeBallCapsuleComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeDVNums.ForEach(x => x.Value = 0);
            pokeFormsComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeHeldItemComboBoxes.ForEach(x => x.SelectedIndex = -1);
            for (int i = 0; i < pokeMoves.Count; i++)
            {
                pokeMoves[i] = null;
            };
        }

        private void InitializeTrainerEditor()
        {
            trainer_UndoAll_Btn.Enabled = false;
            trainer_SaveBtn.Enabled = false;
            trainer_AddTrainerBtn.Enabled = false;
            trainer_RemoveBtn.Enabled = false;
            trainer_ImportAllBtn.Enabled = false;
            trainer_ExportAllBtn.Enabled = false;
            trainer_FilterBox.Enabled = false;
            trainer_ClearFilterBtn.Enabled = false;
            trainer_TrainersListBox.Enabled = false;
            trainer_SpriteExportBtn.Enabled = false;
            trainer_SpriteFrameNum.Enabled = false;
            trainer_SpriteImportBtn.Enabled = false;
            trainer_Copy_Btn.Enabled = false;
            trainer_Paste_Btn.Enabled = MainEditorModel.ClipboardTrainer != null;
            trainer_Import_Btn.Enabled = false;
            trainer_Export_Btn.Enabled = false;
            trainer_ClassListBox.Enabled = false;
            trainer_ViewClassBtn.Enabled = false;
            trainer_NameTextBox.Enabled = false;
            trainer_PropertiesTabControl.Enabled = false;
        }

        #endregion Initialize

        private void OpenMoveSelector(int partyIndex, int pokemonId)
        {
            // Set new array if null
            if (pokeMoves[partyIndex] == null)
            {
                pokeMoves[partyIndex] = new ushort[4];
            }
            MoveSelector = new MoveSelector(partyIndex, GetPokemonNameById(pokemonId), pokeMoves[partyIndex], MainEditorModel.MoveNames);
            MoveSelector.ShowDialog();

            // Assign selected Move Ids to pokemon's moves array
            pokeMoves[partyIndex][0] = MoveSelector.MoveId1;
            pokeMoves[partyIndex][1] = MoveSelector.MoveId2;
            pokeMoves[partyIndex][2] = MoveSelector.MoveId3;
            pokeMoves[partyIndex][3] = MoveSelector.MoveId4;

            if (!UnsavedTrainerEditorChanges && MainEditorModel.SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves != null)
            {
                bool hasChanges = MainEditorModel.SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[0] != pokeMoves[partyIndex][0]
                     || MainEditorModel.SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[1] != pokeMoves[partyIndex][1]
                     || MainEditorModel.SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[2] != pokeMoves[partyIndex][2]
                     || MainEditorModel.SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[3] != pokeMoves[partyIndex][3];

                EditedTrainerParty(hasChanges);
            }
        }

        private void poke1AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(0);
            }
        }

        private void poke1BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(0);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(0);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke1DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke1ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(0, poke1ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void poke2AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(1);
            }
        }

        private void poke2BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(1);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(1);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke2DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke2ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(1, poke2ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void poke3AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(2);
            }
        }

        private void poke3BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(2);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(2);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke3DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke3ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(2, poke3ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void poke4AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(3);
            }
        }

        private void poke4BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(3);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(3);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke4DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke4ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(3, poke4ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void poke5AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(4);
            }
        }

        private void poke5BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(4);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(4);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke5DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke5ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(4, poke5ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void poke6AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateAbilty(5);
            }
        }

        private void poke6BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetPokeComboBoxValidation(5);
            if (!IsLoadingData)
            {
                SetPokemonSpecialData(5);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                EditedTrainerParty(true);
            }
        }

        private void poke6DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6MoveBtn_Click(object sender, EventArgs e)
        {
            if (poke6ComboBox.SelectedIndex > 0)
            {
                OpenMoveSelector(5, poke6ComboBox.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select a Pokemon.", "Cannot Set Moves", MessageBoxButtons.OK);
            }
        }

        private void PopulateItemComboBox(ComboBox comboBox)
        {
            if (comboBox.Items.Count == 0)
            {
                comboBox.PopulateItems(MainEditorModel.ItemNames);
                comboBox.MakeSearchable(() => MainEditorModel.ItemNames);
                comboBox.SetSelectedIndexSafely(0, () => MainEditorModel.ItemNames);
            }
        }

        private void PopulatePartyData(TrainerParty trainerParty, int teamSize, bool chooseMoves)
        {
            IsLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty(trainerParty, teamSize, chooseMoves);
            IsLoadingData = false;
        }

        private void PopulatePokemonComboBoxes()
        {
            foreach (var comboBox in pokeComboBoxes)
            {
                // Populate the ComboBox with Pokémon names
                comboBox.PopulateItems(MainEditorModel.PokemonNames(MainEditorModel.PokemonNamesFull));

                // Make the ComboBox searchable (without TextChanged)
                comboBox.MakeSearchable(() => MainEditorModel.PokemonNames(MainEditorModel.PokemonNamesFull));

                // Set the selected index after population (e.g., index 1)
                comboBox.SetSelectedIndexSafely(0, () => MainEditorModel.PokemonNames(MainEditorModel.PokemonNamesFull));

            }
        }

        private void PopulatePokemonGenderComboBoxes()
        {
            for (int i = 0; i < pokeGenderComboBoxes.Count; i++)
            {
                pokeGenderComboBoxes[i].Items.Clear();
                foreach (var item in Gender.PokemonGenders)
                {
                    pokeGenderComboBoxes[i].Items.Add(item);
                }
            }
        }

        private void PopulateTrainerClassList(List<TrainerClass> classes)
        {
            trainer_ClassListBox.Items.Clear();
            foreach (var item in classes)
            {
                trainer_ClassListBox.Items.Add(item.ListName);
            }
        }

        private void PopulateTrainerData(Trainer trainer)
        {
            IsLoadingData = true;
            SetTrainerPartyProperties(trainer.TrainerProperties);
            SetTrainerProperties(trainer.TrainerProperties);
            SetTrainerName(trainer);
            IsLoadingData = false;
        }

        private void PopulateTrainerList(List<Trainer> trainers)
        {
            trainer_TrainersListBox.Items.Clear();
            UnfilteredTrainers = [];
            foreach (var item in trainers)
            {
                trainer_TrainersListBox.Items.Add(item.ListName);
                UnfilteredTrainers.Add(item.ListName);
            }

            trainer_AddTrainerBtn.Enabled = true;
            trainer_ImportAllBtn.Enabled = true;
            trainer_ExportAllBtn.Enabled = true;
            trainer_FilterBox.Enabled = true;
            trainer_TrainersListBox.Enabled = true;
        }

        private void ResetPokeComboBoxValidation(int partyIndex)
        {
            if (pokeComboBoxes[partyIndex].BackColor == Color.PaleVioletRed)
            {
                pokeComboBoxes[partyIndex].BackColor = Color.White;
            }
        }

        private bool SaveTrainerName(int trainerId)
        {
            var saveTrainerName = fileSystemMethods.WriteTrainerName(MainEditorModel.TrainerNames, trainerId, trainer_NameTextBox.Text, LoadedRom.TrainerNamesTextNumber);
            if (!saveTrainerName.Success)
            {
                MessageBox.Show(saveTrainerName.ErrorMessage, "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                EditedTrainerData(false);
                trainer_NameTextBox.BackColor = Color.White;
                MainEditorModel.TrainerNames[trainerId] = trainer_NameTextBox.Text;
                MainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).TrainerName = trainer_NameTextBox.Text;
                var index = trainer_TrainersListBox.FindString(UnfilteredTrainers[trainerId - 1]);
                if (index > -1)
                {
                    trainer_TrainersListBox.Items[index] = MainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).ListName;
                    trainer_TrainersListBox.SelectedIndex = index;
                }
                UnfilteredTrainers[trainerId - 1] = MainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).ListName;
            }
            return saveTrainerName.Success;
        }

        private bool SaveTrainerMessage(int messageId)
        {
            var battleMessages = MainEditorModel.BattleMessages.OrderBy(x => x.MessageId).Select(x => x.MessageText).ToList();
            var saveMessage = fileSystemMethods.WriteBattleMessage(battleMessages, messageId, trainer_MessageTextBox.Text, LoadedRom.BattleMessageTextNumber);
            if (!saveMessage.Success)
            {
                MessageBox.Show(saveMessage.ErrorMessage, "Unable to Save Battle Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int trainerId = MainEditorModel.SelectedTrainer.TrainerId;
                int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
                var message = MainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
                if (message != default)
                {
                    message.MessageText = trainer_MessageTextBox.Text;
                }
            }
            return saveMessage.Success;
        }

        private bool SaveTrainerParty(int trainerId, bool displaySuccess = false)
        {
            List<Pokemon> newPokemons = [];
            TrainerPartyPokemonData[] newPokemonDatas = new TrainerPartyPokemonData[(int)trainer_TeamSizeNum.Value];
            for (int i = 0; i < trainer_TeamSizeNum.Value; i++)
            {
                var species = GetSpeciesBySpeciesId(pokeComboBoxes[i].SelectedIndex);

                byte genderAbilityOverride = species.HasMoreThanOneGender ? (byte)(pokeGenderComboBoxes[i].SelectedIndex + (pokeAbilityComboBoxes[i].SelectedIndex << 4)) : (byte)(pokeAbilityComboBoxes[i].SelectedIndex << 4);
                var newPokemon = new Pokemon((byte)pokeDVNums[i].Value, genderAbilityOverride, (ushort)pokeLevelNums[i].Value, (ushort)pokeComboBoxes[i].SelectedIndex, (ushort)pokeFormsComboBoxes[i].SelectedIndex, (ushort?)pokeHeldItemComboBoxes[i].SelectedIndex, pokeMoves[i], (ushort?)pokeBallCapsuleComboBoxes[i].SelectedIndex);
                var newPokemonData = trainerEditorMethods.NewTrainerPartyPokemonData(newPokemon, trainer_ChooseMovesCheckbox.Checked, trainer_HeldItemsCheckbox.Checked, RomFile.GameFamily != GameFamily.DiamondPearl);
                newPokemons.Add(newPokemon);
                newPokemonDatas[i] = newPokemonData;
            }
            var trainerPartyData = new TrainerPartyData(newPokemonDatas);
            var writeFile = fileSystemMethods.WriteTrainerPartyData(trainerPartyData, trainerId, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked, RomFile.GameFamily != GameFamily.DiamondPearl);
            if (writeFile.Success)
            {
                // Add dummy pokemon data
                for (int i = 0; i < 6 - newPokemons.Count; i++)
                {
                    newPokemons.Add(new Pokemon());
                }
                var trainerParty = new TrainerParty { Pokemons = newPokemons };

                MainEditorModel.SelectedTrainer.TrainerParty = trainerParty;
                MainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).TrainerParty = trainerParty;
                LoadedRom.TrainersPartyData[trainerId - 1] = trainerPartyData;
                EditedTrainerParty(false);
                if (displaySuccess)
                {
                    MessageBox.Show("Trainer's Party updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(writeFile.ErrorMessage, "Unable to Save Trainer Party Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return writeFile.Success;
        }

        private bool SaveTrainerProperties(int trainerId, bool displaySucces = false)
        {
            List<bool> aiFlags = [];
            for (int i = 0; i < trainer_AiFlags_listbox.Items.Count; i++)
            {
                bool isChecked = trainer_AiFlags_listbox.GetItemChecked(i);
                aiFlags.Add(isChecked);
            }

            var trainerProperties = trainerEditorMethods.NewTrainerProperties(
                (byte)trainer_TeamSizeNum.Value,
                trainer_ChooseMovesCheckbox.Checked,
                trainer_HeldItemsCheckbox.Checked,
                trainer_DblBattleCheckBox.Checked,
                (byte)TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString()),
                (ushort)trainer_ItemComboBox1.SelectedIndex,
                (ushort)trainer_ItemComboBox2.SelectedIndex,
                (ushort)trainer_ItemComboBox3.SelectedIndex,
                (ushort)trainer_ItemComboBox4.SelectedIndex,
                aiFlags
                );

            var newTrainerData = trainerEditorMethods.NewTrainerData(trainerProperties);

            var writeFile = fileSystemMethods.WriteTrainerData(newTrainerData, trainerId);
            if (writeFile.Success)
            {
                MainEditorModel.SelectedTrainer.TrainerProperties = trainerProperties;
                MainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).TrainerProperties = trainerProperties;
                LoadedRom.TrainersData[trainerId - 1] = newTrainerData;
                EditedTrainerProperty(false);
                if (displaySucces)
                {
                    MessageBox.Show("Trainer Properties updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(writeFile.ErrorMessage, "Unable to Save Trainer Properties", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return writeFile.Success;
        }

        private void SetPokemonForms(int pokemonId, int partyIndex)
        {
            pokeFormsComboBoxes[partyIndex].Items.Clear();
            switch (pokemonId)
            {
                case Pokemon.Pokedex.PichuId:
                    Species.AltForms.FormNames.Pichu.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.UnownId:
                    Species.AltForms.FormNames.Unown.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.CastformId:
                    Species.AltForms.FormNames.Castform.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.DeoxysId:
                    Species.AltForms.FormNames.Deoxys.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.BurmyId:
                case Pokemon.Pokedex.WormadamId:
                    Species.AltForms.FormNames.BurmyWormadam.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.ShellosId:
                case Pokemon.Pokedex.GastrodonId:
                    Species.AltForms.FormNames.ShellosGastrodon.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.RotomId:
                    Species.AltForms.FormNames.Rotom.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.GiratinaId:
                    Species.AltForms.FormNames.GiratinaForms.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.ShayminId:
                    Species.AltForms.FormNames.ShayminForms.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                default:
                    pokeFormsComboBoxes[partyIndex].Items.Add(Species.AltForms.FormNames.Default); break;
            }
        }

        private void SetPokemonSpecialData(int partyIndex)
        {
            if (pokeComboBoxes[partyIndex].SelectedIndex > -1)
            {
                ushort speciesId = Species.GetSpecialSpecies((ushort)pokeComboBoxes[partyIndex].SelectedIndex, 0);
                var species = GetSpeciesBySpeciesId(speciesId);
                pokeAbilityComboBoxes[partyIndex].Enabled = false;
                pokeAbilityComboBoxes[partyIndex].Items.Clear();
                pokeAbilityComboBoxes[partyIndex].SelectedIndex = -1;

                if (LoadedRom.IsHeartGoldSoulSilver)
                {
                    pokeGenderComboBoxes[partyIndex].SelectedIndex = species.GenderRatio switch
                    {
                        Species.Constants.GenderRatioGenderless => 0,
                        Species.Constants.GenderRatioMale => 1,
                        Species.Constants.GenderRatioFemale => 2,
                        _ => 0,
                    };
                    pokeGenderComboBoxes[partyIndex].Enabled = species.HasMoreThanOneGender;
                }
                pokeAbilityComboBoxes[partyIndex].Items.Add("-");

                if (species.Ability1 > 0)
                {
                    pokeAbilityComboBoxes[partyIndex].Items.Add(GetAbilityNameByAbilityId(species.Ability1));
                }
                if (species.Ability2 > 0)
                {
                    pokeAbilityComboBoxes[partyIndex].Items.Add(GetAbilityNameByAbilityId(species.Ability2));
                }
                pokeAbilityComboBoxes[partyIndex].SelectedIndex = 0;
                pokeAbilityComboBoxes[partyIndex].Enabled = species.HasMoreThanOneAbility;

                if (RomFile.GameFamily != GameFamily.DiamondPearl)
                {
                    SetPokemonForms(pokeComboBoxes[partyIndex].SelectedIndex, partyIndex);
                    pokeFormsComboBoxes[partyIndex].SelectedIndex = 0;
                    pokeFormsComboBoxes[partyIndex].Enabled = Species.HasMoreThanOneForm(pokeComboBoxes[partyIndex].SelectedIndex);
                }
            }
        }

        private void SetTrainerName(Trainer trainer)
        {
            trainer_NameTextBox.Text = trainer.TrainerName;
        }

        private int GetIndex(ushort? index)
        {
            if (!index.HasValue)
            {
                return 0;
            }
            else if (index.Value == 0xFFFF)
            {
                return 0;
            }
            else
            {
                return index.Value;
            }
        }

        private void SetTrainerParty(TrainerParty trainerParty, int teamSize, bool chooseMoves)
        {
            for (int i = 0; i < teamSize; i++)
            {
                var species = GetSpeciesBySpeciesId(trainerParty.Pokemons[i].SpeciesId);
                pokeComboBoxes[i].SetSelectedIndexSafely(trainerParty.Pokemons[i].PokemonId, () => MainEditorModel.PokemonNames(MainEditorModel.PokemonNamesFull));
                pokeLevelNums[i].Value = trainerParty.Pokemons[i].Level;
                pokeDVNums[i].Value = trainerParty.Pokemons[i].DifficultyValue;
                pokeAbilityComboBoxes[i].Items.Clear();
                pokeFormsComboBoxes[i].Items.Clear();
                pokeHeldItemComboBoxes[i].SetSelectedIndexSafely(GetIndex(trainerParty.Pokemons[i].HeldItemId), () => MainEditorModel.ItemNames);
                pokeBallCapsuleComboBoxes[i].SelectedIndex = RomFile.GameFamily != GameFamily.DiamondPearl ? GetIndex(trainerParty.Pokemons[i].BallCapsuleId) : -1;
                if (chooseMoves)
                {
                    pokeMoves[i] = new ushort[4];
                    pokeMoves[i][0] = trainerParty.Pokemons[i].Moves[0];
                    pokeMoves[i][1] = trainerParty.Pokemons[i].Moves[1];
                    pokeMoves[i][2] = trainerParty.Pokemons[i].Moves[2];
                    pokeMoves[i][3] = trainerParty.Pokemons[i].Moves[3];
                }

                if (LoadedRom.IsHeartGoldSoulSilver)
                {
                    switch (species.GenderRatio)
                    {
                        case Species.Constants.GenderRatioGenderless:
                            pokeGenderComboBoxes[i].SelectedIndex = 0; break;
                        case Species.Constants.GenderRatioMale:
                            pokeGenderComboBoxes[i].SelectedIndex = 1; break;
                        case Species.Constants.GenderRatioFemale:
                            pokeGenderComboBoxes[i].SelectedIndex = 2; break;
                        default:
                            switch (trainerParty.Pokemons[i].GenderOverride)
                            {
                                case GenderOverride.None:
                                    pokeGenderComboBoxes[i].SelectedIndex = 0; break;
                                case GenderOverride.IsMale:
                                    pokeGenderComboBoxes[i].SelectedIndex = 1; break;
                                case GenderOverride.IsFemale:
                                    pokeGenderComboBoxes[i].SelectedIndex = 2; break;
                            }
                            break;
                    }
                }

                if (RomFile.GameFamily != GameFamily.DiamondPearl)
                {
                    SetPokemonForms(trainerParty.Pokemons[i].PokemonId, i);
                    pokeFormsComboBoxes[i].SelectedIndex = trainerParty.Pokemons[i].FormId;
                }

                if (species.Ability1 > 0 && species.Ability2 > 0)
                {
                    pokeAbilityComboBoxes[i].Items.Add("-");
                    pokeAbilityComboBoxes[i].Items.Add(GetAbilityNameByAbilityId(species.Ability1));
                    pokeAbilityComboBoxes[i].Items.Add(GetAbilityNameByAbilityId(species.Ability2));
                    pokeAbilityComboBoxes[i].SelectedIndex = trainerParty.Pokemons[i].AbilityOverride
                               switch
                    {
                        AbilityOverride.None => 0,
                        AbilityOverride.Ability1 => 1,
                        AbilityOverride.Ability2 => 2,
                        _ => 0
                    };
                }
                else
                {
                    pokeAbilityComboBoxes[i].Items.Add(GetAbilityNameByAbilityId(species.Ability1));
                    pokeAbilityComboBoxes[i].SelectedIndex = 0;
                }
            }
        }

        private void SetTrainerPartyProperties(TrainerProperty trainerProperties)
        {
            trainer_TeamSizeNum.Maximum = trainerProperties.DoubleBattle ? 3 : 6;
            trainer_TeamSizeNum.Value = trainerProperties.TeamSize == 0 ? 1 : trainerProperties.TeamSize;

            trainer_DblBattleCheckBox.Checked = trainerProperties.DoubleBattle;
            trainer_HeldItemsCheckbox.Checked = trainerProperties.ChooseItems;
            trainer_ChooseMovesCheckbox.Checked = trainerProperties.ChooseMoves;
        }

        private void SetTrainerProperties(TrainerProperty trainerProperties)
        {
            for (int i = 0; i < 4; i++)
            {
                trainerItemsComboBoxes[i].SelectedIndex = trainerProperties.Items[i];
            }
            var trainerClass = GetTrainerClassByTrainerClassId(trainerProperties.TrainerClassId);
            trainer_ClassListBox.SelectedIndex = trainer_ClassListBox.Items.IndexOf(trainerClass.ListName);

            for (int i = 0; i < trainerProperties.AIFlags.Count; i++)
            {
                trainer_AiFlags_listbox.SetItemChecked(i, trainerProperties.AIFlags[i]);
            }
            if (trainerProperties.TeamSize < 1)
            {
                MessageBox.Show("This trainer does not currently have any Pokemon set.\nYou must set at least one Pokemon before use in-game", "Set a Party Pokemon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                EditedTrainerParty(true);
                EditedTrainerProperty(true);
            }
        }

        private void SetupPartyEditorFields()
        {
            pokeComboBoxes =
            [
                poke1ComboBox,
                poke2ComboBox,
                poke3ComboBox,
                poke4ComboBox,
                poke5ComboBox,
                poke6ComboBox
            ];

            pokeIconsPictureBoxes =
            [
                poke1IconPicBox,
                poke2IconPicBox,
                poke3IconPicBox,
                poke4IconPicBox,
                poke5IconPicBox,
                poke6IconPicBox,
            ];

            pokeLevelNums =
            [
                poke1LevelNum,
                poke2LevelNum,
                poke3LevelNum,
                poke4LevelNum,
                poke5LevelNum,
                poke6LevelNum,
            ];

            pokeDVNums =
            [
                poke1DVNum,
                poke2DVNum,
                poke3DVNum,
                poke4DVNum,
                poke5DVNum,
                poke6DVNum,
            ];

            pokeGenderComboBoxes =
                [
                poke1GenderComboBox,
                poke2GenderComboBox,
                poke3GenderComboBox,
                poke4GenderComboBox,
                poke5GenderComboBox,
                poke6GenderComboBox,
            ];

            pokeAbilityComboBoxes =
                [
                poke1AbilityComboBox,
                poke2AbilityComboBox,
                poke3AbilityComboBox,
                poke4AbilityComboBox,
                poke5AbilityComboBox,
                poke6AbilityComboBox,
                ];

            pokeBallCapsuleComboBoxes =
                [
                poke1BallCapsuleComboBox,
                poke2BallCapsuleComboBox,
                poke3BallCapsuleComboBox,
                poke4BallCapsuleComboBox,
                poke5BallCapsuleComboBox,
                poke6BallCapsuleComboBox,
                ];

            pokeFormsComboBoxes =
                [
                poke1FormComboBox,
                poke2FormComboBox,
                poke3FormComboBox,
                poke4FormComboBox,
                poke5FormComboBox,
                poke6FormComboBox,
                ];

            pokeHeldItemComboBoxes =
                [
                poke1HeldItemComboBox,
                poke2HeldItemComboBox,
                poke3HeldItemComboBox,
                poke4HeldItemComboBox,
                poke5HeldItemComboBox,
                poke6HeldItemComboBox,
                ];

            pokeMoveButtons =
                [
                poke1MoveBtn,
                poke2MoveBtn,
                poke3MoveBtn,
                poke4MoveBtn,
                poke5MoveBtn,
                poke6MoveBtn,
                ];

            pokeMoves =
                [
                poke1Moves,
                poke2Moves,
                poke3Moves,
                poke4Moves,
                poke5Moves,
                poke6Moves,
                ];
        }

        private void SetupTrainerEditor()
        {
            IsLoadingData = true;
            if (trainerItemsComboBoxes == null || trainerItemsComboBoxes?.Count == 0)
            {
                trainerItemsComboBoxes = [trainer_ItemComboBox1, trainer_ItemComboBox2, trainer_ItemComboBox3, trainer_ItemComboBox4];
            }
            trainerItemsComboBoxes?.ForEach(PopulateItemComboBox);

            if (trainer_TrainersListBox.Items.Count == 0)
            {
                trainer_TrainersListBox.SelectedIndex = -1;
                PopulateTrainerList(MainEditorModel.Trainers);
            }
            if (trainer_ClassListBox.Items.Count == 0)
            {
                trainer_ClassListBox.SelectedIndex = -1;
                PopulateTrainerClassList(MainEditorModel.Classes);
            }
            if (trainer_AiFlags_listbox.Items.Count == 0)
            {
                InitializeAiFlags();
            }
            if (poke1ComboBox.Items.Count == 0)
            {
                SetupPartyEditorFields();
                pokeComboBoxes?.ForEach(x => x.SelectedIndex = -1);
                PopulatePokemonComboBoxes();
            }
            if (poke1GenderComboBox.Items.Count == 0 && LoadedRom.IsHeartGoldSoulSilver)
            {
                PopulatePokemonGenderComboBoxes();
                pokeGenderComboBoxes?.ForEach(x => x.Visible = true);
                pokeGenderComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            }
            else
            {
                pokeGenderComboBoxes?.ForEach(x => x.Visible = false);
                pokeGenderComboBoxes?.ForEach(x => x.Enabled = false);
            }
            if (poke1HeldItemComboBox.Items.Count == 0)
            {
                pokeHeldItemComboBoxes?.ForEach(PopulateItemComboBox);
            }
            if (poke1BallCapsuleComboBox.Items.Count == 0 && RomFile.GameFamily != GameFamily.DiamondPearl)
            {
                InitializeBallCapsules();
            }
            trainer_TrainersListBox.Enabled = true;
            IsLoadingData = false;
        }

        private void trainer_AddTrainerBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (UnsavedTrainerEditorChanges && ConfirmUnsavedChanges())
                {
                    ClearUnsavedTrainerChanges();
                    AddNewTrainer();
                }
                else if (!UnsavedTrainerEditorChanges)
                {
                    AddNewTrainer();
                }
            }
        }

        private void trainer_AiFlags_listbox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ChooseMovesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void trainer_ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
            trainer_ViewClassBtn.Enabled = trainer_ClassListBox.SelectedIndex > -1;
        }

        private void trainer_ClearFilterBtn_Click(object sender, EventArgs e)
        {
            trainer_FilterBox.Text = "";
        }

        private void trainer_DblBattleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            trainer_TeamSizeNum.Maximum = trainer_DblBattleCheckBox.Checked ? 3 : 6;

            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void trainer_FilterBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (string.IsNullOrEmpty(trainer_FilterBox.Text))
                {
                    PopulateTrainerList(MainEditorModel.Trainers);
                    trainer_ClearFilterBtn.Enabled = false;
                }
                else
                {
                    FilterListBox(trainer_TrainersListBox, trainer_FilterBox.Text, UnfilteredTrainers);
                    trainer_ClearFilterBtn.Enabled = true;
                }
            }
        }

        private void trainer_HeldItemsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void trainer_ItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (trainer_NameTextBox.BackColor == Color.PaleVioletRed)
            {
                trainer_NameTextBox.BackColor = Color.White;
            }
            if (!IsLoadingData)
            {
                EditedTrainerData(true);
            }
        }

        private void trainer_RemoveBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (MainEditorModel.SelectedTrainer.TrainerId <= LoadedRom.VanillaTotalTrainers - 1)
                {
                    MessageBox.Show("This is one of the game's core Trainers.\nYou cannot remove this file as it will cause issues.", "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    var removeTrainer = fileSystemMethods.RemoveTrainer(MainEditorModel.SelectedTrainer.TrainerId);
                    if (removeTrainer.Success)
                    {
                        MessageBox.Show("Trainer removed", "Success");
                    }
                    else
                    {
                        MessageBox.Show(removeTrainer.ErrorMessage, "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void trainer_SaveBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            if (ValidateTrainerName() && ValidatePokemon() && ValidatePokemonMoves() && SaveTrainerName(MainEditorModel.SelectedTrainer.TrainerId) && SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId) && SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId))
            {
                MessageBox.Show("Trainer Data updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            IsLoadingData = false;
        }

        private void trainer_SaveParty_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemon() && ValidatePokemonMoves())
            {
                if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked
                    && MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId, true);
                }
            }
        }

        private void trainer_SaveProperties_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemonMoves())
            {
                if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked
                    && MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(MainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerProperties(MainEditorModel.SelectedTrainer.TrainerId, true);
                }
            }
        }

        private void trainer_TeamSizeNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (trainer_TeamSizeNum.Minimum == 0)
                {
                    trainer_TeamSizeNum.Minimum = 1;
                }
                EditedTrainerProperty(true);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void PopualteTrainerUsages(List<TrainerUsage> trainerUsage)
        {
            trainer_ScriptUsage.Items.Clear();
            trainer_EventUsage.Items.Clear();
            foreach (var item in trainerUsage.Where(x => x.TrainerUsageType == TrainerUsageType.Script || x.TrainerUsageType == TrainerUsageType.Function).OrderBy(x => x.FileId))
            {
                trainer_ScriptUsage.Items.Add(item.ListItemName);
            }
        }

        private void trainer_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData && trainer_TrainersListBox.SelectedIndex > -1)
            {
                string selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();

                if (selectedTrainer != MainEditorModel.SelectedTrainer.ListName)
                {
                    if (UnsavedTrainerEditorChanges && !InhibitTrainerChange)
                    {
                        if (ConfirmUnsavedChanges())
                        {
                            ClearUnsavedTrainerChanges();
                        }
                        else
                        {
                            InhibitTrainerChange = true;
                            trainer_TrainersListBox.SelectedIndex = trainer_TrainersListBox.Items.IndexOf(MainEditorModel.SelectedTrainer.ListName);
                        }
                    }

                    if (!InhibitTrainerChange)
                    {
                        selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();
                        MainEditorModel.SelectedTrainer = new Trainer(trainerEditorMethods.GetTrainer(MainEditorModel.Trainers, Trainer.ListNameToTrainerId(selectedTrainer)));

                        if (MainEditorModel.SelectedTrainer.TrainerId > 0)
                        {
                            PopulateTrainerData(MainEditorModel.SelectedTrainer);
                            PopulatePartyData(MainEditorModel.SelectedTrainer.TrainerParty, MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
                            PopulateTrainerBattleMessageTriggers(MainEditorModel.SelectedTrainer);
                            PopualteTrainerUsages(MainEditorModel.SelectedTrainer.TrainerUsages);
                            EnableTrainerEditor();
                            EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
                        }
                    }
                    else
                    {
                        InhibitTrainerChange = false;
                    }
                }
            }
        }

        private void trainer_MessageTriggerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                int trainerId = MainEditorModel.SelectedTrainer.TrainerId;
                int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
                var message = MainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
                if (message != default)
                {
                    trainer_MessageTextBox.Text = message.MessageText;
                    trainerEditor_SaveMessage.Enabled = true;
                }
                else
                {
                    trainer_MessageTextBox.Text = "";
                    trainerEditor_SaveMessage.Enabled = false;
                }
            }
        }

        private void trainer_MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateTextPreview(trainer_MessageTextBox.Text, trainer_MessagePreviewText, trainer_MessageUpBtn, trainer_MessageDownBtn);
            }
        }

        private void trainer_MessageUpBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewBack(trainer_MessageDownBtn, trainer_MessageUpBtn, trainer_MessagePreviewText);
        }

        private void trainer_MessageDownBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewNext(trainer_MessageDownBtn, trainer_MessageUpBtn, trainer_MessagePreviewText);
        }

        private void PopulateTrainerBattleMessageTriggers(Trainer trainer)
        {
            IsLoadingData = true;
            trainer_MessageTriggerListBox.Items.Clear();
            trainer_MessagePreviewText.Text = "";
            trainer_MessageTextBox.Text = "";
            List<string> messageTriggers = [];
            foreach (var item in MainEditorModel.BattleMessages.Where(x => x.TrainerId == trainer.TrainerId))
            {
                messageTriggers.Add(MessageTrigger.MessageTriggers.Find(x => x.MessageTriggerId == item.MessageTriggerId).ListName);
            }
            messageTriggers.Sort();
            messageTriggers.ForEach(x => trainer_MessageTriggerListBox.Items.Add(x));
            trainer_MessageUpBtn.Enabled = false;
            trainer_MessageUpBtn.Visible = false;
            trainer_MessageDownBtn.Enabled = false;
            trainer_MessageDownBtn.Visible = false;
            IsLoadingData = false;
        }

        private void trainer_UndoAll_Btn_Click(object sender, EventArgs e)
        {
            UndoTrainerChanges();
        }

        private void trainer_UndoParty_btn_Click(object sender, EventArgs e)
        {
            UndoTrainerPartyChanges();
        }

        private void trainer_UndoProperties_Click(object sender, EventArgs e)
        {
            UndoTrainerPropertyChanges();
        }

        private void trainer_ViewClassBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData && trainer_ClassListBox.SelectedIndex > -1)
            {
                if (UnsavedClassChanges && ConfirmUnsavedChanges())
                {
                    ClearUnsavedClassChanges();
                    int classId = TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString());
                    GoToSelectedClass(classId);
                }
                else if (!UnsavedClassChanges)
                {
                    int classId = TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString());
                    GoToSelectedClass(classId);
                }
            }
        }

        private void UndoTrainerChanges()
        {
            IsLoadingData = true;
            SetTrainerName(MainEditorModel.SelectedTrainer);
            SetTrainerPartyProperties(MainEditorModel.SelectedTrainer.TrainerProperties);
            SetTrainerProperties(MainEditorModel.SelectedTrainer.TrainerProperties);
            InitializePartyEditor();
            SetTrainerParty(MainEditorModel.SelectedTrainer.TrainerParty, MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
            EditedTrainerData(false);
            EditedTrainerProperty(false);
            EditedTrainerParty(false);
            IsLoadingData = false;
        }

        private void UndoTrainerPartyChanges()
        {
            IsLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty(MainEditorModel.SelectedTrainer.TrainerParty, MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
            EditedTrainerParty(false);
            IsLoadingData = false;
        }

        private void UndoTrainerPropertyChanges()
        {
            IsLoadingData = true;
            SetTrainerPartyProperties(MainEditorModel.SelectedTrainer.TrainerProperties);
            SetTrainerProperties(MainEditorModel.SelectedTrainer.TrainerProperties);
            EditedTrainerProperty(false);
            IsLoadingData = false;
        }

        private void UpdateAbilty(int index)
        {
            EditedTrainerParty(true);
        }

        private bool ValidatePokemon()
        {
            bool valid = true;
            for (int i = 0; i < trainer_TeamSizeNum.Value; i++)
            {
                if (pokeComboBoxes[i].SelectedIndex <= 0)
                {
                    pokeComboBoxes[i].BackColor = Color.PaleVioletRed;
                    valid = false;
                }
            }
            if (!valid)
            {
                MessageBox.Show("You must select a Pokemon", "Unable to Save Trainer Party", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return valid;
        }

        private bool ValidatePokemonMoves()
        {
            if (trainer_ChooseMovesCheckbox.Checked)
            {
                int partyCount = 0;
                foreach (var moveArray in pokeMoves)
                {
                    if (partyCount == trainer_TeamSizeNum.Value)
                    {
                        break;
                    }
                    if (moveArray == null)
                    {
                        MessageBox.Show("You have not set moves for all Party Pokemon!\nClick the Moves button next to each Pokemon to set moves", "Unable to Save Trainer Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }

                    int movesSelected = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        if (moveArray[i] > 0)
                        {
                            movesSelected++;
                        }
                    }

                    if (movesSelected == 0)
                    {
                        MessageBox.Show("You have not set moves for all Party Pokemon!\nClick the Moves button next to each Pokemon to set moves", "Unable to Save Trainer Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    partyCount++;
                }
            }
            return true;
        }

        private bool ValidateTrainerName()
        {
            if (string.IsNullOrEmpty(trainer_NameTextBox.Text))
            {
                trainer_NameTextBox.BackColor = Color.PaleVioletRed;
                MessageBox.Show("You must enter a name for this Trainer.", "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (trainer_NameTextBox.Text.Length > RomFile.TrainerNameMaxLength && !RomFile.TrainerNameExpansion)
            {
                trainer_NameTextBox.BackColor = Color.PaleVioletRed;
                MessageBox.Show($"Trainer name cannot be longer than {RomFile.TrainerNameMaxLength} characters.\n\nYou can expand this by applying the Trainer Names Expansion patch.", "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var confirmPatch = MessageBox.Show("Do you wish to apply this patch now?", "Apply Trainer Name Expansion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirmPatch == DialogResult.Yes)
                {
                    return RomPatches.ExpandTrainerNames(LoadedRom);
                }
                else
                {
                    return false;
                }
            }
            else if (trainer_NameTextBox.Text.Length > RomFile.TrainerNameMaxLength && RomFile.TrainerNameExpansion)
            {
                trainer_NameTextBox.BackColor = Color.PaleVioletRed;
                MessageBox.Show($"Trainer name cannot be longer than {RomFile.TrainerNameMaxLength} characters.", "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}