namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class TrainerParty
    {
        public List<Pokemon> Pokemons { get; set; }

        public TrainerParty()
        {
            Pokemons = [];
        }
    }
}