namespace VsMaker2Core.RomFiles
{
    public class BattleMessageOffsetData
    {
        public int MessageId { get; set; }
        public uint Offset => (uint)(MessageId * 4);

        public static int OffsetToMessageId(uint offset) => (int)(offset / 4);

        public BattleMessageOffsetData()
        { }

        public BattleMessageOffsetData(int messageId)
        {
            MessageId = messageId;
        }
    }
}