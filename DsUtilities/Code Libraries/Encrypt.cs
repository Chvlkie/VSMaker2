using System.Text;
using VsMaker2Core.Database;
using static VsMaker2Core.Enums;

namespace DSUtils
{
    public class Encrypt
    {
        public List<string> DecryptTextArchive(int textArchive)
        {
            List<string> messages = [];
            string textArchivePath = VsMakerDatabase.RomData.GameDirectories[NarcDirectory.TextArchive].unpackedDirectory;
            FileStream memoryStream = new($"{textArchivePath}\\{textArchive:D4}", FileMode.Open);
            BinaryReader readText = new(memoryStream);
            int stringCount;
            int initialKey;
            try
            {
                stringCount = readText.ReadUInt16();
                initialKey = readText.ReadUInt16();
            }
            catch (EndOfStreamException)
            {
                memoryStream.Close();
                return new List<string>();
                throw;
            }

            int key1 = (initialKey * 0x2FD) & 0xFFFF;
            int[] currentOffset = new int[stringCount];
            int[] currentSize = new int[stringCount];

            for (int i = 0; i < stringCount; i++)
            { // Reads and stores string offsets and sizes 
                int key2 = (key1 * (i + 1) & 0xFFFF);
                int realKey = key2 | (key2 << 16);
                currentOffset[i] = ((int)readText.ReadUInt32()) ^ realKey;
                currentSize[i] = ((int)readText.ReadUInt32()) ^ realKey;
            }

            for (int i = 0; i < stringCount; i++)
            { // Adds new string
                bool hasFormatCharacter = false;
                bool isCompressed = false;
                key1 = (0x91BD3 * (i + 1)) & 0xFFFF;
                readText.BaseStream.Position = currentOffset[i];
                StringBuilder decodedText = new("");

                for (int j = 0; j < currentSize[i]; j++) // Adds new characters to string
                {
                    int car = (readText.ReadUInt16()) ^ key1;

                    switch (car)
                    { // Special characters
                        case 0xE000:
                            decodedText.Append(@"\n");
                            break;
                        case 0x25BC:
                            decodedText.Append(@"\r");
                            break;
                        case 0x25BD:
                            decodedText.Append(@"\f");
                            break;
                        case 0xF100:
                            isCompressed = true;
                            break;
                        case 0xFFFE:
                            decodedText.Append(@"\v");
                            hasFormatCharacter = true;
                            break;
                        case 0xFFFF:
                            decodedText.Append("");
                            break;
                        default:
                            if (hasFormatCharacter)
                            {
                                decodedText.Append(car.ToString("X4"));
                                hasFormatCharacter = false;
                            }
                            else if (isCompressed)
                            {
                                int shift = 0;
                                int trans = 0;
                                const string uncomp = "";
                                while (true)
                                {
                                    int tmp = car >> shift;
                                    int tmp1 = tmp;
                                    if (shift >= 0xF)
                                    {
                                        shift -= 0xF;
                                        if (shift > 0)
                                        {
                                            tmp1 = (trans | ((car << (9 - shift)) & 0x1FF));
                                            if ((tmp1 & 0xFF) == 0xFF)
                                            {
                                                break;
                                            }
                                            if (tmp1 != 0x0 && tmp1 != 0x1)
                                            {
                                                if (!VsMakerDatabase.RomData.TextCharacters.ReadTextDictionary.TryGetValue(tmp1, out string character))
                                                {
                                                    decodedText.Append("\\x").Append(tmp1.ToString("X4"));
                                                }
                                                else
                                                {
                                                    decodedText.Append(character);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        tmp1 = ((car >> shift) & 0x1FF);
                                        if ((tmp1 & 0xFF) == 0xFF)
                                        {
                                            break;
                                        }
                                        if (tmp1 != 0x0 && tmp1 != 0x1)
                                        {
                                            if (!VsMakerDatabase.RomData.TextCharacters.ReadTextDictionary.TryGetValue(tmp1, out string character))
                                            {
                                                decodedText.Append("\\x").Append(tmp1.ToString("X4"));
                                            }
                                            else
                                            {
                                                decodedText.Append(character);
                                            }
                                        }
                                        shift += 9;
                                        if (shift < 0xF)
                                        {
                                            trans = ((car >> shift) & 0x1FF);
                                            shift += 9;
                                        }
                                        key1 += 0x493D;
                                        key1 &= 0xFFFF;
                                        car = Convert.ToUInt16(readText.ReadUInt16() ^ key1);
                                        j++;
                                    }
                                }
                                decodedText.Append(uncomp);
                            }
                            else if (VsMakerDatabase.RomData.TextCharacters.ReadTextDictionary.TryGetValue(car, out string character))
                            {
                                decodedText.Append(character);
                            }
                            else
                            {
                                decodedText.Append("\\x").Append(car.ToString("X4"));
                            }

                            break;
                    }
                    key1 += 0x493D;
                    key1 &= 0xFFFF;
                }
                messages.Add(decodedText.ToString());
            }
            readText.Dispose();
            return messages;
        }
    }
}
