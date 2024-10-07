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

        /// <summary>
        ///
        /// </summary>
        /// <param name="abilityNameArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetAbilityNamesAsync(int abilityNameArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="battleMessageOffsetPath"></param>
        /// <returns></returns>
        Task<List<BattleMessageOffsetData>> GetBattleMessageOffsetDataAsync(string battleMessageOffsetPath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="battleMessageArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetBattleMessagesAsync(int battleMessageArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerTextTablePath"></param>
        /// <returns></returns>
        Task<List<BattleMessageTableData>> GetBattleMessageTableDataAsync(string trainerTextTablePath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="classDescriptionsArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetClassDescriptionsAsync(int classDescriptionsArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="numberOfClasses"></param>
        /// <param name="classGenderOffsetToRam"></param>
        /// <returns></returns>
        Task<List<ClassGenderData>> GetClassGendersAsync(int numberOfClasses, uint classGenderOffsetToRam);

        /// <summary>
        ///
        /// </summary>
        /// <param name="classNamesArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetClassNamesAsync(int classNamesArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="eyeContactMusicTableOffsetToRam"></param>
        /// <param name="gameFamily"></param>
        /// <returns></returns>
        Task<List<EyeContactMusicData>> GetEyeContactMusicDataAsync(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily);

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemNameArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetItemNamesAsync(int itemNameArchive);

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        Task<List<MessageArchive>> GetMessageArchiveContentsAsync(int messageArchiveId, bool discardLines = false);

        /// <summary>
        ///
        /// </summary>
        /// <param name="messageArchive"></param>
        /// <returns></returns>
        Task<int> GetMessageInitialKeyAsync(int messageArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="moveTextArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetMoveNamesAsync(int moveTextArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pokemonNameArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetPokemonNamesAsync(int pokemonNameArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="loadedRom"></param>
        /// <returns></returns>
        Task<List<PrizeMoneyData>> GetPrizeMoneyDataAsync();

        /// <summary>
        /// Get all Pokemon Species data from extracted ROM Files.
        /// </summary>
        /// <returns></returns>
        Task<List<Species>> GetSpeciesAsync();

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerClassNameArchive"></param>
        /// <returns></returns>
        Task<int> GetTotalNumberOfTrainerClassesAsync(int trainerClassNameArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerNameArchive"></param>
        /// <returns></returns>
        Task<int> GetTotalNumberOfTrainersAsync(int trainerNameArchive);

        /// <summary>
        /// Get the TrainerNames from the trainerNameMessageArchive.
        /// </summary>
        /// <param name="trainerNameMessageArchive"></param>
        /// <returns></returns>
        Task<List<string>> GetTrainerNamesAsync(int trainerNameMessageArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="numberOfTrainers"></param>
        /// <returns></returns>
        Task<List<TrainerData>> GetTrainersDataAsync(int numberOfTrainers);

        /// <summary>
        ///
        /// </summary>
        /// <param name="numberOfTrainers"></param>
        /// <param name="trainerData"></param>
        /// <param name="gameFamily"></param>
        /// <returns></returns>
        Task<List<TrainerPartyData>> GetTrainersPartyDataAsync(int numberOfTrainers, List<TrainerData> trainerData, GameFamily gameFamily);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerId"></param>
        /// <returns></returns>

        Task<TrainerData> ReadTrainerDataAsync(int trainerId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerId"></param>
        /// <param name="teamSize"></param>
        /// <param name="trainerType"></param>
        /// <param name="hasBallCapsule"></param>
        /// <returns></returns>
        Task<TrainerPartyData> ReadTrainerPartyDataAsync(int trainerId, byte teamSize, byte trainerType, bool hasBallCapsule);

        /// <summary>
        ///
        /// </summary>
        /// <param name="ndsFileName"></param>
        /// <returns></returns>
        Task RepackRomAsync(string ndsFileName);

        /// <summary>
        /// Setup the required NarcDirectory paths for opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="gameVersion"></param>
        /// <param name="gameFamily"></param>
        /// <param name="gameLanguage"></param>
        void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerNameOffset"></param>
        /// <returns></returns>
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