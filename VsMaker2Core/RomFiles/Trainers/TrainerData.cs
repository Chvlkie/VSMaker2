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
            TrainerType = 0;
            TeamSize = 0;
            Items = [0, 0, 0, 0];
            AIFlags = 0;
            IsDoubleBattle = 0;
            TrainerClassId = 2;
        }

        public TrainerData(byte trainerType, byte trainerClassId, byte padding, byte teamSize, ushort[] items, uint aiFlags, uint isDoubleBattle)
        {
            TrainerType = trainerType;
            TrainerClassId = trainerClassId;
            Padding = padding;
            TeamSize = teamSize;
            Items = items;
            AIFlags = aiFlags;
            IsDoubleBattle = isDoubleBattle;
        }
    }
}