namespace VsMaker2Core.RomFiles
{
    public class ClassGenderData
    {
        public long Offset { get; set; }
        public byte Gender { get; set; }
        public int TrainerClassId { get; set; }

        public ClassGenderData()
        { }

        public ClassGenderData(long offset, byte gender, int trainerClassId)
        {
            Offset = offset;
            Gender = gender;
            TrainerClassId = trainerClassId;
        }
    }
}