﻿using Main.Forms;
using System.Xml.Linq;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace Main
{
    // TRAINER EDITOR
    public partial class Mainform : Form
    {
        public ComboBox[] poke1Moves = new ComboBox[4];
        public ComboBox[] poke2Moves = new ComboBox[4];
        public ComboBox[] poke3Moves = new ComboBox[4];
        public ComboBox[] poke4Moves = new ComboBox[4];
        public ComboBox[] poke5Moves = new ComboBox[4];
        public ComboBox[] poke6Moves = new ComboBox[4];

        public NumericUpDown[] poke1PPs = new NumericUpDown[4];
        public NumericUpDown[] poke2PPs = new NumericUpDown[4];
        public NumericUpDown[] poke3PPs = new NumericUpDown[4];
        public NumericUpDown[] poke4PPs = new NumericUpDown[4];
        public NumericUpDown[] poke5PPs = new NumericUpDown[4];
        public NumericUpDown[] poke6PPs = new NumericUpDown[4];

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
        private List<ComboBox[]> pokeMovesComboBoxes;
        private List<NumericUpDown[]> pokePpNumberBoxes;
        private bool TrainerBattleMessagesChange;
        private bool TrainerDataChange;
        private List<ComboBox> trainerItemsComboBoxes;
        private bool TrainerPartyChange;
        private bool TrainerPropertyChange;
        private List<string> UnfilteredTrainers = [];
        private bool UnsavedTrainerEditorChanges => TrainerDataChange || TrainerPartyChange || TrainerPropertyChange || TrainerBattleMessagesChange;

        private void trainer_Copy_Btn_Click(object sender, EventArgs e)
        {
            mainEditorModel.ClipboardTrainer = new Trainer(mainEditorModel.SelectedTrainer);
            mainEditorModel.ClipboardTrainerProperties = new TrainerProperty(mainEditorModel.SelectedTrainer.TrainerProperties);
            mainEditorModel.ClipboardTrainerParty = new TrainerParty(mainEditorModel.SelectedTrainer.TrainerParty);
            trainer_Paste_Btn.Enabled = true;
            trainer_PastePropeties_btn.Enabled = true;
            trainer_PasteParty_btn.Enabled = true;
        }

        private void trainer_CopyParty_btn_Click(object sender, EventArgs e)
        {
            mainEditorModel.ClipboardTrainerParty = new TrainerParty(mainEditorModel.SelectedTrainer.TrainerParty)
            {
                ChooseItems = mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems,
                ChooseMoves = mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves,
                DoubleBattle = mainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle,
                TeamSize = mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize,
            };
            trainer_PasteParty_btn.Enabled = true;
        }

        private void trainer_CopyProperties_btn_Click(object sender, EventArgs e)
        {
            mainEditorModel.ClipboardTrainerProperties = new TrainerProperty(mainEditorModel.SelectedTrainer.TrainerProperties);
            trainer_PastePropeties_btn.Enabled = true;
        }

        private void trainer_InsertE_btn_Click(object sender, EventArgs e) => AppendBattleMessage(trainer_MessageTextBox, "é");

        private void trainer_InsertF_Btn_Click(object sender, EventArgs e) => AppendBattleMessage(trainer_MessageTextBox, "\\f");

        private void trainer_InsertN_btn_Click(object sender, EventArgs e) => AppendBattleMessage(trainer_MessageTextBox, "\\n");

        private void trainer_InsertR_btn_Click(object sender, EventArgs e) => AppendBattleMessage(trainer_MessageTextBox, "\\r");

        private void trainer_Paste_Btn_Click(object sender, EventArgs e)
        {
            int selectedTrainerId = mainEditorModel.SelectedTrainer.TrainerId;
            var pasteTrainer = new Trainer(selectedTrainerId, mainEditorModel.ClipboardTrainer);

            PopulateTrainerData(pasteTrainer);
            PopulatePartyData(pasteTrainer.TrainerParty, pasteTrainer.TrainerProperties.TeamSize, pasteTrainer.TrainerProperties.ChooseMoves);
            PopulateTrainerBattleMessageTriggers(pasteTrainer);
            EnableTrainerEditor();
            EnableDisableParty();
            EditedTrainerData(true);
        }

        private void trainer_PasteParty_btn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;
            if (mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize != mainEditorModel.ClipboardTrainerParty.TeamSize
                                || mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != mainEditorModel.ClipboardTrainerParty.ChooseItems
                || mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != mainEditorModel.ClipboardTrainerParty.ChooseMoves
                || mainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle != mainEditorModel.ClipboardTrainerParty.DoubleBattle
                )
            {
                var pasteProperties = new TrainerProperty(mainEditorModel.SelectedTrainer.TrainerProperties, mainEditorModel.ClipboardTrainerParty.DoubleBattle,
                   mainEditorModel.ClipboardTrainerParty.TeamSize,
                    mainEditorModel.ClipboardTrainerParty.ChooseMoves,
                   mainEditorModel.ClipboardTrainerParty.ChooseItems);
                SetTrainerPartyProperties(pasteProperties);
                EditedTrainerProperty(true);
            }
            else
            {
            }
            isLoadingData = false;
            PopulatePartyData(mainEditorModel.ClipboardTrainerParty, mainEditorModel.ClipboardTrainerParty.TeamSize, mainEditorModel.ClipboardTrainerParty.ChooseMoves);
            EditedTrainerParty(true);
            EnableDisableParty();
        }

        private void trainer_PastePropeties_btn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;
            if (mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize != mainEditorModel.ClipboardTrainerProperties.TeamSize
                                || mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != mainEditorModel.ClipboardTrainerProperties.ChooseItems
                || mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != mainEditorModel.ClipboardTrainerProperties.ChooseMoves
                || mainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle != mainEditorModel.ClipboardTrainerProperties.DoubleBattle
                || mainEditorModel.SelectedTrainer.TrainerProperties.Items != mainEditorModel.ClipboardTrainerProperties.Items
                || mainEditorModel.SelectedTrainer.TrainerProperties.AIFlags != mainEditorModel.ClipboardTrainerProperties.AIFlags
                )
            {
                var pasteProperties = new TrainerProperty(mainEditorModel.ClipboardTrainerProperties);
                SetTrainerProperties(pasteProperties);
                EditedTrainerProperty(true);
            }

            isLoadingData = false;
            PopulatePartyData(mainEditorModel.SelectedTrainer.TrainerParty, mainEditorModel.ClipboardTrainerProperties.TeamSize, mainEditorModel.ClipboardTrainerProperties.ChooseMoves);
            EditedTrainerParty(true);
            EnableDisableParty();
        }

        private void trainerEditor_SaveMessage_Click(object sender, EventArgs e)
        {
            int trainerId = mainEditorModel.SelectedTrainer.TrainerId;
            int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
            var message = mainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
            if (SaveTrainerMessage(message.MessageId))
            {
                if (battleMessage_MessageTableDataGrid.Rows.Count > 0)
                {
                    var row = battleMessage_MessageTableDataGrid.Rows.Cast<DataGridViewRow>()
                        .SingleOrDefault(x => x.Cells[1].Value.ToString() == mainEditorModel.SelectedTrainer.ListName
                        && x.Cells[2].Value.ToString() == trainer_MessageTriggerListBox!.SelectedItem.ToString());

                    if (row != default)
                    {
                        row.Cells[3].Value = message.MessageText;
                    }
                }
            }
        }

        private void AddNewTrainer()
        {
            isLoadingData = true;
            trainer_FilterBox.Text = "";
            trainer_TrainersListBox.SelectedIndex = -1;
            int newTrainerId = RomFile.TotalNumberOfTrainers;
            mainEditorModel.TrainerNames.Add("-");
            mainEditorModel.Trainers.Add(new Trainer(newTrainerId));
            UnfilteredTrainers.Add(mainEditorModel.Trainers[newTrainerId].ListName);

            // New TrainerProperties
            fileSystemMethods.WriteTrainerName(mainEditorModel.TrainerNames, newTrainerId, "-", RomFile.TrainerNamesTextNumber);
            fileSystemMethods.WriteTrainerData(new TrainerData(), newTrainerId);
            fileSystemMethods.WriteTrainerPartyData(new TrainerPartyData(), newTrainerId, [false, false, false], RomFile.IsNotDiamondPearl);

            // Create new Trainer Script
            var updateScripts = fileSystemMethods.UpdateTrainerScripts(RomFile.TotalNumberOfTrainers);
            if (!updateScripts.Success)
            {
                MessageBox.Show(updateScripts.ErrorMessage, "Couldn't update Trainer Scripts");
            }

            RefreshTrainerData();
            trainer_TrainersListBox.Items.Add(mainEditorModel.Trainers[newTrainerId].ListName);
            isLoadingData = false;
            trainer_TrainersListBox.SelectedIndex = newTrainerId;
            EditedTrainerData(true);
            EditedTrainerParty(true);
            EditedTrainerProperty(true);
        }

        private void RefreshTrainerData()
        {
            RomFile.TrainerNames = romFileMethods.GetTrainerNames();
            RomFile.TrainersData = romFileMethods.GetTrainersData();
            RomFile.TrainersPartyData = romFileMethods.GetTrainersPartyData();
        }

        private void ClearTrainerEditorData()
        {
            ClearListBox(trainer_TrainersListBox);
            ClearListBox(trainer_ClassListBox);
            ClearComboBox(trainer_ItemComboBox1);
            ClearComboBox(trainer_ItemComboBox2);
            ClearComboBox(trainer_ItemComboBox3);
            ClearComboBox(trainer_ItemComboBox4);

            trainer_TeamSizeNum.Value = 1;
            trainer_NameTextBox.Text = "";

            trainer_AiFlags_listbox.Items.Clear();
            trainer_PropertyFlags.Items.Clear();

            ClearComboBoxes(pokeComboBoxes);
            ClearComboBoxes(pokeGenderComboBoxes);
            ClearComboBoxes(pokeFormsComboBoxes);
            ClearComboBoxes(pokeAbilityComboBoxes);
            ClearComboBoxes(pokeBallCapsuleComboBoxes);
            ClearComboBoxes(pokeHeldItemComboBoxes);
            ClearMovesComboBoxes(pokeMovesComboBoxes);
            ClearPpNumberBoxes(pokePpNumberBoxes);

            mainEditorModel.SelectedTrainer = new Trainer();
        }

        private static void ClearListBox(ListBox listBox)
        {
            listBox.Items.Clear();
            listBox.SelectedIndex = -1;
        }

        private static void ClearComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();
            comboBox.SelectedIndex = -1;
            comboBox.ResetText();
        }

        private static void ClearComboBoxes(List<ComboBox> comboBoxes)
        {
            comboBoxes?.ForEach(ClearComboBox);
        }

        private static void ClearMovesComboBoxes(List<ComboBox[]> moveComboBoxes)
        {
            moveComboBoxes?.ForEach(items => items.ToList().ForEach(comboBox => comboBox.SelectedIndex = -1));
        }

        private static void ClearPpNumberBoxes(List<NumericUpDown[]> ppNumberBoxes)
        {
            ppNumberBoxes?.ForEach(items => items.ToList().ForEach(numBox => numBox.Value = 0));
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
            trainer_SaveParty_btn.Enabled = hasChanges;
            trainer_UndoParty_btn.Enabled = hasChanges;
            trainer_UndoAll_Btn.Enabled = UnsavedTrainerEditorChanges;
        }

        private void EditedTrainerProperty(bool hasChanges)
        {
            TrainerPropertyChange = hasChanges;
            main_MainTab_TrainerTab.Text = UnsavedTrainerEditorChanges ? "Trainers *" : "Trainers";
            trainer_TrainerData_tab.Text = TrainerPropertyChange || TrainerPartyChange ? "Trainer Data *" : "Trainer Data";
            trainer_SaveProperties_btn.Enabled = hasChanges;
            trainer_UndoProperties_btn.Enabled = hasChanges;
            trainer_UndoAll_Btn.Enabled = UnsavedTrainerEditorChanges;
        }

        private void EnableDisableParty()
        {
            static void DisableControls<T>(IEnumerable<T> controls) where T : Control
            {
                foreach (var control in controls)
                {
                    control.Enabled = false;
                    if (control is ComboBox comboBox)
                        comboBox.BackColor = Color.White;
                }
            }

            // Disable all controls
            DisableControls(pokeComboBoxes);
            DisableControls(pokeIconsPictureBoxes);
            DisableControls(pokeLevelNums);
            DisableControls(pokeDVNums);
            DisableControls(pokeBallCapsuleComboBoxes);
            DisableControls(pokeFormsComboBoxes);
            DisableControls(pokeHeldItemComboBoxes);
            DisableControls(pokeGenderComboBoxes);
            DisableControls(pokeAbilityComboBoxes);

            // Disable controls for moves and PP
            for (int i = 0; i < pokeMovesComboBoxes.Count; i++)
            {
                DisableControls(pokeMovesComboBoxes[i]);
                DisableControls(pokePpNumberBoxes[i]);
            }

            if (trainer_PropertyFlags.GetItemChecked(0) && trainer_TeamSizeNum.Value < 2)
            {
                trainer_TeamSizeNum.Value = 2;
            }
            // Reset selected tab
            trainer_TeamSizeNum.Minimum = trainer_PropertyFlags.GetItemChecked(0) ? 2 : 1;

            int partySize = (int)trainer_TeamSizeNum.Value;

            // Enable controls based on the party size
            for (int i = 0; i < 6; i++)
            {
                bool isActive = i < partySize;

                // Update tab enabled state
                trainer_PartyData_tabControl.TabPages[i].Enabled = isActive;

                if (isActive)
                {
                    var selectedPokemonName = pokeComboBoxes[i].Text;

                    if (!string.IsNullOrWhiteSpace(selectedPokemonName))
                    {
                        int pokemonId = GetPokemonIdFromComboBoxText(selectedPokemonName);
                        if (pokemonId >= 0)
                        {
                            ushort speciesId = Species.GetSpecialSpecies((ushort)pokemonId, 0);
                            var species = GetSpeciesBySpeciesId(speciesId);

                            if (species != null)
                            {
                                // Enable controls for this Pokémon slot
                                EnablePokemonSlotControls(i, species, pokemonId);
                            }
                        }
                    }
                }
                UpdatePokemonTabName(i);
            }
        }

        private void EnablePokemonSlotControls(int index, Species species, int pokemonId)
        {
            pokeComboBoxes[index].Enabled = true;
            pokeIconsPictureBoxes[index].Enabled = true;
            pokeLevelNums[index].Enabled = true;
            pokeDVNums[index].Enabled = true;
            pokeBallCapsuleComboBoxes[index].Enabled = RomFile.IsNotDiamondPearl;
            pokeAbilityComboBoxes[index].Enabled = RomFile.IsNotDiamondPearl && species.HasMoreThanOneAbility;
            pokeFormsComboBoxes[index].Enabled = RomFile.IsNotDiamondPearl && Species.HasMoreThanOneForm(pokemonId);
            pokeHeldItemComboBoxes[index].Enabled = trainer_PropertyFlags.GetItemChecked(2);
            pokeGenderComboBoxes[index].Enabled = RomFile.IsNotDiamondPearl && species.HasMoreThanOneGender;

            if (index < pokeMovesComboBoxes.Count)
            {
                for (int j = 0; j < 4; j++)
                {
                    pokeMovesComboBoxes[index][j].Enabled = trainer_PropertyFlags.GetItemChecked(1);
                    pokePpNumberBoxes[index][j].Enabled = trainer_PropertyFlags.GetItemChecked(1) && RomFile.IsHgEngine;
                }
            }
        }

        private void EnableTrainerEditor()
        {
            Console.WriteLine("Enable Trainer Editor UI");
            trainer_RemoveBtn.Enabled = true;
            trainer_Copy_Btn.Enabled = true;
            trainer_Paste_Btn.Enabled = mainEditorModel.ClipboardTrainer != null;
            trainer_Import_Btn.Enabled = true;
            trainer_Export_Btn.Enabled = true;
            trainer_ClassListBox.Enabled = true;
            trainer_ViewClassBtn.Enabled = trainer_ClassListBox.SelectedIndex > -1;
            trainer_NameTextBox.Enabled = true;
            trainer_PropertiesTabControl.Enabled = true;
            trainer_SaveBtn.Enabled = true;
            Console.WriteLine("Enable Trainer Editor UI | Success");
        }

        #region Get

        private string GetAbilityNameByAbilityId(int abilityId)
        {
            Console.WriteLine($"Getting Ability Name for abilityId {abilityId}");
            string abiiltyName = mainEditorModel.AbilityNames[abilityId];
            if (!string.IsNullOrEmpty(abiiltyName))
            {
                Console.WriteLine($"Ability name found: " + abiiltyName);
            }
            else
            {
                Console.WriteLine($"Unable to match Ability name to abilityId {abilityId}");
            }
            return abiiltyName;
        }

        private Species GetSpeciesBySpeciesId(int speciesId)
        {
            Console.WriteLine($"Getting Species Data for speciesId {speciesId}");
            var species = mainEditorModel.PokemonSpecies.Find(x => x.SpeciesId == speciesId);
            if (species != null)
            {
                Console.WriteLine($"Species Data found");
            }
            else
            {
                Console.WriteLine($"Unable to match Species Data to speciesId {speciesId}");
            }
            return species;
        }

        private TrainerClass GetTrainerClassByTrainerClassId(int trainerClassId) => mainEditorModel.Classes.Find(x => x.TrainerClassId == trainerClassId);

        #endregion Get

        private static int GetIndex(ushort? index)
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

        private void GoToSelectedClass(int classId)
        {
            class_FilterTextBox.Text = "";
            main_MainTab.SelectedTab = main_MainTab_ClassTab;
            class_ClassListBox.SelectedIndex = classId;
            UpdateTrainerClassSprite(class_SpritePicBox, class_SpriteFrameNum, classId);
        }

        #region Initialize

        private void InitializeAiFlags()
        {
            foreach (var flag in AiFlags.AiFlagNames)
            {
                trainer_AiFlags_listbox.Items.Add(flag);
            }
        }

        private void InitializePropertyFlags()
        {
            foreach (var flag in TrainerPropertyFlags.TrainerPropertyFlagNames)
            {
                trainer_PropertyFlags.Items.Add(flag);
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
            foreach (var items in pokeMovesComboBoxes)
            {
                foreach (var comboBox in items)
                {
                    comboBox.SelectedIndex = -1;
                }
            }

            foreach (var items in pokePpNumberBoxes)
            {
                foreach (var numBox in items)
                {
                    numBox.Value = 0;
                }
            }
        }

        private void InitializeTrainerEditor()
        {
            Console.WriteLine("Initialize Trainer Editor");
            trainer_UndoAll_Btn.Enabled = false;
            trainer_SaveBtn.Enabled = false;
            trainer_AddTrainerBtn.Enabled = false;
            trainer_RemoveBtn.Enabled = false;
            trainer_ImportAllBtn.Enabled = false;
            trainer_ExportAllBtn.Enabled = false;
            trainer_FilterBox.Enabled = false;
            trainer_ClearFilterBtn.Enabled = false;
            trainer_TrainersListBox.Enabled = false;
            trainer_Copy_Btn.Enabled = false;
            trainer_Paste_Btn.Enabled = mainEditorModel.ClipboardTrainer != null;
            trainer_Import_Btn.Enabled = false;
            trainer_Export_Btn.Enabled = false;
            trainer_ClassListBox.Enabled = false;
            trainer_ViewClassBtn.Enabled = false;
            trainer_NameTextBox.Enabled = false;
            trainer_PropertiesTabControl.Enabled = false;
            Console.WriteLine("Initialize Trainer Editor | Success");
        }

        #endregion Initialize

        private void PopulateMoveComboBoxes()
        {
            foreach (var comboBoxArray in pokeMovesComboBoxes)
            {
                for (int i = 0; i < 4; i++)
                {
                    comboBoxArray[i].Items.Clear();
                    comboBoxArray[i].Items.AddRange(mainEditorModel.MoveNames.ToArray());
                }
            }
        }

        private void poke1AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(0);
            }
        }

        private void poke1BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke1LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(1);
            }
        }

        private void poke2BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(2);
            }
        }

        private void poke3BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(3);
            }
        }

        private void poke4BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(4);
            }
        }

        private void poke5BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6AbilityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateAbilty(5);
            }
        }

        private void poke6BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6DVNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6FormComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6LevelNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerParty(true);
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
            foreach (var item in trainerUsage.Where(x => x.TrainerUsageType == TrainerUsageType.Event).OrderBy(x => x.FileId))
            {
                trainer_EventUsage.Items.Add(item.ListItemName);
            }
        }

        private void PopulateItemComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            if (comboBox.Items.Count == 0)
            {
                comboBox.BeginUpdate();
                comboBox.Items.AddRange(mainEditorModel.ItemNames.ToArray());
                comboBox.EndUpdate();
            }
        }

        private void PopulatePartyData(TrainerParty trainerParty, int teamSize, bool chooseMoves)
        {
            isLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty(trainerParty, teamSize, chooseMoves);
            for (int i = 0; i < 6; i++)
            {
                UpdatePokemonTabName(i);
            }
            isLoadingData = false;
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

        private void PopulateTrainerBattleMessageTriggers(Trainer trainer)
        {
            isLoadingData = true;
            trainer_MessageTriggerListBox.Items.Clear();
            trainer_MessagePreviewText.Text = "";
            trainer_MessageTextBox.Text = "";
            List<string> messageTriggers = [];
            foreach (var item in mainEditorModel.BattleMessages.Where(x => x.TrainerId == trainer.TrainerId))
            {
                messageTriggers.Add(MessageTrigger.MessageTriggers.Find(x => x.MessageTriggerId == item.MessageTriggerId).ListName);
            }
            messageTriggers.Sort();
            messageTriggers.ForEach(x => trainer_MessageTriggerListBox.Items.Add(x));
            trainer_MessageUpBtn.Enabled = false;
            trainer_MessageUpBtn.Visible = false;
            trainer_MessageDownBtn.Enabled = false;
            trainer_MessageDownBtn.Visible = false;
            isLoadingData = false;
        }

        private void PopulateTrainerClassList(List<TrainerClass> classes)
        {
            trainer_ClassListBox.BeginUpdate();
            trainer_ClassListBox.Items.AddRange(classes.Select(c => c.ListName).ToArray());
            trainer_ClassListBox.EndUpdate();
        }

        private void PopulateTrainerData(Trainer trainer)
        {
            isLoadingData = true;
            SetTrainerPartyProperties(trainer.TrainerProperties);
            SetTrainerProperties(trainer.TrainerProperties);
            SetTrainerName(trainer);
            isLoadingData = false;
        }

        private void PopulateTrainerList(List<Trainer> trainers)
        {
            trainer_TrainersListBox.BeginUpdate();
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
            trainer_TrainersListBox.EndUpdate();
        }

        private void ResetPokeComboBoxValidation(int partyIndex)
        {
            if (pokeComboBoxes[partyIndex].BackColor == Color.PaleVioletRed)
            {
                pokeComboBoxes[partyIndex].BackColor = Color.White;
            }
        }

        private bool SaveTrainerMessage(int messageId)
        {
            var battleMessages = mainEditorModel.BattleMessages.OrderBy(x => x.MessageId).Select(x => x.MessageText).ToList();
            var saveMessage = fileSystemMethods.WriteBattleMessage(battleMessages, messageId, trainer_MessageTextBox.Text, RomFile.BattleMessageTextNumber);
            if (!saveMessage.Success)
            {
                MessageBox.Show(saveMessage.ErrorMessage, "Unable to Save Battle Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int trainerId = mainEditorModel.SelectedTrainer.TrainerId;
                int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
                var message = mainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
                if (message != default)
                {
                    message.MessageText = trainer_MessageTextBox.Text;
                }
            }
            return saveMessage.Success;
        }

        private bool SaveTrainerName(int trainerId)
        {
            var saveTrainerName = fileSystemMethods.WriteTrainerName(mainEditorModel.TrainerNames, trainerId, trainer_NameTextBox.Text, RomFile.TrainerNamesTextNumber);
            if (!saveTrainerName.Success)
            {
                MessageBox.Show(saveTrainerName.ErrorMessage, "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                EditedTrainerData(false);
                trainer_NameTextBox.BackColor = Color.White;
                mainEditorModel.TrainerNames[trainerId] = trainer_NameTextBox.Text;
                mainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).TrainerName = trainer_NameTextBox.Text;
                var index = trainer_TrainersListBox.FindString(UnfilteredTrainers[trainerId]);
                if (index > -1)
                {
                    trainer_TrainersListBox.Items[index] = mainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).ListName;
                    trainer_TrainersListBox.SelectedIndex = index;
                }
                UnfilteredTrainers[trainerId] = mainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).ListName;
            }
            return saveTrainerName.Success;
        }

        private bool SaveTrainerParty(int trainerId, bool displaySuccess = false)
        {
            bool isNotDiamondPearl = RomFile.IsNotDiamondPearl;
            List<bool> trainerTypeFlags = [];

            // Populate the flags based on the checklistbox items
            for (int i = 0; i < trainer_PropertyFlags.Items.Count; i++)
            {
                trainerTypeFlags.Add(trainer_PropertyFlags.GetItemChecked(i));
            }

            List<Pokemon> newPokemons = new List<Pokemon>();
            List<TrainerPartyPokemonData> newPokemonDatas = new List<TrainerPartyPokemonData>();

            int teamSize = (int)trainer_TeamSizeNum.Value;

            for (int i = 0; i < teamSize; i++)
            {
                int pokemonId = GetPokemonIdFromComboBoxText(pokeComboBoxes[i].Text);
                ushort formId = isNotDiamondPearl ? (ushort)pokeFormsComboBoxes[i].SelectedIndex : (ushort)0;
                ushort speciesId = Species.GetSpecialSpecies((ushort)pokemonId, formId);
                var species = GetSpeciesBySpeciesId(speciesId);

                byte genderAbilityOverride = species.HasMoreThanOneGender
                    ? (byte)(pokeGenderComboBoxes[i].SelectedIndex + (pokeAbilityComboBoxes[i].SelectedIndex << 4))
                    : (byte)(pokeAbilityComboBoxes[i].SelectedIndex << 4);

                ushort[]? pokemonMoves = trainerTypeFlags[1]
                    ?
                    [
                (ushort)pokeMovesComboBoxes[i][0].SelectedIndex,
                (ushort)pokeMovesComboBoxes[i][1].SelectedIndex,
                (ushort)pokeMovesComboBoxes[i][2].SelectedIndex,
                (ushort)pokeMovesComboBoxes[i][3].SelectedIndex
                    ]
                    : null;

                var newPokemon = new Pokemon(
                    (byte)pokeDVNums[i].Value,
                    genderAbilityOverride,
                    (ushort)pokeLevelNums[i].Value,
                    (ushort)pokemonId,
                    formId,
                    (ushort?)pokeHeldItemComboBoxes[i].SelectedIndex,
                    pokemonMoves,
                    (ushort?)pokeBallCapsuleComboBoxes[i].SelectedIndex
                );
                var newPokemonData = trainerEditorMethods.NewTrainerPartyPokemonData(
                    newPokemon, trainerTypeFlags[1], trainerTypeFlags[2], isNotDiamondPearl
                );

                newPokemons.Add(newPokemon);
                newPokemonDatas.Add(newPokemonData);
            }

            // Add dummy Pokemon data for remaining spots in the party
            for (int i = teamSize; i < 6; i++)
            {
                newPokemons.Add(new Pokemon());
            }

            var trainerPartyData = new TrainerPartyData(newPokemonDatas.ToArray());

            var writeFile = fileSystemMethods.WriteTrainerPartyData(
                trainerPartyData, trainerId, trainerTypeFlags.ToArray(), isNotDiamondPearl
            );

            if (writeFile.Success)
            {
                var trainerParty = new TrainerParty { Pokemons = newPokemons };
                mainEditorModel.SelectedTrainer.TrainerParty = trainerParty;
                mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize = (byte)teamSize;
                mainEditorModel.Trainers[trainerId].TrainerParty = trainerParty;
                mainEditorModel.Trainers[trainerId].TrainerProperties.TeamSize = (byte)teamSize;
                RomFile.TrainersPartyData[trainerId] = trainerPartyData;

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
            List<bool> propertyFlags = [];
            for (int i = 0; i < trainer_AiFlags_listbox.Items.Count; i++)
            {
                bool isChecked = trainer_AiFlags_listbox.GetItemChecked(i);
                aiFlags.Add(isChecked);
            }

            for (int i = 0; i < trainer_PropertyFlags.Items.Count; i++)
            {
                bool isChecked = trainer_PropertyFlags.GetItemChecked(i);
                propertyFlags.Add(isChecked);
            }

            var trainerProperties = trainerEditorMethods.NewTrainerProperties(
                (byte)trainer_TeamSizeNum.Value,
                (byte)TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString()),
                (ushort)trainer_ItemComboBox1.SelectedIndex,
                (ushort)trainer_ItemComboBox2.SelectedIndex,
                (ushort)trainer_ItemComboBox3.SelectedIndex,
                (ushort)trainer_ItemComboBox4.SelectedIndex,
                aiFlags,
                propertyFlags
                );
            var newTrainerData = trainerEditorMethods.NewTrainerData(trainerProperties);

            var writeFile = fileSystemMethods.WriteTrainerData(newTrainerData, trainerId);
            if (writeFile.Success)
            {
                mainEditorModel.SelectedTrainer.TrainerProperties = trainerProperties;
                mainEditorModel.Trainers.Single(x => x.TrainerId == trainerId).TrainerProperties = trainerProperties;
                RomFile.TrainersData[trainerId] = newTrainerData;
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
                case Pokemon.Pokedex.Pichu:
                    Species.AltForms.FormNames.Pichu.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Unown:
                    Species.AltForms.FormNames.Unown.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Castform:
                    Species.AltForms.FormNames.Castform.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Deoxys:
                    Species.AltForms.FormNames.Deoxys.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Burmy:
                case Pokemon.Pokedex.Wormadam:
                    Species.AltForms.FormNames.BurmyWormadam.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Shellos:
                case Pokemon.Pokedex.Gastrodon:
                    Species.AltForms.FormNames.ShellosGastrodon.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Rotom:
                    Species.AltForms.FormNames.Rotom.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Giratina:
                    Species.AltForms.FormNames.GiratinaForms.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                case Pokemon.Pokedex.Shaymin:
                    Species.AltForms.FormNames.ShayminForms.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                    break;

                default:
                    if (RomFile.IsHgEngine)
                    {
                        switch (pokemonId)
                        {
                            case Pokemon.Pokedex.Pikachu:
                                Species.AltForms.FormNames.HgEngineForms.Pikachu.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                                break;
                            case Pokemon.Pokedex.Groudon:
                                Species.AltForms.FormNames.HgEngineForms.Groudon.ForEach(x => pokeFormsComboBoxes[partyIndex].Items.Add(x));
                                break;
                            default:
                                pokeFormsComboBoxes[partyIndex].Items.Add(Species.AltForms.FormNames.Default);
                                break;
                        }
                    }
                    else
                    {
                        pokeFormsComboBoxes[partyIndex].Items.Add(Species.AltForms.FormNames.Default);
                    }
                    break;
            }
        }

        private void SetPokemonSpecialData(int partyIndex)
        {
            var selectedPokemonName = pokeComboBoxes[partyIndex].Text;

            if (!string.IsNullOrWhiteSpace(selectedPokemonName))
            {
                int pokemonId = GetPokemonIdFromComboBoxText(selectedPokemonName);
                ushort speciesId = Species.GetSpecialSpecies((ushort)pokemonId, 0);
                var species = GetSpeciesBySpeciesId(speciesId);

                // Clear and configure the ability ComboBox
                pokeAbilityComboBoxes[partyIndex].Enabled = false;
                pokeAbilityComboBoxes[partyIndex].Items.Clear();
                pokeAbilityComboBoxes[partyIndex].SelectedIndex = -1;

                if (species != null) // Ensure species is valid
                {
                    if (RomFile.IsNotDiamondPearl)
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

                    // Populate abilities
                    pokeAbilityComboBoxes[partyIndex].Items.Add("-");
                    if (species.Ability1 > 0)
                    {
                        pokeAbilityComboBoxes[partyIndex].Items.Add(GetAbilityNameByAbilityId(species.Ability1));
                    }
                    if (species.Ability2 > 0)
                    {
                        pokeAbilityComboBoxes[partyIndex].Items.Add(GetAbilityNameByAbilityId(species.Ability2));
                    }

                    // Set the selected index based on ability availability
                    pokeAbilityComboBoxes[partyIndex].SelectedIndex = species.HasNoAbilities ? 0 : species.HasMoreThanOneAbility ? 0 : 1;
                    pokeAbilityComboBoxes[partyIndex].Enabled = !species.HasNoAbilities && species.HasMoreThanOneAbility;

                    // Handle forms for non-Diamond/Pearl games
                    if (RomFile.IsNotDiamondPearl)
                    {
                        SetPokemonForms(pokemonId, partyIndex);
                        pokeFormsComboBoxes[partyIndex].SelectedIndex = 0;
                        pokeFormsComboBoxes[partyIndex].Enabled = Species.HasMoreThanOneForm(pokemonId);
                    }

                    pokeBallCapsuleComboBoxes[partyIndex].SelectedIndex = RomFile.IsNotDiamondPearl ? 0 : -1;
                }
            }
        }

        private void SetTrainerName(Trainer trainer) => trainer_NameTextBox.Text = trainer.TrainerName;

        private void SetTrainerParty(TrainerParty trainerParty, int teamSize, bool chooseMoves)
        {
            for (int i = 0; i < teamSize; i++)
            {
                pokeComboBoxes[i].BeginUpdate();
                pokeComboBoxes[i].Items.Clear();
                pokeComboBoxes[i].Items.AddRange(mainEditorModel.PokemonNames.ToArray());
                pokeComboBoxes[i].EndUpdate();

                // Use pokemonId as an index into MainEditorModel.PokemonNames
                int pokemonId = trainerParty.Pokemons[i].PokemonId;

                // Ensure pokemonId is within bounds
                if (pokemonId >= 0 && pokemonId < mainEditorModel.PokemonNamesFull.Count)
                {
                    if (pokemonId > 543)
                    {
                        pokemonId -= 50;
                    }
                    // Set the selected item directly to match the Pokémon name
                    pokeComboBoxes[i].SelectedItem = mainEditorModel.PokemonNames[pokemonId];
                }
                else
                {
                    // Handle invalid Pokémon ID gracefully
                    pokeComboBoxes[i].SelectedItem = null; // Deselect if ID is out of range
                }
                var species = GetSpeciesBySpeciesId(trainerParty.Pokemons[i].SpeciesId);

                // Enable filtering and handle events for the ComboBox
                EnablePokemonComboBoxFiltering(pokeComboBoxes[i], i);
                pokeLevelNums[i].Value = trainerParty.Pokemons[i].Level;
                pokeDVNums[i].Value = trainerParty.Pokemons[i].DifficultyValue;
                pokeAbilityComboBoxes[i].Items.Clear();
                pokeFormsComboBoxes[i].Items.Clear();
                pokeHeldItemComboBoxes[i].SelectedIndex = GetIndex(trainerParty.Pokemons[i].HeldItemId);
                pokeBallCapsuleComboBoxes[i].SelectedIndex = RomFile.IsNotDiamondPearl ? GetIndex(trainerParty.Pokemons[i].BallCapsuleId) : -1;

                if (chooseMoves)
                {
                    for (int move = 0; move < 4; move++)
                    {
                        pokeMovesComboBoxes[i][move].SelectedIndex = trainerParty.Pokemons[i].Moves[move];
                    }
                }

                if (RomFile.IsNotDiamondPearl)
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

                if (RomFile.IsNotDiamondPearl)
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
            trainer_TeamSizeNum.Minimum = trainerProperties.DoubleBattle ? 2 : 1;
            trainer_TeamSizeNum.Value = trainerProperties.TeamSize == 0 ? 1 : trainerProperties.TeamSize;

            trainer_PropertyFlags.SetItemChecked(0, trainerProperties.DoubleBattle);
            trainer_PropertyFlags.SetItemChecked(2, trainerProperties.ChooseItems);
            trainer_PropertyFlags.SetItemChecked(1, trainerProperties.ChooseMoves);
            if (RomFile.IsHgEngine)
            {
                trainer_PropertyFlags.SetItemChecked(3, trainerProperties.ChooseAbility_Hge);
                trainer_PropertyFlags.SetItemChecked(4, trainerProperties.ChooseBall_Hge);
                trainer_PropertyFlags.SetItemChecked(5, trainerProperties.SetIvEv_Hge);
                trainer_PropertyFlags.SetItemChecked(6, trainerProperties.ChooseNature_Hge);
                trainer_PropertyFlags.SetItemChecked(7, trainerProperties.ShinyLock_Hge);
                trainer_PropertyFlags.SetItemChecked(8, trainerProperties.AdditionalFlags_Hge);
            }
        }

        private void SetTrainerProperties(TrainerProperty trainerProperties)
        {
            for (int i = 0; i < 4; i++)
            {
                trainerItemsComboBoxes[i].SelectedIndex = trainerProperties.Items[i] == 0xFFFF ? 0 : trainerProperties.Items[i];
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
            pokeComboBoxes = [poke1ComboBox, poke2ComboBox, poke3ComboBox, poke4ComboBox, poke5ComboBox, poke6ComboBox];

            pokeIconsPictureBoxes = [poke1IconPicBox, poke2IconPicBox, poke3IconPicBox, poke4IconPicBox, poke5IconPicBox, poke6IconPicBox,];

            pokeLevelNums = [poke1LevelNum, poke2LevelNum, poke3LevelNum, poke4LevelNum, poke5LevelNum, poke6LevelNum,];

            pokeDVNums = [poke1DVNum, poke2DVNum, poke3DVNum, poke4DVNum, poke5DVNum, poke6DVNum,];

            pokeGenderComboBoxes = [poke1GenderComboBox, poke2GenderComboBox, poke3GenderComboBox, poke4GenderComboBox, poke5GenderComboBox, poke6GenderComboBox,];

            pokeAbilityComboBoxes = [poke1AbilityComboBox, poke2AbilityComboBox, poke3AbilityComboBox, poke4AbilityComboBox, poke5AbilityComboBox, poke6AbilityComboBox,];

            pokeBallCapsuleComboBoxes = [poke1BallCapsuleComboBox, poke2BallCapsuleComboBox, poke3BallCapsuleComboBox, poke4BallCapsuleComboBox, poke5BallCapsuleComboBox, poke6BallCapsuleComboBox,];

            pokeFormsComboBoxes = [poke1FormComboBox, poke2FormComboBox, poke3FormComboBox, poke4FormComboBox, poke5FormComboBox, poke6FormComboBox,];

            pokeHeldItemComboBoxes = [poke1HeldItemComboBox, poke2HeldItemComboBox, poke3HeldItemComboBox, poke4HeldItemComboBox, poke5HeldItemComboBox, poke6HeldItemComboBox,];

            poke1Moves = [poke1Moves1_comboBox, poke1Moves2_comboBox, poke1Moves3_comboBox, poke1Moves4_comboBox];
            poke2Moves = [poke2Moves1_comboBox, poke2Moves2_comboBox, poke2Moves3_comboBox, poke2Moves4_comboBox];
            poke3Moves = [poke3Moves1_comboBox, poke3Moves2_comboBox, poke3Moves3_comboBox, poke3Moves4_comboBox];
            poke4Moves = [poke4Moves1_comboBox, poke4Moves2_comboBox, poke4Moves3_comboBox, poke4Moves4_comboBox];
            poke5Moves = [poke5Moves1_comboBox, poke5Moves2_comboBox, poke5Moves3_comboBox, poke5Moves4_comboBox];
            poke6Moves = [poke6Moves1_comboBox, poke6Moves2_comboBox, poke6Moves3_comboBox, poke6Moves4_comboBox];

            poke1PPs = [poke1_Move1_PP_num, poke1_Move2_PP_num, poke1_Move3_PP_num, poke1_Move4_PP_num];
            poke2PPs = [poke2_Move1_PP_num, poke2_Move2_PP_num, poke2_Move3_PP_num, poke2_Move4_PP_num];
            poke3PPs = [poke3_Move1_PP_num, poke3_Move2_PP_num, poke3_Move3_PP_num, poke3_Move4_PP_num];
            poke4PPs = [poke4_Move1_PP_num, poke4_Move2_PP_num, poke4_Move3_PP_num, poke4_Move4_PP_num];
            poke5PPs = [poke5_Move1_PP_num, poke5_Move2_PP_num, poke5_Move3_PP_num, poke5_Move4_PP_num];
            poke6PPs = [poke6_Move1_PP_num, poke6_Move2_PP_num, poke6_Move3_PP_num, poke6_Move4_PP_num];

            pokeMovesComboBoxes = [poke1Moves, poke2Moves, poke3Moves, poke4Moves, poke5Moves, poke6Moves,];
            pokePpNumberBoxes = [poke1PPs, poke2PPs, poke3PPs, poke4PPs, poke5PPs, poke5PPs,];
        }

        private void SetupTrainerEditor()
        {
            isLoadingData = true;
            if (trainerItemsComboBoxes == null || trainerItemsComboBoxes?.Count == 0)
            {
                trainerItemsComboBoxes = [trainer_ItemComboBox1, trainer_ItemComboBox2, trainer_ItemComboBox3, trainer_ItemComboBox4];
            }
            trainerItemsComboBoxes?.ForEach(PopulateItemComboBox);

            if (trainer_TrainersListBox.Items.Count == 0)
            {
                trainer_TrainersListBox.SelectedIndex = -1;
                PopulateTrainerList(mainEditorModel.Trainers);
            }
            if (trainer_ClassListBox.Items.Count == 0)
            {
                trainer_ClassListBox.SelectedIndex = -1;
                PopulateTrainerClassList(mainEditorModel.Classes);
            }
            if (trainer_AiFlags_listbox.Items.Count == 0)
            {
                InitializeAiFlags();
            }
            if (trainer_PropertyFlags.Items.Count == 0)
            {
                InitializePropertyFlags();
            }
            if (poke1ComboBox.Items.Count == 0)
            {
                SetupPartyEditorFields();
                pokeComboBoxes?.ForEach(x => x.SelectedIndex = -1);
                PopulatePokemonComboBoxes();
            }
            if (poke1GenderComboBox.Items.Count == 0 && RomFile.IsNotDiamondPearl)
            {
                PopulatePokemonGenderComboBoxes();
                pokeGenderComboBoxes?.ForEach(x => x.SelectedIndex = -1);
            }
            else
            {
                pokeGenderComboBoxes?.ForEach(x => x.Enabled = false);
            }
            PopulateMoveComboBoxes();
            if (poke1HeldItemComboBox.Items.Count == 0)
            {
                pokeHeldItemComboBoxes?.ForEach(PopulateItemComboBox);
            }

            if (poke1BallCapsuleComboBox.Items.Count == 0 && RomFile.IsNotDiamondPearl)
            {
                InitializeBallCapsules();
            }
            trainer_TrainersListBox.Enabled = true;
            isLoadingData = false;
        }

        private void trainer_AddTrainerBtn_Click(object sender, EventArgs e)
        {
            if (!isLoadingData)
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
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ChooseMovesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty();
            }
        }

        private void trainer_ClassListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
                int trainerClassId = TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString());
                UpdateTrainerClassSprite(trainer_SpritePicBox, trainer_SpriteFrameNum, trainerClassId);
            }

            trainer_ViewClassBtn.Enabled = trainer_ClassListBox.SelectedIndex > -1;
        }

        private void trainer_ClearFilterBtn_Click(object sender, EventArgs e) => trainer_FilterBox.Text = "";

        private void trainer_DblBattleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            trainer_TeamSizeNum.Minimum = trainer_PropertyFlags.GetItemChecked(0) ? 2 : 1;

            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty();
            }
        }

        private void trainer_FilterBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                if (string.IsNullOrEmpty(trainer_FilterBox.Text))
                {
                    PopulateTrainerList(mainEditorModel.Trainers);
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
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
                EnableDisableParty();
            }
        }

        private void trainer_ItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_ItemComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                EditedTrainerProperty(true);
            }
        }

        private void trainer_MessageDownBtn_Click(object sender, EventArgs e) => MessagePreviewNavigate(true, trainer_MessageDownBtn, trainer_MessageUpBtn, trainer_MessagePreviewText);

        private void trainer_MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                UpdateTextPreview(trainer_MessageTextBox.Text, trainer_MessagePreviewText, trainer_MessageUpBtn, trainer_MessageDownBtn);
            }
        }

        private void trainer_MessageTriggerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                int trainerId = mainEditorModel.SelectedTrainer.TrainerId;
                int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
                var message = mainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
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

        private void trainer_MessageUpBtn_Click(object sender, EventArgs e) => MessagePreviewNavigate(false, trainer_MessageDownBtn, trainer_MessageUpBtn, trainer_MessagePreviewText);

        private void trainer_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (trainer_NameTextBox.BackColor == Color.PaleVioletRed)
            {
                trainer_NameTextBox.BackColor = Color.White;
            }
            if (!isLoadingData)
            {
                EditedTrainerData(true);
            }
        }

        private void ReloadTrainersList()
        {
            mainEditorModel.TrainerNames.Clear();
            mainEditorModel.Trainers.Clear();
            UnfilteredTrainers.Clear();

            trainer_TrainersListBox.Items.Clear();
            mainEditorModel.TrainerNames = new List<string>(RomFile.TrainerNames);
            mainEditorModel.Trainers = trainerEditorMethods.GetTrainers();
            PopulateTrainerList(mainEditorModel.Trainers);
        }

        private void trainer_RemoveBtn_Click(object sender, EventArgs e)
        {
            if (isLoadingData) return;

            int trainerId = mainEditorModel.SelectedTrainer.TrainerId;

            if (trainerId <= RomFile.VanillaTotalTrainers - 1)
            {
                MessageBox.Show("This is one of the game's core Trainers." +
                    "\nYou cannot remove this file as it will cause issues.", "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (mainEditorModel.BattleMessages.Any(x => x.TrainerId == trainerId))
            {
                MessageBox.Show("This Trainer has Battle Messages assigned." +
                    "\nYou must remove these first before removing the trainer", "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (mainEditorModel.SelectedTrainer.TrainerUsages.Count > 0)
            {
                var confirmDelete = MessageBox.Show("This Trainer is used either in an event, or script." +
                    "\nIf you remove this trainer you will have to change the references to the trainer." +
                    "\nRemove Trainer?", "Trainer Usage Found", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (confirmDelete != DialogResult.Yes) return;
            }

            DeleteTrainer(trainerId);
        }

        private void DeleteTrainer(int trainerId)
        {
            var removeTrainer = trainerEditorMethods.RemoveTrainer(trainerId);

            if (removeTrainer.Success)
            {
                trainer_FilterBox.Text = "";
                RefreshTrainerData();
                ReloadTrainersList();
                ClearUnsavedTrainerChanges();
                trainer_TrainersListBox.SelectedIndex = trainerId - 1;
                Console.WriteLine($"Removed Trainer number: {trainerId:D4}");
                MessageBox.Show($"Removed Trainer number: {trainerId:D4}", "Success");
            }
            else
            {
                Console.WriteLine(removeTrainer.ErrorMessage);
                MessageBox.Show(removeTrainer.ErrorMessage, "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void trainer_SaveBtn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;
            if (ValidateTrainerName() && ValidatePokemon() && ValidatePokemonMoves() && SaveTrainerName(mainEditorModel.SelectedTrainer.TrainerId) && SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId) && SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId))
            {
                MessageBox.Show("Trainer Data updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            isLoadingData = false;
        }

        private void trainer_SaveParty_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemon() && ValidatePokemonMoves())
            {
                if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_PropertyFlags.GetItemChecked(1)
                    && mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_PropertyFlags.GetItemChecked(2))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_PropertyFlags.GetItemChecked(1))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_PropertyFlags.GetItemChecked(2))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId, true);
                }
            }
        }

        private void trainer_SaveProperties_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemonMoves())
            {
                if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_PropertyFlags.GetItemChecked(1)
                    && mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_PropertyFlags.GetItemChecked(2))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != trainer_PropertyFlags.GetItemChecked(1))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else if (mainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != trainer_PropertyFlags.GetItemChecked(2))
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(mainEditorModel.SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerProperties(mainEditorModel.SelectedTrainer.TrainerId, true);
                }
            }
        }

        private void trainer_SpriteFrameNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                int trainerClassId = TrainerClass.ListNameToTrainerClassId(trainer_ClassListBox.SelectedItem.ToString());
                UpdateTrainerClassSprite(trainer_SpritePicBox, trainer_SpriteFrameNum, trainerClassId);
            }
        }

        private void trainer_TeamSizeNum_ValueChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                if (trainer_TeamSizeNum.Minimum == 0)
                {
                    trainer_TeamSizeNum.Minimum = 1;
                }
                EditedTrainerProperty(true);
                EnableDisableParty();
            }
        }

        private void trainer_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoadingData && trainer_TrainersListBox.SelectedIndex > -1)
            {
                string selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();

                if (selectedTrainer != mainEditorModel.SelectedTrainer.ListName)
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
                            trainer_TrainersListBox.SelectedIndex = trainer_TrainersListBox.Items.IndexOf(mainEditorModel.SelectedTrainer.ListName);
                        }
                    }

                    if (!InhibitTrainerChange)
                    {
                        selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();
                        mainEditorModel.SelectedTrainer = new Trainer(trainerEditorMethods.GetTrainer(mainEditorModel.Trainers, Trainer.ListNameToTrainerId(selectedTrainer)));

                        if (mainEditorModel.SelectedTrainer.TrainerId >= 0)
                        {
                            PopulateTrainerData(mainEditorModel.SelectedTrainer);
                            PopulatePartyData(mainEditorModel.SelectedTrainer.TrainerParty, mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
                            PopulateTrainerBattleMessageTriggers(mainEditorModel.SelectedTrainer);
                            PopualteTrainerUsages(mainEditorModel.SelectedTrainer.TrainerUsages);
                            PopulateTrainerClassSprite(trainer_SpritePicBox, trainer_SpriteFrameNum, mainEditorModel.SelectedTrainer.TrainerProperties.TrainerClassId);
                            EnableTrainerEditor();
                            EnableDisableParty();
                        }
                    }
                    else
                    {
                        InhibitTrainerChange = false;
                    }
                }
            }
        }

        private void trainer_UndoAll_Btn_Click(object sender, EventArgs e) => UndoTrainerChanges();

        private void trainer_UndoParty_btn_Click(object sender, EventArgs e) => UndoTrainerPartyChanges();

        private void trainer_UndoProperties_Click(object sender, EventArgs e) => UndoTrainerPropertyChanges();

        private void trainer_ViewClassBtn_Click(object sender, EventArgs e)
        {
            if (!isLoadingData && trainer_ClassListBox.SelectedIndex > -1)
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
            isLoadingData = true;
            SetTrainerName(mainEditorModel.SelectedTrainer);
            SetTrainerPartyProperties(mainEditorModel.SelectedTrainer.TrainerProperties);
            SetTrainerProperties(mainEditorModel.SelectedTrainer.TrainerProperties);
            InitializePartyEditor();
            SetTrainerParty(mainEditorModel.SelectedTrainer.TrainerParty, mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
            EditedTrainerData(false);
            EditedTrainerProperty(false);
            EditedTrainerParty(false);
            isLoadingData = false;
        }

        private void UndoTrainerPartyChanges()
        {
            isLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty(mainEditorModel.SelectedTrainer.TrainerParty, mainEditorModel.SelectedTrainer.TrainerProperties.TeamSize, mainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves);
            EditedTrainerParty(false);
            isLoadingData = false;
        }

        private void UndoTrainerPropertyChanges()
        {
            isLoadingData = true;
            SetTrainerPartyProperties(mainEditorModel.SelectedTrainer.TrainerProperties);
            SetTrainerProperties(mainEditorModel.SelectedTrainer.TrainerProperties);
            EditedTrainerProperty(false);
            isLoadingData = false;
        }

        private void UpdatePokemonTabName(int index)
        {
            if (pokeComboBoxes[index].Enabled)
            {
                string selectedText = pokeComboBoxes[index].Text;
                string name = selectedText.Substring(7);
                trainer_PartyData_tabControl.TabPages[index].Text = $"{name}";
            }
            else
            {
                trainer_PartyData_tabControl.TabPages[index].Text = $"-----";
            }
            
        }

        private void UpdateAbilty(int index) => EditedTrainerParty(true);

        private bool ValidatePokemon()
        {
            var invalidCombos = pokeComboBoxes.Take((int)trainer_TeamSizeNum.Value)
                .Where(cb =>
                {
                    var selectedItem = cb.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(selectedItem)) return true;

                    // Check if the format matches "[0000] xxxxx"
                    var match = System.Text.RegularExpressions.Regex.Match(selectedItem, @"^\[(\d{4})\] .+");
                    if (match.Success)
                    {
                        if (int.TryParse(match.Groups[1].Value, out int number))
                        {
                            // Valid if number is greater than 0
                            return number <= 0;
                        }
                    }

                    // If the format is invalid or number parsing fails, it's an invalid combo
                    return true;
                })
                .ToList();

            foreach (var combo in invalidCombos)
            {
                combo.BackColor = Color.PaleVioletRed;
            }

            if (invalidCombos.Any())
            {
                MessageBox.Show("You must select a Pokemon", "Unable to Save Trainer Party", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        private bool ValidatePokemonMoves()
        {
            if (trainer_PropertyFlags.GetItemChecked(1))
            {
                for (int i = 0; i < trainer_TeamSizeNum.Value; i++)
                {
                    var moveArray = pokeMovesComboBoxes[i];
                    if (moveArray == null)
                    {
                        MessageBox.Show("You have not set moves for all Party Pokemon!\nClick the Moves button next to each Pokemon to set moves", "Unable to Save Trainer Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                    else if (!moveArray.Any(x => x.SelectedIndex > 0))
                    {
                        MessageBox.Show("You have not set moves for all Party Pokemon!\nClick the Moves button next to each Pokemon to set moves", "Unable to Save Trainer Properties", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
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
                    return RomPatches.ExpandTrainerNames();
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

        #region PokemonComboBoxes

        private bool isUpdatingComboBox = false;

        private object previousSelection;

        private void EnablePokemonComboBoxFiltering(ComboBox pokeComboBox, int partyIndex)
        {
            pokeComboBox.TextUpdate -= HandleTextUpdate;
            pokeComboBox.KeyDown -= HandleKeyDown;
            pokeComboBox.Leave -= (s, e) => ResetComboBoxOnFocusLoss(pokeComboBox);
            pokeComboBox.SelectedIndexChanged -= (s, e) => HandlePokeComboBoxSelectionChanged(pokeComboBox, partyIndex);

            pokeComboBox.DropDownStyle = ComboBoxStyle.DropDown;
            pokeComboBox.TextUpdate += HandleTextUpdate;
            pokeComboBox.KeyDown += HandleKeyDown;
            pokeComboBox.Leave += (s, e) => ResetComboBoxOnFocusLoss(pokeComboBox);
            pokeComboBox.SelectedIndexChanged += (s, e) => HandlePokeComboBoxSelectionChanged(pokeComboBox, partyIndex);
        }

        private void FilterTimer_Tick(object sender, EventArgs e)
        {
            filterTimer.Stop(); // Stop the timer
            var pokeComboBox = (ComboBox)filterTimer.Tag; // Get the last input
            PerformFiltering(pokeComboBox); // Call the update method with the last input
        }

        private int GetPokemonIdFromComboBoxText(string selectedItemText)
        {
            if (string.IsNullOrWhiteSpace(selectedItemText))
            {
                return -1; // Return -1 if the input is invalid
            }
            string pokemonName = selectedItemText.Substring(7);
            int pokemonId = mainEditorModel.PokemonNamesFull.IndexOf(pokemonName);
            return pokemonId;
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            var pokeComboBox = sender as ComboBox;

            // Open dropdown on any key press except Enter
            if (e.KeyCode != Keys.Enter)
            {
                pokeComboBox.DroppedDown = true; // Keep dropdown open
            }
        }

        private void HandlePokeComboBoxSelectionChanged(ComboBox pokeComboBox, int partyIndex)
        {
            // Store the previously selected item
            previousSelection = pokeComboBox.SelectedItem;

            // Call the method that handles additional logic when the selection changes
            if (!isLoadingData)
            {
                SetPokemonSpecialData(partyIndex);
                EnableDisableParty();
                EditedTrainerParty(true);
                UpdatePokemonTabName(partyIndex);
            }
        }

        private void HandleTextUpdate(object sender, EventArgs e)
        {
            var pokeComboBox = sender as ComboBox;

            // If the timer is already running, stop it
            if (filterTimer.Enabled)
            {
                filterTimer.Stop();
            }

            // Set the last input to the timer's Tag property
            filterTimer.Tag = pokeComboBox;
            filterTimer.Start(); // Start the timer
        }

        private void PerformFiltering(ComboBox pokeComboBox)
        {
            if (isUpdatingComboBox) return;

            isUpdatingComboBox = true;

            string userInput = pokeComboBox.Text.ToLower();
            var allPokemon = mainEditorModel.PokemonNames;

            var filteredPokemon = allPokemon
                .Where(pokemon => pokemon.ToLower().Contains(userInput))
                .ToArray();

            pokeComboBox.BeginUpdate();
            pokeComboBox.Items.Clear();
            pokeComboBox.Items.AddRange(filteredPokemon);
            pokeComboBox.EndUpdate();

            if (pokeComboBox.Items.Count > 0)
            {
                pokeComboBox.DroppedDown = true;
            }

            // Set caret position to the end to avoid highlighting
            pokeComboBox.Text = userInput; // Ensure the text remains intact
            pokeComboBox.SelectionStart = userInput.Length; // Move caret to the end

            isUpdatingComboBox = false;
        }

        private void PopulatePokemonComboBoxes()
        {
            for (int i = 0; i < pokeComboBoxes.Count; i++)
            {
                var comboBox = pokeComboBoxes[i];

                comboBox.BeginUpdate();
                comboBox.Items.Clear();
                comboBox.Items.AddRange(mainEditorModel.PokemonNames.ToArray());
                EnablePokemonComboBoxFiltering(comboBox, i);

                comboBox.EndUpdate();
            }
        }

        private void ResetComboBoxItems(ComboBox pokeComboBox)
        {
            if (!isUpdatingComboBox)
            {
                isUpdatingComboBox = true;

                pokeComboBox.BeginUpdate();
                pokeComboBox.Items.Clear();
                pokeComboBox.Items.AddRange(mainEditorModel.PokemonNames.ToArray());
                pokeComboBox.EndUpdate();

                isUpdatingComboBox = false;
            }
        }

        private void ResetComboBoxOnFocusLoss(ComboBox pokeComboBox)
        {
            if (pokeComboBox.SelectedItem == null && !string.IsNullOrWhiteSpace(pokeComboBox.Text))
            {
                ResetComboBoxItems(pokeComboBox);
                pokeComboBox.SelectedItem = previousSelection;
            }
        }

        #endregion PokemonComboBoxes
    }
}