using VsMaker2Core.DataModels;
using VsMaker2Core.RomFiles;

namespace VsMaker2Core.Methods
{
    public class BattleMessageEditorMethods : IBattleMessageEditorMethods
    {
        private IRomFileMethods romFileMethods;

        public BattleMessageEditorMethods()
        {
            romFileMethods = new RomFileMethods();
        }

        public async Task<List<BattleMessage>> GetBattleMessagesAsync(List<BattleMessageTableData> tableDatas, int battleMessageArchive)
        {
            List<BattleMessage> battleMessages = [];
            var messages = await romFileMethods.GetBattleMessagesAsync(battleMessageArchive);
            for (int i = 0; i < messages.Count(); i++)
            {
                var tableData = tableDatas[i];
                battleMessages.Add(new BattleMessage((int)tableData.TrainerId, tableData.MessageId, tableData.MessageTriggerId, messages[tableData.MessageId]));
            }
            return battleMessages;
        }
    }
}