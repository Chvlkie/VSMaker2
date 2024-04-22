using System.Diagnostics;
using System.Net;
using VsMaker2Core.Database;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.Rom
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
            for (int i = 0; i < trainerNames.Count; i++)
            {
                var trainer = new Trainer { TrainerId = (uint)i, TrainerName = trainerNames[i] };
                trainers.Add(trainer);
            }
            return trainers;
        }
    }
}