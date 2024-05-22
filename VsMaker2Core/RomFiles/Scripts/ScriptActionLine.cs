namespace VsMaker2Core.RomFiles
{
    public class ScriptActionLine
    {
        public ushort? ActionCommandId { get; set; }
        public ushort? Repetitions { get; set; }

        public ScriptActionLine()
        { }

        public ScriptActionLine(ushort? actionCommandId, ushort? repetitions)
        {
            ActionCommandId = actionCommandId;
            Repetitions = repetitions;
        }
    }
}