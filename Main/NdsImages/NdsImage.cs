﻿using Ekona.Images;
using Images;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.NdsImages
{
    public class NdsImage : INdsImage
    {
        private static readonly string TrainerGraphicsPath = Database.VsMakerDatabase.RomData.GameDirectories[NarcDirectory.trainerGraphics].unpackedDirectory;

        public static void AddNewTrainerClassSprite()
        {
            var files = Directory.GetFiles(TrainerGraphicsPath);
            int totalFiles = files.Length;

            string[] filesToCopy = { "0000", "0001", "0002", "0003", "0004" };

            for (int i = 0; i < filesToCopy.Length; i++)
            {
                string sourceFile = Path.Combine(TrainerGraphicsPath, filesToCopy[i]);

                if (File.Exists(sourceFile))
                {
                    string newFileName = (totalFiles + i).ToString("D4"); // e.g., "0644" or "0645", etc.
                    string destinationFile = Path.Combine(TrainerGraphicsPath, newFileName);

                    File.Copy(sourceFile, destinationFile);

                    Console.WriteLine($"Copied {sourceFile} to {destinationFile}");
                }
                else
                {
                    Console.WriteLine($"File {filesToCopy[i]} does not exist in the directory.");
                }
            }

            Console.WriteLine("File duplication complete.");
        }

        public NdsImage()
        { }

        public Image GetTrainerClassSrite(PaletteBase palette, ImageBase image, SpriteBase sprite, int frameNumber)
        {
            int bank0OAMcount = sprite.Banks[0].oams.Length;

            int[] OAMenabled = new int[bank0OAMcount];
            for (int i = 0; i < OAMenabled.Length; i++)
            {
                OAMenabled[i] = i;
            }

            frameNumber = Math.Min(sprite.Banks.Length, frameNumber);
            return sprite.Get_Image(image, palette, frameNumber, 100, 100, false, false, false, true, true, -1, OAMenabled);
        }

        public PaletteBase GetTrainerClassPaletteBase(int trainerClassId)
        {
            int baseIndex = trainerClassId * 5;

            int nclrId = baseIndex + 1;
            string nclrFileName = nclrId.ToString("D4");
            string nclrFilePath = Path.Combine(TrainerGraphicsPath, nclrFileName);

            return new NCLR(nclrFilePath, nclrId, nclrFileName);
        }

        public ImageBase GetTrainerClassImageBase(int trainerClassId)
        {
            int baseIndex = trainerClassId * 5;
            int ncgrId = baseIndex;
            string ncgrFileName = ncgrId.ToString("D4");
            string ncgrFilePath = Path.Combine(TrainerGraphicsPath, ncgrFileName);
            return new NCGR(ncgrFilePath, ncgrId, ncgrFileName);
        }

        public SpriteBase GetTrainerClassSpriteBase(int trainerClassId)
        {
            int baseIndex = trainerClassId * 5;

            int ncerId = baseIndex + 2;
            string ncerFileName = ncerId.ToString("D4");
            string ncerFilePath = Path.Combine(TrainerGraphicsPath, ncerFileName);

            return new NCER(ncerFilePath, ncerId, ncerFileName);
        }
    }
}