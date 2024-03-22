using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.Rom
{
    public interface IRomFileMethods
    {
        GameFamily SetGameFamily(GameVersion gameVersion);
        GameLanguage SetGameLanguage(string romId);
        Dictionary<DirectoryNames, (string packedDirectory, string unpackedDirectory)> SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage);
    }
}
