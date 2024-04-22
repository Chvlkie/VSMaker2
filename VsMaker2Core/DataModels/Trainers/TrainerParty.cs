namespace VsMaker2Core.DataModels
{
    public class TrainerParty
    {
        public ushort PartySize { get; set; }
        public bool DoubleBattle { get; set; }
        public bool HeldItems { get; set; }
        public bool ChooseMoves { get; set; }
        public List<MessageArchive> Pokemons { get; set; }

        public TrainerParty()
        {
            Pokemons = [];
        }
    }
}