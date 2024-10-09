using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class Pokemon : IEquatable<Pokemon?>
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
        public ushort? BallCapsuleId { get; set; }

        public Pokemon()
        {
            DifficultyValue = 0;
            GenderAbilityOverride = 0;
            Level = 1;
            PokemonId = 0;
            FormId = 0;
            HeldItemId = 0;
            Moves = null;
            BallCapsuleId = 0;
        }

        public Pokemon(byte difficultyValue, byte genderAbilityOverride, ushort level, ushort pokemonId, ushort formId, ushort? heldItemId, ushort[]? moves, ushort? ballCapsuleId = null)
        {
            DifficultyValue = difficultyValue;
            GenderAbilityOverride = genderAbilityOverride;
            Level = level;
            PokemonId = pokemonId;
            FormId = formId;
            HeldItemId = heldItemId;
            Moves = moves;
            BallCapsuleId = ballCapsuleId;
        }

        public override bool Equals(object? obj) => Equals(obj as Pokemon);

        public bool Equals(Pokemon? other) => other is not null &&
                   DifficultyValue == other.DifficultyValue &&
                   GenderOverride == other.GenderOverride &&
                   AbilityOverride == other.AbilityOverride &&
                   GenderAbilityOverride == other.GenderAbilityOverride &&
                   Level == other.Level &&
                   SpeciesId == other.SpeciesId &&
                   IconId == other.IconId &&
                   PokemonId == other.PokemonId &&
                   FormId == other.FormId &&
                   HeldItemId == other.HeldItemId &&
                   EqualityComparer<ushort[]?>.Default.Equals(Moves, other.Moves) &&
                   BallCapsuleId == other.BallCapsuleId;

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(DifficultyValue);
            hash.Add(GenderOverride);
            hash.Add(AbilityOverride);
            hash.Add(GenderAbilityOverride);
            hash.Add(Level);
            hash.Add(SpeciesId);
            hash.Add(IconId);
            hash.Add(PokemonId);
            hash.Add(FormId);
            hash.Add(HeldItemId);
            hash.Add(Moves);
            hash.Add(BallCapsuleId);
            return hash.ToHashCode();
        }
    }
}