using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    [Serializable]
    public partial class Pokemon
    {
        public byte DifficultyValue { get; set; }
        public GenderAbilityFlags GenderAbilityFlags { get; set; }
        public ushort Level { get; set; }
        public ushort SpeciesId => Species.GetSpecialSpecies(PokemonId, FormId);
        public int IconId => GetSpecialIcon(PokemonId, FormId);
        public ushort PokemonId { get; set; }
        public ushort FormId { get;set; }
        public ushort? HeldItemId { get; set; }
        public ushort[] Moves { get; set; }
        public ushort? BallSealId { get; set; }
    }
}