using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class RomFile
    {
        #region ARM9

        public const uint SynthOverlayLoadAddress = 0x023C8000;
        public static string Arm9Path { get; set; }
        public uint Arm9SpwanOffset { get; set; }

        #endregion ARM9

        #region Overlay

        public string OverlayPath { get; set; }
        public string OverlayTablePath { get; set; }

        #endregion Overlay

        #region Trainer Text Table
        public string TrainerTablePath { get; set; }
        #endregion

        #region Palettes
        public uint PokemonIconPalTableAddress { get; set; }
        #endregion Palettes

        #region PrizeMoney

        public uint PrizeMoneyTableOffset { get; set; }
        public int PrizeMoneyTableOverlayNumber { get; set; }
        public int PrizeMoneyTableSize { get; set; }

        #endregion PrizeMoney

        #region ClassGenders

        public uint ClassGenderOffsetToRamAddress { get; set; }

        #endregion ClassGenders

        #region EyeContactMusic

        public uint EncounterMusicTableOffsetToRamAddress { get; set; }

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
        public  int TrainerNameMaxByteOffset => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage switch
            {
                GameLanguage.English  => 0x6AC32,
                GameLanguage.Italian => 0x6AC6E,
                GameLanguage.Spanish => 0x6AC8E,
                GameLanguage.German => 0x6AC8E,
                GameLanguage.French => 0x6AC8E,
                GameLanguage.Japanese => -1, // to add
                _ => -1
            },
            GameFamily.Platinum => GameLanguage switch
            {
                GameLanguage.English => 0x791DE,
                GameLanguage.Italian => 0x7927E,
                GameLanguage.Spanish => 0x7927E,
                GameLanguage.German => 0x7927E,
                GameLanguage.French => 0x7927E,
                GameLanguage.Japanese => 0x78AB6,
                _ => -1
            },
            GameFamily.HeartGoldSoulSilver => GameVersion switch
            {
                GameVersion.SoulSilver => GameLanguage switch {
                    GameLanguage.Japanese => 0x7342E,
                    GameLanguage.Italian => 0x72EC2,
                    GameLanguage.Spanish => 0x72EC2,
                    GameLanguage.German => 0x72EC2,
                    GameLanguage.French => 0x72EC2,
                    GameLanguage.English => 0x72EC2,
                    _ => -1
                },
                GameVersion.HeartGold => GameLanguage switch
                {
                    GameLanguage.Japanese => 0x7342E,
                    GameLanguage.Italian => 0x7342E,
                    GameLanguage.Spanish => 0x73426,
                    GameLanguage.German => 0x7342E,
                    GameLanguage.French => 0x7342E,
                    GameLanguage.English => 0x7342E,
                    _ => -1
                },

                _ => -1
            },
            GameFamily.HgEngine => GameLanguage switch
            {
                GameLanguage.Japanese => 0x7342E,
                GameLanguage.Italian => 0x7342E,
                GameLanguage.Spanish => 0x73426,
                GameLanguage.German => 0x7342E,
                GameLanguage.French => 0x7342E,
                GameLanguage.English => 0x7342E,
                _ => -1,
            }
        };
        #endregion
    }
}