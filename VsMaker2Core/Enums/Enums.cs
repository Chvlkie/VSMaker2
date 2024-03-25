using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core
{
    public static class Enums
    {
        public enum LoadType
        {
            UnpackRom = 0,
            LoadRomData = 1,
            UnpackNarcs = 2,
            SetupEditor = 3,
            SaveRom = 4,
            SaveTrainerTextTable = 5,
            RepointTextTable = 6,
            ImportTextTable = 7,
            ExportTextTable = 8,
        }

        public enum GameFamily : byte
        {
            Unknown,
            DiamondPearl,
            Platinum,
            HeartGoldSoulSilver,
            HgEngine
        }

        public enum GameVersion : byte
        {
            Unknown,
            Diamond,
            Pearl,
            Platinum,
            HeartGold,
            SoulSilver,
            HgEngine
        }

        public enum GameLanguage : byte
        {
            Unknown,
            English,
            Japanese,
            Italian,
            Spanish,
            French,
            German,
            Chinese
        }

        public enum NarcDirectory : byte
        {
            Unknown,
            BattleStagePokeData,
            BattleTowerPokeData,
            BattleTowerTrainerData,
            PersonalPokeData,
            SynthOverlay,
            TextArchive,
            TrainerProperties,
            TrainerParty,
            TrainerGraphics,
            PokemonIcons,
            MoveData,
            BattleMessageTable,
            BattleMessageOffset
        }
    }
}