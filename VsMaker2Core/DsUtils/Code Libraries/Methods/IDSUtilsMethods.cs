namespace VsMaker2Core
{
    public interface IDSUtilsMethods
    {
        #region Read
        /// <summary>
        /// Read bytes from a given filePath, starting at startOffset for numberOfBytes.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="startOffset"></param>
        /// <param name="numberOfBytes"></param>
        /// <returns>byte[]</returns>
        byte[] ReadFromFile(string filePath, long startOffset = 0, long numberOfBytes = 0);

        #endregion Read

        #region Write
        /// <summary>
        /// Write to given filePath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="toOutput"></param>
        /// <param name="writeAt"></param>
        /// <param name="indexFirstByteToWrite"></param>
        /// <param name="indexLastByteToWrite"></param>
        /// <param name="fileMode"></param>
        void WriteToFile(string filePath, byte[] toOutput, uint writeAt = 0, int indexFirstByteToWrite = 0, int? indexLastByteToWrite = null, FileMode fileMode = FileMode.OpenOrCreate);
        #endregion Write
    }
}