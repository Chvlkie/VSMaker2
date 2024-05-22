namespace VsMaker2Core.DataModels
{
    public class TrainerClassProperty
    {
        public int EyeContactMusicDay { get; set; }
        public int? EyeContactMusicNight { get; set; }
        public int PrizeMoneyMultiplier { get; set; }
        public int? Gender { get; set; }
        public string Description { get; set; }

        public TrainerClassProperty() { }
        public TrainerClassProperty(int gender, int prizeMoney, string description, int eyeContactMusicDay, int? eyeContactMusicNight = null)
        {
            Gender = gender;
            PrizeMoneyMultiplier = prizeMoney;
            Description = description;
            EyeContactMusicDay = eyeContactMusicDay;
            EyeContactMusicNight = eyeContactMusicNight;
        }
    }
}