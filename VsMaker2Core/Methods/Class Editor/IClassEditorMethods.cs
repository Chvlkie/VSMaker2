using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;

namespace VsMaker2Core.Methods
{
    public interface IClassEditorMethods
    {
        TrainerClass GetTrainerClass(List<TrainerClass> classes, int classId);

        List<TrainerClass> GetTrainerClasses(List<Trainer> trainers, List<string> classNames, List<string> classDescriptions, RomFile loadedRom);
        List<Trainer> GetUsedByTrainers(int classId, List<Trainer> trainers);
    }
}