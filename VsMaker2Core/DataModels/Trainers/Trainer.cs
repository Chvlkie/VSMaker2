namespace VsMaker2Core.DataModels
{
    public partial class Trainer
    {
        public ushort TrainerId { get; set; }
        public string TrainerName { get; set; }
        public TrainerProperty TrainerProperties { get; set; }
        public TrainerParty TrainerParty { get; set; }

        public string ListName => $"[{TrainerId:D4}] {TrainerName}";

        public static int ListNameToTrainerId(string listName) => int.Parse(listName.Substring(1, 4));

        public Trainer()
        {
            TrainerProperties = new();
            TrainerParty = new();
        }

        public Trainer(ushort trainerId, string trainerName, TrainerProperty trainerProperties, TrainerParty trainerParty)
        {
            TrainerId = trainerId;
            TrainerName = trainerName;
            TrainerProperties = trainerProperties;
            TrainerParty = trainerParty;
        }

        // Default New Trainer
        public Trainer(int trainerId)
        {
            TrainerId = (ushort)trainerId;
            TrainerName = "-";
            TrainerProperties = new();
            TrainerParty = new();
        }
    }
}