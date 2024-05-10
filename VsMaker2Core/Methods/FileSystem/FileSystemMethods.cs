using MsgPack.Serialization;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class FileSystemMethods : IFileSystemMethods
    {
        private readonly Dictionary<int, int> WriteTextDictionary = VsMakerDatabase.RomData.TextCharacters.WriteTextDictionary;
        private IRomFileMethods romFileMethods;

        public FileSystemMethods()
        {
            romFileMethods = new RomFileMethods();
        }

        public VsTrainersFile BuildVsTrainersFile(List<Trainer> trainers, GameFamily gameFamily, int trainerNameTextArchiveId, int classesCount, int battleMessagesCount)
        {
            return new VsTrainersFile
            {
                TrainerData = trainers,
                GameFamily = gameFamily,
                ClassesCount = classesCount,
                BattleMessagesCount = battleMessagesCount,
                TrainerNamesFile = GetTrainerNamesFile(trainerNameTextArchiveId),
                TrainerPartyFiles = GetAllTrainerPartyFiles(trainers.Count),
                TrainerPropertyFiles = GetAllTrainerPropertyFiles(trainers.Count)
            };
        }

        #region Write

        public (bool Success, string ErrorMessage) WriteClassName(List<string> classNames, int classId, string newName, int classNamesArchive)
        {
            classNames[classId] = newName;
            return WriteMessage(classNames, classNamesArchive);
        }

        public (bool Success, string ErrorMessage) WriteTrainerName(List<string> trainerNames, int trainerId, string newName, int trainerNamesArchive)
        {
            trainerNames[trainerId] = newName;
            List<string> writeMessages = [];
            foreach (string message in trainerNames)
            {
                writeMessages.Add($"{{TRNNAME}}{message}");
            }
            return WriteMessage(writeMessages, trainerNamesArchive);
        }

        public (bool Success, string ErrorMessage) WriteMessage(List<string> messages, int messageArchive)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchives].unpackedDirectory}\\{messageArchive:D4}";
            int initialKey = romFileMethods.GetMessageInitialKey(messageArchive);

            var stream = new MemoryStream();
            using BinaryWriter writer = new(stream);
            try
            {
                var encoded = EncodeMessages(messages);
                writer.Write((ushort)encoded.Count);
                writer.Write((ushort)initialKey);
                int key = (initialKey * 0x2FD) & 0xFFFF;
                int key2 = 0;
                int realKey = 0;
                int offset = 0x4 + (encoded.Count * 8);
                int[] stringLengths = new int[encoded.Count];

                for (int i = 0; i < encoded.Count; i++)
                { // Reads and stores string offsets and sizes
                    key2 = (key * (i + 1) & 0xFFFF);
                    realKey = key2 | (key2 << 16);
                    writer.Write(offset ^ realKey);
                    int[] currentString = encoded[i];
                    int length = encoded[i].Length;
                    stringLengths[i] = length;
                    writer.Write(length ^ realKey);
                    offset += length * 2;
                }

                for (int i = 0; i < encoded.Count; i++)
                { // Encodes strings and writes them to file
                    key = (0x91BD3 * (i + 1)) & 0xFFFF;
                    int[] currentString = encoded[i];
                    for (int j = 0; j < stringLengths[i] - 1; j++)
                    {
                        writer.Write((ushort)(currentString[j] ^ key));
                        key += 0x493D;
                        key &= 0xFFFF;
                    }
                    writer.Write((ushort)(0xFFFF ^ key));
                }
                File.WriteAllBytes(directory, stream.ToArray());
                stream.Close();
            }
            catch (Exception ex)
            {
                stream.Close();
                Console.WriteLine(ex.Message);
                throw;
                return (false, ex.Message);
            }
            return (true, "");
        }

        public (bool Success, string ErrorMessage) WriteTrainerData(TrainerData trainerData, int trainerId)
        {
            string directory = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerProperties].unpackedDirectory}\\{trainerId:D4}";
            var stream = new MemoryStream();
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(trainerData.TrainerType);
                writer.Write(trainerData.TrainerClassId);
                writer.Write(trainerData.Padding);
                writer.Write(trainerData.TeamSize);
                foreach (var item in trainerData.Items)
                {
                    writer.Write(item);
                }
                writer.Write(trainerData.AIFlags);
                writer.Write(trainerData.IsDoubleBattle);
            }
            try
            {
                File.WriteAllBytes(directory, stream.ToArray());
                stream.Close();
                return (true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                stream.Close();
                return (false, ex.Message);
            }
        }

        public (bool Success, string ErrorMessage) WriteTrainerPartyData(TrainerPartyData partyData, int trainerId, bool chooseItems, bool chooseMoves, bool hasBallCapsule)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerParty].unpackedDirectory}\\{trainerId:D4}";
            var stream = new MemoryStream();
            using (BinaryWriter writer = new(stream))
            {
                for (int i = 0; i < partyData.PokemonData.Length; i++)
                {
                    TrainerPartyPokemonData pokemon = partyData.PokemonData[i];
                    writer.Write(pokemon.Difficulty);
                    writer.Write(pokemon.GenderAbilityOverride);
                    writer.Write(pokemon.Level);
                    writer.Write(pokemon.Species);
                    if (chooseItems) { writer.Write(pokemon.ItemId ?? 0); }
                    if (chooseMoves)
                    {
                        writer.Write(pokemon.MoveIds[0]);
                        writer.Write(pokemon.MoveIds[1]);
                        writer.Write(pokemon.MoveIds[2]);
                        writer.Write(pokemon.MoveIds[3]);
                    }
                    if (hasBallCapsule) { writer.Write(pokemon.BallCapsule ?? 0); }
                }
            }
            try
            {
                File.WriteAllBytes(directory, stream.ToArray());
                stream.Close();
                return (true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                stream.Close();
                return (false, ex.Message);
            }
        }

        #endregion Write

        public (bool Success, string ErrorMessage) ExportTrainers(VsTrainersFile export, string filePath)
        {
            try
            {
                var stream = new MemoryStream();
                var serializer = MessagePackSerializer.Get<VsTrainersFile>();
                serializer.Pack(stream, export);
                stream.Position = 0;

                var fileSteam = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                stream.WriteTo(fileSteam);
                stream.Close();
                fileSteam.Close();
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, string.Empty);
        }

        public (VsTrainersFile VsTrainersFile, bool Success, string ErrorMessage) ImportTrainers(string filePath)
        {
            var vsTrainersFile = new VsTrainersFile();
            try
            {
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var serializer = MessagePackSerializer.Get<VsTrainersFile>();
                VsTrainersFile imported = serializer.Unpack(fileStream);
                vsTrainersFile.TrainerData = imported.TrainerData;
                vsTrainersFile.GameFamily = imported.GameFamily;
                vsTrainersFile.ClassesCount = imported.ClassesCount;
                vsTrainersFile.BattleMessagesCount = imported.BattleMessagesCount;
                vsTrainersFile.TrainerNamesFile = imported.TrainerNamesFile;
                vsTrainersFile.TrainerPartyFiles = imported.TrainerPartyFiles;
                vsTrainersFile.TrainerPropertyFiles = imported.TrainerPropertyFiles;
                fileStream.Close();
            }
            catch (Exception ex)
            {
                return (null, false, ex.Message);
            }
            return (vsTrainersFile, true, string.Empty);
        }

        private int[] EncodeMessage(string message)
        {
            List<int> encoded = [];
            int compressionBuffer = 0;
            int bit = 0;
            bool isTrainerName = message.Contains("{TRNNAME}");
            if (isTrainerName)
            {
                message = message.Substring(9);
                encoded.Add(0xF100);
            }
            var charArray = message.ToCharArray();
            string characterId;
            for (int i = 0; i < charArray.Length; i++)
            {
                switch (charArray[i])
                {
                    case '\\':
                        switch (charArray[i + 1])
                        {
                            case 'r':
                                encoded.Add(0x25BC);
                                i++;
                                break;

                            case 'n':
                                encoded.Add(0xE000);
                                i++;
                                break;

                            case 'f':
                                encoded.Add(0x25BD);
                                i++;
                                break;

                            case 'v':
                                encoded.Add(0xFFFE);
                                characterId = $"{charArray[i + 2]}{charArray[i + 3]}{charArray[i + 4]}{charArray[i + 5]}";
                                encoded.Add((int)Convert.ToUInt32(characterId, 16));
                                i += 5;
                                break;

                            case 'x':
                                if (charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '0')
                                {
                                    encoded.Add(0x0000);
                                    i += 5;
                                    break;
                                }
                                else if (charArray[i + 2] == '0' && charArray[i + 3] == '0' && charArray[i + 4] == '0' && charArray[i + 5] == '1')
                                {
                                    encoded.Add(0x0001);
                                    i += 5;
                                    break;
                                }
                                else
                                {
                                    characterId = $"{charArray[i + 2]}{charArray[i + 3]}{charArray[i + 4]}{charArray[i + 5]}";
                                    encoded.Add((int)Convert.ToUInt32(characterId, 16));
                                    i += 5;
                                    break;
                                }
                        }
                        break;

                    case '[':
                        switch (charArray[i + 1])
                        {
                            case 'P':
                                encoded.Add(0x01E0);
                                i += 3;
                                break;

                            case 'M':
                                encoded.Add(0x01E1);
                                i += 3;
                                break;
                        }
                        break;

                    default:

                        WriteTextDictionary.TryGetValue(charArray[i], out int code);
                        if (isTrainerName)
                        {
                            compressionBuffer |= code << bit;
                            bit += 9;
                            if (bit >= 15)
                            {
                                bit -= 15;
                                encoded.Add((int)Convert.ToUInt32(compressionBuffer & 0x7FFF));
                                compressionBuffer >>= 15;
                            }
                        }
                        else
                        {
                            encoded.Add(code);
                        }
                        break;
                }
            }
            if (isTrainerName && bit > 1)
            {
                compressionBuffer |= (0xFFFF << bit);
                encoded.Add((int)Convert.ToUInt32(compressionBuffer & 0x7FFF));
            }
            encoded.Add(0xFFFF);
            return [.. encoded];
        }

        private List<int[]> EncodeMessages(List<string> messages)
        {
            List<int[]> messagesArray = [];
            foreach (var message in messages)
            {
                messagesArray.Add(EncodeMessage(message));
            }
            return messagesArray;
        }

        private List<byte[]> GetAllTrainerPartyFiles(int trainerCount)
        {
            List<byte[]> trainerFiles = [];

            for (int i = 0; i < trainerCount; i++)
            {
                string directory = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerParty].unpackedDirectory}\\{i:D4}";
                var fileStream = new FileStream(directory, FileMode.Open);
                using var stream = new MemoryStream();
                fileStream.CopyTo(stream);
                trainerFiles.Add(stream.ToArray());
            }

            return trainerFiles;
        }

        private List<byte[]> GetAllTrainerPropertyFiles(int trainerCount)
        {
            List<byte[]> trainerFiles = [];

            for (int i = 0; i < trainerCount; i++)
            {
                string directory = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerProperties].unpackedDirectory}\\{i:D4}";
                var fileStream = new FileStream(directory, FileMode.Open);
                using var stream = new MemoryStream();
                fileStream.CopyTo(stream);
                trainerFiles.Add(stream.ToArray());
            }

            return trainerFiles;
        }

        private byte[] GetTrainerNamesFile(int trainerNameTextArchiveId)
        {
            string directory = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchives].unpackedDirectory}\\{trainerNameTextArchiveId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open);
            using var stream = new MemoryStream();
            fileStream.CopyTo(stream);
            return stream.ToArray();
        }
    }
}