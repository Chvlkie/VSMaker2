using Main.Forms;
using Main.Misc;
using Main.Models;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.Methods;
using VsMaker2Core.Methods.EventFile;
using VsMaker2Core.Methods.NdsImages;
using static VsMaker2Core.Enums;

namespace Main
{
    public partial class Mainform : Form
    {
        private const int debounceDelay = 300;
        private readonly System.Windows.Forms.Timer filterTimer;
        private readonly string? appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(4);
        private string defaultFolderPath = "";

        #region Forms

        private LoadingData loadingData;
        private MoveSelector moveSelector;
        private RomPatches romPatches;

        #endregion Forms

        #region Methods

        private IBattleMessageEditorMethods battleMessageEditorMethods;
        private IClassEditorMethods classEditorMethods;
        private IEventFileMethods eventFileMethods;
        private IFileSystemMethods fileSystemMethods;
        private INdsImage ndsImage;
        private IRomFileMethods romFileMethods;
        private IScriptFileMethods scriptFileMethods;
        private ITrainerEditorMethods trainerEditorMethods;

        #endregion Methods

        private bool inhibitTabChange = false;
        private bool isLoadingData;
        private bool loadingError = false;
        private MainEditorModel mainEditorModel;
        private bool romLoaded;

        public Mainform()
        {
            InitializeComponent();
            filterTimer = new System.Windows.Forms.Timer
            {
                Interval = debounceDelay
            };
            filterTimer.Tick += FilterTimer_Tick;

            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            romLoaded = false;
            romName_Label.Text = "";
            mainEditorModel = new();

            Text = $"VS Maker 2 - v{appVersion}";
        }

        private bool UnsavedChanges => UnsavedTrainerEditorChanges || UnsavedClassChanges || UnsavedBattleMessageChanges;

        public async void BeginSaveRomChanges(IProgress<int> progress, string filePath)
        {
            int count = 0;
            int increment = 100 / VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories.Count;

            foreach (var kvp in VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories)
            {
                DirectoryInfo di = new(kvp.Value.unpackedDirectory);
                if (di.Exists)
                {
                    Narc.FromFolder(kvp.Value.unpackedDirectory).Save(kvp.Value.packedDirectory);
                }
                progress?.Report(count += increment);
            }

            HandleArm9Compression(progress!);
            await HandleOverlayCompressionAsync(progress!);
            await romFileMethods.RepackRomAsync(filePath);
            progress?.Report(100);
            MessageBox.Show("ROM File saved to " + filePath, "Success!");
        }

