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
        /// Get Ability Names from the abilityNameArchive in the rom's text narc
        /// </summary>
        /// <param name="abilityNameArchive"></param>
        /// <returns></returns>
        List<string> GetAbilityNames(int abilityNameArchive);

        /// <summary>
        /// Get the contents of the battle message table.
        /// </summary>
        /// <param name="battleMessageOffsetPath"></param>
        /// <returns></returns>
        List<BattleMessageOffsetData> GetBattleMessageOffsetData(string battleMessageOffsetPath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="battleMessageArchive"></param>
        /// <returns></returns>
        List<string> GetBattleMessages(int battleMessageArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerTextTablePath"></param>
        /// <returns></returns>
        List<BattleMessageTableData> GetBattleMessageTableData(string trainerTextTablePath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="classDescriptionsArchive"></param>
        /// <returns></returns>
        List<string> GetClassDescriptions(int classDescriptionsArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="numberOfClasses"></param>
        /// <param name="classGenderOffsetToRam"></param>
        /// <returns></returns>
        List<ClassGenderData> GetClassGenders(int numberOfClasses, uint classGenderOffsetToRam);

        /// <summary>
        ///
        /// </summary>
        /// <param name="classNamesArchive"></param>
        /// <returns></returns>
        List<string> GetClassNames(int classNamesArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="eyeContactMusicTableOffsetToRam"></param>
        /// <param name="gameFamily"></param>
        /// <returns></returns>
        List<EyeContactMusicData> GetEyeContactMusicData(uint eyeContactMusicTableOffsetToRam, GameFamily gameFamily);

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemNameArchive"></param>
        /// <returns></returns>
        List<string> GetItemNames(int itemNameArchive);

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines = false);

        /// <summary>
        ///
        /// </summary>
        /// <param name="messageArchive"></param>
        /// <returns></returns>
        int GetMessageInitialKey(int messageArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="moveTextArchive"></param>
        /// <returns></returns>
        List<string> GetMoveNames(int moveTextArchive);

        /// <summary>
        ///
        /// </summary>
        /// <param name="pokemonNameArchive"></param>
        /// <returns></returns>
        List<string> GetPokemonNames(int pokemonNameArchive);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        Task<List<PrizeMoneyData>> GetPrizeMoneyDataAsync();

        /// <summary>
        /// Get all Pokemon Species data from extracted ROM Files.
        /// </summary>
        /// <returns></returns>
        List<Species> GetSpecies();

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerClassNameArchive"></param>
        /// <returns></returns>
        int GetTotalNumberOfTrainerClasses(int trainerClassNameArchive);

        /// <summary>
        /// Get the TrainerNames from the trainerNameMessageArchive.
        /// </summary>
        /// <returns></returns>
        List<string> GetTrainerNames();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        List<TrainerData> GetTrainersData();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        List<TrainerPartyData> GetTrainersPartyData();

        /// <summary>
        ///
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (bool Success, string ErrorMessage) LoadInitialRomData(string filePath);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerId"></param>
        /// <returns></returns>

        TrainerData ReadTrainerData(int trainerId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="trainerId"></param>
        /// <param name="teamSize"></param>
        /// <param name="trainerType"></param>
        /// <param name="hasBallCapsule"></param>
        /// <returns></returns>
       TrainerPartyData ReadTrainerPartyData(int trainerId, byte teamSize, byte trainerType, bool hasBallCapsule);

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