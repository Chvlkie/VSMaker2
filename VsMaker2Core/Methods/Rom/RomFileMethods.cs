using System.Diagnostics;
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
    public class RomFileMethods : IRomFileMethods
    {
        #region Extract

        public (bool Success, string ErrorMessage) LoadInitialRomData(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return (false, "Invalid ROM file path provided.");
            }

            try
            {
                using EasyReader reader = new(filePath, 0xC);

                RomFile.GameCode = Encoding.ASCII.GetString(reader.ReadBytes(4));

                reader.BaseStream.Position = 0x1E;
                RomFile.EuropeByte = reader.ReadByte();
                return (true, "");
            }
            catch (Exception ex)
            {
                return (false, $"Error loading ROM data: {ex.Message}");
            }
        }

        public async Task<(bool Success, string ExceptionMessage)> ExtractRomContentsAsync(string workingDirectory, string fileName)
        {
            var arguments = $"-x \"{fileName}\" -9 \"{RomFile.Arm9Path}\" -7 \"{Path.Combine(workingDirectory, Common.Arm7FilePath)}\" " +
                            $"-y9 \"{Path.Combine(workingDirectory, Common.Y9FilePath)}\" -y7 \"{Path.Combine(workingDirectory, Common.Y7FilePath)}\" " +
                            $"-d \"{Path.Combine(workingDirectory, Common.DataFilePath)}\" -y \"{Path.Combine(workingDirectory, Common.OverlayFilePath)}\" " +
                            $"-t \"{Path.Combine(workingDirectory, Common.BannerFilePath)}\" -h \"{Path.Combine(workingDirectory, Common.HeaderFilePath)}\"";

            using var unpack = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Common.NdsToolsFilePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };

            try
            {
                unpack.Start();

                var outputTask = unpack.StandardOutput.ReadToEndAsync();
                var errorTask = unpack.StandardError.ReadToEndAsync();

                await unpack.WaitForExitAsync();

                string output = await outputTask;
                string error = await errorTask;

                if (unpack.ExitCode == 0)
                {
                    return (true, string.Empty);
                }
                else
                {
                    return (false, $"Process failed with exit code: {unpack.ExitCode}. Error: {error}. Output: {output}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }

        #endregion Extract

        #region Get

        public List<string> GetAbilityNames(int abilityNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(abilityNameArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<BattleMessageOffsetData> GetBattleMessageOffsetData(string battleMessageOffsetPath)
        {
            var battleMessageOffsetData = new List<BattleMessageOffsetData>();

            try
            {
                if (string.IsNullOrWhiteSpace(battleMessageOffsetPath) || !File.Exists(battleMessageOffsetPath))
                {
                    Console.WriteLine($"Invalid or missing file path: {battleMessageOffsetPath}");
                    return battleMessageOffsetData;
                }

                using var fileStream = new FileStream(battleMessageOffsetPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                using var reader = new BinaryReader(fileStream);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ushort offset = reader.ReadUInt16();
                    battleMessageOffsetData.Add(new BattleMessageOffsetData(BattleMessageOffsetData.OffsetToMessageId(offset)));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO exception while reading the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }

            return battleMessageOffsetData;
        }

        public List<string> GetBattleMessages(int battleMessageArchive)
        {
            var messageArchives = GetMessageArchiveContents(battleMessageArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<BattleMessageTableData> GetBattleMessageTableData(string trainerTextTablePath)
        {
            var trainerTextTableDatas = new List<BattleMessageTableData>();

            try
            {
                if (string.IsNullOrWhiteSpace(trainerTextTablePath) || !File.Exists(trainerTextTablePath))
                {
                    Console.WriteLine($"Invalid or missing file path: {trainerTextTablePath}");
                    return trainerTextTableDatas;
                }

                using FileStream fileStream = new(trainerTextTablePath, FileMode.Open, FileAccess.Read);
                using BinaryReader reader = new(fileStream);

                int messageId = 0;
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    ushort trainerId = reader.ReadUInt16();
                    ushort messageTriggerId = reader.ReadUInt16();
                    trainerTextTableDatas.Add(new BattleMessageTableData(messageId++, trainerId, messageTriggerId));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while reading the file: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }

            return trainerTextTableDatas;
        }

        public List<string> GetClassDescriptions(int classDescriptionsArchive)
        {
            var messageArchives = GetMessageArchiveContents(classDescriptionsArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam)
        {
            if (numberOfClasses <= 0)
            {
                Console.WriteLine("Invalid number of classes.");
                return [];
            }
            var classGenders = new List<ClassGenderData>(numberOfClasses);

            if (RomFile.IsHgEngine)
            {
                using var overlayStream = new FileStream(Overlay.OverlayFilePath(RomFile.HgEngineOverlay), FileMode.Open, FileAccess.Read);
                uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(0xFFB90, 4), 0) - RomFile.HgEngineOverlayLoadAddress;

                overlayStream.Position = tableStartAddress;

                using var reader = new BinaryReader(overlayStream);
                try
                {
                    for (int i = 0; i < numberOfClasses; i++)
                    {
                        long offset = reader.BaseStream.Position;
                        byte gender = reader.ReadByte();
                        classGenders.Add(new ClassGenderData(offset, gender, i));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while reading class genders: {ex.Message}");
                    throw;
                }
            }
            else
            {
                uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(classGenderOffsetToRam, 4), 0);
                tableStartAddress = RomFile.ClassGenderExpanded ? tableStartAddress - RomFile.SynthOverlayLoadAddress : tableStartAddress - Arm9.Address;

                if (RomFile.ClassGenderExpanded)
                {
                    using FileStream fileStream = new(RomFile.SynthOverlayFilePath, FileMode.Open, FileAccess.Read);
                    using BinaryReader reader = new(fileStream);
                    fileStream.Position = tableStartAddress;

                    for (int i = 0; i < 150; i++)
                    {
                        long offset = fileStream.Position;
                        byte gender = reader.ReadByte(); // Read 1 byte for each entry
                        classGenders.Add(new ClassGenderData(offset, gender, i));
                    }
                }
                else
                {
                    using var reader = new Arm9.Arm9Reader(tableStartAddress);
                    try
                    {
                        for (int i = 0; i < numberOfClasses; i++)
                        {
                            long offset = reader.BaseStream.Position;
                            byte gender = reader.ReadByte();
                            classGenders.Add(new ClassGenderData(offset, gender, i));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while reading class genders: {ex.Message}");
                        throw;
                    }
                }
            }
            return [.. classGenders.OrderBy(x => x.TrainerClassId)];
        }

        public List<string> GetClassNames(int classNamesArchive)
        {
            var messageArchives = GetMessageArchiveContents(classNamesArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<EyeContactMusicData> GetEyeContactMusicData()
        {
            var eyeContactMusic = new List<EyeContactMusicData>();
            if (RomFile.IsHgEngine)
            {
                byte tableSize = Arm9.ReadByte(RomFile.EyeContactTableSizeOffset);
                uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(RomFile.EyeContactTablePointerOffset, 4), 0) - RomFile.HgEngineOverlayLoadAddress;
                using var overlayStream = new FileStream(Overlay.OverlayFilePath(RomFile.HgEngineOverlay), FileMode.Open, FileAccess.Read);
                overlayStream.Position = tableStartAddress;

                using var reader = new BinaryReader(overlayStream);
                try
                {
                    for (int i = 0; i < tableSize; i++)
                    {
                        uint offset = (uint)reader.BaseStream.Position;
                        ushort trainerClassId = reader.ReadUInt16();
                        ushort musicDayId = reader.ReadUInt16();
                        ushort? musicNightId = RomFile.IsHeartGoldSoulSilver ? reader.ReadUInt16() : null;

                        var eyeContactMusicData = new EyeContactMusicData(offset, trainerClassId, musicDayId, musicNightId);
                        eyeContactMusic.Add(eyeContactMusicData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading eye contact music data: {ex.Message}");
                    throw;
                }
            }
            else if (RomFile.EyeContactExpanded)
            {
                using var overlayStream = new FileStream(RomFile.SynthOverlayFilePath, FileMode.Open, FileAccess.Read);
                overlayStream.Position = RomFile.EyeContactRepointOffset;

                using var reader = new BinaryReader(overlayStream);
                try
                {
                    for (int i = 0; i < 150; i++)
                    {
                        uint offset = (uint)reader.BaseStream.Position;
                        ushort trainerClassId = reader.ReadUInt16();
                        ushort musicDayId = reader.ReadUInt16();
                        ushort? musicNightId = RomFile.IsHeartGoldSoulSilver ? reader.ReadUInt16() : null;

                        var eyeContactMusicData = new EyeContactMusicData(offset, trainerClassId, musicDayId, musicNightId);
                        eyeContactMusic.Add(eyeContactMusicData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading eye contact music data: {ex.Message}");
                    throw;
                }
            }
            else
            {
                uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(RomFile.EyeContactTablePointerOffset, 4), 0) - Arm9.Address;
                uint tableSizeOffset = (uint)(RomFile.IsHeartGoldSoulSilver ? 12 : 10);
                byte tableSize = Arm9.ReadByte(RomFile.EyeContactTableSizeOffset);
                using Arm9.Arm9Reader reader = new(tableStartAddress);

                try
                {
                    for (int i = 0; i < tableSize; i++)
                    {
                        uint offset = (uint)reader.BaseStream.Position;
                        ushort trainerClassId = reader.ReadUInt16();
                        ushort musicDayId = reader.ReadUInt16();
                        ushort? musicNightId = RomFile.IsHeartGoldSoulSilver ? reader.ReadUInt16() : null;

                        var eyeContactMusicData = new EyeContactMusicData(offset, trainerClassId, musicDayId, musicNightId);
                        eyeContactMusic.Add(eyeContactMusicData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading eye contact music data: {ex.Message}");
                    throw;
                }
            }

            return [.. eyeContactMusic.OrderBy(x => x.TrainerClassId)];
        }

        public List<string> GetItemNames(int itemNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(itemNameArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false)
        {
            var messageArchives = new List<MessageArchive>();

            try
            {
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchiveId:D4}";

                if (!File.Exists(directory))
                {
                    Console.WriteLine($"File not found: {directory}");
                    return messageArchives;
                }

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                var messages = EncryptText.ReadMessageArchive(fileStream, discardLines);

                messageArchives = new List<MessageArchive>(messages.Count);

                for (int i = 0; i < messages.Count; i++)
                {
                    messageArchives.Add(new MessageArchive(i, messages[i]));
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found exception: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO exception occurred while reading the message archive: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the message archive: {ex.Message}");
                throw;
            }

            return messageArchives;
        }

        public int GetMessageInitialKey(int messageArchive)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchive:D4}";

            try
            {
                if (!File.Exists(directory))
                {
                    Console.WriteLine($"File not found: {directory}");
                    return -1;
                }

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(fileStream);
                reader.BaseStream.Position = 2;
                int initialKey = reader.ReadUInt16();

                return initialKey;
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                return -1;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error while reading file: {ex.Message}");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the message initial key: {ex.Message}");
                return -1;
            }
        }

        public List<string> GetMoveNames(int moveTextArchive)
        {
            var messageArchives = GetMessageArchiveContents(moveTextArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public List<string> GetPokemonNames(int pokemonNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(pokemonNameArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.ConvertAll(item => item.MessageText);
        }

        public async Task<List<PrizeMoneyData>> GetPrizeMoneyDataAsync()
        {
            var prizeMoneyData = new List<PrizeMoneyData>();

            try
            {
                Console.WriteLine("Getting Prize Money table data...");
                if (RomFile.IsHgEngine)
                {
                    try
                    {
                        string filePath = Overlay.OverlayFilePath(12);

                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File not found: {filePath}");
                            return prizeMoneyData;
                        }
                        await using var overlay12 = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        using var reader1 = new BinaryReader(overlay12);
                        reader1.BaseStream.Position = RomFile.PrizeMoneyTableSizeOffset;
                        byte tableSize = reader1.ReadByte();

                        reader1.BaseStream.Position = RomFile.PrizeMoneyPointerOffset;
                        uint tableOffset = reader1.ReadUInt32() - RomFile.HgEngineOverlayLoadAddress;

                        filePath = Overlay.OverlayFilePath(RomFile.HgEngineOverlay);

                        if (!File.Exists(filePath))
                        {
                            Console.WriteLine($"File not found: {filePath}");
                            return prizeMoneyData;
                        }
                        reader1.Close();
                        overlay12.Close();

                        await using var overlayStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        using var reader2 = new BinaryReader(overlayStream);

                        reader2.BaseStream.Position = tableOffset;

                        for (int i = 0; i < tableSize; i++)
                        {
                            long offset = reader2.BaseStream.Position;
                            ushort trainerClassId = reader2.ReadUInt16();
                            ushort prizeMoney = reader2.ReadUInt16();
                            prizeMoneyData.Add(new PrizeMoneyData(offset, trainerClassId, prizeMoney));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Getting Prize Money Table failed | " + ex.Message);
                        throw;
                    }
                }
                else
                {
                    if (RomFile.IsHeartGoldSoulSilver && !RomFile.PrizeMoneyExpanded)
                    {
                        bool isCompressed = await Overlay.CheckOverlayIsCompressedAsync(RomFile.PrizeMoneyTableOverlayNumber);
                        if (isCompressed)
                        {
                            await Overlay.DecompressOverlayAsync(RomFile.PrizeMoneyTableOverlayNumber);
                            Overlay.SetOverlayCompressionInTable(RomFile.PrizeMoneyTableOverlayNumber, 0);
                        }
                    }

                    string filePath = RomFile.PrizeMoneyExpanded ? RomFile.SynthOverlayFilePath : Overlay.OverlayFilePath(RomFile.PrizeMoneyTableOverlayNumber);

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        return prizeMoneyData;
                    }

                    await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    using var reader = new BinaryReader(fileStream);
                    reader.BaseStream.Position = RomFile.PrizeMoneyTableOffset;
                    long streamEndPosition = reader.BaseStream.Position + RomFile.PrizeMoneyTableSize;
                    ushort count = 0;

                    while (reader.BaseStream.Position < streamEndPosition)
                    {
                        long offset = reader.BaseStream.Position;

                        if (RomFile.IsHeartGoldSoulSilver)
                        {
                            ushort trainerClassId = reader.ReadUInt16();
                            ushort prizeMoney = reader.ReadUInt16();
                            prizeMoneyData.Add(new PrizeMoneyData(offset, trainerClassId, prizeMoney));
                        }
                        else
                        {
                            byte prizeMoney = reader.ReadByte();
                            prizeMoneyData.Add(new PrizeMoneyData(offset, count, prizeMoney));
                            count++;
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: File not found - {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Error while reading prize money data: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing prize money data: {ex.Message}");
                throw;
            }

            return [.. prizeMoneyData.OrderBy(x => x.TrainerClassId)];
        }

        public List<Species> GetSpecies()
        {
            var allSpecies = new List<Species>();

            string unpackedDirectory = VsMakerDatabase.RomData.GameDirectories[NarcDirectory.personalPokeData].unpackedDirectory;

            if (!Directory.Exists(unpackedDirectory))
            {
                Console.WriteLine($"Directory not found: {unpackedDirectory}");
                return allSpecies;
            }

            int numberOfSpecies = Directory.GetFiles(unpackedDirectory, "*").Length;
            allSpecies.Capacity = numberOfSpecies;

            for (int i = 0; i < numberOfSpecies; i++)
            {
                string filePath = Path.Combine(unpackedDirectory, $"{i:D4}");

                try
                {
                    var species = new Species { SpeciesId = (ushort)i };

                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
                    using var reader = new BinaryReader(fileStream);

                    reader.BaseStream.Position = Species.Constants.GenderRatioByteOffset;
                    species.GenderRatio = reader.ReadByte();
                    reader.BaseStream.Position = Species.Constants.AbilitySlot1ByteOffset;
                    species.Ability1 = reader.ReadByte();
                    species.Ability2 = reader.ReadByte();

                    allSpecies.Add(species);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine($"File not found: {filePath}. Skipping. Error: {ex.Message}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"IO error while reading file {filePath}. Skipping. Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error processing file {filePath}: {ex.Message}");
                }
            }

            return allSpecies;
        }

        public int GetTotalNumberOfItemsInArchive(int archiveId)
        {
            var messageArchives = GetMessageArchiveContents(archiveId, false);

            return messageArchives?.Count ?? 0;
        }

        public int GetTotalNumberOfTrainerClasses(int trainerClassNameArchive) => GetTotalNumberOfItemsInArchive(trainerClassNameArchive);

        public List<string> GetTrainerNames()
        {
            var messageArchives = GetMessageArchiveContents(RomFile.TrainerNamesTextNumber, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives
                   .Where(item => item?.MessageText != null)
                   .Select(item => item.MessageText)
                   .ToList();
        }

        public List<TrainerData> GetTrainersData()
        {
            List<TrainerData> trainersData = [];

            for (int i = 0; i < RomFile.TotalNumberOfTrainers; i++)
            {
                try
                {
                    var trainerData = ReadTrainerData(i);
                    trainersData.Add(trainerData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while reading data for trainer {i}: {ex.Message}");
                }
            }

            return trainersData;
        }

        public List<TrainerPartyData> GetTrainersPartyData()
        {
            List<TrainerPartyData> trainersPartyData = [];

            for (int i = 0; i < RomFile.TotalNumberOfTrainers; i++)
            {
                if (i >= RomFile.TrainersData.Count)
                {
                    Console.WriteLine($"Warning: Trainer data index {i} exceeds available entries in trainerData list.");
                    break;
                }

                var trainer = RomFile.TrainersData[i];
                bool isNotDiamondPearl = RomFile.GameFamily != GameFamily.DiamondPearl;

                try
                {
                    if (i == 11)
                    {
                        bool test = true;
                    }
                    var partyData = ReadTrainerPartyData(i, trainer.TeamSize, trainer.TrainerType, isNotDiamondPearl);
                    trainersPartyData.Add(partyData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while reading party data for trainer {i}: {ex.Message}");
                    throw ex;
                }
            }

            return trainersPartyData;
        }

        #endregion Get

        #region Read

        public TrainerData ReadTrainerData(int trainerId)
        {
            var trainerData = new TrainerData();
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerProperties].unpackedDirectory}\\{trainerId:D4}";

            if (!File.Exists(directory))
            {
                throw new FileNotFoundException("The trainer data file was not found.", directory);
            }

            try
            {
                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(fileStream);

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
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine($"Unexpected end of stream while reading trainer data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred while reading trainer data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading trainer data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }

            return trainerData;
        }

        public TrainerPartyData ReadTrainerPartyData(int trainerId, byte teamSize, byte trainerType, bool hasBallCapsule)
        {
            var trainerPartyData = new TrainerPartyData
            {
                PokemonData = new TrainerPartyPokemonData[teamSize],
            };

            bool hasMoves = (trainerType & 0x01) != 0;
            bool heldItems = (trainerType & 0x02) != 0;
            bool ability = (trainerType & 0x04) != 0;
            bool chooseBall = (trainerType & 0x08) != 0;
            bool chooseIvEv = (trainerType & 0x10) != 0;
            bool chooseNature = (trainerType & 0x20) != 0;
            bool shinyLock = (trainerType & 0x40) != 0;
            bool additionalFlags = (trainerType & 0x80) != 0;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{trainerId:D4}";

            if (!File.Exists(directory))
            {
                throw new FileNotFoundException("The trainer party data file was not found.", directory);
            }

            try
            {
                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
                using var reader = new BinaryReader(fileStream);
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
                        trainerPartyPokemonData.MoveIds = new ushort[4];
                        for (int j = 0; j < 4; j++)
                        {
                            trainerPartyPokemonData.MoveIds[j] = reader.ReadUInt16();
                        }
                    }
                    if (RomFile.IsHgEngine)
                    {
                        if (ability)
                        {
                            trainerPartyPokemonData.Ability_Hge = reader.ReadUInt16();
                        }

                        if (chooseBall)
                        {
                            trainerPartyPokemonData.Ball_Hge = reader.ReadUInt16();
                        }

                        if (chooseIvEv)
                        {
                            trainerPartyPokemonData.IvNums_Hge = new byte[6];
                            trainerPartyPokemonData.EvNums_Hge = new byte[6];
                            for (int ivs = 0; ivs < 6; ivs++)
                            {
                                trainerPartyPokemonData.IvNums_Hge[ivs] = reader.ReadByte();
                            }

                            for (int evs = 0; evs < 6; evs++)
                            {
                                trainerPartyPokemonData.EvNums_Hge[evs] = reader.ReadByte();
                            }
                        }

                        if (chooseNature)
                        {
                            trainerPartyPokemonData.Nature_Hge = reader.ReadByte();
                        }

                        if (shinyLock)
                        {
                            trainerPartyPokemonData.ShinyLock_Hge = reader.ReadByte();
                        }

                        if (additionalFlags)
                        {
                            trainerPartyPokemonData.AdditionalFlags_Hge = reader.ReadUInt32();

                            // Now additional flag checks
                            bool chooseStatus = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x01) != 0;
                            bool chooseHP = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x02) != 0;
                            bool chooseATK = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x04) != 0;
                            bool chooseDEF = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x08) != 0;
                            bool chooseSPEED = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x10) != 0;
                            bool chooseSpATK = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x20) != 0;
                            bool chooseSpDEF = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x40) != 0;
                            bool chooseTypes = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x80) != 0;
                            bool choosePP = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x100) != 0;
                            bool chooseNickname = (trainerPartyPokemonData.AdditionalFlags_Hge & 0x200) != 0;

                            if (chooseStatus)
                            {
                                trainerPartyPokemonData.Status_Hge = reader.ReadUInt32();
                            }

                            if (chooseHP)
                            {
                                trainerPartyPokemonData.Hp_Hge = reader.ReadUInt16();
                            }

                            if (chooseATK)
                            {
                                trainerPartyPokemonData.Atk_Hge = reader.ReadUInt16();
                            }

                            if (chooseDEF)
                            {
                                trainerPartyPokemonData.Def_Hge = reader.ReadUInt16();
                            }

                            if (chooseSPEED)
                            {
                                trainerPartyPokemonData.Speed_Hge = reader.ReadUInt16();
                            }

                            if (chooseSpATK)
                            {
                                trainerPartyPokemonData.SpAtk_Hge = reader.ReadUInt16();
                            }

                            if (chooseSpDEF)
                            {
                                trainerPartyPokemonData.SpDef_Hge = reader.ReadUInt16();
                            }

                            if (chooseTypes)
                            {
                                trainerPartyPokemonData.Types_Hge = new byte[2];
                                for (int types = 0; types < 2; types++)
                                {
                                    trainerPartyPokemonData.Types_Hge[types] = reader.ReadByte();
                                }
                            }

                            if (choosePP)
                            {
                                trainerPartyPokemonData.PpCounts_Hge = new byte[4];
                                for (int pp = 0; pp < 4; pp++)
                                {
                                    trainerPartyPokemonData.PpCounts_Hge[pp] = reader.ReadByte();
                                }
                            }

                            if (chooseNickname)
                            {
                                trainerPartyPokemonData.Nickname_Hge = new ushort[11];
                                for (int nameChar = 0; nameChar < 11; nameChar++)
                                {
                                    trainerPartyPokemonData.Nickname_Hge[nameChar] = reader.ReadUInt16();
                                }
                            }
                        }
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
                Console.WriteLine($"Unexpected end of stream while reading trainer party data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An IO error occurred while reading trainer party data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while reading trainer party data for Trainer ID {trainerId}: {ex.Message}");
                throw;
            }

            return trainerPartyData;
        }

        #endregion Read

        #region Set

        public void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage)
        {
            Console.WriteLine("Setting up NARC directories");

            Dictionary<NarcDirectory, string> packedDirectories = gameFamily switch
            {
                GameFamily.DiamondPearl => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.monIcons] = @"data\poketool\icongra\poke_icon.narc",
                    [NarcDirectory.moveData] = @"data\poketool\waza\waza_tbl.narc",
                    [NarcDirectory.personalPokeData] = gameVersion == GameVersion.Pearl
                                           ? @"data\poketool\personal_pearl\personal.narc"
                                           : @"data\poketool\personal\personal.narc",
                    [NarcDirectory.eventFiles] = !RomFile.GameLanguage.Equals(GameLanguage.Japanese)
                                           ? @"data\fielddata\eventdata\zone_event_release.narc"
                                           : @"data\fielddata\eventdata\zone_event.narc",
                    [NarcDirectory.scripts] = !RomFile.GameLanguage.Equals(GameLanguage.Japanese)
                                           ? @"data\fielddata\script\scr_seq_release.narc"
                                           : @"data\fielddata\script\scr_seq.narc",
                    [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                    [NarcDirectory.textArchives] = @"data\msgdata\msg.narc",
                    [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                    [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                    [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                    [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                    [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc"
                },
                GameFamily.Platinum => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.monIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                    [NarcDirectory.moveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                    [NarcDirectory.personalPokeData] = @"data\poketool\personal\pl_personal.narc",
                    [NarcDirectory.eventFiles] = @"data\fielddata\eventdata\zone_event.narc",
                    [NarcDirectory.scripts] = @"data\fielddata\script\scr_seq.narc",
                    [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                    [NarcDirectory.textArchives] = @"data\msgdata\" + gameVersion.ToString()[..2].ToLower() + '_' + "msg.narc",
                    [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                    [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                    [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                    [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                    [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc",
                },
                GameFamily.HeartGoldSoulSilver => new Dictionary<NarcDirectory, string>
                {
                    [NarcDirectory.battleStagePokeData] = @"data\a\2\0\4",
                    [NarcDirectory.battleTowerPokeData] = @"data\a\2\0\3",
                    [NarcDirectory.battleTowerTrainerData] = @"data\a\2\0\2",
                    [NarcDirectory.monIcons] = @"data\a\0\2\0",
                    [NarcDirectory.eventFiles] = @"data\a\0\3\2",
                    [NarcDirectory.moveData] = @"data\a\0\1\1",
                    [NarcDirectory.personalPokeData] = @"data\a\0\0\2",
                    [NarcDirectory.scripts] = @"data\a\0\1\2",
                    [NarcDirectory.synthOverlay] = @"data\a\0\2\8",
                    [NarcDirectory.textArchives] = @"data\a\0\2\7",
                    [NarcDirectory.trainerGraphics] = @"data\a\0\5\8",
                    [NarcDirectory.trainerParty] = @"data\a\0\5\6",
                    [NarcDirectory.trainerProperties] = @"data\a\0\5\5",
                    [NarcDirectory.trainerTextOffset] = @"data\a\1\3\1",
                    [NarcDirectory.trainerTextTable] = @"data\a\0\5\7",
                },
                _ => throw new ArgumentException($"Unrecognized GameFamily: {gameFamily}")
            };

            var directories = new Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)>(packedDirectories.Count);
            foreach (var kvp in packedDirectories)
            {
                directories.Add(kvp.Key, (
                    Path.Combine(workingDirectory, kvp.Value),
                    Path.Combine(workingDirectory, "unpacked", kvp.Key.ToString())
                ));
            }

            VsMakerDatabase.RomData.GameDirectories = directories;
            Console.WriteLine("Setting up NARC directories | Success");
        }

        public int SetTrainerNameMax(int trainerNameOffset)
        {
            if (trainerNameOffset <= 0)
            {
                return 8;
            }

            try
            {
                using var ar = new Arm9.Arm9Reader(trainerNameOffset);
                if (ar.BaseStream.Length == 0)
                {
                    Console.WriteLine("Stream length is zero, cannot read trainer name length.");
                    return 8;
                }

                return ar.ReadByte();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer name length: {ex.Message}");
                return 8;
            }
        }

        #endregion Set

        #region Unpack

        public async Task<(bool Success, string ExceptionMessage)> UnpackNarcsAsync(List<NarcDirectory> narcs, IProgress<int> progress)
        {
            if (narcs.Count == 0)
            {
                progress?.Report(100);
                return (true, string.Empty);
            }

            double progressStep = 100.0 / narcs.Count;
            double currentProgress = 0;

            foreach (var narc in narcs)
            {
                var (success, exceptionMessage) = await UnpackNarcAsync(narc);
                if (!success)
                {
                    progress?.Report(100);
                    return (false, exceptionMessage);
                }

                currentProgress += progressStep;
                progress?.Report((int)currentProgress);
            }

            progress?.Report(100);
            return (true, string.Empty);
        }

        private static async Task<(bool Success, string ExceptionMessage)> UnpackNarcAsync(NarcDirectory narcPath)
        {
            try
            {
                if (!VsMakerDatabase.RomData.GameDirectories.TryGetValue(narcPath, out (string packedPath, string unpackedPath) paths))
                {
                    return (false, $"NARC directory not found in dictionary: {narcPath}");
                }

                DirectoryInfo directoryInfo = new(paths.unpackedPath);

                if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0)
                {
                    Narc openedNarc = await Narc.OpenAsync(paths.packedPath);
                    if (openedNarc == null)
                    {
                        return (false, $"Failed to open NARC at path: {paths.packedPath}");
                    }

                    await openedNarc.ExtractToFolderAsync(paths.unpackedPath);

                    if (Directory.GetFiles(paths.unpackedPath).Length == 0)
                    {
                        return (false, $"Extraction failed for NARC at path: {paths.packedPath}");
                    }
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, $"Exception occurred: {ex.Message}");
            }
        }

        #endregion Unpack

        #region Repack

        public async Task RepackRomAsync(string ndsFileName)
        {
            using Process repack = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"Tools\ndstool.exe",
                    Arguments = $"-c \"{ndsFileName}\" -9 \"{RomFile.Arm9Path}\" -7 \"{Path.Combine(RomFile.WorkingDirectory, "arm7.bin")}\" " +
                                $"-y9 \"{Path.Combine(RomFile.WorkingDirectory, "y9.bin")}\" -y7 \"{Path.Combine(RomFile.WorkingDirectory, "y7.bin")}\" " +
                                $"-d \"{Path.Combine(RomFile.WorkingDirectory, "data")}\" -y \"{Path.Combine(RomFile.WorkingDirectory, "overlay")}\" " +
                                $"-t \"{Path.Combine(RomFile.WorkingDirectory, "banner.bin")}\" -h \"{Path.Combine(RomFile.WorkingDirectory, "header.bin")}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            try
            {
                repack.Start();
                await repack.WaitForExitAsync();

                if (repack.ExitCode != 0)
                {
                    throw new InvalidOperationException($"Repack process failed with exit code {repack.ExitCode}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during repacking: {ex.Message}");
                throw;
            }
            finally
            {
                if (!repack.HasExited)
                {
                    repack.Kill();
                }
            }
        }

        #endregion Repack
    }
}