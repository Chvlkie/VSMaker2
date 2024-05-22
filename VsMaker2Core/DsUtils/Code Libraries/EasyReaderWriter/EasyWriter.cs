namespace VsMaker2Core
{
    public class EasyWriter : BinaryWriter
    {
        public EasyWriter(string path, long position = 0, FileMode fileMode = FileMode.OpenOrCreate) : base(new FileStream(path, fileMode,  FileAccess.Write, FileShare.None))
        {
            BaseStream.Position = position;
        }

        public void EditSize(int increment)
        {
            BaseStream.SetLength(BaseStream.Length + increment);
        }
    }
}
