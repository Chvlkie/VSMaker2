using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.RomFiles
{
    public class TrainerPartyData
    {
        public TrainerPartyPokemonData[] PokemonData { get; set; }

        public TrainerPartyData()
        {
            PokemonData = [];
        }

        public TrainerPartyData(TrainerPartyPokemonData[] pokemonData)
        {
            PokemonData = pokemonData;
        }
    }
}