using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core.RomFiles
{
    public class ClassGenderData
    {
        public long Offset { get; set; }
        public byte Gender { get; set; }
        public int TrainerClassId { get; set; }
    }
}