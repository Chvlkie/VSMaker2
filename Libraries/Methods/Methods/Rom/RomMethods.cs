using Data.DataModels.Rom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DataModels.Main;
using System.Formats.Asn1;
using DsUtils;
using Data.Databases;
using static Data.Global.Enums;
using Data.Global.Constants;
using System.Diagnostics;
using static Data.Global.Constants.FileSystem;
using DsUtils.DataModels;

namespace Methods.Rom
{
    public class RomMethods : IRomMethods
    {
        public GameVersion CheckGameVersion(RomData romData)
        {
            ArgumentNullException.ThrowIfNull(romData);
            return Database.GameVersions.GetValueOrDefault(romData.GameCode, GameVersion.Unknown);
        }

        public async Task ExtractRomContentsAsync(FileData fileData)
        {
            ArgumentNullException.ThrowIfNull(fileData);

            var processArgs = BuildExtractionArguments(fileData);

            using var unpackProcess = CreateExtractionProcess(processArgs);

            try
            {
                unpackProcess.Start();

                var outputTask = unpackProcess.StandardOutput.ReadToEndAsync();
                var errorTask = unpackProcess.StandardError.ReadToEndAsync();

                // Replace blocking calls with await
                await Task.WhenAll(outputTask, errorTask);
                await unpackProcess.WaitForExitAsync(); // Using custom extension (see below)

                if (unpackProcess.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"NDS extraction failed (exit code {unpackProcess.ExitCode}).\n" +
                        $"Error: {await errorTask}\n" +
                        $"Output: {await outputTask}");
                }

                if (!File.Exists(fileData.Arm9Path))
                {
                    throw new FileNotFoundException("ARM9 file not found", fileData.Arm9Path);
                }
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException("Failed to extract ROM contents", ex);
            }
            finally
            {
                await Task.Delay(1000);
            }
        }

        public RomData LoadInitialRomData(FileData fileData)
        {
            ArgumentNullException.ThrowIfNull(fileData);

            if (string.IsNullOrEmpty(fileData.FileName) ||
                string.IsNullOrEmpty(fileData.RomFilePath) ||
                string.IsNullOrEmpty(fileData.WorkingDirectory))
            {
                throw new ArgumentException("File Data properties cannot be null or empty", nameof(fileData));
            }

            if (!File.Exists(fileData.RomFilePath))
            {
                throw new FileNotFoundException("ROM file not found", fileData.RomFilePath);
            }

            if (!Directory.Exists(fileData.WorkingDirectory))
            {
                throw new DirectoryNotFoundException($"Working directory not found: {fileData.WorkingDirectory}");
            }

            try
            {
                using var reader = new EasyReader(fileData.RomFilePath, 0xC);

                return new RomData
                {
                    GameCode = Encoding.ASCII.GetString(reader.ReadBytes(4)),
                    EuropeByte = ReadEuropeByte(reader)
                };
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new InvalidOperationException("Failed to read ROM data", ex);
            }
        }

        public async Task UnpackNarcsAsync(FileData fileData, IProgress<int> progress)
        {
            ArgumentNullException.ThrowIfNull(fileData);
            if (fileData.NarcDirectories.Count == 0)
            {
                progress?.Report(100);
                return;
            }

            double progressStep = 100.0 / fileData.NarcDirectories.Count;
            double currentProgress = 0;
            try
            {
                for (int i = 0; i < fileData.NarcDirectories.Count; i++)
                {
                    string packedPath = fileData.PackedPaths[i];
                    string unpackedPath = fileData.UnpackedPaths[i];

                    Narc openedNarc = await Narc.OpenAsync(packedPath);
                    await Task.Delay(500);
                    if (openedNarc == null)
                    {
                        throw new Exception($"Failed to open NARC at path: {packedPath}");
                    }

                    await openedNarc.ExtractToFolderAsync(unpackedPath);
                    await Task.Delay(500);

                    if (Directory.GetFiles(unpackedPath).Length == 0)
                    {
                        throw new Exception($"Extraction failed for NARC at path: {packedPath}");
                    }

                    currentProgress += progressStep;
                    progress?.Report((int)currentProgress);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await Task.Delay(1000);
            }
        }

        private static string BuildExtractionArguments(FileData fileData)
        {
            string workingDir = fileData.WorkingDirectory;

            return new StringBuilder()
                .Append($"-x \"{fileData.RomFilePath}\"")
                .Append($" -9 \"{fileData.Arm9Path}\"")
                .Append($" -7 \"{Path.Combine(workingDir, FileSystem.Paths.Arm7FilePath)}\"")
                .Append($" -y9 \"{Path.Combine(workingDir, FileSystem.Paths.Y9FilePath)}\"")
                .Append($" -y7 \"{Path.Combine(workingDir, FileSystem.Paths.Y7FilePath)}\"")
                .Append($" -d \"{Path.Combine(workingDir, FileSystem.Paths.DataFilePath)}\"")
                .Append($" -y \"{Path.Combine(workingDir, FileSystem.Paths.OverlayFilePath)}\"")
                .Append($" -t \"{Path.Combine(workingDir, FileSystem.Paths.BannerFilePath)}\"")
                .Append($" -h \"{Path.Combine(workingDir, FileSystem.Paths.HeaderFilePath)}\"")
                .ToString();
        }

        private static Process CreateExtractionProcess(string arguments)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FileSystem.Paths.NdsToolsFilePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        private static byte ReadEuropeByte(EasyReader reader)
        {
            reader.BaseStream.Position = 0x1E;
            return reader.ReadByte();
        }
    }
}