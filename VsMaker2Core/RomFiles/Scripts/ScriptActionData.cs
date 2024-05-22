using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class ScriptActionData
    {
        public List<ScriptActionLine> Lines { get; set; }
        public uint ActionNumber { get; set; }

        public ScriptActionData()
        { }

        public ScriptActionData(ScriptActionData scriptDataCopy, uint actionNumber)
        {
            ActionNumber = actionNumber;
            Lines = scriptDataCopy.Lines;
        }

        public ScriptActionData(uint actionNumber, List<ScriptActionLine> lines = null)
        {
            ActionNumber = actionNumber;
            Lines = lines;
        }
    }
}