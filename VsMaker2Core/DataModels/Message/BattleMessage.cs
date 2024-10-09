namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class BattleMessage : IEquatable<BattleMessage?>
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

        public BattleMessage(int trainerId, int messageTriggerId)
        {
            TrainerId = trainerId;
            MessageTriggerId = messageTriggerId;
        }

        public override bool Equals(object? obj) => Equals(obj as BattleMessage);

        public bool Equals(BattleMessage? other) => other is not null &&
                   TrainerId == other.TrainerId &&
                   MessageId == other.MessageId &&
                   MessageTriggerId == other.MessageTriggerId &&
                   MessageText == other.MessageText;

        public override int GetHashCode() => HashCode.Combine(TrainerId, MessageId, MessageTriggerId, MessageText);
    }
}