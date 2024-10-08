using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class LoadingData : Form
    {
        private LoadType loadType;
        private Mainform mainForm;
        private string FilePath;

        public LoadingData()
        {
            InitializeComponent();
        }

        public LoadingData(Mainform mainForm, LoadType loadType)
        {
            this.mainForm = mainForm;
            this.loadType = loadType;
            InitializeComponent();
            LoadData();
        }

        public LoadingData(Mainform mainForm, LoadType loadType, string filePath)
        {
            this.mainForm = mainForm;
            this.loadType = loadType;
            this.FilePath = filePath;
            InitializeComponent();
            LoadData();
        }

        public async Task LoadRomData()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => progressBar.Value = value);
            await Task.Run(() => mainForm.LoadRomData(progress));
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task SetupEditor()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => progressBar.Value = value);
            await Task.Run(() => mainForm.GetInitialData(progress));
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task UnpackNarcs()
        {
            await Task.Delay(500);  // Simulate delay if needed, remove if not required

            var progress = new Progress<int>(value => progressBar.Value = value);

            // Await the async method directly
            await mainForm.BeginUnpackNarcsAsync(progress);
            await Task.Delay(500);

            FormClosing -= LoadingData_FormClosing;  // Unsubscribe from event handler if needed
            Close();  // Close the form
        }

        public async Task UnpackRom()
        {
            await Task.Delay(500);  // Simulate delay if needed, remove if not required

            progressBar.Style = ProgressBarStyle.Continuous;

            try
            {
                // Await the async method directly
                await mainForm.BeginUnpackRomDataAsync();
                progressBar.Value = 100;
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., show an error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await Task.Delay(500);

            FormClosing -= LoadingData_FormClosing;  // Unsubscribe from event handler if needed
            Close();  // Close the form
        }

        public async Task SaveRom()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSaveRomChanges(progress, FilePath));
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task SaveTrainerTextTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSaveBattleMessages(progress));
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task RepointTrainerTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSortRepointTrainerText(progress, progressBar.Maximum));
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task ExportTrainerTextTable()
        {
            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                progressBar.Refresh();
            });

            await mainForm.BeginExportBattleMessagesAsync(progress, FilePath);

            await Task.Delay(100);

            this.Close();
        }

        public async Task ImportTrainerTextTable()
        {
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await mainForm.BeginImportBattleMessagesAsync(FilePath);
            await Task.Delay(500);
            FormClosing -= LoadingData_FormClosing;

            Close();
        }

        private async void LoadData()
        {
            progressBar.Value = 0;
            switch (loadType)
            {
                case LoadType.UnpackRom:
                    Text = "Unpacking ROM Data...";
                    await UnpackRom();
                    break;

                case LoadType.LoadRomData:
                    Text = "Loading ROM Data...";
                    await LoadRomData();
                    break;

                case LoadType.SetupEditor:
                    Text = "Setting up VS Maker 2...";
                    await SetupEditor();
                    break;

                case LoadType.UnpackNarcs:
                    Text = "Unpacking Essential NARCs...";
                    await UnpackNarcs();
                    break;

                case LoadType.SaveRom:
                    Text = "Saving ROM...";
                    await SaveRom();
                    break;

                case LoadType.SaveTrainerTextTable:
                    Text = "Saving Battle Message Table...";
                    progressBar.Maximum = mainForm.BattleMessageCount + 25;
                    await SaveTrainerTextTable();
                    break;

                case LoadType.ExportTextTable:
                    Text = "Exporting Battle Messages...";
                    progressBar.Maximum = mainForm.BattleMessageCount + 50;
                    await ExportTrainerTextTable();
                    break;

                case LoadType.ImportTextTable:
                    Text = "Importing Battle Messages...";
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.Value = 100;
                    await ImportTrainerTextTable();
                    break;

                case LoadType.RepointTextTable:
                    Text = "Sorting & Repointing Battle Message Table...";
                    progressBar.Maximum = mainForm.BattleMessageCount + 25;
                    await RepointTrainerTable();
                    break;

                default:
                    FormClosing -= LoadingData_FormClosing;
                    Close();
                    break;
            }
        }

        private void LoadingData_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        public void UpdateProgressBarStyle(ProgressBarStyle style)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action<ProgressBarStyle>(UpdateProgressBarStyle), style);
            }
            else
            {
                progressBar.Style = style;
            }
        }

        public void UpdateProgressBar(int value)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                progressBar.Value = value;
            }
        }
    }
}