namespace VsMaker2Core.RomFiles
{
    public class TrainerPartyData
    {
        public TrainerPartyPokemonData[] PokemonData { get; set; }

        public TrainerPartyData()
        {
            PokemonData = [];
        }

        public TrainerPartyData(TrainerPartyPokemonData[] pokemonData)
        {
            PokemonData = pokemonData;
        }
    }
}