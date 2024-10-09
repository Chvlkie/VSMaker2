using static VsMaker2Core.Enums;

namespace VsMaker2Core
{
    public static class GameFamilyNarcs
    {
        public static List<NarcDirectory> DiamondPearl = [
                        NarcDirectory.monIcons,
                        NarcDirectory.moveData,
                        NarcDirectory.personalPokeData,
                        NarcDirectory.scripts,
                        NarcDirectory.eventFiles,
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                        NarcDirectory.trainerGraphics,
                        NarcDirectory.trainerParty,
                        NarcDirectory.trainerProperties,
                        NarcDirectory.trainerTextTable,
                        NarcDirectory.trainerTextOffset
                        ];

        public static List<NarcDirectory> Platinum = [
                        NarcDirectory.monIcons,
                        NarcDirectory.moveData,
                        NarcDirectory.personalPokeData,
                        NarcDirectory.scripts,
                        NarcDirectory.eventFiles,
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                        NarcDirectory.trainerGraphics,
                        NarcDirectory.trainerParty,
                        NarcDirectory.trainerProperties,
                        NarcDirectory.trainerTextTable,
                        NarcDirectory.trainerTextOffset
                      ];

        public static List<NarcDirectory> HeartGoldSoulSilver = [
                       NarcDirectory.battleStagePokeData,
                        NarcDirectory.battleTowerPokeData,
                        NarcDirectory.battleTowerTrainerData,
                        NarcDirectory.monIcons,
                        NarcDirectory.moveData,
                        NarcDirectory.personalPokeData,
                        NarcDirectory.scripts,
                        NarcDirectory.eventFiles,
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                        NarcDirectory.trainerGraphics,
                        NarcDirectory.trainerParty,
                        NarcDirectory.trainerProperties,
                        NarcDirectory.trainerTextTable,
                        NarcDirectory.trainerTextOffset
                      ];

        public static List<NarcDirectory> HgEngine = [
                     NarcDirectory.battleStagePokeData,
                        NarcDirectory.battleTowerPokeData,
                        NarcDirectory.battleTowerTrainerData,
                        NarcDirectory.monIcons,
                        NarcDirectory.moveData,
                        NarcDirectory.personalPokeData,
                        NarcDirectory.scripts,
                        NarcDirectory.eventFiles,
                        NarcDirectory.synthOverlay,
                        NarcDirectory.textArchives,
                        NarcDirectory.trainerGraphics,
                        NarcDirectory.trainerParty,
                        NarcDirectory.trainerProperties,
                        NarcDirectory.trainerTextTable,
                        NarcDirectory.trainerTextOffset
                      ];

        public static List<NarcDirectory> GetGameFamilyNarcs(GameFamily gameFamily) => gameFamily switch
        {
            GameFamily.DiamondPearl => DiamondPearl,
            GameFamily.Platinum => Platinum,
            GameFamily.HeartGoldSoulSilver => HeartGoldSoulSilver,
            GameFamily.HgEngine => HgEngine,
            _ => [],
        };
    }
}