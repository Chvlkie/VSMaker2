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

        public (bool Success, string ExceptionMessage) ExtractRomContents(string workingDirectory, string fileName)
        {
            Process unpack = new();
            unpack.StartInfo.FileName = Common.NdsToolsFilePath;
            unpack.StartInfo.Arguments = "-x " + '"' + fileName + '"'
                + " -9 " + '"' + RomFile.Arm9Path + '"'
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
            catch (Exception ex)
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

        public List<BattleMessageOffsetData> GetBattleMessageOffsetData(string battleMessageOffsetPath)
        {
            List<BattleMessageOffsetData> battleMessageOffsetData = [];
            using BinaryReader reader = new(new FileStream(battleMessageOffsetPath, FileMode.Open, FileAccess.Read));
            try
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    uint offset = reader.ReadUInt16();
                    battleMessageOffsetData.Add(new BattleMessageOffsetData(BattleMessageOffsetData.OffsetToMessageId(offset)));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                throw;
            }
            return battleMessageOffsetData;
        }

        public List<string> GetBattleMessages(int battleMessageArchive)
        {
            var messageArchives = GetMessageArchiveContents(battleMessageArchive, false);
            var trainerNames = new List<string>();
            foreach (var item in messageArchives)
            {
                trainerNames.Add(item.MessageText);
            }
            return trainerNames;
        }

        public List<BattleMessageTableData> GetBattleMessageTableData(string trainerTextTablePath)
        {
            List<BattleMessageTableData> trainerTextTableDatas = [];
            using BinaryReader reader = new(new FileStream(trainerTextTablePath, FileMode.Open, FileAccess.Read));
            int messageId = 0;
            try
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    uint trainerId = reader.ReadUInt16();
                    ushort messageTriggerId = reader.ReadUInt16();
                    trainerTextTableDatas.Add(new BattleMessageTableData(messageId, trainerId, messageTriggerId));
                    messageId++;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                throw;
            }
            return trainerTextTableDatas;
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

        public List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam)
        {
            uint test = BitConverter.ToUInt32(Arm9.ReadBytes(classGenderOffsetToRam, 4), 0);

            uint tableStartAddress = test - Arm9.Address;

            List<ClassGenderData> classGenders = [];
            using Arm9.Arm9Reader reader = new(tableStartAddress);
            try
            {
                for (int i = 0; i < numberOfClasses; i++)
                {
                    long offset = reader.BaseStream.Position;
                    byte gender = reader.ReadByte();
                    int trainerClassId = i;
                    classGenders.Add(new ClassGenderData(offset, gender, trainerClassId));
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

        public List<EyeContactMusicData> GetEyeContactMusicData(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily)
        {
            List<EyeContactMusicData> eyeContactMusic = [];
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

        public List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchiveId:D4}";
            var fileStream = new FileStream(directory, FileMode.Open, FileAccess.Read);
            var messages = EncryptText.ReadMessageArchive(fileStream, discardLines);
            List<MessageArchive> messageArchives = [];

            for (int i = 0; i < messages.Count; i++)
            {
                messageArchives.Add(new MessageArchive(i, messages[i]));
            }
            return messageArchives;
        }

        public int GetMessageInitialKey(int messageArchive)
        {
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.textArchives].unpackedDirectory}\\{messageArchive:D4}";
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

        public List<PrizeMoneyData> GetPrizeMoneyData(RomFile loadedRom)
        {
            List<PrizeMoneyData> prizeMoneyData = [];
            if ((loadedRom.IsHeartGoldSoulSilver)
                && Overlay.CheckOverlayIsCompressed(loadedRom.PrizeMoneyTableOverlayNumber))
            {
                Overlay.DecompressOverlay(loadedRom.PrizeMoneyTableOverlayNumber);
                Overlay.SetOverlayCompressionInTable(loadedRom.PrizeMoneyTableOverlayNumber, 0);
            }
            string filePath = Overlay.OverlayFilePath(loadedRom.PrizeMoneyTableOverlayNumber);
            using BinaryReader reader = new(new FileStream(filePath, FileMode.Open));
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
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                reader.Close();
                throw;
            }
            return prizeMoneyData;
        }

        public List<Species> GetSpecies()
        {
            List<Species> allSpecies = [];
            int numberOfSpecies = Directory.GetFiles(VsMakerDatabase.RomData.GameDirectories[NarcDirectory.personalPokeData].unpackedDirectory, "*").Length;

            for (int i = 0; i < numberOfSpecies; i++)
            {
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.personalPokeData].unpackedDirectory}\\{i:D4}";

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
        #endregion Get

        #region Read

        public TrainerData ReadTrainerData(int trainerId)
        {
            var trainerData = new TrainerData();
            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerProperties].unpackedDirectory}\\{trainerId:D4}";
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
                PokemonData = new TrainerPartyPokemonData[teamSize],
            };

            bool hasMoves = (trainerType & 1) != 0;
            bool heldItems = (trainerType & 2) != 0;

            string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerParty].unpackedDirectory}\\{trainerId:D4}";
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
                        [NarcDirectory.monIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [NarcDirectory.moveData] = @"data\poketool\waza\waza_tbl.narc",
                        [NarcDirectory.personalPokeData] = gameVersion == GameVersion.Pearl ? @"data\poketool\personal_pearl\personal.narc" : @"data\poketool\personal\personal.narc",
                        [NarcDirectory.scripts] = gameLanguage == GameLanguage.Japanese ? @"data\fielddata\script\scr_seq_release.narc" : @"data\fielddata\script\scr_seq.narc",
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
                        [NarcDirectory.textArchives] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",
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
    }
}