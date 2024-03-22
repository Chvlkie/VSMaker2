using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core
{
    public static class Enums
    {
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

        public enum DirectoryNames : byte
        {
            Unknown,
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
