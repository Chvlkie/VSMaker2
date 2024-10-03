namespace VsMaker2Core
{
    public class EasyWriter : BinaryWriter
    {
        public EasyWriter(string path, long position = 0, FileMode fileMode = FileMode.OpenOrCreate)
            : base(OpenFileStream(path, fileMode))
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
                Console.WriteLine($"Error initializing EasyWriter: {ex.Message}");
                throw;
            }
        }

        public void EditSize(int increment)
        {
            try
            {
                long newSize = BaseStream.Length + increment;

                if (newSize < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(increment), "File size cannot be negative.");
                }

                BaseStream.SetLength(newSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing file size: {ex.Message}");
                throw;
            }
        }

        private static FileStream OpenFileStream(string path, FileMode fileMode)
        {
            try
            {
                return new FileStream(path, fileMode, FileAccess.Write, FileShare.None);
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