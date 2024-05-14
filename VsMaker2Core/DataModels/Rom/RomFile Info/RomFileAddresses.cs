﻿using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class RomFile
    {
        #region ARM9

        public const uint SynthOverlayLoadAddress = 0x023C8000;
        public static string Arm9Path => WorkingDirectory + Common.Arm9FilePath;
        public uint Arm9SpawnOffset { get; set; }

        #endregion ARM9

        #region Overlay

        public static string OverlayPath => WorkingDirectory + Common.OverlayFilePath;
        public static string OverlayTablePath => WorkingDirectory + Common.Y9FilePath;

        #endregion Overlay

        #region Trainer Text Table

        public static string TrainerTablePath => WorkingDirectory + "\\unpacked\\TrainerTextTable\\0000";

        #endregion Trainer Text Table

        #region Palettes

        public uint PokemonIconPalTableAddress { get; set; }

        #endregion Palettes

        #region PrizeMoney

        public uint PrizeMoneyTableOffset => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? (uint)0x32960 : 0x32960,
            GameFamily.Platinum => 0x359E0,
            GameFamily.HeartGoldSoulSilver => 0x34C04,
            GameFamily.HgEngine => 0x34C04,
            _ => 0
        };
        public int PrizeMoneyTableOverlayNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 11,
            GameFamily.Platinum => 16,
            GameFamily.HeartGoldSoulSilver => 12,
            GameFamily.HgEngine => 12,
            _ => 0
        };
        public int PrizeMoneyTableSize => GameFamily switch
        {
            GameFamily.DiamondPearl => 98,
            GameFamily.Platinum => 105,
            GameFamily.HeartGoldSoulSilver => 516,
            GameFamily.HgEngine => 516,
            _ => 0
        };

        #endregion PrizeMoney

        #region ClassGenders

        public uint ClassGenderOffsetToRamAddress => GameFamily switch

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
            GameFamily.HgEngine =>
            GameLanguage switch
            {
                GameLanguage.English => 0x073600,
                GameLanguage.French => 0x073600,
                GameLanguage.German => 0x073600,
                GameLanguage.Italian => 0x073600,
                GameLanguage.Japanese => 0x073098,
                GameLanguage.Spanish => 0x0735f8,
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

        public uint EyeContactMusicTableOffsetToRam => GameFamily switch
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

            GameFamily.HgEngine => GameLanguage switch
            {
                GameLanguage.English => 0x550E0,
                GameLanguage.French => 0x550E0,
                GameLanguage.German => 0x550E0,
                GameLanguage.Italian => 0x550E0,
                GameLanguage.Japanese => 0x54B44,
                GameLanguage.Spanish => 0x550D8,
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
                GameLanguage.Japanese => 0x4D9AC,
                GameLanguage.Spanish => 0x4ADAC,
                _ => 0,
            },
            _ => 0,
        };

        #endregion EyeContactMusic

        #region EffectsTable

        public uint EffectsComboTableOffsetToRamAddress { get; set; }
        public uint EffectsComboTableOffsetToSizeLimiter { get; set; }

        public uint VsPokemonEntryTableOffsetToRamAddress { get; set; }
        public uint VsPokemonEntryTableOffsetToSizeLimiter { get; set; }

        public uint VsTrainerEntryTableOffsetToRamAddress { get; set; }
        public uint VsTrainerEntryTableOffsetToSizeLimiter { get; set; }

        #endregion EffectsTable

        #region TrainerNames

        public int TrainerNameMaxByteOffset => GameFamily switch
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
                GameVersion.HeartGold => GameLanguage switch
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
            GameFamily.HgEngine => GameLanguage switch
            {
                GameLanguage.English => 0x7342E,
                GameLanguage.French => 0x7342E,
                GameLanguage.German => 0x7342E,
                GameLanguage.Italian => 0x7342E,
                GameLanguage.Japanese => 0x7342E,
                GameLanguage.Spanish => 0x73426,
                _ => -1,
            }
        };

        #endregion TrainerNames
    }
}