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
        private Trainer SelectedTrainer = new();
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
            fileSystemMethods.WriteTrainerPartyData(new TrainerPartyData(), newTrainerId, false, false, LoadedRom.GameFamily != GameFamily.DiamondPearl);
            UnfilteredTrainers.Add(MainEditorModel.Trainers[newTrainerId - 1].ListName);
            trainer_TrainersListBox.Items.Add(MainEditorModel.Trainers[newTrainerId - 1].ListName);
            LoadedRom.TotalNumberOfTrainers++;
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
            SelectedTrainer = new Trainer();
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
                pokeBallCapsuleComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl;
                pokeAbilityComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl && species.HasMoreThanOneAbility;
                pokeFormsComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl && Species.HasMoreThanOneForm(pokeComboBoxes[i].SelectedIndex);
                pokeHeldItemComboBoxes[i].Enabled = chooseItems;
                pokeMoveButtons[i].Enabled = chooseMoves;
                pokeGenderComboBoxes[i].Enabled = LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver
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
            trainer_Paste_Btn.Enabled = true;
            trainer_Import_Btn.Enabled = true;
            trainer_Export_Btn.Enabled = true;
            trainer_ClassListBox.Enabled = true;
            trainer_ViewClassBtn.Enabled = false;
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
            trainer_Paste_Btn.Enabled = false;
            trainer_Import_Btn.Enabled = false;
            trainer_Export_Btn.Enabled = false;
            trainer_ClassListBox.Enabled = false;
            trainer_ViewClassBtn.Enabled = false;
            trainer_NameTextBox.Enabled = false;
            trainer_PropertiesTabControl.Enabled = false;
        }

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

            if (!UnsavedTrainerEditorChanges && SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves != null)
            {
                bool hasChanges = SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[0] != pokeMoves[partyIndex][0]
                     || SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[1] != pokeMoves[partyIndex][1]
                     || SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[2] != pokeMoves[partyIndex][2]
                     || SelectedTrainer.TrainerParty.Pokemons[partyIndex].Moves[3] != pokeMoves[partyIndex][3];

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

        private void poke1HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void poke2HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void poke3HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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

        private void poke4HeldItemComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
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
                foreach (var item in MainEditorModel.ItemNames) { comboBox.Items.Add(item); }
            }
        }

        private void PopulatePartyData()
        {
            IsLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty();
            IsLoadingData = false;
        }

        private void PopulatePokemonComboBoxes()
        {
            foreach (var comboBox in pokeComboBoxes)
            {
                comboBox.Items.Clear();
                comboBox.Items.Add("---------");
                for (int i = 1; i < MainEditorModel.PokemonNames.Count; i++)
                {
                    comboBox.Items.Add($"[{i:D4}] {MainEditorModel.PokemonNames[i]}");
                }
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

        private void PopulateTrainerData()
        {
            IsLoadingData = true;
            SetTrainerProperties();
            SetTrainerName();
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
                MainEditorModel.Trainers[trainerId - 1].TrainerName = trainer_NameTextBox.Text;
                var index = trainer_TrainersListBox.FindString(UnfilteredTrainers[trainerId - 1]);
                if (index > -1)
                {
                    trainer_TrainersListBox.Items[index] = MainEditorModel.Trainers[trainerId - 1].ListName;
                    trainer_TrainersListBox.SelectedIndex = index;
                }
                UnfilteredTrainers[trainerId - 1] = MainEditorModel.Trainers[trainerId - 1].ListName;
            }
            return saveTrainerName.Success;
        }

        private bool SaveTrainerParty(int trainerId, bool displaySuccess = false)
        {
            List<Pokemon> newPokemons = [];
            TrainerPartyPokemonData[] newPokemonDatas = new TrainerPartyPokemonData[(int)trainer_TeamSizeNum.Value];
            for (int i = 0; i < trainer_TeamSizeNum.Value; i++)
            {
                var species = GetSpeciesBySpeciesId(pokeComboBoxes[i].SelectedIndex);
                byte genderAbilityOverride = species.HasMoreThanOneGender ? (byte)(pokeGenderComboBoxes[i].SelectedIndex + (pokeAbilityComboBoxes[i].SelectedIndex << 4)) : (byte)(pokeAbilityComboBoxes[i].SelectedIndex << 4);
                var newPokemon = trainerEditorMethods.NewPartyPokemon((ushort)pokeComboBoxes[i].SelectedIndex, (ushort)pokeLevelNums[i].Value, (byte)pokeDVNums[i].Value, genderAbilityOverride, (ushort)pokeFormsComboBoxes[i].SelectedIndex, (ushort?)pokeBallCapsuleComboBoxes[i].SelectedIndex, (ushort?)pokeHeldItemComboBoxes[i].SelectedIndex, pokeMoves[i]);
                var newPokemonData = trainerEditorMethods.NewTrainerPartyPokemonData(newPokemon, trainer_ChooseMovesCheckbox.Checked, trainer_HeldItemsCheckbox.Checked, LoadedRom.GameFamily != GameFamily.DiamondPearl);
                newPokemons.Add(newPokemon);
                newPokemonDatas[i] = newPokemonData;
            }
            var trainerPartyData = trainerEditorMethods.NewTrainerPartyData(newPokemonDatas);

            var writeFile = fileSystemMethods.WriteTrainerPartyData(trainerPartyData, trainerId, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked, LoadedRom.GameFamily != GameFamily.DiamondPearl);
            if (writeFile.Success)
            {
                // Add dummy pokemon data
                for (int i = 0; i < 6 - newPokemons.Count; i++)
                {
                    newPokemons.Add(new Pokemon());
                }
                var trainerParty = new TrainerParty { Pokemons = newPokemons };

                SelectedTrainer.TrainerParty = trainerParty;
                MainEditorModel.Trainers[trainerId - 1].TrainerParty = trainerParty;
                LoadedRom.TrainersPartyData[trainerId] = trainerPartyData;
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
                SelectedTrainer.TrainerProperties = trainerProperties;
                MainEditorModel.Trainers[trainerId - 1].TrainerProperties = trainerProperties;
                LoadedRom.TrainersData[trainerId] = newTrainerData;
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

                if (LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver)
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

                if (LoadedRom.GameFamily != GameFamily.DiamondPearl)
                {
                    SetPokemonForms(pokeComboBoxes[partyIndex].SelectedIndex, partyIndex);
                    pokeFormsComboBoxes[partyIndex].SelectedIndex = 0;
                    pokeFormsComboBoxes[partyIndex].Enabled = Species.HasMoreThanOneForm(pokeComboBoxes[partyIndex].SelectedIndex);
                }
            }
        }

        private void SetTrainerName()
        {
            trainer_NameTextBox.Text = SelectedTrainer.TrainerName;
        }

        private void SetTrainerParty()
        {
            for (int i = 0; i < SelectedTrainer.TrainerProperties.TeamSize; i++)
            {
                var species = GetSpeciesBySpeciesId(SelectedTrainer.TrainerParty.Pokemons[i].SpeciesId);
                pokeComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].PokemonId;
                pokeLevelNums[i].Value = SelectedTrainer.TrainerParty.Pokemons[i].Level;
                pokeDVNums[i].Value = SelectedTrainer.TrainerParty.Pokemons[i].DifficultyValue;
                pokeAbilityComboBoxes[i].Items.Clear();
                pokeFormsComboBoxes[i].Items.Clear();
                pokeHeldItemComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].HeldItemId ?? 0;
                pokeBallCapsuleComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].BallCapsuleId ?? 0;
                if (SelectedTrainer.TrainerProperties.ChooseMoves)
                {
                    pokeMoves[i] = new ushort[4];
                    pokeMoves[i][0] = SelectedTrainer.TrainerParty.Pokemons[i].Moves[0];
                    pokeMoves[i][1] = SelectedTrainer.TrainerParty.Pokemons[i].Moves[1];
                    pokeMoves[i][2] = SelectedTrainer.TrainerParty.Pokemons[i].Moves[2];
                    pokeMoves[i][3] = SelectedTrainer.TrainerParty.Pokemons[i].Moves[3];
                }

                if (LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver)
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
                            switch (SelectedTrainer.TrainerParty.Pokemons[i].GenderOverride)
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

                if (LoadedRom.GameFamily != GameFamily.DiamondPearl)
                {
                    SetPokemonForms(SelectedTrainer.TrainerParty.Pokemons[i].PokemonId, i);
                    pokeFormsComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].FormId;
                }

                pokeAbilityComboBoxes[i].Items.Add("-");

                if (species.Ability1 > 0)
                {
                    pokeAbilityComboBoxes[i].Items.Add(GetAbilityNameByAbilityId(species.Ability1));
                }
                if (species.Ability2 > 0)
                {
                    pokeAbilityComboBoxes[i].Items.Add(GetAbilityNameByAbilityId(species.Ability2));
                }
                pokeAbilityComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].AbilityOverride
                    switch
                {
                    AbilityOverride.None => 0,
                    AbilityOverride.Ability1 => 1,
                    AbilityOverride.Ability2 => 2,
                    _ => 0
                };
            }
        }

        private void SetTrainerProperties()
        {
            trainer_TeamSizeNum.Maximum = SelectedTrainer.TrainerProperties.DoubleBattle ? 3 : 6;
            trainer_TeamSizeNum.Value = SelectedTrainer.TrainerProperties.TeamSize == 0 ? 1 : SelectedTrainer.TrainerProperties.TeamSize;

            trainer_DblBattleCheckBox.Checked = SelectedTrainer.TrainerProperties.DoubleBattle;
            trainer_HeldItemsCheckbox.Checked = SelectedTrainer.TrainerProperties.ChooseItems;
            trainer_ChooseMovesCheckbox.Checked = SelectedTrainer.TrainerProperties.ChooseMoves;
            for (int i = 0; i < 4; i++)
            {
                trainerItemsComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerProperties.Items[i];
            }
            var trainerClass = GetTrainerClassByTrainerClassId(SelectedTrainer.TrainerProperties.TrainerClassId);
            trainer_ClassListBox.SelectedIndex = trainer_ClassListBox.Items.IndexOf(trainerClass.ListName);

            for (int i = 0; i < SelectedTrainer.TrainerProperties.AIFlags.Count; i++)
            {
                trainer_AiFlags_listbox.SetItemChecked(i, SelectedTrainer.TrainerProperties.AIFlags[i]);
            }
            if (SelectedTrainer.TrainerProperties.TeamSize < 1)
            {
                MessageBox.Show("This trainer does not currently have any Pokemon set.\nYou must set at least one Pokemon before use in-game", "Set a Party Pokemon", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (poke1GenderComboBox.Items.Count == 0 && LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver)
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
            if (poke1BallCapsuleComboBox.Items.Count == 0 && LoadedRom.GameFamily != GameFamily.DiamondPearl)
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
                else
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
                if (SelectedTrainer.TrainerId <= LoadedRom.VanillaTotalTrainers)
                {
                    MessageBox.Show("This is one of the game's core Trainers.\nYou cannot remove this file as it will cause issues.", "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        private void trainer_SaveBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            if (ValidateTrainerName() && ValidatePokemon() && ValidatePokemonMoves() && SaveTrainerName(SelectedTrainer.TrainerId) && SaveTrainerProperties(SelectedTrainer.TrainerId) && SaveTrainerParty(SelectedTrainer.TrainerId))
            {
                MessageBox.Show("Trainer Data updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            IsLoadingData = false;
        }

        private void trainer_SaveParty_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemon() && ValidatePokemonMoves())
            {
                if (SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked
                    && SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(SelectedTrainer.TrainerId, true);
                    }
                }
                else if (SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(SelectedTrainer.TrainerId, true);
                    }
                }
                else if (SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Property Data must also be saved.\n\nDo you want to save Trainer Property Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && SaveTrainerProperties(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerParty(SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerParty(SelectedTrainer.TrainerId, true);
                }
            }
        }

        private void trainer_SaveProperties_btn_Click(object sender, EventArgs e)
        {
            if (ValidatePokemonMoves())
            {
                if (SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked
                    && SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' and 'Choose Items' properties have been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(SelectedTrainer.TrainerId, true);
                    }
                }
                else if (SelectedTrainer.TrainerProperties.ChooseMoves != trainer_ChooseMovesCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Moves' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(SelectedTrainer.TrainerId, true);
                    }
                }
                else if (SelectedTrainer.TrainerProperties.ChooseItems != trainer_HeldItemsCheckbox.Checked)
                {
                    var savePokemonWarning = MessageBox.Show("This Trainer's 'Choose Items' property has been changed." +
                        "\nTrainer Party Data must also be saved.\n\nDo you want to save Trainer Party Data?", "Party Properties Changed",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (savePokemonWarning == DialogResult.Yes && ValidatePokemon() && SaveTrainerParty(SelectedTrainer.TrainerId))
                    {
                        SaveTrainerProperties(SelectedTrainer.TrainerId, true);
                    }
                }
                else
                {
                    SaveTrainerProperties(SelectedTrainer.TrainerId, true);
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

        private void trainer_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData && trainer_TrainersListBox.SelectedIndex > -1)
            {
                string selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();

                if (selectedTrainer != SelectedTrainer.ListName)
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
                            trainer_TrainersListBox.SelectedIndex = trainer_TrainersListBox.Items.IndexOf(SelectedTrainer.ListName);
                        }
                    }

                    if (!InhibitTrainerChange)
                    {
                        selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();
                        SelectedTrainer = trainerEditorMethods.GetTrainer(MainEditorModel.Trainers, Trainer.ListNameToTrainerId(selectedTrainer));

                        if (SelectedTrainer.TrainerId > 0)
                        {
                            PopulateTrainerData();
                            PopulatePartyData();
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

        private void UndoTrainerChanges()
        {
            IsLoadingData = true;
            SetTrainerName();
            SetTrainerProperties();
            InitializePartyEditor();
            SetTrainerParty();
            EditedTrainerData(false);
            EditedTrainerProperty(false);
            EditedTrainerParty(false);
            IsLoadingData = false;
        }

        private void UndoTrainerPartyChanges()
        {
            IsLoadingData = true;
            InitializePartyEditor();
            SetTrainerParty();
            EditedTrainerParty(false);
            IsLoadingData = false;
        }

        private void UndoTrainerPropertyChanges()
        {
            IsLoadingData = true;
            SetTrainerProperties();
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
            else if (trainer_NameTextBox.Text.Length > 10 && !LoadedRom.TrainerNameExpansion)
            {
                trainer_NameTextBox.BackColor = Color.PaleVioletRed;
                MessageBox.Show("Trainer name cannot be longer than 10 characters.\n\nYou can expand this to 16 characters by applying the Trainer Names Expansion patch.", "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            else if (trainer_NameTextBox.Text.Length > 16 && LoadedRom.TrainerNameExpansion)
            {
                trainer_NameTextBox.BackColor = Color.PaleVioletRed;
                MessageBox.Show("Trainer name cannot be longer than 16 characters.", "Unable to Save Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}