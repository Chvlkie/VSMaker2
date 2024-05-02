using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    [Serializable]
    public partial class Pokemon
    {
        public byte DifficultyValue { get; set; }
        public GenderOverride GenderOverride => (GenderOverride)(GenderAbilityOverride & 0xF);
        public AbilityOverride AbilityOverride => (AbilityOverride)((GenderAbilityOverride & 0xF0) >> 4);
        public byte GenderAbilityOverride { get; set; }
        public ushort Level { get; set; }
        public ushort SpeciesId => Species.GetSpecialSpecies(PokemonId, FormId);
        public int IconId => GetSpecialIcon(PokemonId, FormId);
        public ushort PokemonId { get; set; }
        public ushort FormId { get; set; }
        public ushort? HeldItemId { get; set; }
        public ushort[]? Moves { get; set; }
        public ushort? BallSealId { get; set; }

        public Pokemon()
        {
            DifficultyValue = 0;
            GenderAbilityOverride = 0;
            Level = 1;
            PokemonId = 0;
            FormId = 0;
            HeldItemId = 0;
            Moves = null;
            BallSealId = 0;
        }
    }
}