using static VsMaker2Core.Enums;

namespace VsMaker2Core.RomFiles
{
    public class EventOverworld
    {
        public EventOverworld()
        { }

        public EventType EventType => EventType.Overworld;
        public ushort OverworldId { get; set; }
        public ushort OverworldTableEntry { get; set; }
        public ushort Movment { get; set; }
        public ushort OverworldType { get; set; }
        public ushort Flag { get; set; }
        public ushort ScriptNumber { get; set; }
        public ushort Orientation { get; set; }
        public ushort SightRange { get; set; }
        public ushort Unknown1 { get; set; }
        public ushort Unknown2 { get; set; }
        public ushort HorizontalRange { get; set; }
        public ushort VerticalRange { get; set; }
        public ushort XPosition { get; set; }
        public ushort YPosition { get; set; }
        public uint ZPosition { get; set; }

        public bool IsTrainer => OverworldType == 1 && (ScriptNumber >= 2999 || ScriptNumber >= 4999);
        public bool IsPartner => IsTrainer && ScriptNumber >= 4999;

        public int TrainerId => IsTrainer ? IsPartner ? ScriptNumber - 4999 : ScriptNumber - 2999 : -1;
    }
}