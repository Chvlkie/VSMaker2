using Data.DataModels.Main;
using Data.DataModels.Rom;
using Data.Global.Constants;
using Methods.FileHandling;
using Methods.Rom;
using System;
using static Data.Global.Enums;

namespace Main
{
    public partial class MainForm : Form
    {
        public MainData mainData;
        private bool IsLoadingData = false;
        private IRomMethods romMethods;
        private IFileMethods fileMethods;

        #region Trainer Editor

        public ComboBox[] AbilitySlotComboBoxes = new ComboBox[6];
        public ComboBox[] BallSealComboBoxes = new ComboBox[6];
        public NumericUpDown[] DvNumBoxes = new NumericUpDown[6];
        public ComboBox[] FormComboBoxes = new ComboBox[6];
        public NumericUpDown[] FormNumBoxes = new NumericUpDown[6];
        public ComboBox[] HeldItemComboBoxes = new ComboBox[6];
        public NumericUpDown[] LevelNumBoxes = new NumericUpDown[6];
        public ComboBox[] Move1ComboBoxes = new ComboBox[6];
        public ComboBox[] Move2ComboBoxes = new ComboBox[6];
        public ComboBox[] Move3ComboBoxes = new ComboBox[6];
        public ComboBox[] Move4ComboBoxes = new ComboBox[6];
        public ComboBox[] PokeGenderComboBoxes = new ComboBox[6];
        public PictureBox[] PokeSpritePicBoxes = new PictureBox[6];
        public ComboBox[] SpeciesComboBoxes = new ComboBox[6];

        #region HG Engine

        public ComboBox[] AbilityOverrideComboBoxes = new ComboBox[6];
        public CheckedListBox[] AdditionalFlagCheckListBox = new CheckedListBox[6];
        public ComboBox[] NatureComboBoxes = new ComboBox[6];
        public TextBox[] NicknameTextBoxes = new TextBox[6];
        public ComboBox[] PokeBallComboBoxes = new ComboBox[6];
        public NumericUpDown[] Pp1NumBoxes = new NumericUpDown[6];
        public NumericUpDown[] Pp2NumBoxes = new NumericUpDown[6];
        public NumericUpDown[] Pp3NumBoxes = new NumericUpDown[6];
        public NumericUpDown[] Pp4NumBoxes = new NumericUpDown[6];
        public Button[] SetIvEvButtons = new Button[6];
        public Button[] SetStatButtons = new Button[6];
        public CheckBox[] ShinyCheckBoxes = new CheckBox[6];
        public ComboBox[] StatusEffectComboBoxes = new ComboBox[6];
        public ComboBox[] Type1ComboBoxes = new ComboBox[6];
        public ComboBox[] Type2ComboBoxes = new ComboBox[6];

        #endregion HG Engine

        #endregion Trainer Editor

        public MainForm()
        {
            SuspendLayout();
            InitializeComponent();
            InitializeDisableMenus();
            MinimumSize = new Size(800, 600);
            AutoScaleMode = AutoScaleMode.Dpi;
            ResumeLayout(false);
            PerformLayout();
            romMethods = new RomMethods();
            fileMethods = new FileMethods();
        }

        public void InitializeDisableMenus()
        {
            tabControl_Main.Enabled = false;
            menu_SaveAll.Enabled = false;
            menu_SaveRom.Enabled = false;
            menu_Import.Enabled = false;
            menu_Export.Enabled = false;
            menu_RomPatches.Enabled = false;
            menu_CloseProject.Enabled = false;
            ts_SaveAll.Enabled = false;
            ts_SaveRom.Enabled = false;
            ts_Import.Enabled = false;
            ts_Export.Enabled = false;
            ts_RomPatches.Enabled = false;
        }

        public void EnableMainMenus()
        {
            menu_SaveRom.Enabled = true;
            menu_Import.Enabled = true;
            menu_Export.Enabled = true;
            menu_RomPatches.Enabled = true;
            menu_CloseProject.Enabled = true;
            ts_SaveRom.Enabled = true;
            ts_Import.Enabled = true;
            ts_Export.Enabled = true;
            ts_RomPatches.Enabled = true;
        }

        private async Task BeginUnpackRom(FileData fileData, bool useExistingFolder)
        {
            if (!useExistingFolder)
            {
                status_InfoLabel.Text = "Extracting ROM contents...";
                status_ProgressBar.Style = ProgressBarStyle.Continuous;
                status_ProgressBar.Visible = true;
                this.Enabled = false;

                await romMethods.ExtractRomContentsAsync(fileData);

                status_InfoLabel.Text = "Extraction complete!";
                status_ProgressBar.Style = ProgressBarStyle.Blocks;
                status_ProgressBar.Value = 100;
            }
            else
            {
                status_InfoLabel.Text = "Using existing extracted files";
                status_ProgressBar.Visible = false;
            }
        }

