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
            using EasyReader reader = new(RomFile.Arm9Path, startOffset);
            return reader.ReadByte();
        }

        public static byte[] ReadBytes(uint startOffset, long numberOfBytes = 0)
        {
            return ReadFromFile(RomFile.Arm9Path, startOffset, numberOfBytes);
        }

        public static byte[] ReadFromFile(string filePath, long startOffset = 0, long numberOfBytes = 0)
        {
            byte[] buffer;

            using (EasyReader reader = new(filePath, startOffset))
            {
                try
                {
                    long bytesToRead = numberOfBytes == 0 ? reader.BaseStream.Length - reader.BaseStream.Position : numberOfBytes;

                    buffer = new byte[bytesToRead];
                    reader.Read(buffer, 0, (int)bytesToRead);
                }
                catch (EndOfStreamException ex)
                {
                    Console.WriteLine($"End of FileStream encountered: {ex.Message}");
                    buffer = [];
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"I/O error occurred while reading: {ex.Message}");
                    buffer = [];
                }
            }

            return buffer;
        }

        public static void WriteToFile(string filePath, byte[] toOutput, uint writeAt = 0, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null, FileMode fileMode = FileMode.OpenOrCreate)
        {
            int lastByteIndex = indexLastByteToWrite ?? toOutput.Length;
            if (indexFirstByteToWrite < 0 || indexFirstByteToWrite >= toOutput.Length || lastByteIndex > toOutput.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid byte range specified for writing.");
            }

            try
            {
                using EasyWriter writer = new(filePath, writeAt, fileMode);

                int count = lastByteIndex - indexFirstByteToWrite;
                writer.Write(toOutput, indexFirstByteToWrite, count);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error occurred while writing to file: {ex.Message}");
                throw;
            }
        }

        public static async Task<bool> Arm9CompressAsync(string path)
        {
            Console.WriteLine("Compressing Arm9...");
            try
            {
                using Process compress = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Common.BlzFilePath,
                        Arguments = @" -en9 " + '"' + path + '"',
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };

                compress.Start();

                await compress.WaitForExitAsync();

                if (compress.ExitCode != 0)
                {
                    Console.WriteLine($"Compression process failed with exit code {compress.ExitCode}.");
                    return false;
                }

                FileInfo fileInfo = new(RomFile.Arm9Path);
                if (!fileInfo.Exists)
                {
                    Console.WriteLine("ARM9 file not found after compression.");
                    return false;
                }
                Console.WriteLine("Compressing Arm9 | Success");

                return fileInfo.Length <= 0xBC000;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during ARM9 compression: {ex.Message}");
                return false;
            }
        }

        public static async Task<bool> Arm9DecompressAsync(string path)
        {
            Console.WriteLine("Decompressing Arm9...");

            try
            {
                using Process decompress = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Common.BlzFilePath,
                        Arguments = @" -d " + '"' + path + '"',
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };

                decompress.Start();

                await decompress.WaitForExitAsync();

                if (decompress.ExitCode != 0)
                {
                    Console.WriteLine($"Decompression process failed with exit code {decompress.ExitCode}.");
                    return false;
                }

                FileInfo fileInfo = new(RomFile.Arm9Path);
                if (!fileInfo.Exists)
                {
                    Console.WriteLine("ARM9 file not found after decompression.");
                    return false;
                }

                Console.WriteLine("Decompressing Arm9 | Success");
                return fileInfo.Length > 0xBC000;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during ARM9 decompression: {ex.Message}");
                return false;
            }
        }

        public static void Arm9EditSize(int increment)
        {
            Console.WriteLine("Edit Arm9 size...");

            try
            {
                if (increment == 0)
                {
                    Console.WriteLine("No size change requested.");
                    return;
                }

                using Arm9Writer writer = new();
                writer.EditSize(increment);

                Console.WriteLine($"Successfully edited ARM9 size by {increment} bytes.");
                Console.WriteLine("Edit Arm9 size | Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while editing ARM9 size: {ex.Message}");
                throw;
            }
        }

        public static bool CheckCompressionMark(GameFamily gameFamily)
        {
            const uint DiamondPearlOffset = 0xB7C;
            const uint OtherGamesOffset = 0xBB4;

            uint offset = gameFamily == GameFamily.DiamondPearl ? DiamondPearlOffset : OtherGamesOffset;

            try
            {
                byte[] bytes = ReadBytes(offset, 4);

                if (bytes.Length != 4)
                {
                    Console.WriteLine($"Failed to read 4 bytes from offset {offset:X}");
                    return false;
                }

                return BitConverter.ToInt32(bytes, 0) != 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking compression mark: {ex.Message}");
                return false;
            }
        }

        public static void WriteByte(byte value, uint destinationOffset)
        {
            try
            {
                using FileStream arm9Stream = new FileStream(RomFile.Arm9Path, FileMode.Open, FileAccess.Write, FileShare.None);
                arm9Stream.Position = destinationOffset;

                arm9Stream.WriteByte(value);

                arm9Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to ARM9: {ex.Message}");
                throw;  // Re-throw the exception if needed
            }
        }

        public static void WriteBytes(byte[] bytesToWrite, uint destinationOffset, int indexFirstByte = 0, int? indexLastByte = null)
        {
            int lastByteIndex = indexLastByte ?? bytesToWrite.Length;

            if (indexFirstByte < 0 || indexFirstByte >= bytesToWrite.Length || lastByteIndex > bytesToWrite.Length)
            {
                throw new ArgumentOutOfRangeException("Invalid byte range specified for writing.");
            }

            WriteToFile(RomFile.Arm9Path, bytesToWrite, destinationOffset, indexFirstByte, lastByteIndex);
        }

        public class Arm9Reader : EasyReader
        {
            public Arm9Reader(long position = 0) : base(RomFile.Arm9Path, position)
            {
                try
                {
                    BaseStream.Position = position;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error opening arm9.bin for reading: {ex.Message}");
                    throw;
                }
            }
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            //FC B5 05 48 C0 46 41 21
            //09 22 02 4D A8 47 00 20
            //03 21 FC BD F1 64 00 02
            //00 80 3C 02
            if (string.IsNullOrWhiteSpace(hexString))
                return [];

            hexString = hexString.Replace(" ", "").Trim();

            if (hexString.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even number of characters.");

            byte[] byteArray = new byte[hexString.Length / 2];

            for (int i = 0; i < byteArray.Length; i++)
            {
                byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return byteArray;
        }

        public class Arm9Writer : EasyWriter
        {
            public Arm9Writer(long position = 0) : base(RomFile.Arm9Path, position)
            {
                try
                {
                    BaseStream.Position = position;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Error opening arm9.bin for writing: {ex.Message}");
                    throw;
                }
            }
        }
    }
}