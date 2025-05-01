using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Global.Constants
{
    public static class FileSystem
    {
        public static class Filters
        {
            public const string NdsRomFilter = "NDS Rom (*.nds)|*.nds";
        }

        public static class Directories
        {
            public const string DspreFolder = "_DSPRE_contents";
            public const string VsMaker2Folder = "_VSMaker2_Data";
        }

        public static class Paths
        {
            
            public const string NdsToolsFilePath = @"Tools\ndstool.exe";
            public const string BlzFilePath = @"Tools\blz.exe";
            public const string Arm9FilePath = "arm9.bin";
            public const string Arm7FilePath = "arm7.bin";
            public const string Y9FilePath = "y9.bin";
            public const string Y7FilePath = "y7.bin";
            public const string DataFilePath = "data";
            public const string OverlayFilePath = "overlay";
            public const string BannerFilePath = "banner.bin";
            public const string HeaderFilePath = "header.bin";
        }
    }
}