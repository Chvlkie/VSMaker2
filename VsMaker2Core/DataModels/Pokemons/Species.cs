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

        public static bool HasMoreThanOneForm(int pokemonId)
        {
            if (pokemonId > 493)
            {
                pokemonId -= 50;
            }

            return pokemonId
            switch
            {
                #region GEN I

                Pokedex.Venusaur => RomFile.IsHgEngine,
                Pokedex.Charizard => RomFile.IsHgEngine,
                Pokedex.Blastoise => RomFile.IsHgEngine,
                Pokedex.Beedrill => RomFile.IsHgEngine,
                Pokedex.Pidgeot => RomFile.IsHgEngine,
                Pokedex.Rattata => RomFile.IsHgEngine,
                Pokedex.Raticate => RomFile.IsHgEngine,
                Pokedex.Pikachu => RomFile.IsHgEngine,
                Pokedex.Raichu => RomFile.IsHgEngine,
                Pokedex.Sandshrew => RomFile.IsHgEngine,
                Pokedex.Sandslash => RomFile.IsHgEngine,
                Pokedex.Vulpix => RomFile.IsHgEngine,
                Pokedex.Ninetales => RomFile.IsHgEngine,
                Pokedex.Diglett => RomFile.IsHgEngine,
                Pokedex.Dugtrio => RomFile.IsHgEngine,
                Pokedex.Meowth => RomFile.IsHgEngine,
                Pokedex.Persian => RomFile.IsHgEngine,
                Pokedex.Growlithe => RomFile.IsHgEngine,
                Pokedex.Arcanine => RomFile.IsHgEngine,
                Pokedex.Alakazam => RomFile.IsHgEngine,
                Pokedex.Geodude => RomFile.IsHgEngine,
                Pokedex.Graveler => RomFile.IsHgEngine,
                Pokedex.Golem => RomFile.IsHgEngine,
                Pokedex.Ponyta => RomFile.IsHgEngine,
                Pokedex.Rapidash => RomFile.IsHgEngine,
                Pokedex.Slowpoke => RomFile.IsHgEngine,
                Pokedex.Slowbro => RomFile.IsHgEngine,
                Pokedex.Farfetchd => RomFile.IsHgEngine,
                Pokedex.Grimer => RomFile.IsHgEngine,
                Pokedex.Muk => RomFile.IsHgEngine,
                Pokedex.Gengar => RomFile.IsHgEngine,
                Pokedex.Voltorb => RomFile.IsHgEngine,
                Pokedex.Electrode => RomFile.IsHgEngine,
                Pokedex.Exeggutor => RomFile.IsHgEngine,
                Pokedex.Marowak => RomFile.IsHgEngine,
                Pokedex.Weezing => RomFile.IsHgEngine,
                Pokedex.Kangaskhan => RomFile.IsHgEngine,
                Pokedex.MrMime => RomFile.IsHgEngine,
                Pokedex.Pinsir => RomFile.IsHgEngine,
                Pokedex.Tauros => RomFile.IsHgEngine,
                Pokedex.Gyarados => RomFile.IsHgEngine,
                Pokedex.Eevee => RomFile.IsHgEngine,
                Pokedex.Aerodactyl => RomFile.IsHgEngine,
                Pokedex.Articuno => RomFile.IsHgEngine,
                Pokedex.Zapdos => RomFile.IsHgEngine,
                Pokedex.Moltres => RomFile.IsHgEngine,
                Pokedex.Mewtwo => RomFile.IsHgEngine,

                #endregion GEN I

                #region GEN II

                Pokedex.Typhlosion => RomFile.IsHgEngine,
                Pokedex.Pichu => true,
                Pokedex.Wooper => RomFile.IsHgEngine,
                Pokedex.Slowking => RomFile.IsHgEngine,
                Pokedex.Ampharos => RomFile.IsHgEngine,
                Pokedex.Unown => true,
                Pokedex.Steelix => RomFile.IsHgEngine,
                Pokedex.Qwilfish => RomFile.IsHgEngine,
                Pokedex.Scizor => RomFile.IsHgEngine,
                Pokedex.Heracross => RomFile.IsHgEngine,
                Pokedex.Sneasel => RomFile.IsHgEngine,
                Pokedex.Corsola => RomFile.IsHgEngine,
                Pokedex.Houndoom => RomFile.IsHgEngine,
                Pokedex.Tyranitar => RomFile.IsHgEngine,

                #endregion GEN II

                #region GEN III

                Pokedex.Sceptile => RomFile.IsHgEngine,
                Pokedex.Blaziken => RomFile.IsHgEngine,
                Pokedex.Swampert => RomFile.IsHgEngine,
                Pokedex.Zigzagoon => RomFile.IsHgEngine,
                Pokedex.Linoone => RomFile.IsHgEngine,
                Pokedex.Gardevoir => RomFile.IsHgEngine,
                Pokedex.Sableye => RomFile.IsHgEngine,
                Pokedex.Mawile => RomFile.IsHgEngine,
                Pokedex.Aggron => RomFile.IsHgEngine,
                Pokedex.Medicham => RomFile.IsHgEngine,
                Pokedex.Manectric => RomFile.IsHgEngine,
                Pokedex.Sharpedo => RomFile.IsHgEngine,
                Pokedex.Camerupt => RomFile.IsHgEngine,
                Pokedex.Altaria => RomFile.IsHgEngine,
                Pokedex.Castform => true,
                Pokedex.Banette => RomFile.IsHgEngine,
                Pokedex.Absol => RomFile.IsHgEngine,
                Pokedex.Glalie => RomFile.IsHgEngine,
                Pokedex.Salamence => RomFile.IsHgEngine,
                Pokedex.Metagross => RomFile.IsHgEngine,
                Pokedex.Latias => RomFile.IsHgEngine,
                Pokedex.Latios => RomFile.IsHgEngine,
                Pokedex.Kyogre => RomFile.IsHgEngine,
                Pokedex.Groudon => RomFile.IsHgEngine,
                Pokedex.Rayquaza => RomFile.IsHgEngine,
                Pokedex.Deoxys => true,

                #endregion GEN III

                #region GEN IV

                Pokedex.Burmy => true,
                Pokedex.Wormadam => true,
                Pokedex.Shellos => true,
                Pokedex.Gastrodon => true,
                Pokedex.Lopunny => RomFile.IsHgEngine,
                Pokedex.Garchomp => RomFile.IsHgEngine,
                Pokedex.Lucario => RomFile.IsHgEngine,
                Pokedex.Abomasnow => RomFile.IsHgEngine,
                Pokedex.Gallade => RomFile.IsHgEngine,
                Pokedex.Rotom => true,
                Pokedex.Dialga => RomFile.IsHgEngine,
                Pokedex.Palkia => RomFile.IsHgEngine,
                Pokedex.Giratina => true,
                Pokedex.Shaymin => true,

                #endregion GEN IV

                #region GEN V

                Pokedex.Samurott => RomFile.IsHgEngine,
                Pokedex.Unfezant => RomFile.IsHgEngine,
                Pokedex.Audino => RomFile.IsHgEngine,
                Pokedex.Lilligant => RomFile.IsHgEngine,
                Pokedex.Darumaka => RomFile.IsHgEngine,
                Pokedex.Darmanitan => RomFile.IsHgEngine,
                Pokedex.Yamask => RomFile.IsHgEngine,
                Pokedex.Basculin => RomFile.IsHgEngine,
                Pokedex.Zorua => RomFile.IsHgEngine,
                Pokedex.Zoroark => RomFile.IsHgEngine,
                Pokedex.Deerling => RomFile.IsHgEngine,
                Pokedex.Sawsbuck => RomFile.IsHgEngine,
                Pokedex.Frillish => RomFile.IsHgEngine,
                Pokedex.Jellicent => RomFile.IsHgEngine,
                Pokedex.Stunfisk => RomFile.IsHgEngine,
                Pokedex.Braviary => RomFile.IsHgEngine,
                Pokedex.Tornadus => RomFile.IsHgEngine,
                Pokedex.Thundurus => RomFile.IsHgEngine,
                Pokedex.Landorus => RomFile.IsHgEngine,
                Pokedex.Kyurem => RomFile.IsHgEngine,
                Pokedex.Keldeo => RomFile.IsHgEngine,
                Pokedex.Meloetta => RomFile.IsHgEngine,
                Pokedex.Genesect => RomFile.IsHgEngine,

                #endregion GEN V

                #region GEN VI

                Pokedex.Greninja => RomFile.IsHgEngine,
                Pokedex.Vivillon => RomFile.IsHgEngine,
                Pokedex.Flabébé => RomFile.IsHgEngine,
                Pokedex.Floette => RomFile.IsHgEngine,
                Pokedex.Florges => RomFile.IsHgEngine,
                Pokedex.Furfrou => RomFile.IsHgEngine,
                Pokedex.Pyroar => RomFile.IsHgEngine,
                Pokedex.Aegislash => RomFile.IsHgEngine,
                Pokedex.Meowstic => RomFile.IsHgEngine,
                Pokedex.Sliggoo => RomFile.IsHgEngine,
                Pokedex.Goodra => RomFile.IsHgEngine,
                Pokedex.Pumpkaboo => RomFile.IsHgEngine,
                Pokedex.Gourgeist => RomFile.IsHgEngine,
                Pokedex.Avalugg => RomFile.IsHgEngine,
                Pokedex.Xerneas => RomFile.IsHgEngine,
                Pokedex.Zygarde => RomFile.IsHgEngine,
                Pokedex.Diancie => RomFile.IsHgEngine,
                Pokedex.Hoopa => RomFile.IsHgEngine,

                #endregion GEN VI

                #region GEN VII

                Pokedex.Decidueye => RomFile.IsHgEngine,
                Pokedex.Oricorio => RomFile.IsHgEngine,
                Pokedex.Rockruff => RomFile.IsHgEngine,
                Pokedex.Lycanroc => RomFile.IsHgEngine,
                Pokedex.Wishiwashi => RomFile.IsHgEngine,
                Pokedex.Minior => RomFile.IsHgEngine,
                Pokedex.Necrozma => RomFile.IsHgEngine,
                Pokedex.Magearna => RomFile.IsHgEngine,

                #endregion GEN VII

                #region GEN VIII

                Pokedex.Cramorant => RomFile.IsHgEngine,
                Pokedex.Toxtricity => RomFile.IsHgEngine,
                Pokedex.Sinistea => RomFile.IsHgEngine,
                Pokedex.Polteageist => RomFile.IsHgEngine,
                Pokedex.Alcremie => RomFile.IsHgEngine,
                Pokedex.Eiscue => RomFile.IsHgEngine,
                Pokedex.Indeedee => RomFile.IsHgEngine,
                Pokedex.Morpeko => RomFile.IsHgEngine,
                Pokedex.Zacian => RomFile.IsHgEngine,
                Pokedex.Zamazenta => RomFile.IsHgEngine,
                Pokedex.Eternatus => RomFile.IsHgEngine,
                Pokedex.Urshifu => RomFile.IsHgEngine,
                Pokedex.Zarude => RomFile.IsHgEngine,
                Pokedex.Calyrex => RomFile.IsHgEngine,
                Pokedex.Ursaluna => RomFile.IsHgEngine,
                Pokedex.Basculegion => RomFile.IsHgEngine,
                Pokedex.Enamorus => RomFile.IsHgEngine,

                #endregion GEN VIII

                #region GEN IX

                Pokedex.Oinkologne => RomFile.IsHgEngine,
                Pokedex.Maushold => RomFile.IsHgEngine,
                Pokedex.Squawkabilly => RomFile.IsHgEngine,
                Pokedex.Palafin => RomFile.IsHgEngine,
                Pokedex.Revavroom => RomFile.IsHgEngine,
                Pokedex.Tatsugiri => RomFile.IsHgEngine,
                Pokedex.Dudunsparce => RomFile.IsHgEngine,
                Pokedex.Gimmighoul => RomFile.IsHgEngine,
                Pokedex.Poltchageist => RomFile.IsHgEngine,
                Pokedex.Sinistcha => RomFile.IsHgEngine,
                Pokedex.Ogerpon => RomFile.IsHgEngine,
                Pokedex.Terapagos => RomFile.IsHgEngine,

                #endregion GEN IX

                _ => false,
            };
        }

        public override bool Equals(object? obj) => Equals(obj as Species);

        public bool Equals(Species? other) => other is not null &&
                   SpeciesId == other.SpeciesId &&
                   GenderRatio == other.GenderRatio &&
                   Ability1 == other.Ability1 &&
                   Ability2 == other.Ability2 &&
                   HasMoreThanOneGender == other.HasMoreThanOneGender &&
                   HasMoreThanOneAbility == other.HasMoreThanOneAbility;

        public override int GetHashCode() => HashCode.Combine(SpeciesId, GenderRatio, Ability1, Ability2, HasMoreThanOneGender, HasMoreThanOneAbility);

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