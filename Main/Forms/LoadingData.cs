﻿using static VsMaker2Core.Enums;

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
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task SetupEditor()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => progressBar.Value = value);
            await Task.Run(() => mainForm.GetInitialData(progress));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task UnpackNarcs()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => progressBar.Value = value);
            await Task.Run(() => mainForm.BeginUnpackNarcs(progress));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task UnpackRom()
        {
            await Task.Delay(500);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
            await Task.Run(() => mainForm.BeginUnpackRomData());
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task SaveRom()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSaveRomChanges(progress, FilePath));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task SaveTrainerTextTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSaveBattleMessages(progress));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task RepointTrainerTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginSortRepointTrainerText(progress, progressBar.Maximum));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task ExportTrainerTextTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginExportBattleMessages(progress, FilePath));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        public async Task ImportTrainerTextTable()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginImportBattleMessages(FilePath));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }

        private void LoadData()
        {
            progressBar.Value = 0;
            switch(loadType)
            {
                case LoadType.UnpackRom:
                    Text = "Unpacking ROM Data";
                    UnpackRom();
                    break;
                case LoadType.LoadRomData:
                    Text = "Loading ROM Data";
                    LoadRomData();
                    break;
                case LoadType.SetupEditor:
                    Text = "Setting up VS Maker";
                    SetupEditor();
                    break;
                case LoadType.UnpackNarcs:
                    Text = "Unpacking Essential NARCs";
                    UnpackNarcs();
                    break;
                case LoadType.SaveRom:
                    Text = "Saving ROM";
                    SaveRom();
                    break;
                case LoadType.SaveTrainerTextTable:
                    Text = "Saving Battle Message Table";
                    progressBar.Maximum = mainForm.BattleMessageCount + 25;
                    SaveTrainerTextTable();
                    break;

                case LoadType.ExportTextTable:
                    Text = "Exporting Battle Messages";
                    progressBar.Maximum = mainForm.BattleMessageCount +50;
                    ExportTrainerTextTable();
                    break;

                case LoadType.ImportTextTable:
                    Text = "Importing Battle Messages";
                    progressBar.Style = ProgressBarStyle.Marquee;
                    progressBar.Value = 100;
                    ImportTrainerTextTable();
                    break;

                case LoadType.RepointTextTable:
                    Text = "Sorting & Repointing Battle Message Table";
                    progressBar.Maximum = mainForm.BattleMessageCount + 25;
                    RepointTrainerTable();
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
                // We are not on the UI thread, so use Invoke to update the ProgressBar style
                progressBar.Invoke(new Action<ProgressBarStyle>(UpdateProgressBarStyle), style);
            }
            else
            {
                // We are on the UI thread, so update the ProgressBar style directly
                progressBar.Style = style;
            }
        }
        public void UpdateProgressBar(int value)
        {
            if (progressBar.InvokeRequired)
            {
                // We are not on the UI thread, so use Invoke to update the ProgressBar
                progressBar.Invoke(new Action<int>(UpdateProgressBar), value);
            }
            else
            {
                // We are on the UI thread, so update the ProgressBar directly
                progressBar.Value = value;
            }
        }

    }
}
