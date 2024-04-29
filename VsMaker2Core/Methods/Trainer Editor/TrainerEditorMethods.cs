using System.Diagnostics;
using System.Net;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public class TrainerEditorMethods : ITrainerEditorMethods
    {
        private IRomFileMethods romFileMethods;

        public TrainerEditorMethods()
        {
            romFileMethods = new RomFileMethods();
        }

        public List<Trainer> GetTrainers(int trainerMessageArchive, GameFamily gameFamily, bool partyReadFirstByte = false)
        {
            var trainerNames = romFileMethods.GetTrainerNames(trainerMessageArchive);
            List<Trainer> trainers = [];
            // Start from i 1 to skip player trainer file
            for (int i = 1; i < trainerNames.Count; i++)
            {
                trainers.Add(romFileMethods.GetTrainerDataByTrainerId(i, trainerNames[i], gameFamily, partyReadFirstByte));
            }
            return trainers;
        }

        public Trainer GetTrainer(List<Trainer> trainers, int trainerId)
        {
            return trainers.SingleOrDefault(x => x.TrainerId == trainerId);
        }
    }
}