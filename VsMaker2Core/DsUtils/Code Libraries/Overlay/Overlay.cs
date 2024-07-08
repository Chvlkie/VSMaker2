using System.Diagnostics;
using VsMaker2Core.DataModels;
using static System.Net.Mime.MediaTypeNames;
namespace VsMaker2Core.DsUtils
{
    public static class Overlay
    {
        private const string backupSuffix = ".backup";
        public const int ERR_OVERLAY_NOTFOUND = -1;
        public const int ERR_OVERLAY_ALREADY_UNCOMPRESSED = -2;
        public static bool CheckOverlayHasCompressionFlag(int overlayNumber)
        {
            using BinaryReader f = new(File.OpenRead(RomFile.OverlayTablePath));
            f.BaseStream.Position = (overlayNumber * 32) + 31; //overlayNumber * size of entry + offset
            return f.ReadByte() % 2 != 0;
        }

        public static bool CheckOverlayIsCompressed(int overlayNumber)
        {
            return new FileInfo(OverlayFilePath(overlayNumber)).Length < GetOverlayUncompressedSize(overlayNumber);
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

        public static uint GetOverlayUncompressedSize(int overlayNumber)
        {
            using BinaryReader reader = new(File.OpenRead(RomFile.OverlayTablePath));
            {
                reader.BaseStream.Position = (overlayNumber * 32) + 8; //overlayNumber * size of entry + offset
                return reader.ReadUInt32();
            }
        }

        public static string OverlayFilePath(int overlayNumber)
        {
            return $"{RomFile.WorkingDirectory}overlay\\overlay_{overlayNumber:D4}.bin";
        }
        public static (bool Success, string Error) RestoreOverlayFromCompressedBackup(int overlayNumber, bool eventEditorIsReady)
        {
            var overlayFilePath = OverlayFilePath(overlayNumber);

            if (File.Exists(overlayFilePath + backupSuffix))
            {
                if (new FileInfo(overlayFilePath).Length <= new FileInfo(overlayFilePath + backupSuffix).Length)
                { //if overlay is bigger than its backup
                    string message = $"Overlay {overlayNumber} is already compressed.";
                    Console.WriteLine(message);
                    return (false, message);
                }
                else
                {
                    File.Delete(overlayFilePath);
                    File.Move(overlayFilePath + backupSuffix, overlayFilePath);
                }
            }
            else
            {
                string msg = $"Overlay File {overlayFilePath} {backupSuffix} couldn't be found and restored."; 
                Console.WriteLine(msg);

               return (false, msg);
            }
            return (true, "");
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

        public static async Task<(bool Success, string Error, int ExitCode)> CompressOverlay(int overlayNumber)
        {
            string overlayFilePath = OverlayFilePath(overlayNumber);

            if (!File.Exists(overlayFilePath))
            {
                string message = "Overlay to decompress #" + overlayNumber + " doesn't exist";
                return (false, message, ERR_OVERLAY_NOTFOUND);
            }

            var tcs = new TaskCompletionSource<(bool Success, string Error, int ExitCode)>();
            Process compress = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"Tools\blz.exe",
                    Arguments = "-en " + '"' + overlayFilePath + '"',
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            compress.Exited += (sender, args) =>
            {
                bool success = compress.ExitCode == 0;
                tcs.SetResult((success, "", compress.ExitCode));
                compress.Dispose();
            };

            compress.Start();
            return await tcs.Task;
        }
    }
}