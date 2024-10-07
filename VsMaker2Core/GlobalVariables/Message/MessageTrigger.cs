namespace VsMaker2Core.DataModels
{
    public partial class MessageTrigger
    {
        public static List<MessageTrigger> MessageTriggers =>
            [
            new MessageTrigger(0, Descriptions.PreBattleOw),
            new MessageTrigger(1, Descriptions.PlayerWins),
            new MessageTrigger(2, Descriptions.PostBattleOw),
            new MessageTrigger(3, Descriptions.DoublePreBattleOwTrainer1),
            new MessageTrigger(4, Descriptions.DoublePlayerWinTrainer1),
            new MessageTrigger(5, Descriptions.DoublePostBattleOwTrainer1),
            new MessageTrigger(6, Descriptions.DoubleOnlyOnePokeTrainer1),
            new MessageTrigger(7, Descriptions.DoublePreBattleOwTrainer2),
            new MessageTrigger(8, Descriptions.DoublePlayerWinTrainer2),
            new MessageTrigger(9, Descriptions.DoublePostBattleOwTrainer2),
            new MessageTrigger(10, Descriptions.DoubleOnlyOnePokeTrainer2),
            new MessageTrigger(11, "notUsed0B"),
            new MessageTrigger(12, "notUsed0C"),
            new MessageTrigger(13, "notUsed0D"),
            new MessageTrigger(14, "notUsed0E"),
            new MessageTrigger(15, Descriptions.TrainerLastPoke),
            new MessageTrigger(16, Descriptions.TrainerLastPokeCritical),
            new MessageTrigger(17, Descriptions.Rematch),
            new MessageTrigger(18, Descriptions.DoubleRematchTrainer1),
            new MessageTrigger(19, Descriptions.DoubleRematchTrainer2),
            new MessageTrigger(20, Descriptions.PlayerLost),
            ];

        public static class Descriptions
        {
            public const string PreBattleOw = "Pre-Battle OW";
            public const string PlayerWins = "Player Wins";
            public const string PostBattleOw = "Post-Battle OW";
            public const string PlayerLost = "Player Lost";
            public const string TrainerLastPoke = "Opponent Last Pokemon";
            public const string TrainerLastPokeCritical = "Opponent Last Pokemon Crit. HP";

            public const string DoublePreBattleOwTrainer1 = "DOUBLE Pre-Battle OW; Trainer 1";
            public const string DoublePlayerWinTrainer1 = "DOUBLE Player Wins; Trainer 1";
            public const string DoublePostBattleOwTrainer1 = "DOUBLE Post-Battle OW; Trainer 1";
            public const string DoubleOnlyOnePokeTrainer1 = "DOUBLE Only One Poke; Trainer 1";

            public const string DoublePreBattleOwTrainer2 = "DOUBLE Pre-Battle OW; Trainer 2";
            public const string DoublePlayerWinTrainer2 = "DOUBLE Player Wins; Trainer 2";
            public const string DoublePostBattleOwTrainer2 = "DOUBLE Post-Battle OW; Trainer 2";
            public const string DoubleOnlyOnePokeTrainer2 = "DOUBLE Only One Poke; Trainer 2";
            public const string Rematch = "Rematch";
            public const string DoubleRematchTrainer1 = "Double Rematch Trainer 1";
            public const string DoubleRematchTrainer2 = "Double Rematch Trainer 2";
        }
    }
}