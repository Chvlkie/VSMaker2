using static VsMaker2Core.Enums;

namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class VsTrainersFile()
    {
        public List<Trainer> TrainerData { get; set; }
        public GameFamily GameFamily { get; set; }
        public List<byte[]> TrainerPartyFiles { get; set; }
        public List<byte[]> TrainerPropertyFiles { get; set; }
        public byte[] TrainerNamesFile { get; set; }
        public int ClassesCount { get; set; }
        public int BattleMessagesCount { get; set; }
    }
}