        private async Task BeginUnpackNarcs(FileData fileData, RomData romData)
        {
            var progress = new Progress<int>(percent =>
            {
                status_ProgressBar.Style = ProgressBarStyle.Marquee;
                status_ProgressBar.Value = percent;
                status_ProgressBar.Visible = true;
                status_InfoLabel.Text = $"Extracting NARCs... {percent}%";
            });

            status_ProgressBar.Style = ProgressBarStyle.Marquee;
            status_ProgressBar.Minimum = 0;
            status_ProgressBar.Maximum = 100;
            status_ProgressBar.Value = 0;
            status_ProgressBar.Visible = true;

            try
            {
                await romMethods.UnpackNarcsAsync(fileData, progress);

                status_ProgressBar.Value = 100;
                status_InfoLabel.Text = "NARC extraction complete!";
            }
            catch (Exception ex)
            {
                status_InfoLabel.Text = "NARC extraction failed";
                status_ProgressBar.Value = 0;
                MessageBox.Show($"Error extracting NARCs: {ex.Message}",
                              "Extraction Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
                status_ProgressBar.Visible = false;
            }
        }
        private async void OpenRom()
        {
            using var openRom = new OpenFileDialog { Filter = FileSystem.Filters.NdsRomFilter };

            if (openRom.ShowDialog(this) != DialogResult.OK || string.IsNullOrEmpty(openRom.FileName))
            {
                return;
            }

            status_InfoLabel.Text = "Opening ROM...";
            status_ProgressBar.Visible = false;
            this.Enabled = false;

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(openRom.FileName);
                var fileData = new FileData
                {
                    FileName = fileName,
                    RomFilePath = openRom.FileName,
                    WorkingDirectory = Path.Combine(Path.GetDirectoryName(openRom.FileName), fileName)
                };

                status_InfoLabel.Text = "Checking for existing data...";
                bool useExistingFolder = TryUseExistingFolder(fileData, FileSystem.Directories.DspreFolder,
                    "DSPRE Contents found",
                    "An extracted DSPRE folder exists for\nthe selected .nds file.\n\nDo you want to open this folder instead?");

                if (!useExistingFolder)
                {
                    useExistingFolder = HandleVsMakerFolder(fileData);
                }

                CloseProject();
                InitializeNewProject(fileData, useExistingFolder);

                if (ReadRomData())
                {
                    await BeginUnpackRom(fileData, useExistingFolder);
                    await BeginUnpackNarcs(fileData, mainData.RomData);
                    EnableMainMenus();
                    tabControl_Main.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                status_InfoLabel.Text = "ERROR: Unable to extract ROM Files";
                MessageBox.Show($"Error: {ex.Message}", "ROM Extraction failed",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                status_ProgressBar.Visible = false;
                this.Enabled = true;
                status_InfoLabel.Text = "Ready";
            }
        }

        private bool ReadRomData()
        {
            IsLoadingData = true;
            status_InfoLabel.Text = "Reading ROM Data...";
            status_ProgressBar.Style = ProgressBarStyle.Marquee;
            status_ProgressBar.Visible = true;
            var romData = romMethods.LoadInitialRomData(mainData.FileData);
            if (romData == null)
            {
                ShowErrorMessage("Unable to load ROM File", "Error Loading ROM");
                return EndMethod(false);
            }

            romData.GameVersion = romMethods.CheckGameVersion(romData);

            if (romData.GameVersion == GameVersion.Unknown)
            {
                status_InfoLabel.Text = "ERROR: ROM not supported";
                status_ProgressBar.Visible = false;
                ShowUnsupportedRomError();
                Console.WriteLine("ROM version is unsupported.");
                return EndMethod(false);
            }
            status_InfoLabel.Text = "Setting NARC directories...";
            status_ProgressBar.Style = ProgressBarStyle.Marquee;
            status_ProgressBar.Visible = true;
            mainData.FileData.NarcDirectories = fileMethods.SetNarcDirectories(mainData.FileData.WorkingDirectory, romData);
            UpdateUiWithRomData(romData);
            return EndMethod(true);

            bool EndMethod(bool success)
            {
                IsLoadingData = false;
                return success;
            }
        }

        private static void ShowErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void ShowUnsupportedRomError()
        {
            const string errorMessage = "The ROM file you have selected is not supported by VSMaker 2.\n\n" +
                                       "VSMaker 2 currently only supports Pokémon Diamond, Pearl, Platinum, " +
                                       "HeartGold or SoulSilver versions.";
            ShowErrorMessage(errorMessage, "Unsupported ROM");
        }

        private void UpdateUiWithRomData(RomData romData)
        {
            mainData.RomData = romData;
            ts_RomName.Text = romData.GameVersion.ToString();
            ts_RomName.Visible = true;
            ts_gameCode.Text = romData.GameCode;
            ts_gameCode.Visible = true;
            ts_gameLang.Text = romData.GameLanguage.ToString();
            ts_gameLang.Visible = true;
        }

        private static bool TryUseExistingFolder(FileData fileData, string folderSuffix, string title, string message)
        {
            if (!CheckExtractedContentsFolderExists(fileData.WorkingDirectory, folderSuffix))
            {
                return false;
            }

            var dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                fileData.WorkingDirectory += folderSuffix;
                return true;
            }

            return false;
        }

        private bool HandleVsMakerFolder(FileData fileData)
        {
            if (!CheckExtractedContentsFolderExists(fileData.WorkingDirectory, FileSystem.Directories.VsMaker2Folder))
            {
                return false;
            }

            var dialogResult = MessageBox.Show(
                "An extracted VS Maker 2 Data folder exists for\nthe selected .nds file.\n\nDo you want to open this folder instead?",
                "VS Maker 2 Contents found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                fileData.WorkingDirectory += FileSystem.Directories.VsMaker2Folder;
                return true;
            }

            return dialogResult == DialogResult.No ? HandleVsMakerDeletion(fileData) : false;
        }

        private bool HandleVsMakerDeletion(FileData fileData)
        {
            var dialogResult = MessageBox.Show(
                "Do you want to delete this folder and extract the rom again?",
                "Extract Rom Data",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogResult != DialogResult.Yes)
            {
                return false;
            }

            string directoryToDelete = fileData.WorkingDirectory + FileSystem.Directories.VsMaker2Folder;

            try
            {
                status_InfoLabel.Text = "Deleting folder...";
                status_ProgressBar.Style = ProgressBarStyle.Marquee;
                status_ProgressBar.Visible = true;
                Directory.Delete(directoryToDelete, true);

                if (Directory.Exists(directoryToDelete))
                {
                    status_InfoLabel.Text = "ERROR: Unable to delete folder";
                    status_ProgressBar.Visible = false;
                    MessageBox.Show("Unable to delete VS Maker 2 Data Folder.", "Folder not deleted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                status_InfoLabel.Text = "ERROR: Unable to delete folder";
                status_ProgressBar.Visible = false;
                MessageBox.Show($"Unable to delete VS Maker 2 Data Folder: {ex.Message}", "Folder not deleted", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return false;
        }

        private void InitializeNewProject(FileData fileData, bool useExistingFolder)
        {
            mainData = new MainData
            {
                FileData = fileData,
                RomData = new RomData()
            };

            if (!useExistingFolder)
            {
                status_InfoLabel.Text = "Creating folder...";
                status_ProgressBar.Style = ProgressBarStyle.Marquee;
                status_ProgressBar.Visible = true;
                fileData.WorkingDirectory += FileSystem.Directories.VsMaker2Folder;

                try
                {
                    Directory.CreateDirectory(fileData.WorkingDirectory);

                    if (!Directory.Exists(fileData.WorkingDirectory))
                    {
                        status_InfoLabel.Text = "ERROR: Unable to create folder";
                        status_ProgressBar.Visible = false;
                        throw new DirectoryNotFoundException("Folder not created after creation attempt");
                    }
                }
                catch (Exception ex)
                {
                    status_InfoLabel.Text = "ERROR: Unable to create folder";
                    status_ProgressBar.Visible = false;
                    MessageBox.Show($"Unable to create VS Maker 2 Data Folder: {ex.Message}", "Folder not created", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void CloseProject()
        {
        }

        private static bool CheckExtractedContentsFolderExists(string filePath, string folderName)
        {
            return Directory.Exists(filePath + folderName);
        }

        private void menu_OpenRom_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                OpenRom();
            }
        }

        private void ts_OpenRom_Click(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                OpenRom();
            }
        }

        private void ts_OpenFolder_Click(object sender, EventArgs e)
        {

        }
    }
}