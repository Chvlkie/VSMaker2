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

        //HG Engine Params
        public ushort? AbilityHge { get; set; }
        public ushort? BallHge { get; set; }
        public byte[]? IvNumsHge { get; set; }
        public byte[]? EvNumsHge { get; set; }
        public byte? NatureHge { get; set; }
        public byte? ShinyLockHge { get; set; }
        public uint? AdditionalFlagsHge { get; set; }
        public uint? StatusHge { get; set; }
        public ushort? HpHge { get; set; }
        public ushort? AtkHge { get; set; }
        public ushort? DefHge { get; set; }
        public ushort? SpeedHge { get; set; }
        public ushort? SpAtkHge { get; set; }
        public ushort? SpDefHge { get; set; }
        public byte[]? TypesHge { get; set; }
        public byte[]? PpCountsHge { get; set; }
        public ushort[]? NicknameHge { get; set; }
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