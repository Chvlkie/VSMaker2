namespace VsMaker2Core
{
    public static class Enums
    {
        public enum AbilityOverride
        {
            None = 0,
            Ability1 = 1,
            Ability2 = 2,
        }

        public enum GameFamily : byte
        {
            Unknown,
            DiamondPearl,
            Platinum,
            HeartGoldSoulSilver,
        }

        public enum GameLanguage : byte
        {
            Unknown,
            English,
            Japanese,
            Italian,
            Spanish,
            French,
            German,
            Chinese
        }

        public enum GameVersion : byte
        {
            Unknown,
            Diamond,
            Pearl,
            Platinum,
            HeartGold,
            SoulSilver,
            HgEngine
        }

        public enum GenderOverride
        {
            None = 0,
            IsMale = 1,
            IsFemale = 2,
        }

        public enum LoadType
        {
            UnpackRom = 0,
            LoadRomData = 1,
            UnpackNarcs = 2,
            SetupEditor = 3,
            SaveRom = 4,
            SaveTrainerTextTable = 5,
            RepointTextTable = 6,
            ImportTextTable = 7,
            ExportTextTable = 8,
        }

        public enum NarcDirectory : byte
        {
            Unknown,
            battleStagePokeData,
            battleTowerPokeData,
            battleTowerTrainerData,
            personalPokeData,
            eventFiles,
            scripts,
            synthOverlay,
            textArchives,
            trainerProperties,
            trainerParty,
            trainerGraphics,
            monIcons,
            moveData,
            trainerTextTable,
            trainerTextOffset
        }

        public enum ScriptType
        {
            Script,
            Function,
            Action
        }

        public enum TrainerUsageType
        {
            Unknown = 0,
            Script = 1,
            Function = 2,
            Event = 3
        }

        public enum ViewVsMakerFileType
        {
            ViewOnly,
            Import,
            Export
        }

        public enum EventType : byte
        {
            Spawnable,
            Overworld,
            Warp,
            Trigger
        }

        public enum Status : uint
        {
            None = 0x0,
            Sleep0 = 0x1,
            Sleep1 = 0x2,
            Sleep2 = 0x4,
            Poison = 0x8,
            Burn = 0x10,
            Freeze = 0x20,
            Paralysis = 0x40,
            BadPoison = 0x80,
            BadPoisonCounter0 = 0x100,
            BadPoisonCounter1 = 0x200,
            BadPoisonCounter2 = 0x400,
            BadPoisonCounter3 = 0x800,
            Unused12 = 0x1000,
            Unused13 = 0x2000,
            Unused14 = 0x4000,
            Unused15 = 0x8000,
            Unused16 = 0x10000,
            Unused17 = 0x20000,
            Unused18 = 0x40000,
            Unused19 = 0x80000,
            Unused20 = 0x100000,
            Unused21 = 0x200000,
            Unused22 = 0x400000,
            Unused23 = 0x800000,
            Unused24 = 0x1000000,
            Unused25 = 0x2000000,
            Unused26 = 0x4000000,
            Unused27 = 0x8000000,
            Unused28 = 0x10000000,
            Unused29 = 0x20000000,
            Unused30 = 0x40000000,
            Unused31 = 0x80000000,
            Sleep = 0x7,
            PoisonCount = 0xF00,
            PoisonAll = 0xF88,
            StatusAll = 0xFF,
            UnusedSlots = 0xFFFFF000,
            NotSleep = 0xFFFFFFF8,
            CanSynchronize = 0x58,
            FacadeBoost = 0xD8
        }

        public enum Types : byte
        {
            None = 0xFF,
            Normal = 0x00,
            Fight = 0x01,
            Flying = 0x02,
            Poison = 0x03,
            Ground = 0x04,
            Rock = 0x05,
            Bug = 0x06,
            Ghost = 0x07,
            Steel = 0x08,
            Mystery = 0x09,
            Fairy = 0x09,
            Fire = 0x0A,
            Water = 0x0B,
            Grass = 0x0C,
            Electric = 0x0D,
            Psychic = 0x0E,
            Ice = 0x0F,
            Dragon = 0x10,
            Dark = 0x11,
            Typeless = 0x12,
            Stellar = 0x13,
        }
        public enum Nature : byte {
            Hardy,
            Lonely,
            Brave,
            Adamant,
            Naughty,
            Bold,
            Docile,
            Relaxed,
            Impish,
            Lax,
            Timid,
            Hasty,
            Serious,
            Jolly,
            Naive,
            Modest,
            Mild,
            Quiet,
            Bashful,
            Rash,
            Calm,
            Gentle,
            Sassy,
            Careful,
            Quirky,
        }
    }

}