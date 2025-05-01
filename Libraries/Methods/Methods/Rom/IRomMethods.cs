using Data.DataModels.Main;
using Data.DataModels.Rom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Data.Global.Enums;

namespace Methods.Rom
{
    public interface IRomMethods
    {
        GameVersion CheckGameVersion(RomData romData);
        Task ExtractRomContentsAsync(FileData fileData);
        RomData LoadInitialRomData(FileData fileData);
        Task UnpackNarcsAsync(FileData fileData, IProgress<int> progress);
    }
}
