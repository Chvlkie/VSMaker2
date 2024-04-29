using System.Runtime.CompilerServices;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public class RomFile
    {
        #region RomInfo

        public const uint SynthOverlayLoadAddress = 0x023C8000;
        public static string Arm9Path { get; set; }
        public int AbilityNamesTextNumber { get; set; }
        public uint Arm9SpwanOffset { get; set; }
        public int AttackNamesTextNumber { get; set; }
        public int BattleMessageTextNumber { get; set; }
        public int ClassDescriptionMessageNumber { get; set; }
        public uint ClassGenderOffsetToRamAddress { get; set; }
        public int ClassNamesTextNumber { get; set; }
        public uint EffectsComboTableOffsetToRamAddress { get; set; }
        public uint EffectsComboTableOffsetToSizeLimiter { get; set; }
        public uint EncounterMusicTableOffsetToRamAddress { get; set; }
        public byte EuropeByte { get; set; }
        public string GameCode { get; set; }

        public int ItemNamesTextNumber { get; set; }
        public int MoveNameTextNumber { get; set; }
        public string MoveTablePath { get; set; }
        public string OverlayPath { get; set; }
        public string OverlayTablePath { get; set; }
        public uint PokemonIconPalTableAddress { get; set; }
        public int PokemonNamesTextNumber { get; set; }
        public uint PrizeMoneyTableOffset { get; set; }
        public int PrizeMoneyTableOverlayNumber { get; set; }
        public int PrizeMoneyTableSize { get; set; }
        public int TrainerNamesTextNumber { get; set; }
        public string TrainerTablePath { get; set; }
        public int TypeNamesTextNumber { get; set; }
        public int VanillaTotalTrainers => GameFamily switch
        {
            GameFamily.DiamondPearl => Trainer.Constants.DefaultTotalTrainers.DiamondPearlTotalTrainers,
            GameFamily.Platinum => Trainer.Constants.DefaultTotalTrainers.PlatinumTotalTrainers,
            GameFamily.HeartGoldSoulSilver => Trainer.Constants.DefaultTotalTrainers.HeartGoldSoulSilverTotalTrainers,
            _ => 0,
        };

        public uint VsPokemonEntryTableOffsetToRamAddress { get; set; }
        public uint VsPokemonEntryTableOffsetToSizeLimiter { get; set; }
        public uint VsTrainerEntryTableOffsetToRamAddress { get; set; }
        public uint VsTrainerEntryTableOffsetToSizeLimiter { get; set; }
        public string WorkingDirectory { get; set; }
        #endregion RomInfo

        public RomFile()
        {
        }

        public RomFile(string romId, string romName, string workingDirectory, byte europeByte, bool suffix = true)
        {
            ExtractedFolderSuffix = suffix ? Common.VsMakerContentsFolder : "";
            WorkingDirectory = workingDirectory;
            Arm9Path = WorkingDirectory + Common.Arm9FilePath;
            OverlayTablePath = WorkingDirectory + Common.Y9FilePath;
            OverlayPath = WorkingDirectory + Common.OverlayFilePath;

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

        public string ExtractedFolderSuffix { get; set; }
        public string FileName { get; set; }

        public GameFamily GameFamily { get; set; }
        public GameLanguage GameLanguage { get; set; }
        public GameVersion GameVersion { get; set; }
    }
}