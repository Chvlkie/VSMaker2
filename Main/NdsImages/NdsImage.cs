using Ekona.Images;
using Images;
using VsMaker2Core.DataModels;

namespace VsMaker2Core.Methods.NdsImages
{
    public class NdsImage : INdsImage
    {

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
            string nclrFilePath = Path.Combine(RomFile.TrainerGraphicsPath, nclrFileName);

            return new NCLR(nclrFilePath, nclrId, nclrFileName);
        }

        public ImageBase GetTrainerClassImageBase(int trainerClassId)
        {
            int baseIndex = trainerClassId * 5;
            int ncgrId = baseIndex;
            string ncgrFileName = ncgrId.ToString("D4");
            string ncgrFilePath = Path.Combine(RomFile.TrainerGraphicsPath, ncgrFileName);
            return new NCGR(ncgrFilePath, ncgrId, ncgrFileName);
        }

        public SpriteBase GetTrainerClassSpriteBase(int trainerClassId)
        {
            int baseIndex = trainerClassId * 5;

            int ncerId = baseIndex + 2;
            string ncerFileName = ncerId.ToString("D4");
            string ncerFilePath = Path.Combine(RomFile.TrainerGraphicsPath, ncerFileName);

            return new NCER(ncerFilePath, ncerId, ncerFileName);
        }
    }
}