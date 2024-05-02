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
    }
}