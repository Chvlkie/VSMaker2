namespace VsMaker2Core.RomFiles
{
    public class TrainerPartyPokemonData
    {
        public byte Difficulty { get; set; }
        public byte GenderAbilityOverride { get; set; }
        public ushort Level { get; set; }
        public ushort Species { get; set; }
        public ushort? BallCapsule { get; set; }

        public ushort? ItemId { get; set; }
        public ushort[]? MoveIds { get; set; }

        public TrainerPartyPokemonData()
        { }

        public TrainerPartyPokemonData(byte difficulty, byte genderAbilityOverride, ushort level, ushort species, ushort? ballCapsule, ushort? itemId, ushort[]? moveIds)
        {
            Difficulty = difficulty;
            GenderAbilityOverride = genderAbilityOverride;
            Level = level;
            Species = species;
            BallCapsule = ballCapsule;
            ItemId = itemId;
            MoveIds = moveIds;
        }
    }
}