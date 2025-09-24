namespace VsMaker2Core.DataModels
{
    public class TrainerParty : IEquatable<TrainerParty?>
    {
        public List<Pokemon> Pokemons { get; set; }
        public bool ChooseItems { get; set; }
        public bool ChooseMoves { get; set; }
        public uint BattleType { get; set; }
        public byte TeamSize { get; set; }

        public TrainerParty()
        {
            Pokemons = [];
        }

        public TrainerParty(List<Pokemon> pokemons)
        {
            Pokemons = pokemons;
        }

        public TrainerParty(TrainerParty toCopy)
        {
            Pokemons = toCopy.Pokemons;
            ChooseItems = toCopy.ChooseItems;
            ChooseMoves = toCopy.ChooseMoves;
            BattleType = toCopy.BattleType;
        }

        public override bool Equals(object? obj) => Equals(obj as TrainerParty);

        public bool Equals(TrainerParty? other) => other is not null &&
                   EqualityComparer<List<Pokemon>>.Default.Equals(Pokemons, other.Pokemons) &&
                   ChooseItems == other.ChooseItems &&
                   ChooseMoves == other.ChooseMoves &&
                   BattleType == other.BattleType &&
                   TeamSize == other.TeamSize;

        public override int GetHashCode() => HashCode.Combine(Pokemons, ChooseItems, ChooseMoves, BattleType, TeamSize);
    }
}