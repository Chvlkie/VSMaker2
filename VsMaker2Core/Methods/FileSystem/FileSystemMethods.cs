using MsgPack.Serialization;
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
            return WriteMessage(trainerNames, trainerNamesArchive, true);
        }

        public (bool Success, string ErrorMessage) WriteMessage(List<string> messages, int messageArchive, bool isTrainerName = false)
        {
            try
            {
                EncryptText.WriteMessageArchive(messageArchive, messages, isTrainerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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

        public (bool Success, string ErrorMessage) WriteClassDescription(List<string> descriptions, int classId, string newDescription, int classDescriptionMessageNumber)
        {
            descriptions[classId] = newDescription;
            return WriteMessage(descriptions, classDescriptionMessageNumber);
        }

        public (bool Success, string ErrorMessage) WriteClassGenderData(ClassGenderData classGenderData)
        {
            try
            {
                Arm9.WriteByte(classGenderData.Gender, (uint)classGenderData.Offset);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, ex.Message);
            }
            return (true, "");
        }

        public (bool Success, string ErrorMessage) WriteEyeContactMusicData(EyeContactMusicData eyeContactMusicData, RomFile loadedRom)
        {
            try
            {
                if (loadedRom.IsHeartGoldSoulSilver)
                {
                    Arm9.WriteBytes(BitConverter.GetBytes(eyeContactMusicData.MusicDayId), eyeContactMusicData.Offset + 2);
                    Arm9.WriteBytes(BitConverter.GetBytes(eyeContactMusicData.MusicNightId ?? 0), eyeContactMusicData.Offset + 4);
                }
                else
                {
                    Arm9.WriteBytes(BitConverter.GetBytes(eyeContactMusicData.MusicDayId), eyeContactMusicData.Offset + 2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return (false, ex.Message);
            }
            return (true, "");
        }

        public (bool Success, string ErrorMessage) WritePrizeMoneyData(PrizeMoneyData prizeMoneyData, RomFile loadedRom)
        {
            if (loadedRom.IsHeartGoldSoulSilver)
            {
                if (Overlay.CheckOverlayIsCompressed(loadedRom.PrizeMoneyTableOverlayNumber))
                {
                    Overlay.DecompressOverlay(loadedRom.PrizeMoneyTableOverlayNumber);
                    Overlay.SetOverlayCompressionInTable(loadedRom.PrizeMoneyTableOverlayNumber, 0);
                }
                using EasyWriter writer = new(Overlay.OverlayFilePath(loadedRom.PrizeMoneyTableOverlayNumber), prizeMoneyData.Offset);
                try
                {
                    writer.Write(prizeMoneyData.TrainerClassId);
                    writer.Write(prizeMoneyData.PrizeMoney);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    writer.Close();
                    return (false, ex.Message);
                }
            }
            else
            {
                using EasyWriter writer = new(RomFile.OverlayPath, prizeMoneyData.Offset);
                try
                {
                    writer.Write((byte)prizeMoneyData.PrizeMoney);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    writer.Close();
                    return (false, ex.Message);
                }
            }
            return (true, "");
        }

        #endregion Write

        #region Delete

        public (bool Success, string ErrorMessage) RemoveTrainer(int trainerId)
        {
            string trainerPropertiesDirectory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerProperties].unpackedDirectory}\\{trainerId:D4}";
            string trainerPartyDirectory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerParty].unpackedDirectory}\\{trainerId:D4}";
            return (true, "");
        }

        #endregion Delete

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

        private List<byte[]> GetAllTrainerPartyFiles(int trainerCount)
        {
            List<byte[]> trainerFiles = [];

            for (int i = 0; i < trainerCount; i++)
            {
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerParty].unpackedDirectory}\\{i:D4}";
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
                string directory = $"{VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TrainerProperties].unpackedDirectory}\\{i:D4}";
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