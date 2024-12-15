using System.Windows.Forms;
using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public class ClassEditorMethods : IClassEditorMethods
    {
        private IRomFileMethods romFileMethods;

        public ClassEditorMethods(IRomFileMethods romFileMethods)
        {
            romFileMethods = new RomFileMethods();
            this.romFileMethods = romFileMethods;
        }

        public TrainerClass GetTrainerClass(List<TrainerClass> trainerClasses, int trainerClassId)
        {
            var trainerClass = trainerClasses.SingleOrDefault(x => x.TrainerClassId == trainerClassId);

            if (trainerClass == null)
            {
                Console.WriteLine($"Trainer class with ID {trainerClassId} not found.");
            }

            return trainerClass;
        }

        public List<TrainerClass> GetTrainerClasses(List<Trainer> trainers, List<string> trainerClassNames, List<string> trainerClassDescriptions)
        {
            List<TrainerClass> trainerClasses = [];

            for (int i = 0; i < trainerClassNames.Count; i++)
            {
                Console.WriteLine($"Getting data from Trainer Class: {trainerClassNames[i]}");
                EyeContactMusicData eyeContactData;
                try
                {
                    Console.WriteLine("Checking Eye-contact table...");
                    eyeContactData = RomFile.EyeContactMusicData.SingleOrDefault(x => x.TrainerClassId == i);
                    Console.WriteLine("Eye-contact table checked.");
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("More than 1 entry for Trainer Class\nin Eye-Contact table found!", "Multiple Entries", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.Write(ex.Message);
                    throw;
                }

                var gender = (RomFile.IsNotDiamondPearl && i < RomFile.ClassGenderData.Count)
                    ? (byte?)RomFile.ClassGenderData[i].Gender : null;

                var prizeMoneyMultiplier = (i < RomFile.PrizeMoneyData.Count)
                    ? RomFile.PrizeMoneyData[i].PrizeMoney
                    : 0;

                var trainerClass = new TrainerClass
                {
                    TrainerClassId = i,
                    TrainerClassName = trainerClassNames[i],
                    ClassProperties = new TrainerClassProperty
                    {
                        Description = trainerClassDescriptions[i],
                        Gender = gender,
                        EyeContactMusicDay = eyeContactData?.MusicDayId ?? -1,
                        EyeContactMusicNight = RomFile.IsHeartGoldSoulSilver
                            ? eyeContactData?.MusicNightId ?? -1 : null,
                        PrizeMoneyMultiplier = prizeMoneyMultiplier
                    },
                    UsedByTrainers = GetUsedByTrainers(i, trainers),
                };

                trainerClasses.Add(trainerClass);
            }

            return trainerClasses;
        }

        public List<Trainer> GetUsedByTrainers(int trainerClassId, List<Trainer> trainers) => trainers == null ? (List<Trainer>)([]) : trainers.Where(x => x.TrainerProperties.TrainerClassId == trainerClassId).ToList();
    }
}