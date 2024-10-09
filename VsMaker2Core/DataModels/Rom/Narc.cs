namespace VsMaker2Core.DataModels
{
    public class Narc
    { //Nitro Archive
        private const int FILE_ALLOCATION_TABLE_ELEMENT_LENGTH = 0x8;
        private const int FILE_ALLOCATION_TABLE_HEADER_LENGTH = 12;
        private const int FILE_ALLOCATION_TABLE_NUM_ELEMENTS_OFFSET = 0x18;
        private const int FILE_ALLOCATION_TABLE_OFFSET = 0x10;
        private const int FILE_IMAGE_HEADER_SIZE = 0x8;
        private const int FILE_NAME_TABLE_SIGNATURE_LENGTH = 0x4;
        private const int NARC_FILE_MAGIC_NUM = 0x4352414E;
        private MemoryStream[] Elements;
        private int FileNameTableOffset, FileImageOffset;

        //"NARC" in ascii/unicode
        private Narc(String name)
        {
            this.Name = name;
        }

        public String Name { get; set; }

        public MemoryStream this[int elemIndex]
        {
            get
            {
                return Elements[elemIndex];
            }
            set
            {
                Elements[elemIndex] = value;
            }
        }

        public static Narc FromFolder(String dirPath)
        {
            Narc narc = new Narc(Path.GetDirectoryName(dirPath));
            String[] fileNames = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            uint numberOfElements = (uint)fileNames.Length;
            narc.Elements = new MemoryStream[numberOfElements];

            Parallel.For(0, numberOfElements, i =>
            {
                FileStream fs = File.OpenRead(fileNames[i]);
                MemoryStream ms = new MemoryStream();
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                ms.Write(buffer, 0, (int)fs.Length);
                narc.Elements[i] = ms;
                fs.Close();
            });
            return narc;
        }

        public static Narc NewEmpty(String name = "NewNarc")
        {
            Narc narc = new Narc(name);
            return narc;
        }

        public static async Task<Narc> OpenAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            Narc narc = new Narc(Path.GetFileNameWithoutExtension(filePath));

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    // Read the magic number asynchronously
                    uint magicNum = await ReadUInt32Async(br.BaseStream);
                    if (magicNum != NARC_FILE_MAGIC_NUM)
                    {
                        return null;
                    }

                    await narc.ReadOffsetsAsync(br);
                    await narc.ReadElementsAsync(br);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Failed to open or read file \"{filePath}\": {ex.Message}");
                return null;
            }

            return narc;
        }

        public async Task<(bool Success, string ErrorMessage)> ExtractToFolderAsync(string dirPath, string extension = null)
        {
            if (string.IsNullOrWhiteSpace(dirPath))
            {
                return (false, $"Dir path \"{dirPath}\" is invalid.");
            }

            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                    Console.WriteLine($"Created NARC folder \"{dirPath}\".");
                }
                catch (IOException ex)
                {
                    return (false, $"NARC has not been extracted.\nCan't create directory: \n{dirPath}\nThis might be a temporary issue.\nMake sure no other process is using it and try again.\n{ex.Message}");
                }
            }

            var tasks = new List<Task>();

            for (int i = 0; i < Elements.Length; i++)
            {
                int index = i; // Capture the loop variable
                tasks.Add(Task.Run(async () =>
                {
                    string path = Path.Combine(dirPath, index.ToString("D4") + (string.IsNullOrWhiteSpace(extension) ? "" : extension));
                    try
                    {
                        long len = Elements[index].Length;
                        byte[] buffer = new byte[len];
                        Elements[index].Seek(0, SeekOrigin.Begin);
                        await Elements[index].ReadAsync(buffer, 0, (int)len);

                        using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                        {
                            await fs.WriteAsync(buffer, 0, (int)len);
                        }
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Failed to write file \"{path}\": {ex.Message}");
                    }
                }));
            }

            await Task.WhenAll(tasks);

            return (true, "");
        }

        public void Free() => Parallel.For(0, Elements.Length, i =>
                                       {
                                           Elements[i].Close();
                                       });

        public int GetElementsLength() => Elements.Length;

        public void Save(String filePath)
        {
            uint fileSizeOffset, fileImageSizeOffset, curOffset;

            BinaryWriter bw = new BinaryWriter(File.Create(filePath));
            // Write NARC Section
            bw.Write(NARC_FILE_MAGIC_NUM);
            bw.Write(0x0100FFFE);
            fileSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);
            bw.Write((ushort)16);                   //full size of header section
            bw.Write((ushort)3);                    //the number of sections in the header
            // Write FATB Section
            bw.Write(0x46415442);                   // "BTAF"
            bw.Write((UInt32)(FILE_ALLOCATION_TABLE_HEADER_LENGTH + Elements.Length * FILE_ALLOCATION_TABLE_ELEMENT_LENGTH));
            bw.Write((UInt32)Elements.Length);      // Number of elements
            curOffset = 0;
            for (int i = 0; i < Elements.Length; i++)
            {
                while (curOffset % 4 != 0)
                {
                    curOffset++;     // Force offsets to be a multiple of 4
                }

                bw.Write(curOffset);
                curOffset += (uint)Elements[i].Length;
                bw.Write(curOffset);
            }
            // Write FNTB Section (No names, sorry =( )
            bw.Write(0x464E5442);       //"BTNF"
            bw.Write(0x10);             //FNTB Size
            bw.Write(0x4);              //the offset of the first name directory
            bw.Write(0x10000);          //filler data describing a file at position 0 with 1 directory in the archive
            // Write FIMG Section
            bw.Write(0x46494D47);       // "GMIF"
            fileImageSizeOffset = (uint)bw.BaseStream.Position;
            bw.Write((UInt32)0x0);
            curOffset = 0;
            byte[] buffer;
            for (int i = 0; i < Elements.Length; i++)
            {
                while (curOffset % 4 != 0)
                { // Force offsets to be a multiple of 4
                    bw.Write((Byte)0xFF); curOffset++;
                }
                // Data writin'
                buffer = new byte[Elements[i].Length];
                Elements[i].Seek(0, SeekOrigin.Begin);
                Elements[i].Read(buffer, 0, (int)Elements[i].Length);
                bw.Write(buffer, 0, (int)Elements[i].Length);
                curOffset += (uint)Elements[i].Length;
            }
            // Writes sizes
            int fileSize = (int)bw.BaseStream.Position;
            bw.Seek((int)fileSizeOffset, SeekOrigin.Begin);         // File size
            bw.Write((UInt32)fileSize);
            bw.Seek((int)fileImageSizeOffset, SeekOrigin.Begin);         // seeks back to FIMG size
            bw.Write((UInt32)curOffset + FILE_IMAGE_HEADER_SIZE);   // FIMG size == Last end offset + File image header size
            bw.Close();
        }

        private static async Task ReadAsync(Stream stream, byte[] buffer)
        {
            int bytesRead = 0;
            int totalBytesRead = 0;
            while (totalBytesRead < buffer.Length)
            {
                bytesRead = await stream.ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);
                if (bytesRead == 0)
                {
                    throw new EndOfStreamException("Failed to read all bytes from the stream.");
                }
                totalBytesRead += bytesRead;
            }
        }

        private static async Task<uint> ReadUInt32Async(Stream stream)
        {
            byte[] buffer = new byte[4];
            int bytesRead = await stream.ReadAsync(buffer, 0, 4);
            if (bytesRead != 4)
            {
                throw new EndOfStreamException("Failed to read 4 bytes from the stream.");
            }
            return BitConverter.ToUInt32(buffer, 0);
        }

        private async Task ReadElementsAsync(BinaryReader br)
        {
            uint numberOfElements;
            uint[] startOffsets, endOffsets;

            // Create array of elements
            br.BaseStream.Position = FILE_ALLOCATION_TABLE_NUM_ELEMENTS_OFFSET;
            numberOfElements = await ReadUInt32Async(br.BaseStream);
            Elements = new MemoryStream[numberOfElements];

            // Read offsets of each element
            startOffsets = new uint[numberOfElements];
            endOffsets = new uint[numberOfElements];
            br.BaseStream.Position = FILE_ALLOCATION_TABLE_OFFSET + FILE_ALLOCATION_TABLE_HEADER_LENGTH;
            for (int i = 0; i < numberOfElements; i++)
            {
                startOffsets[i] = await ReadUInt32Async(br.BaseStream);
                endOffsets[i] = await ReadUInt32Async(br.BaseStream);
            }

            // Read elements
            for (int i = 0; i < numberOfElements; i++)
            {
                br.BaseStream.Position = FileImageOffset + startOffsets[i] + FILE_IMAGE_HEADER_SIZE;
                byte[] buffer = new byte[endOffsets[i] - startOffsets[i]];
                await ReadAsync(br.BaseStream, buffer);
                Elements[i] = new MemoryStream(buffer);
            }
        }

        private async Task ReadOffsetsAsync(BinaryReader br)
        {
            // Check the validity of the BinaryReader's BaseStream
            if (br.BaseStream == null)
            {
                throw new InvalidOperationException("The BinaryReader's BaseStream is null.");
            }

            // Read the first offset asynchronously
            br.BaseStream.Position = FILE_ALLOCATION_TABLE_NUM_ELEMENTS_OFFSET;
            uint fileNameTableOffset = await ReadUInt32Async(br.BaseStream);
            FileNameTableOffset = (int)fileNameTableOffset * FILE_ALLOCATION_TABLE_ELEMENT_LENGTH + FILE_ALLOCATION_TABLE_OFFSET + FILE_ALLOCATION_TABLE_HEADER_LENGTH;

            // Read the second offset asynchronously
            br.BaseStream.Position = FileNameTableOffset + FILE_NAME_TABLE_SIGNATURE_LENGTH;
            uint fileImageOffset = await ReadUInt32Async(br.BaseStream);
            FileImageOffset = (int)fileImageOffset + FileNameTableOffset;
        }
    }
}