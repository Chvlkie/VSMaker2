using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.DataModels.Trainers
{
    public class TrainerPartyData
    {
        public byte TrainerType { get; set; }
        public TrainerPartyPokemonData[] PokemonData { get; set; }
    }
}