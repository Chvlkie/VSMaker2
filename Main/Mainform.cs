using Main.Forms;
using Main.Models;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using System.Text;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.Methods;
using VsMaker2Core.Methods.NdsImages;
using static VsMaker2Core.Enums;

namespace Main
{
    public partial class Mainform : Form
    {
        private const int debounceDelay = 300;
        private System.Windows.Forms.Timer filterTimer;
        // Delay in milliseconds

        #region Forms

        private LoadingData LoadingData;
        private MoveSelector MoveSelector;
        private RomPatches RomPatches;
        private Settings Settings;

        #endregion Forms

        #region Methods

        private IBattleMessageEditorMethods battleMessageEditorMethods;
        private IClassEditorMethods classEditorMethods;
        private IFileSystemMethods fileSystemMethods;
        private IRomFileMethods romFileMethods;
        private IScriptFileMethods scriptFileMethods;
        private ITrainerEditorMethods trainerEditorMethods;
        private INdsImage imageMethods;

        #endregion Methods

        private bool InhibitTabChange = false;
        private bool IsLoadingData;
        private RomFile LoadedRom;
        private bool LoadingError = false;
        private MainEditorModel MainEditorModel;
        private bool RomLoaded;

        public Mainform()
        {
            InitializeComponent();
            filterTimer = new System.Windows.Forms.Timer();
            filterTimer.Interval = debounceDelay;
            filterTimer.Tick += FilterTimer_Tick;

            startupTab.Appearance = TabAppearance.FlatButtons; startupTab.ItemSize = new Size(0, 1); startupTab.SizeMode = TabSizeMode.Fixed;
            RomLoaded = false;
            romName_Label.Text = "";
            MainEditorModel = new();
            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3); 

            Text = $"VS Maker 2 v {version}";
        }

        private bool UnsavedChanges => UnsavedTrainerEditorChanges || UnsavedClassChanges || UnsavedBattleMessageChanges;

