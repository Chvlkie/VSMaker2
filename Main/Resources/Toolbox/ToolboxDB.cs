using System.Reflection;
using System.Resources;
using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;
using static VsMaker2Core.Enums;

namespace Main.Resources.Toolbox
{
    public class ToolboxDB
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

        internal class BDHCAMPatchData
        {
            private const string BaseName = "Main.Resources.Toolbox.BDHCAMPatchDB";
            internal byte OverlayNumber;

            internal uint BranchOffset;
            internal string BranchString;

            internal uint OverlayOffset1;
            internal uint OverlayOffset2;

            internal string OverlayString1;
            internal string OverlayString2;

            internal byte[] Subroutine;

            public static Dictionary<string, string> BDHCamCodeDB = new()
            {
                ["branchString" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = "B9 F3 E2 F8",
                ["branchString" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = "B9 F3 AA F8",
                ["branchString" + "_" + GameFamily.HeartGoldSoulSilver] = "B6 F3 2E FA", //Also valid for SS, both ESP and ENG

                ["overlayString1"] = "00 4B 18 47 41 9C 3D 02",
                ["overlayString2"] = "00 4B 18 47 01 9C 3D 02",
            };

            public static Dictionary<string, uint> BDHCamOffsetsDB = new()
            {
                ["branchOffset" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = 0x0202040C,
                ["branchOffset" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = 0x0202047C,
                ["branchOffset" + "_" + GameFamily.HeartGoldSoulSilver] = 0x02023174, //Also valid for SS, both ESP and ENG

                ["overlayOffset1" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = 0x0001E1B4,
                ["overlayOffset1" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = 0x0001E1BC,
                ["overlayOffset1" + "_" + GameFamily.HeartGoldSoulSilver] = 0x0001574C,

                ["overlayOffset2" + "_" + GameFamily.Platinum + "_" + GameLanguage.English] = 0x0001E2CC,
                ["overlayOffset2" + "_" + GameFamily.Platinum + "_" + GameLanguage.Spanish] = 0x0001E2D4,
                ["overlayOffset2" + "_" + GameFamily.HeartGoldSoulSilver] = 0x00015864,
            };

            public static uint BDHCamSubroutineOffset = 0x000115B0;

            internal BDHCAMPatchData()
            {
                switch (RomFile.GameFamily)
                {
                    case GameFamily.Platinum:
                        OverlayNumber = 5;
                        BranchString = BDHCamCodeDB[nameof(BranchString) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];

                        BranchOffset = BDHCamOffsetsDB[nameof(BranchOffset) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];
                        OverlayOffset1 = BDHCamOffsetsDB[nameof(OverlayOffset1) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];
                        OverlayOffset2 = BDHCamOffsetsDB[nameof(OverlayOffset2) + "_" + RomFile.GameFamily + "_" + RomFile.GameLanguage];
                        break;

                    case GameFamily.HeartGoldSoulSilver:
                        OverlayNumber = 1;
                        BranchString = BDHCamCodeDB[nameof(BranchString) + "_" + RomFile.GameFamily];

                        BranchOffset = BDHCamOffsetsDB[nameof(BranchOffset) + "_" + RomFile.GameFamily];
                        OverlayOffset1 = BDHCamOffsetsDB[nameof(OverlayOffset1) + "_" + RomFile.GameFamily];
                        OverlayOffset2 = BDHCamOffsetsDB[nameof(OverlayOffset2) + "_" + RomFile.GameFamily];
                        break;
                }
                BranchOffset -= Arm9.Address;
                OverlayString1 = BDHCamCodeDB[nameof(OverlayString1)];
                OverlayString2 = BDHCamCodeDB[nameof(OverlayString2)];
                Subroutine = (byte[])new ResourceManager(BaseName, Assembly.GetExecutingAssembly()).GetObject(RomFile.GameCode + "_cam");
            }
        }

        internal class DynamicHeadersPatchData
        {
            internal uint initOffset;
            internal string initString;
            internal string REFERENCE_STRING = "19 00 C0 46";
            internal int pointerDiff;

            internal int specialCaseOffset1 = 0x3B522;
            internal int specialCaseOffset2 = 0x3B526;
            internal int specialCaseOffset3 = 0x3B53C;

            internal string specialCaseData1 = "19 00";
            internal string specialCaseData2 = "C0 46";
            internal string specialCaseData3 = "00 00 00 00";

            public static uint GetDynamicHeadersInitOffset(string romID) => romID switch
            {
                "CPUE" => 0x3A024,
                "CPUS" or "CPUI" or "CPUF" or "CPUD" => 0x3A0C8,
                "CPUJ" => 0x39BE0,
                "IPKS" => 0x3B260,
                "IPKJ" or "IPGJ" => 0x3AE20,
                _ => 0x3B268,
            };

            public static string GetDynamicHeadersInitString(string romID) => romID switch
            {
                "CPUE" => "00 B5 01 1C 94 20 00 22 CC F7 48 FD 03 1C DE F7 C7 F8 00 BD",
                "CPUS" or "CPUI" or "CPUF" or "CPUD" => "00 B5 01 1C 94 20 00 22 CC F7 00 FD 03 1C CC F7 74 FC 00 BD",
                "CPUJ" => "00 B5 01 1C 94 20 00 22 CC F7 0A FF 03 1C DE F7 3D F9 00 BD",
                "IPKS" => "00 B5 01 1C 32 20 00 22 CC F7 5C F9 03 1C DF F7 4D FC 00 BD",
                "IPKJ" or "IPGJ" => "00 B5 01 1C 32 20 00 22 CC F7 08 FB 03 1C DF F7 C7 FC 00 BD",
                _ => "00 B5 01 1C 32 20 00 22 CC F7 58 F9 03 1C DF F7 49 FC 00 BD",
            };

            public static Dictionary<GameFamily, Tuple<uint, uint>[]> dynamicHeadersPointersDB = new()
            {
                // format: headerID*18 offset, (ARM9_HEADER_TABLE_OFFSET + n) offset
                [GameFamily.Platinum] = [
                    new Tuple<uint, uint>(0x3A03E, 0x3A048),
                    new Tuple<uint, uint>(0x3A052, 0x3A05C),
                    new Tuple<uint, uint>(0x3A066, 0x3A080),
                    new Tuple<uint, uint>(0x3A08E, 0x3A098),
                    new Tuple<uint, uint>(0x3A0A2, 0x3A0AC),
                    new Tuple<uint, uint>(0x3A0B6, 0x3A0C0),
                    new Tuple<uint, uint>(0x3A0CA, 0x3A0D4),
                    new Tuple<uint, uint>(0x3A0DE, 0x3A0E8),
                    new Tuple<uint, uint>(0x3A0F2, 0x3A108),
                    new Tuple<uint, uint>(0x3A116, 0x3A120),
                    new Tuple<uint, uint>(0x3A12A, 0x3A134),
                    new Tuple<uint, uint>(0x3A13E, 0x3A150),
                    new Tuple<uint, uint>(0x3A15A, 0x3A170),
                    new Tuple<uint, uint>(0x3A17A, 0x3A184),
                    new Tuple<uint, uint>(0x3A18E, 0x3A198),
                    new Tuple<uint, uint>(0x3A1A2, 0x3A1B4),
                    new Tuple<uint, uint>(0x3A1BE, 0x3A1D0),
                    new Tuple<uint, uint>(0x3A1DA, 0x3A1EC),
                    new Tuple<uint, uint>(0x3A1F6, 0x3A208),
                    new Tuple<uint, uint>(0x3A212, 0x3A224),
                ],
                [GameFamily.HeartGoldSoulSilver] = [
                    new(0x3B282, 0x3B28C),
                    new(0x3B296, 0x3B2A8),
                    new(0x3B2B2, 0x3B2BC),
                    new(0x3B2C6, 0x3B2D0),
                    new(0x3B2DA, 0x3B2E4),
                    new(0x3B2EE, 0x3B2F8),
                    new(0x3B302, 0x3B30C),
                    new(0x3B316, 0x3B320),
                    new(0x3B32A, 0x3B340),
                    new(0x3B34A, 0x3B354),
                    new(0x3B35E, 0x3B368),
                    new(0x3B372, 0x3B384),
                    new(0x3B38E, 0x3B3A4),
                    new(0x3B3AE, 0x3B3C4),
                    new(0x3B3CE, 0x3B3E0),
                    new(0x3B3EA, 0x3B3FC),
                    new(0x3B406, 0x3B418),
                    new(0x3B422, 0x3B434),
                    new(0x3B43E, 0x3B450),
                    new(0x3B45A, 0x3B46C),
                    new(0x3B476, 0x3B488),
                    new(0x3B492, 0x3B4A4),
                    new(0x3B4AE, 0x3B4C0),
                    new(0x3B4CA, 0x3B4D8),
                    new(0x3B4E2, 0x3B4F4),
                    new(0x3B4FE, 0x3B514),
                ],
            };

            internal DynamicHeadersPatchData()
            {
                initOffset = GetDynamicHeadersInitOffset(RomFile.GameCode);
                initString = GetDynamicHeadersInitString(RomFile.GameCode);

                if (RomFile.GameFamily == GameFamily.HeartGoldSoulSilver)
                {
                    pointerDiff = (int)(initOffset - GetDynamicHeadersInitOffset("IPKE"));
                }
                else
                {
                    pointerDiff = (int)(initOffset - GetDynamicHeadersInitOffset("CPUE"));
                }
            }
        }

        public static Dictionary<GameFamily, uint> SyntheticOverlayFileNumbersDB = new()
        {
            [GameFamily.DiamondPearl] = 9,
            [GameFamily.Platinum] = 9,
            [GameFamily.HeartGoldSoulSilver] = 0,
        };

        public static Dictionary<uint[], string> matrixExpansionDB = new()
        {
            [[0x0203AEBE]] = "FF 01",
            [[0x0203AEC0]] = "76 01",
            [[0x0203AF58]] = "C9 01",
            [[0x0203AF72]] = "49 01",
            [[0x0203AF8C]] = "3E 06 00 00",
            [[0x0203AF90]] = "3C 1F 00 00",
            [[0x0203AFA8]] = "50 1F 00 00",
            [[  0x0203AFF8,
                0x0203B108,
                0x0203B1F0,
                0x0203B25C ]] = "C4 12 00 00",
            [[0x0203B088]] = "84 0C 00 00",
            [[0x0203B0BC]] = "7C 0C 00 00",
        };
    }
}