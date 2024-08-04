using Main.Forms;
using Main.Models;
using System.Text;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.Methods;
using VsMaker2Core.RomFiles;
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

        private IBattleMessageEditorMethods battleMessageEditorMethods;
        private IClassEditorMethods classEditorMethods;
        private IFileSystemMethods fileSystemMethods;
        private IRomFileMethods romFileMethods;
        private ITrainerEditorMethods trainerEditorMethods;
        private IScriptFileMethods scriptFileMethods;

        #endregion Methods

        private bool InhibitTabChange = false;
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

        public void BeginSortRepointTrainerText(IProgress<int> progress, int max)
        {
            List<BattleMessage> messageData = [];
            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                string selectedTrainer = row.Cells[1].Value.ToString();
                string selectedMessageTrigger = row.Cells[2].Value.ToString();
                string messageText = row.Cells[3].Value.ToString();
                int trainerId = int.Parse(selectedTrainer.Remove(0, 1).Remove(4));
                int messageTriggerId = MessageTrigger.MessageTriggers.Find(x => x.ListName == selectedMessageTrigger).MessageTriggerId;
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }
            messageData = [.. messageData.OrderBy(x => x.TrainerId).ThenBy(x => x.MessageTriggerId)];
            SaveBattleMessages(messageData, progress);
            RepointBattleMessageOffsets(messageData, progress);
        }

        public void BeginSaveBattleMessages(IProgress<int> progress)
        {
            List<BattleMessage> messageData = [];
            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                string selectedTrainer = row.Cells[1].Value.ToString();
                string selectedMessageTrigger = row.Cells[2].Value.ToString();
                string messageText = row.Cells[3].Value.ToString();
                int trainerId = int.Parse(selectedTrainer.Remove(0, 1).Remove(4));
                int messageTriggerId = MessageTrigger.MessageTriggers.Find(x => x.ListName == selectedMessageTrigger).MessageTriggerId;
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }

            messageData = messageData.OrderBy(x => x.MessageId).ToList();
            SaveBattleMessages(messageData, progress);
        }

        public void BeginSaveRomChanges(IProgress<int> progress, string filePath)
        {
            int count = 0;
            int increment = 100 / VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories.Count;
            // Repack NARCs
            foreach (KeyValuePair<NarcDirectory, (string packedDirectory, string unpackedDirectory)> kvp in VsMaker2Core.Database.VsMakerDatabase.RomData.GameDirectories)
            {
                DirectoryInfo di = new(kvp.Value.unpackedDirectory);
                if (di.Exists)
                {
                    Narc.FromFolder(kvp.Value.unpackedDirectory).Save(kvp.Value.packedDirectory); // Make new NARC from folder
                }
                progress?.Report(count += increment);
            }

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
                    for (int i = 0; i < 100; i += 10)
                    {
                        progress?.Report(i);
                    }
                }
            }

            progress?.Report(0);

            if (Overlay.CheckOverlayHasCompressionFlag(1))
            {
                if (RomPatches.LoadOverlay1FromBackup)
                {
                    var restore = Overlay.RestoreOverlayFromCompressedBackup(1, false);
                    if (!restore.Success)
                    {
                        MessageBox.Show(restore.Error, "Unable to Restore Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    if (!Overlay.CheckOverlayIsCompressed(1))
                    {
                        Overlay.CompressOverlay(1);
                    }
                }
            }

            if (Overlay.CheckOverlayHasCompressionFlag(LoadedRom.InitialMoneyOverlayNumber) && !Overlay.CheckOverlayIsCompressed(LoadedRom.InitialMoneyOverlayNumber))
            {
                Overlay.CompressOverlay(LoadedRom.InitialMoneyOverlayNumber);
            }
            progress?.Report(90);

            if (RomFile.GameFamily == GameFamily.HeartGoldSoulSilver || RomFile.GameFamily == GameFamily.HeartGoldSoulSilver)
            {
                if (Overlay.CheckOverlayIsCompressed(LoadedRom.PrizeMoneyTableOverlayNumber))
                {
                    Overlay.CompressOverlay(LoadedRom.PrizeMoneyTableOverlayNumber);
                }
            }
            romFileMethods.RepackRom(filePath);

            if (RomFile.GameFamily == GameFamily.HeartGoldSoulSilver || RomFile.GameFamily == GameFamily.HgEngine)
            {
                if (Overlay.CheckOverlayIsCompressed(1))
                {
                    Overlay.DecompressOverlay(1);
                }
            }

            progress?.Report(100);
            MessageBox.Show("ROM File saved to " + filePath, "Success!");
        }

        public void BeginUnpackNarcs(IProgress<int> progress)
        {
            var narcs = GameFamilyNarcs.GetGameFamilyNarcs(RomFile.GameFamily);

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
            var unpack = romFileMethods.ExtractRomContents(RomFile.WorkingDirectory, LoadedRom.FileName);
            if (!unpack.Success)
            {
                MessageBox.Show($"{unpack.ExceptionMessage}", "Unable to Extract ROM Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseProject();
                return;
            }
            RomLoaded = true;
        }

        public void GetInitialData(IProgress<int> progress = null)
        {
            IsLoadingData = true;
            int progressCount = 0;
            const int increment = 10;

            MainEditorModel.PokemonSpecies = romFileMethods.GetSpecies();
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.TrainerNames = romFileMethods.GetTrainerNames(LoadedRom.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ClassNames = romFileMethods.GetClassNames(LoadedRom.ClassNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ClassDescriptions = romFileMethods.GetClassDescriptions(LoadedRom.ClassDescriptionMessageNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.Trainers = trainerEditorMethods.GetTrainers(MainEditorModel.TrainerNames, LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.Classes = classEditorMethods.GetTrainerClasses(MainEditorModel.Trainers, MainEditorModel.ClassNames, MainEditorModel.ClassDescriptions, LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount); MainEditorModel.PokemonNames = romFileMethods.GetPokemonNames(LoadedRom.PokemonNamesTextNumber);

            MainEditorModel.MoveNames = romFileMethods.GetMoveNames(LoadedRom.MoveNameTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.AbilityNames = romFileMethods.GetAbilityNames(LoadedRom.AbilityNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.ItemNames = romFileMethods.GetItemNames(LoadedRom.ItemNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            MainEditorModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(LoadedRom.BattleMessageTableData, LoadedRom.BattleMessageTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            IsLoadingData = false;
            progress?.Report(100);
        }

        public void LoadRomData(IProgress<int> progress)
        {
            IsLoadingData = true;
            int progressCount = 0;
            int increment = 100 / 11;

            LoadedRom.ScriptFileData = scriptFileMethods.GetScriptFiles();
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TotalNumberOfTrainers = romFileMethods.GetTotalNumberOfTrainers(LoadedRom.TrainerNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TrainersData = romFileMethods.GetTrainersData(LoadedRom.TotalNumberOfTrainers);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TrainersPartyData = romFileMethods.GetTrainersPartyData(LoadedRom.TotalNumberOfTrainers, LoadedRom.TrainersData, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            RomFile.TrainerNameMaxByte = romFileMethods.SetTrainerNameMax(LoadedRom.TrainerNameMaxByteOffset);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.TotalNumberOfTrainerClasses = romFileMethods.GetTotalNumberOfTrainerClassess(LoadedRom.ClassNamesTextNumber);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.ClassGenderData = RomFile.GameFamily != GameFamily.DiamondPearl ? romFileMethods.GetClassGenders(LoadedRom.TotalNumberOfTrainerClasses, LoadedRom.ClassGenderOffsetToRamAddress) : [];
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.EyeContactMusicData = romFileMethods.GetEyeContactMusicData(LoadedRom.EyeContactMusicTableOffsetToRam, RomFile.GameFamily);
            progressCount += increment;
            progress?.Report(progressCount);

            LoadedRom.PrizeMoneyData = romFileMethods.GetPrizeMoneyData(LoadedRom);
            progressCount += increment;
            progress?.Report(progressCount);

            IsLoadingData = false;
            progress?.Report(100);
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

        private string GetPokemonNameById(int pokemonId)
        {
            return MainEditorModel.PokemonNames[pokemonId];
        }

        #region Event Handlers

        private void main_MainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!InhibitTabChange)
            {
                if (UnsavedBattleMessageChanges && ConfirmUnsavedChanges())
                {
                    UndoBattleMessageChanges(false);
                    InhibitTabChange = false;
                }
                else if (UnsavedBattleMessageChanges)
                {
                    InhibitTabChange = true;
                    main_MainTab.SelectedTab = main_MainTable_BattleMessageTab;
                }
                if (main_MainTab.SelectedTab == main_MainTab_TrainerTab)
                {
                    SetupTrainerEditor();
                }
                else if (main_MainTab.SelectedTab == main_MainTab_ClassTab)
                {
                    SetupClassEditor();
                }
                else if (main_MainTab.SelectedTab == main_MainTable_BattleMessageTab)
                {
                    SetupBattleMessageEditor();
                }
            }
            else
            {
                InhibitTabChange = false;
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
                }
            }
            else
            {
                OpenRom();
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
                }
            }
            else
            {
                OpenRom();
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

        private void OpenRom()
        {
            OpenFileDialog openRom = new()
            {
                Filter = Common.NdsRomFilter
            };

            if (openRom.ShowDialog(this) == DialogResult.OK && !string.IsNullOrEmpty(openRom.FileName))
            {
                SelectWorkingFolderDirectory(openRom.FileName);
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
                        if (RomLoaded)
                        {
                            Arm9.Arm9EditSize(-12);

                            if (Arm9.CheckCompressionMark(RomFile.GameFamily))
                            {
                                if (!Arm9.Arm9Decompress(RomFile.Arm9Path))
                                {
                                    MessageBox.Show("Unable to decompress Arm9");
                                    return;
                                }
                            }
                            BeginExtractRomData();
                            InitializeTrainerEditor();
                            InitializeClassEditor();
                            EndOpenRom();
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

        private void battleMessages_AddLineBtn_Click(object sender, EventArgs e)
        {
            int index = battleMessage_MessageTableDataGrid.Rows.Count - 1;

            // Get current selected row index - Can't multi-select
            if (battleMessage_MessageTableDataGrid.SelectedRows.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedRows[0].Index;
            }
            else if (battleMessage_MessageTableDataGrid.SelectedCells.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedCells[0].RowIndex;
            }
            string[] currentTrainers = new string[MainEditorModel.Trainers.Count];
            string[] currentMessageTriggers = new string[MessageTrigger.MessageTriggers.Count];

            for (int i = 0; i < MainEditorModel.Trainers.Count; i++)
            {
                currentTrainers[i] = MainEditorModel.Trainers[i].ListName;
            }

            for (int i = 0; i < MessageTrigger.MessageTriggers.Count; i++)
            {
                currentMessageTriggers[i] = MessageTrigger.MessageTriggers[i].ListName;
            }

            DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
            row.Cells[0].Value = battleMessage_MessageTableDataGrid.Rows.Count;
            row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[0] };
            row.Cells[2] = new DataGridViewComboBoxCell { DataSource = currentMessageTriggers, Value = currentMessageTriggers[0] };
            row.Cells[3].Value = "";
            ThreadSafeDataTable(row, index + 1);
            battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = index > 0 ? index - 1 : index;
            battleMessage_MessageTableDataGrid.ClearSelection();
            battleMessage_MessageTableDataGrid.Rows[index + 1].Selected = true;
            EditedBattleMessage(true);
        }

        private void battleMessages_RemoveBtn_Click(object sender, EventArgs e)
        {
            int index = 0;

            if (battleMessage_MessageTableDataGrid.SelectedRows.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedRows[0].Index;
            }
            else if (battleMessage_MessageTableDataGrid.SelectedCells.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedCells[0].RowIndex;
            }
            battleMessage_MessageTableDataGrid.Rows.RemoveAt(index);
            battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = index > 0 ? index - 1 : index;
            battleMessage_MessageTableDataGrid.ClearSelection();
            EditedBattleMessage(true);
            battleMessages_RemoveBtn.Enabled = false;
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

        private void battleMessages_SaveBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            battleMessage_MessageTableDataGrid.EndEdit();
            var verify = VerifyBattleMessageTable();
            if (!verify.Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + verify.Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = verify.Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[verify.Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("Save all changes to Battle Messsage Table?\nThis might take some time...", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.No)
                {
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    BattleMessageCount = battleMessage_MessageTableDataGrid.RowCount;
                    OpenLoadingDialog(LoadType.SaveTrainerTextTable);
                    LoadedRom.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
                    LoadedRom.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
                    MainEditorModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(LoadedRom.BattleMessageTableData, LoadedRom.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                }
            }
            IsLoadingData = false;
        }

        private void battleMessages_SortBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;

            battleMessage_MessageTableDataGrid.EndEdit();
            var verify = VerifyBattleMessageTable();
            if (!verify.Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + verify.Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = verify.Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[verify.Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("This will sort the Battle Message table to group Trainers.\n\nThe Battle Messagex lookup table will also be sorted.\nThis allows for more efficient loading in-game.\n\nAll changes will be saved.", "Sort and Repoint", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.OK)
                {
                    BattleMessageCount = battleMessage_MessageTableDataGrid.Rows.Count;
                    OpenLoadingDialog(LoadType.RepointTextTable);
                    LoadedRom.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
                    LoadedRom.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
                    MainEditorModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(LoadedRom.BattleMessageTableData, LoadedRom.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                }
            }
            IsLoadingData = false;
        }
    }
}