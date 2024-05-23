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

        public Trainer(Trainer trainer)
        {
            TrainerId = trainer.TrainerId;
            TrainerName = trainer.TrainerName;
            TrainerProperties = trainer.TrainerProperties;
            TrainerParty = trainer.TrainerParty;
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
    }
}