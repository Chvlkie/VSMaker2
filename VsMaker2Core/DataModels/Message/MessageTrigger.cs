namespace VsMaker2Core.DataModels
{
    [Serializable]
    public partial class MessageTrigger
    {
        public int MessageTriggerId { get; set; }
        public string MessageTriggerName { get; set; }
        public string ListName => $"[{MessageTriggerId:D2}] - {MessageTriggerName}";
        public static int ListNameToMessageTriggerId(string listName) => int.Parse(listName.Substring(1, 2));

        public MessageTrigger() { }
        public MessageTrigger(int messageTriggerId, string messageTriggerName)
        {
            MessageTriggerId = messageTriggerId;
            MessageTriggerName = messageTriggerName;
        }
    }
}
