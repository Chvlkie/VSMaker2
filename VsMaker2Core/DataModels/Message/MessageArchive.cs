namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class MessageArchive
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }

        public MessageArchive(int messageId, string messageText)
        {
            MessageId = messageId;
            MessageText = messageText;
        }
    }
}