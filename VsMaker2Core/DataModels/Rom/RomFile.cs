using VsMaker2Core.Database;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class RomFile
    {
        #region RomInfo

        public byte EuropeByte { get; set; }
        public string ExtractedFolderSuffix { get; set; }
        public string FileName { get; set; }
        public static string GameCode { get; set; }

        public bool IsHeartGoldSoulSilver => GameFamily == GameFamily.HeartGoldSoulSilver || GameFamily == GameFamily.HgEngine;

        public static GameFamily GameFamily => GameVersion switch
        {
            GameVersion.Diamond or GameVersion.Pearl => GameFamily.DiamondPearl,
            GameVersion.Platinum => GameFamily.Platinum,
            GameVersion.HeartGold or GameVersion.SoulSilver => GameFamily.HeartGoldSoulSilver,
            GameVersion.HgEngine => GameFamily.HgEngine,
            _ => GameFamily.Unknown,
        };

        public static Dictionary<ushort, byte[]> BuildCommandParametersDatabase(GameFamily gameFam)
        {
            Dictionary<ushort, byte[]> commonDictionaryParams;
            Dictionary<ushort, byte[]> specificDictionaryParams;

            switch (gameFam)
            {
                case GameFamily.DiamondPearl:
                    commonDictionaryParams = ScriptDatabase.DPPtScrCmdParameters;
                    specificDictionaryParams = ScriptDatabase.DPScrCmdParameters;
                    break;

                case GameFamily.Platinum:
                    commonDictionaryParams = ScriptDatabase.DPPtScrCmdParameters;
                    specificDictionaryParams = ScriptDatabase.PlatScrCmdParameters;
                    break;

                default:
                    commonDictionaryParams = ScriptDatabase.HGSSScrCmdParameters;
#if true
                    specificDictionaryParams = [];
#else
                        specificDictionaryParams = ScriptDatabase.CustomScrCmdParameters;
#endif
                    break;
            }
            return commonDictionaryParams.Concat(specificDictionaryParams).ToLookup(x => x.Key, x => x.Value).ToDictionary(x => x.Key, g => g.First());
        }

        public static GameLanguage GameLanguage => GameCode switch
        {
            "ADAE" or "APAE" or "CPUE" or "IPKE" or "IPGE" => GameLanguage.English,
            "ADAS" or "APAS" or "CPUS" or "IPKS" or "IPGS" or "LATA" => GameLanguage.Spanish,
            "ADAI" or "APAI" or "CPUI" or "IPKI" or "IPGI" => GameLanguage.Italian,
            "ADAF" or "APAF" or "CPUF" or "IPKF" or "IPGF" => GameLanguage.French,
            "ADAD" or "APAD" or "CPUD" or "IPKD" or "IPGD" => GameLanguage.German,
            _ => GameLanguage.Japanese,
        };

        public static Dictionary<ushort, byte[]> ScriptCommandParametersDict => BuildCommandParametersDatabase(GameFamily);
        public static GameVersion GameVersion { get; set; }
        public int TotalNumberOfTrainerClasses { get; set; }
        public int TotalNumberOfTrainers { get; set; }
        public int InitialMoneyOverlayNumber => SetInitialMoneyOverlay().overlayNumber;
        public int InitialMoneyOverlayOffset => SetInitialMoneyOverlay().offset;
        public List<TrainerData> TrainersData { get; set; }
        public List<TrainerPartyData> TrainersPartyData { get; set; }
        public List<ClassGenderData> ClassGenderData { get; set; }
        public List<PrizeMoneyData> PrizeMoneyData { get; set; }
        public List<EyeContactMusicData> EyeContactMusicData { get; set; }
        public List<BattleMessageTableData> BattleMessageTableData { get; set; }

        public List<BattleMessageOffsetData> BattleMessageOffsetData { get; set; }

        public List<ScriptFileData> ScriptFileData { get; set; }
        public List<ScriptFileData> EventFileData { get; set; }

        public int VanillaTotalTrainers => GameFamily switch
        {
            GameFamily.DiamondPearl => Trainer.Constants.DefaultTotalTrainers.DiamondPearlTotalTrainers,
            GameFamily.Platinum => Trainer.Constants.DefaultTotalTrainers.PlatinumTotalTrainers,
            GameFamily.HeartGoldSoulSilver => Trainer.Constants.DefaultTotalTrainers.HeartGoldSoulSilverTotalTrainers,
            _ => 0,
        };

        public static string WorkingDirectory { get; set; }

        #endregion RomInfo

        public static bool TrainerNameExpansion => TrainerNameMaxByte > 8;
        public static int TrainerNameMaxLength => TrainerNameMaxByte + ((TrainerNameMaxByte - 4) / 2);

        public static int TrainerNameMaxByte { get; set; }

        public RomFile()
        {
        }

        public RomFile(string romId, string romName, string workingDirectory, byte europeByte, bool suffix = true)
        {
            ExtractedFolderSuffix = suffix ? Common.VsMakerContentsFolder : "";
            WorkingDirectory = workingDirectory;

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