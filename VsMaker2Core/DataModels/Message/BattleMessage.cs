namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class BattleMessage
    {
        public int TrainerId { get; set; }
        public int MessageId { get; set; }
        public int MessageTriggerId { get; set; }
        public string MessageText { get; set; }

        public BattleMessage()
        { }

        public BattleMessage(int trainerId, int messageId, int messageTriggerId, string messageText)
        {
            TrainerId = trainerId;
            MessageId = messageId;
            MessageTriggerId = messageTriggerId;
            MessageText = messageText;
        }
    }
}