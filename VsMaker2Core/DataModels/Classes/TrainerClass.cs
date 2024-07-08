
namespace VsMaker2Core.DataModels
{
    public class TrainerClass : IEquatable<TrainerClass?>
    {
        public int TrainerClassId { get; set; }
        public string TrainerClassName { get; set; }

        public List<Trainer> UsedByTrainers { get; set; }

        public string ListName => $"[{TrainerClassId:D4}] {TrainerClassName}";

        public TrainerClassProperty ClassProperties { get; set; }

        public static int ListNameToTrainerClassId(string listName) => int.Parse(listName.Substring(1, 4));

        public override bool Equals(object? obj)
        {
            return Equals(obj as TrainerClass);
        }

        public bool Equals(TrainerClass? other)
        {
            return other is not null &&
                   TrainerClassId == other.TrainerClassId &&
                   TrainerClassName == other.TrainerClassName &&
                   EqualityComparer<List<Trainer>>.Default.Equals(UsedByTrainers, other.UsedByTrainers) &&
                   ListName == other.ListName &&
                   EqualityComparer<TrainerClassProperty>.Default.Equals(ClassProperties, other.ClassProperties);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrainerClassId, TrainerClassName, UsedByTrainers, ListName, ClassProperties);
        }

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