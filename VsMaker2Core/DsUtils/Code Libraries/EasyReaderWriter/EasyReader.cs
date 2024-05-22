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
