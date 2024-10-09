namespace Main.Misc
{
    public static class Config
    {
        private const int MaxRecentItems = 5;
        private static readonly string ConfigFilePath = Path.Combine("Config", "config.txt");
        private static readonly string RecentFilesPath = Path.Combine("Config", "recent_files.txt");
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

        public static string GetFirstRecentItem() => recentItems.FirstOrDefault();

        public static string GetRomFolderPath()
        {
            return romFolderPath;
        }

        public static void LoadConfig()
        {
            if (File.Exists(RecentFilesPath))
            {
                recentItems = File.ReadAllLines(RecentFilesPath).Select(p => p.TrimEnd('\\')).ToList();
            }

            if (File.Exists(ConfigFilePath))
            {
                foreach (var line in File.ReadAllLines(ConfigFilePath))
                {
                    if (line.StartsWith("LoadLastOpened="))
                    {
                        loadLastOpened = bool.Parse(line.Split('=')[1]);
                    }
                    if (line.StartsWith("RomFolderPath="))
                    {
                        romFolderPath = line.Split('=')[1].TrimEnd('\\');
                    }
                    if (line.StartsWith("ShowConsoleWindow="))
                    {
                        showConsoleWindow = bool.Parse(line.Split('=')[1]);
                    }
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

        private static void EnsureConfigFolderExists()
        {
            string configDirectory = Path.GetDirectoryName(ConfigFilePath);
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }
        }
    }
}