using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Trainers
{
    public class Trainer
    {
        public uint TrainerId { get; set; }
        public string TrainerName { get; set; }
        public TrainerProperty TrainerProperties { get; set; }
        public TrainerParty TrainerParty { get; set; }

        public Trainer()
        {
            TrainerProperties = new();
            TrainerParty = new();
        }
    }
}