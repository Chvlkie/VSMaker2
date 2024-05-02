using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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