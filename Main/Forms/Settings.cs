using Main.Misc;

namespace Main.Forms
{
    public partial class Settings : Form
    {
        public event EventHandler ClearRecentItemsRequested;

        public Settings()
        {
            InitializeComponent();
            checkBox_OpenLast.Text = checkBox_OpenLast.Checked ? "Yes" : "No";

        }

        private void btn_ClearRecent_Click(object sender, EventArgs e)
        {
            ClearRecentItemsRequested?.Invoke(this, EventArgs.Empty);
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

        private void btn_Apply_Click(object sender, EventArgs e)
        {
            // Save the 'Load Last Opened ROM' setting
            Config.LoadLastOpened = checkBox_OpenLast.Checked;

            // Save the ROM folder path if it's set
            if (!string.IsNullOrWhiteSpace(tb_RomFolder.Text))
            {
                Config.SaveRomFolderPath(tb_RomFolder.Text);
            }

            // Inform the user that the settings have been applied
            MessageBox.Show("Settings applied successfully!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBox_OpenLast_CheckedChanged(object sender, EventArgs e)
        {
            Config.LoadLastOpened = checkBox_OpenLast.Checked;
            checkBox_OpenLast.Text = checkBox_OpenLast.Checked ? "Yes" : "No";
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            checkBox_OpenLast.Checked = Config.LoadLastOpened;
            tb_RomFolder.Text = Config.GetRomFolderPath();
            toolTip_AutoLoad.SetToolTip(lbl_AutoLoadHelp, "When enabled, VS Maker 2 will automatically\nload the most recently opened project.");
            toolTip_DefaultRomFolder.SetToolTip(lbl_DefaultRomFolder, "The default browser location when\nselecting a rom/project folder.");
        }
    }
}