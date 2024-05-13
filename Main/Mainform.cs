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
            var unpack = romFileMethods.ExtractRomContents(LoadedRom.WorkingDirectory, LoadedRom.FileName);
            if (!unpack.Success)
            {
                MessageBox.Show($"{unpack.ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            using EasyReader reader = new(filePath, 0xC);
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
            IsLoadingData = true;
            romName_Label.Text = "";
            LoadedRom = new RomFile();
            RomLoaded = false;
            startupTab.SelectedTab = startupPage;
            MainEditorModel = new();
            ClearTrainerEditorData();

            class_ClassListBox.SelectedIndex = -1;
            class_ClassListBox.Items.Clear();
            EnableDisableMenu(false);
            ClearUnsavedChanges();
            IsLoadingData = false;
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
            EnableDisableMenu(RomLoaded);
            startupTab.SelectedTab = RomLoaded ? mainPage : startupPage;
            if (RomLoaded)
            {
                LoadedRom.TotalNumberOfTrainers = romFileMethods.GetTotalNumberOfTrainers(LoadedRom.TrainerNamesTextNumber);
                LoadedRom.TrainersData = romFileMethods.GetTrainersData(LoadedRom.TotalNumberOfTrainers);
                LoadedRom.TrainersPartyData = romFileMethods.GetTrainersPartyData(LoadedRom.TotalNumberOfTrainers, LoadedRom.TrainersData, LoadedRom.GameFamily);
                LoadedRom.TrainerNameMaxByte = romFileMethods.SetTrainerNameMax(LoadedRom.TrainerNameMaxByteOffset);
                LoadedRom.TotalNumberOfTrainerClasses = romFileMethods.GetTotalNumberOfTrainerClassess(LoadedRom.ClassNamesTextNumber);
                LoadedRom.ClassGenderData = romFileMethods.GetClassGenders(LoadedRom.TotalNumberOfTrainerClasses, LoadedRom.ClassGenderOffsetToRamAddress);
                LoadedRom.EyeContactMusicData = romFileMethods.GetEyeContactMusicData(LoadedRom.EyeContactMusicTableOffsetToRam, LoadedRom.GameFamily);
                InitializeTrainerEditor();
                InitializeClassEditor();
                GetInitialData();
                main_MainTab.SelectedTab = main_MainTab_TrainerTab;
                SetupTrainerEditor();
            }
            IsLoadingData = false;
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

        private void FilterListBox(ListBox listBox, string filter, List<string> unfiltered)
        {
            List<string> filteredList = [];
            foreach (string item in unfiltered)
            {
                string name = item.ToLower();
                if (name.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
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
            MainEditorModel.TrainerNames = romFileMethods.GetTrainerNames(LoadedRom.TrainerNamesTextNumber);
            MainEditorModel.ClassNames = romFileMethods.GetClassNames(LoadedRom.ClassNamesTextNumber);
            MainEditorModel.ClassDescriptions = romFileMethods.GetClassDescriptions(LoadedRom.ClassDescriptionMessageNumber);
            MainEditorModel.Trainers = trainerEditorMethods.GetTrainers(MainEditorModel.TrainerNames, LoadedRom);
            MainEditorModel.Classes = classEditorMethods.GetTrainerClasses(MainEditorModel.Trainers, MainEditorModel.ClassNames, MainEditorModel.ClassDescriptions, LoadedRom);
            MainEditorModel.PokemonNames = romFileMethods.GetPokemonNames(LoadedRom.PokemonNamesTextNumber);
            MainEditorModel.MoveNames = romFileMethods.GetMoveNames(LoadedRom.MoveNameTextNumber);
            MainEditorModel.AbilityNames = romFileMethods.GetAbilityNames(LoadedRom.AbilityNamesTextNumber);
            MainEditorModel.ItemNames = romFileMethods.GetItemNames(LoadedRom.ItemNamesTextNumber);
            IsLoadingData = false;
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
            IsLoadingData = true;

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
            IsLoadingData = false;
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
                        InitializeClassEditor();
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
                    InitializeClassEditor();
                }
                EndOpenRom();
            }
        }

        private void main_SettingsBtn_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
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
            romFileMethods = new RomFileMethods();
            trainerEditorMethods = new TrainerEditorMethods();
            classEditorMethods = new ClassEditorMethods();
            fileSystemMethods = new FileSystemMethods();
        }

        private void menu_Export_Trainers_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                var trainers = MainEditorModel.Trainers;
                var gameFamily = LoadedRom.GameFamily;
                int classesCount = 100;
                int battleMessagesCount = 200;
                var vsTrainersFile = fileSystemMethods.BuildVsTrainersFile(trainers, gameFamily, LoadedRom.TrainerNamesTextNumber, classesCount, battleMessagesCount);
            }
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
                        InitializeClassEditor();
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
                    InitializeClassEditor();
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
                        InitializeClassEditor();
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
                    InitializeClassEditor();
                }
                EndOpenRom();
            }
        }

        private void menu_Import_Trainers_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
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
            RomPatches = new RomPatches(LoadedRom);
            RomPatches.ShowDialog();
        }

        private void OpenSettingsWindow()
        {
            Settings.ShowDialog();
        }

        private void poke1BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke2BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke3BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke4BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke5BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void poke6BallCapsuleComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedTrainerParty(true);
            }
        }

        private void ProcessImportTrainers(List<Trainer> newTrainers, List<Trainer> oldTrainers)
        {
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
            romFileMethods.SetNarcDirectories(workingDirectory, LoadedRom.GameVersion, LoadedRom.GameFamily, LoadedRom.GameLanguage);

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

        private void class_UndoPropertiesBtn_Click(object sender, EventArgs e)
        {
            ClearUnsavedClassPropertiesChanges();
            class_GenderComboBox.SelectedIndex = SelectedClass.Gender;
            class_DescriptionTextBox.Text = SelectedClass.Description;
        }
    }
}