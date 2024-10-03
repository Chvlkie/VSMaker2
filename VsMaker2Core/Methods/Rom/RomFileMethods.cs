using System.Diagnostics;
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

            return messageArchives.Select(item => item.MessageText).ToList();
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

                using BinaryReader reader = new(new FileStream(battleMessageOffsetPath, FileMode.Open, FileAccess.Read));
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

            return messageArchives.Select(item => item.MessageText).ToList();
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

            return messageArchives.Select(item => item.MessageText).ToList();
        }

        public List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam)
        {
            if (numberOfClasses <= 0)
            {
                Console.WriteLine("Invalid number of classes.");
                return [];
            }

            uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(classGenderOffsetToRam, 4), 0) - Arm9.Address;

            var classGenders = new List<ClassGenderData>(numberOfClasses);

            using (var reader = new Arm9.Arm9Reader(tableStartAddress))
            {
                try
                {
                    for (int i = 0; i < numberOfClasses; i++)
                    {
                        byte gender = reader.ReadByte();
                        classGenders.Add(new ClassGenderData(reader.BaseStream.Position, gender, i));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while reading class genders: {ex.Message}");
                    throw;
                }
            }

            return classGenders;
        }

        public List<string> GetClassNames(int classNamesArchive)
        {
            var messageArchives = GetMessageArchiveContents(classNamesArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.Select(item => item.MessageText).ToList();
        }


        public List<EyeContactMusicData> GetEyeContactMusicData(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily)
        {
            var eyeContactMusic = new List<EyeContactMusicData>();

            uint tableStartAddress = BitConverter.ToUInt32(Arm9.ReadBytes(eyeContactMusicTableOffsetToRam, 4), 0) - Arm9.Address;
            uint tableSizeOffset = (uint)(gameFamily == GameFamily.HeartGoldSoulSilver || gameFamily == GameFamily.HgEngine ? 12 : 10);

            byte tableSize = Arm9.ReadByte(eyeContactMusicTableOffsetToRam - tableSizeOffset);

            using Arm9.Arm9Reader reader = new(tableStartAddress);

            try
            {
                for (int i = 0; i < tableSize; i++)
                {
                    uint offset = (uint)reader.BaseStream.Position;
                    ushort trainerClassId = reader.ReadUInt16();
                    ushort musicDayId = reader.ReadUInt16();
                    ushort? musicNightId = null;

                    if (gameFamily == GameFamily.HgEngine || gameFamily == GameFamily.HeartGoldSoulSilver)
                    {
                        musicNightId = reader.ReadUInt16();
                    }

                    var eyeContactMusicData = new EyeContactMusicData(offset, trainerClassId, musicDayId, musicNightId);
                    eyeContactMusic.Add(eyeContactMusicData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading eye contact music data: {ex.Message}");
                throw;
            }

            return eyeContactMusic;
        }


        public List<string> GetItemNames(int itemNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(itemNameArchive, false);

            if (messageArchives == null || messageArchives.Count == 0)
            {
                return [];
            }

            return messageArchives.Select(item => item.MessageText).ToList();
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

                using var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
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
            // Construct file path
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchive:D4}";

            try
            {
                // Open file stream with using statement for automatic disposal
                using (var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fileStream))
                {
                    // Set the position and read the initial key
                    reader.BaseStream.Position = 2;
                    int initialKey = reader.ReadUInt16();

                    return initialKey;
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"An error occurred while reading the message initial key: {ex.Message}");
                throw; // Re-throw the exception if necessary
            }
        }

        public List<string> GetMoveNames(int moveTextArchive)
        {
            var messageArchives = GetMessageArchiveContents(moveTextArchive, false);

            if (messageArchives == null)
            {
                // Handle the case where messageArchives is null, if necessary
                throw new InvalidOperationException("Message archives cannot be null.");
            }

            // Use LINQ to project MessageText from each MessageArchive
            var moveNames = messageArchives.Select(item => item.MessageText).ToList();

            return moveNames;
        }

        public List<string> GetPokemonNames(int pokemonNameArchive)
        {
            var messageArchives = GetMessageArchiveContents(pokemonNameArchive, false);

            if (messageArchives == null)
            {
                // Handle the case where messageArchives is null, if necessary
                throw new InvalidOperationException("Message archives cannot be null.");
            }

            // Use LINQ to project MessageText from each MessageArchive
            var pokemonNames = messageArchives.Select(item => item.MessageText).ToList();

            return pokemonNames;
        }

        public List<PrizeMoneyData> GetPrizeMoneyData(RomFile loadedRom)
        {
            var prizeMoneyData = new List<PrizeMoneyData>();

            // Check if the overlay needs to be decompressed
            if (loadedRom.IsHeartGoldSoulSilver &&
                Overlay.CheckOverlayIsCompressed(loadedRom.PrizeMoneyTableOverlayNumber))
            {
                Overlay.DecompressOverlay(loadedRom.PrizeMoneyTableOverlayNumber);
                Overlay.SetOverlayCompressionInTable(loadedRom.PrizeMoneyTableOverlayNumber, 0);
            }

            string filePath = Overlay.OverlayFilePath(loadedRom.PrizeMoneyTableOverlayNumber);

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fileStream))
            {
                try
                {
                    reader.BaseStream.Position = loadedRom.PrizeMoneyTableOffset;
                    long streamSize = reader.BaseStream.Position + loadedRom.PrizeMoneyTableSize;
                    ushort count = 0;

                    while (reader.BaseStream.Position <= streamSize)
                    {
                        long offset = reader.BaseStream.Position;

                        if (loadedRom.IsHeartGoldSoulSilver)
                        {
                            ushort trainerClassId = reader.ReadUInt16();
                            ushort prizeMoney = reader.ReadUInt16();
                            var item = new PrizeMoneyData(offset, trainerClassId, prizeMoney);
                            prizeMoneyData.Add(item);
                        }
                        else
                        {
                            byte prizeMoney = reader.ReadByte();
                            var item = new PrizeMoneyData(offset, count, prizeMoney);
                            prizeMoneyData.Add(item);
                            count++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw; // Rethrow exception to maintain stack trace
                }
            }

            return prizeMoneyData;
        }

        public List<Species> GetSpecies()
        {
            var allSpecies = new List<Species>();
            string unpackedDirectory = VsMakerDatabase.RomData.GameDirectories[NarcDirectory.personalPokeData].unpackedDirectory;

            // Get the number of species based on the number of files in the directory
            int numberOfSpecies = Directory.GetFiles(unpackedDirectory, "*").Length;

            for (int i = 0; i < numberOfSpecies; i++)
            {
                string filePath = Path.Combine(unpackedDirectory, $"{i:D4}");

                try
                {
                    var species = new Species { SpeciesId = (ushort)i };

                    // Use using statements to ensure resources are disposed of properly
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (var reader = new BinaryReader(fileStream))
                    {
                        reader.BaseStream.Position = Species.Constants.GenderRatioByteOffset;
                        species.GenderRatio = reader.ReadByte();
                        reader.BaseStream.Position = Species.Constants.AbilitySlot1ByteOffset;
                        species.Ability1 = reader.ReadByte();
                        species.Ability2 = reader.ReadByte();
                    }

                    allSpecies.Add(species);
                }
                catch (Exception ex) // General exception handling, consider handling specific exceptions
                {
                    Console.WriteLine($"Error processing file {i:D4}: {ex.Message}");
                    // Optionally, you can choose to continue processing other files even if one fails
                }
            }
            return allSpecies;
        }

        public int GetTotalNumberOfTrainerClassess(int trainerClassNameArchive)
        {
            return GetMessageArchiveContents(trainerClassNameArchive, false).Count;
        }

        public int GetTotalNumberOfTrainers(int trainerNameArchive)
        {
            return GetMessageArchiveContents(trainerNameArchive, false).Count;
        }

        public List<string> GetTrainerNames(int trainerNameMessageArchive)
        {
            var messageArchives = GetMessageArchiveContents(trainerNameMessageArchive, false);

            // Handle potential null values
            if (messageArchives == null)
            {
                throw new InvalidOperationException("Failed to retrieve message archives.");
            }

            // Initialize the list and add the message texts
            var trainerNames = new List<string>(messageArchives.Count);

            foreach (var item in messageArchives)
            {
                if (item?.MessageText != null) // Check for null MessageText if needed
                {
                    trainerNames.Add(item.MessageText);
                }
            }

            return trainerNames;
        }

        public List<TrainerData> GetTrainersData(int numberOfTrainers)
        {
            var trainersData = new List<TrainerData>(numberOfTrainers);

            try
            {
                for (int i = 0; i < numberOfTrainers; i++)
                {
                    trainersData.Add(ReadTrainerData(i));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer data: {ex.Message}");
                // Handle the exception or rethrow as needed
                throw;
            }

            return trainersData;
        }

        public List<TrainerPartyData> GetTrainersPartyData(int numberOfTrainers, List<TrainerData> trainerData, GameFamily gameFamily)
        {
            var trainersPartyData = new List<TrainerPartyData>(numberOfTrainers);

            try
            {
                for (int i = 0; i < numberOfTrainers; i++)
                {
                    // Ensure trainerData[i] is valid
                    if (i >= trainerData.Count)
                    {
                        throw new IndexOutOfRangeException($"Index {i} is out of range for trainerData list.");
                    }

                    var trainer = trainerData[i];
                    bool isNotDiamondPearl = gameFamily != GameFamily.DiamondPearl;
                    trainersPartyData.Add(ReadTrainerPartyData(i, trainer.TeamSize, trainer.TrainerType, isNotDiamondPearl));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer party data: {ex.Message}");
                // Handle the exception or rethrow as needed
                throw;
            }

            return trainersPartyData;
        }

        public int SetTrainerNameMax(int trainerNameOffset)
        {
            if (trainerNameOffset <= 0)
            {
                return 8;
            }

            try
            {
                using (var ar = new Arm9.Arm9Reader(trainerNameOffset))
                {
                    // Ensure that reading from the stream is safe
                    if (ar.BaseStream.Length > 0)
                    {
                        int trainerNameLength = ar.ReadByte();
                        return trainerNameLength;
                    }
                    else
                    {
                        throw new InvalidOperationException("Stream length is zero, cannot read trainer name length.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer name length: {ex.Message}");
                return 0; // Default value or rethrow
            }
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
                Console.WriteLine($"Unexpected end of stream while reading trainer data: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An IO error occurred while reading trainer data: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer data: {ex.Message}");
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

            bool hasMoves = (trainerType & 1) != 0;
            bool heldItems = (trainerType & 2) != 0;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{trainerId:D4}";

            if (!File.Exists(directory))
            {
                throw new FileNotFoundException("The trainer party data file was not found.", directory);
            }

            try
            {
                using (var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read))
                using (var reader = new BinaryReader(fileStream))
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
                            trainerPartyPokemonData.MoveIds = new ushort[4]
                            {
                        reader.ReadUInt16(),
                        reader.ReadUInt16(),
                        reader.ReadUInt16(),
                        reader.ReadUInt16(),
                            };
                        }

                        if (hasBallCapsule)
                        {
                            trainerPartyPokemonData.BallCapsule = reader.ReadUInt16();
                        }

                        trainerPartyData.PokemonData[i] = trainerPartyPokemonData;
                    }
                }
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine($"Unexpected end of stream while reading trainer party data: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An IO error occurred while reading trainer party data: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading trainer party data: {ex.Message}");
                throw;
            }

            return trainerPartyData;
        }

        #endregion Read

        #region Set

        public void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage)
        {
            Dictionary<NarcDirectory, string> packedDirectories = new();

            switch (gameFamily)
            {
                case GameFamily.DiamondPearl:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.monIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [NarcDirectory.moveData] = @"data\poketool\waza\waza_tbl.narc",
                        [NarcDirectory.personalPokeData] = gameVersion == GameVersion.Pearl
                            ? @"data\poketool\personal_pearl\personal.narc"
                            : @"data\poketool\personal\personal.narc",
                        [NarcDirectory.scripts] = gameLanguage == GameLanguage.Japanese
                            ? @"data\fielddata\script\scr_seq_release.narc"
                            : @"data\fielddata\script\scr_seq.narc",
                        [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.textArchives] = @"data\msgdata\msg.narc",
                        [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc",
                    };
                    break;

                case GameFamily.Platinum:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.monIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                        [NarcDirectory.moveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                        [NarcDirectory.personalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.scripts] = @"data\fielddata\script\scr_seq.narc",
                        [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.textArchives] = Path.Combine(
                            @"data\msgdata",
                            $"{gameVersion.ToString().Substring(0, 2).ToLower()}_msg.narc"),
                        [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc",
                    };
                    break;

                case GameFamily.HeartGoldSoulSilver:
                case GameFamily.HgEngine:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.battleStagePokeData] = @"data\a\2\0\4",
                        [NarcDirectory.battleTowerPokeData] = @"data\a\2\0\3",
                        [NarcDirectory.battleTowerTrainerData] = @"data\a\2\0\2",
                        [NarcDirectory.monIcons] = @"data\a\0\2\0",
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
                    };
                    break;

                default:
                    throw new ArgumentException($"Unrecognized GameFamily: {gameFamily}");
            }

            var directories = new Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)>();
            foreach (var kvp in packedDirectories)
            {
                directories.Add(kvp.Key, (
                    Path.Combine(workingDirectory, kvp.Value),
                    Path.Combine(workingDirectory, "unpacked", kvp.Key.ToString())
                ));
            }

            VsMakerDatabase.RomData.GameDirectories = directories;
        }

        #endregion Set

        #region Unpack

        public async Task<(bool Success, string ExceptionMessage)> UnpackNarcsAsync(List<NarcDirectory> narcs, IProgress<int> progress)
        {
            if (narcs.Count == 0)
            {
                progress?.Report(100); // Report 100% if there are no narcs to process
                return (true, string.Empty);
            }

            int progressStep = 100 / narcs.Count;
            int count = 0;

            foreach (var narc in narcs)
            {
                var (success, exceptionMessage) = await UnpackNarcAsync(narc);
                if (!success)
                {
                    progress?.Report(100);
                    return (false, exceptionMessage);
                }

                count += progressStep;
                progress?.Report(count);
            }

            progress?.Report(100); // Ensure we report 100% completion at the end
            return (true, string.Empty);
        }

        private async Task<(bool Success, string ExceptionMessage)> UnpackNarcAsync(NarcDirectory narcPath)
        {
            try
            {
                if (VsMakerDatabase.RomData.GameDirectories.TryGetValue(narcPath, out (string packedPath, string unpackedPath) paths))
                {
                    DirectoryInfo directoryInfo = new(paths.unpackedPath);
                    if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0)
                    {
                        Narc openedNarc = await Narc.OpenAsync(paths.packedPath);
                        if (openedNarc == null)
                        {
                            return (false, $"Failed to open NARC at path: {paths.packedPath}");
                        }
                        else
                        {
                            await openedNarc.ExtractToFolderAsync(paths.unpackedPath);
                            // Optional: Validate if extraction was successful
                            if (!Directory.GetFiles(paths.unpackedPath).Any())
                            {
                                return (false, $"Extraction failed for NARC at path: {paths.packedPath}");
                            }
                        }
                    }
                    return (true, string.Empty);
                }
                else
                {
                    return (false, $"NARC directory not found in dictionary: {narcPath}");
                }
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
            Process repack = new()
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

            var tcs = new TaskCompletionSource<bool>();
            repack.Exited += (sender, args) =>
            {
                tcs.SetResult(repack.ExitCode == 0);
                repack.Dispose();
            };

            try
            {
                repack.Start();
                bool success = await tcs.Task;
                if (!success)
                {
                    throw new InvalidOperationException($"Repack process failed with exit code {repack.ExitCode}.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log or rethrow)
                Console.WriteLine($"An error occurred during repacking: {ex.Message}");
                throw;
            }
            finally
            {
                if (!repack.HasExited)
                {
                    repack.Kill();
                }
                repack.Dispose();
            }
        }

        #endregion Repack
    }
}