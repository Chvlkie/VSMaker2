using System.Diagnostics;
using VsMaker2Core.DataModels;
using static System.Net.Mime.MediaTypeNames;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.DsUtils
{
    public static class Overlay
    {
        private const string backupSuffix = ".backup";
        public const int ERR_OVERLAY_NOTFOUND = -1;
        public const int ERR_OVERLAY_ALREADY_UNCOMPRESSED = -2;
        public static bool CheckOverlayHasCompressionFlag(int overlayNumber)
        {
            const int entrySize = 32;
            const int compressionFlagOffset = 31;

            try
            {
                using FileStream fileStream = File.Open(RomFile.OverlayTablePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using BinaryReader reader = new(fileStream);

                long requiredPosition = (overlayNumber * entrySize) + compressionFlagOffset;

                if (reader.BaseStream.Length < requiredPosition + 1)
                {
                    throw new InvalidOperationException($"Overlay number {overlayNumber} is out of bounds.");
                }

                reader.BaseStream.Position = requiredPosition;
                byte flagByte = reader.ReadByte();

                return flagByte % 2 != 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking compression flag for overlay {overlayNumber}: {ex.Message}");
                throw;
            }
        }

        public static async Task<bool> CheckOverlayIsCompressedAsync(int overlayNumber)
        {
            string overlayFilePath = OverlayFilePath(overlayNumber);

            try
            {
                var fileInfo = new FileInfo(overlayFilePath);

                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException($"Overlay file not found at path: {overlayFilePath}");
                }

                long fileSize = fileInfo.Length;

                long uncompressedSize = await GetOverlayUncompressedSizeAsync(overlayNumber);

                return fileSize < uncompressedSize;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if overlay {overlayNumber} is compressed: {ex.Message}");
                throw;
            }
        }

        public static async Task<int> DecompressOverlayAsync(int overlayNumber, bool makeBackup = true)
        {
            string overlayPath = OverlayFilePath(overlayNumber);

            if (!File.Exists(overlayPath))
            {
                throw new FileNotFoundException($"Overlay path not found: {overlayPath}");
            }

            if (makeBackup)
            {
                string backupPath = overlayPath + "_backup";

                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Copy(overlayPath, backupPath);
            }

            using Process unpack = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"Tools\blz.exe",
                    Arguments = "-d " + '"' + overlayPath + '"',
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = false
                }
            };

            try
            {
                unpack.Start();

                await unpack.WaitForExitAsync();

                return unpack.ExitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decompressing overlay {overlayNumber}: {ex.Message}");
                throw;
            }
        }

        public static async Task<uint> GetOverlayUncompressedSizeAsync(int overlayNumber)
        {
            using var fileStream = new FileStream(RomFile.OverlayTablePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
            using var reader = new BinaryReader(fileStream);

            try
            {
                reader.BaseStream.Position = (overlayNumber * 32) + 8; // overlayNumber * size of entry + offset
                return await Task.FromResult(reader.ReadUInt32());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the uncompressed size for overlay {overlayNumber}: {ex.Message}");
                throw;
            }
        }

        public static string OverlayFilePath(int overlayNumber)
        {
            return $"{RomFile.WorkingDirectory}overlay\\overlay_{overlayNumber:D4}.bin";
        }

        public static async Task<(bool Success, string Error)> RestoreOverlayFromCompressedBackupAsync(int overlayNumber, bool eventEditorIsReady)
        {
            var overlayFilePath = OverlayFilePath(overlayNumber);
            string backupFilePath = overlayFilePath + backupSuffix;

            try
            {
                if (File.Exists(backupFilePath))
                {
                    FileInfo overlayFile = new(overlayFilePath);
                    FileInfo backupFile = new(backupFilePath);

                    if (overlayFile.Length <= backupFile.Length)
                    {
                        string message = $"Overlay {overlayNumber} is already compressed.";
                        Console.WriteLine(message);
                        return (false, message);
                    }
                    else
                    {
                        // Delete and move file asynchronously
                        await Task.Run(() => File.Delete(overlayFilePath));
                        await Task.Run(() => File.Move(backupFilePath, overlayFilePath));
                    }
                }
                else
                {
                    string msg = $"Overlay File {backupFilePath} couldn't be found and restored.";
                    Console.WriteLine(msg);
                    return (false, msg);
                }

                return (true, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring overlay from backup: {ex.Message}");
                return (false, ex.Message);
            }
        }

        public static void SetOverlayCompressionInTable(int overlayNumber, byte compression)
        {
            if (compression > 3)
            {
                Console.WriteLine("Can't compress, invalid compression value.");
                return;
            }

            try
            {
                using FileStream fs = new(RomFile.OverlayTablePath, FileMode.Open, FileAccess.Write);
                using BinaryWriter writer = new(fs);

                writer.BaseStream.Position = overlayNumber * 32 + 31;
                writer.Write(compression);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO error occurred: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public static async Task<(bool Success, string Error, int ExitCode)> CompressOverlayAsync(int overlayNumber)
        {
            string overlayFilePath = OverlayFilePath(overlayNumber);

            if (!File.Exists(overlayFilePath))
            {
                string message = $"Overlay to compress #{overlayNumber} doesn't exist";
                return (false, message, ERR_OVERLAY_NOTFOUND);
            }

            try
            {
                var tcs = new TaskCompletionSource<(bool Success, string Error, int ExitCode)>();

                using Process compress = new()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = @"Tools\blz.exe",
                        Arguments = "-en " + '"' + overlayFilePath + '"',
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    },
                    EnableRaisingEvents = true
                };

                compress.Exited += (sender, args) =>
                {
                    string errorOutput = compress.StandardError.ReadToEnd();
                    string standardOutput = compress.StandardOutput.ReadToEnd();
                    bool success = compress.ExitCode == 0;
                    string message = success ? standardOutput : errorOutput;
                    tcs.SetResult((success, message, compress.ExitCode));
                    compress.Dispose();
                };

                compress.Start();

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                return (false, $"Error during compression: {ex.Message}", -1);
            }
        }
    }
}