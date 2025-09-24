using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class Pokemon : IEquatable<Pokemon?>
    {
        public byte DifficultyValue { get; set; }
        public GenderOverride GenderOverride => (GenderOverride)(GenderAbilityOverride & 0xF);
        public AbilityOverride AbilityOverride => (AbilityOverride)((GenderAbilityOverride & 0xF0) >> 4);
        public byte GenderAbilityOverride { get; set; }
        public ushort Level { get; set; }
        public ushort SpeciesId => Species.GetSpecialSpecies(PokemonId, FormId);
        public int IconId => GetSpecialIcon(PokemonId, FormId);
        public ushort PokemonId { get; set; }
        public ushort FormId { get; set; }
        public ushort? HeldItemId { get; set; }
        public ushort[]? Moves { get; set; } = [0, 0, 0, 0];
        public ushort? BallCapsuleId { get; set; }

        // HG Engine Data
        public ushort? Ability_Hge { get; set; }
        public ushort? Ball_Hge { get; set; }
        public byte[]? IvNums_Hge { get; set; } = [0, 0, 0, 0, 0, 0];
        public byte[]? EvNums_Hge { get; set; } = [0, 0, 0, 0, 0, 0];
        public byte? Nature_Hge { get; set; }
        public byte? ShinyLock_Hge { get; set; }
        public uint? Status_Hge { get; set; }
        public ushort? Hp_Hge { get; set; }
        public ushort? Atk_Hge { get; set; }
        public ushort? Def_Hge { get; set; }
        public ushort? Speed_Hge { get; set; }
        public ushort? SpAtk_Hge { get; set; }
        public ushort? SpDef_Hge { get; set; }
        public byte[]? Types_Hge { get; set; } = [0, 0];
        public byte[]? PpCounts_Hge { get; set; } = [0, 0, 0, 0];
        public ushort[]? Nickname_Hge { get; set; } = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

        // Pokemon Additional Flags
        public bool ChooseStatus_Hge { get; set; } = false;
        public bool ChooseHP_Hge { get; set; } = false;
        public bool ChooseATK_Hge { get; set; } = false;
        public bool ChooseDEF_Hge { get; set; } = false;
        public bool ChooseSPEED_Hge { get; set; } = false;
        public bool Choose_SpATK_Hge { get; set; } = false;
        public bool Choose_SpDEF_Hge { get; set; } = false;
        public bool ChoosePP_Hge { get; set; } = false;
        public bool ChooseNickname_HGE { get; set; } = false;

        public bool ChooseStats_HGE =>
            ChooseHP_Hge || ChooseATK_Hge || ChooseDEF_Hge || ChooseSPEED_Hge || Choose_SpATK_Hge || Choose_SpDEF_Hge;
        public Pokemon()
        {
            DifficultyValue = 0;
            GenderAbilityOverride = 0;
            Level = 1;
            PokemonId = 0;
            FormId = 0;
            HeldItemId = 0;
            Moves = null;
            BallCapsuleId = 0;
        }

        public Pokemon(byte difficultyValue, byte genderAbilityOverride, ushort level, ushort pokemonId, ushort formId, ushort? heldItemId, ushort[]? moves, ushort? ballCapsuleId = null)
        {
            DifficultyValue = difficultyValue;
            GenderAbilityOverride = genderAbilityOverride;
            Level = level;
            PokemonId = pokemonId;
            FormId = formId;
            HeldItemId = heldItemId;
            Moves = moves;
            BallCapsuleId = ballCapsuleId;
        }

        public override bool Equals(object? obj) => Equals(obj as Pokemon);

        public bool Equals(Pokemon? other) => other is not null &&
                   DifficultyValue == other.DifficultyValue &&
                   GenderOverride == other.GenderOverride &&
                   AbilityOverride == other.AbilityOverride &&
                   GenderAbilityOverride == other.GenderAbilityOverride &&
                   Level == other.Level &&
                   SpeciesId == other.SpeciesId &&
                   IconId == other.IconId &&
                   PokemonId == other.PokemonId &&
                   FormId == other.FormId &&
                   HeldItemId == other.HeldItemId &&
                   EqualityComparer<ushort[]?>.Default.Equals(Moves, other.Moves) &&
                   BallCapsuleId == other.BallCapsuleId;

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(DifficultyValue);
            hash.Add(GenderOverride);
            hash.Add(AbilityOverride);
            hash.Add(GenderAbilityOverride);
            hash.Add(Level);
            hash.Add(SpeciesId);
            hash.Add(IconId);
            hash.Add(PokemonId);
            hash.Add(FormId);
            hash.Add(HeldItemId);
            hash.Add(Moves);
            hash.Add(BallCapsuleId);
            return hash.ToHashCode();
        }
    }
}