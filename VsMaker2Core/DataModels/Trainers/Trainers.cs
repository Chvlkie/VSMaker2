namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class Trainers(List<Trainer> trainerData)
    {
        public List<Trainer> TrainerData { get; set; } = trainerData;
    }
}