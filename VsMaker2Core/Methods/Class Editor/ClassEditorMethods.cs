using VsMaker2Core.DataModels;

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

        public List<TrainerClass> GetTrainerClasses(int classMessageArchive)
        {
            var classNames = romFileMethods.GetClassNames(classMessageArchive);
            List<TrainerClass> trainerClasses = [];
            // Start from i 2 to skip player classes
            for (int i = 2; i < classNames.Count; i++)
            {
                var trainerClass = new TrainerClass { TrainerClassId = (uint)i, TrainerClassName = classNames[i] };
                trainerClasses.Add(trainerClass);
            }
            return trainerClasses;
        }
    }
}