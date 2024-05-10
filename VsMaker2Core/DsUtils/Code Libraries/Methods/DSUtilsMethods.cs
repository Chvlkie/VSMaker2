namespace VsMaker2Core
{
    public class DSUtilsMethods : IDSUtilsMethods
    {
        #region Read
        public byte[] ReadFromFile(string filePath, long startOffset = 0, long numberOfBytes = 0)
        {
            byte[] buffer = null;
            using (EasyReader reader = new(filePath, startOffset))
            {
                try
                {
                    buffer = reader.ReadBytes(numberOfBytes == 0 ? (int)(reader.BaseStream.Length - reader.BaseStream.Position) : (int)numberOfBytes);
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("End of FileStream");
                }
            }
            return buffer;
        }
        #endregion Read

        #region Write
        public void WriteToFile(string filePath, byte[] toOutput, uint writeAt = 0, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null, FileMode fileMode = FileMode.OpenOrCreate)
        {
            using EasyWriter writer = new(filePath, writeAt, fileMode);
            writer.Write(toOutput, indexFirstByteToWrite, indexLastByteToWrite is null ? toOutput.Length - indexFirstByteToWrite : (int)indexLastByteToWrite);
        }
        #endregion Write
    }
}