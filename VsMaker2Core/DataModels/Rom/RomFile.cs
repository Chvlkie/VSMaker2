using VsMaker2Core.Database;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
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

        public static bool IsHeartGoldSoulSilver => GameFamily == GameFamily.HeartGoldSoulSilver || GameFamily == GameFamily.HgEngine;

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
        public static int TotalNumberOfTrainerClasses { get; set; }
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
        public static bool Arm9Expanded { get; set; } = false;
        public static bool PrizeMoneyExpanded { get; set; } = false;
        public static bool EyeContactExpanded { get; set; } = false;
        public static bool ClassGenderExpanded { get; set; } = false;
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

        public static bool CheckForClassGenderExpansion()
        {
            try
            {
                // Read the 4-byte pointer from arm9.bin at ClassGenderOffsetToRamAddress
                uint currentPointer = BitConverter.ToUInt32(Arm9.ReadBytes(ClassGenderOffsetToRamAddress, 4), 0);

                // Subtract SynthOverlayLoadAddress to get the offset within the synthetic overlay
                uint calculatedOffset = currentPointer - SynthOverlayLoadAddress;

                // Return whether the calculated offset matches ClassGenderRepointOffset
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
                // Read the 4-byte pointer from arm9.bin at ClassGenderOffsetToRamAddress
                uint currentPointer = BitConverter.ToUInt32(Arm9.ReadBytes(EyeContactMusicTableOffsetToRam, 4), 0);

                // Subtract SynthOverlayLoadAddress to get the offset within the synthetic overlay
                uint calculatedOffset = currentPointer - SynthOverlayLoadAddress;

                // Return whether the calculated offset matches ClassGenderRepointOffset
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

                // Check if the overlay needs to be decompressed (only for HGSS)
                if (IsHeartGoldSoulSilver)
                {
                    bool isCompressed = await Overlay.CheckOverlayIsCompressedAsync(PrizeMoneyTableOverlayNumber);
                    if (isCompressed)
                    {
                        await Overlay.DecompressOverlayAsync(PrizeMoneyTableOverlayNumber);
                        Overlay.SetOverlayCompressionInTable(PrizeMoneyTableOverlayNumber, 0);
                    }
                }

                // Pointers to check based on game family
                uint originalPointer = GameFamily switch
                {
                    GameFamily.DiamondPearl => 0x0225FF20, // Original pointer for Diamond/Pearl
                    GameFamily.Platinum => 0x02270B20, // Original pointer for Platinum
                    GameFamily.HeartGoldSoulSilver => 0x0226C4C4, // Original pointer for HGSS
                    _ => throw new ArgumentException("Unsupported game family")
                };

                // Offset locations to check based on game family
                uint pointerOffset = GameFamily switch
                {
                    GameFamily.DiamondPearl => 0x782C,
                    GameFamily.Platinum => 0x816C,
                    GameFamily.HeartGoldSoulSilver => 0x8380,
                    _ => throw new ArgumentException("Unsupported game family")
                };

                uint? additionalPointerOffsetHGSS = IsHeartGoldSoulSilver ? (uint?)0x8384 : null;

                // Read the current pointers from the overlay file
                using (FileStream overlayStream = new(overlayFilePath, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new(overlayStream))
                {
                    // Check the first pointer
                    overlayStream.Position = pointerOffset;
                    uint currentPointer = reader.ReadUInt32();

                    // Compare the current pointer with the original pointer
                    if (currentPointer != originalPointer)
                    {
                        return true; // Pointer is modified, prize money table has been expanded
                    }

                    // Check the additional pointer for HGSS if it exists
                    if (additionalPointerOffsetHGSS.HasValue)
                    {
                        overlayStream.Position = additionalPointerOffsetHGSS.Value;
                        uint additionalPointer = reader.ReadUInt32();

                        if (additionalPointer != originalPointer + 2)
                        {
                            return true; // Additional pointer is modified in HGSS
                        }
                    }
                }

                // If no changes were found, return false
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