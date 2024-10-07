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
            HgEngine
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
    }
}