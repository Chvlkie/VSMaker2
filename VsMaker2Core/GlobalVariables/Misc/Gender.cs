namespace VsMaker2Core.DataModels
{
    public static class Gender
    {
        public const string Default = "-";
        public const string Male = "M";
        public const string Female = "F";

        public static List<string> ClassGenders =>
            [
            Male,
            Female,
            Default,
            ];

        public static List<string> PokemonGenders =>
            [
            Default,
            Male,
            Female
            ];
    }
}