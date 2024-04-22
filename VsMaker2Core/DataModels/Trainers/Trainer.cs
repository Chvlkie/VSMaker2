namespace VsMaker2Core.DataModels
{
    public class Trainer
    {
        public uint TrainerId { get; set; }
        public string TrainerName { get; set; }
        public TrainerProperty TrainerProperties { get; set; }
        public TrainerParty TrainerParty { get; set; }

        public string ListName => $"[{TrainerId:D3}] {TrainerName}";

        public Trainer()
        {
            TrainerProperties = new();
            TrainerParty = new();
        }
    }
}