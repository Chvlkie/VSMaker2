using Data.Global;
using Data.Global.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Global.Enums;

namespace Data.DataModels.Main
{
    public class FileData
    {
        public string FileName { get; set; }
        public string RomFilePath { get; set; }
        public string WorkingDirectory { get; set; }
        public string Arm9Path => WorkingDirectory + FileSystem.Paths.Arm9FilePath;
        public Dictionary<NarcDirectory, (string packedPath, string unpackedPath)> NarcDirectories { get; set; }

        public List<string> PackedPaths => [.. NarcDirectories.Values.Select(x => x.packedPath)];
        public List<string> UnpackedPaths => [.. NarcDirectories.Values.Select(x => x.unpackedPath)];
    }
}