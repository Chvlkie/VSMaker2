﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VsMaker2Core.DataModels;

namespace VsMaker2Core.Methods
{
    public interface IClassEditorMethods
    {
        List<TrainerClass> GetTrainerClasses(int classMessageArchive);
    }
}
