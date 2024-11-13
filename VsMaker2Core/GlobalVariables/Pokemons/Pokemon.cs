namespace VsMaker2Core.DataModels
{
    public partial class Pokemon
    {
        public static class Pokedex
        {
            public const ushort PichuId = 172;
            public const ushort UnownId = 201;
            public const ushort CastformId = 351;
            public const ushort DeoxysId = 386;
            public const ushort BurmyId = 412;
            public const ushort WormadamId = 413;
            public const ushort CherrimId = 421;
            public const ushort ShellosId = 422;
            public const ushort GastrodonId = 423;
            public const ushort RotomId = 479;
            public const ushort GiratinaId = 487;
            public const ushort ShayminId = 492;
        }

        public static int GetSpecialIcon(ushort pokemonId, ushort formId) => pokemonId switch
        {
            Pokedex.UnownId => AltIcons.UnownAlts(formId),
            Pokedex.BurmyId => formId > 0 ? AltIcons.BurmyAlts(formId) : pokemonId,
            Pokedex.WormadamId => formId > 0 ? AltIcons.WormadamAlts(formId) : pokemonId,
            Pokedex.ShellosId => formId > 0 ? AltIcons.ShellosAlt : pokemonId,
            Pokedex.GastrodonId => formId > 0 ? AltIcons.GastrodonAlt : pokemonId,
            Pokedex.GiratinaId => formId > 0 ? AltIcons.GiratinaOrigin : pokemonId,
            Pokedex.ShayminId => formId > 0 ? AltIcons.ShayminSky : pokemonId,
            Pokedex.CastformId => formId > 0 ? AltIcons.CastformAlts(formId) : pokemonId,
            Pokedex.RotomId => formId > 0 ? AltIcons.RotomAlts(formId) : pokemonId,
            _ => pokemonId,
        };

        public static class AltIcons
        {
            public static int UnownAlts(ushort formId) => 499 + formId;

            public static int BurmyAlts(ushort formId) => 526 + formId;

            public static int WormadamAlts(ushort formId) => 528 + formId;

            public const int ShellosAlt = 531;
            public const int GastrodonAlt = 532;
            public const int GiratinaOrigin = 533;
            public const int ShayminSky = 534;

            public static int RotomAlts(ushort formId) => 534 + formId;

            public static int CastformAlts(ushort formId) => 539 + formId;

            public const int CherrimAlt = 543;
        }

        public static class Constants
        {
            public static int PokemonNumberBitSize => RomFile.GameVersion == Enums.GameVersion.HgEngine ? 11 : 10;
            public static int PokemonNumberBitMask => (1 << PokemonNumberBitSize) - 1;

            public const int PokemonFormBitSize = 6;
            public static int PokemonFormBitMask => ((1 << PokemonFormBitSize) - 1) << PokemonNumberBitSize;
        }
    }
}