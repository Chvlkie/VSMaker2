namespace VsMaker2Core.RomFiles
{
    public class EyeContactMusicData
    {
        public uint Offset { get; set; }
        public ushort TrainerClassId { get; set; }
        public ushort MusicDayId { get; set; }
        public ushort? MusicNightId { get; set; }

        public EyeContactMusicData()
        { }

        public EyeContactMusicData(uint offset, ushort trainerClassId, ushort musicDayId, ushort? musicNightId)
        {
            Offset = offset;
            TrainerClassId = trainerClassId;
            MusicDayId = musicDayId;
            MusicNightId = musicNightId;
        }
    }
}