using Main.Misc;

namespace Main.Forms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            checkBox_OpenLast.Text = checkBox_OpenLast.Checked ? "Yes" : "No";
            checkBox_ShowConsole.Text = checkBox_ShowConsole.Checked ? "Shown" : "Hidden";
            Console.SetOut(consoleOutput);
        }

        public event EventHandler ClearRecentItemsRequested;

        private StringWriter consoleOutput = new();

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            Config.LoadLastOpened = checkBox_OpenLast.Checked;
            Config.ShowConsoleWindow = checkBox_ShowConsole.Checked;

            if (!string.IsNullOrWhiteSpace(tb_RomFolder.Text))
            {
                Config.SaveRomFolderPath(tb_RomFolder.Text);
            }

            MessageBox.Show("Settings applied successfully!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_BrowseRomFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select ROM Folder";
            folderBrowser.ShowNewFolderButton = false;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                tb_RomFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void btn_ClearRecent_Click(object sender, EventArgs e) => ClearRecentItemsRequested?.Invoke(this, EventArgs.Empty);

        private void btn_ExportLogs_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.Title = "Export Console Logs";
            saveDialog.FileName = "ConsoleLog.txt";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                Config.ExportConsoleLogs(saveDialog.FileName);
            }
        }

        private void checkBox_OpenLast_CheckedChanged(object sender, EventArgs e)
        {
            Config.LoadLastOpened = checkBox_OpenLast.Checked;
            checkBox_OpenLast.Text = checkBox_OpenLast.Checked ? "Yes" : "No";
        }

        private void checkBox_ShowConsole_CheckedChanged(object sender, EventArgs e)
        {
            Config.ShowConsole(checkBox_ShowConsole.Checked);
            checkBox_ShowConsole.Text = checkBox_ShowConsole.Checked ? "Shown" : "Hidden";
            Config.ShowConsoleWindow = checkBox_ShowConsole.Checked;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            checkBox_OpenLast.Checked = Config.LoadLastOpened;
            checkBox_ShowConsole.Checked = Config.ShowConsoleWindow;
            tb_RomFolder.Text = Config.GetRomFolderPath();
            toolTip_AutoLoad.SetToolTip(lbl_AutoLoadHelp, "When enabled, VS Maker 2 will automatically\nload the most recently opened project.");
            toolTip_DefaultRomFolder.SetToolTip(lbl_DefaultRomFolder, "The default browser location when\nselecting a rom/project folder.");
        }

        private void btn_Close_Click(object sender, EventArgs e) => this.Close();
    }
}