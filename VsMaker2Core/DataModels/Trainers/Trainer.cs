
namespace VsMaker2Core.DataModels
{
    public partial class Trainer : IEquatable<Trainer?>
    {
        public ushort TrainerId { get; set; }
        public string TrainerName { get; set; }
        public TrainerProperty TrainerProperties { get; set; }
        public TrainerParty TrainerParty { get; set; }

        public List<TrainerUsage> TrainerUsages { get; set; }
        public string ListName => $"[{TrainerId:D4}] {TrainerName}";

        public static int ListNameToTrainerId(string listName) => int.Parse(listName.Substring(1, 4));

        public override bool Equals(object? obj)
        {
            return Equals(obj as Trainer);
        }

        public bool Equals(Trainer? other)
        {
            return other is not null &&
                   TrainerId == other.TrainerId &&
                   TrainerName == other.TrainerName &&
                   EqualityComparer<TrainerProperty>.Default.Equals(TrainerProperties, other.TrainerProperties) &&
                   EqualityComparer<TrainerParty>.Default.Equals(TrainerParty, other.TrainerParty) &&
                   EqualityComparer<List<TrainerUsage>>.Default.Equals(TrainerUsages, other.TrainerUsages) &&
                   ListName == other.ListName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrainerId, TrainerName, TrainerProperties, TrainerParty, TrainerUsages, ListName);
        }

        public Trainer()
        {
            TrainerProperties = new();
            TrainerParty = new();
        }

        public Trainer(ushort trainerId, string trainerName, TrainerProperty trainerProperties, TrainerParty trainerParty, List<TrainerUsage> trainerUsage)
        {
            TrainerId = trainerId;
            TrainerName = trainerName;
            TrainerProperties = trainerProperties;
            TrainerParty = trainerParty;
            TrainerUsages = trainerUsage;
        }

        // Default New Trainer
        public Trainer(int trainerId)
        {
            TrainerId = (ushort)trainerId;
            TrainerName = "-";
            TrainerProperties = new();
            TrainerParty = new();
            TrainerUsages = [];
        }

        public Trainer(Trainer trainer)
        {
            TrainerId = trainer.TrainerId;
            TrainerName = trainer.TrainerName;
            TrainerProperties = trainer.TrainerProperties;
            TrainerParty = trainer.TrainerParty;
            TrainerUsages = trainer.TrainerUsages;
        }

        public Trainer(Trainer trainer, TrainerProperty toCopy)
        {
            TrainerId = trainer.TrainerId;
            TrainerName = trainer.TrainerName;
            TrainerProperties = toCopy;
            TrainerParty = trainer.TrainerParty;
        }

        public Trainer(Trainer trainer, TrainerParty toCopy)
        {
            TrainerId = trainer.TrainerId;
            TrainerName = trainer.TrainerName;
            TrainerProperties = trainer.TrainerProperties;
            TrainerParty = toCopy;
        }

        public Trainer(int originalId, Trainer clipBoard)
        {
            TrainerId = (ushort)originalId;
            TrainerName = clipBoard.TrainerName;
            TrainerProperties = clipBoard.TrainerProperties;
            TrainerParty = clipBoard.TrainerParty;
        }

        public static bool operator ==(Trainer? left, Trainer? right)
        {
            return EqualityComparer<Trainer>.Default.Equals(left, right);
        }

        public static bool operator !=(Trainer? left, Trainer? right)
        {
            return !(left == right);
        }
    }
}