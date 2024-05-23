namespace VsMaker2Core.DataModels
{
    public class TrainerParty
    {
        public List<Pokemon> Pokemons { get; set; }
        public bool ChooseItems { get; set; }
        public bool ChooseMoves { get; set; }
        public bool DoubleBattle { get; set; }
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
            DoubleBattle = toCopy.DoubleBattle;
        }
    }
}