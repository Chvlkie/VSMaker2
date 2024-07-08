
namespace VsMaker2Core.RomFiles
{
    public class ScriptFileData : IEquatable<ScriptFileData?>
    {
        public int ScriptFileId { get; set; }
        public bool IsLevelScript { get; set; }
        public List<ScriptData> Scripts { get; set; }
        public List<ScriptData> Functions { get; set; }
        public List<ScriptActionData> Actions { get; set; }

        public ScriptFileData()
        { }

        public ScriptFileData(int scriptFileId, bool isLevelScript, List<ScriptData> scripts, List<ScriptData> functions, List<ScriptActionData> actions)
        {
            ScriptFileId = scriptFileId;
            IsLevelScript = isLevelScript;
            Scripts = scripts;
            Functions = functions;
            Actions = actions;
        }

        public ScriptFileData(ScriptFileData original, List<ScriptData> scripts)
        {
            ScriptFileId = original.ScriptFileId;
            IsLevelScript = original.IsLevelScript;
            Scripts = scripts;
            Functions = original.Functions;
            Actions = original.Actions;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ScriptFileData);
        }

        public bool Equals(ScriptFileData? other)
        {
            return other is not null &&
                   ScriptFileId == other.ScriptFileId &&
                   IsLevelScript == other.IsLevelScript &&
                   EqualityComparer<List<ScriptData>>.Default.Equals(Scripts, other.Scripts) &&
                   EqualityComparer<List<ScriptData>>.Default.Equals(Functions, other.Functions) &&
                   EqualityComparer<List<ScriptActionData>>.Default.Equals(Actions, other.Actions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ScriptFileId, IsLevelScript, Scripts, Functions, Actions);
        }

        public static bool operator ==(ScriptFileData? left, ScriptFileData? right)
        {
            return EqualityComparer<ScriptFileData>.Default.Equals(left, right);
        }

        public static bool operator !=(ScriptFileData? left, ScriptFileData? right)
        {
            return !(left == right);
        }
    }
}