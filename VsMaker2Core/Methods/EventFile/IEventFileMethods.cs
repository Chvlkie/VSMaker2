using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods.EventFile
{
    public interface IEventFileMethods
    {
        EventFileData GetEventFileData(int eventFileId);
        List<EventFileData> GetEventFiles();
    }
}