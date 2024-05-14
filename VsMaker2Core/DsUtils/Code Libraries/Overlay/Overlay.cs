using System.Diagnostics;
using VsMaker2Core.DataModels;

namespace VsMaker2Core.DsUtils
{
    public static class Overlay
    {
        public static string OverlayFilePath(int overlayNumber)
        {
            return $"{RomFile.WorkingDirectory}overlay\\overlay_{overlayNumber:D4}.bin";
        }

        public static bool CheckOverlayIsCompressed(int overlayNumber)
        {
            return new FileInfo(OverlayFilePath(overlayNumber)).Length < GetOverlayUncompressedSize(overlayNumber);
        }

        public static uint GetOverlayUncompressedSize(int overlayNumber)
        {
            using BinaryReader reader = new(File.OpenRead(RomFile.OverlayTablePath));
            {
                reader.BaseStream.Position = (overlayNumber * 32) + 8; //overlayNumber * size of entry + offset
                return reader.ReadUInt32();
            }
        }

        public static void SetOverlayCompressionInTable(int overlayNumber, byte compression)
        {
            if (compression < 0 || compression > 3)
            {
                Console.WriteLine("Can't compress, invalid compression");
                return;
            }
            using BinaryWriter writer = new(File.OpenWrite(RomFile.OverlayTablePath));
            try
            {
                writer.BaseStream.Position = overlayNumber * 32 + 31;
                writer.Write(compression);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static int DecompressOverlay(int overlayNumber, bool makeBackup = true)
        {
            string overlayPath = OverlayFilePath(overlayNumber);
            if (!File.Exists(overlayPath))
            {
                throw new Exception("Overlay path not found");
            }
            if (makeBackup)
            {
                if (File.Exists(overlayPath + "_backup"))
                {
                    File.Delete(overlayPath + "_backup");
                }
                File.Copy(overlayPath, overlayPath + "_backup");
            }

            Process unpack = new Process();
            unpack.StartInfo.FileName = @"Tools\blz.exe";
            unpack.StartInfo.Arguments = "-d " + '"' + overlayPath + '"';
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            unpack.StartInfo.CreateNoWindow = false;
            unpack.Start();
            unpack.WaitForExit();
            return unpack.ExitCode;
        }
    }
}