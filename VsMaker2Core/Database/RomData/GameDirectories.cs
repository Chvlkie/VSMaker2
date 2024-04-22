using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Database
{
    public partial class VsMakerDatabase
    {
        public partial class RomData
        {
            public static Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)> GameDirectories { get; set; }
        }
    }
}