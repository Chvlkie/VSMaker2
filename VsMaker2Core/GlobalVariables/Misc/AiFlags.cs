namespace VsMaker2Core
{
    public static class AiFlags
    {
        public const string Basic = "Basic";
        public const string EvaluateAttack = "Evaluate Attack";
        public const string Expert = "Expert";
        public const string StatusEffect = "Status Effects";
        public const string Risky = "Risky";
        public const string DamagePriority = "Damage Priority";
        public const string BatonPass = "Baton Pass";
        public const string TagTeam = "Tag Team";
        public const string CheckHp = "Check HP";
        public const string WeatherEffect = "Weather Effects";
        public const string Harassment = "Harassment";

        public static List<string> AiFlagNames =>
            [
            Basic,
            EvaluateAttack,
            Expert,
            StatusEffect,
            Risky,
            DamagePriority,
            BatonPass,
            TagTeam,
            CheckHp, 
            WeatherEffect,
            Harassment
            ];
    }
}