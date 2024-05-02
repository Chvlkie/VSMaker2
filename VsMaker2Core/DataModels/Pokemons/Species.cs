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
    }
}