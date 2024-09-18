using static VsMaker2Core.DataModels.Pokemon;

namespace VsMaker2Core.DataModels
{
    public partial class Species : IEquatable<Species?>
    {
        public ushort SpeciesId { get; set; }
        public byte GenderRatio { get; set; }
        public byte Ability1 { get; set; }
        public byte Ability2 { get; set; }

        public bool HasMoreThanOneGender => GenderRatio switch
        {
            Constants.GenderRatioMale or Constants.GenderRatioFemale or Constants.GenderRatioGenderless => false,
            _ => true,
        };

        public bool HasNoAbilities => Ability1 == 0 && Ability2 == 0;
        public bool HasMoreThanOneAbility => Ability2 > 0;

        public static bool HasMoreThanOneForm(int pokemonId) => pokemonId switch
        {
            Pokedex.PichuId => true,
            Pokedex.UnownId => true,
            Pokedex.CastformId => true,
            Pokedex.DeoxysId => true,
            Pokedex.BurmyId => true,
            Pokedex.WormadamId => true,
            Pokedex.ShellosId => true,
            Pokedex.GastrodonId => true,
            Pokedex.RotomId => true,
            Pokedex.GiratinaId => true,
            Pokedex.ShayminId => true,
            _ => false,
        };

        public override bool Equals(object? obj)
        {
            return Equals(obj as Species);
        }

        public bool Equals(Species? other)
        {
            return other is not null &&
                   SpeciesId == other.SpeciesId &&
                   GenderRatio == other.GenderRatio &&
                   Ability1 == other.Ability1 &&
                   Ability2 == other.Ability2 &&
                   HasMoreThanOneGender == other.HasMoreThanOneGender &&
                   HasMoreThanOneAbility == other.HasMoreThanOneAbility;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpeciesId, GenderRatio, Ability1, Ability2, HasMoreThanOneGender, HasMoreThanOneAbility);
        }

        public Species()
        { }

        public Species(ushort speciesId, byte genderRatio, byte ability1, byte ability2)
        {
            SpeciesId = speciesId;
            GenderRatio = genderRatio;
            Ability1 = ability1;
            Ability2 = ability2;
        }
    }
}