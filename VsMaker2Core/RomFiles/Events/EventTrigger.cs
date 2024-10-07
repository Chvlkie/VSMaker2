using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class EventTrigger
    {
        public EventTrigger()
        { }

        public EventType EventType => EventType.Trigger;

        public ushort ScriptNumber { get; set; }
        public ushort XPosition { get; set; }
        public ushort YPosition { get; set; }
        public ushort WidthX { get; set; }
        public ushort HeightY { get; set; }
        public ushort ZPosition { get; set; }
        public ushort ExpectedVarValue { get; set; }
        public ushort VariableWatched { get; set; }
    }
}