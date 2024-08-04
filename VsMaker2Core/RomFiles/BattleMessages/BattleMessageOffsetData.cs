namespace VsMaker2Core.RomFiles
{
    public class BattleMessageOffsetData
    {
        public int MessageId { get; set; }
        public ushort Offset => (ushort)(MessageId * 4);

        public static int OffsetToMessageId(ushort offset) => offset / 4;

        public BattleMessageOffsetData()
        { }

        public BattleMessageOffsetData(int messageId)
        {
            MessageId = messageId;
        }
    }
}