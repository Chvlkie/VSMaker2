using Data.DataModels.Rom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Global.Enums;

namespace Methods.FileHandling
{
    public class FileMethods : IFileMethods
    {
        public Dictionary<NarcDirectory, (string packedPath, string unpackedPath)> SetNarcDirectories(
        string workingDirectory,
        RomData romData)
        {
            ArgumentNullException.ThrowIfNull(romData);
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                throw new ArgumentException("Working directory cannot be null or empty", nameof(workingDirectory));
            }

            var packedPaths = romData.GameFamily switch
            {
                GameFamily.DiamondPearl => GetDiamondPearlPaths(romData),
                GameFamily.Platinum => GetPlatinumPaths(romData),
                GameFamily.HeartGoldSoulSilver => GetHgSsPaths(),
                _ => throw new ArgumentException($"Unsupported GameFamily: {romData.GameFamily}")
            };

            return CreateDirectoryMappings(workingDirectory, packedPaths);
        }

        private static Dictionary<NarcDirectory, string> GetDiamondPearlPaths(RomData romData)
        {
            bool isPearl = romData.GameVersion == GameVersion.Pearl;
            bool isJapanese = romData.GameLanguage == GameLanguage.Japanese;

            return new()
            {
                [NarcDirectory.monIcons] = @"data\poketool\icongra\poke_icon.narc",
                [NarcDirectory.moveData] = @"data\poketool\waza\waza_tbl.narc",
                [NarcDirectory.personalPokeData] = isPearl
                    ? @"data\poketool\personal_pearl\personal.narc"
                    : @"data\poketool\personal\personal.narc",
                [NarcDirectory.eventFiles] = isJapanese
                    ? @"data\fielddata\eventdata\zone_event.narc"
                    : @"data\fielddata\eventdata\zone_event_release.narc",
                [NarcDirectory.scripts] = isJapanese
                    ? @"data\fielddata\script\scr_seq.narc"
                    : @"data\fielddata\script\scr_seq_release.narc",
                [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                [NarcDirectory.textArchives] = @"data\msgdata\msg.narc",
                [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc"
            };
        }

        private static Dictionary<NarcDirectory, string> GetPlatinumPaths(RomData romData)
        {
            string langPrefix = romData.GameVersion.ToString()[..2].ToLower();
            return new()
            {
                [NarcDirectory.monIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                [NarcDirectory.moveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                [NarcDirectory.personalPokeData] = @"data\poketool\personal\pl_personal.narc",
                [NarcDirectory.eventFiles] = @"data\fielddata\eventdata\zone_event.narc",
                [NarcDirectory.scripts] = @"data\fielddata\script\scr_seq.narc",
                [NarcDirectory.synthOverlay] = @"data\data\weather_sys.narc",
                [NarcDirectory.textArchives] = $@"data\msgdata\{langPrefix}_msg.narc",
                [NarcDirectory.trainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                [NarcDirectory.trainerParty] = @"data\poketool\trainer\trpoke.narc",
                [NarcDirectory.trainerProperties] = @"data\poketool\trainer\trdata.narc",
                [NarcDirectory.trainerTextOffset] = @"data\poketool\trmsg\trtblofs.narc",
                [NarcDirectory.trainerTextTable] = @"data\poketool\trmsg\trtbl.narc"
            };
        }

        private static Dictionary<NarcDirectory, string> GetHgSsPaths() => new()
        {
            [NarcDirectory.battleStagePokeData] = @"data\a\2\0\4",
            [NarcDirectory.battleTowerPokeData] = @"data\a\2\0\3",
            [NarcDirectory.battleTowerTrainerData] = @"data\a\2\0\2",
            [NarcDirectory.monIcons] = @"data\a\0\2\0",
            [NarcDirectory.eventFiles] = @"data\a\0\3\2",
            [NarcDirectory.moveData] = @"data\a\0\1\1",
            [NarcDirectory.personalPokeData] = @"data\a\0\0\2",
            [NarcDirectory.scripts] = @"data\a\0\1\2",
            [NarcDirectory.synthOverlay] = @"data\a\0\2\8",
            [NarcDirectory.textArchives] = @"data\a\0\2\7",
            [NarcDirectory.trainerGraphics] = @"data\a\0\5\8",
            [NarcDirectory.trainerParty] = @"data\a\0\5\6",
            [NarcDirectory.trainerProperties] = @"data\a\0\5\5",
            [NarcDirectory.trainerTextOffset] = @"data\a\1\3\1",
            [NarcDirectory.trainerTextTable] = @"data\a\0\5\7",
        };

        private static Dictionary<NarcDirectory, (string, string)> CreateDirectoryMappings(
            string workingDir,
            Dictionary<NarcDirectory, string> packedPaths)
        {
            var result = new Dictionary<NarcDirectory, (string, string)>(packedPaths.Count);
            string unpackedRoot = Path.Combine(workingDir, "unpacked");

            foreach (var (narcDir, packedPath) in packedPaths)
            {
                result[narcDir] = (
                    Path.Combine(workingDir, packedPath),
                    Path.Combine(unpackedRoot, narcDir.ToString())
                );
            }

            return result;
        }
    }
}
