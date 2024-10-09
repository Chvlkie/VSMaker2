using System.Runtime.InteropServices;

namespace Main.Misc
{
    public static class Config
    {
        private const int MaxRecentItems = 5;
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private static readonly string ConfigFilePath = Path.Combine("Config", "config.txt");
        private static readonly string RecentFilesPath = Path.Combine("Config", "recent_files.txt");
        private static bool consoleAllocated = false;
        private static StringWriter consoleLogWriter = new StringWriter();
        private static bool loadLastOpened;
        private static List<string> recentItems = [];
        private static string romFolderPath = "";
        private static bool showConsoleWindow;

        public static bool LoadLastOpened
        {
            get => loadLastOpened;
            set
            {
                loadLastOpened = value;
                SaveConfig();
            }
        }

        public static bool ShowConsoleWindow
        {
            get => showConsoleWindow;
            set
            {
                showConsoleWindow = value;
                SaveConfig();
            }
        }

        public static void AddToRecentItems(string path)
        {
            path = path.TrimEnd('\\');

            recentItems.Remove(path);

            recentItems.Insert(0, path);

            if (recentItems.Count > MaxRecentItems)
            {
                recentItems = recentItems.Take(MaxRecentItems).ToList();
            }

            SaveConfig();
        }

        public static void ClearConsoleLogs() => consoleLogWriter.GetStringBuilder().Clear();

        public static void ClearRecentItems(ToolStripMenuItem openRecentMenu)
        {
            recentItems.Clear();

            if (File.Exists(RecentFilesPath))
            {
                File.Delete(RecentFilesPath);
            }

            openRecentMenu.DropDownItems.Clear();
            openRecentMenu.Enabled = false;
        }

        public static void ExportConsoleLogs(string filePath)
        {
            try
            {
                File.WriteAllText(filePath, GetConsoleLogs());
                MessageBox.Show("Console logs exported successfully.", "Export Logs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting logs: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetConsoleLogs() => consoleLogWriter.ToString();

        public static string GetFirstRecentItem() => recentItems.FirstOrDefault();

        public static string GetRomFolderPath() => romFolderPath;

        public static void LoadConfig()
        {
            Console.WriteLine("Checking for recently opened files list");
            if (File.Exists(RecentFilesPath))
            {
                Console.WriteLine($"{RecentFilesPath} found. Loading recently opened files");
                recentItems = File.ReadAllLines(RecentFilesPath).Select(p => p.TrimEnd('\\')).ToList();
            }

            Console.WriteLine("Checking for config");
            if (File.Exists(ConfigFilePath))
            {
                Console.WriteLine($"{ConfigFilePath} found. Loading config");
                foreach (var line in File.ReadAllLines(ConfigFilePath))
                {
                    if (line.StartsWith("LoadLastOpened="))
                    {
                        loadLastOpened = bool.Parse(line.Split('=')[1]);
                    }
                    Console.WriteLine("Config - Load Last Opened=" + loadLastOpened.ToString());

                    if (line.StartsWith("RomFolderPath="))
                    {
                        romFolderPath = line.Split('=')[1].TrimEnd('\\');
                    }
                    Console.WriteLine("Config - Default ROM Folder path=" + romFolderPath);

                    if (line.StartsWith("ShowConsoleWindow="))
                    {
                        showConsoleWindow = bool.Parse(line.Split('=')[1]);
                    }
                    Console.WriteLine("Config - Show Console Window=" + showConsoleWindow.ToString());
                }
            }
        }

        public static void SaveConfig()
        {
            EnsureConfigFolderExists();

            var configLines = new List<string>
            {
               $"ShowConsoleWindow={showConsoleWindow}",
                $"LoadLastOpened={loadLastOpened}",
                $"RomFolderPath={romFolderPath}"
            };

            try
            {
                File.WriteAllLines(ConfigFilePath, configLines);
                File.WriteAllLines(RecentFilesPath, recentItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SaveRomFolderPath(string path)
        {
            romFolderPath = path.TrimEnd('\\');

            SaveConfig();
        }

        public static void ShowConsole(bool show)
        {
            IntPtr consoleWindow = GetConsoleWindow();

            if (consoleWindow == IntPtr.Zero && show && !consoleAllocated)
            {
                AllocConsole();
                consoleAllocated = true;
                consoleWindow = GetConsoleWindow();
                ResetConsoleOutput();
            }

            if (consoleWindow != IntPtr.Zero)
            {
                ShowWindow(consoleWindow, show ? SW_SHOW : SW_HIDE);
                ResetConsoleOutput();
            }
        }

        public static void UpdateRecentItemsMenu(ToolStripMenuItem openRecentMenu, Action<string> openRecentFileAction)
        {
            openRecentMenu.DropDownItems.Clear();

            foreach (var item in recentItems)
            {
                ToolStripMenuItem menuItem = new(item);
                menuItem.Click += (s, e) => openRecentFileAction(item);
                openRecentMenu.DropDownItems.Add(menuItem);
            }

            openRecentMenu.Enabled = recentItems.Count > 0;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        private static void EnsureConfigFolderExists()
        {
            Console.WriteLine("Checking for config");
            string configDirectory = Path.GetDirectoryName(ConfigFilePath);
            if (!Directory.Exists(configDirectory))
            {
                Console.WriteLine("Creating " + configDirectory);
                Directory.CreateDirectory(configDirectory);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();
        public static void ResetConsoleOutput()
        {
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            TextWriter multiWriter = new MultiTextWriter(standardOutput, consoleLogWriter);
            Console.SetOut(multiWriter);
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}