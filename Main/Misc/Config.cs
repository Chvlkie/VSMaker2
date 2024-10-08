namespace Main.Misc
{
    public static class Config
    {
        // Define file paths
        private static readonly string ConfigFilePath = Path.Combine("Config", "config.txt");
        private static readonly string RecentFilesPath = Path.Combine("Config", "recent_files.txt");
        private const int MaxRecentItems = 5; // Maximum number of recent items to store

        // Configuration fields
        private static string romFolderPath = "";
        private static bool loadLastOpened;

        // List of recent items
        private static List<string> recentItems = new List<string>();

        // Property for the 'LoadLastOpened' setting
        public static bool LoadLastOpened
        {
            get => loadLastOpened;
            set
            {
                loadLastOpened = value;
                SaveConfig(); // Save when the setting changes
            }
        }

        // Get the first recent item (used as the last opened file/folder)
        public static string GetFirstRecentItem()
        {
            return recentItems.FirstOrDefault();
        }

        // Save the ROM folder path
        public static void SaveRomFolderPath(string path)
        {
            // Remove trailing backslash if it exists
            romFolderPath = path.TrimEnd('\\');

            // Save the updated config
            SaveConfig();
        }

        // Get the ROM folder path
        public static string GetRomFolderPath()
        {
            return romFolderPath;
        }

        // Load the configuration settings from files
        public static void LoadConfig()
        {
            if (File.Exists(RecentFilesPath))
            {
                // Trim trailing backslashes from recent items
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
                        romFolderPath = line.Split('=')[1].TrimEnd('\\'); // Trim trailing backslash
                    }
                }
            }
        }


        // Save the configuration settings to the file
        public static void SaveConfig()
        {
            EnsureConfigFolderExists(); // Ensure the config folder exists

            var configLines = new List<string>
            {
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

        // Clear the recent items and update the recent menu
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

        // Add a path to recent items, moving it to the top if it already exists
        public static void AddToRecentItems(string path)
        {
            // Remove trailing backslash if it exists
            path = path.TrimEnd('\\');

            // Remove any existing instances of the path
            recentItems.Remove(path);

            // Insert the path at the top of the list
            recentItems.Insert(0, path);

            // Ensure the list doesn't exceed the maximum number of recent items
            if (recentItems.Count > MaxRecentItems)
            {
                recentItems = recentItems.Take(MaxRecentItems).ToList();
            }

            // Save the updated config
            SaveConfig();
        }


        // Update the "Open Recent" menu
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

        // Ensure the Config folder exists before reading/writing files
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