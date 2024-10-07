using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Main.Misc
{
    public static class Config
    {
        private const string RecentFilesPath = "recent_files.txt"; // Path to store recent files
        private const int MaxRecentItems = 5; // Maximum number of recent items to store

        private static List<string> recentItems = [];

        public static void LoadRecentItems()
        {
            if (File.Exists(RecentFilesPath))
            {
                recentItems = File.ReadAllLines(RecentFilesPath).ToList();
            }
        }

        private static void SaveRecentItems()
        {
            File.WriteAllLines(RecentFilesPath, recentItems);
        }

        public static List<string> GetRecentItems()
        {
            return recentItems;
        }

        public static void AddToRecentItems(string path)
        {
            recentItems.Remove(path);
            recentItems.Insert(0, path);

            if (recentItems.Count > MaxRecentItems)
            {
                recentItems = recentItems.Take(MaxRecentItems).ToList();
            }

            SaveRecentItems();
        }


        public static void UpdateRecentItemsMenu(ToolStripMenuItem openRecent, Action<string> openRecentFileAction)
        {
            openRecent.DropDownItems.Clear();

            foreach (var item in recentItems)
            {
                ToolStripMenuItem menuItem = new(item);
                menuItem.Click += (s, e) => openRecentFileAction(item); 
                openRecent.DropDownItems.Add(menuItem);
            }

            openRecent.Enabled = recentItems.Count > 0;
        }
    }
}
