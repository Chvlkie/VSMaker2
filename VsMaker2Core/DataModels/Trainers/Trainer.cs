namespace VsMaker2Core.DataModels
{
    public class Trainer
    {
        public uint TrainerId { get; set; }
        public string TrainerName { get; set; }
        public TrainerProperty TrainerProperties { get; set; }
        public TrainerParty TrainerParty { get; set; }

        public string ListName => $"[{TrainerId:D3}] {TrainerName}";

        public static int ListNameToTrainerId(string listName) => int.Parse(listName.Substring(1, 3));

        public Trainer()
        {
            TrainerProperties = new();
            TrainerParty = new();
        }
    }
}