using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.DataModels
{
    [Serializable]
    public class TrainerProperty
    {
        public List<bool> AIFlags { get; set; }
        public uint? Item1Id { get; set; }
        public uint? Item2Id { get; set; }
        public uint? Item3Id { get; set; }
        public uint? Item4Id { get; set; }

        public TrainerProperty()
        {
            AIFlags = [];
        }
    }
}
