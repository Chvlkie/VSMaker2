using System.Diagnostics;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DSUtils
{
    public static class Arm9
    {
        public static readonly uint Address = 0x02000000;

        public static byte ReadByte(uint startOffset)
        {
            return ReadFromFile(RomFile.Arm9Path, startOffset, 1)[0];
        }

        public static byte[] ReadBytes(uint startOffset, long numberOfBytes = 0)
        {
            return ReadFromFile(RomFile.Arm9Path, startOffset, numberOfBytes);
        }

        public static byte[] ReadFromFile(string filePath, long startOffset = 0, long numberOfBytes = 0)
        {
            byte[] buffer = null;
            using (EasyReader reader = new(filePath, startOffset))
            {
                try
                {
                    buffer = reader.ReadBytes(numberOfBytes == 0 ? (int)(reader.BaseStream.Length - reader.BaseStream.Position) : (int)numberOfBytes);
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("End of FileStream");
                }
            }
            return buffer;
        }

        public static void WriteToFile(string filePath, byte[] toOutput, uint writeAt = 0, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null, FileMode fileMode = FileMode.OpenOrCreate)
        {
            using EasyWriter writer = new(filePath, writeAt, fileMode);
            writer.Write(toOutput, indexFirstByteToWrite, indexLastByteToWrite is null ? toOutput.Length - indexFirstByteToWrite : (int)indexLastByteToWrite);
        }

        public static bool Arm9Compress(string path)
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

        public static bool Arm9Decompress(string path)
        {
            Process decompress = new();
            decompress.StartInfo.FileName = Common.BlzFilePath;
            decompress.StartInfo.Arguments = @" -d " + '"' + path + '"';
            decompress.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            decompress.StartInfo.CreateNoWindow = false;
            decompress.Start();
            decompress.WaitForExit();

            return new FileInfo(RomFile.Arm9Path).Length > 0xBC000;
        }

        public static void Arm9EditSize(int increment)
        {
            using Arm9Writer writer = new();
            writer.EditSize(increment);
        }
        public static bool CheckCompressionMark(GameFamily gameFamily)
        {
            return BitConverter.ToInt32(ReadBytes((uint)(gameFamily == GameFamily.DiamondPearl ? 0xB7C : 0xBB4), 4), 0) != 0;
        }
        public static void WriteByte(byte value, uint destinationOffset)
        {
            WriteToFile(RomFile.Arm9Path, BitConverter.GetBytes((short)value), destinationOffset, 0);
        }

        public static void WriteBytes(byte[] bytesToWrite, uint destinationOffset, int indexFirstByte = 0, int? indexLastByte = null)
        {
            WriteToFile(RomFile.Arm9Path, bytesToWrite, destinationOffset, indexFirstByte, indexLastByte);
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