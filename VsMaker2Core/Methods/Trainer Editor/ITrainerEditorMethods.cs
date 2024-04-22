using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods
{
    public interface ITrainerEditorMethods
    {
        List<Trainer> GetTrainers(int trainerMessageArchive);
    }
}
