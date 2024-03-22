using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class Pokemon
    {
        public uint PokemonId { get; set; }
        public uint SpeciesId { get; set; }
        public ushort Level { get; set; }
        public ushort DV { get; set; }
        public ushort GenderId { get; set; }
        public ushort FormId { get; set; }
        public ushort AbilityId { get; set; }
        public ushort BallSealId { get; set; }
        public uint? Move1Id { get; set; }
        public uint? Move2Id { get; set; }
        public uint? Move3Id { get; set; }
        public uint? Move4Id { get; set; }
        public uint? ItemId { get; set; }
    }
}