namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class MessageArchive : IEquatable<MessageArchive?>
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }

        public MessageArchive(int messageId, string messageText)
        {
            MessageId = messageId;
            MessageText = messageText;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as MessageArchive);
        }

        public bool Equals(MessageArchive? other)
        {
            return other is not null &&
                   MessageId == other.MessageId &&
                   MessageText == other.MessageText;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MessageId, MessageText);
        }
    }
}