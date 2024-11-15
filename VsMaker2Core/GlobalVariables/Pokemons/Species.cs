namespace VsMaker2Core.DataModels
{
    public partial class Species
    {
        public static ushort GetSpecialSpecies(ushort pokemonId, ushort formId) => pokemonId switch
        {
            Pokemon.Pokedex.Deoxys => formId > 0 ? AltForms.DeoxysAltForms(formId) : pokemonId,
            Pokemon.Pokedex.Wormadam => formId > 0 ? AltForms.WormadamAltForms(formId) : pokemonId,
            Pokemon.Pokedex.Rotom => formId > 0 ? AltForms.RotomAltForms(formId) : pokemonId,
            Pokemon.Pokedex.Giratina => formId > 0 ? AltForms.GiratinaAltForm : pokemonId,
            Pokemon.Pokedex.Shaymin => formId > 0 ? (ushort)(AltForms.ShayminAltForm + formId) : pokemonId,
            _ =>  pokemonId,
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

                public static List<string> Unown => Enumerable.Range('A', 26).Select(c => ((char)c).ToString()).Concat(["!", "?"]).ToList();

                public static class HgEngineForms
                {
                    #region HG Engine Forms

                    #region GEN I

                    public static List<string> Aerodactyl => [Default, .. MegaEvolutionForm()];
                    public static List<string> Alakazam => [Default, .. MegaEvolutionForm()];
                    public static List<string> Articuno => [Default, Galarian];
                    public static List<string> Beedrill => [Default, .. MegaEvolutionForm()];
                    public static List<string> Blastoise => [Default, .. MegaEvolutionForm()];
                    public static List<string> Charizard => [Default, .. MegaEvolutionForm(true)];
                    public static List<string> Diglett => [Default, Alolan];
                    public static List<string> Dugtrio => [Default, Alolan];
                    public static List<string> Exeggutor => [Default, Alolan];
                    public static List<string> Farfetchd => [Default, Galarian];
                    public static List<string> Gengar => [Default, .. MegaEvolutionForm()];
                    public static List<string> Geodude => [Default, Alolan];
                    public static List<string> Golem => [Default, Alolan];
                    public static List<string> Graveler => [Default, Alolan];
                    public static List<string> Grimer => [Default, Alolan];
                    public static List<string> Gyarados => [Default, .. MegaEvolutionForm()];
                    public static List<string> Kangaskhan => [Default, .. MegaEvolutionForm()];
                    public static List<string> Marowak => [Default, Alolan];
                    public static List<string> Meowth => [Default, Alolan, Galarian];
                    public static List<string> Mewtwo => [Default, .. MegaEvolutionForm(true)];
                    public static List<string> Moltres => [Default, Galarian];
                    public static List<string> MrMime => [Default, Galarian];
                    public static List<string> Muk => [Default, Alolan];
                    public static List<string> Nintetales => [Default, Alolan];
                    public static List<string> Persian => [Default, Alolan];
                    public static List<string> Pidgeot => [Default, .. MegaEvolutionForm()];
                    public static List<string> Pinsir => [Default, .. MegaEvolutionForm()];
                    public static List<string> Ponyta => [Default, Galarian];
                    public static List<string> Pikachu => [Default, .. PikachuCosmetic];
                    public static List<string> Eevee => [Default, EeveeCosmetic];
                    public static List<string> Raichu => [Default, Alolan];
                    public static List<string> Rapidash => [Default, Galarian];
                    public static List<string> Raticate => [Default, Alolan];
                    public static List<string> Rattata => [Default, Alolan];
                    public static List<string> Sandshrew => [Default, Alolan];
                    public static List<string> Sandslash => [Default, Alolan];
                    public static List<string> Slowbro => [Default, .. MegaEvolutionForm(), Galarian];
                    public static List<string> Slowpoke => [Default, Galarian + " A", Galarian + " B"];
                    public static List<string> Venusaur => [Default, .. MegaEvolutionForm()];
                    public static List<string> Vulpix => [Default, Alolan];
                    public static List<string> Weezing => [Default, Galarian];
                    public static List<string> Zapdos => [Default, Galarian];

                    #endregion GEN I

                    #region GEN II

                    public static List<string> Ampharos => [Default, .. MegaEvolutionForm()];
                    public static List<string> Steelix => [Default, .. MegaEvolutionForm()];
                    public static List<string> Scizor => [Default, .. MegaEvolutionForm()];
                    public static List<string> Heracross => [Default, .. MegaEvolutionForm()];
                    public static List<string> Houndoom => [Default, .. MegaEvolutionForm()];
                    public static List<string> Tyranitar => [Default, .. MegaEvolutionForm()];
                    public static List<string> Slowking => [Default, Galarian];
                    public static List<string> Corsola => [Default, Galarian];

                    #endregion GEN II

                    #region GEN III

                    public static List<string> Sceptile => [Default, .. MegaEvolutionForm()];
                    public static List<string> Blaziken => [Default, .. MegaEvolutionForm()];
                    public static List<string> Swampert => [Default, .. MegaEvolutionForm()];
                    public static List<string> Gardevoir => [Default, .. MegaEvolutionForm()];
                    public static List<string> Sableye => [Default, .. MegaEvolutionForm()];
                    public static List<string> Mawile => [Default, .. MegaEvolutionForm()];
                    public static List<string> Aggron => [Default, .. MegaEvolutionForm()];
                    public static List<string> Medicham => [Default, .. MegaEvolutionForm()];
                    public static List<string> Manectric => [Default, .. MegaEvolutionForm()];
                    public static List<string> Sharpedo => [Default, .. MegaEvolutionForm()];
                    public static List<string> Camerupt => [Default, .. MegaEvolutionForm()];
                    public static List<string> Altaria => [Default, .. MegaEvolutionForm()];
                    public static List<string> Banette => [Default, .. MegaEvolutionForm()];
                    public static List<string> Absol => [Default, .. MegaEvolutionForm()];
                    public static List<string> Glalie => [Default, .. MegaEvolutionForm()];
                    public static List<string> Salamence => [Default, .. MegaEvolutionForm()];
                    public static List<string> Metagross => [Default, .. MegaEvolutionForm()];
                    public static List<string> Latias => [Default, .. MegaEvolutionForm()];
                    public static List<string> Latios => [Default, .. MegaEvolutionForm()];
                    public static List<string> Kyogre => [Default, Primal];
                    public static List<string> Groudon => [Default, Primal];
                    public static List<string> Rayquaza => [Default, .. MegaEvolutionForm()];
                    public static List<string> Zizagoon => [Default, Galarian];
                    public static List<string> Linoone => [Default, Galarian];

                    #endregion GEN III

                    #region GEN IV

                    public static List<string> Lopunny => [Default, .. MegaEvolutionForm()];
                    public static List<string> Garchomp => [Default, .. MegaEvolutionForm()];
                    public static List<string> Lucario => [Default, .. MegaEvolutionForm()];
                    public static List<string> Abomasnow => [Default, .. MegaEvolutionForm()];
                    public static List<string> Gallade => [Default, .. MegaEvolutionForm()];

                    #endregion GEN IV

                    #region GEN V

                    public static List<string> Audino => [Default, .. MegaEvolutionForm()];

                    public static List<string> Darumaka => [Default, Galarian];
                    public static List<string> Darmanitan => [Default, Galarian];
                    public static List<string> Yamask => [Default, Galarian];
                    public static List<string> Stunfisk => [Default, Galarian];
                    public static List<string> Basulin => [Default, .. BasculingCosmetic];
                    public static List<string> Deerling => [Default, .. DeerlingCosmetic];
                    public static List<string> Sawsbuck => [Default, .. Sawsbuck];

                    #endregion GEN V

                    #region GEN VI

                    public static List<string> Diancie => [Default, .. MegaEvolutionForm()];

                    #endregion GEN VI

                    #endregion HG Engine Forms

                    #region Mega Evolutions

                    public const string Mega = "Mega";
                    public const string MegaX = "Mega X";
                    public const string MegaY = "Mega Y";

                    public static List<string> MegaEvolutionForm(bool hasXY = false) => hasXY ? [MegaX, MegaY] : [Mega];

                    #endregion Mega Evolutions

                    #region Primal Reversions

                    public const string Primal = "Primal";

                    #endregion Primal Reversions

                    #region Alolan

                    public const string Alolan = "Alolan";

                    #endregion Alolan

                    #region Galarian

                    public const string Galarian = "Galarian";

                    #endregion Galarian

                    #region Cosmetic

                    public static List<string> PikachuCosmetic =>
                        ["Cosplay", "Rock Star", "Belle", "Pop Star", "PHD", "Libre", "Original Cap", "Hoen Cap", "Sinnoh Cap", "Unova Cap", "Kalos Cap", "Alola Cap",
                    "Partner Cap", "World Cap", "Let's Go"];

                    public const string EeveeCosmetic = "Let's Go";

                    public static List<string> BasculingCosmetic =>
                        ["Blue Stripe", "White Stripe"];

                    public static List<string> DeerlingCosmetic =>
                         ["Summer", "Autumn", "Winter"];

                    public static List<string> SawsbuckCosmetic =>
                       ["Summer", "Autumn", "Winter"];

                    public const string TornadusCosmetic = "Therian";
                    public const string ThundurusCosmetic = "Therian";
                    public const string LandorusCosmetic = "Therian";

                    public static List<string> KyurimCosmetic =>
                     ["White", "Black"];

                    public const string KeldeoCosmetic = "Resolute";

                    public static List<string> GenesectCosmetic =>
                               ["Blue Stripe", "White Stripe"];

                    #endregion Cosmetic
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