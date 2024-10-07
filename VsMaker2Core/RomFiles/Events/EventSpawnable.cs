using System.Diagnostics;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class EventSpawnable
    {
        public EventSpawnable() { }
        public EventType EventType => EventType.Spawnable;
        public ushort ScriptNumber { get; set; }
        public ushort SpawnableType { get; set; }
        public ushort XPosition { get; set; }
        public ushort Unknown2 { get; set; }
        public ushort YPosition { get; set; }
        public uint ZPosition { get; set; }
        public ushort Unknown4 { get; set; }
        public ushort Direction { get; set; }
        public ushort Unknown5 { get; set; }
    }
}