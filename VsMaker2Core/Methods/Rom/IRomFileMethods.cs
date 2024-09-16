using System.Threading.Tasks;
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
        Task<(bool Success, string ExceptionMessage)> ExtractRomContentsAsync(string workingDirectory, string fileName);

        List<string> GetAbilityNames(int abilityNameArchive);

        List<BattleMessageOffsetData> GetBattleMessageOffsetData(string battleMessageOffsetPath);

        List<string> GetBattleMessages(int battleMessageArchive);

        List<BattleMessageTableData> GetBattleMessageTableData(string trainerTextTablePath);

        List<string> GetClassDescriptions(int classDescriptionsArchive);

        List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam);

        List<string> GetClassNames(int classNamesArchive);
        List<EyeContactMusicData> GetEyeContactMusicData(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily);

        List<string> GetItemNames(int itemNameArchive);

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false);

        int GetMessageInitialKey(int messageArchive);

        List<string> GetMoveNames(int moveTextArchive);
        List<string> GetPokemonNames(int pokemonNameArchive);

        List<PrizeMoneyData> GetPrizeMoneyData(RomFile loadedRom);

        /// <summary>
        /// Get all Pokemon Species data from extracted ROM Files.
        /// </summary>
        /// <returns></returns>
        List<Species> GetSpecies();

        int GetTotalNumberOfTrainerClassess(int trainerClassNameArchive);

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

        Task RepackRomAsync(string ndsFileName);

        /// <summary>
        /// Setup the required NarcDirectory paths for opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="gameVersion"></param>
        /// <param name="gameFamily"></param>
        /// <param name="gameLanguage"></param>
        void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);

        int SetTrainerNameMax(int trainerNameOffset);

        /// <summary>
        /// Unpack required NARCs from a ROM's Extracted data.
        /// </summary>
        /// <param name="narcs"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
       Task<(bool Success, string ExceptionMessage)> UnpackNarcsAsync(List<NarcDirectory> narcs, IProgress<int> progress);
    }
}