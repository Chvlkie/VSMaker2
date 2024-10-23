namespace VsMaker2Core.DataModels
{
    public class TrainerProperty : IEquatable<TrainerProperty?>
    {
        public List<bool> AIFlags { get; set; }

        public bool ChooseMoves { get; set; }
        public bool ChooseItems { get; set; }

        public byte TrainerClassId { get; set; }

        public ushort[] Items { get; set; }

        public bool DoubleBattle { get; set; }

        public byte TeamSize { get; set; }

        public TrainerProperty()
        {
            AIFlags = [true, false, false, false, false, false, false, false, false, false, false];
            Items = [0, 0, 0, 0];
            TrainerClassId = 2;
            TeamSize = 0;
        }

        public TrainerProperty(TrainerProperty original, bool doubleBattle, byte teamSize, bool chooseMoves, bool chooseItems)
        {
            AIFlags = original.AIFlags;
            Items = original.Items;
            TeamSize = teamSize;
            ChooseMoves = chooseMoves;
            ChooseItems = chooseItems;
            TrainerClassId = original.TrainerClassId;
            DoubleBattle = doubleBattle;
        }

        public TrainerProperty(bool doubleBattle, byte teamSize, bool chooseMoves, bool chooseItems, byte trainerClassId, ushort[] items, List<bool> aiFlags)
        {
            AIFlags = aiFlags;
            Items = items;
            TeamSize = teamSize;
            ChooseMoves = chooseMoves;
            ChooseItems = chooseItems;
            TrainerClassId = trainerClassId;
            DoubleBattle = doubleBattle;
        }

        public TrainerProperty(TrainerProperty toCopy)
        {
            AIFlags = toCopy.AIFlags;
            Items = toCopy.Items;
            TeamSize = toCopy.TeamSize;
            ChooseMoves = toCopy.ChooseMoves;
            ChooseItems = toCopy.ChooseItems;
            TrainerClassId = toCopy.TrainerClassId;
            DoubleBattle = toCopy.DoubleBattle;
        }

        public override bool Equals(object? obj) => Equals(obj as TrainerProperty);

        public bool Equals(TrainerProperty? other) => other is not null &&
                   EqualityComparer<List<bool>>.Default.Equals(AIFlags, other.AIFlags) &&
                   ChooseMoves == other.ChooseMoves &&
                   ChooseItems == other.ChooseItems &&
                   TrainerClassId == other.TrainerClassId &&
                   EqualityComparer<ushort[]>.Default.Equals(Items, other.Items) &&
                   DoubleBattle == other.DoubleBattle &&
                   TeamSize == other.TeamSize;

        public override int GetHashCode() => HashCode.Combine(AIFlags, ChooseMoves, ChooseItems, TrainerClassId, Items, DoubleBattle, TeamSize);
    }
}