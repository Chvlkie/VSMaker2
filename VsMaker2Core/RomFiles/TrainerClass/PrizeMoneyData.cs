namespace VsMaker2Core.RomFiles
{
    public class PrizeMoneyData
    {
        public long Offset { get; set; }
        public ushort TrainerClassId { get; set; }
        public ushort PrizeMoney { get; set; }

        public PrizeMoneyData()
        { Offset = -1; }

        public PrizeMoneyData(long offset, ushort trainerClassId, ushort prizeMoney)
        {
            Offset = offset;
            TrainerClassId = trainerClassId;
            PrizeMoney = prizeMoney;
        }
    }
}