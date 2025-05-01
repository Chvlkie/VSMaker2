using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsUtils
{
    public sealed class EasyReader : BinaryReader
    {
        private const int OptimalBufferSize = 8192;
        private const FileOptions FileStreamOptions = FileOptions.SequentialScan | FileOptions.Asynchronous;

        public EasyReader(string filePath, long position = 0)
            : base(CreateOptimizedFileStream(filePath), Encoding.ASCII, leaveOpen: false)
        {
            ValidatePosition(position);
            BaseStream.Position = position;
        }

        private static FileStream CreateOptimizedFileStream(string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? throw new ArgumentException("File path cannot be null or empty", nameof(path))
                : new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                OptimalBufferSize,
                FileStreamOptions);
        }

        private void ValidatePosition(long position)
        {
            if (position < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position cannot be negative");
            }

            if (position > 0 && position > BaseStream.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "Position exceeds file length");
            }
        }

        public byte[] ReadBytesAt(long position, int count)
        {
            ValidatePosition(position);
            BaseStream.Position = position;
            return ReadBytes(count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Dispose error: {ex.Message}");
                throw;
            }
        }
    }
}