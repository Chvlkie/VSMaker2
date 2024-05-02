using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.RomFiles
{
    public class TrainerPartyPokemonData
    {
        public byte Difficulty { get; set; }
        public byte GenderAbilityOverride { get; set; }
        public ushort Level { get; set; }
        public ushort Species { get; set; }
        public ushort BallCapsule { get; set; }

        public ushort ItemId { get; set; }
        public ushort[] MoveIds { get; set; }
    }
}