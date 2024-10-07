using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace Main
{
    // BATTLE MESSAGES EDITOR
    public partial class Mainform : Form
    {
        private readonly string Seperator = @"\r";
        private bool UnsavedBattleMessageChanges;
        public int BattleMessageCount => battleMessage_MessageTableDataGrid.RowCount;

        private static string? ReadLine(string text, int lineNumber)
        {
            var reader = new StringReader(text);

            string line;
            int currentLineNumber = 0;

            do
            {
                currentLineNumber++;
                line = reader.ReadLine();
            }
            while (line != null && currentLineNumber < lineNumber);

            return (currentLineNumber == lineNumber) ? line : string.Empty;
        }

        private void AppendBattleMessage(RichTextBox messageText, string appendText)
        {
            int selectionIndex = messageText.SelectionStart;
            messageText.Text = messageText.Text.Insert(selectionIndex, appendText);
            messageText.SelectionStart = selectionIndex + appendText.Length;
        }

        private void battleMessage_InsertE_Btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(battleMessages_MessageTextBox, "é");
        }

        private void battleMessage_InsertF_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(battleMessages_MessageTextBox, "\\f");
        }

        private void battleMessage_InsertN_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(battleMessages_MessageTextBox, "\\n");
        }

        private void battleMessage_InsertR_btn_Click(object sender, EventArgs e)
        {
            AppendBattleMessage(battleMessages_MessageTextBox, "\\r");
        }

        private void battleMessage_MessageTableDataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!IsLoadingData)
            {
                EditedBattleMessage(true);
            }
        }

        private void battleMessage_MessageTableDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                MainEditorModel.SelectedBattleMessageRowIndex = battleMessage_MessageTableDataGrid.CurrentCell.RowIndex;
                battleMessages_MessageTextBox.Enabled = true;
                battleMessages_MessageTextBox.Text = battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString();
                battleMessages_RemoveBtn.Enabled = true;
            }
        }

        private void battleMessages_AddLineBtn_Click(object sender, EventArgs e)
        {
            int index = battleMessage_MessageTableDataGrid.Rows.Count - 1;

            // Get current selected row index - Can't multi-select
            if (battleMessage_MessageTableDataGrid.SelectedRows.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedRows[0].Index;
            }
            else if (battleMessage_MessageTableDataGrid.SelectedCells.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedCells[0].RowIndex;
            }
            string[] currentTrainers = new string[MainEditorModel.Trainers.Count];
            string[] currentMessageTriggers = new string[MessageTrigger.MessageTriggers.Count];

            for (int i = 0; i < MainEditorModel.Trainers.Count; i++)
            {
                currentTrainers[i] = MainEditorModel.Trainers[i].ListName;
            }

            for (int i = 0; i < MessageTrigger.MessageTriggers.Count; i++)
            {
                currentMessageTriggers[i] = MessageTrigger.MessageTriggers[i].ListName;
            }

            DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
            row.Cells[0].Value = battleMessage_MessageTableDataGrid.Rows.Count;
            row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[0] };
            row.Cells[2] = new DataGridViewComboBoxCell { DataSource = currentMessageTriggers, Value = currentMessageTriggers[0] };
            row.Cells[3].Value = "";
            ThreadSafeDataTable(row, index + 1);
            battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = index > 0 ? index - 1 : index;
            battleMessage_MessageTableDataGrid.ClearSelection();
            battleMessage_MessageTableDataGrid.Rows[index + 1].Selected = true;
            EditedBattleMessage(true);
        }

        private void battleMessages_ExportBtn_Click(object sender, EventArgs e)
        {
            ExportBattleMessagesAsCsv();
        }

        private void battleMessages_ImportBtn_Click(object sender, EventArgs e)
        {
            ImportBattleMessageCsv();
        }

        private void battleMessages_MessageDownBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewNavigate(true, battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);
        }

        private void battleMessages_MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
                {
                    EditedBattleMessage(true);
                }
                battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;
                UpdateTextPreview(battleMessages_MessageTextBox.Text, battleMessage_PreviewText, battleMessages_MessageUpBtn, battleMessages_MessageDownBtn);
            }
        }

        private void battleMessages_MessageUpBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewNavigate(false, battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);
        }

        private void battleMessages_RemoveBtn_Click(object sender, EventArgs e)
        {
            int index = 0;

            if (battleMessage_MessageTableDataGrid.SelectedRows.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedRows[0].Index;
            }
            else if (battleMessage_MessageTableDataGrid.SelectedCells.Count > 0)
            {
                index = battleMessage_MessageTableDataGrid.SelectedCells[0].RowIndex;
            }
            battleMessage_MessageTableDataGrid.Rows.RemoveAt(index);
            battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = index > 0 ? index - 1 : index;
            battleMessage_MessageTableDataGrid.ClearSelection();
            EditedBattleMessage(true);
            battleMessages_RemoveBtn.Enabled = false;
        }

        private async void battleMessages_SaveBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;
            battleMessage_MessageTableDataGrid.EndEdit();
            var verify = VerifyBattleMessageTable();
            if (!verify.Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + verify.Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = verify.Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[verify.Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("Save all changes to Battle Messsage Table?\nThis might take some time...", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.No)
                {
                }
                else if (dialogResult == DialogResult.Yes)
                {
                    OpenLoadingDialog(LoadType.SaveTrainerTextTable);
                    RomFile.BattleMessageTableData = await romFileMethods.GetBattleMessageTableDataAsync(RomFile.BattleMessageTablePath);
                    RomFile.BattleMessageOffsetData = await romFileMethods.GetBattleMessageOffsetDataAsync(RomFile.BattleMessageOffsetPath);
                    MainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                }
            }
            IsLoadingData = false;
        }

        private async void battleMessages_SortBtn_Click(object sender, EventArgs e)
        {
            IsLoadingData = true;

            battleMessage_MessageTableDataGrid.EndEdit();
            var verify = VerifyBattleMessageTable();
            if (!verify.Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + verify.Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = verify.Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[verify.Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("This will sort the Battle Message table to group Trainers.\n\nThe Battle Messagex lookup table will also be sorted.\nThis allows for more efficient loading in-game.\n\nAll changes will be saved.", "Sort and Repoint", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.OK)
                {
                    OpenLoadingDialog(LoadType.RepointTextTable);
                    RomFile.BattleMessageTableData = await romFileMethods.GetBattleMessageTableDataAsync(RomFile.BattleMessageTablePath);
                    RomFile.BattleMessageOffsetData = await romFileMethods.GetBattleMessageOffsetDataAsync(RomFile.BattleMessageOffsetPath);
                    MainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                }
            }
            IsLoadingData = false;
        }

        private void battleMessages_UndoAllBtn_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("This will undo all unsaved changes!", "Confirm Undo Changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                UndoBattleMessageChanges(true);
            }
        }

        private void battleMessages_UndoMessageBtn_Click(object sender, EventArgs e)
        {
        }

        private void battleMessages_UpdateText_Click(object sender, EventArgs e)
        {
            if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
            {
                EditedBattleMessage(true);
            }
            battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;
        }

        private async Task BeginPopulateBattleMessages()
        {
            battleMessage_MessageTableDataGrid.AllowUserToAddRows = true;
            battleMessage_MessageTableDataGrid.Enabled = false;

            await PopulateBattleMessages();

            battleMessage_MessageTableDataGrid.AllowUserToAddRows = false;
            battleMessage_MessageTableDataGrid.Enabled = true;
        }

        private void EditedBattleMessage(bool hasChanges)
        {
            UnsavedBattleMessageChanges = hasChanges;
            main_MainTable_BattleMessageTab.Text = UnsavedBattleMessageChanges ? "Battle Messages *" : "Battle Messages";
            battleMessages_UndoAllBtn.Enabled = UnsavedBattleMessageChanges;
        }

        private void InitializeBattleMessageEditor()
        {
            battleMessage_MessageTableDataGrid.Enabled = false;
            battleMessage_MessageTableDataGrid.Rows.Clear();
            battleMessages_AddLineBtn.Enabled = false;
            battleMessages_ExportBtn.Enabled = false;
            battleMessages_ImportBtn.Enabled = false;
            battleMessages_MessageDownBtn.Enabled = false;
            battleMessages_MessageTextBox.Enabled = false;
            battleMessages_MessageUpBtn.Enabled = false;
            battleMessages_RedoMessageBtn.Enabled = false;
            battleMessages_RemoveBtn.Enabled = false;
            battleMessages_SaveBtn.Enabled = false;
            battleMessages_SortBtn.Enabled = false;
            battleMessages_UndoAllBtn.Enabled = false;
            battleMessages_UndoMessageBtn.Enabled = false;
        }

        private async void LoadBattleMessages()
        {
            await BeginPopulateBattleMessages();
        }

        private void MessagePreviewNavigate(bool isNext, Button nextButton, Button backButton, Label previewText)
        {
            if (isNext)
            {
                MainEditorModel.BattleMessageDisplayIndex++;
            }
            else
            {
                MainEditorModel.BattleMessageDisplayIndex--;
            }

            backButton.Enabled = MainEditorModel.BattleMessageDisplayIndex > 0;
            backButton.Visible = backButton.Enabled;

            nextButton.Enabled = MainEditorModel.BattleMessageDisplayIndex < MainEditorModel.DisplayBattleMessageText.Count - 1;
            nextButton.Visible = nextButton.Enabled;

            if (MainEditorModel.BattleMessageDisplayIndex >= 0 &&
                MainEditorModel.BattleMessageDisplayIndex < MainEditorModel.DisplayBattleMessageText.Count)
            {
                previewText.Text = MainEditorModel.DisplayBattleMessageText[MainEditorModel.BattleMessageDisplayIndex];
            }
            else
            {
                // If index is out of range, revert the index change
                MainEditorModel.BattleMessageDisplayIndex = isNext ? MainEditorModel.BattleMessageDisplayIndex - 1 : MainEditorModel.BattleMessageDisplayIndex + 1;
            }
        }

        private async Task PopulateBattleMessages()
        {
            string[] currentTrainers = MainEditorModel.Trainers.Select(x => x.ListName).ToArray();
            string[] messageTriggers = MessageTrigger.MessageTriggers.Select(x => x.ListName).ToArray();

            // Suspend layout logic to prevent flickering
            battleMessage_MessageTableDataGrid.SuspendLayout();

            // Clear existing rows if necessary
            battleMessage_MessageTableDataGrid.Rows.Clear();

            // Use a loop to add the battle messages
            for (int i = 0; i < MainEditorModel.BattleMessages.Count; i++)
            {
                // Access the current message data
                int trainerId = MainEditorModel.BattleMessages[i].TrainerId - 1;
                int messageTriggerIndex = MainEditorModel.BattleMessages[i].MessageTriggerId;

                // Clone the row and set its values
                DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
                row.Cells[0].Value = i;
                row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[trainerId] };
                row.Cells[2] = new DataGridViewComboBoxCell { DataSource = messageTriggers, Value = messageTriggers[messageTriggerIndex] };
                row.Cells[3].Value = MainEditorModel.BattleMessages[i].MessageText;

                // Use your ThreadSafeDataTable method to add the row
                ThreadSafeDataTable(row);
            }

            // Resume layout logic after all updates
            battleMessage_MessageTableDataGrid.ResumeLayout();
        }

        private void RepointBattleMessageOffsets(List<BattleMessage> messageData, IProgress<int> progress)
        {
            progress?.Report(0);
            List<ushort> offsets = [];
            // Increase count by 1 to account for player trainer data.
            offsets.Add(0);
            for (int i = 1; i < MainEditorModel.Trainers.Count + 1; i++)
            {
                int index = 0;
                var trainer = MainEditorModel.Trainers[i - 1];
                List<BattleMessage> trainerMessages = messageData.Where(x => x.TrainerId == trainer.TrainerId).ToList();
                if (trainerMessages.Count > 0)
                {
                    var firstMessage = trainerMessages[0];
                    index = messageData.FindIndex(x => x.TrainerId == firstMessage.TrainerId && x.MessageTriggerId == firstMessage.MessageTriggerId);
                    index *= 4;
                }

                offsets.Add((ushort)index);
            }
            var writeOffsets = fileSystemMethods.WriteBattleMessageOffsetData(offsets, progress);
            if (!writeOffsets.Success)
            {
                MessageBox.Show(writeOffsets.ErrorMessage, "Unable to Save Battle Offset Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progress?.Report(100);
        }

        private void SaveBattleMessages(List<BattleMessage> messageData, IProgress<int> progress)
        {
            List<string> messageTexts = messageData.ConvertAll(x => x.MessageText);
            var writeData = fileSystemMethods.WriteBattleMessageTableData(messageData, progress);
            if (!writeData.Success)
            {
                MessageBox.Show(writeData.ErrorMessage, "Unable to Save Battle Message Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var writeText = fileSystemMethods.WriteBattleMessageTexts(messageTexts, RomFile.BattleMessageTextNumber);
                if (!writeText.Success)
                {
                    MessageBox.Show(writeText.ErrorMessage, "Unable to Save Battle Message Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            progress?.Report(messageData.Count + 10);
        }

        private void SetupBattleMessageEditor()
        {
            IsLoadingData = true;

            if (battleMessage_MessageTableDataGrid.RowCount == 0)
            {
                LoadBattleMessages();
            }

            battleMessages_SaveBtn.Enabled = true;
            battleMessages_ImportBtn.Enabled = true;
            battleMessages_ExportBtn.Enabled = true;
            battleMessages_AddLineBtn.Enabled = true;
            battleMessages_RemoveBtn.Enabled = false;
            battleMessages_SortBtn.Enabled = true;
            IsLoadingData = false;
        }

        private void ThreadSafeDataTable(DataGridViewRow row)
        {
            if (battleMessage_MessageTableDataGrid.InvokeRequired)
            {
                battleMessage_MessageTableDataGrid.Invoke((MethodInvoker)delegate
                {
                    battleMessage_MessageTableDataGrid.Rows.Add(row);
                });
            }
            else
            {
                battleMessage_MessageTableDataGrid.Rows.Add(row);
            }
        }

        private void ThreadSafeDataTable(DataGridViewRow row, int index)
        {
            if (battleMessage_MessageTableDataGrid.InvokeRequired)
            {
                battleMessage_MessageTableDataGrid.Invoke((MethodInvoker)delegate
                {
                    battleMessage_MessageTableDataGrid.Rows.Insert(index, row);
                });
            }
            else
            {
                battleMessage_MessageTableDataGrid.Rows.Insert(index, row);
            }
        }

        private void UndoBattleMessageChanges(bool repopulate)
        {
            EditedBattleMessage(false);
            IsLoadingData = true;
            battleMessage_MessageTableDataGrid.Rows.Clear();
            if (repopulate)
            {
                SetupBattleMessageEditor();
            }
            IsLoadingData = false;
        }

        private void UpdateTextPreview(string battleMessage, Label previewText, Button nextButton, Button backButton)
        {
            MainEditorModel.BattleMessageDisplayIndex = 0;
            MainEditorModel.DisplayBattleMessageText = [];

            battleMessage = battleMessage.Replace("\\n", Environment.NewLine);
            battleMessage = battleMessage.Replace("\\f", Environment.NewLine);

            foreach (var item in battleMessage.Split(new string[] { Seperator }, StringSplitOptions.None))
            {
                int numLines = item.Split('\n').Length;
                if (numLines >= 3 && !string.IsNullOrEmpty(ReadLine(item, 3)))
                {
                    string text1 = ReadLine(item, 1) + Environment.NewLine + ReadLine(item, 2);
                    string text2 = ReadLine(item, 2) + Environment.NewLine + ReadLine(item, 3);

                    MainEditorModel.DisplayBattleMessageText.Add(text1);
                    MainEditorModel.DisplayBattleMessageText.Add(text2);
                }
                else
                {
                    MainEditorModel.DisplayBattleMessageText.Add(item);
                }
            }

            // Remove last item if blank line - is the case as trainer text formatted as ending with \n.
            if (MainEditorModel.DisplayBattleMessageText.Count > 1 && string.IsNullOrEmpty(MainEditorModel.DisplayBattleMessageText.Last()))
            {
                MainEditorModel.DisplayBattleMessageText.Remove(MainEditorModel.DisplayBattleMessageText.Last());
            }
            //  trainer_Message.Font = new Font(vsMakerFont.VsMakerFontCollection.Families[0], trainer_Message.Font.Size);
            previewText.Text = MainEditorModel.DisplayBattleMessageText[0];
            backButton.Enabled = MainEditorModel.DisplayBattleMessageText.Count > 1;
            backButton.Visible = backButton.Enabled;
            nextButton.Enabled = false;
            nextButton.Visible = nextButton.Enabled;
        }

        private (bool Valid, int Row) VerifyBattleMessageTable()
        {
            foreach (var trainer in MainEditorModel.Trainers)
            {
                var checkMessages = new List<string>();
                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    var selectedMessageTrigger = battleMessage_MessageTableDataGrid.Rows[i].Cells[2].Value.ToString();
                    var selectedTrainer = battleMessage_MessageTableDataGrid.Rows[i].Cells[1].Value.ToString();
                    int trainerId = int.Parse(selectedTrainer.Remove(0, 1).Remove(4));
                    if (trainerId == trainer.TrainerId)
                    {
                        if (checkMessages.Contains(selectedMessageTrigger))
                        {
                            return (false, i);
                        }
                        else
                        {
                            checkMessages.Add(selectedMessageTrigger);
                        }
                    }
                }
            }

            return (true, -1);
        }
    }
}