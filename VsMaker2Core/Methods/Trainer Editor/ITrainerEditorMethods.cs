using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public interface ITrainerEditorMethods
    {
        /// <summary>
        /// Get a list of Trainer Data from Extracted ROM files.
        /// </summary>
        /// <returns></returns>
        List<Trainer> GetTrainers();

        /// <summary>
        /// Get Data from a specific Trainer from given trainerId.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="trainerId"></param>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        Trainer GetTrainer(List<Trainer> trainers, int trainerId);

        TrainerParty BuildTrainerPartyFromRomData(TrainerPartyData trainerPartyData, int teamSize, bool hasBallCapsule, List<bool> trainerPropertyFlags);

        TrainerProperty BuildTrainerPropertyFromRomData(TrainerData trainerData);

        Trainer BuildTrainerData(int trainerId, string trainerName, TrainerData trainerData, TrainerPartyData trainerPartyData, bool hasBallCapsule);

        TrainerData NewTrainerData(TrainerProperty trainerProperties);

        TrainerProperty NewTrainerProperties(byte teamSize, byte trainerClassId, ushort item1, ushort item2, ushort item3, ushort item4, List<bool> aiFlags, List<bool> propertyFlags, uint battleType);

        TrainerPartyPokemonData NewTrainerPartyPokemonData(Pokemon pokemon, bool chooseMoves, bool chooseItems, bool hasBallCapsule);
        (bool Success, string ErrorMessage) RemoveTrainer(int trainerId);
    }
}