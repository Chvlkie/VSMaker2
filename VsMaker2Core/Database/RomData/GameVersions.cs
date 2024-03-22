using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Database
{
    public partial class VsMakerDatabase
    {
        public static class RomData
        {
            public static Dictionary<string, GameVersion> GameVersions = new Dictionary<string, GameVersion>()
            {
                ["ADAE"] = GameVersion.Diamond,
                ["ADAS"] = GameVersion.Diamond,
                ["ADAI"] = GameVersion.Diamond,
                ["ADAF"] = GameVersion.Diamond,
                ["ADAD"] = GameVersion.Diamond,
                ["ADAJ"] = GameVersion.Diamond,

                ["APAE"] = GameVersion.Pearl,
                ["APAS"] = GameVersion.Pearl,
                ["APAI"] = GameVersion.Pearl,
                ["APAF"] = GameVersion.Pearl,
                ["APAD"] = GameVersion.Pearl,
                ["APAJ"] = GameVersion.Pearl,

                ["CPUE"] = GameVersion.Platinum,
                ["CPUS"] = GameVersion.Platinum,
                ["CPUI"] = GameVersion.Platinum,
                ["CPUF"] = GameVersion.Platinum,
                ["CPUD"] = GameVersion.Platinum,
                ["CPUJ"] = GameVersion.Platinum,
                ["CPUP"] = GameVersion.Platinum,

                ["IPKE"] = GameVersion.HeartGold,
                ["IPKS"] = GameVersion.HeartGold,
                ["IPKI"] = GameVersion.HeartGold,
                ["IPKF"] = GameVersion.HeartGold,
                ["IPKD"] = GameVersion.HeartGold,
                ["IPKJ"] = GameVersion.HeartGold,

                ["IPGE"] = GameVersion.SoulSilver,
                ["IPGS"] = GameVersion.SoulSilver,
                ["IPGI"] = GameVersion.SoulSilver,
                ["IPGF"] = GameVersion.SoulSilver,
                ["IPGD"] = GameVersion.SoulSilver,
                ["IPGJ"] = GameVersion.SoulSilver
            };
        }
    }
}
