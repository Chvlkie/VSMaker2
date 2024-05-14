using static VsMaker2Core.DataModels.Pokemon;

namespace VsMaker2Core.DataModels
{
    public partial class Species
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