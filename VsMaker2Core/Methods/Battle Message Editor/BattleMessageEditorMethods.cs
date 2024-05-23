﻿using VsMaker2Core.DataModels;
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

        public List<BattleMessage> GetBattleMessages(List<BattleMessageTableData> tableDatas, int battleMessageArchive)
        {
            List<BattleMessage> battleMessages = [];
            var messages = romFileMethods.GetBattleMessages(battleMessageArchive);
            foreach (var tableData in tableDatas)
            {
                battleMessages.Add(new BattleMessage((int)tableData.TrainerId, tableData.MessageId, tableData.MessageTriggerId, messages[tableData.MessageId]));
            }

            return battleMessages;
        }
    }
}