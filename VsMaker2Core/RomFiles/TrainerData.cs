using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.RomFiles
{
    public class TrainerData
    {
        /// <summary>
        /// Standard = 0x00,
        /// Pokemon Has Items = 0x01,
        /// Pokemon Choose Moves = 0x02,
        /// Pokemon Has Items and Choose Moves = 0x03.
        /// </summary>
        public byte TrainerType { get; set; }
        public byte TrainerClassId { get; set; }
        public byte Padding { get; set; }
        public byte TeamSize { get; set; }
        public ushort[] Items { get; set; }
        public uint AIFlags { get; set; }
        public uint IsDoubleBattle { get; set; }

        public TrainerData()
        {
            Padding = 0;
        }
    }
}