using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class LoadingData : Form
    {
        private LoadType loadType;
        private Mainform mainForm;
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
            await Task.Run(() => mainForm.BeginUnpackRomData());
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Value = 100;
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
            }
        }
        private void LoadingData_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
