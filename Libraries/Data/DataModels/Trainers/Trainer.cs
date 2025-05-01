using Data.DataModels.TrainerTexts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataModels.Trainers
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Trainer
    {
        [JsonProperty("id")]
        [DefaultValue(0u)]
        public ushort TrainerId { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        [DisallowNull]
        public string TrainerName { get; set; }

        [JsonProperty("data", Required = Required.Always)]
        [DisallowNull]
        public TrainerData TrainerData { get; set; }

        [JsonProperty("party", Required = Required.Always)]
        [DisallowNull]
        public PartyData PartyData { get; set; }

        [JsonProperty("texts", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<TrainerText> TrainerTexts { get; set; } = [];

        [SetsRequiredMembers]
        public Trainer(ushort trainerId, string trainerName, TrainerData trainerData, PartyData partyData)
        {
            TrainerId = trainerId;
            TrainerName = trainerName ?? throw new ArgumentNullException(nameof(trainerName));
            TrainerData = trainerData ?? throw new ArgumentNullException(nameof(trainerData));
            PartyData = partyData ?? throw new ArgumentNullException(nameof(partyData));
        }

        public Trainer()
        { }

        public bool ShouldSerializeTrainerTexts() => TrainerTexts?.Count > 0;

        public bool Validate()
        {
            return !string.IsNullOrWhiteSpace(TrainerName)
                && TrainerData != null
                && PartyData != null;
        }
    }
}