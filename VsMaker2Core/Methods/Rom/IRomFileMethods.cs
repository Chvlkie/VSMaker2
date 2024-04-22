using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
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

        /// <summary>
        /// Get the contents of a Message Archive for given messageArchiveId.
        /// </summary>
        /// <param name="messageArchiveId"></param>
        /// <param name="discardLines"></param>
        /// <returns></returns>
        List<MessageArchive> GetMessageArchiveContents(int messageArchiveId, bool discardLines);

        /// <summary>
        /// Get the TrainerNames from the trainerNameMessageArchive.
        /// </summary>
        /// <param name="trainerNameMessageArchive"></param>
        /// <returns></returns>
        List<string> GetTrainerNames(int trainerNameMessageArchive);

        /// <summary>
        /// Set the GameFamily of opened ROM File.
        /// </summary>
        /// <param name="gameVersion"></param>
        /// <returns></returns>
        GameFamily SetGameFamily(GameVersion gameVersion);

        /// <summary>
        /// Set the GameLanguage of opened ROM File.
        /// </summary>
        /// <param name="romId"></param>
        /// <returns></returns>
        GameLanguage SetGameLanguage(string romId);

        /// <summary>
        /// Setup the required NarcDirectory paths for opened ROM File.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="gameVersion"></param>
        /// <param name="gameFamily"></param>
        /// <param name="gameLanguage"></param>
        void SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);
        
        /// <summary>
        /// Set the MessageArchiveId for the TrainerName archive for opened ROM File.
        /// </summary>
        /// <param name="gameFamily"></param>
        /// <param name="gameLanguage"></param>
        /// <returns></returns>
        int SetTrainerNameTextArchiveNumber(GameFamily gameFamily, GameLanguage gameLanguage);

        /// <summary>
        /// Unpack required NARCs from a ROM's Extracted data.
        /// </summary>
        /// <param name="narcs"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        (bool Success, string ExceptionMessage) UnpackNarcs(List<NarcDirectory> narcs, IProgress<int> progress);
    }
}