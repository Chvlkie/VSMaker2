using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class ScriptData
    {
        public List<ScriptLine> Lines { get; set; }
        public uint ScriptNumber { get; set; }
        public int? UsedScriptId { get; set; }
        public ScriptType ScriptType { get; set; }

        public ScriptData()
        { }

        public ScriptData(ScriptData scriptDataCopy,  uint scriptNumber)
        {
            ScriptNumber = scriptNumber;
            ScriptType = scriptDataCopy.ScriptType;
            UsedScriptId = scriptDataCopy.UsedScriptId;
            Lines = scriptDataCopy.Lines;
        }
        public ScriptData(uint scriptNumber, ScriptType scriptType, int? usedByScriptId = null, List<ScriptLine> lines = null)
        {
            ScriptNumber = scriptNumber;
            ScriptType = scriptType;
            UsedScriptId = usedByScriptId;
            Lines = lines;
        }
    }
}