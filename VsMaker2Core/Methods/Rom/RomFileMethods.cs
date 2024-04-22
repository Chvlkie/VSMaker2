using System.Diagnostics;
using System.Text;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class RomFileMethods : IRomFileMethods
    {
        private readonly Dictionary<int, string> ReadTextDictionary = VsMakerDatabase.RomData.TextCharacters.ReadTextDictionary;

        public (bool Success, string ExceptionMessage) ExtractRomContents(string workingDirectory, string fileName)
        {
            Process unpack = new();
            unpack.StartInfo.FileName = Common.NdsToolsFilePath;
            unpack.StartInfo.Arguments = "-x " + '"' + fileName + '"'
                + " -9 " + '"' + workingDirectory + "\\" + Common.Arm9FilePath + '"'
                + " -7 " + '"' + workingDirectory + "\\" + Common.Arm7FilePath + '"'
                + " -y9 " + '"' + workingDirectory + "\\" + Common.Y9FilePath + '"'
                + " -y7 " + '"' + workingDirectory + "\\" + Common.Y7FilePath + '"'
                + " -d " + '"' + workingDirectory + "\\" + Common.DataFilePath + '"'
                + " -y " + '"' + workingDirectory + "\\" + Common.OverlayFilePath + '"'
                + " -t " + '"' + workingDirectory + "\\" + Common.BannerFilePath + '"'
                + " -h " + '"' + workingDirectory + "\\" + Common.HeaderFilePath + '"';
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            try
            {
                unpack.Start();
                unpack.WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, "");
        }

        public List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines)
        {
            int initialKey = 0;
            int stringCount = 0;
            List<string> messages = [];
            bool success = false;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchive].unpackedDirectory}\\{messageArchiveId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open);
            BinaryReader readText = new(fileStream);
            try
            {
                stringCount = readText.ReadUInt16();
                success = true;
            }
            catch (EndOfStreamException)
            {
                readText.Close();
                throw;
            }

            if (success)
            {
                try
                {
                    initialKey = readText.ReadUInt16();
                    if (!discardLines)
                    {
                        int key1 = (initialKey * 0x2FD) & 0xFFFF;
                        int[] currentOffset = new int[stringCount];
                        int[] currentSize = new int[stringCount];

                        // Get Offset and Sizes
                        for (int i = 0; i < stringCount; i++)
                        {
                            int key2 = (key1 * (i + 1) & 0xFFFF);
                            int actualKey = key2 | (key2 << 16);
                            currentOffset[i] = ((int)readText.ReadUInt32()) ^ actualKey;
                            currentSize[i] = ((int)readText.ReadUInt32()) ^ actualKey;
                        }

                        // Build String
                        for (int i = 0; i < stringCount; i++)
                        {
                            bool hasSpecialCharacter = false;
                            bool isCompressed = false;
                            key1 = (0x91BD3 * (i + 1)) & 0xFFFF;
                            readText.BaseStream.Position = currentOffset[i];
                            StringBuilder text = new StringBuilder("");

                            for (int j = 0; j < currentSize[i]; j++)
                            {
                                int textChar = (readText.ReadUInt16()) ^ key1;

                                switch (textChar)
                                {
                                    case 0xE000:
                                        text.Append("\\n");
                                        break;

                                    case 0x25BC:
                                        text.Append("\\r");
                                        break;

                                    case 0x25BD:
                                        text.Append("\\f");
                                        break;

                                    case 0xF100:
                                        isCompressed = true;
                                        break;

                                    case 0xFFFE:
                                        text.Append(@"\v");
                                        hasSpecialCharacter = true;
                                        break;

                                    case 0xFFFF:
                                        text.Append("");
                                        break;

                                    default:
                                        if (hasSpecialCharacter)
                                        {
                                            text.Append(textChar.ToString("X4"));
                                            hasSpecialCharacter = false;
                                        }
                                        if (isCompressed)
                                        {
                                            int shift = 0;
                                            int trans = 0;
                                            while (true)
                                            {
                                                int compChar = textChar >> shift;
                                                if (shift >= 0xF)
                                                {
                                                    shift -= 0xF;
                                                    if (shift > 0)
                                                    {
                                                        compChar = (trans | ((textChar << (9 - shift)) & 0x1FF));
                                                        if ((compChar & 0xFF) == 0xFF)
                                                        {
                                                            break;
                                                        }
                                                        if (compChar != 0x0 && compChar != 0x1)
                                                        {
                                                            text.Append(DecodeCharacter(compChar));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    compChar = (textChar >> shift) & 0x1FF;
                                                    if ((compChar & 0xFF) == 0xFF)
                                                    {
                                                        break;
                                                    }
                                                    if (compChar != 0x0 && compChar != 0x1)
                                                    {
                                                        text.Append(DecodeCharacter(compChar));
                                                    }
                                                    shift += 9;
                                                    if (shift < 0xF)
                                                    {
                                                        trans = (textChar >> shift) & 0x1FF;
                                                        shift += 9;
                                                    }
                                                    key1 += 0x493D;
                                                    key1 &= 0xFFFF;
                                                    textChar = Convert.ToUInt16(readText.ReadUInt16() ^ key1);
                                                    j++;
                                                }
                                            }
                                            text.Append("");
                                        }
                                        else
                                        {
                                            text.Append(DecodeCharacter(textChar));
                                        }
                                        break;
                                }
                                key1 += 0x493D;
                                key1 &= 0xFFFF;
                            }
                            messages.Add(text.ToString());
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                    readText.Close();
                    throw;
                }
            }
            else
            {
                return [];
            }

            List<MessageArchive> messageArchives = [];

            for (int i = 0; i < messages.Count; i++)
            {
                var item = new MessageArchive { MessageId = i, MessageText = messages[i] };
                messageArchives.Add(item);
            }
            readText.Close();
            readText.Dispose();
            return messageArchives;
        }

        public List<string> GetTrainerNames(int trainerNameMessageArchive)
        {
            var messageArchives = GetMessageArchiveContents(trainerNameMessageArchive, false);
            var trainerNames = new List<string>();
            foreach (var item in messageArchives)
            {
                trainerNames.Add(item.MessageText);
            }
            return trainerNames;
        }

        public GameFamily SetGameFamily(GameVersion gameVersion)
        {
            return gameVersion switch
            {
                GameVersion.Diamond or GameVersion.Pearl => GameFamily.DiamondPearl,
                GameVersion.Platinum => GameFamily.Platinum,
                GameVersion.HeartGold or GameVersion.SoulSilver => GameFamily.HeartGoldSoulSilver,
                GameVersion.HgEngine => GameFamily.HgEngine,
                _ => GameFamily.Unknown,
            };
        }

        public GameLanguage SetGameLanguage(string romId)
        {
            return romId switch
            {
                "ADAE" or "APAE" or "CPUE" or "IPKE" or "IPGE" => GameLanguage.English,
                "ADAS" or "APAS" or "CPUS" or "IPKS" or "IPGS" or "LATA" => GameLanguage.Spanish,
                "ADAI" or "APAI" or "CPUI" or "IPKI" or "IPGI" => GameLanguage.Italian,
                "ADAF" or "APAF" or "CPUF" or "IPKF" or "IPGF" => GameLanguage.French,
                "ADAD" or "APAD" or "CPUD" or "IPKD" or "IPGD" => GameLanguage.German,
                _ => GameLanguage.Japanese,
            };
        }

        public void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage)
        {
            Dictionary<NarcDirectory, string> packedDirectories = null;
            switch (gameFamily)
            {
                case GameFamily.DiamondPearl:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.PersonalPokeData] = gameVersion == GameVersion.Pearl ? @"data\poketool\personal_pearl\personal.narc" : @"data\poketool\personal\personal.narc",
                        [NarcDirectory.SynthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.TextArchive] = @"data\msgdata\msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.PokemonIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [NarcDirectory.MoveData] = @"data\poketool\waza\waza_tbl.narc",
                    };
                    break;

                case GameFamily.Platinum:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.SynthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.TextArchive] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.PokemonIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                        [NarcDirectory.MoveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                    };
                    break;

                case GameFamily.HeartGoldSoulSilver:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.BattleStagePokeData] = @"data\a\2\0\4",
                        [NarcDirectory.BattleTowerPokeData] = @"data\a\2\0\3",
                        [NarcDirectory.BattleTowerTrainerData] = @"data\a\2\0\2",
                        [NarcDirectory.PersonalPokeData] = @"data\a\0\0\2",
                        [NarcDirectory.SynthOverlay] = @"data\a\0\2\8",
                        [NarcDirectory.TextArchive] = @"data\a\0\2\7",
                        [NarcDirectory.TrainerProperties] = @"data\a\0\5\5",
                        [NarcDirectory.TrainerParty] = @"data\a\0\5\6",
                        [NarcDirectory.TrainerGraphics] = @"data\a\0\5\8",
                        [NarcDirectory.BattleMessageTable] = @"data\a\0\5\7",
                        [NarcDirectory.BattleMessageOffset] = @"data\a\1\3\1",
                        [NarcDirectory.PokemonIcons] = @"data\a\0\2\0",
                        [NarcDirectory.MoveData] = @"data\a\0\1\1",
                    };
                    break;
            }

            var directories = new Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)>();
            foreach (KeyValuePair<NarcDirectory, string> kvp in packedDirectories)
            {
                directories.Add(kvp.Key, ($"{workingDirectory}{kvp.Value}", $"{workingDirectory}unpacked\\{kvp.Key}"));
            }
            VsMakerDatabase.RomData.GameDirectories = directories;
        }

        public int SetTrainerNameTextArchiveNumber(GameFamily gameFamily, GameLanguage gameLanguage)
        {
            switch (gameFamily)
            {
                case GameFamily.DiamondPearl:
                    {
                        if (gameLanguage.Equals(GameLanguage.Japanese))
                        {
                            return 550;
                        }
                        else
                        {
                            return 559;
                        }
                    }

                case GameFamily.Platinum:
                    return 618;

                case GameFamily.HeartGoldSoulSilver:
                case GameFamily.HgEngine:
                    if (gameLanguage == GameLanguage.Japanese)
                    {
                        return 719;
                    }
                    else
                    {
                        return 729;
                    }
                default:
                    return 0;
            }
        }
        public (bool Success, string ExceptionMessage) UnpackNarcs(List<NarcDirectory> narcs, IProgress<int> progress)
        {
            int progressStep = 100 / narcs.Count;
            int count = 0;

            foreach (var item in narcs)
            {
                var (success, exceptionMessage) = UnpackNarc(item);
                if (success)
                {
                    progress?.Report(count += progressStep);
                }
                else
                {
                    progress?.Report(100);
                    return (false, exceptionMessage);
                }
            }
            return (true, null);
        }

        private string DecodeCharacter(int textChar)
        {
            if (ReadTextDictionary.TryGetValue(textChar, out var character))
            {
                return character;
            }
            else
            {
                return $"\\x{textChar:X4}";
            }
        }

        private (bool Succes, string ExceptionMessage) UnpackNarc(NarcDirectory narcPath)
        {
            try
            {
                if (VsMakerDatabase.RomData.GameDirectories.TryGetValue(narcPath, out (string packedPath, string unpackedPath) paths))
                {
                    DirectoryInfo directoryInfo = new(paths.unpackedPath);
                    if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0)
                    {
                        Narc openedNarc = Narc.Open(paths.packedPath);
                        if (openedNarc == null) { throw new NullReferenceException(); }
                        else
                        {
                            openedNarc.ExtractToFolder(paths.unpackedPath);
                        }
                    }
                    return (true, "");
                }
                else
                {
                    return (false, $"Error unpacking \"{paths.packedPath}\" - \"{narcPath}\"\n\nNarc not found in dictionary.");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}