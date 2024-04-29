using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public interface ITrainerEditorMethods
    {
        /// <summary>
        /// Get a list of Trainer Data from Extracted ROM files.
        /// </summary>
        /// <param name="trainerMessageArchive"></param>
        /// <param name="gameFamily"></param>
        /// <param name="partyReadFirstByte"></param>
        /// <returns></returns>
        List<Trainer> GetTrainers(int trainerMessageArchive, GameFamily gameFamily, bool partyReadFirstByte = false);

        /// <summary>
        /// Get Data from a specific Trainer from given trainerId.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        Trainer GetTrainer(List<Trainer> trainers, int trainerId);
    }
}
