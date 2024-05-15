namespace VsMaker2Core.RomFiles
{
    public class BattleMessageTableData
    {
        public int MessageId { get; set; }
        public uint TrainerId { get; set; }
        public ushort MessageTriggerId { get; set; }

        public BattleMessageTableData()
        { }

        public BattleMessageTableData(int messageId, uint trainerId, ushort messageTriggerId)
        {
            MessageId = messageId;
            TrainerId = trainerId;
            MessageTriggerId = messageTriggerId;
        }
    }
}