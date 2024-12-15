using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public static partial class RomFile
    {
        #region ARM9

        public static string Arm9Path => $"{WorkingDirectory}{Common.Arm9FilePath}";
        public static uint Arm9SpawnOffset { get; set; }

        #endregion ARM9

        #region Overlay

        public static string OverlayPath => $"{WorkingDirectory}{Common.OverlayFilePath}";
        public static string OverlayTablePath => $"{WorkingDirectory}{Common.Y9FilePath}";
        public static string SynthOverlayFilePath => Path.Combine(Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.synthOverlay].unpackedDirectory, "0000");
        public const uint SynthOverlayLoadAddress = 0x023C8000;

        public const int HgEngineOverlay = 129;
        public const uint HgEngineOverlayLoadAddress = 0x023D8000;
        #endregion Overlay

        #region Battle Message Table

        public static string BattleMessageOffsetPath => Path.Combine(Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerTextOffset].unpackedDirectory, "0000");
        public static string BattleMessageTablePath => Path.Combine(Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerTextTable].unpackedDirectory, "0000");

        #endregion Battle Message Table

        #region PrizeMoney

        public const uint PrizeMoneyRepointOffset = 0x1100;

        public static uint PrizeMoneyTableSizeOffset => GameFamily switch
        {
            GameFamily.HeartGoldSoulSilver => 0x08310,
            _ => 0
        };

        public static uint PrizeMoneyPointerOffset => GameFamily switch
        {
            GameFamily.HeartGoldSoulSilver => 0x08380,
            _ => 0
        };
        public static uint PrizeMoneyTableOffset => PrizeMoneyExpanded ? PrizeMoneyRepointOffset : GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? (uint)0x32960 : 0x32960,
            GameFamily.Platinum => 0x359E0,
            GameFamily.HeartGoldSoulSilver => 0x34C04,
            _ => 0
        };

        public static int PrizeMoneyTableOverlayNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 11,
            GameFamily.Platinum => 16,
            GameFamily.HeartGoldSoulSilver => 12,
            _ => 0
        };

        public static int PrizeMoneyTableSize => GameFamily switch
        {
            GameFamily.DiamondPearl => PrizeMoneyExpanded ? 150 : 98,
            GameFamily.Platinum => PrizeMoneyExpanded ? 150 : 105,
            GameFamily.HeartGoldSoulSilver => PrizeMoneyExpanded ? 600 : 516,
            _ => 0
        };

        #endregion PrizeMoney

        #region ClassGenders

        public const uint ClassGenderRepointOffset = 0x1400;

        public static uint ClassGenderOffsetToRamAddress => GameFamily switch

        {
            GameFamily.HeartGoldSoulSilver =>
            GameLanguage switch
            {
                GameLanguage.English => 0x073600,
                GameLanguage.French => 0x073600,
                GameLanguage.German => 0x073600,
                GameLanguage.Italian => 0x073600,
                GameLanguage.Japanese => 0x073098,
                GameLanguage.Spanish => GameVersion == GameVersion.HeartGold ? (uint)0x0735f8 : 0x073600,
                _ => 0
            },

            GameFamily.Platinum =>
            GameLanguage switch
            {
                GameLanguage.English => 0x793b4,
                GameLanguage.French => 0x079454,
                GameLanguage.German => 0x079454,
                GameLanguage.Italian => 0x079454,
                GameLanguage.Japanese => 0x078c8c,
                GameLanguage.Spanish => 0x079454,
                _ => 0
            },
            _ => 0
        };

        #endregion ClassGenders

        #region EyeContactMusic

        public const uint EyeContactRepointOffset = 0x1600;

        public static uint EyeContactTableSizeOffset => GameFamily switch
        {

            GameFamily.HeartGoldSoulSilver => GameLanguage switch
            {
                GameLanguage.English => 0x550D4,
                GameLanguage.French => 0x550D4,
                GameLanguage.German => 0x550D4,
                GameLanguage.Italian => 0x550D4,
                GameLanguage.Japanese => 0x54B50,
                GameLanguage.Spanish => GameVersion == GameVersion.HeartGold ? (uint)0x550D8 : 0x550D4,
                _ => RomFile.IsHgEngine ? (uint)0x550D4 : 0x0,
            },

            _ => 0,
        };
        public static uint EyeContactTablePointerOffset => GameFamily switch
        {
            GameFamily.HeartGoldSoulSilver => GameLanguage switch
            {
                GameLanguage.English => 0x550E0,
                GameLanguage.French => 0x550E0,
                GameLanguage.German => 0x550E0,
                GameLanguage.Italian => 0x550E0,
                GameLanguage.Japanese => 0x54B44,
                GameLanguage.Spanish => GameVersion == GameVersion.HeartGold ? (uint)0x550D8 : 0x550E0,
                _ => 0,
            },

            GameFamily.Platinum => GameLanguage switch
            {
                GameLanguage.English => 0x5563C,
                GameLanguage.French => 0x556E0,
                GameLanguage.German => 0x556E0,
                GameLanguage.Italian => 0x556E0,
                GameLanguage.Japanese => 0x54F04,
                GameLanguage.Spanish => 0x556E0,
                _ => 0,
            },

            GameFamily.DiamondPearl => GameLanguage switch
            {
                GameLanguage.English => 0x4AD3C,
                GameLanguage.French => 0x4ADAC,
                GameLanguage.German => 0x4ADAC,
                GameLanguage.Italian => 0x4ADAC,
                GameLanguage.Japanese => 0x4ADAC,
                GameLanguage.Spanish => 0x4ADAC,
                _ => 0,
            },
            _ => 0,
        };

        #endregion EyeContactMusic

        #region EffectsTable

        public static uint EffectsComboTableOffsetToRamAddress { get; set; }
        public static uint EffectsComboTableOffsetToSizeLimiter { get; set; }

        public static uint VsPokemonEntryTableOffsetToRamAddress { get; set; }
        public static uint VsPokemonEntryTableOffsetToSizeLimiter { get; set; }

        public static uint VsTrainerEntryTableOffsetToRamAddress { get; set; }
        public static uint VsTrainerEntryTableOffsetToSizeLimiter { get; set; }

        #endregion EffectsTable

        #region TrainerNames

        public static int TrainerNameMaxByteOffset => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage switch
            {
                GameLanguage.English => 0x6AC32,
                GameLanguage.French => 0x6AC8E,
                GameLanguage.German => 0x6AC8E,
                GameLanguage.Italian => 0x6AC6E,
                GameLanguage.Japanese => -1, // to add
                GameLanguage.Spanish => 0x6AC8E,
                _ => -1
            },
            GameFamily.Platinum => GameLanguage switch
            {
                GameLanguage.English => 0x791DE,
                GameLanguage.French => 0x7927E,
                GameLanguage.German => 0x7927E,
                GameLanguage.Italian => 0x7927E,
                GameLanguage.Japanese => 0x78AB6,
                GameLanguage.Spanish => 0x7927E,
                _ => -1
            },
            GameFamily.HeartGoldSoulSilver => GameVersion switch
            {
                GameVersion.SoulSilver => GameLanguage switch
                {
                    GameLanguage.English => 0x72EC2,
                    GameLanguage.French => 0x72EC2,
                    GameLanguage.German => 0x72EC2,
                    GameLanguage.Italian => 0x72EC2,
                    GameLanguage.Japanese => 0x7342E,
                    GameLanguage.Spanish => 0x72EC2,
                    _ => -1
                },
                GameVersion.HeartGold or GameVersion.HgEngine => GameLanguage switch
                {
                    GameLanguage.English => 0x7342E,
                    GameLanguage.French => 0x7342E,
                    GameLanguage.German => 0x7342E,
                    GameLanguage.Italian => 0x7342E,
                    GameLanguage.Japanese => 0x7342E,
                    GameLanguage.Spanish => 0x73426,
                    _ => -1
                },

                _ => -1
            },

            _ => throw new NotImplementedException()
        };

        #endregion TrainerNames

        #region TrainerEncounter

        public static int OriginalTrainerEncounterScript => GameFamily switch
        {
            GameFamily.DiamondPearl => 851,
            GameFamily.Platinum => 929,
            GameFamily.HeartGoldSoulSilver => 740,
            _ => 0
        };

        public static uint TrainerEncounterScriptOffset => GameFamily switch
        {
            GameFamily.DiamondPearl =>
            GameLanguage switch
            {
                GameLanguage.English => 0x5C6B8,
                GameLanguage.French => 0x5C728,
                GameLanguage.Spanish => 0x5C728,
                GameLanguage.Italian => 0x5C728,
                GameLanguage.German => 0x5C728,
                GameLanguage.Japanese => 0x609CC,
                _ => 0
            },
            GameFamily.Platinum => GameLanguage switch
            {
                GameLanguage.English => 0x67BA4,
                GameLanguage.French => 0x67C48,
                GameLanguage.Spanish => 0x67C48,
                GameLanguage.Italian => 0x67C48,
                GameLanguage.German => 0x67C48,
                GameLanguage.Japanese => 0x6746C,
                _ => 0
            },
            GameFamily.HeartGoldSoulSilver => GameLanguage switch
            {
                GameLanguage.English => 0x641E8,
                GameLanguage.French => 0x641E8,
                GameLanguage.Spanish => GameVersion == GameVersion.SoulSilver ? (uint)0x641E8 : 0x641E0,
                GameLanguage.Italian => 0x641E8,
                GameLanguage.German => 0x641E8,
                GameLanguage.Japanese => 0x63C44,
                _ => 0
            },
            _ => 0
        };

        public static int TrainerScriptFile => GameFamily switch
        {
            GameFamily.DiamondPearl => 1040,
            GameFamily.Platinum => 1114,
            GameFamily.HeartGoldSoulSilver => 953,
            _ => 0
        };

        #endregion TrainerEncounter

        public static int InitialMoneyOverlayNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 52,
            GameFamily.Platinum => 57,
            GameFamily.HeartGoldSoulSilver => 36,
            _ => -1
        };

        public static string TrainerGraphicsPath => Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerGraphics].unpackedDirectory;
    }
}