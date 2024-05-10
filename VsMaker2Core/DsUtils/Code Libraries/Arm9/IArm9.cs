using static VsMaker2Core.Enums;

namespace VsMaker2Core
{
    public interface IArm9
    {
        bool Arm9Compress(string path);

        bool Arm9Decompress(string path);

        void Arm9EditSize(int increment);

        bool CheckCompressionMark(GameFamily gameFamily);

        byte[] ReadBytes(uint startOffset, long numberOfBytes = 0);

        void WriteBytes(byte[] bytesToWrite, uint destinationOffset, int indexFirstByte = 0, int? indexLastByte = null);

        byte ReadByte(uint startOffset);

        void WriteByte(byte value, uint destinationOffset);
    }
}