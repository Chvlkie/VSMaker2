using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
using VsMaker2Core.DataModels.Trainers;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public interface ITrainerEditorMethods
    {
        /// <summary>
        /// Get a list of Trainer Data from Extracted ROM files.
        /// </summary>
        /// <param name="loadedRom"></param>
        /// <returns></returns>
        List<Trainer> GetTrainers(RomFile loadedRom);

        /// <summary>
        /// Get Data from a specific Trainer from given trainerId.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        Trainer GetTrainer(List<Trainer> trainers, int trainerId);

        TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasItems, bool chooseMoves, bool hasBallCapsule);
        TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData);
        Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule);
    }
}
