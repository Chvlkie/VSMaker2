using System.Diagnostics;
using System.Text;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using VsMaker2Core.DSUtils;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class RomFileMethods : IRomFileMethods
    {
        private readonly Dictionary<int, string> ReadTextDictionary = VsMakerDatabase.RomData.TextCharacters.ReadTextDictionary;

        #region Extract

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

        #endregion Extract

        #region Get

        public List<string> GetAbilityNames(int abiltyNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(abiltyNameArchive, false);
            var abilityNames = new List<string>();
            foreach (var item in messageArchives)
            {
                abilityNames.Add(item.MessageText);
            }
            return abilityNames;
        }

        public List<string> GetClassDescriptions(int classDescriptionsArchive)
        {
            var messageArchives = GetMessageArchiveContents(classDescriptionsArchive, false);
            var classDescriptions = new List<string>();
            foreach (var item in messageArchives)
            {
                classDescriptions.Add(item.MessageText);
            }
            return classDescriptions;
        }

        public List<string> GetClassNames(int classNamesArchive)
        {
            var messageArchives = GetMessageArchiveContents(classNamesArchive, false);
            var classNames = new List<string>();
            foreach (var item in messageArchives)
            {
                classNames.Add(item.MessageText);
            }
            return classNames;
        }

        public List<string> GetItemNames(int itemNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(itemNameArchive, false);
            var itemNames = new List<string>();
            foreach (var item in messageArchives)
            {
                itemNames.Add(item.MessageText);
            }
            return itemNames;
        }

        public int SetTrainerNameMax(int trainerNameOffset)
        {
            if (trainerNameOffset > 0)
            {
                using Arm9.Arm9Reader ar = new(trainerNameOffset);
                int trainerNameLength = ar.ReadByte();
                ar.Close();
                return trainerNameLength;
            }
            else
            {
                return 8;
            }
        }

        public List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines)
        {
            int initialKey = 0;
            int stringCount = 0;
            List<string> messages = [];
            bool success = false;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchives].unpackedDirectory}\\{messageArchiveId:D4}";
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
                            StringBuilder text = new("");

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
                                        text.Append("\\v");
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

        public List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam)
        {
            uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(classGenderOffsetToRam, 4), 0) - Arm9.Address;
            List<ClassGenderData> classGenders = [];
            using Arm9.Arm9Reader reader = new(tableStartAddress);
            try
            {
                for (int i = 0; i < numberOfClasses; i++)
                {
                    var classGender = new ClassGenderData
                    {
                        Offset = reader.BaseStream.Position,
                        Gender = reader.ReadByte(),
                        TrainerClassId = i
                    };
                    classGenders.Add(classGender);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                throw;
            }
            return classGenders;
        }

        public int GetMessageInitialKey(int messageArchive)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchives].unpackedDirectory}\\{messageArchive:D4}";
            var fileStream = new FileStream(directory, FileMode.Open);
            BinaryReader readText = new(fileStream);
            try
            {
                readText.BaseStream.Position = 2;
                int initialKey = readText.ReadUInt16();
                readText.Close();
                return initialKey;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                readText.Close();
                throw;
            }
        }

        public List<string> GetMoveNames(int moveTextArchive)
        {
            var messageArchives = GetMessageArchiveContents(moveTextArchive, false);
            var moveNames = new List<string>();
            foreach (var item in messageArchives)
            {
                moveNames.Add(item.MessageText);
            }
            return moveNames;
        }

        public List<string> GetPokemonNames(int pokemonNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(pokemonNameArchive, false);
            var pokemonNames = new List<string>();
            foreach (var item in messageArchives)
            {
                pokemonNames.Add(item.MessageText);
            }
            return pokemonNames;
        }

        public List<Species> GetSpecies()
        {
            List<Species> allSpecies = [];
            int numberOfSpecies = Directory.GetFiles(VsMakerDatabase.RomData.GameDirectories[NarcDirectory.PersonalPokeData].unpackedDirectory, "*").Length;

            for (int i = 0; i < numberOfSpecies; i++)
            {
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.PersonalPokeData].unpackedDirectory}\\{i:D4}";

                var species = new Species { SpeciesId = (ushort)i };
                var fileStream = new FileStream(directory, FileMode.Open);
                using BinaryReader reader = new(fileStream);
                try
                {
                    reader.BaseStream.Position = Species.Constants.GenderRatioByteOffset;
                    species.GenderRatio = reader.ReadByte();
                    reader.BaseStream.Position = Species.Constants.AbilitySlot1ByteOffset;
                    species.Ability1 = reader.ReadByte();
                    species.Ability2 = reader.ReadByte();
                    allSpecies.Add(species);
                }
                catch (EndOfStreamException ex)
                {
                    Console.WriteLine(ex.Message);
                    reader.Close();
                    fileStream.Close();
                    throw;
                }
                reader.Close();
                fileStream.Close();
            }
            return allSpecies;
        }

        public int GetTotalNumberOfTrainers(int trainerNameArchive)
        {
            return GetMessageArchiveContents(trainerNameArchive, false).Count;
        }

        public int GetTotalNumberOfTrainerClassess(int trainerClassNameArchive)
        {
            return GetMessageArchiveContents(trainerClassNameArchive, false).Count;
        }

        public List<EyeContactMusicData> GetEyeContactMusicData(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily)
        {
            List<EyeContactMusicData> eyeContactMusic = [];
            uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(eyeContactMusicTableOffsetToRam, 4), 0) - Arm9.Address;
            uint tableSizeOffset = (uint)(gameFamily == GameFamily.HeartGoldSoulSilver ? 12 : 10);
            byte tableSize = Arm9.ReadByte(eyeContactMusicTableOffsetToRam - tableSizeOffset);
            using Arm9.Arm9Reader reader = new(tableStartAddress);
            try
            {
                for (int i = 0; i < tableSize; i++)
                {
                    uint offset = (uint)reader.BaseStream.Position;
                    ushort trainerClassId = reader.ReadUInt16();
                    ushort musicDayId = reader.ReadUInt16();
                    ushort? musicNightId = gameFamily == GameFamily.HeartGoldSoulSilver ? reader.ReadUInt16() : null;
                    var eyeContactMusicData = new EyeContactMusicData
                    {
                        Offset = offset,
                        TrainerClassId = trainerClassId,
                        MusicDayId = musicDayId,
                        MusicNightId = musicNightId,
                    };
                    eyeContactMusic.Add(eyeContactMusicData);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();

                throw;
            }

            return eyeContactMusic;
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

        public List<TrainerData> GetTrainersData(int numberOfTrainers)
        {
            var trainersData = new List<TrainerData>();
            for (int i = 0; i < numberOfTrainers; i++)
            {
                trainersData.Add(ReadTrainerData(i));
            }
            return trainersData;
        }

        public List<TrainerPartyData> GetTrainersPartyData(int numberOfTrainers, List<TrainerData> trainerData, GameFamily gameFamily)
        {
            var trainersPartyData = new List<TrainerPartyData>();
            for (int i = 0; i < numberOfTrainers; i++)
            {
                trainersPartyData.Add(ReadTrainerPartyData(i, trainerData[i].TeamSize, trainerData[i].TrainerType, gameFamily != GameFamily.DiamondPearl));
            }
            return trainersPartyData;
        }

        #endregion Get

        #region Read

        public TrainerData ReadTrainerData(int trainerId)
        {
            var trainerData = new TrainerData();
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerProperties].unpackedDirectory}\\{trainerId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open);
            using BinaryReader reader = new(fileStream);
            try
            {
                trainerData.TrainerType = reader.ReadByte();
                trainerData.TrainerClassId = reader.ReadByte();
                trainerData.Padding = reader.ReadByte();
                trainerData.TeamSize = reader.ReadByte();
                trainerData.Items = new ushort[4];
                for (int i = 0; i < 4; i++)
                {
                    trainerData.Items[i] = reader.ReadUInt16();
                }
                trainerData.AIFlags = reader.ReadUInt32();
                trainerData.IsDoubleBattle = reader.ReadUInt32();
                reader.Close();
                fileStream.Close();
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                fileStream.Close();
                throw;
            }

            return trainerData;
        }

        public TrainerPartyData ReadTrainerPartyData(int trainerId, byte teamSize, byte trainerType, bool hasBallCapsule)
        {
            var trainerPartyData = new TrainerPartyData
            {
                TrainerType = trainerType,
                PokemonData = new TrainerPartyPokemonData[teamSize],
            };

            bool hasMoves = (trainerType & 1) != 0;
            bool heldItems = (trainerType & 2) != 0;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerParty].unpackedDirectory}\\{trainerId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open);
            using var reader = new BinaryReader(fileStream);
            try
            {
                for (int i = 0; i < teamSize; i++)
                {
                    var trainerPartyPokemonData = new TrainerPartyPokemonData
                    {
                        Difficulty = reader.ReadByte(),
                        GenderAbilityOverride = reader.ReadByte(),
                        Level = reader.ReadUInt16(),
                        Species = reader.ReadUInt16()
                    };
                    if (heldItems)
                    {
                        trainerPartyPokemonData.ItemId = reader.ReadUInt16();
                    }
                    if (hasMoves)
                    {
                        trainerPartyPokemonData.MoveIds =
                        [
                            reader.ReadUInt16(),
                            reader.ReadUInt16(),
                            reader.ReadUInt16(),
                            reader.ReadUInt16(),
                        ];
                    }
                    if (hasBallCapsule)
                    {
                        trainerPartyPokemonData.BallCapsule = reader.ReadUInt16();
                    }
                    trainerPartyData.PokemonData[i] = trainerPartyPokemonData;
                }
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                fileStream.Close();
                throw;
            }
            reader.Close();
            fileStream.Close();
            return trainerPartyData;
        }

        #endregion Read

        #region Set

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
                        [NarcDirectory.TextArchives] = @"data\msgdata\msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.MonIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [NarcDirectory.MoveData] = @"data\poketool\waza\waza_tbl.narc",
                    };
                    break;

                case GameFamily.Platinum:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.SynthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.TextArchives] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.MonIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
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
                        [NarcDirectory.TextArchives] = @"data\a\0\2\7",
                        [NarcDirectory.TrainerProperties] = @"data\a\0\5\5",
                        [NarcDirectory.TrainerParty] = @"data\a\0\5\6",
                        [NarcDirectory.TrainerGraphics] = @"data\a\0\5\8",
                        [NarcDirectory.BattleMessageTable] = @"data\a\0\5\7",
                        [NarcDirectory.BattleMessageOffset] = @"data\a\1\3\1",
                        [NarcDirectory.MonIcons] = @"data\a\0\2\0",
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

        #endregion Set

        #region Unpack

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

        #endregion Unpack

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
    }
}