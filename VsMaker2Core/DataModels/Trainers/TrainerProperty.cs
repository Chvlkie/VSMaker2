namespace VsMaker2Core.DataModels
{
    public class TrainerProperty : IEquatable<TrainerProperty?>
    {
        public List<bool> AIFlags { get; set; }

        public bool ChooseMoves { get; set; }
        public bool ChooseItems { get; set; }

        public byte TrainerClassId { get; set; }

        public ushort[] Items { get; set; }

        public uint BattleType { get; set; }

        public byte TeamSize { get; set; }

        public List<bool> PropertyFlags => RomFile.IsHgEngine ?
            [
            ChooseMoves,
            ChooseItems,
            ChooseAbility_Hge,
            ChooseBall_Hge, SetIvEv_Hge,
            ChooseNature_Hge,
            ShinyLock_Hge,
            AdditionalFlags_Hge
            ]
        :
            [
            ChooseMoves,
            ChooseItems
            ];

        //HG Engine Parameters
        public bool ChooseAbility_Hge { get; set; }

        public bool ChooseBall_Hge { get; set; }
        public bool SetIvEv_Hge { get; set; }
        public bool ChooseNature_Hge { get; set; }
        public bool ShinyLock_Hge { get; set; }
        public bool AdditionalFlags_Hge { get; set; }

        public TrainerProperty()
        {
            AIFlags = [true, false, false, false, false, false, false, false, false, false, false];
            Items = [0, 0, 0, 0];
            TrainerClassId = 2;
            TeamSize = 0;
        }

        public TrainerProperty(TrainerProperty original, uint battleType, byte teamSize, bool chooseMoves, bool chooseItems)
        {
            AIFlags = original.AIFlags;
            Items = original.Items;
            TeamSize = teamSize;
            ChooseMoves = chooseMoves;
            ChooseItems = chooseItems;
            TrainerClassId = original.TrainerClassId;
            BattleType = battleType;
        }

        public TrainerProperty(uint battleType, byte teamSize, bool chooseMoves, bool chooseItems, byte trainerClassId, ushort[] items, List<bool> aiFlags)
        {
            AIFlags = aiFlags;
            Items = items;
            TeamSize = teamSize;
            ChooseMoves = chooseMoves;
            ChooseItems = chooseItems;
            TrainerClassId = trainerClassId;
            BattleType = battleType;
        }

        public TrainerProperty(TrainerProperty toCopy)
        {
            AIFlags = toCopy.AIFlags;
            Items = toCopy.Items;
            TeamSize = toCopy.TeamSize;
            ChooseMoves = toCopy.ChooseMoves;
            ChooseItems = toCopy.ChooseItems;
            TrainerClassId = toCopy.TrainerClassId;
            BattleType = toCopy.BattleType;
        }

        public override bool Equals(object? obj) => Equals(obj as TrainerProperty);

        public bool Equals(TrainerProperty? other) => other is not null &&
                   EqualityComparer<List<bool>>.Default.Equals(AIFlags, other.AIFlags) &&
                   ChooseMoves == other.ChooseMoves &&
                   ChooseItems == other.ChooseItems &&
                   TrainerClassId == other.TrainerClassId &&
                   EqualityComparer<ushort[]>.Default.Equals(Items, other.Items) &&
                   BattleType == other.BattleType &&
                   TeamSize == other.TeamSize;

        public override int GetHashCode() => HashCode.Combine(AIFlags, ChooseMoves, ChooseItems, TrainerClassId, Items, BattleType,  TeamSize);
    }
}