﻿using VsMaker2Core.Database;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public static partial class RomFile
    {
        #region RomInfo

        public static byte EuropeByte { get; set; }
        public static string ExtractedFolderSuffix { get; set; }
        public static string FileName { get; set; }
        public static string GameCode { get; set; }

        public static bool IsHgEngine => GameVersion == GameVersion.HgEngine;
        public static bool IsHeartGoldSoulSilver => GameFamily == GameFamily.HeartGoldSoulSilver;
        public static bool IsNotDiamondPearl => GameFamily != GameFamily.DiamondPearl;

        public static GameFamily GameFamily => GameVersion switch
        {
            GameVersion.Diamond or GameVersion.Pearl => GameFamily.DiamondPearl,
            GameVersion.Platinum => GameFamily.Platinum,
            GameVersion.HeartGold or GameVersion.SoulSilver or GameVersion.HgEngine => GameFamily.HeartGoldSoulSilver,
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
        public static int TotalNumberOfTrainerClasses { get; set; }
        public static int TotalNumberOfTrainers => RomFile.TrainerNames.Count;

        public static List<string> TrainerNames { get; set; }
        public static List<TrainerData> TrainersData { get; set; }
        public static List<TrainerPartyData> TrainersPartyData { get; set; }
        public static List<ClassGenderData> ClassGenderData { get; set; }
        public static List<PrizeMoneyData> PrizeMoneyData { get; set; }
        public static List<EyeContactMusicData> EyeContactMusicData { get; set; }
        public static List<BattleMessageTableData> BattleMessageTableData { get; set; }
        public static List<BattleMessageOffsetData> BattleMessageOffsetData { get; set; }

        public static List<ScriptFileData> ScriptFileData { get; set; }
        public static List<EventFileData> EventFileData { get; set; }

        public static int VanillaTotalTrainers => GameFamily switch
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
        public static bool Arm9Expanded { get; set; } = false;
        public static bool PrizeMoneyExpanded { get; set; } = false;
        public static bool EyeContactExpanded { get; set; } = false;
        public static bool ClassGenderExpanded { get; set; } = false;
        public static int TrainerNameMaxByte { get; set; }

        public static void Reset()
        {
            // Reset static fields
            GameCode = string.Empty;
            GameVersion = default;
            TotalNumberOfTrainerClasses = 0;
            WorkingDirectory = string.Empty;
            Arm9Expanded = false;
            PrizeMoneyExpanded = false;
            EyeContactExpanded = false;
            ClassGenderExpanded = false;
            TrainerNameMaxByte = 0;
            EuropeByte = 0;
            ExtractedFolderSuffix = string.Empty;
            FileName = string.Empty;
            TrainersData = [];
            TrainersPartyData = [];
            ClassGenderData = [];
            PrizeMoneyData = [];
            EyeContactMusicData = [];
            BattleMessageTableData = [];
            BattleMessageOffsetData = [];
            ScriptFileData = [];
            EventFileData = [];

            if (VsMakerDatabase.RomData.GameDirectories != null)
            {
                VsMakerDatabase.RomData.GameDirectories.Clear();
            }
            else
            {
                VsMakerDatabase.RomData.GameDirectories = [];
            }
        }

        public static void SetupRomFile(string romName, string workingDirectory, bool suffix = true)
        {
            Console.WriteLine("Setup Rom File");
            ExtractedFolderSuffix = suffix ? Common.VsMakerContentsFolder : "";
            WorkingDirectory = workingDirectory;

            try
            {
                GameVersion = VsMakerDatabase.RomData.GameVersions[GameCode];
            }
            catch (KeyNotFoundException)
            {
                GameVersion = GameVersion.Unknown;
                Console.WriteLine("Rom not supported");
            }

            FileName = romName;
            Console.WriteLine("Setup Rom File | Success");
        }

        public static bool CheckForClassGenderExpansion()
        {
            try
            {
                uint currentPointer = BitConverter.ToUInt32(Arm9.ReadBytes(ClassGenderOffsetToRamAddress, 4), 0);
                uint calculatedOffset = currentPointer - SynthOverlayLoadAddress;

                return calculatedOffset == ClassGenderRepointOffset;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for class gender expansion: {ex.Message}");
                throw;
            }
        }

        public static bool CheckForEyeContactExpansion()
        {
            try
            {
                uint currentPointer = BitConverter.ToUInt32(Arm9.ReadBytes(EyeContactTablePointerOffset, 4), 0);
                uint calculatedOffset = currentPointer - SynthOverlayLoadAddress;

                return calculatedOffset == EyeContactRepointOffset;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for eye contact expansion: {ex.Message}");
                throw;
            }
        }

        public static async Task<bool> CheckForPrizeMoneyExpansionAsync()
        {
            try
            {
                string overlayFilePath = Overlay.OverlayFilePath(PrizeMoneyTableOverlayNumber);

                if (IsHeartGoldSoulSilver)
                {
                    bool isCompressed = await Overlay.CheckOverlayIsCompressedAsync(PrizeMoneyTableOverlayNumber);
                    if (isCompressed)
                    {
                        await Overlay.DecompressOverlayAsync(PrizeMoneyTableOverlayNumber);
                        Overlay.SetOverlayCompressionInTable(PrizeMoneyTableOverlayNumber, 0);
                    }
                }

                uint originalPointer = GameFamily switch
                {
                    GameFamily.DiamondPearl => 0x0225FF20,
                    GameFamily.Platinum => 0x02270B20,
                    GameFamily.HeartGoldSoulSilver => 0x0226C4C4,
                    _ => throw new ArgumentException("Unsupported game family")
                };

                uint pointerOffset = GameFamily switch
                {
                    GameFamily.DiamondPearl => 0x782C,
                    GameFamily.Platinum => 0x816C,
                    GameFamily.HeartGoldSoulSilver => 0x8380,
                    _ => throw new ArgumentException("Unsupported game family")
                };

                uint? additionalPointerOffsetHGSS = IsHeartGoldSoulSilver ? (uint?)0x8384 : null;

                using FileStream overlayStream = new(overlayFilePath, FileMode.Open, FileAccess.Read);
                using BinaryReader reader = new(overlayStream);
                overlayStream.Position = pointerOffset;
                uint currentPointer = reader.ReadUInt32();

                if (currentPointer != originalPointer)
                {
                    return true;
                }

                if (additionalPointerOffsetHGSS.HasValue)
                {
                    overlayStream.Position = additionalPointerOffsetHGSS.Value;
                    uint additionalPointer = reader.ReadUInt32();

                    if (additionalPointer != originalPointer + 2)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking for prize money expansion: {ex.Message}");
                throw;
            }
        }
    }
}