namespace VsMaker2Core.DataModels
{
    [Serializable]
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

        public Trainer(Trainer trainer)
        {
            TrainerId = trainer.TrainerId;
            TrainerName = trainer.TrainerName;
            TrainerProperties = trainer.TrainerProperties;
            TrainerParty = trainer.TrainerParty;
        }
    }
}