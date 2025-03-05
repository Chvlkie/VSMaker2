using Main.Forms;
using Main.Misc;
using Main.Models;
using System.Reflection;
using System.Runtime.InteropServices;
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
    public partial class MainForm : Form
    {
        private const int debounceDelay = 300;
        private readonly System.Windows.Forms.Timer filterTimer;
        private readonly string? appVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(4);
        private string defaultFolderPath = "";

        #region Forms

        private LoadingData loadingData;
        private RomPatches romPatches;
        private HgeIvEvForm hgeIvEvForm;
        private EditStatsForm editStatsForm;

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

        private bool inhibitTabChange;
        private bool isLoadingData;
        private bool loadingError;
        private MainDataModel mainDataModel;
        private bool romLoaded;

        public MainForm()
        {
            InitializeComponent();
            AutoScaleMode = AutoScaleMode.None;
            this.Font = new Font("Segoe UI", 9f); 
            AdjustFormSize();
            filterTimer = new System.Windows.Forms.Timer
            {
                Interval = debounceDelay
            };
            filterTimer.Tick += FilterTimer_Tick!;

            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            romLoaded = false;
            romName_Label.Text = "";
            mainDataModel = new();

            Text = $"VS Maker 2 - v{appVersion} HgE Experimental Build";
        }

        private void AdjustFormSize()
        {
            // Get the screen bounds
            Rectangle screenBounds = Screen.FromControl(this).Bounds;

            // Ensure the form fits within the screen bounds
            if (this.Width > screenBounds.Width || this.Height > screenBounds.Height)
            {
                this.Size = new Size(
                    Math.Min(this.Width, screenBounds.Width),
                    Math.Min(this.Height, screenBounds.Height)
                );
            }

            // Ensure the form is positioned within the screen bounds
            if (this.Right > screenBounds.Right)
            {
                this.Left = screenBounds.Right - this.Width;
            }
            if (this.Bottom > screenBounds.Bottom)
            {
                this.Top = screenBounds.Bottom - this.Height;
            }
        }

        private bool UnsavedChanges => UnsavedTrainerEditorChanges || UnsavedClassChanges || unsavedBattleMessageChanges;

        public async Task BeginSaveRomChangesAsync(IProgress<int> progress, string filePath)
        {
            try
            {
                int count = 0;
                int totalDirectories = VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories.Count;
                int increment = totalDirectories > 0 ? 100 / totalDirectories : 0;

                foreach (var kvp in VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories)
                {
                    DirectoryInfo di = new(kvp.Value.unpackedDirectory);
                    if (di.Exists)
                    {
                        Narc.FromFolder(kvp.Value.unpackedDirectory).Save(kvp.Value.packedDirectory);
                    }

                    count += increment;
                    progress?.Report(count);
                }

                HandleArm9Compression(progress!);
                await HandleOverlayCompressionAsync(progress!);

                await romFileMethods.RepackRomAsync(filePath);

                progress?.Report(100);

                MessageBox.Show("ROM File saved to " + filePath, "Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving ROM file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task BeginUnpackNarcsAsync(IProgress<int> progress)
        {
            try
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
                    progress?.Report(100);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error during NARC unpacking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadingError = true;
                CloseProject();
            }
        }

        public async Task BeginUnpackRomDataAsync()
        {
            try
            {
                var (Success, ExceptionMessage) = await romFileMethods.ExtractRomContentsAsync(RomFile.WorkingDirectory, RomFile.FileName);

                if (!Success)
                {
                    MessageBox.Show($"{ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    loadingError = true;
                    CloseProject();
                }
                else
                {
                    loadingError = false;
                    romLoaded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error during ROM extraction: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loadingError = true;
                CloseProject();
            }
        }

        public static void FilterListBox(ListBox listBox, string filter, List<string> unfiltered)
        {
            listBox.BeginUpdate();

            var filteredList = string.IsNullOrEmpty(filter)
                ? unfiltered
                : unfiltered.Where(item => item.Contains(filter, StringComparison.OrdinalIgnoreCase));

            listBox.Items.Clear();
            listBox.Items.AddRange(filteredList.ToArray());

            listBox.EndUpdate();
        }

        public void GetInitialData(IProgress<int>? progress = null)
        {
            try
            {
                isLoadingData = true;

                const int totalSteps = 11;
                int progressCount = 0;
                const int increment = 100 / totalSteps;

                void ReportProgress()
                {
                    progressCount += increment;
                    progress?.Report(progressCount);
                }

                mainDataModel.PokemonSpecies = romFileMethods.GetSpecies();
                ReportProgress();

                mainDataModel.TrainerNames = new(RomFile.TrainerNames);
                ReportProgress();

                mainDataModel.ClassNames = romFileMethods.GetClassNames(RomFile.ClassNamesTextNumber);
                ReportProgress();

                mainDataModel.ClassDescriptions = romFileMethods.GetClassDescriptions(RomFile.ClassDescriptionMessageNumber);
                ReportProgress();

                mainDataModel.Trainers = trainerEditorMethods.GetTrainers();
                ReportProgress();

                mainDataModel.Classes = classEditorMethods.GetTrainerClasses(mainDataModel.Trainers, mainDataModel.ClassNames, mainDataModel.ClassDescriptions);
                ReportProgress();

                mainDataModel.PokemonNamesFull = romFileMethods.GetPokemonNames(RomFile.PokemonNamesTextNumber);
                mainDataModel.PokemonNames = MainDataModel.SetPokemonNames(mainDataModel.PokemonNamesFull);
                ReportProgress();

                mainDataModel.MoveNames = romFileMethods.GetMoveNames(RomFile.MoveNameTextNumber);
                ReportProgress();

                mainDataModel.AbilityNames = romFileMethods.GetAbilityNames(RomFile.AbilityNamesTextNumber);
                ReportProgress();

                mainDataModel.ItemNames = romFileMethods.GetItemNames(RomFile.ItemNamesTextNumber);
                ReportProgress();
                mainDataModel.PokeBallNames = SetPokeBallNames();
                mainDataModel.TypeNames = SetTypeNames();
                mainDataModel.StatusNames = SetStatusNames();
                mainDataModel.NatureNames = SetNatureNames();
                mainDataModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                ReportProgress();

                progress?.Report(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading initial data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progress?.Report(0);
                throw;
            }
            finally
            {
                isLoadingData = false;
            }
        }

        private List<string> SetPokeBallNames()
        {
            var pokeBalls = new List<string> { "-" };
            pokeBalls.AddRange(mainDataModel.ItemNames.Where(x => x.EndsWith(" Ball") && !x.Contains("Light") && !x.Contains("Smoke") && !x.Contains("Iron")).ToList());
            return pokeBalls;
        }

        private List<string> SetTypeNames()
        {
            var typeNames = new List<string> { "-" };

            return typeNames;
        }

        private List<string> SetStatusNames()
        {
            var statusNames = new List<string> { "-" };

            return statusNames;
        }

        private List<string> SetNatureNames()
        {
            var natures = new List<string> { "-" };

            return natures;
        }

        public async Task LoadRomDataAsync(IProgress<int> progress)
        {
            try
            {
                isLoadingData = true;
                int progressCount = 0;
                const int totalSteps = 13;
                const int increment = 100 / totalSteps;

                void ReportProgress()
                {
                    progressCount += increment;
                    progress?.Report(progressCount);
                }

                // Check HG-Engine
                if (RomFile.GameVersion == GameVersion.HeartGold && File.Exists(Overlay.OverlayFilePath(129)))
                {
                    RomFile.GameVersion = GameVersion.HgEngine;
                }

                RomFile.ScriptFileData = scriptFileMethods.GetScriptFiles();
                ReportProgress();

                RomFile.EventFileData = eventFileMethods.GetEventFiles();
                ReportProgress();

                RomFile.TrainerNames = romFileMethods.GetTrainerNames();
                ReportProgress();

                RomFile.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
                ReportProgress();

                RomFile.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
                ReportProgress();

                RomFile.EventFileData = eventFileMethods.GetEventFiles();
                ReportProgress();

                RomFile.TrainersData = romFileMethods.GetTrainersData();
                ReportProgress();

                RomFile.TrainersPartyData = romFileMethods.GetTrainersPartyData();
                ReportProgress();

                RomFile.TrainerNameMaxByte = romFileMethods.SetTrainerNameMax(RomFile.TrainerNameMaxByteOffset);
                ReportProgress();

                RomFile.TotalNumberOfTrainerClasses = romFileMethods.GetTotalNumberOfTrainerClasses(RomFile.ClassNamesTextNumber);
                ReportProgress();

                RomFile.Arm9Expanded = RomPatches.CheckFilesArm9ExpansionApplied();
                if (RomFile.Arm9Expanded && !RomFile.IsHgEngine)
                {
                    RomFile.PrizeMoneyExpanded = await RomFile.CheckForPrizeMoneyExpansionAsync();
                    RomFile.ClassGenderExpanded = RomFile.CheckForClassGenderExpansion();
                    RomFile.EyeContactExpanded = RomFile.CheckForEyeContactExpansion();
                }

                RomFile.ClassGenderData = RomFile.IsNotDiamondPearl
                    ? romFileMethods.GetClassGenders(RomFile.TotalNumberOfTrainerClasses, RomFile.ClassGenderOffsetToRamAddress)
                    : [];
                ReportProgress();

                RomFile.EyeContactMusicData = romFileMethods.GetEyeContactMusicData();
                ReportProgress();

                RomFile.PrizeMoneyData = await romFileMethods.GetPrizeMoneyDataAsync();
                ReportProgress();

                isLoadingData = false;
                progress?.Report(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading ROM data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isLoadingData = false;
                progress?.Report(0);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static bool CreateDirectory(string workingDirectory)
        {
            try
            {
                if (!Directory.Exists(workingDirectory))
                {
                    Console.WriteLine($"Creating directory {workingDirectory}");
                    Directory.CreateDirectory(workingDirectory);
                    Console.WriteLine($"Created directory {workingDirectory} | Success");
                    return true;
                }
                else
                {
                    MessageBox.Show("Directory already exists.\n\nPlease select a different location or delete the existing directory.",
                        "Directory Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Console.WriteLine($"Directory {workingDirectory} already exists.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to create directory {workingDirectory}.\n\nError: {ex.Message}",
                    "Error Creating Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Error creating directory {workingDirectory}: {ex.Message}");
                return false;
            }
        }

        private void BeginExtractRomData() => OpenLoadingDialog(LoadType.UnpackNarcs);

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
            mainDataModel = new();
            ClearTrainerEditorData();
            class_ClassListBox.SelectedIndex = -1;
            class_ClassListBox.Items.Clear();
            mainDataModel.SelectedTrainerClass = new();
            EnableDisableMenu(false);
            ClearUnsavedChanges();
            isLoadingData = false;
            Console.WriteLine("Projects closed.");
        }

        private static void ConfirmImportTrainers(List<Trainer> newTrainers, List<Trainer> oldTrainers)
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
                    if (confirm != DialogResult.Yes)
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
                    if (confirm != DialogResult.Yes)
                    {
                        ConfirmImportTrainers(newTrainers, oldTrainers);
                    }
                }
            }
        }

        private static bool ConfirmUnsavedChanges()
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

        private string GetPokemonNameById(int pokemonId) => mainDataModel.PokemonNamesFull[pokemonId];

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
                    Arm9.WriteBytes([0, 0, 0, 0], (uint)(RomFile.GameFamily == GameFamily.DiamondPearl ? 0xB7C : 0xBB4));
                    progress?.Report(10);
                }
            }
        }

        private static async Task HandleOverlayCompressionAsync(IProgress<int> progress)
        {
            ArgumentNullException.ThrowIfNull(progress);

            if (Overlay.CheckOverlayHasCompressionFlag(1))
            {
                if (RomPatches.LoadOverlay1FromBackup)
                {
                    var (Success, Error) = await Overlay.RestoreOverlayFromCompressedBackupAsync(1, false);
                    if (!Success)
                    {
                        MessageBox.Show(Error, "Unable to Restore Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void main_SaveRomBtn_Click(object sender, EventArgs e) => SaveRomChanges();

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

        private void menu_File_Save_Click(object sender, EventArgs e) => SaveRomChanges();

        private void menu_Import_BattleMessages_Click(object sender, EventArgs e) => ImportBattleMessageCsv();

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
                var saveChanges = MessageBox.Show(
                    "You have unsaved changes.\n\n" +
                    "You will lose these changes if you close the project.\n" +
                    "Do you still want to close?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (saveChanges == DialogResult.Cancel)
                {
                    // User cancelled the action, stop further processing
                    isLoadingData = false;
                    return;
                }
            }

            CloseProjectAndLoadRom(filePath);

            isLoadingData = false;
        }

        private void CloseProjectAndLoadRom(string filePath)
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

            settingsForm.ClearRecentItemsRequested += SettingsForm_ClearRecentItemsRequested!;

            settingsForm.ShowDialog();
        }

        private void PopulateTrainerClassSprite(PictureBox pictureBox, NumericUpDown frameNumBox, int trainerClassId) => UpdateTrainerClassSprite(pictureBox, frameNumBox, trainerClassId);

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

            string? fileName = Directory.GetFiles(selectedFolder).SingleOrDefault(x => x.Contains(Common.HeaderFilePath));
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
            try
            {
                Console.WriteLine("Reading ROM File...");

                isLoadingData = true;
                Console.WriteLine("Load initial ROM Data");

                var (Success, ErrorMessage) = romFileMethods.LoadInitialRomData(fileName);
                if (!Success)
                {
                    MessageBox.Show(ErrorMessage, "Error Reading ROM File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine($"Error: {ErrorMessage}");
                    return false;
                }

                Console.WriteLine("Load initial ROM Data | Success");

                RomFile.SetupRomFile(fileName, workingDirectory);

                if (RomFile.GameVersion == GameVersion.Unknown)
                {
                    const string errorMessage = "The ROM file you have selected is not supported by VSMaker 2." +
                                          "\n\nVSMaker 2 currently only supports Pokémon Diamond, Pearl, Platinum, " +
                                          "HeartGold or SoulSilver versions.";
                    MessageBox.Show(errorMessage, "Unsupported ROM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("ROM version is unsupported.");
                    return false;
                }

                romFileMethods.SetNarcDirectories(workingDirectory, RomFile.GameVersion, RomFile.GameFamily, RomFile.GameLanguage);

                Console.WriteLine("Reading ROM File | Success");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                isLoadingData = false;
            }
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
                CloseProjectAndLoadRom(selectFolder.SelectedPath);
            }
        }

        private async Task SelectWorkingFolderDirectoryAsync(string fileName)
        {
            using FolderBrowserDialog selectFolder = new()
            {
                Description = "Choose Where to Extract ROM Contents",
                UseDescriptionForTitle = true,
            };

            if (selectFolder.ShowDialog() != DialogResult.OK) return;

            CloseProject();
            string workingDirectory = GetWorkingDirectory(selectFolder.SelectedPath, fileName);
            Config.AddToRecentItems(workingDirectory);
            Config.UpdateRecentItemsMenu(menu_File_OpenRecent, OpenRecentFile);

            if (HandleExistingDirectory(workingDirectory, fileName))
            {
                await HandleRomFileProcessingAsync(workingDirectory, fileName);
            }
        }

        private static string GetWorkingDirectory(string selectedPath, string fileName)
        {
            return $"{selectedPath}\\{Path.GetFileNameWithoutExtension(fileName)}{Common.VsMakerContentsFolder}\\";
        }

        private bool HandleExistingDirectory(string workingDirectory, string fileName)
        {
            if (!Directory.Exists(workingDirectory))
            {
                CreateDirectory(workingDirectory);
                return true;
            }

            var directoryExists = MessageBox.Show(
                "An extracted contents folder for this ROM has been found.\n\nDo you wish to open this folder?",
                "Extracted ROM Data exists",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (directoryExists == DialogResult.Yes)
            {
                CloseProject();
                Console.WriteLine("Opening existing contents folder " + workingDirectory);
                romLoaded = ReadRomExtractedFolder(workingDirectory);
                return true;
            }

            var extractContentsAgain = MessageBox.Show(
                "ROM Data will be re-extracted.\nThis will delete all existing data in the old folder.\n\nDo you wish to proceed?",
                "Re-Extract ROM Data",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (extractContentsAgain == DialogResult.Yes)
            {
                Console.WriteLine("Deleting existing ROM contents folder " + workingDirectory);
                Directory.Delete(workingDirectory, true);
                Console.WriteLine("Deleted folder " + workingDirectory);
                CreateDirectory(workingDirectory);
            }
            else
            {
                _ = SelectWorkingFolderDirectoryAsync(fileName);
                return false;
            }

            return true;
        }

        private async Task HandleRomFileProcessingAsync(string workingDirectory, string fileName)
        {
            if (!ReadRomFile(workingDirectory, fileName))
            {
                CloseProject();
                return;
            }
            OpenLoadingDialog(LoadType.UnpackRom);

            if (!romLoaded || loadingError)
            {
                CloseProject();
                return;
            }

            Arm9.Arm9EditSize(-12);

            if (Arm9.CheckCompressionMark(RomFile.GameFamily) && !await Arm9.Arm9DecompressAsync(RomFile.Arm9Path))
            {
                MessageBox.Show("Unable to decompress Arm9");
                Console.WriteLine("Unable to decompress Arm9");
                return;
            }

            BeginExtractRomData();

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

            if (unsavedBattleMessageChanges && !ConfirmUnsavedChanges())
            {
                inhibitTabChange = true;
                main_MainTab.SelectedTab = main_MainTable_BattleMessageTab;
            }
            else
            {
                HandleTabChange(main_MainTab.SelectedTab!);
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

        private void main_OpenPatchesBtn_Click(object sender, EventArgs e) => OpenRomPatchesWindow();

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

        private void main_SettingsBtn_Click(object sender, EventArgs e) => OpenSettingsWindow();

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
            fileSystemMethods = new FileSystemMethods(romFileMethods, scriptFileMethods);
            eventFileMethods = new EventFileMethods();
            battleMessageEditorMethods = new BattleMessageEditorMethods(romFileMethods);
            trainerEditorMethods = new TrainerEditorMethods(romFileMethods, fileSystemMethods);
            classEditorMethods = new ClassEditorMethods(romFileMethods);
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
            //    var trainers = MainDataModel.Trainers;
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

        private void menu_File_Exit_Click(object sender, EventArgs e) => Application.Exit();

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
                    await OpenRomAsync();
                }
            }
            else
            {
                await OpenRomAsync();
            }
        }

        private void menu_Import_Trainers_Click(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
            }
        }

        private void menu_Tools_RomPatcher_Click(object sender, EventArgs e) => OpenRomPatchesWindow();

        private void menu_Tools_Settings_Click(object sender, EventArgs e) => OpenSettingsWindow();

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