namespace VsMaker2Core.DataModels
{
    public class TrainerClass
    {
        public int TrainerClassId { get; set; }
        public string TrainerClassName { get; set; }

        public int EyeContactMusic { get; set; }
        public int? EyeContactMusicNight { get; set; }
        public int PrizeMoneyMultiplier { get; set; }
        public int Gender { get; set; }
        public List<Trainer> UsedByTrainers { get; set; }
        public string Description { get; set; }

        public string ListName => $"[{TrainerClassId:D4}] {TrainerClassName}";
        public string FullDescription => Description + " " + TrainerClassName;

        public static int ListNameToTrainerClassId(string listName) => int.Parse(listName.Substring(1, 4));

        public TrainerClass()
        {
            UsedByTrainers = [];
        }
    }
}