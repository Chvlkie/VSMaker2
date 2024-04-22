using Main.Forms;
using Main.Models;
using System.Text;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.Methods;
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
        private ITrainerEditorMethods trainerEditorMethods;
        private IFileSystemMethods fileSystemMethods;

        #endregion Methods

        private bool IsLoadingData;
        private RomFile LoadedRom;
        private MainEditorModel MainEditorModel;
        private bool RomLoaded;

        public Mainform()
        {
            InitializeComponent();
            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            RomLoaded = false;
            romName_Label.Text = "";
            MainEditorModel = new();
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
            trainerEditorMethods = new TrainerEditorMethods();
            fileSystemMethods = new FileSystemMethods();
        }

        #region MainMenu

        public void BeginUnpackNarcs(IProgress<int> progress)
        {
            var narcs = GameFamilyNarcs.GetGameFamilyNarcs(LoadedRom.GameFamily);

            var (success, exception) = romFileMethods.UnpackNarcs(narcs, progress);
            if (!success)
            {
                MessageBox.Show(exception, "Unable to Unpack NARCs", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progress?.Report(100);
                return;
            }
            progress?.Report(100);
        }

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

        private static void CreateDirectory(string workingDirectory)
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

        private static (byte EuropeByte, string GameCode) LoadInitialRomData(string filePath)
        {
            using DSUtils.EasyReader reader = new(filePath, 0xC);
            string gameCode = Encoding.UTF8.GetString(reader.ReadBytes(4));
            reader.BaseStream.Position = 0x1E;
            byte europeByte = reader.ReadByte();
            reader.Close();
            return (europeByte, gameCode);
        }

        private void BeginExtractRomData()
        {
            OpenLoadingDialog(LoadType.UnpackNarcs);
        }

        private void ClearUnsavedChanges()
        {
            ClearUnsavedTrainerChanges();
        }

        private void CloseProject()
        {
            romName_Label.Text = "";
            EditTrainer = new();
            LoadedRom = new RomFile();
            IsLoadingData = false;
            RomLoaded = false;
            startupTab.SelectedTab = startupPage;
            MainEditorModel = new();
            EnableDisableMenu(false);
            ClearUnsavedChanges();
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
            main_OpenPatchesBtn.Enabled = enable;
            main_UnpackNarcsBtn.Enabled = enable;
        }

        private void EndOpenRom()
        {
            IsLoadingData = false;
            EnableDisableMenu(RomLoaded);
            startupTab.SelectedTab = RomLoaded ? mainPage : startupPage;
            if (RomLoaded)
            {
                InitializeTrainerEditor();
                main_MainTab.SelectedTab = main_MainTab_TrainerTab;
                SetupTrainerEditor();
            }
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
                    EndOpenRom();
                }
            }
            else
            {
                SelectExtractedRomFolder();
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
                        InitializeTrainerEditor();
                    }
                    EndOpenRom();
                }
            }
            else
            {
                OpenRom();
                if (RomLoaded)
                {
                    InitializeTrainerEditor();
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
                        InitializeTrainerEditor();
                    }
                    EndOpenRom();
                }
            }
            else
            {
                SelectExtractedRomFolder();
                if (RomLoaded)
                {
                    InitializeTrainerEditor();
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
                        InitializeTrainerEditor();
                    }
                    EndOpenRom();
                }
            }
            else
            {
                OpenRom();
                if (RomLoaded)
                {
                    InitializeTrainerEditor();
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
                Filter = Common.NdsRomFilter
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

            string fileName = Directory.GetFiles(selectedFolder).SingleOrDefault(x => x.Contains(Common.HeaderFilePath));
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
                    "\n\nVSMaker 2 currently only support Pokémon Diamond, Pearl, Platinum, HeartGold or Soul Silver version."
                    , "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            LoadedRom.GameFamily = romFileMethods.SetGameFamily(LoadedRom.GameVersion);
            LoadedRom.GameLanguage = romFileMethods.SetGameLanguage(LoadedRom.GameCode);
            romFileMethods.SetNarcDirectories(workingDirectory, LoadedRom.GameVersion, LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.TrainerNamesTextNumber = romFileMethods.SetTrainerNameTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
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
                string workingDirectory = $"{selectFolder.SelectedPath}\\{Path.GetFileNameWithoutExtension(fileName)}{Common.VsMakerContentsFolder}\\";
                if (Directory.Exists(workingDirectory))
                {
                    var directoryExists = MessageBox.Show("An extracted contents folder for this ROM has been found." +
                        "\n\nDo you wish to open this folder?", "Extracted ROM Data exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (directoryExists == DialogResult.Yes)
                    {
                        CloseProject();
                        RomLoaded = ReadRomExtractedFolder(workingDirectory);
                        if (RomLoaded)
                        {
                            BeginExtractRomData();
                        }
                        else
                        {
                            return;
                        }
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

        private void main_MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (main_MainTab.SelectedTab == main_MainTab_TrainerTab)
            {
                SetupTrainerEditor();
            }
        }

        private void FilterListBox(ListBox listBox, string filter, List<string> unfiltered)
        {
            List<string> filteredList = [];
            foreach (string item in unfiltered)
            {
                string name = item.ToLower();
                if (name.Contains(filter.ToLower()))
                {
                    filteredList.Add(item);
                }
            }

            listBox.Items.Clear();
            foreach (string item in filteredList)
            {
                listBox.Items.Add(item);
            }
        }

        #endregion MainMenu

        #region TrainerEditor

        private bool ShowPlayerWarning = true;
        private Trainer EditTrainer = new();
        private bool TrainerBattleMessagesChange;
        private bool TrainerDataChange;
        private bool TrainerPartyChange;
        private bool TrainerPropertyChange;
        private List<string> UnfilteredTrainers = [];
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
            trainer_ViewClassBtn.Enabled = true;
            trainer_NameTextBox.Enabled = true;
            trainer_PropertiesTabControl.Enabled = true;
        }

        private void SetupTrainerEditor()
        {
            IsLoadingData = true;

            var trainers = trainerEditorMethods.GetTrainers(LoadedRom.TrainerNamesTextNumber);
            var trainerEditorModel = new TrainerEditorModel()
            {
                Trainers = trainers
            };

            MainEditorModel.TrainerEditor = trainerEditorModel;
            PopulateTrainerList(trainers);
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
            if (!IsLoadingData)
            {
                EditTrainer.TrainerName = trainer_NameTextBox.Text;
                EditedTrainerData(EditTrainer.TrainerName != MainEditorModel.TrainerEditor.TrainerData.SelectedTrainer.TrainerName);
            }
        }

        private void trainer_TeamSizeNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditTrainer.TrainerParty.PartySize = (ushort)trainer_TeamSizeNum.Value;
                EditedTrainerParty(EditTrainer.TrainerParty.PartySize != MainEditorModel.TrainerEditor.TrainerData.SelectedTrainer.TrainerParty.PartySize);
            }
        }

        private void trainer_FilterBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (string.IsNullOrEmpty(trainer_FilterBox.Text))
                {
                    PopulateTrainerList(MainEditorModel.TrainerEditor.Trainers);
                    trainer_ClearFilterBtn.Enabled = false;
                }
                else
                {
                    FilterListBox(trainer_TrainersListBox, trainer_FilterBox.Text, UnfilteredTrainers);
                    trainer_ClearFilterBtn.Enabled = true;
                }
            }
        }

        private void trainer_TrainersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                string selectedTrainer = trainer_TrainersListBox.SelectedItem.ToString();
                MainEditorModel.TrainerEditor.SelectedTrainerId = Trainer.ListNameToTrainerId(selectedTrainer);

                if (MainEditorModel.TrainerEditor.SelectedTrainerId > 0)
                {
                    PopulateTrainerData();
                    EnableTrainerEditor();
                }
            }
        }

        private void trainer_RemoveBtn_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (MainEditorModel.TrainerEditor.SelectedTrainerId <= LoadedRom.VanillaTotalTrainers)
                {
                    MessageBox.Show("This is one of the game's core Trainers.\nYou cannot remove this file as it will cause issues.", "Unable to Remove Trainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        private ViewTrainerDataModel GetTrainerData(int trainerId)
        {
            return new ViewTrainerDataModel
            {
                SelectedTrainer = MainEditorModel.TrainerEditor.Trainers.SingleOrDefault(x => x.TrainerId == trainerId)
            };
        }

        private void PopulateTrainerData()
        {
            IsLoadingData = true;
            MainEditorModel.TrainerEditor.TrainerData = GetTrainerData(MainEditorModel.TrainerEditor.SelectedTrainerId);
            var selectedTrainer = MainEditorModel.TrainerEditor.TrainerData.SelectedTrainer;
            trainer_NameTextBox.Text = selectedTrainer.TrainerName;
            IsLoadingData = false;
        }

        private void trainer_ClearFilterBtn_Click(object sender, EventArgs e)
        {
            trainer_FilterBox.Text = "";
        }

        #endregion TrainerEditor

        #region ClassEditor

        private bool UnsavedClassChanges;

        #endregion ClassEditor

        #region BattleMessageEditor

        private bool UnsavedBattleMessageChanges;

        #endregion BattleMessageEditor

        private void menu_Export_Trainers_Click(object sender, EventArgs e)
        {
        }

        private void exportAstrainersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var save = new SaveFileDialog
            {
                Filter = "VS Maker Trainers|*.vstrainers",
                Title = "Export VS Maker Trainers"
            };
            save.ShowDialog();
            if (!string.IsNullOrEmpty(save.FileName))
            {
                var export = fileSystemMethods.ExportTrainers(MainEditorModel.TrainerEditor.Trainers, save.FileName);
                if (export.Success)
                {
                    MessageBox.Show("VS Maker Trainers exported!", "Success");
                }
                else
                {
                    MessageBox.Show(export.ErrorMessage, "Unable to Export VS Maker Trainers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}