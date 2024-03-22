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

        public Dictionary<DirectoryNames, (string packedDirectory, string unpackedDirectory)> SetNarcDirectories(string workingDirectory, GameVersion gameVersion, GameFamily gameFamily, GameLanguage gameLanguage)
        {
            Dictionary<DirectoryNames, string> packedDirectories = null;
            switch (gameFamily)
            {
                case GameFamily.DiamondPearl:
                    packedDirectories = new Dictionary<DirectoryNames, string>()
                    {
                        [DirectoryNames.PersonalPokeData] = gameVersion == GameVersion.Pearl ? @"data\poketool\personal_pearl\personal.narc" : @"data\poketool\personal\personal.narc",
                        [DirectoryNames.SynthOverlay] = @"data\data\weather_sys.narc",
                        [DirectoryNames.TextArchive] = @"data\msgdata\msg.narc",
                        [DirectoryNames.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [DirectoryNames.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [DirectoryNames.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [DirectoryNames.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [DirectoryNames.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [DirectoryNames.PokemonIcons] = @"data\poketool\icongra\poke_icon.narc",
                        [DirectoryNames.MoveData] = @"data\poketool\waza\waza_tbl.narc",
                    };
                    break;

                case GameFamily.Platinum:
                    packedDirectories = new Dictionary<DirectoryNames, string>()
                    {
                        [DirectoryNames.PersonalPokeData] = @"data\poketool\personal\pl_personal.narc",
                        [DirectoryNames.SynthOverlay] = @"data\data\weather_sys.narc",
                        [DirectoryNames.TextArchive] = @"data\msgdata\" + gameVersion.ToString().Substring(0, 2).ToLower() + '_' + "msg.narc",
                        [DirectoryNames.TrainerProperties] = @"data\poketool\trainer\trdata.narc",
                        [DirectoryNames.TrainerParty] = @"data\poketool\trainer\trpoke.narc",
                        [DirectoryNames.TrainerGraphics] = @"data\poketool\trgra\trfgra.narc",
                        [DirectoryNames.BattleMessageTable] = @"data\poketool\trmsg\trtbl.narc",
                        [DirectoryNames.BattleMessageOffset] = @"data\poketool\trmsg\trtblofs.narc",
                        [DirectoryNames.PokemonIcons] = @"data\poketool\icongra\pl_poke_icon.narc",
                        [DirectoryNames.MoveData] = @"data\poketool\waza\pl_waza_tbl.narc",
                    };
                    break;

                case GameFamily.HeartGoldSoulSilver:
                    packedDirectories = new Dictionary<DirectoryNames, string>()
                    {
                        [DirectoryNames.PersonalPokeData] = @"data\a\0\0\2",
                        [DirectoryNames.SynthOverlay] = @"data\a\0\2\8",
                        [DirectoryNames.TextArchive] = @"data\a\0\2\7",
                        [DirectoryNames.TrainerProperties] = @"data\a\0\5\5",
                        [DirectoryNames.TrainerParty] = @"data\a\0\5\6",
                        [DirectoryNames.TrainerGraphics] = @"data\a\0\5\8",
                        [DirectoryNames.BattleMessageTable] = @"data\a\0\5\7",
                        [DirectoryNames.BattleMessageOffset] = @"data\a\1\3\1",
                        [DirectoryNames.PokemonIcons] = @"data\a\0\2\0",
                        [DirectoryNames.MoveData] = @"data\a\0\1\1",
                    };
                    break;
            }

            var directories = new Dictionary<DirectoryNames, (string packedDirectory, string unpackedDirectory)>();
            foreach (KeyValuePair<DirectoryNames, string> kvp in packedDirectories)
            {
                directories.Add(kvp.Key, (workingDirectory + kvp.Value, workingDirectory + @"unpacked" + '\\' + kvp.Key.ToString()));
            }
            return directories;
        }
    }
}