using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core
{
    public static class Common
    {
        public const string VsMakerContentsFolder = "_VSMaker2_Data";
        public const string NdsToolsFilePath = @"Tools\ndstool.exe";
        public const string BlzFilePath = @"Tools\blz.exe";
        public const string ConfigFilePath = "Config";
        public const string RecentFilesPath = "\\recent.txt";
        public const int MaxRecentFiles = 5;

        public const string NdsRomFilter = "NDS Rom (*.nds)|*.nds";

      
        public const string Arm9FilePath = "arm9.bin";
        public const string Arm7FilePath = "arm7.bin";
        public const string Y9FilePath = "y9.bin";
        public const string Y7FilePath = "y7.bin";
        public const string DataFilePath = "data";
        public const string OverlayFilePath = "overlay";
        public const string BannerFilePath = "banner.bin";
        public const string HeaderFilePath = "header.bin";

        public const int DiamondPearlTotalTrainers = 850;
        public const int PlatinumTotalTrainers = 928;
        public const int HeartGoldSoulSilverTotalTrainers = 738;
    }
}
