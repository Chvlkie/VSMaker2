using Microsoft.VisualBasic;
using System.Diagnostics;
using VsMaker2Core.DataModels;
using VsMaker2Core.Glossary;
using static System.Net.Mime.MediaTypeNames;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.Rom
{
    public class RomFileMethods : IRomFileMethods
    {
        public GameFamily SetGameFamily(GameVersion gameVersion)
        {
            return gameVersion switch
            {
                GameVersion.Diamond or GameVersion.Pearl => GameFamily.DiamondPearl,
                GameVersion.Platinum => GameFamily.Platinum,
                GameVersion.HeartGold or GameVersion.SoulSilver => GameFamily.HeartGoldSoulSilver,
                GameVersion.HgEngine => GameFamily.HgEngine,
                _ => GameFamily.Unknown,
            };
        }

        public GameLanguage SetGameLanguage(string romId)
        {
            return romId switch
            {
                "ADAE" or "APAE" or "CPUE" or "IPKE" or "IPGE" => GameLanguage.English,
                "ADAS" or "APAS" or "CPUS" or "IPKS" or "IPGS" or "LATA" => GameLanguage.Spanish,
                "ADAI" or "APAI" or "CPUI" or "IPKI" or "IPGI" => GameLanguage.Italian,
                "ADAF" or "APAF" or "CPUF" or "IPKF" or "IPGF" => GameLanguage.French,
                "ADAD" or "APAD" or "CPUD" or "IPKD" or "IPGD" => GameLanguage.German,
                _ => GameLanguage.Japanese,
            };
        }

        public Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)> SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage)
        {
            Dictionary<NarcDirectory, string> packedDirectories = null;
            switch (gameFamily)
            {
                case GameFamily.DiamondPearl:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.PersonalPokeData] = gameVersion == GameVersion.Pearl ? @"data\poketool\personal_pearl\personal.narc" : @"data\poketool\personal\personal.narc",
                        [NarcDirectory.SynthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.TextArchive] = @"data\msgdata\msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.PokemonIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [NarcDirectory.MoveData] = @"data\poketool\waza\waza_tbl.narc",
                    };
                    break;

                case GameFamily.Platinum:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [NarcDirectory.SynthOverlay] = @"data\data\weather_sys.narc",
                        [NarcDirectory.TextArchive] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",
                        [NarcDirectory.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [NarcDirectory.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [NarcDirectory.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [NarcDirectory.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [NarcDirectory.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [NarcDirectory.PokemonIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                        [NarcDirectory.MoveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                    };
                    break;

                case GameFamily.HeartGoldSoulSilver:
                    packedDirectories = new Dictionary<NarcDirectory, string>()
                    {
                        [NarcDirectory.BattleStagePokeData] = @"data\a\2\0\4",
                        [NarcDirectory.BattleTowerPokeData] = @"data\a\2\0\3",
                        [NarcDirectory.BattleTowerTrainerData] = @"data\a\2\0\2",
                        [NarcDirectory.PersonalPokeData] = @"data\a\0\0\2",
                        [NarcDirectory.SynthOverlay] = @"data\a\0\2\8",
                        [NarcDirectory.TextArchive] = @"data\a\0\2\7",
                        [NarcDirectory.TrainerProperties] = @"data\a\0\5\5",
                        [NarcDirectory.TrainerParty] = @"data\a\0\5\6",
                        [NarcDirectory.TrainerGraphics] = @"data\a\0\5\8",
                        [NarcDirectory.BattleMessageTable] = @"data\a\0\5\7",
                        [NarcDirectory.BattleMessageOffset] = @"data\a\1\3\1",
                        [NarcDirectory.PokemonIcons] = @"data\a\0\2\0",
                        [NarcDirectory.MoveData] = @"data\a\0\1\1",
                    };
                    break;
            }

            var directories = new Dictionary<NarcDirectory, (string packedDirectory, string unpackedDirectory)>();
            foreach (KeyValuePair<NarcDirectory, string> kvp in packedDirectories)
            {
                directories.Add(kvp.Key, (workingDirectory + kvp.Value, workingDirectory + @"unpacked" + '\\' + kvp.Key.ToString()));
            }
            return directories;
        }

        public (bool Success, string ExceptionMessage) ExtractRomContents(string workingDirectory, string fileName)
        {
            Process unpack = new();
            unpack.StartInfo.FileName = GlobalConstants.NdsToolsFilePath;
            unpack.StartInfo.Arguments = "-x " + '"' + fileName + '"'
                + " -9 " + '"' + workingDirectory + "\\" + GlobalConstants.Arm9FilePath + '"'
                + " -7 " + '"' + workingDirectory + "\\" + GlobalConstants.Arm7FilePath + '"'
                + " -y9 " + '"' + workingDirectory + "\\" + GlobalConstants.Y9FilePath + '"'
                + " -y7 " + '"' + workingDirectory + "\\" + GlobalConstants.Y7FilePath + '"'
                + " -d " + '"' + workingDirectory + "\\" + GlobalConstants.DataFilePath + '"'
                + " -y " + '"' + workingDirectory + "\\" + GlobalConstants.OverlayFilePath + '"'
                + " -t " + '"' + workingDirectory + "\\" + GlobalConstants.BannerFilePath + '"'
                + " -h " + '"' + workingDirectory + "\\" + GlobalConstants.HeaderFilePath + '"';
            unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            unpack.StartInfo.CreateNoWindow = true;
            try
            {
                unpack.Start();
                unpack.WaitForExit();
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                return (false, ex.Message);
            }
            return (true, "");
        }

        private (bool Succes, string ExceptionMessage) UnpackNarc(RomFile romFile, NarcDirectory narcPath)
        {
            try
            {
                if (romFile.Directories.TryGetValue(narcPath, out (string packedPath, string unpackedPath) paths))
                {
                    DirectoryInfo directoryInfo = new(paths.unpackedPath);
                    if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0)
                    {
                        Narc openedNarc = Narc.Open(paths.packedPath);
                        if (openedNarc == null) { throw new NullReferenceException(); }
                        else
                        {
                            openedNarc.ExtractToFolder(paths.unpackedPath);
                        }
                    }
                    return (true, "");
                }
                else
                {
                    return (false, $"Error unpacking \"{paths.packedPath}\" - \"{narcPath}\"\n\nNarc not found in dictionary.");
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool Success, string ExceptionMessage) UnpackNarcs(RomFile romFile, List<NarcDirectory> narcs, IProgress<int> progress)
        {
            int progressStep = 100 / narcs.Count;
            int count = 0;

            foreach (var item in narcs)
            {
                var (success, exceptionMessage) = UnpackNarc(romFile, item);
                if (success)
                {
                    progress?.Report(count += progressStep);
                }
                else
                {
                    progress?.Report(100);
                    return (false, exceptionMessage);
                }
            }
            return (true, null);
        }
    }
}