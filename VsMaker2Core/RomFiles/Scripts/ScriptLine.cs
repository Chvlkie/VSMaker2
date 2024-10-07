namespace VsMaker2Core.RomFiles
{
    public class ScriptLine
    {
        public ushort? ScriptCommandId { get; set; }
        public List<byte[]> Parameters { get; set; }

        public ScriptLine()
        { }

        public ScriptLine(ushort scriptCommandId, List<byte[]> parameters)
        {
            switch (parameters)
            {
                case null:
                    ScriptCommandId = null;
                    break;

                default:
                    ScriptCommandId = scriptCommandId;
                    Parameters = parameters;
                    break;
            }
        }
    }
}