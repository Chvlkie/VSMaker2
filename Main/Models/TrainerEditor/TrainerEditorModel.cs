using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;

namespace Main.Models
{
    public class TrainerEditorModel
    {
        
        public List<Trainer> Trainers { get; set; }
        public int SelectedTrainerId { get; set; }
        public ViewTrainerDataModel TrainerData { get; set; }
    }
}
