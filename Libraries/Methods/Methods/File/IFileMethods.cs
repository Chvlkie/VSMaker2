using Data.DataModels.Rom;
using Data.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Methods.FileHandling
{
    public interface IFileMethods
    {
        Dictionary<Enums.NarcDirectory, (string packedPath, string unpackedPath)> SetNarcDirectories(string workingDirectory, RomData romData);
    }
}
