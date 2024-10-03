namespace VsMaker2Core
{
    public class EasyReader : BinaryReader
    {
        public EasyReader(string path, long position = 0) : base(OpenFile(path))
        {
            try
            {
                if (position < 0 || position > BaseStream.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(position), "Position is out of bounds for the file.");
                }

                BaseStream.Position = position;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing EasyReader: {ex.Message}");
                throw;
            }
        }

        private static FileStream OpenFile(string path)
        {
            try
            {
                return File.OpenRead(path);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied to file: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error occurred: {ex.Message}");
                throw;
            }
        }
    }
}