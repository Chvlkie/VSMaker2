using Data.DataModels.HgEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Global.Enums;

namespace Data.DataModels.Pokemons
{
    public class PartyPokemon
    {
        public AbilitySlots AbilitySlot { get; set; }
        public ushort? BallSealId { get; set; }
        public byte DV { get; set; }
        public ushort FormId { get; set; }
        public ushort? HeldItemId { get; set; }
        public HgEngine? HgEngineData { get; set; }
        public byte Level { get; set; }
        public ushort[]? Moves { get; set; }
        public PokemonGenders PokemonGender { get; set; }
        public ushort PokemonId { get; set; }
        public ushort SpeciesId { get; set; }
        public class HgEngine

        {
            public ushort? AbilityOverrideId { get; set; }
            public AdditionalFlags AdditionalFlags { get; set; }
            public EVs EVs { get; set; }
            public bool IsShiny { get; set; } = false;
            public IVs IVs { get; set; }
            public ushort? NatureId { get; set; }
            public string Nickname { get; set; }
            public ushort? PokeBallId { get; set; }
            public Stats Stats { get; set; }
            public Status StatusEffect { get; set; }
            public byte[]? Types { get; set; }
        }
    }
}