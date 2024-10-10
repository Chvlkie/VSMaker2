using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public interface IBattleMessageEditorMethods
    {
        List<BattleMessage> GetBattleMessages(List<BattleMessageTableData> tableDatas, int battleMessageArchive);
    }
}