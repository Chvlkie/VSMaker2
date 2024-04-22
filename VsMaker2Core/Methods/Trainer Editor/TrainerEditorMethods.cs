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

        public List<Trainer> GetTrainers(int trainerMessageArchive)
        {
            var trainerNames = romFileMethods.GetTrainerNames(trainerMessageArchive);
            List<Trainer> trainers = [];
            // Start from i 1 to skip player trainer file
            for (int i = 1; i < trainerNames.Count; i++)
            {
                var trainer = new Trainer { TrainerId = (uint)i, TrainerName = trainerNames[i] };
                trainers.Add(trainer);
            }
            return trainers;
        }

      
    }
}