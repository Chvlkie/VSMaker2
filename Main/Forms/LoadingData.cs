using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class LoadingData : Form
    {
        private Mainform mainForm;
        private LoadType loadType;
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

        private void LoadData()
        {
            progressBar.Value = 0;
            switch(loadType)
            {
                case LoadType.UnpackRom:
                    Text = "Unpacking ROM Data";
                    UnpackRom();
                    break;

                case LoadType.UnpackNarcs:
                    Text = "Unpacking Essential NARCs";
                    progressBar.Maximum = 100;
                    UnpackNarcs();
                    break;
            }
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

        public async Task UnpackNarcs()
        {
            await Task.Delay(500);
            var progress = new Progress<int>(value => { progressBar.Value = value; });
            await Task.Run(() => mainForm.BeginUnpackNarcs(progress));
            FormClosing -= LoadingData_FormClosing;
            Close();
        }
        private void LoadingData_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