        public async Task BeginExportBattleMessagesAsync(IProgress<int> progress, string filePath)
        {
            try
            {
                using StreamWriter outputFile = new(filePath, false, new UTF8Encoding(true));
                var export = new StringBuilder();
                var headers = battleMessage_MessageTableDataGrid.Columns.Cast<DataGridViewColumn>();

                export.AppendLine(string.Join(",", headers.Select(column => $"\"{column.HeaderText}\"")));
                await outputFile.WriteLineAsync(export.ToString());

                export.Clear();

                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    DataGridViewRow row = battleMessage_MessageTableDataGrid.Rows[i];
                    string messageId = row.Index.ToString();
                    string trainerId = Trainer.ListNameToTrainerId(row.Cells[1].Value.ToString()).ToString();
                    string messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(row.Cells[2].Value.ToString()).ToString();
                    string messageText = row.Cells[3].Value.ToString();

                    export.AppendLine($@"""{messageId}"",""{trainerId}"",""{messageTriggerId}"",""{messageText}""");

                    await outputFile.WriteLineAsync(export.ToString());
                    export.Clear();

                    // Report progress
                    progress?.Report(i + 1);

                    await Task.Yield();
                }

                progress?.Report(battleMessage_MessageTableDataGrid.Rows.Count + 50);

                await Task.Delay(250);

                MessageBox.Show("Battle Message exported successfully.", "Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task BeginImportBattleMessagesAsync(string filePath)
        {
            try
            {
                var newMessages = new List<BattleMessage>();

                using (var parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    if (!parser.EndOfData)
                    {
                        parser.ReadLine();
                    }

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        if (fields.Length != 4)
                        {
                            throw new Exception("Unexpected number of columns");
                        }

                        int messageId = int.Parse(fields[0]);
                        int trainerId = int.Parse(fields[1]);
                        int messageTriggerId = int.Parse(fields[2]);
                        string messageText = fields[3];

                        newMessages.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
                    }
                }

                await Task.Run(() =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            MainEditorModel.BattleMessages = newMessages;
                            LoadBattleMessages();
                            LoadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                            MessageBox.Show("Battle messages imported successfully!", "Success");
                        }));
                    }
                    else
                    {
                        MainEditorModel.BattleMessages = newMessages;
                        LoadBattleMessages();
                        LoadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                        MessageBox.Show("Battle messages imported successfully!", "Success");
                    }
                });
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("An error occurred while reading the file:\n" + ex.Message, "Unable to Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show("An error occurred while reading the file:\n" + ex.Message, "Unable to Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void BeginSaveBattleMessages(IProgress<int> progress)
        {
            var messageTriggers = MessageTrigger.MessageTriggers.ToDictionary(mt => mt.ListName, mt => mt.MessageTriggerId);
            List<BattleMessage> messageData = new List<BattleMessage>(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString().Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()];
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                string messageText = row.Cells[3].Value.ToString();

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }

            messageData = messageData.OrderBy(x => x.MessageId).ToList();
            SaveBattleMessages(messageData, progress);
        }

        public void BeginSaveRomChanges(IProgress<int> progress, string filePath)
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

            HandleArm9Compression(progress);
            HandleOverlayCompressionAsync(progress);

            romFileMethods.RepackRomAsync(filePath);
            progress?.Report(100);
            MessageBox.Show("ROM File saved to " + filePath, "Success!");
        }

        public void BeginSortRepointTrainerText(IProgress<int> progress, int max)
        {
            var messageTriggers = MessageTrigger.MessageTriggers.ToDictionary(mt => mt.ListName, mt => mt.MessageTriggerId);
            List<BattleMessage> messageData = new List<BattleMessage>(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString().Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()];
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                string messageText = row.Cells[3].Value.ToString();

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }
            messageData = messageData.OrderBy(x => x.TrainerId).ThenBy(x => x.MessageTriggerId).ToList();
            SaveBattleMessages(messageData, progress);
            RepointBattleMessageOffsets(messageData, progress);
            progress?.Report(max);
        }

        public async Task BeginUnpackNarcsAsync(IProgress<int> progress)
        {
            var narcs = GameFamilyNarcs.GetGameFamilyNarcs(RomFile.GameFamily);

            var (success, exception) = await romFileMethods.UnpackNarcsAsync(narcs, progress);
            if (!success)
            {
                MessageBox.Show(exception, "Unable to Unpack NARCs", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadingError = true;
                CloseProject();
            }
            else
            {
                LoadingError = false;
            }
            progress?.Report(100);
            return;
        }

        public async Task BeginUnpackRomDataAsync()
        {
            var unpack = await romFileMethods.ExtractRomContentsAsync(RomFile.WorkingDirectory, LoadedRom.FileName);
            if (!unpack.Success)
            {
                MessageBox.Show($"{unpack.ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadingError = true;
                CloseProject();
            }
            else
            {
                LoadingError = false;
            }
            RomLoaded = true;
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
            IsLoadingData = true;
            int progressCount = 0;
            const int increment = 100 / 11;

            MainEditorModel.PokemonSpecies = await romFileMethods.GetSpeciesAsync();
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.TrainerNames = await romFileMethods.GetTrainerNamesAsync(LoadedRom.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ClassNames = await romFileMethods.GetClassNamesAsync(LoadedRom.ClassNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ClassDescriptions = await romFileMethods.GetClassDescriptionsAsync(LoadedRom.ClassDescriptionMessageNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.Trainers = trainerEditorMethods.GetTrainers(MainEditorModel.TrainerNames, LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.Classes = classEditorMethods.GetTrainerClasses(MainEditorModel.Trainers, MainEditorModel.ClassNames, MainEditorModel.ClassDescriptions, LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.PokemonNamesFull = await romFileMethods.GetPokemonNamesAsync(LoadedRom.PokemonNamesTextNumber);
            MainEditorModel.PokemonNames = MainEditorModel.SetPokemonNames(MainEditorModel.PokemonNamesFull);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.MoveNames = await romFileMethods.GetMoveNamesAsync(LoadedRom.MoveNameTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.AbilityNames = await romFileMethods.GetAbilityNamesAsync(LoadedRom.AbilityNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ItemNames = await romFileMethods.GetItemNamesAsync(LoadedRom.ItemNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(LoadedRom.BattleMessageTableData, LoadedRom.BattleMessageTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            IsLoadingData = false;
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
            IsLoadingData = true;
            int progressCount = 0;
            int increment = 100 / 11;

            LoadedRom.ScriptFileData = scriptFileMethods.GetScriptFiles();
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TotalNumberOfTrainers = await romFileMethods.GetTotalNumberOfTrainersAsync(LoadedRom.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.BattleMessageTableData = await romFileMethods.GetBattleMessageTableDataAsync(RomFile.BattleMessageTablePath);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.BattleMessageOffsetData = await romFileMethods.GetBattleMessageOffsetDataAsync(RomFile.BattleMessageOffsetPath);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TrainersData = await romFileMethods.GetTrainersDataAsync(LoadedRom.TotalNumberOfTrainers);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TrainersPartyData = await romFileMethods.GetTrainersPartyDataAsync(LoadedRom.TotalNumberOfTrainers, LoadedRom.TrainersData, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TrainerNameMaxByte = romFileMethods.SetTrainerNameMax(LoadedRom.TrainerNameMaxByteOffset);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TotalNumberOfTrainerClasses = await romFileMethods.GetTotalNumberOfTrainerClassesAsync(LoadedRom.ClassNamesTextNumber);
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
            LoadedRom.ClassGenderData = RomFile.GameFamily != GameFamily.DiamondPearl ? await romFileMethods.GetClassGendersAsync(RomFile.TotalNumberOfTrainerClasses, RomFile.ClassGenderOffsetToRamAddress) : [];
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.EyeContactMusicData = await romFileMethods.GetEyeContactMusicDataAsync(RomFile.EyeContactMusicTableOffsetToRam, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.PrizeMoneyData = await romFileMethods.GetPrizeMoneyDataAsync(LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount);

            IsLoadingData = false;
            progress?.Report(100);
        }

        public void RefreshTrainerClasses()
        {
            MainEditorModel.Classes = classEditorMethods.GetTrainerClasses(MainEditorModel.Trainers, MainEditorModel.ClassNames, MainEditorModel.ClassDescriptions, LoadedRom);
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
            using (EasyReader reader = new(filePath, 0xC))
            {
                string gameCode = Encoding.UTF8.GetString(reader.ReadBytes(4));
                reader.BaseStream.Position = 0x1E;
                byte europeByte = reader.ReadByte();
                return (europeByte, gameCode);
            }
        }

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
                    "Continue?", "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                OpenLoadingDialog(LoadType.LoadRomData);
                InitializeTrainerEditor();
                InitializeClassEditor();
                InitializeBattleMessageEditor();
                OpenLoadingDialog(LoadType.SetupEditor);
                main_MainTab.SelectedTab = main_MainTab_TrainerTab;
                SetupTrainerEditor();
            }
            IsLoadingData = false;
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
            return MainEditorModel.PokemonNamesFull[pokemonId];
        }

        private void HandleArm9Compression(IProgress<int> progress)
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

            if (Overlay.CheckOverlayHasCompressionFlag(LoadedRom.InitialMoneyOverlayNumber) &&
                !await Overlay.CheckOverlayIsCompressedAsync(LoadedRom.InitialMoneyOverlayNumber))
            {
                await Overlay.CompressOverlayAsync(LoadedRom.InitialMoneyOverlayNumber);
            }
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
            if (InhibitTabChange)
            {
                InhibitTabChange = false;
                return;
            }

            if (UnsavedBattleMessageChanges && !ConfirmUnsavedChanges())
            {
                InhibitTabChange = true;
                main_MainTab.SelectedTab = main_MainTable_BattleMessageTab;
            }
            else
            {
                HandleTabChange(main_MainTab.SelectedTab);
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
                }
            }
            else
            {
                SelectExtractedRomFolder();
            }
            IsLoadingData = false;
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
            scriptFileMethods = new ScriptFileMethods();
            battleMessageEditorMethods = new BattleMessageEditorMethods();
            trainerEditorMethods = new TrainerEditorMethods();
            classEditorMethods = new ClassEditorMethods();
            fileSystemMethods = new FileSystemMethods();
            imageMethods = new NdsImage();
        }

        private void menu_Export_Trainers_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                var trainers = MainEditorModel.Trainers;
                var gameFamily = RomFile.GameFamily;
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
            System.Windows.Forms.Application.Exit();
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

        #endregion Event Handlers

        private void main_SaveRomBtn_Click(object sender, EventArgs e)
        {
            SaveRomChanges();
        }

        private void menu_Export_BattleMessages_Click(object sender, EventArgs e)
        {
            if (battleMessage_MessageTableDataGrid.RowCount == 0)
            {
                LoadBattleMessages();
            }
            ExportBattleMessagesAsCsv();
        }

        private void menu_Import_BattleMessages_Click(object sender, EventArgs e)
        {
            ImportBattleMessageCsv();
        }

        private void OpenLoadingDialog(LoadType loadType)
        {
            UseWaitCursor = true;
            LoadingData = new(this, loadType);
            LoadingData.ShowDialog();
            UseWaitCursor = false;
        }

        private void OpenLoadingDialog(LoadType loadType, string filePath)
        {
            UseWaitCursor = true;
            LoadingData = new(this, loadType, filePath);
            LoadingData.ShowDialog();
            UseWaitCursor = false;
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
            RomPatches = new RomPatches(this, LoadedRom, romFileMethods);
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
            if (RomFile.GameVersion == GameVersion.Unknown)
            {
                MessageBox.Show("The ROM file you have selected is not supported by VSMaker 2." +
                    "\n\nVSMaker 2 currently only support Pokémon Diamond, Pearl, Platinum, HeartGold or Soul Silver version."
                    , "Unsupported ROM",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            romFileMethods.SetNarcDirectories(workingDirectory, RomFile.GameVersion, RomFile.GameFamily, RomFile.GameLanguage);
            return true;
        }

        private void SaveRomChanges()
        {
            var save = new SaveFileDialog()
            {
                Filter = Common.NdsRomFilter
            };
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

            if (selectFolder.ShowDialog() == DialogResult.OK)
            {
                CloseProject();
                RomLoaded = ReadRomExtractedFolder(selectFolder.SelectedPath);
                if (RomLoaded)
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
                string workingDirectory = $"{selectFolder.SelectedPath}\\{Path.GetFileNameWithoutExtension(fileName)}{Common.VsMakerContentsFolder}\\";

                if (Directory.Exists(workingDirectory))
                {
                    var directoryExists = MessageBox.Show("An extracted contents folder for this ROM has been found." +
                        "\n\nDo you wish to open this folder?", "Extracted ROM Data exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (directoryExists == DialogResult.Yes)
                    {
                        CloseProject();
                        RomLoaded = ReadRomExtractedFolder(workingDirectory);
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

                        if (RomLoaded && !LoadingError)
                        {
                            Arm9.Arm9EditSize(-12);

                            // Check for compression mark and await the asynchronous decompression
                            if (Arm9.CheckCompressionMark(RomFile.GameFamily))
                            {
                                // Await the Arm9DecompressAsync method
                                if (!await Arm9.Arm9DecompressAsync(RomFile.Arm9Path))
                                {
                                    MessageBox.Show("Unable to decompress Arm9");
                                    return;
                                }
                            }

                            BeginExtractRomData();
                        }

                        if (!LoadingError)
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

        private void trainer_Copy_Btn_Click(object sender, EventArgs e)
        {
            MainEditorModel.ClipboardTrainer = new Trainer(MainEditorModel.SelectedTrainer);
            MainEditorModel.ClipboardTrainerProperties = new TrainerProperty(MainEditorModel.SelectedTrainer.TrainerProperties);
            MainEditorModel.ClipboardTrainerParty = new TrainerParty(MainEditorModel.SelectedTrainer.TrainerParty);
            trainer_Paste_Btn.Enabled = true;
            trainer_PastePropeties_btn.Enabled = true;
            trainer_PasteParty_btn.Enabled = true;
        }

        private void trainer_CopyParty_btn_Click(object sender, EventArgs e)
        {
            MainEditorModel.ClipboardTrainerParty = new TrainerParty(MainEditorModel.SelectedTrainer.TrainerParty)
            {
                ChooseItems = MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems,
                ChooseMoves = MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves,
                DoubleBattle = MainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle,
                TeamSize = MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize,
            };
            trainer_PasteParty_btn.Enabled = true;
        }

        private void trainer_CopyProperties_btn_Click(object sender, EventArgs e)
        {
            MainEditorModel.ClipboardTrainerProperties = new TrainerProperty(MainEditorModel.SelectedTrainer.TrainerProperties);
            trainer_PastePropeties_btn.Enabled = true;
        }

        private void trainer_InsertE_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(trainer_MessageTextBox, "é");
        }

        private void trainer_InsertF_Btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(trainer_MessageTextBox, "\\f");
        }

        private void trainer_InsertN_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(trainer_MessageTextBox, "\\n");
        }

        private void trainer_InsertR_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(trainer_MessageTextBox, "\\r");
        }

        private void trainer_Paste_Btn_Click(object sender, EventArgs e)
        {
            int selectedTrainerId = MainEditorModel.SelectedTrainer.TrainerId;
            var pasteTrainer = new Trainer(selectedTrainerId, MainEditorModel.ClipboardTrainer);

            PopulateTrainerData(pasteTrainer);
            PopulatePartyData(pasteTrainer.TrainerParty, pasteTrainer.TrainerProperties.TeamSize, pasteTrainer.TrainerProperties.ChooseMoves);
            PopulateTrainerBattleMessageTriggers(pasteTrainer);
            EnableTrainerEditor();
            EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
            EditedTrainerData(true);
        }

        private void trainer_PasteParty_btn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            if (MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize != MainEditorModel.ClipboardTrainerParty.TeamSize
                                || MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != MainEditorModel.ClipboardTrainerParty.ChooseItems
                || MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != MainEditorModel.ClipboardTrainerParty.ChooseMoves
                || MainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle != MainEditorModel.ClipboardTrainerParty.DoubleBattle
                )
            {
                var pasteProperties = new TrainerProperty(MainEditorModel.SelectedTrainer.TrainerProperties, MainEditorModel.ClipboardTrainerParty.DoubleBattle,
                   MainEditorModel.ClipboardTrainerParty.TeamSize,
                    MainEditorModel.ClipboardTrainerParty.ChooseMoves,
                   MainEditorModel.ClipboardTrainerParty.ChooseItems);
                SetTrainerPartyProperties(pasteProperties);
                EditedTrainerProperty(true);
            }
            else
            {
            }
            IsLoadingData = false;
            PopulatePartyData(MainEditorModel.ClipboardTrainerParty, MainEditorModel.ClipboardTrainerParty.TeamSize, MainEditorModel.ClipboardTrainerParty.ChooseMoves);
            EditedTrainerParty(true);
            EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
        }

        private void trainer_PastePropeties_btn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            if (MainEditorModel.SelectedTrainer.TrainerProperties.TeamSize != MainEditorModel.ClipboardTrainerProperties.TeamSize
                                || MainEditorModel.SelectedTrainer.TrainerProperties.ChooseItems != MainEditorModel.ClipboardTrainerProperties.ChooseItems
                || MainEditorModel.SelectedTrainer.TrainerProperties.ChooseMoves != MainEditorModel.ClipboardTrainerProperties.ChooseMoves
                || MainEditorModel.SelectedTrainer.TrainerProperties.DoubleBattle != MainEditorModel.ClipboardTrainerProperties.DoubleBattle
                || MainEditorModel.SelectedTrainer.TrainerProperties.Items != MainEditorModel.ClipboardTrainerProperties.Items
                || MainEditorModel.SelectedTrainer.TrainerProperties.AIFlags != MainEditorModel.ClipboardTrainerProperties.AIFlags
                )
            {
                var pasteProperties = new TrainerProperty(MainEditorModel.ClipboardTrainerProperties);
                SetTrainerProperties(pasteProperties);
                EditedTrainerProperty(true);
            }

            IsLoadingData = false;
            PopulatePartyData(MainEditorModel.SelectedTrainer.TrainerParty, MainEditorModel.ClipboardTrainerProperties.TeamSize, MainEditorModel.ClipboardTrainerProperties.ChooseMoves);
            EditedTrainerParty(true);
            EnableDisableParty((byte)trainer_TeamSizeNum.Value, trainer_HeldItemsCheckbox.Checked, trainer_ChooseMovesCheckbox.Checked);
        }

        private void trainerEditor_SaveMessage_Click(object sender, EventArgs e)
        {
            int trainerId = MainEditorModel.SelectedTrainer.TrainerId;
            int messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(trainer_MessageTriggerListBox!.SelectedItem.ToString());
            var message = MainEditorModel.BattleMessages.SingleOrDefault(x => x.TrainerId == trainerId && x.MessageTriggerId == messageTriggerId);
            if (SaveTrainerMessage(message.MessageId))
            {
                if (battleMessage_MessageTableDataGrid.Rows.Count > 0)
                {
                    var row = battleMessage_MessageTableDataGrid.Rows.Cast<DataGridViewRow>()
                        .SingleOrDefault(x => x.Cells[1].Value.ToString() == MainEditorModel.SelectedTrainer.ListName
                        && x.Cells[2].Value.ToString() == trainer_MessageTriggerListBox!.SelectedItem.ToString());

                    if (row != default)
                    {
                        row.Cells[3].Value = message.MessageText;
                    }
                }
            }
        }

        private (bool Valid, int Row) VerifyBattleMessageTable()
        {
            foreach (var trainer in MainEditorModel.Trainers)
            {
                var checkMessages = new List<string>();
                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    var selectedMessageTrigger = battleMessage_MessageTableDataGrid.Rows[i].Cells[2].Value.ToString();
                    var selectedTrainer = battleMessage_MessageTableDataGrid.Rows[i].Cells[1].Value.ToString();
                    int trainerId = int.Parse(selectedTrainer.Remove(0, 1).Remove(4));
                    if (trainerId == trainer.TrainerId)
                    {
                        if (checkMessages.Contains(selectedMessageTrigger))
                        {
                            return (false, i);
                        }
                        else
                        {
                            checkMessages.Add(selectedMessageTrigger);
                        }
                    }
                }
            }

            return (true, -1);
        }

        private void PopulateTrainerClassSprite(PictureBox pictureBox, NumericUpDown frameNumBox, int trainerClassId)
        {
            UpdateTrainerClassSprite(pictureBox, frameNumBox, trainerClassId);
        }

        private void UpdateTrainerClassSprite(PictureBox pictureBox, NumericUpDown frameNumBox, int trainerClassId)
        {
            var palette = imageMethods.GetTrainerClassPaletteBase(trainerClassId);
            var image = imageMethods.GetTrainerClassImageBase(trainerClassId);
            var sprite = imageMethods.GetTrainerClassSpriteBase(trainerClassId);
            frameNumBox.Enabled = sprite.NumBanks > 1;
            frameNumBox.Maximum = sprite.NumBanks - 1;
            if (frameNumBox.Value > frameNumBox.Maximum)
            {
                frameNumBox.Value = 0;
            }
            int frame = (int)(frameNumBox.Enabled ? frameNumBox.Value : 0);
            pictureBox.Image = imageMethods.GetTrainerClassSrite(palette, image, sprite, frame);
        }

        private void class_SpriteFrameNum_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                int trainerClassId = TrainerClass.ListNameToTrainerClassId(class_ClassListBox.SelectedItem.ToString());
                UpdateTrainerClassSprite(class_SpritePicBox, class_SpriteFrameNum, trainerClassId);
            }
        }
    }
}