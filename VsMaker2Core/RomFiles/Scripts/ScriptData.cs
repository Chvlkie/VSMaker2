using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class ScriptData : IEquatable<ScriptData?>
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

        public override bool Equals(object? obj)
        {
            return Equals(obj as ScriptData);
        }

        public bool Equals(ScriptData? other)
        {
            return other is not null &&
                   EqualityComparer<List<ScriptLine>>.Default.Equals(Lines, other.Lines) &&
                   ScriptNumber == other.ScriptNumber &&
                   UsedScriptId == other.UsedScriptId &&
                   ScriptType == other.ScriptType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Lines, ScriptNumber, UsedScriptId, ScriptType);
        }

        public static bool operator ==(ScriptData? left, ScriptData? right)
        {
            return EqualityComparer<ScriptData>.Default.Equals(left, right);
        }

        public static bool operator !=(ScriptData? left, ScriptData? right)
        {
            return !(left == right);
        }
    }
}