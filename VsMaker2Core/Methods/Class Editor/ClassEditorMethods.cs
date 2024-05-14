using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public class ClassEditorMethods : IClassEditorMethods
    {
        private IRomFileMethods romFileMethods;

        public ClassEditorMethods()
        {
            romFileMethods = new RomFileMethods();
        }

        public TrainerClass GetTrainerClass(List<TrainerClass> classes, int classId)
        {
            return classes.SingleOrDefault(x => x.TrainerClassId == classId);
        }


        public List<TrainerClass> GetTrainerClasses(List<Trainer> trainers, List<string> classNames, List<string> classDescriptions, RomFile loadedRom)
        {
            List<TrainerClass> trainerClasses = [];
            // Start from i 2 to skip player classes
            for (int i = 2; i < classNames.Count; i++)
            {
                var eyeContactData = loadedRom.EyeContactMusicData.SingleOrDefault(x => x.TrainerClassId == i);
                var trainerClass = new TrainerClass
                {
                    TrainerClassId = i,
                    TrainerClassName = classNames[i],
                    ClassProperties = new TrainerClassProperty
                    {
                        Description = classDescriptions[i],
                        Gender = loadedRom.ClassGenderData[i].Gender,
                        EyeContactMusicDay = eyeContactData != default ? eyeContactData.MusicDayId : -1,
                        EyeContactMusicNight = eyeContactData != default ? eyeContactData.MusicNightId : null,
                        PrizeMoneyMultiplier = loadedRom.PrizeMoneyData[i].PrizeMoney
                    },
                    UsedByTrainers = GetUsedByTrainers(i, trainers),
                };
                trainerClasses.Add(trainerClass);
            }
            return trainerClasses;
        }

        public List<Trainer> GetUsedByTrainers(int classId, List<Trainer> trainers)
        {
            return trainers.Where(x => x.TrainerProperties.TrainerClassId == classId).ToList();
        }
    }
}