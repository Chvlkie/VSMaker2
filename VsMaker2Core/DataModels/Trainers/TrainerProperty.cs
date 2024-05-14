using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.DataModels
{
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
            Items = [0, 0, 0, 0];
            TrainerClassId = 2;
        }

        public TrainerProperty(bool doubleBattle, byte teamSize, bool chooseMoves, bool chooseItems, byte trainerClassId, ushort[] items, List<bool> aiFlags)
        {
            AIFlags = aiFlags;
            Items = items;
            TeamSize = teamSize;
            ChooseMoves = chooseMoves;
            ChooseItems = chooseItems;
            TrainerClassId = trainerClassId;
            DoubleBattle = doubleBattle;
        }
    }
}