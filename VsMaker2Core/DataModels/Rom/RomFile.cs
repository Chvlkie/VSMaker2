using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public class RomFile
    {
        #region RomInfo

        public byte EuropeByte { get; set; }
        public string GameCode { get; set; }

        public string WorkingDirectory { get; set; }
        public static string Arm9Path { get; set; }
        public string OverlayTablePath { get; set; }
        public string OverlayPath { get; set; }

        public const uint SynthOverlayLoadAddress = 0x023C8000;
        public uint Arm9SpwanOffset { get; set; }

        public int PrizeMoneyTableOverlayNumber { get; set; }
        public uint PrizeMoneyTableOffset { get; set; }
        public int PrizeMoneyTableSize { get; set; }

        public uint ClassGenderOffsetToRamAddress { get; set; }
        public uint EncounterMusicTableOffsetToRamAddress { get; set; }

        public uint VsTrainerEntryTableOffsetToRamAddress { get; set; }
        public uint VsTrainerEntryTableOffsetToSizeLimiter { get; set; }

        public uint VsPokemonEntryTableOffsetToRamAddress { get; set; }
        public uint VsPokemonEntryTableOffsetToSizeLimiter { get; set; }

        public uint EffectsComboTableOffsetToRamAddress { get; set; }
        public uint EffectsComboTableOffsetToSizeLimiter { get; set; }

        public string TrainerTablePath { get; set; }
        public string MoveTablePath { get; set; }

        public uint PokemonIconPalTableAddress { get; set; }

        public int AbilityNamesTextNumber { get; set; }
        public int AttackNamesTextNumber { get; set; }
        public int PokemonNamesTextNumber { get; set; }
        public int ItemNamesTextNumber { get; set; }
        public int TypeNamesTextNumber { get; set; }
        public int ClassNamesTextNumber { get; set; }
        public int TrainerNamesTextNumber { get; set; }
        public int BattleMessageTextNumber { get; set; }
        public int MoveInfoTextNumber { get; set; }

        #endregion RomInfo

        public string ExtractedFolderSuffix { get; set; }
        public string FileName { get; set; }

        public GameFamily GameFamily { get; set; }
        public GameVersion GameVersion { get; set; }
        public GameLanguage GameLanguage { get; set; }
        public Dictionary<DirectoryNames, (string packedDirectory, string unpackedDirectory)> Directories { get; set; }

        public RomFile()
        {

        }

        public RomFile(string romId, string romName, byte europeByte, bool suffix = true)
        {
            ExtractedFolderSuffix = suffix ? Common.ExtractedFolderSuffix : "";
            WorkingDirectory = $"{Path.GetDirectoryName(romName)}\\{Path.GetFileNameWithoutExtension(romName)}{ExtractedFolderSuffix}\\";
            Arm9Path = WorkingDirectory + "arm9.bin";
            OverlayTablePath = WorkingDirectory + "y9.bin";
            OverlayPath = WorkingDirectory + "overlay";

            try
            {
                GameVersion = Database.VsMakerDatabase.RomData.GameVersions[romId];
            }
            catch (KeyNotFoundException)
            {
                GameVersion = GameVersion.Unknown;
                Console.WriteLine("Rom not supported");
            }

            GameCode = romId;
            FileName = romName;
            EuropeByte = europeByte;
        }
    }
}