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
        /// Export all Trainer Data to .vstrainers file.
        /// </summary>
        /// <param name="trainers"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (bool Success, string ErrorMessage) ExportTrainers(List<Trainer> trainers, string filePath);

        /// <summary>
        /// Import Trainer Data from a .vstrainers file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        (List<Trainer> Trainers, bool Success, string ErrorMessage) ImportTrainers(string filePath);
    }
}