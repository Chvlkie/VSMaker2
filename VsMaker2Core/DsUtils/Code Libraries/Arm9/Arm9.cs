using System.Diagnostics;
using VsMaker2Core;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DSUtils
{
    public class Arm9 : IArm9
    {
        public readonly uint Address = 0x02000000;
        private readonly IDSUtilsMethods methods;

        public Arm9()
        {
            methods = new DSUtilsMethods();
        }

        public bool Arm9Compress(string path)
        {
            Process compress = new();
            compress.StartInfo.FileName = Common.BlzFilePath;
            compress.StartInfo.Arguments = @" -en9 " + '"' + path + '"';
            compress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            compress.StartInfo.CreateNoWindow = true;
            compress.Start();
            compress.WaitForExit();

            return new FileInfo(RomFile.Arm9Path).Length <= 0xBC000;
        }

        public bool Arm9Decompress(string path)
        {
            Process decompress = new();
            decompress.StartInfo.FileName = Common.BlzFilePath;
            decompress.StartInfo.Arguments = @" -d " + '"' + path + '"';
            decompress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            decompress.StartInfo.CreateNoWindow = true;
            decompress.Start();
            decompress.WaitForExit();

            return new FileInfo(RomFile.Arm9Path).Length > 0xBC000;
        }

        public void Arm9EditSize(int increment)
        {
            using Arm9Writer writer = new();
            writer.EditSize(increment);
        }

        public bool CheckCompressionMark(GameFamily gameFamily)
        {
            return BitConverter.ToInt32(ReadBytes((uint)(gameFamily == GameFamily.DiamondPearl ? 0xB7C : 0xBB4), 4), 0) != 0;
        }

        public byte ReadByte(uint startOffset)
        {
            return methods.ReadFromFile(RomFile.Arm9Path, startOffset, 1)[0];
        }

        public byte[] ReadBytes(uint startOffset, long numberOfBytes = 0)
        {
            return methods.ReadFromFile(RomFile.Arm9Path, startOffset, numberOfBytes);
        }

        public void WriteByte(byte value, uint destinationOffset)
        {
            methods.WriteToFile(RomFile.Arm9Path, BitConverter.GetBytes((short)value), destinationOffset, 0);
        }

        public void WriteBytes(byte[] bytesToWrite, uint destinationOffset, int indexFirstByte = 0, int? indexLastByte = null)
        {
            methods.WriteToFile(RomFile.Arm9Path, bytesToWrite, destinationOffset, indexFirstByte, indexLastByte);
        }

        public class Arm9Reader : EasyReader
        {
            public Arm9Reader(long position = 0) : base(RomFile.Arm9Path, position)
            {
                BaseStream.Position = position;
            }
        }

        public class Arm9Writer : EasyWriter
        {
            public Arm9Writer(long position = 0) : base(RomFile.Arm9Path, position)
            {
                BaseStream.Position = position;
            }
        }
    }
}