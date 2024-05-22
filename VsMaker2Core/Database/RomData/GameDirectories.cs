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