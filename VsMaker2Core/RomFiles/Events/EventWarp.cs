using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class EventWarp
    {
        public EventWarp() { }
        public EventType EventType => EventType.Warp;
        public ushort XPosition { get; set; }
        public ushort YPosition { get; set; }
        public ushort Header { get; set; }
        public ushort Anchor { get; set; }
        public uint Height { get; set; }
    }
}