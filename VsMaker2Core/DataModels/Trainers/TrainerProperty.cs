using System;
using System.Collections;
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

        public bool ChooseMoves { get; set; }
        public bool ChooseItems { get; set; }

        public byte TrainerClassId { get; set; }


        public ushort[] Items { get; set; }

        public bool DoubleBattle { get; set; }

        public byte TeamSize { get; set; }

        public TrainerProperty()
        {
            AIFlags = [];
        }
    }
}