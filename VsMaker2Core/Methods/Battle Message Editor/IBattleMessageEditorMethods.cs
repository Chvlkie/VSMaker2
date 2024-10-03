using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public interface IBattleMessageEditorMethods
    {
        Task<List<BattleMessage>> GetBattleMessagesAsync(List<BattleMessageTableData> tableDatas, int battleMessageArchive);
    }
}