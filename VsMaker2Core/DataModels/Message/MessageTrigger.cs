namespace VsMaker2Core.DataModels
{
    [Serializable]
    public partial class MessageTrigger : IEquatable<MessageTrigger?>
    {
        public int MessageTriggerId { get; set; }
        public string MessageTriggerName { get; set; }
        public string ListName => $"[{MessageTriggerId:D2}] - {MessageTriggerName}";

        public static int ListNameToMessageTriggerId(string listName) => int.Parse(listName.Substring(1, 2));

        public override bool Equals(object? obj)
        {
            return Equals(obj as MessageTrigger);
        }

        public bool Equals(MessageTrigger? other)
        {
            return other is not null &&
                   MessageTriggerId == other.MessageTriggerId &&
                   MessageTriggerName == other.MessageTriggerName &&
                   ListName == other.ListName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MessageTriggerId, MessageTriggerName, ListName);
        }

        public MessageTrigger()
        { }

        public MessageTrigger(int messageTriggerId, string messageTriggerName)
        {
            MessageTriggerId = messageTriggerId;
            MessageTriggerName = messageTriggerName;
        }
    }
}