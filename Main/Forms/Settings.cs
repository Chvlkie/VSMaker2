using Main.Misc;
using System.Runtime.InteropServices;

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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private StringWriter consoleOutput = new StringWriter(); // Captures the console output

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

        private void btn_ClearRecent_Click(object sender, EventArgs e)
        {
            ClearRecentItemsRequested?.Invoke(this, EventArgs.Empty);
        }

        private void btn_ExportLogs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveDialog.Title = "Export Console Logs";
                saveDialog.FileName = "ConsoleLog.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveDialog.FileName, consoleOutput.ToString());

                    MessageBox.Show("Logs exported successfully.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void checkBox_OpenLast_CheckedChanged(object sender, EventArgs e)
        {
            Config.LoadLastOpened = checkBox_OpenLast.Checked;
            checkBox_OpenLast.Text = checkBox_OpenLast.Checked ? "Yes" : "No";
        }

        private void checkBox_ShowConsole_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ShowConsole.Checked)
            {
                AllocConsole(); // Attach the console window
                IntPtr consoleWindow = GetConsoleWindow();
                ShowWindow(consoleWindow, SW_SHOW); // Show the console
                checkBox_ShowConsole.Text = "Shown";
            }
            else
            {
                FreeConsole(); // Hide the console window
                checkBox_ShowConsole.Text = "Hidden";
            }

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
    }
}