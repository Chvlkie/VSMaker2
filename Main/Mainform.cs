using Main.Forms;
using System.Text;
using VsMaker2Core.DataModels;
using VsMaker2Core.Glossary;
using VsMaker2Core.Methods.Rom;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static VsMaker2Core.Enums;

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

        #region Methods

        private IRomFileMethods romFileMethods;

        #endregion Methods

        public const string NdsRomFilter = "NDS File (*.nds)|*.nds";
        private bool IsLoadingData;
        private RomFile LoadedRom;
        private bool RomLoaded;

        public Mainform()
        {
            InitializeComponent();
            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            RomLoaded = false;
            romName_Label.Text = "";
        }

        private bool UnsavedChanges => UnsavedTrainerEditorChanges || UnsavedClassChanges || UnsavedBattleMessageChanges;

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            string closeMessage = RomLoaded ? "Do you wish to close VS-Maker?\n\nAny unsaved changes will be lost." : "Do you wish to close VS-Maker?";
            if (MessageBox.Show(closeMessage, "Close VS-Maker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
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
            romFileMethods = new RomFileMethods();
        }

        #region MainMenu

        public void BeginUnpackRomData()
        {
            var (success, exception) = romFileMethods.ExtractRomContents(LoadedRom.WorkingDirectory, LoadedRom.FileName);
            if (!success)
            {
                MessageBox.Show($"{exception}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseProject();
                return;
            }
            RomLoaded = true;
        }

        private void BeginExtractRomData()
        {
            OpenLoadingDialog(LoadType.UnpackNarcs);
        }

        public void BeginUnpackNarcs(IProgress<int> progress)
        {
            var narcs = new List<NarcDirectory>();
            switch (LoadedRom.GameFamily)
            {
                case GameFamily.DiamondPearl:
                    narcs = [
                        NarcDirectory.PokemonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchive,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                        ];
                    break;

                case GameFamily.Platinum:
                    narcs = [
                        NarcDirectory.PokemonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchive,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                        ];
                    break;

                case GameFamily.HeartGoldSoulSilver:
                    narcs = [
                        NarcDirectory.BattleStagePokeData,
                        NarcDirectory.BattleTowerPokeData,
                        NarcDirectory.BattleTowerTrainerData,
                        NarcDirectory.PokemonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchive,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                       ];
                    break;

                case GameFamily.HgEngine:
                    narcs = [
                       NarcDirectory.BattleStagePokeData,
                        NarcDirectory.BattleTowerPokeData,
                        NarcDirectory.BattleTowerTrainerData,
                        NarcDirectory.PokemonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchive,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                      ];
                    break;

                default:
                    break;
            }

            var (success, exception) = romFileMethods.UnpackNarcs(LoadedRom, narcs, progress);
            if (!success)
            {
                MessageBox.Show(exception, "Unable to Unpack NARCs", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progress?.Report(100);
                return;
            }
            progress?.Report(100);
        }

        private void ClearUnsavedChanges()
        {
            ClearUnsavedTrainerChanges();
        }

        private void CloseProject()
        {
            Controls.Clear();
            FormClosing -= Mainform_FormClosing;
            InitializeComponent();
            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            romName_Label.Text = "";
            CurrentTrainer = new();
            EditTrainer = new();
            LoadedRom = new RomFile();
            IsLoadingData = false;
            RomLoaded = false;
            startupTab.SelectedTab = startupPage;
            EnableDisableMenu(false);
            ClearUnsavedChanges();
        }

        private void CreateDirectory(string workingDirectory)
        {
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            else
            {
                MessageBox.Show("Unable to extract contents.\n\n" +
                    "Contents folder may already exist", "Unable to Extract ROM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void EnableDisableMenu(bool enable)
        {
            main_SaveRomBtn.Enabled = enable;
            main_UnpackNarcsBtn.Enabled = enable;

            menu_File_Save.Enabled = enable;
            menu_File_SaveAs.Enabled = enable;
            menu_File_Close.Enabled = enable;
            menu_Import.Enabled = enable;
            menu_Export.Enabled = enable;
            menu_Tools_RomPatcher.Enabled = enable;
            menu_Tools_UnpackNarcs.Enabled = enable;
        }

        private void EndOpenRom()
        {
            IsLoadingData = false;
            EnableDisableMenu(RomLoaded);
            startupTab.SelectedTab = RomLoaded ? mainPage : startupPage;
        }

        private (byte EuropeByte, string GameCode) LoadInitialRomData(string filePath)
        {
            using DSUtils.EasyReader reader = new(filePath, 0xC);
            string gameCode = Encoding.UTF8.GetString(reader.ReadBytes(4));
            reader.BaseStream.Position = 0x1E;
            byte europeByte = reader.ReadByte();
            reader.Close();
            return (europeByte, gameCode);
        }

        private void main_OpenFolderBtn_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    SelectExtractedRomFolder();
                    if (RomLoaded)
                    {
                    }
                    EndOpenRom();
                }
            }
            else
            {
                SelectExtractedRomFolder();
                if (RomLoaded)
                {
                }
                EndOpenRom();
            }
        }

        private void main_OpenPatchesBtn_Click(object sender, EventArgs e)
        {
            OpenRomPatchesWindow();
        }

        private void main_OpenRomBtn_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    OpenRom();
                    if (RomLoaded)
                    {
                    }
                    EndOpenRom();
                }
            }
            else
            {
                OpenRom();
                if (RomLoaded)
                {
                }
                EndOpenRom();
            }
        }

        private void main_SettingsBtn_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }

        private void menu_File_Close_Click(object sender, EventArgs e)
        {
            var confirmClose = MessageBox.Show("Are you sure you want to close this project?", "Close Project", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmClose == DialogResult.Yes)
            {
                if (UnsavedChanges)
                {
                    var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (saveChanges == DialogResult.Yes)
                    {
                        CloseProject();
                    }
                    else
                    {
                        return;
                    }
                }
                CloseProject();
            }
        }

        private void menu_File_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menu_File_OpenFolder_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    SelectExtractedRomFolder();
                    if (RomLoaded)
                    {
                    }
                    EndOpenRom();
                }
            }
            else
            {
                SelectExtractedRomFolder();
                if (RomLoaded)
                {
                }
                EndOpenRom();
            }
        }

        private void menu_File_OpenRom_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    OpenRom();
                    if (RomLoaded)
                    {
                    }
                    EndOpenRom();
                }
            }
            else
            {
                OpenRom();
                if (RomLoaded)
                {
                }
                EndOpenRom();
            }
        }

        private void menu_Tools_RomPatcher_Click(object sender, EventArgs e)
        {
            OpenRomPatchesWindow();
        }

        private void menu_Tools_Settings_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }

        private void OpenLoadingDialog(LoadType loadType)
        {
            UseWaitCursor = true;
            LoadingData = new(this, loadType);
            LoadingData.ShowDialog();
            UseWaitCursor = false;
        }

        private void OpenRom()
        {
            OpenFileDialog openRom = new()
            {
                Filter = NdsRomFilter
            };

            if (openRom.ShowDialog(this) == DialogResult.OK)
            {
                SelectWorkingFolderDirectory(openRom.FileName);
                if (RomLoaded)
                {
                    BeginExtractRomData();
                }
            }
        }

        private void OpenRomPatchesWindow()
        {
            RomPatches.ShowDialog();
        }

        private void OpenSettingsWindow()
        {
            Settings.ShowDialog();
        }

        private bool ReadRomExtractedFolder(string selectedFolder)
        {
            if (string.IsNullOrEmpty(selectedFolder))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                    "\n\nPlease ensure you select an extracted ROM contents folder" +
                    "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string fileName = Directory.GetFiles(selectedFolder).SingleOrDefault(x => x.Contains(GlobalConstants.HeaderFilePath));
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                    "\n\nPlease ensure you select an extracted ROM contents folder" +
                    "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return ReadRomFile(selectedFolder + "\\", fileName);
        }

        private bool ReadRomFile(string workingDirectory, string fileName)
        {
            IsLoadingData = true;
            var loadedRom = LoadInitialRomData(fileName);
            LoadedRom = new RomFile(loadedRom.GameCode, fileName, workingDirectory, loadedRom.EuropeByte);
            if (LoadedRom.GameVersion == GameVersion.Unknown)
            {
                MessageBox.Show("The ROM file you have selected is not supported by VSMaker 2." +
                    "\n\nVSMaker 2 currently only support Pokémon Diamond, Pearl, Platinum, HeartGold or Sould Silver version."
                    , "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            LoadedRom.GameFamily = romFileMethods.SetGameFamily(LoadedRom.GameVersion);
            LoadedRom.GameLanguage = romFileMethods.SetGameLanguage(LoadedRom.GameCode);
            LoadedRom.Directories = romFileMethods.SetNarcDirectories(workingDirectory, LoadedRom.GameVersion, LoadedRom.GameFamily, LoadedRom.GameLanguage);
            return true;
        }

        private void SelectExtractedRomFolder()
        {
            FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Extracted ROM Contents Folder",
                UseDescriptionForTitle = true
            };

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                RomLoaded = ReadRomExtractedFolder(selectFolder.SelectedPath);
                if (RomLoaded)
                {
                    BeginExtractRomData();
                }
            }
        }

        private void SelectWorkingFolderDirectory(string fileName)
        {
            FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Where to Extract ROM Contents",
                UseDescriptionForTitle = true,
            };

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                string workingDirectory = $"{selectFolder.SelectedPath}\\{Path.GetFileNameWithoutExtension(fileName)}{GlobalConstants.VsMakerContentsFolder}\\";
                if (Directory.Exists(workingDirectory))
                {
                    var directoryExists = MessageBox.Show("An extracted contents folder for this ROM has been found." +
                        "\n\nDo you wish to open this folder?", "Extracted ROM Data exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (directoryExists == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        var extractContentsAgain = MessageBox.Show("ROM Data will be re-extracted.\nThis will delete all existing data in the old folder." +
                            "\n\nDo you wish to proceed?", "Re-Extract ROM Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (extractContentsAgain == DialogResult.Yes)
                        {
                            Directory.Delete(workingDirectory, true);
                            CreateDirectory(workingDirectory);
                        }
                        else
                        {
                            SelectWorkingFolderDirectory(fileName);
                        }
                    }
                }
                else
                {
                    CreateDirectory(workingDirectory);
                }

                if (Directory.Exists(workingDirectory))
                {
                    if (ReadRomFile(workingDirectory, fileName))
                    {
                        OpenLoadingDialog(LoadType.UnpackRom);
                    }
                    else
                    {
                        CloseProject();
                    }
                }
                else
                {
                    CloseProject();
                }
            }
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