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
        public ushort? Ability_Hge { get; set; }
        public ushort? Ball_Hge { get; set; }
        public byte[]? IvNums_Hge { get; set; }
        public byte[]? EvNums_Hge { get; set; }
        public byte? Nature_Hge { get; set; }
        public byte? ShinyLock_Hge { get; set; }
        public uint? AdditionalFlags_Hge { get; set; }
        public uint? Status_Hge { get; set; }
        public ushort? Hp_Hge { get; set; }
        public ushort? Atk_Hge { get; set; }
        public ushort? Def_Hge { get; set; }
        public ushort? Speed_Hge { get; set; }
        public ushort? SpAtk_Hge { get; set; }
        public ushort? SpDef_Hge { get; set; }
        public byte[]? Types_Hge { get; set; }
        public byte[]? PpCounts_Hge { get; set; }
        public ushort[]? Nickname_Hge { get; set; }
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