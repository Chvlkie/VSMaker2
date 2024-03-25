using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.Rom
{
    public interface IRomFileMethods
    {
        GameFamily SetGameFamily(GameVersion gameVersion);
        GameLanguage SetGameLanguage(string romId);
        Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)> SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);
        (bool Success, string ExceptionMessage) ExtractRomContents(string workingDirectory, string fileName);
        (bool Success, string ExceptionMessage) UnpackNarcs(RomFile romFile, List<NarcDirectory> narcs, IProgress<int> progress);
    }
}
