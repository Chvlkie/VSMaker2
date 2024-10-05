using Ekona.Images;
using Images;
using static Images.NCSC.sNCSC;
using static VsMaker2Core.Enums;

namespace VsMaker2Core.Methods.NdsImages
{
    public interface INdsImage
    {
        ImageBase GetTrainerClassImageBase(int trainerClassId);
        PaletteBase GetTrainerClassPaletteBase(int trainerClassId);
        SpriteBase GetTrainerClassSpriteBase(int trainerClassId);
        Image GetTrainerClassSrite(PaletteBase palette, ImageBase image, SpriteBase sprite, int frameNumber);
    }
}