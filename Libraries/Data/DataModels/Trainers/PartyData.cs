using Data.DataModels.Pokemons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModels.Trainers
{
    public class PartyData
    {
        [JsonProperty("pokemon")]
        [MaxLength(6, ErrorMessage = "Party cannot exceed 6 Pokémon")]
        public PartyPokemon[] PartyPokemons { get; set; } = new PartyPokemon[6];

        public PartyData()
        {
            PartyPokemons = new PartyPokemon[6];
            for (int i = 0; i < 6; i++)
            {
                PartyPokemons[i] = new PartyPokemon();
            }
        }

        public bool ShouldSerializePartyPokemons()
        {
            return PartyPokemons?.Any(p => p != null) ?? false;
        }
    }
}
