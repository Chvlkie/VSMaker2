using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsMaker2Core
{
    public class EasyReader : BinaryReader
    {
        public EasyReader(string path, long position = 0) : base(File.OpenRead(path))
        {
            BaseStream.Position = position;
        }
    }
}
