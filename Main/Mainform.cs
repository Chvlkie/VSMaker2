using Data;
using Main.Forms;

namespace Main
{
    public partial class Mainform : Form
    {
        #region Forms

        private LoadingData LoadingData;
        private MoveSelector MoveSelector;
        private RomPatches RomPatches;
        private Settings Settings;

        #endregion Forms

        private bool IsLoadingData;

        public const string NdsRomFilter = "NDS File (*.nds)|*.nds";
        public Mainform()
        {
            InitializeComponent();
            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
        }

        private bool UnsavedChanges => UnsavedTrainerEditorChanges || UnsavedClassChanges || UnsavedBattleMessageChanges;

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure you want to close VS Maker 2?" +
                "\n\nAny unsaved changes will be lost.", "Close VS Maker 2", MessageBoxButtons.YesNo);
            if (dialogResult != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            Settings = new Settings();
            MoveSelector = new MoveSelector();
            LoadingData = new LoadingData();
            RomPatches = new RomPatches();
        }

        #region MainMenu

        private async void OpenRom()
        {
            OpenFileDialog openRom = new()
            {
                Filter = NdsRomFilter
            };
        }
        private void main_OpenPatchesBtn_Click(object sender, EventArgs e)
        {
            OpenRomPatchesWindow();
        }

        private void main_SettingsBtn_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }

        private void menu_Tools_RomPatcher_Click(object sender, EventArgs e)
        {
            OpenRomPatchesWindow();
        }

        private void menu_Tools_Settings_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }

        private void menu_File_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void main_OpenRomBtn_Click(object sender, EventArgs e)
        {
            OpenRom();
            IsLoadingData = false;
        }

        private void OpenRomPatchesWindow()
        {
            RomPatches.ShowDialog();
        }

        private void OpenSettingsWindow()
        {
            Settings.ShowDialog();
        }

        #endregion MainMenu

        #region TrainerEditor

        private Trainer CurrentTrainer = new();
        private Trainer EditTrainer = new();
        private bool TrainerBattleMessagesChange;
        private bool TrainerDataChange;
        private bool TrainerPartyChange;
        private bool TrainerPropertyChange;
        private bool UnsavedTrainerEditorChanges => TrainerDataChange || TrainerPartyChange || TrainerPropertyChange || TrainerBattleMessagesChange;

        #region PartyEditor

        private List<ComboBox> pokeAbilityComboBoxes;
        private List<ComboBox> pokeBalLSealComboBoxes;
        private List<ComboBox> pokeComboBoxes;
        private List<NumericUpDown> pokeDVNums;
        private List<ComboBox> pokeFormsComboBoxes;
        private List<ComboBox> pokeGenderComboBoxes;
        private List<ComboBox> pokeHeldItemComboBoxes;
        private List<PictureBox> pokeIconsPictureBoxes;
        private List<NumericUpDown> pokeLevelNums;
        private List<Button> pokeMoveButtons;

        #endregion PartyEditor

        private void EditedTrainerBattleMessages(bool hasChanges)
        {
            TrainerBattleMessagesChange = hasChanges;
            main_MainTab_TrainerTab.Text = hasChanges ? "Trainers *" : "Trainers";
            trainer_BattleMessageTab.Text = hasChanges ? "Battle Messages *" : "Battle";
        }

        private void EditedTrainerData(bool hasChanges)
        {
            TrainerDataChange = hasChanges;
            main_MainTab_TrainerTab.Text = hasChanges ? "Trainers *" : "Trainers";
        }

        private void EditedTrainerParty(bool hasChanges)
        {
            TrainerPartyChange = hasChanges;
            main_MainTab_TrainerTab.Text = hasChanges ? "Trainers *" : "Trainers";
            trainer_PartyTab.Text = hasChanges ? "Party *" : "Party";
        }

        private void EditedTrainerProperty(bool hasChanges)
        {
            TrainerPropertyChange = hasChanges;
            main_MainTab_TrainerTab.Text = hasChanges ? "Trainers *" : "Trainers";
            trainer_PropertiesTab.Text = hasChanges ? "Properties *" : "Properties";
        }

        private void SetupPartyEditor()
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
        }

        private void trainer_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            EditTrainer.TrainerName = trainer_NameTextBox.Text;
            EditedTrainerData(EditTrainer.TrainerName != CurrentTrainer.TrainerName);
        }

        private void trainer_TeamSizeNum_ValueChanged(object sender, EventArgs e)
        {
            EditTrainer.TrainerParty.PartySize = (ushort)trainer_TeamSizeNum.Value;
            EditedTrainerParty(EditTrainer.TrainerParty.PartySize != CurrentTrainer.TrainerParty.PartySize);
        }

        #endregion TrainerEditor

        #region ClassEditor

        private bool UnsavedClassChanges;

        #endregion ClassEditor

        #region BattleMessageEditor

        private bool UnsavedBattleMessageChanges;

        #endregion BattleMessageEditor

       
    }
}