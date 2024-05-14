using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public interface ITrainerEditorMethods
    {
        /// <summary>
        /// Get a list of Trainer Data from Extracted ROM files.
        /// </summary>
        /// <param name="loadedRom"></param>
        /// <returns></returns>
        List<Trainer> GetTrainers(List<string> trainerNames, RomFile loadedRom);

        /// <summary>
        /// Get Data from a specific Trainer from given trainerId.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        Trainer GetTrainer(List<Trainer> trainers, int trainerId);

        TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasItems, bool chooseMoves, bool hasBallCapsule);
        TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData);
        Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule);
        TrainerData NewTrainerData(TrainerProperty trainerProperties);
        TrainerProperty NewTrainerProperties(byte teamSize, bool chooseMoves, bool chooseItems, bool isDouble, byte trainerClassId, ushort item1, ushort item2, ushort item3, ushort item4, List<bool> aiFlags);
        TrainerPartyPokemonData NewTrainerPartyPokemonData(Pokemon pokemon, bool chooseMoves, bool chooseItems, bool hasBallCapsule);
    }
}
