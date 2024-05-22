namespace VsMaker2Core.DataModels
{
    public partial class Species
    {
        public static ushort GetSpecialSpecies(ushort pokemonId, ushort formId) => pokemonId switch
        {
            Pokemon.Pokedex.DeoxysId => formId > 0 ? AltForms.DeoxysAltForms(formId) : pokemonId,
            Pokemon.Pokedex.WormadamId => formId > 0 ? AltForms.WormadamAltForms(formId) : pokemonId,
            Pokemon.Pokedex.RotomId => formId > 0 ? AltForms.RotomAltForms(formId) : pokemonId,
            Pokemon.Pokedex.GiratinaId => formId > 0 ? AltForms.GiratinaAltForm : pokemonId,
            Pokemon.Pokedex.ShayminId => formId > 0 ? (ushort)(AltForms.ShayminAltForm + formId) : pokemonId,
            _ => pokemonId,
        };

        public static class AltForms
        {
            public const ushort GiratinaAltForm = 501;

            public const ushort ShayminAltForm = 502;

            public static ushort DeoxysAltForms(ushort formId) => (ushort)(495 + formId);

            public static ushort RotomAltForms(ushort formId) => (ushort)(502 + formId);

            public static ushort WormadamAltForms(ushort formId) => (ushort)(498 + formId);

            public static class FormNames
            {
                public const string Default = "-";

                public static List<string> BurmyWormadam => ["Plant", "Sand", "Trash"];
                public static List<string> Castform => [Default, "Rainy", "Sunny", "Snowy",];

                public static List<string> Deoxys => [Default, "Attack", "Defense", "Speed",];

                public static List<string> GiratinaForms => [Default, "Origin"];

                public static List<string> Pichu => [Default, "Spiky-Ear"];

                public static List<string> Rotom => [Default, "Heat", "Wash", "Frost", "Fan", "Mow"];
                public static List<string> ShayminForms => ["Land", "Sky"];
                public static List<string> ShellosGastrodon => ["West", "East"];

                public static List<string> Unown
                {
                    get
                    {
                        List<string> forms = [];
                        for (char c = 'A'; c <= 'Z'; c++)
                        {
                            forms.Add(c.ToString());
                        }
                        forms.Add("!");
                        forms.Add("?");

                        return forms;
                    }
                }
            }
        }

        public static class Constants
        {
            public const int AbilitySlot1ByteOffset = 22;
            public const int GenderRatioByteOffset = 16;
            public const int GenderRatioFemale = 254;
            public const int GenderRatioGenderless = 255;
            public const int GenderRatioMale = 0;
        }
    }
}