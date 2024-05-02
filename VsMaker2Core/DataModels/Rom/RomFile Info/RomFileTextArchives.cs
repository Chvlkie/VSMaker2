using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public partial class RomFile
    {
        public int AbilityNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 552,
            GameFamily.Platinum => 610,
            GameFamily.HeartGoldSoulSilver => 720,
            GameFamily.HgEngine => 720,
            _ => 0
        };

        public int BattleMessageTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? 549 : 558,
            GameFamily.Platinum => 617,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 718 : 728,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 718 : 728,
            _ => 0
        };

        public int ClassDescriptionMessageNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? 552 : 561,
            GameFamily.Platinum => 620,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 721 : 731,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 721 : 731,
            _ => 0
        };

        public int ClassNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? 551 : 560,
            GameFamily.Platinum => 619,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 720 : 730,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 720 : 730,
            _ => 0
        };

        public int ItemNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 344,
            GameFamily.Platinum => 392,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 219 : 222,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 219 : 222,
            _ => 0
        };

        public int MoveNameTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 588,
            GameFamily.Platinum => 647,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 739: 750,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 739: 750,
            _ => 0,
        };

        public int PokemonNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 362,
            GameFamily.Platinum => 412,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 232 : 237,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 232 : 237,
            _ => 0,
        };

        public int TrainerNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => GameLanguage == GameLanguage.Japanese ? 550 : 559,
            GameFamily.Platinum => 618,
            GameFamily.HeartGoldSoulSilver => GameLanguage == GameLanguage.Japanese ? 719 : 729,
            GameFamily.HgEngine => GameLanguage == GameLanguage.Japanese ? 719 : 729,
            _ => 0,
        };

        public int TypeNamesTextNumber => GameFamily switch
        {
            GameFamily.DiamondPearl => 565,
            GameFamily.Platinum => 624,
            GameFamily.HeartGoldSoulSilver => 735,
            GameFamily.HgEngine => 735,
            _ => 0,
        };
    }
}