namespace VsMaker2Core.DataModels
{
    public class TrainerClassProperty : IEquatable<TrainerClassProperty?>
    {
        public int EyeContactMusicDay { get; set; }
        public int? EyeContactMusicNight { get; set; }
        public int PrizeMoneyMultiplier { get; set; }
        public int? Gender { get; set; }
        public string Description { get; set; }

        public TrainerClassProperty()
        { }

        public TrainerClassProperty(int gender, int prizeMoney, string description, int eyeContactMusicDay, int? eyeContactMusicNight = null)
        {
            Gender = gender;
            PrizeMoneyMultiplier = prizeMoney;
            Description = description;
            EyeContactMusicDay = eyeContactMusicDay;
            EyeContactMusicNight = eyeContactMusicNight;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as TrainerClassProperty);
        }

        public bool Equals(TrainerClassProperty? other)
        {
            return other is not null &&
                   EyeContactMusicDay == other.EyeContactMusicDay &&
                   EyeContactMusicNight == other.EyeContactMusicNight &&
                   PrizeMoneyMultiplier == other.PrizeMoneyMultiplier &&
                   Gender == other.Gender &&
                   Description == other.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EyeContactMusicDay, EyeContactMusicNight, PrizeMoneyMultiplier, Gender, Description);
        }
    }
}