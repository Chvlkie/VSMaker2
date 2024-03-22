using Data.Rom;
using DSUtils.EasyReaderWriter;
using System.Diagnostics;

namespace DSUtils
{
    public static class Arm9
    {
        public static readonly uint Address = 0x02000000;

        public static bool Arm9Compress(string path)
        {
            Process compress = new();
            compress.StartInfo.FileName = @"Tools\blz.exe";
            compress.StartInfo.Arguments = @" -en9 " + '"' + path + '"';
            compress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compress.StartInfo.CreateNoWindow = true;
            compress.Start();
            compress.WaitForExit();

            return new FileInfo(RomInfo.Arm9Path).Length <= 0xBC000;
        }

        public static bool Arm9Decompress(string path)
        {
            Process decompress = new();
            decompress.StartInfo.FileName = @"Tools\blz.exe";
            decompress.StartInfo.Arguments = @" -d " + '"' + path + '"';
            decompress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            decompress.StartInfo.CreateNoWindow = true;
            decompress.Start();
            decompress.WaitForExit();

            return new FileInfo(RomInfo.Arm9Path).Length > 0xBC000;
        }

        public static void Arm9EditSize(int increment)
        {
            using Arm9Writer writer = new();
            writer.EditSize(increment);
        }

        public class Arm9Reader : EasyReader
        {
            public Arm9Reader(long position = 0) : base(RomInfo.Arm9Path, position)
            {
                BaseStream.Position = position;
            }
        }

        public class Arm9Writer : EasyWriter
        {
            public Arm9Writer(long position = 0) : base(RomInfo.Arm9Path, position)
            {
                BaseStream.Position = position;
            }
        }

        public static bool CheckCompressionMark()
        {
            return BitConverter.ToInt32(Rea)
        }
    }
}