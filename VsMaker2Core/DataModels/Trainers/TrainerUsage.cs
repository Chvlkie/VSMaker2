using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    public class TrainerUsage
    {
        public int TrainerId { get; set; }
        public int FileId { get; set; }
        public int ReferenceId { get; set; }
        public TrainerUsageType TrainerUsageType { get; set; }

        public string ListItemName => TrainerUsageType switch
        {
            TrainerUsageType.Script => $"Script File: {FileId} |  Script #: {ReferenceId}",
            TrainerUsageType.Function => $"Script File: {FileId} |  Function #: {ReferenceId}",
            TrainerUsageType.Event => $"Event File: {FileId} |  Script #: {ReferenceId}",
            TrainerUsageType.Unknown => throw new NotImplementedException(),
            _ => "",
        };

        public TrainerUsage()
        { }

        public TrainerUsage(int trainerId, int fileId, int referenceId, TrainerUsageType trainerUsageType)
        {
            TrainerId = trainerId;
            FileId = fileId;
            ReferenceId = referenceId;
            TrainerUsageType = trainerUsageType;
        }
    }
}