        public async Task BeginUnpackNarcsAsync(IProgress<int> progress)
        {
            var narcs = GameFamilyNarcs.GetGameFamilyNarcs(RomFile.GameFamily);

            var (success, exception) = await romFileMethods.UnpackNarcsAsync(narcs, progress);
            if (!success)
            {
                MessageBox.Show(exception, "Unable to Unpack NARCs", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadingError = true;
                CloseProject();
            }
            else
            {
                loadingError = false;
            }
            progress?.Report(100);
            return;
        }

        public async Task BeginUnpackRomDataAsync()
        {
            var unpack = await romFileMethods.ExtractRomContentsAsync(RomFile.WorkingDirectory, RomFile.FileName);
            if (!unpack.Success)
            {
                MessageBox.Show($"{unpack.ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadingError = true;
                CloseProject();
            }
            else
            {
                loadingError = false;
            }
            romLoaded = true;
            return;
        }

        public void FilterListBox(ListBox listBox, string filter, List<string> unfiltered)
        {
            listBox.BeginUpdate();
            var filteredList = unfiltered
                .Where(item => item.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

            listBox.Items.Clear();
            listBox.Items.AddRange(filteredList.ToArray());
            listBox.EndUpdate();
        }

        public async void GetInitialData(IProgress<int> progress = null)
        {
            isLoadingData = true;
            int progressCount = 0;
            const int increment = 100 / 11;

            mainEditorModel.PokemonSpecies = await romFileMethods.GetSpeciesAsync();
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.TrainerNames = await romFileMethods.GetTrainerNamesAsync(RomFile.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.ClassNames = await romFileMethods.GetClassNamesAsync(RomFile.ClassNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.ClassDescriptions = await romFileMethods.GetClassDescriptionsAsync(RomFile.ClassDescriptionMessageNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.Trainers = trainerEditorMethods.GetTrainers(mainEditorModel.TrainerNames);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.Classes = classEditorMethods.GetTrainerClasses(mainEditorModel.Trainers, mainEditorModel.ClassNames, mainEditorModel.ClassDescriptions);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.PokemonNamesFull = await romFileMethods.GetPokemonNamesAsync(RomFile.PokemonNamesTextNumber);
            mainEditorModel.PokemonNames = MainEditorModel.SetPokemonNames(mainEditorModel.PokemonNamesFull);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.MoveNames = await romFileMethods.GetMoveNamesAsync(RomFile.MoveNameTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.AbilityNames = await romFileMethods.GetAbilityNamesAsync(RomFile.AbilityNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.ItemNames = await romFileMethods.GetItemNamesAsync(RomFile.ItemNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            mainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            isLoadingData = false;
            progress?.Report(100);
        }

        public void ImportBattleMessageCsv()
        {
            var confirmImport = MessageBox.Show("Importing a CSV will overwrite existing data.\n\nAre you sure?", "Import CSV", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmImport == DialogResult.Yes)
            {
                var importFile = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Open CSV File"
                };

                if (importFile.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }
                OpenLoadingDialog(LoadType.ImportTextTable, importFile.FileName);
            }
        }

        public async void LoadRomData(IProgress<int> progress)
        {
            isLoadingData = true;
            int progressCount = 0;
            int increment = 100 / 12;

            RomFile.ScriptFileData = scriptFileMethods.GetScriptFiles();
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.EventFileData = eventFileMethods.GetEventFiles();
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TotalNumberOfTrainers = await romFileMethods.GetTotalNumberOfTrainersAsync(RomFile.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.BattleMessageTableData = await romFileMethods.GetBattleMessageTableDataAsync(RomFile.BattleMessageTablePath);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.BattleMessageOffsetData = await romFileMethods.GetBattleMessageOffsetDataAsync(RomFile.BattleMessageOffsetPath);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TrainersData = await romFileMethods.GetTrainersDataAsync(RomFile.TotalNumberOfTrainers);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TrainersPartyData = await romFileMethods.GetTrainersPartyDataAsync(RomFile.TotalNumberOfTrainers, RomFile.TrainersData, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TrainerNameMaxByte = romFileMethods.SetTrainerNameMax(RomFile.TrainerNameMaxByteOffset);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TotalNumberOfTrainerClasses = await romFileMethods.GetTotalNumberOfTrainerClassesAsync(RomFile.ClassNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);
            RomFile.Arm9Expanded = RomFile.Arm9Expanded = RomPatches.CheckFilesArm9ExpansionApplied();
            if (RomFile.Arm9Expanded)
            {
                bool prizeMoneyExpanded = await RomFile.CheckForPrizeMoneyExpansionAsync();
                RomFile.PrizeMoneyExpanded = prizeMoneyExpanded;
                RomFile.ClassGenderExpanded = RomFile.CheckForClassGenderExpansion();
                RomFile.EyeContactExpanded = RomFile.CheckForEyeContactExpansion();
            }
            RomFile.ClassGenderData = RomFile.IsNotDiamondPearl ? await romFileMethods.GetClassGendersAsync(RomFile.TotalNumberOfTrainerClasses, RomFile.ClassGenderOffsetToRamAddress) : [];
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.EyeContactMusicData = await romFileMethods.GetEyeContactMusicDataAsync(RomFile.EyeContactMusicTableOffsetToRam, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.PrizeMoneyData = await romFileMethods.GetPrizeMoneyDataAsync();
            progressCount += increment;
            progress?.Report(progressCount);

            isLoadingData = false;
            progress?.Report(100);
        }

        public void RefreshTrainerClasses()
        {
            mainEditorModel.Classes = classEditorMethods.GetTrainerClasses(mainEditorModel.Trainers, mainEditorModel.ClassNames, mainEditorModel.ClassDescriptions);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static void CreateDirectory(string workingDirectory)
        {
            if (!Directory.Exists(workingDirectory))
            {
                Console.WriteLine("Creating directory " + workingDirectory);
                Directory.CreateDirectory(workingDirectory);
                Console.WriteLine("Created directory " + workingDirectory + " | Success");
            }
            else
            {
                MessageBox.Show("Unable to extract contents.\n\n" +
                    "Contents folder may already exist", "Unable to Extract ROM", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Console.WriteLine("Unable to create directory " + workingDirectory);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        private static (byte EuropeByte, string GameCode) LoadInitialRomData(string filePath)
        {
            using (EasyReader reader = new(filePath, 0xC))
            {
                string gameCode = Encoding.UTF8.GetString(reader.ReadBytes(4));
                reader.BaseStream.Position = 0x1E;
                byte europeByte = reader.ReadByte();
                return (europeByte, gameCode);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void BeginExtractRomData()
        {
            OpenLoadingDialog(LoadType.UnpackNarcs);
        }

        private void ClearUnsavedChanges()
        {
            ClearUnsavedTrainerChanges();
            ClearUnsavedClassChanges();
            ClearUnsavedClassPropertiesChanges();
        }

        private void CloseProject()
        {
            Console.WriteLine("Closing any open projects...");
            isLoadingData = true;
            romName_Label.Text = "";
            RomFile.Reset();
            romLoaded = false;
            startupTab.SelectedTab = startupPage;
            mainEditorModel = new();
            ClearTrainerEditorData();
            class_ClassListBox.SelectedIndex = -1;
            class_ClassListBox.Items.Clear();
            EnableDisableMenu(false);
            ClearUnsavedChanges();
            isLoadingData = false;
            Console.WriteLine("Projects closed.");
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
                    "Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return saveChanges == DialogResult.Yes;
        }

        private void EnableDisableMenu(bool enable)
        {
            main_SaveRomBtn.Enabled = enable;

            menu_File_Save.Enabled = enable;
            menu_File_Close.Enabled = enable;
            menu_Import.Enabled = enable;
            menu_Export.Enabled = enable;
            menu_Tools_RomPatcher.Enabled = enable;
            main_OpenPatchesBtn.Enabled = enable;
        }

        private void EndOpenRom()
        {
            EnableDisableMenu(romLoaded);
            startupTab.SelectedTab = romLoaded ? mainPage : startupPage;
            if (romLoaded)
            {
                OpenLoadingDialog(LoadType.LoadRomData);
                InitializeTrainerEditor();
                InitializeClassEditor();
                InitializeBattleMessageEditor();
                OpenLoadingDialog(LoadType.SetupEditor);
                main_MainTab.SelectedTab = main_MainTab_TrainerTab;
                SetupTrainerEditor();
            }
            isLoadingData = false;
            Console.WriteLine("Rom loaded | Success");
        }

        private void ExportBattleMessagesAsCsv()
        {
            var exportFile = new SaveFileDialog
            {
                FileName = "Battle Messages",
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Save CSV File" +
                "",
            };

            if (exportFile.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            OpenLoadingDialog(LoadType.ExportTextTable, exportFile.FileName);
        }

        private string GetPokemonNameById(int pokemonId)
        {
            return mainEditorModel.PokemonNamesFull[pokemonId];
        }

        private static void HandleArm9Compression(IProgress<int> progress)
        {
            if (Arm9.CheckCompressionMark(RomFile.GameFamily))
            {
                DialogResult d = MessageBox.Show("The ARM9 file of this ROM is currently uncompressed, but marked as compressed.\n" +
                    "This will prevent your ROM from working on native hardware.\n\n" +
                    "Do you want to mark the ARM9 as uncompressed?", "ARM9 compression mismatch detected",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                progress?.Report(0);
                if (d == DialogResult.Yes)
                {
                    Arm9.WriteBytes(new byte[] { 0, 0, 0, 0 }, (uint)(RomFile.GameFamily == GameFamily.DiamondPearl ? 0xB7C : 0xBB4));
                    progress?.Report(10);
                }
            }
        }

        private async Task HandleOverlayCompressionAsync(IProgress<int> progress)
        {
            if (Overlay.CheckOverlayHasCompressionFlag(1))
            {
                if (RomPatches.LoadOverlay1FromBackup)
                {
                    var restore = await Overlay.RestoreOverlayFromCompressedBackupAsync(1, false);
                    if (!restore.Success)
                    {
                        MessageBox.Show(restore.Error, "Unable to Restore Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else if (!await Overlay.CheckOverlayIsCompressedAsync(1))
                {
                    await Overlay.CompressOverlayAsync(1);
                }
            }

            if (Overlay.CheckOverlayHasCompressionFlag(RomFile.InitialMoneyOverlayNumber) &&
                !await Overlay.CheckOverlayIsCompressedAsync(RomFile.InitialMoneyOverlayNumber))
            {
                await Overlay.CompressOverlayAsync(RomFile.InitialMoneyOverlayNumber);
            }
        }

        private void main_SaveRomBtn_Click(object sender, EventArgs e)
        {
            SaveRomChanges();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            AllocConsole();
            Config.ResetConsoleOutput();
            Console.WriteLine($"VS Maker 2 - v{appVersion}");
            Config.LoadConfig();
            Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);
            defaultFolderPath = Config.GetRomFolderPath();
            Config.ShowConsole(Config.ShowConsoleWindow);
        }

        private void menu_Export_BattleMessages_Click(object sender, EventArgs e)
        {
            if (battleMessage_MessageTableDataGrid.RowCount == 0)
            {
                LoadBattleMessages();
            }
            ExportBattleMessagesAsCsv();
        }

        private void menu_File_Save_Click(object sender, EventArgs e)
        {
            SaveRomChanges();
        }

        private void menu_Import_BattleMessages_Click(object sender, EventArgs e)
        {
            ImportBattleMessageCsv();
        }

        private void OpenLoadingDialog(LoadType loadType)
        {
            UseWaitCursor = true;
            loadingData = new(this, loadType);
            loadingData.ShowDialog();
            UseWaitCursor = false;
        }

        private void OpenLoadingDialog(LoadType loadType, string filePath)
        {
            UseWaitCursor = true;
            loadingData = new(this, loadType, filePath);
            loadingData.ShowDialog();
            UseWaitCursor = false;
        }

        private void OpenRecentFile(string filePath)
        {
            isLoadingData = true;

            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    CloseProject();
                    romLoaded = ReadRomExtractedFolder(filePath);
                    Config.AddToRecentItems(filePath);
                    Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);
                    if (romLoaded)
                    {
                        BeginExtractRomData();
                        InitializeTrainerEditor();
                        InitializeClassEditor();
                        EndOpenRom();
                    }
                }
            }
            else
            {
                CloseProject();
                romLoaded = ReadRomExtractedFolder(filePath);
                Config.AddToRecentItems(filePath);
                Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);
                if (romLoaded)
                {
                    BeginExtractRomData();
                    InitializeTrainerEditor();
                    InitializeClassEditor();
                    EndOpenRom();
                }
            }
            isLoadingData = false;
        }

        private async Task OpenRomAsync()
        {
            OpenFileDialog openRom = new()
            {
                Filter = Common.NdsRomFilter
            };

            if (openRom.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(openRom.FileName))
            {
                await SelectWorkingFolderDirectoryAsync(openRom.FileName);
            }
        }

        private void OpenRomPatchesWindow()
        {
            romPatches = new RomPatches(this, romFileMethods);
            romPatches.ShowDialog();
        }

        private void OpenSettingsWindow()
        {
            var settingsForm = new Settings();

            settingsForm.ClearRecentItemsRequested += SettingsForm_ClearRecentItemsRequested;

            settingsForm.ShowDialog();
        }

        private void PopulateTrainerClassSprite(PictureBox pictureBox, NumericUpDown frameNumBox, int trainerClassId)
        {
            UpdateTrainerClassSprite(pictureBox, frameNumBox, trainerClassId);
        }

        private bool ReadRomExtractedFolder(string selectedFolder)
        {
            Console.WriteLine("Reading ROM Contents from " + selectedFolder);
            if (string.IsNullOrEmpty(selectedFolder))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                    "\n\nPlease ensure you select an extracted ROM contents folder" +
                    "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Cannot load ROM header.bin");
                return false;
            }

            string fileName = Directory.GetFiles(selectedFolder).SingleOrDefault(x => x.Contains(Common.HeaderFilePath));
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Cannot load ROM header.bin." +
                    "\n\nPlease ensure you select an extracted ROM contents folder" +
                    "\nand that data is not corrupted.", "Unable to Open Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Cannot load ROM header.bin");
                return false;
            }

            return ReadRomFile(selectedFolder + "\\", fileName);
        }

        private bool ReadRomFile(string workingDirectory, string fileName)
        {
            Console.WriteLine("Reading ROM File...");

            isLoadingData = true;
            Console.WriteLine("Load initial ROM Data");
            var loadedRom = LoadInitialRomData(fileName);
            Console.WriteLine("Load initial ROM Data | Success");
            RomFile.SetupRomFile(loadedRom.GameCode, fileName, workingDirectory, loadedRom.EuropeByte);
            if (RomFile.GameVersion == GameVersion.Unknown)
            {
                MessageBox.Show("The ROM file you have selected is not supported by VSMaker 2." +
                    "\n\nVSMaker 2 currently only support Pokémon Diamond, Pearl, Platinum, HeartGold or Soul Silver version."
                    , "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Unable to read ROM File");

                return false;
            }

            romFileMethods.SetNarcDirectories(workingDirectory, RomFile.GameVersion, RomFile.GameFamily, RomFile.GameLanguage);
            Console.WriteLine("Reading ROM File | Success");
            return true;
        }

        private void SaveRomChanges()
        {
            var save = new SaveFileDialog()
            {
                Filter = Common.NdsRomFilter
            };

            if (!string.IsNullOrEmpty(defaultFolderPath) && Directory.Exists(defaultFolderPath))
            {
                save.InitialDirectory = defaultFolderPath;
            }
            if (save.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            string saveFileName = save.FileName;
            OpenLoadingDialog(LoadType.SaveRom, saveFileName);
        }

        private void SelectExtractedRomFolder()
        {
            FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Extracted ROM Contents Folder",
                UseDescriptionForTitle = true
            };

            if (!string.IsNullOrEmpty(defaultFolderPath) && Directory.Exists(defaultFolderPath))
            {
                selectFolder.InitialDirectory = defaultFolderPath;
            }

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                romLoaded = ReadRomExtractedFolder(selectFolder.SelectedPath);
                Config.AddToRecentItems(selectFolder.SelectedPath);
                Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);
                if (romLoaded)
                {
                    BeginExtractRomData();
                    InitializeTrainerEditor();
                    InitializeClassEditor();
                    EndOpenRom();
                }
            }
        }

        private async Task SelectWorkingFolderDirectoryAsync(string fileName)
        {
            using FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Where to Extract ROM Contents",
                UseDescriptionForTitle = true,
            };

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                Console.WriteLine("Opening ROM file " + fileName);
                string workingDirectory = $"{selectFolder.SelectedPath}\\{Path.GetFileNameWithoutExtension(fileName)}{Common.VsMakerContentsFolder}\\";
                Config.AddToRecentItems(workingDirectory);
                Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);
                if (Directory.Exists(workingDirectory))
                {
                    var directoryExists = MessageBox.Show("An extracted contents folder for this ROM has been found." +
                        "\n\nDo you wish to open this folder?", "Extracted ROM Data exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (directoryExists == DialogResult.Yes)
                    {
                        CloseProject();
                        Console.WriteLine("Opening existing contents folder " + workingDirectory);
                        romLoaded = ReadRomExtractedFolder(workingDirectory);
                    }
                    else
                    {
                        var extractContentsAgain = MessageBox.Show("ROM Data will be re-extracted.\nThis will delete all existing data in the old folder." +
                            "\n\nDo you wish to proceed?", "Re-Extract ROM Data", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (extractContentsAgain == DialogResult.Yes)
                        {
                            Console.WriteLine("Deleting existing ROM contents folder " + workingDirectory);
                            Directory.Delete(workingDirectory, true);
                            Console.WriteLine("Deleted folder " + workingDirectory);
                            CreateDirectory(workingDirectory);
                        }
                        else
                        {
                            await SelectWorkingFolderDirectoryAsync(fileName);
                            return;
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

                        if (romLoaded && !loadingError)
                        {
                            Arm9.Arm9EditSize(-12);

                            // Check for compression mark and await the asynchronous decompression
                            if (Arm9.CheckCompressionMark(RomFile.GameFamily))
                            {
                                // Await the Arm9DecompressAsync method
                                if (!await Arm9.Arm9DecompressAsync(RomFile.Arm9Path))
                                {
                                    MessageBox.Show("Unable to decompress Arm9");
                                    Console.WriteLine("Unable to decompress Arm9");
                                    return;
                                }
                            }

                            BeginExtractRomData();
                        }

                        if (!loadingError)
                        {
                            InitializeTrainerEditor();
                            InitializeClassEditor();
                            EndOpenRom();
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
                else
                {
                    CloseProject();
                }
            }
        }

        private void SettingsForm_ClearRecentItemsRequested(object sender, EventArgs e)
        {
            Config.ClearRecentItems(menu_File_OpenRecent);
            MessageBox.Show("Recent items have been cleared.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region Event Handlers

        private void HandleTabChange(TabPage selectedTab)
        {
            if (selectedTab == main_MainTab_TrainerTab)
            {
                SetupTrainerEditor();
            }
            else if (selectedTab == main_MainTab_ClassTab)
            {
                SetupClassEditor();
            }
            else if (selectedTab == main_MainTable_BattleMessageTab)
            {
                SetupBattleMessageEditor();
            }
        }

        private void main_MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inhibitTabChange)
            {
                inhibitTabChange = false;
                return;
            }

            if (UnsavedBattleMessageChanges && !ConfirmUnsavedChanges())
            {
                inhibitTabChange = true;
                main_MainTab.SelectedTab = main_MainTable_BattleMessageTab;
            }
            else
            {
                HandleTabChange(main_MainTab.SelectedTab);
            }
        }

        private void main_OpenFolderBtn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;

            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    SelectExtractedRomFolder();
                }
            }
            else
            {
                SelectExtractedRomFolder();
            }
            isLoadingData = false;
        }

        private void main_OpenPatchesBtn_Click(object sender, EventArgs e)
        {
            OpenRomPatchesWindow();
        }

        private async void main_OpenRomBtn_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    await OpenRomAsync();
                }
            }
            else
            {
                await OpenRomAsync();
            }
        }

        private void main_SettingsBtn_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow();
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            string closeMessage = romLoaded ? "Do you wish to close VS-Maker?\n\nAny unsaved changes will be lost." : "Do you wish to close VS-Maker?";
            if (MessageBox.Show(closeMessage, "Close VS-Maker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void Mainform_Shown(object sender, EventArgs e)
        {
            loadingData = new LoadingData();
            romFileMethods = new RomFileMethods();
            scriptFileMethods = new ScriptFileMethods();
            eventFileMethods = new EventFileMethods();
            battleMessageEditorMethods = new BattleMessageEditorMethods(romFileMethods);
            trainerEditorMethods = new TrainerEditorMethods(romFileMethods);
            classEditorMethods = new ClassEditorMethods(romFileMethods);
            fileSystemMethods = new FileSystemMethods(romFileMethods, scriptFileMethods);
            ndsImage = new NdsImage();

            if (Config.LoadLastOpened)
            {
                Console.WriteLine("Loading last opened project...");
                string lastOpenedRomFolder = Config.GetFirstRecentItem();
                if (!string.IsNullOrEmpty(lastOpenedRomFolder))
                {
                    OpenRecentFile(lastOpenedRomFolder);
                }
            }
        }

        private void menu_Export_Trainers_Click(object sender, EventArgs e)
        {
            //if (!IsLoadingData)
            //{
            //    var trainers = MainEditorModel.Trainers;
            //    var gameFamily = RomFile.GameFamily;
            //    const int classesCount = 100;
            //    const int battleMessagesCount = 200;
            //    var vsTrainersFile = fileSystemMethods.BuildVsTrainersFile(trainers, gameFamily, RomFile.TrainerNamesTextNumber, classesCount, battleMessagesCount);
            //}
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
                }
            }
            else
            {
                SelectExtractedRomFolder();
            }
        }

        private async void menu_File_OpenRom_Click(object sender, EventArgs e)
        {
            if (UnsavedChanges)
            {
                var saveChanges = MessageBox.Show("You have unsaved changes.\n\n" +
                        "You will lose these changes if you close the project.\n" +
                        "Do you still want to close?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (saveChanges == DialogResult.Yes)
                {
                    OpenRomAsync();
                }
            }
            else
            {
                OpenRomAsync();
            }
        }

        private void menu_Import_Trainers_Click(object sender, EventArgs e)
        {
            if (!isLoadingData)
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

        #endregion Event Handlers

        private void UpdateTrainerClassSprite(PictureBox pictureBox, NumericUpDown frameNumBox, int trainerClassId)
        {
            if (RomFile.IsNotDiamondPearl)
            {
                var palette = ndsImage.GetTrainerClassPaletteBase(trainerClassId);
                var image = ndsImage.GetTrainerClassImageBase(trainerClassId);
                var sprite = ndsImage.GetTrainerClassSpriteBase(trainerClassId);
                frameNumBox.Enabled = sprite.NumBanks > 1;
                frameNumBox.Maximum = sprite.NumBanks - 1;
                if (frameNumBox.Value > frameNumBox.Maximum)
                {
                    frameNumBox.Value = 0;
                }
                int frame = (int)(frameNumBox.Enabled ? frameNumBox.Value : 0);
                pictureBox.Image = ndsImage.GetTrainerClassSrite(palette, image, sprite, frame);
            }
        }
    }
}