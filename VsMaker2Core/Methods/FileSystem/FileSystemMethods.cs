using MsgPack.Serialization;
using System.Text;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using VsMaker2Core.DsUtils;
using VsMaker2Core.DSUtils;
using VsMaker2Core.MessageEncrypt;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class FileSystemMethods : IFileSystemMethods
    {
        private IRomFileMethods romFileMethods;
        private IScriptFileMethods scriptFileMethods;

        public FileSystemMethods()
        {
            romFileMethods = new RomFileMethods();
            scriptFileMethods = new ScriptFileMethods();
        }

        public void AddNewTrainerClassSprite()
        {
            var trainerGraphicsPath = VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerGraphics].unpackedDirectory;

            if (!Directory.Exists(trainerGraphicsPath))
            {
                Console.WriteLine("Trainer graphics directory does not exist.");
                return;
            }

            var files = Directory.GetFiles(trainerGraphicsPath);
            int totalFiles = files.Length;
            string[] filesToCopy = { "0000", "0001", "0002", "0003", "0004" };

            var logBuilder = new StringBuilder();

            for (int i = 0; i < filesToCopy.Length; i++)
            {
                string sourceFile = Path.Combine(trainerGraphicsPath, filesToCopy[i]);

                if (File.Exists(sourceFile))
                {
                    string newFileName = (totalFiles + i).ToString("D4");
                    string destinationFile = Path.Combine(trainerGraphicsPath, newFileName);

                    try
                    {
                        File.Copy(sourceFile, destinationFile, overwrite: false);
                        logBuilder.AppendLine($"Copied {sourceFile} to {destinationFile}");
                    }
                    catch (Exception ex)
                    {
                        logBuilder.AppendLine($"Error copying {sourceFile} to {destinationFile}: {ex.Message}");
                    }
                }
                else
                {
                    logBuilder.AppendLine($"File {filesToCopy[i]} does not exist in the directory.");
                }
            }

            Console.WriteLine(logBuilder.ToString());
            Console.WriteLine("File duplication process complete.");
        }

        public VsTrainersFile BuildVsTrainersFile(List<Trainer> trainers, GameFamily gameFamily, int trainerNameTextArchiveId, int classesCount, int battleMessagesCount)
        {
            if (trainers == null || trainers.Count == 0)
            {
                throw new ArgumentException("Trainers list cannot be null or empty.");
            }

            int trainerCount = trainers.Count;

            return new VsTrainersFile
            {
                TrainerData = trainers,
                GameFamily = gameFamily,
                ClassesCount = classesCount,
                BattleMessagesCount = battleMessagesCount,
                TrainerNamesFile = GetTrainerNamesFile(trainerNameTextArchiveId),
                TrainerPartyFiles = GetAllTrainerPartyFiles(trainerCount),
                TrainerPropertyFiles = GetAllTrainerPropertyFiles(trainerCount)
            };
        }

        #region Write

        public (bool Success, string ErrorMessage) UpdateTrainerScripts(int totalNumberOfTrainers)
        {
            try
            {
                var originalScriptData = scriptFileMethods.GetScriptFileData(RomFile.TrainerScriptFile);

                if (originalScriptData?.Scripts == null || originalScriptData.Scripts.Count == 0)
                {
                    return (false, "Original script data is null or empty.");
                }

                int trainerScriptCount = Math.Max(totalNumberOfTrainers, RomFile.OriginalTrainerEncounterScript);

                List<ScriptData> newScripts = new(trainerScriptCount)
                {
                    originalScriptData.Scripts[0]
                };

                int loopCount = trainerScriptCount - 2;

                // Add trainer scripts (reuse the second script, but update with incremented script index)
                for (int i = 0; i < loopCount; i++)
                {
                    newScripts.Add(new ScriptData(originalScriptData.Scripts[1], (uint)(i + 2)));
                }

                // Add the encounter script (last one)
                newScripts.Add(new ScriptData(originalScriptData.Scripts.Last(), (uint)trainerScriptCount));

                // Create new script data and write it
                var newScriptData = new ScriptFileData(originalScriptData, newScripts);
                var writeScripts = scriptFileMethods.WriteScriptData(newScriptData);

                if (!writeScripts.Success)
                {
                    return (false, writeScripts.ErrorMessage);
                }

                // Calculate the new encounter script ID
                uint encounterScriptId = (uint)(2999 + newScriptData.Scripts.Count);

                // Write the updated encounter script ID
                using (var writer = new Arm9.Arm9Writer(RomFile.TrainerEncounterScriptOffset))
                {
                    writer.Write(encounterScriptId);
                }

                return (true, "");
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) WriteBattleMessage(List<string> battleMessages, int messageId, string newMessage, int battleMessageArchive)
        {
            if (battleMessages == null || !battleMessages.Any())
            {
                return (false, "Battle messages list is null or empty.");
            }

            if (messageId < 0 || messageId >= battleMessages.Count)
            {
                return (false, $"Message ID {messageId} is out of range.");
            }

            if (string.IsNullOrWhiteSpace(newMessage))
            {
                return (false, "New message cannot be null or empty.");
            }

            battleMessages[messageId] = newMessage;

            return WriteMessage(battleMessages, battleMessageArchive, false);
        }

        public (bool Success, string ErrorMessage) WriteBattleMessageOffsetData(List<ushort> offsets, IProgress<int> progress)
        {
            string directoryPath = Path.Combine(VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerTextOffset].unpackedDirectory, "0000");

            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return (false, $"Directory {directoryPath} does not exist.");
                }

                using (var fileStream = new FileStream(directoryPath, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var binaryWriter = new BinaryWriter(bufferedStream))
                {
                    int totalOffsets = offsets.Count;
                    for (int i = 0; i < totalOffsets; i++)
                    {
                        binaryWriter.Write(offsets[i]);

                        if (progress != null)
                        {
                            int progressPercentage = (i + 1) * 100 / totalOffsets;
                            progress.Report(progressPercentage);
                        }
                    }
                }

                return (true, "");
            }
            catch (DirectoryNotFoundException ex)
            {
                return (false, $"Directory not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) WriteBattleMessageTableData(List<BattleMessage> messageData, IProgress<int> progress)
        {
            string directoryPath = Path.Combine(VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerTextTable].unpackedDirectory, "0000");

            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    return (false, $"Directory {directoryPath} does not exist.");
                }

                if (messageData == null || messageData.Count == 0)
                {
                    return (false, "Message data is null or empty.");
                }

                using (var fileStream = new FileStream(directoryPath, FileMode.Create, FileAccess.Write))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var binaryWriter = new BinaryWriter(bufferedStream))
                {
                    int totalMessages = messageData.Count;
                    for (int i = 0; i < totalMessages; i++)
                    {
                        binaryWriter.Write((ushort)messageData[i].TrainerId);
                        binaryWriter.Write((ushort)messageData[i].MessageTriggerId);

                        if (progress != null && i % 5 == 0)
                        {
                            int progressPercentage = (i + 1) * 100 / totalMessages;
                            progress.Report(progressPercentage);
                        }
                    }
                }

                return (true, "");
            }
            catch (DirectoryNotFoundException ex)
            {
                return (false, $"Directory not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) WriteBattleMessageTexts(List<string> messages, int battleMessageArchive)
        {
            if (messages == null || messages.Count == 0)
            {
                return (false, "Messages list is null or empty.");
            }

            if (battleMessageArchive < 0)
            {
                return (false, "Invalid battle message archive ID.");
            }

            return WriteMessage(messages, battleMessageArchive);
        }

        public (bool Success, string ErrorMessage) WriteClassDescription(List<string> descriptions, int classId, string newDescription, int classDescriptionMessageNumber)
        {
            if (descriptions == null || !descriptions.Any())
            {
                return (false, "Descriptions list is null or empty.");
            }

            if (classId < 0 || classId >= descriptions.Count)
            {
                return (false, $"Class ID {classId} is out of range.");
            }

            if (string.IsNullOrWhiteSpace(newDescription))
            {
                return (false, "New description cannot be null or empty.");
            }

            descriptions[classId] = newDescription;

            return WriteMessage(descriptions, classDescriptionMessageNumber);
        }

        public (bool Success, string ErrorMessage) WriteClassGenderData(ClassGenderData classGenderData)
        {
            try
            {
                if (RomFile.ClassGenderExpanded)
                {
                    using FileStream overlayStream = new(RomFile.SynthOverlayFilePath, FileMode.Open, FileAccess.Write);
                    using BinaryWriter writer = new(overlayStream);

                    if (classGenderData.Offset < 0 || classGenderData.Offset > overlayStream.Length)
                    {
                        return (false, $"Invalid offset {classGenderData.Offset} for writing class gender data.");
                    }

                    overlayStream.Position = (uint)classGenderData.Offset;
                    writer.Write(classGenderData.Gender);
                }
                else
                {
                    if (classGenderData.Offset < 0)
                    {
                        return (false, $"Invalid offset {classGenderData.Offset} for writing class gender data.");
                    }

                    Arm9.WriteByte(classGenderData.Gender, (uint)classGenderData.Offset);
                }
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An unexpected error occurred: {ex.Message}");
            }

            return (true, "");
        }

        public (bool Success, string ErrorMessage) WriteClassName(List<string> classNames, int classId, string newName, int classNamesArchive)
        {
            if (classNames == null || classNames.Count == 0)
            {
                return (false, "Class names list is null or empty.");
            }

            if (classId < 0 || classId >= classNames.Count)
            {
                return (false, $"Class ID {classId} is out of range.");
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                return (false, "New class name cannot be null or empty.");
            }

            classNames[classId] = newName;

            return WriteMessage(classNames, classNamesArchive);
        }

        public (bool Success, string ErrorMessage) WriteEyeContactMusicData(EyeContactMusicData eyeContactMusicData, RomFile loadedRom)
        {
            try
            {
                if (eyeContactMusicData == null)
                {
                    return (false, "Eye contact music data is null.");
                }

                if (eyeContactMusicData.Offset < 0)
                {
                    return (false, $"Invalid offset: {eyeContactMusicData.Offset}");
                }

                byte[] musicDayBytes = BitConverter.GetBytes(eyeContactMusicData.MusicDayId);
                byte[] musicNightBytes = BitConverter.GetBytes(eyeContactMusicData.MusicNightId ?? 0);

                if (RomFile.EyeContactExpanded)
                {
                    using FileStream overlayStream = new(RomFile.SynthOverlayFilePath, FileMode.Open, FileAccess.Write);
                    using BinaryWriter writer = new(overlayStream);

                    overlayStream.Position = eyeContactMusicData.Offset + 2;
                    writer.Write(musicDayBytes);

                    if (RomFile.IsHeartGoldSoulSilver)
                    {
                        overlayStream.Position = eyeContactMusicData.Offset + 4;
                        writer.Write(musicNightBytes);
                    }
                }
                else
                {
                    Arm9.WriteBytes(musicDayBytes, eyeContactMusicData.Offset + 2);

                    if (RomFile.IsHeartGoldSoulSilver)
                    {
                        Arm9.WriteBytes(musicNightBytes, eyeContactMusicData.Offset + 4);
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }

            return (true, "");
        }

        public (bool Success, string ErrorMessage) WriteMessage(List<string> messages, int messageArchive, bool isTrainerName = false)
        {
            try
            {
                if (messages == null || !messages.Any())
                {
                    return (false, "Messages list is null or empty.");
                }

                if (messageArchive < 0)
                {
                    return (false, $"Invalid message archive: {messageArchive}");
                }

                EncryptText.WriteMessageArchive(messageArchive, messages, isTrainerName);
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }

            return (true, "");
        }

        public async Task<(bool Success, string ErrorMessage)> WritePrizeMoneyDataAsync(PrizeMoneyData prizeMoneyData, RomFile loadedRom)
        {
            try
            {
                string filePath;

                if (RomFile.IsHeartGoldSoulSilver)
                {
                    if (!RomFile.PrizeMoneyExpanded)
                    {
                        bool isCompressed = await Overlay.CheckOverlayIsCompressedAsync(RomFile.PrizeMoneyTableOverlayNumber);
                        if (isCompressed)
                        {
                            await Overlay.DecompressOverlayAsync(RomFile.PrizeMoneyTableOverlayNumber);
                            Overlay.SetOverlayCompressionInTable(RomFile.PrizeMoneyTableOverlayNumber, 0);
                        }
                    }

                    filePath = RomFile.PrizeMoneyExpanded
                        ? RomFile.SynthOverlayFilePath
                        : Overlay.OverlayFilePath(RomFile.PrizeMoneyTableOverlayNumber);
                }
                else
                {
                    filePath = RomFile.OverlayPath;
                }

                using EasyWriter writer = new(filePath, prizeMoneyData.Offset);

                // Write prize money data
                if (RomFile.IsHeartGoldSoulSilver)
                {
                    writer.Write(prizeMoneyData.TrainerClassId);
                }

                writer.Write(prizeMoneyData.PrizeMoney);

                return (true, "");
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) WriteTrainerData(TrainerData trainerData, int trainerId)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerProperties].unpackedDirectory}\\{trainerId:D4}";

            try
            {
                using var stream = new MemoryStream();
                using (BinaryWriter writer = new(stream))
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

                File.WriteAllBytes(directory, stream.ToArray());

                return (true, "");
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) WriteTrainerName(List<string> trainerNames, int trainerId, string newName, int trainerNamesArchive)
        {
            if (trainerNames == null || trainerNames.Count == 0)
            {
                return (false, "Trainer names list is null or empty.");
            }

            if (trainerId < 0 || trainerId >= trainerNames.Count)
            {
                return (false, $"Trainer ID {trainerId} is out of range.");
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                return (false, "New trainer name cannot be null or empty.");
            }

            trainerNames[trainerId] = newName;

            return WriteMessage(trainerNames, trainerNamesArchive, true);
        }

        public (bool Success, string ErrorMessage) WriteTrainerPartyData(TrainerPartyData partyData, int trainerId, bool chooseItems, bool chooseMoves, bool hasBallCapsule)
        {
            if (partyData.PokemonData == null || partyData.PokemonData.Length == 0)
            {
                return (false, "Pokemon data is null or empty.");
            }

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{trainerId:D4}";

            try
            {
                using var stream = new MemoryStream();
                using (BinaryWriter writer = new(stream))
                {
                    for (int i = 0; i < partyData.PokemonData.Length; i++)
                    {
                        TrainerPartyPokemonData pokemon = partyData.PokemonData[i];

                        writer.Write(pokemon.Difficulty);
                        writer.Write(pokemon.GenderAbilityOverride);
                        writer.Write(pokemon.Level);
                        writer.Write(pokemon.Species);

                        if (chooseItems)
                        {
                            if (pokemon.ItemId.HasValue && pokemon.ItemId == 0xFFFF)
                            {
                                pokemon.ItemId = 0;
                            }
                            writer.Write(pokemon.ItemId ?? 0);
                        }

                        if (chooseMoves)
                        {
                            writer.Write(pokemon.MoveIds[0]);
                            writer.Write(pokemon.MoveIds[1]);
                            writer.Write(pokemon.MoveIds[2]);
                            writer.Write(pokemon.MoveIds[3]);
                        }

                        if (hasBallCapsule)
                        {
                            if (pokemon.BallCapsule.HasValue && pokemon.BallCapsule == 0xFFFF)
                            {
                                pokemon.BallCapsule = 0;
                            }
                            writer.Write(pokemon.BallCapsule ?? 0);
                        }
                    }
                }

                File.WriteAllBytes(directory, stream.ToArray());
                return (true, "");
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        #endregion Write

        #region Delete

        public (bool Success, string ErrorMessage) RemoveTrainer(int trainerId)
        {
            try
            {
                string trainerPropertiesDirectory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerProperties].unpackedDirectory}\\{trainerId:D4}";
                string trainerPartyDirectory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{trainerId:D4}";

                if (File.Exists(trainerPropertiesDirectory))
                {
                    File.Delete(trainerPropertiesDirectory);
                }
                else
                {
                    return (false, $"Trainer properties file not found for trainer ID {trainerId}.");
                }

                if (File.Exists(trainerPartyDirectory))
                {
                    File.Delete(trainerPartyDirectory);
                }
                else
                {
                    return (false, $"Trainer party file not found for trainer ID {trainerId}.");
                }

                return (true, "");
            }
            catch (FileNotFoundException ex)
            {
                return (false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        #endregion Delete

        public (bool Success, string ErrorMessage) ExportTrainers(VsTrainersFile export, string filePath)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var serializer = MessagePackSerializer.Get<VsTrainersFile>();
                    serializer.Pack(stream, export);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        stream.WriteTo(fileStream);
                    }
                }

                return (true, string.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (false, $"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public (VsTrainersFile VsTrainersFile, bool Success, string ErrorMessage) ImportTrainers(string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    var serializer = MessagePackSerializer.Get<VsTrainersFile>();
                    VsTrainersFile imported = serializer.Unpack(fileStream);

                    return (imported, true, string.Empty);
                }
            }
            catch (FileNotFoundException ex)
            {
                return (null, false, $"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (null, false, $"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                return (null, false, $"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (null, false, $"An error occurred: {ex.Message}");
            }
        }

        private static List<byte[]> GetAllTrainerPartyFiles(int trainerCount)
        {
            List<byte[]> trainerFiles = new(trainerCount);

            try
            {
                for (int i = 0; i < trainerCount; i++)
                {
                    string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{i:D4}";

                    using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);
                    trainerFiles.Add(fileData);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return trainerFiles;
        }

        private static List<byte[]> GetAllTrainerPropertyFiles(int trainerCount)
        {
            List<byte[]> trainerFiles = new(trainerCount);

            try
            {
                for (int i = 0; i < trainerCount; i++)
                {
                    string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerProperties].unpackedDirectory}\\{i:D4}";

                    using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);
                    trainerFiles.Add(fileData);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return trainerFiles;
        }

        private static byte[] GetTrainerNamesFile(int trainerNameTextArchiveId)
        {
            try
            {
                string directory = $"{Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{trainerNameTextArchiveId:D4}";

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                byte[] fileData = new byte[fileStream.Length];
                fileStream.Read(fileData, 0, fileData.Length);

                return fileData;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied: {ex.Message}");
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}