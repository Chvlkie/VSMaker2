using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class Enums
    {
        public enum GameFamily : byte
        {
            None,
            DiamondPearl,
            Platinum,
            HeartGoldSoulSilver,
            HgEngine
        }

        public enum GameLanguage : byte
        {
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
