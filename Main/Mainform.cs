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

        private IClassEditorMethods classEditorMethods;
        private IFileSystemMethods fileSystemMethods;
        private IRomFileMethods romFileMethods;
        private ITrainerEditorMethods trainerEditorMethods;

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

        private void class_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                class_DescriptionShowNameTextLbl.Text = class_NameTextBox.Text;
            }
        }

        private void ConfirmImportTrainers(List<Trainer> newTrainers, List<Trainer> oldTrainers)
        {
            int newTrainerCount = newTrainers.Count;
            int oldTrainerCount = oldTrainers.Count;
            int trainerCountDiff = newTrainerCount - oldTrainerCount;
            if (newTrainerCount > oldTrainerCount)
            {
                var confirm = MessageBox.Show("This will create " + trainerCountDiff + " new Trainers.\n\nContinue?", "Import Trainer Files", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (confirm != DialogResult.Yes)
                {
                    confirm = MessageBox.Show("Do you wish to cancel importing Trainer Data?", "Cancel Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        ConfirmImportTrainers(newTrainers, oldTrainers);
                    }
                }
            }
            else if (oldTrainerCount > newTrainerCount)
            {
                if (trainerCountDiff < 0)
                {
                    trainerCountDiff *= -1;
                }

                var confirm = MessageBox.Show("This will remove " + trainerCountDiff + " Trainers.\n\nContinue?", "Import Trainer Files", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (confirm != DialogResult.Yes)
                {
                    confirm = MessageBox.Show("Do you wish to cancel importing Trainer Data?", "Cancel Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        ConfirmImportTrainers(newTrainers, oldTrainers);
                    }
                }
            }
            else
            {
                var confirm = MessageBox.Show("This will replace all " + oldTrainerCount + " Trainers.\n\nContinue?", "Import Trainer Files", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (confirm != DialogResult.Yes)
                {
                    confirm = MessageBox.Show("Do you wish to cancel importing Trainer Data?", "Cancel Import", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirm == DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        ConfirmImportTrainers(newTrainers, oldTrainers);
                    }
                }
            }
        }

        private bool ConfirmUnsavedChanges()
        {
            var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                    "Any unsaved changes will be lost.\n" +
                    "Continue", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return saveChanges == DialogResult.Yes;
        }

        private void exportAstrainersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var save = new SaveFileDialog
            //{
            //    Filter = "VS Maker Trainers|*.vstrainers",
            //    Title = "Export VS Maker Trainers"
            //};
            //save.ShowDialog();
            //if (!string.IsNullOrEmpty(save.FileName))
            //{
            //    var export = fileSystemMethods.ExportTrainers(MainEditorModel.Trainers, save.FileName, LoadedRom.GameFamily, LoadedRom.TrainerNamesTextNumber, 100, 100);
            //    if (export.Success)
            //    {
            //        MessageBox.Show("VS Maker Trainers exported!", "Success");
            //    }
            //    else
            //    {
            //        MessageBox.Show(export.ErrorMessage, "Unable to Export VS Maker Trainers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    }
            //}
        }

        private string GetPokemonNameById(int pokemonId)
        {
            return MainEditorModel.PokemonNames[pokemonId];
        }

        private void importVSMakerTrainersvstrainersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirmImport = MessageBox.Show("This will overwrite all current Trainer Data." +
                "\n\nDo you wish to continue?", "Import Trainer Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmImport == DialogResult.Yes)
            {
                var open = new OpenFileDialog
                {
                    Filter = "VS Maker Trainers|*.vstrainers",
                    Title = "Import VS Maker Trainers"
                };

                if (open.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(open.FileName))
                {
                    var import = fileSystemMethods.ImportTrainers(open.FileName);

                    if (import.Success)
                    {
                        var gameFamily = import.VsTrainersFile.GameFamily;
                        if (gameFamily != LoadedRom.GameFamily)
                        {
                            var confirm = MessageBox.Show("The selected VsTrainers file was exported from " + gameFamily
                                 + ".\nLoaded ROM is " + LoadedRom.GameFamily +
                                 "\n\nThe Trainer Data may cause issues with Loaded ROM.", "Game Family Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            if (confirm != DialogResult.OK)
                            {
                                return;
                            }
                        }
                        var newTrainers = import.VsTrainersFile.TrainerData;
                        var oldTrainers = MainEditorModel.Trainers;

                        ConfirmImportTrainers(newTrainers, oldTrainers);
                    }
                    else
                    {
                        MessageBox.Show(import.ErrorMessage, "Unable to Import VS Maker Trainers", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

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
            LoadingData = new LoadingData();
            RomPatches = new RomPatches();
            romFileMethods = new RomFileMethods();
            trainerEditorMethods = new TrainerEditorMethods();
            classEditorMethods = new ClassEditorMethods();
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

            menu_File_Save.Enabled = enable;
            menu_File_SaveAs.Enabled = enable;
            menu_File_Close.Enabled = enable;
            menu_Import.Enabled = enable;
            menu_Export.Enabled = enable;
            menu_Tools_RomPatcher.Enabled = enable;
            main_OpenPatchesBtn.Enabled = enable;
        }

        private void EndOpenRom()
        {
            IsLoadingData = false;
            EnableDisableMenu(RomLoaded);
            startupTab.SelectedTab = RomLoaded ? mainPage : startupPage;
            if (RomLoaded)
            {
                InitializeTrainerEditor();
                GetInitialData();
                main_MainTab.SelectedTab = main_MainTab_TrainerTab;
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

        private void GetInitialData()
        {
            IsLoadingData = true;
            MainEditorModel.PokemonSpecies = romFileMethods.GetSpecies();
            MainEditorModel.Trainers = trainerEditorMethods.GetTrainers(LoadedRom.TrainerNamesTextNumber, LoadedRom.GameFamily);
            MainEditorModel.Classes = classEditorMethods.GetTrainerClasses(LoadedRom.ClassNamesTextNumber);
            MainEditorModel.PokemonNames = romFileMethods.GetPokemonNames(LoadedRom.PokemonNamesTextNumber);
            MainEditorModel.MoveNames = romFileMethods.GetMoveNames(LoadedRom.MoveNameTextNumber);
            IsLoadingData = false;
        }

        private void main_MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (main_MainTab.SelectedTab == main_MainTab_TrainerTab)
            {
                SetupTrainerEditor();
            }
            else if (main_MainTab.SelectedTab == main_MainTab_ClassTab)
            {
                SetupClassEditor();
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

            if (openRom.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(openRom.FileName))
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
            LoadedRom.PokemonNamesTextNumber = romFileMethods.SetPokemonNameArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.TrainerNamesTextNumber = romFileMethods.SetTrainerNameTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.ClassNamesTextNumber = romFileMethods.SetClassNameTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.ClassDescriptionMessageNumber = romFileMethods.SetClassDescriptionTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.BattleMessageTextNumber = romFileMethods.SetBattleMessageTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
            LoadedRom.MoveNameTextNumber = romFileMethods.SetMoveNameTextArchiveNumber(LoadedRom.GameFamily, LoadedRom.GameLanguage);
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

        #endregion MainMenu

        #region TrainerEditor

        private bool InhibitTrainerChange = false;
        private Trainer SelectedTrainer = new();
        private bool ShowPlayerWarning = true;
        private bool TrainerBattleMessagesChange;
        private bool TrainerDataChange;
        private bool TrainerPartyChange;
        private bool TrainerPropertyChange;
        private List<string> UnfilteredTrainers = [];
        private bool UnsavedTrainerEditorChanges => TrainerDataChange || TrainerPartyChange || TrainerPropertyChange || TrainerBattleMessagesChange;

        #region PartyEditor

        public ushort[] poke1Moves;
        public ushort[] poke2Moves;
        public ushort[] poke3Moves;
        public ushort[] poke4Moves;
        public ushort[] poke5Moves;
        public ushort[] poke6Moves;
        private List<ComboBox> pokeAbilityComboBoxes;
        private List<ComboBox> pokeBallSealComboBoxes;
        private List<ComboBox> pokeComboBoxes;
        private List<NumericUpDown> pokeDVNums;
        private List<ComboBox> pokeFormsComboBoxes;
        private List<ComboBox> pokeGenderComboBoxes;
        private List<ComboBox> pokeHeldItemComboBoxes;
        private List<PictureBox> pokeIconsPictureBoxes;
        private List<NumericUpDown> pokeLevelNums;
        private List<Button> pokeMoveButtons;
        private List<ushort[]> pokeMoves;

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

        private void EnableDisableParty(int partySize, bool chooseItems, bool chooseMoves)
        {
            // Firstly Disable All
            pokeComboBoxes.ForEach(x => x.Enabled = false);
            pokeIconsPictureBoxes.ForEach(x => x.Enabled = false);
            pokeLevelNums.ForEach(x => x.Enabled = false);
            pokeDVNums.ForEach(x => x.Enabled = false);
            pokeBallSealComboBoxes.ForEach(x => x.Enabled = false);
            pokeAbilityComboBoxes.ForEach(x => x.Enabled = false);
            pokeFormsComboBoxes.ForEach(x => x.Enabled = false);
            pokeHeldItemComboBoxes.ForEach(x => x.Enabled = false);
            pokeMoveButtons.ForEach(x => x.Enabled = false);
            pokeGenderComboBoxes.ForEach(x => x.Enabled = false);
            // Enable by Party Size
            for (int i = 0; i < partySize; i++)
            {
                pokeComboBoxes[i].Enabled = true;
                pokeIconsPictureBoxes[i].Enabled = true;
                pokeLevelNums[i].Enabled = true;
                pokeDVNums[i].Enabled = true;
                pokeBallSealComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl;
                pokeAbilityComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl;
                pokeFormsComboBoxes[i].Enabled = LoadedRom.GameFamily != GameFamily.DiamondPearl;
                pokeHeldItemComboBoxes[i].Enabled = chooseItems;
                pokeMoveButtons[i].Enabled = chooseMoves;
                pokeGenderComboBoxes[i].Enabled = LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver
                    && GetSpeciesBySpeciesId(SelectedTrainer.TrainerParty.Pokemons[i].SpeciesId).HasMoreThanOneGender;
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
            trainer_ViewClassBtn.Enabled = true;
            trainer_NameTextBox.Enabled = true;
            trainer_PropertiesTabControl.Enabled = true;
        }

        private void InitializeAiFlags()
        {
            trainer_AiFlags_listbox.Items.Add("Basic");
            trainer_AiFlags_listbox.Items.Add("Evaluate Attack");
            trainer_AiFlags_listbox.Items.Add("Expert");
            trainer_AiFlags_listbox.Items.Add("Status Effects");
            trainer_AiFlags_listbox.Items.Add("Risky");
            trainer_AiFlags_listbox.Items.Add("Damage Priority");
            trainer_AiFlags_listbox.Items.Add("Baton Pass");
            trainer_AiFlags_listbox.Items.Add("Tag Team");
            trainer_AiFlags_listbox.Items.Add("Check HP");
            trainer_AiFlags_listbox.Items.Add("Weather Effects");
            trainer_AiFlags_listbox.Items.Add("???");
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

        private void PopulateClassList(List<TrainerClass> classes)
        {
            class_ClassListBox.Items.Clear();
            UnfilteredClasses = [];
            foreach (var item in classes)
            {
                class_ClassListBox.Items.Add(item.ListName);
                UnfilteredClasses.Add(item.ListName);
            }

            trainer_AddTrainerBtn.Enabled = true;
            trainer_ImportAllBtn.Enabled = true;
            trainer_ExportAllBtn.Enabled = true;
            trainer_FilterBox.Enabled = true;
            trainer_TrainersListBox.Enabled = true;
        }

        private void PopulatePartyData()
        {
            IsLoadingData = true;
            // Initialize Pokemon Data
            pokeComboBoxes.ForEach(x => x.SelectedIndex = 0);
            pokeLevelNums.ForEach(x => x.Value = 1);
            pokeAbilityComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeBallSealComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeDVNums.ForEach(x => x.Value = 0);
            pokeFormsComboBoxes.ForEach(x => x.SelectedIndex = -1);
            pokeHeldItemComboBoxes.ForEach(x => x.SelectedIndex = -1);
            for (int i = 0; i < pokeMoves.Count; i++)
            {
                pokeMoves[i] = null;
            };

            // Set Pokemon values from Party Data
            for (int i = 0; i < SelectedTrainer.TrainerProperties.TeamSize; i++)
            {
                var species = GetSpeciesBySpeciesId(SelectedTrainer.TrainerParty.Pokemons[i].SpeciesId);
                pokeComboBoxes[i].SelectedIndex = SelectedTrainer.TrainerParty.Pokemons[i].PokemonId;
                pokeLevelNums[i].Value = SelectedTrainer.TrainerParty.Pokemons[i].Level;
                pokeDVNums[i].Value = SelectedTrainer.TrainerParty.Pokemons[i].DifficultyValue;

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
                            switch (SelectedTrainer.TrainerParty.Pokemons[i].GenderAbilityFlags)
                            {
                                case GenderAbilityFlags.None:
                                    pokeGenderComboBoxes[i].SelectedIndex = 0; break;
                                case GenderAbilityFlags.IsMale:
                                    pokeGenderComboBoxes[i].SelectedIndex = 1; break;
                                case GenderAbilityFlags.IsFemale:
                                    pokeGenderComboBoxes[i].SelectedIndex = 2; break;
                            }
                            break;
                    }
                }
            }
            IsLoadingData = false;
        }

        private Species GetSpeciesBySpeciesId(int speciesId)
        {
            return MainEditorModel.PokemonSpecies.Find(x => x.SpeciesId == speciesId);
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

        private void PopulateTrainerData()
        {
            IsLoadingData = true;
            trainer_TeamSizeNum.Maximum = 6;
            trainer_DblBattleCheckBox.Checked = SelectedTrainer.TrainerProperties.DoubleBattle;
            trainer_NameTextBox.Text = SelectedTrainer.TrainerName;
            trainer_TeamSizeNum.Value = SelectedTrainer.TrainerProperties.TeamSize;
            trainer_ChooseMovesCheckbox.Checked = SelectedTrainer.TrainerProperties.ChooseMoves;
            trainer_HeldItemsCheckbox.Checked = SelectedTrainer.TrainerProperties.ChooseItems;
            for (int i = 0; i < SelectedTrainer.TrainerProperties.AIFlags.Count; i++)
            {
                trainer_AiFlags_listbox.SetItemChecked(i, SelectedTrainer.TrainerProperties.AIFlags[i]);
            }
            trainer_TeamSizeNum.Maximum = trainer_DblBattleCheckBox.Checked ? 3 : 6;
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

            pokeBallSealComboBoxes =
                [
                poke1BallSealComboBox,
                poke2BallSealComboBox,
                poke3BallSealComboBox,
                poke4BallSealComboBox,
                poke5BallSealComboBox,
                poke6BallSealComboBox,
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
            if (trainer_TrainersListBox.Items.Count == 0)
            {
                PopulateTrainerList(MainEditorModel.Trainers);
            }
            if (trainer_AiFlags_listbox.Items.Count == 0)
            {
                InitializeAiFlags();
            }
            if (poke1ComboBox.Items.Count == 0)
            {
                SetupPartyEditorFields();
                PopulatePokemonComboBoxes();
            }
            if (poke1GenderComboBox.Items.Count == 0 && LoadedRom.GameFamily == GameFamily.HeartGoldSoulSilver)
            {
                PopulatePokemonGenderComboBoxes();
                pokeGenderComboBoxes.ForEach(x => x.Visible = true);
            }
            else
            {
                pokeGenderComboBoxes.ForEach(x => x.Visible = false);
                pokeGenderComboBoxes.ForEach(x => x.Enabled = false);
            }
            trainer_TrainersListBox.Enabled = true;
            IsLoadingData = false;
        }

        private void trainer_ClearFilterBtn_Click(object sender, EventArgs e)
        {
            trainer_FilterBox.Text = "";
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

        private void trainer_NameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerData(trainer_NameTextBox.Text != SelectedTrainer.TrainerName);
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

        private void trainer_TeamSizeNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty((byte)trainer_TeamSizeNum.Value != SelectedTrainer.TrainerProperties.TeamSize);
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
                            int index = trainer_TrainersListBox.Items.IndexOf(SelectedTrainer.ListName);
                            trainer_TrainersListBox.SelectedIndex = index;
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

        private void trainer_ChooseMovesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(trainer_ChooseMovesCheckbox.Checked != SelectedTrainer.TrainerProperties.ChooseMoves);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void trainer_DblBattleCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            trainer_TeamSizeNum.Maximum = trainer_DblBattleCheckBox.Checked ? 3 : 6;

            if (!IsLoadingData)
            {
                EditedTrainerParty(trainer_DblBattleCheckBox.Checked != SelectedTrainer.TrainerProperties.DoubleBattle
                    || (byte)trainer_TeamSizeNum.Value != SelectedTrainer.TrainerProperties.TeamSize);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        private void trainer_HeldItemsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(trainer_HeldItemsCheckbox.Checked != SelectedTrainer.TrainerProperties.ChooseItems);
                EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            }
        }

        #endregion TrainerEditor

        #region ClassEditor

        private List<string> UnfilteredClasses = [];
        private bool UnsavedClassChanges;

        private void SetupClassEditor()
        {
            IsLoadingData = true;
            if (class_ClassListBox.Items.Count == 0)
            {
                PopulateClassList(MainEditorModel.Classes);
            }
            IsLoadingData = false;
        }

        #endregion ClassEditor

        #region BattleMessageEditor

        private bool UnsavedBattleMessageChanges;

        #endregion BattleMessageEditor

        private void menu_Export_Trainers_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                var trainers = MainEditorModel.Trainers;
                var gameFamily = LoadedRom.GameFamily;
                int trainerNamesArchive = LoadedRom.TrainerNamesTextNumber;
                int classesCount = 100;
                int battleMessagesCount = 200;
                var vsTrainersFile = fileSystemMethods.BuildVsTrainersFile(trainers, gameFamily, trainerNamesArchive, classesCount, battleMessagesCount);
                var exportData = new ViewVsTrainerFile(vsTrainersFile, ViewVsMakerFileType.Export);
                exportData.ShowDialog();
            }
        }

        private void menu_Import_Trainers_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                var importData = new ViewVsTrainerFile(new VsTrainersFile(), ViewVsMakerFileType.Import);
                importData.ShowDialog();
            }
        }

        private void ProcessImportTrainers(List<Trainer> newTrainers, List<Trainer> oldTrainers)
        {
        }
    }
}