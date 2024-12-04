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

            #region HG Engine Only

            Pokemon.Pokedex.Pikachu => formId > 0 ? AltForms.PikachuAltForms(formId) : pokemonId,
            Pokemon.Pokedex.Meowth => formId > 0 ? AltForms.MeowthAltForms(formId) : pokemonId,
            Pokemon.Pokedex.Rattata => formId > 0 ? AltForms.HgEngineForms.AlolanForms.Rattata : pokemonId,
            Pokemon.Pokedex.Raticate => formId > 0 ? AltForms.HgEngineForms.AlolanForms.Raticate : pokemonId,
            Pokemon.Pokedex.Growlithe => formId > 0 ? AltForms.HgEngineForms.HisuianForms.Growlithe : pokemonId,
            Pokemon.Pokedex.Ponyta => formId > 0 ? AltForms.HgEngineForms.GalarianForms.Ponyta : pokemonId,
            Pokemon.Pokedex.Rapidash => formId > 0 ? AltForms.HgEngineForms.GalarianForms.Rapidah : pokemonId,
            #endregion HG Engine Only

            _ => pokemonId,
        };

        public static class AltForms
        {
            public const ushort GiratinaAltForm = 501;

            public const ushort ShayminAltForm = 502;

            public static ushort DeoxysAltForms(ushort formId) => (ushort)(495 + formId);

            public static ushort RotomAltForms(ushort formId) => (ushort)(502 + formId);

            public static ushort WormadamAltForms(ushort formId) => (ushort)(498 + formId);

            public static ushort PikachuAltForms(ushort formId) => HgEngineForms.CosmeticForms.PikachuForms()[formId - 1];
            public static ushort MeowthAltForms(ushort formId) => formId switch
            {
                1 => HgEngineForms.AlolanForms.Meowth,
                2 => HgEngineForms.GalarianForms.Meowth,
                _ => 0
            };
            public static class HgEngineForms
            {
                public static ushort MaxPokemon => Pokemon.Pokedex.Pecharunt + 50;

                public static class MegaEvolutions
                {
                    public static ushort Start => (ushort)(MaxPokemon + 1);
                    public static ushort End => Diancie;

                    #region GEN I

                    public static ushort Venusaur => Start;
                    public static ushort CharizardX => (ushort)(Start + 1);
                    public static ushort CharizardY => (ushort)(Start + 2);
                    public static ushort Blastiose => (ushort)(Start + 3);
                    public static ushort Beedrill => (ushort)(Start + 4);
                    public static ushort Pidgeot => (ushort)(Start + 5);
                    public static ushort Alakazam => (ushort)(Start + 6);
                    public static ushort Slowbro => (ushort)(Start + 7);
                    public static ushort Gengar => (ushort)(Start + 8);
                    public static ushort Kangaskhan => (ushort)(Start + 9);
                    public static ushort Pinsir => (ushort)(Start + 10);
                    public static ushort Gyarados => (ushort)(Start + 11);
                    public static ushort Aerodactyl => (ushort)(Start + 12);
                    public static ushort MewtwoX => (ushort)(Start + 13);
                    public static ushort MewtwoY => (ushort)(Start + 14);

                    #endregion GEN I

                    #region GEN II

                    public static ushort Ampharos => (ushort)(Start + 15);
                    public static ushort Steelix => (ushort)(Start + 16);
                    public static ushort Scizor => (ushort)(Start + 17);
                    public static ushort Heracross => (ushort)(Start + 18);
                    public static ushort Houndoom => (ushort)(Start + 19);
                    public static ushort Tyranitar => (ushort)(Start + 20);

                    #endregion GEN II

                    #region GEN III

                    public static ushort Sceptile => (ushort)(Start + 21);
                    public static ushort Blaziken => (ushort)(Start + 22);
                    public static ushort Swampert => (ushort)(Start + 23);
                    public static ushort Gardevoir => (ushort)(Start + 24);
                    public static ushort Sableye => (ushort)(Start + 25);
                    public static ushort Mawile => (ushort)(Start + 26);
                    public static ushort Aggron => (ushort)(Start + 27);
                    public static ushort Medicham => (ushort)(Start + 28);
                    public static ushort Manectric => (ushort)(Start + 29);
                    public static ushort Sharpedo => (ushort)(Start + 30);
                    public static ushort Camerupt => (ushort)(Start + 31);
                    public static ushort Altaria => (ushort)(Start + 32);
                    public static ushort Banette => (ushort)(Start + 33);
                    public static ushort Absol => (ushort)(Start + 34);
                    public static ushort Glalie => (ushort)(Start + 35);
                    public static ushort Salamence => (ushort)(Start + 36);
                    public static ushort Metagross => (ushort)(Start + 37);
                    public static ushort Latios => (ushort)(Start + 38);
                    public static ushort Latias => (ushort)(Start + 39);
                    public static ushort Rayquaza => (ushort)(Start + 40);

                    #endregion GEN III

                    #region GEN IV

                    public static ushort Lopunny => (ushort)(Start + 41);
                    public static ushort Garchomp => (ushort)(Start + 42);
                    public static ushort Lucario => (ushort)(Start + 43);
                    public static ushort Abomasnow => (ushort)(Start + 44);
                    public static ushort Gallade => (ushort)(Start + 45);

                    #endregion GEN IV

                    #region GEN V

                    private static ushort Audino => (ushort)(Start + 46);

                    #endregion GEN V

                    #region GEN VI

                    public static ushort Diancie => (ushort)(Start + 47);

                    #endregion GEN VI
                }

                public static class PrimalForms
                {
                    public static ushort Start => (ushort)(MegaEvolutions.End + 1);
                    public static ushort End => Groudon;

                    public static ushort Groudon => (ushort)(Start + 1);
                }

                public static class AlolanForms
                {
                    public static ushort Start => (ushort)(PrimalForms.End + 1);
                    public static ushort End => Marowak;
                    public static ushort Rattata => Start;
                    public static ushort Raticate => (ushort)(Start + 1);
                    public static ushort Raichu => (ushort)(Start + 2);
                    public static ushort Sandshrew => (ushort)(Start + 3);
                    public static ushort Sandslash => (ushort)(Start + 4);
                    public static ushort Vulpix => (ushort)(Start + 5);
                    public static ushort Ninetales => (ushort)(Start + 6);
                    public static ushort Diglet => (ushort)(Start + 7);
                    public static ushort Dugtrio => (ushort)(Start + 8);
                    public static ushort Meowth => (ushort)(Start + 9);
                    public static ushort Persian => (ushort)(Start + 10);
                    public static ushort Geodude => (ushort)(Start + 11);
                    public static ushort Graveler => (ushort)(Start + 12);
                    public static ushort Golem => (ushort)(Start + 13);
                    public static ushort Grimer => (ushort)(Start + 14);
                    public static ushort Muk => (ushort)(Start + 15);
                    public static ushort Exeggutor => (ushort)(Start + 16);
                    public static ushort Marowak => (ushort)(Start + 17);
                }

                public static class GalarianForms
                {
                    public static ushort Start => (ushort)(AlolanForms.End + 1);
                    public static ushort End => Stunfisk;
                    public static ushort Meowth => Start;
                    public static ushort Ponyta => (ushort)(Start + 1);
                    public static ushort Rapidah => (ushort)(Start + 2);

                    public static ushort Stunfisk => (ushort)(Start + 18);
                }

                public static class CosmeticForms
                {
                    public static ushort Start => (ushort)(GalarianForms.End + 1);
                    public static ushort End => Enamorus;

                    public static List<ushort> PikachuForms()
                    {
                        var forms = new List<ushort>();
                        for (int i = 0; i < FormNames.HgEngineForms.PikachuCosmetic.Count; i++)
                        {
                            forms.Add((ushort)(i + Start));
                        }
                        return forms;
                    }

                    public static ushort Enamorus => (ushort)(Start + 150);
                }

                public static class HisuianForms
                {
                    public static ushort Start => (ushort)(CosmeticForms.End + 1);
                    public static ushort End => Decidueye;
                    public static ushort Growlithe => Start;
                    public static ushort Decidueye => (ushort)(Start + 15);
                }

                public static class GenderDifferenceForms
                {
                    public static ushort Start => (ushort)(HisuianForms.End + 1);
                    public static ushort End => Basculegion;
                    public static ushort Basculegion => (ushort)(Start + 6);
                }

                public static class PaldeanForms
                {
                    public static ushort Start => (ushort)(GenderDifferenceForms.End + 1);
                    public static ushort End => Ursulana;
                    public static ushort Ursulana => (ushort)(Start + 26);
                }
            }

            public static class FormNames
            {
                public const string Default = "-";

                #region VANILLA

                public static List<string> BurmyWormadam => ["Plant", "Sand", "Trash"];
                public static List<string> Castform => [Default, "Rainy", "Sunny", "Snowy",];

                public static List<string> Deoxys => [Default, "Attack", "Defense", "Speed",];

                public static List<string> GiratinaForms => [Default, "Origin"];

                public static List<string> Pichu => [Default, "Spiky-Ear"];

                public static List<string> Rotom => [Default, "Heat", "Wash", "Frost", "Fan", "Mow"];
                public static List<string> ShayminForms => ["Land", "Sky"];
                public static List<string> ShellosGastrodon => ["West", "East"];

                public static List<string> Unown => Enumerable.Range('A', 26).Select(c => ((char)c).ToString()).Concat(["!", "?"]).ToList();

                #endregion VANILLA

                #region HG Engine

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
                    public static List<string> Eevee => [Default, LetsGo];
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
                    public static List<string> Growlithe => [Default, Hisuan];
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
                    public static List<string> Keldeo => [Default, KeldeoCosmetic];
                    public static List<string> Tornadus => [Default, Therian];
                    public static List<string> Thundurus => [Default, Therian];
                    public static List<string> Landorus => [Default, Therian];
                    public static List<string> Basulin => [Default, .. BasculingCosmetic];
                    public static List<string> Deerling => [Default, .. DeerlingSawsbuckCosmetic];
                    public static List<string> Genesect => [Default, .. GenesectCosmetic];
                    public static List<string> Sawsbuck => [Default, .. DeerlingSawsbuckCosmetic];
                    public static List<string> Kyurim => [Default, .. KyurimCosmetic];

                    #endregion GEN V

                    #region GEN VI

                    public static List<string> Diancie => [Default, .. MegaEvolutionForm()];

                    #endregion GEN VI

                    #endregion HG Engine Forms

                    #region Mega Evolutions

                    public const int MegaFormSpeciesStart = 1076;
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

                    public const ushort CosmeticSpeciesStart = 0;

                    public static List<string> PikachuCosmetic =>
                        ["Cosplay", "Rock Star", "Belle", "Pop Star", "PHD", "Libre", "Original Cap", "Hoen Cap", "Sinnoh Cap", "Unova Cap", "Kalos Cap", "Alola Cap",
                    "Partner Cap", "World Cap", LetsGo];

                    public const string LetsGo = "Let's Go";

                    public static List<string> BasculingCosmetic =>
                        ["Blue Stripe", "White Stripe"];

                    public static List<string> DeerlingSawsbuckCosmetic =>
                         ["Summer", "Autumn", "Winter"];

                    public const string Therian = "Therian";

                    public static List<string> KyurimCosmetic =>
                     ["White", "Black"];

                    public const string KeldeoCosmetic = "Resolute";

                    public static List<string> GenesectCosmetic =>
                               ["Douse Drive", "Shock Drive", "Burn Drive", "Chill Drive", "Battle Bond"];

                    public static List<string> VivillianCosmetic =>
                        ["Polar", "Tundra", "Continental", "Garden", "Elegant", "Meadow", "Modern", "Marine", "Archipelago", "High Plains", "Sandstorm", "River", "Monsoon", "Savanna", "Sun", "Ocean", "Jungle", "Fancy", "Poke Ball"];

                    public static List<string> FlabebeCosmetic =>
                        ["Yellow FLower", "Orange Flower", "Blue Flower", "Blue Flower", "White Flower"];

                    public static List<string> FloetteCosmetic =>
                 ["Yellow FLower", "Orange Flower", "Blue Flower", "Blue Flower", "White Flower"];

                    #endregion Cosmetic
                }

                #region Hisuan
                public const string Hisuan = "Hisuan";

                #endregion Hisuan


                #endregion HG Engine
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