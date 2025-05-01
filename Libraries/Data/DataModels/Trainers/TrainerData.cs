using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModels.Trainers
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class TrainerData
    {
        [JsonProperty("typeId")]
        [DefaultValue(0u)]
        public uint TrainerTypeId { get; set; }

        [JsonProperty("partySize")]
        [DefaultValue(0)]
        public byte PartySize { get; set; }

        [JsonProperty("battleItems")]
        [DefaultValue(null)]
        public uint[] BattleItems { get; set; } = new uint[4];

        [JsonProperty("propertyFlags")]
        public List<bool> PropertyFlags { get; set; } = [];

        [JsonProperty("aiFlags")]
        public List<bool> AiFlags { get; set; } = [true];

        public TrainerData()
        {
            BattleItems = [0, 0, 0, 0];
            PropertyFlags = [];
            AiFlags = [true];
        }

        public bool ShouldSerializeBattleItems() => BattleItems?.Any(x => x != 0) ?? false;
        public bool ShouldSerializePropertyFlags() => PropertyFlags?.Count > 0;
        public bool ShouldSerializeAiFlags() => AiFlags?.Count != 1 || !AiFlags[0];
    }
}
