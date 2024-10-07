using VsMaker2Core.DataModels;

namespace VsMaker2Core.Methods
{
    public interface IClassEditorMethods
    {
        TrainerClass GetTrainerClass(List<TrainerClass> classes, int classId);

        List<TrainerClass> GetTrainerClasses(List<Trainer> trainers, List<string> classNames, List<string> classDescriptions);

        List<Trainer> GetUsedByTrainers(int classId, List<Trainer> trainers);
    }
}