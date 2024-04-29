namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class TrainerClass
    {
        public uint TrainerClassId { get; set; }
        public string TrainerClassName { get; set; }

        public uint EyeContactMusic { get; set; }
        public uint? EyeContactMusicNight { get; set; }
        public uint PrizeMoneyMultiplier { get; set; }

        public List<Trainer> UsedByTrainers { get; set; }

        public string ListName => $"[{TrainerClassId:D4}] {TrainerClassName}";

        public static int ListNameToTrainerClassId(string listName) => int.Parse(listName.Substring(1, 4));
    }
}