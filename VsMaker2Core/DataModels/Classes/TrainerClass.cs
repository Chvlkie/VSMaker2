namespace VsMaker2Core.DataModels
{
    public class TrainerClass
    {
        public int TrainerClassId { get; set; }
        public string TrainerClassName { get; set; }

        public List<Trainer> UsedByTrainers { get; set; }

        public string ListName => $"[{TrainerClassId:D4}] {TrainerClassName}";

        public TrainerClassProperty ClassProperties { get; set; }

        public static int ListNameToTrainerClassId(string listName) => int.Parse(listName.Substring(1, 4));

        public TrainerClass()
        {
            UsedByTrainers = [];
            ClassProperties = new();
        }

        public TrainerClass(int trainerClassId, string trainerClassName, TrainerClassProperty trainerClassProperty, List<Trainer> usedByTrainers)
        {
            TrainerClassId = trainerClassId;
            TrainerClassName = trainerClassName;
            ClassProperties = trainerClassProperty;
            UsedByTrainers = usedByTrainers;
        }
    }
}