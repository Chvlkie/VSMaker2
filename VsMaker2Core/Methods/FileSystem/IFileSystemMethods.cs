using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public interface IFileSystemMethods
    {
        /// <summary>
        /// Build a VsTrainersFile model with given data.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="gameFamily"></param>
        /// <param name="trainerNameTextArchiveId"></param>
        /// <param name="classesCount"></param>
        /// <param name="battleMessagesCount"></param>
        /// <returns></returns>
        VsTrainersFile BuildVsTrainersFile(List<Trainer> trainers, GameFamily gameFamily, int trainerNameTextArchiveId, int classesCount, int battleMessagesCount);

        /// <summary>
        /// Export all Trainer Data to .vstrainers file.
        /// </summary>
        /// <param name="export"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (bool Success, string ErrorMessage) ExportTrainers(VsTrainersFile export, string filePath);

        /// <summary>
        /// Import Trainer Data from a .vstrainers file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (VsTrainersFile VsTrainersFile, bool Success, string ErrorMessage) ImportTrainers(string filePath);
    }
}