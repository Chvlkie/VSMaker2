namespace VsMaker2Core.DataModels
{
    public class TrainerParty
    {
        public List<Pokemon> Pokemons { get; set; }

        public TrainerParty()
        {
            Pokemons = [];
        }

        public TrainerParty(List<Pokemon> pokemons)
        {
            Pokemons = pokemons;
        }
    }
}