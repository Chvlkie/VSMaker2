using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public interface IRomFileMethods
    {
        /// <summary>
        /// Extract the contents of opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        (bool Success, string ExceptionMessage) ExtractRomContents(string workingDirectory, string fileName);

        List<string> GetClassNames(int classNamesArchive);
        List<string> GetClassDescriptions(int classDescriptionsArchive);

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines);

        List<string> GetMoveNames(int moveTextArchive);
        List<string> GetAbilityNames(int abilityNameArchive);

        List<string> GetPokemonNames(int pokemonNameArchive);

        /// <summary>
        /// Get all Pokemon Species data from extracted ROM Files.
        /// </summary>
        /// <returns></returns>
        List<Species> GetSpecies();

        int GetTotalNumberOfTrainers(int trainerNameArchive);

        /// <summary>
        /// Get the TrainerNames from the trainerNameMessageArchive.
        /// </summary>
        /// <param name="trainerNameMessageArchive"></param>
        /// <returns></returns>
        List<string> GetTrainerNames(int trainerNameMessageArchive);

        List<TrainerData> GetTrainersData(int numberOfTrainers);

        List<TrainerPartyData> GetTrainersPartyData(int numberOfTrainers, List<TrainerData> trainerData, GameFamily gameFamily);

        TrainerData ReadTrainerData(int trainerId);

        TrainerPartyData ReadTrainerPartyData(int trainerId, byte teamSize, byte trainerType, bool hasBallCapsule);

        /// <summary>
        /// Setup the required NarcDirectory paths for opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="gameVersion"></param>
        /// <param name="gameFamily"></param>
        /// <param name="gameLanguage"></param>
        void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);

        /// <summary>
        /// Unpack required NARCs from a ROM's Extracted data.
        /// </summary>
        /// <param name="narcs"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        (bool Success, string ExceptionMessage) UnpackNarcs(List<NarcDirectory> narcs, IProgress<int> progress);
        List<string> GetItemNames(int itemNameArchive);
        int GetMessageInitialKey(int messageArchive);
        int SetTrainerNameMax(int trainerNameOffset);
    }
}