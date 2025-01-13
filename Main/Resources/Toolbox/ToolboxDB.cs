using System.Reflection;
using System.Resources;
using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;
using static VsMaker2Core.Enums;

namespace Main.Resources.Toolbox
{
    public static class ToolboxDB
    {
        internal class ARM9PatchData
        {
            internal string initString;
            internal string branchString;

            internal uint branchOffset;
            internal uint initOffset;

            public static Dictionary<string, string> arm9ExpansionCodeDB = new()
            {
                ["branchString" + "_" + GameFamily.DiamondPearl + "_" + GameLanguage.English] = "05 F1 34 FC",
                ["branchString" + "_" + GameFamily.DiamondPearl + "_" + GameLanguage.Spanish] = "05 F1 04 FD",
                ["branchString" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = "00 F1 B4 F8",
                ["branchString" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = "00 F1 B2 F9",
                ["branchString" + "_" + GameFamily.HeartGoldSoulSilver + "_" + GameLanguage.English] = "0F F1 30 FB",
                ["branchString" + "_" + GameFamily.HeartGoldSoulSilver + "_" + GameLanguage.Spanish] = "0F F1 40 FB",

                ["initString" + "_" + GameFamily.DiamondPearl] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD F1 64 00 02 00 80 3C 02",  //Valid for ENG and ESP, also for P
                ["initString" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD A5 6A 00 02 00 80 3C 02",
                ["initString" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = "FC B5 05 48 C0 46 41 21 09 22 02 4D A8 47 00 20 03 21 FC BD B9 6A 00 02 00 80 3C 02",
                ["initString" + "_" + GameFamily.HeartGoldSoulSilver] = "FC B5 05 48 C0 46 1C 21 00 22 02 4D A8 47 00 20 03 21 FC BD 09 75 00 02 00 80 3C 02" //Valid for ENG and ESP, also for SS
            };

            public static Dictionary<string, uint> arm9ExpansionOffsetsDB = new()
            {
                ["branchOffset" + "_" + GameFamily.DiamondPearl] = 0x02000C80, //Valid also for P
                ["branchOffset" + "_" + GameFamily.Platinum] = 0x02000CB4,
                ["branchOffset" + "_" + GameFamily.HeartGoldSoulSilver] = 0x02000CD0, //Valid also for SS

                ["initOffset" + "_" + GameFamily.DiamondPearl + "_" + GameLanguage.English] = 0x021064EC,
                ["initOffset" + "_" + GameFamily.DiamondPearl + "_" + GameLanguage.Spanish] = 0x0210668C,
                ["initOffset" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = 0x02100E20,
                ["initOffset" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = 0x0210101C,
                ["initOffset" + "_" + GameFamily.HeartGoldSoulSilver + "_" + GameLanguage.English] = 0x02110334,
                ["initOffset" + "_" + GameFamily.HeartGoldSoulSilver + "_" + GameLanguage.Spanish] = 0x02110354,
            };

            internal ARM9PatchData()
            {
                branchOffset = arm9ExpansionOffsetsDB[nameof(branchOffset) + "_" + RomFile.GameFamily] - Arm9.Address;
                initOffset = arm9ExpansionOffsetsDB[nameof(initOffset) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage] - Arm9.Address;
                branchString = arm9ExpansionCodeDB[nameof(branchString) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];

                if (RomFile.GameFamily == GameFamily.Platinum)
                {
                    initString = arm9ExpansionCodeDB[nameof(initString) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];
                }
                else
                {
                    initString = arm9ExpansionCodeDB[nameof(initString) + "_" + RomFile.GameFamily];
                }
            }
        }

        public static Dictionary<GameFamily, uint> SyntheticOverlayFileNumbersDB = new()
        {
            [GameFamily.DiamondPearl] = 9,
            [GameFamily.Platinum] = 9,
            [GameFamily.HeartGoldSoulSilver] = 0,
        };
    }
}