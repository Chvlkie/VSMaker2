namespace VsMaker2Core.RomFiles
{
    public class EventFileData
    {
        public int EventFileId { get; set; }
        public List<EventSpawnable> Spawnables { get; set; }
        public List<EventOverworld> Overworlds { get; set; }
        public List<EventWarp> Warps { get; set; }
        public List<EventTrigger> Triggers { get; set; }

        public EventFileData()
        { }

        public EventFileData(int eventFileId, List<EventSpawnable> spawnables, List<EventOverworld> overworlds, List<EventWarp> warps, List<EventTrigger> triggers)
        {
            EventFileId = eventFileId;
            Spawnables = spawnables;
            Overworlds = overworlds;
            Warps = warps;
            Triggers = triggers;
        }
    }
}