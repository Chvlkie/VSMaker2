namespace VsMaker2Core.RomFiles
{
    public class ScriptFileData
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
    }
}