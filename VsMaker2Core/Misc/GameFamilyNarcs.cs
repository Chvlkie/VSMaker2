using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core
{
    public static class GameFamilyNarcs
    {
        public static List<NarcDirectory> DiamondPearl = [
                        NarcDirectory.MonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchives,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                        ];

        public static List<NarcDirectory> Platinum = [
                        NarcDirectory.MonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchives,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                      ];

        public static List<NarcDirectory> HeartGoldSoulSilver = [
                       NarcDirectory.BattleStagePokeData,
                        NarcDirectory.BattleTowerPokeData,
                        NarcDirectory.BattleTowerTrainerData,
                        NarcDirectory.MonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchives,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                      ];

        public static List<NarcDirectory> HgEngine = [
                     NarcDirectory.BattleStagePokeData,
                        NarcDirectory.BattleTowerPokeData,
                        NarcDirectory.BattleTowerTrainerData,
                        NarcDirectory.MonIcons,
                        NarcDirectory.MoveData,
                        NarcDirectory.PersonalPokeData,
                        NarcDirectory.SynthOverlay,
                        NarcDirectory.TextArchives,
                        NarcDirectory.TrainerGraphics,
                        NarcDirectory.TrainerParty,
                        NarcDirectory.TrainerProperties,
                        NarcDirectory.BattleMessageTable,
                        NarcDirectory.BattleMessageOffset
                      ];

        public static List<NarcDirectory> GetGameFamilyNarcs(GameFamily gameFamily)
        {
            return gameFamily switch
            {
                GameFamily.DiamondPearl => DiamondPearl,
                GameFamily.Platinum => Platinum,
                GameFamily.HeartGoldSoulSilver => HeartGoldSoulSilver,
                GameFamily.HgEngine => HgEngine,
                _ => [],
            };
        }
    }
}