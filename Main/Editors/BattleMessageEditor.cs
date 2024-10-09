using Microsoft.VisualBasic.FileIO;
using System.Text;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace Main
{
    // BATTLE MESSAGES EDITOR
    public partial class Mainform : Form
    {
        private readonly string Seperator = @"\r";
        private bool isUndoRedo = false;
        private Stack<string> redoStack = new Stack<string>();
        private Stack<string> undoStack = new Stack<string>();
        private bool UnsavedBattleMessageChanges;
        public int BattleMessageCount => battleMessage_MessageTableDataGrid.RowCount;

        public async Task BeginExportBattleMessagesAsync(IProgress<int> progress, string filePath)
        {
            try
            {
                await using StreamWriter outputFile = new(filePath, false, new UTF8Encoding(true));
                var export = new StringBuilder();
                var headers = battleMessage_MessageTableDataGrid.Columns.Cast<DataGridViewColumn>();

                export.AppendLine(string.Join(",", headers.Select(column => $"\"{column.HeaderText}\"")));
                await outputFile.WriteLineAsync(export.ToString());

                export.Clear();

                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    DataGridViewRow row = battleMessage_MessageTableDataGrid.Rows[i];
                    string messageId = row.Index.ToString();
                    string trainerId = Trainer.ListNameToTrainerId(row.Cells[1].Value.ToString()).ToString();
                    string messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(row.Cells[2].Value.ToString()).ToString();
                    string messageText = row.Cells[3].Value.ToString();

                    export.AppendLine($@"""{messageId}"",""{trainerId}"",""{messageTriggerId}"",""{messageText}""");

                    await outputFile.WriteLineAsync(export.ToString());
                    export.Clear();

                    // Report progress
                    progress?.Report(i + 1);

                    await Task.Yield();
                }

                progress?.Report(battleMessage_MessageTableDataGrid.Rows.Count + 50);

                await Task.Delay(250);

                MessageBox.Show("Battle Message exported successfully.", "Success!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task BeginImportBattleMessagesAsync(string filePath)
        {
            try
            {
                var newMessages = new List<BattleMessage>();

                using (var parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    if (!parser.EndOfData)
                    {
                        parser.ReadLine();
                    }

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        if (fields.Length != 4)
                        {
                            throw new Exception("Unexpected number of columns");
                        }

                        int messageId = int.Parse(fields[0]);
                        int trainerId = int.Parse(fields[1]);
                        int messageTriggerId = int.Parse(fields[2]);
                        string messageText = fields[3];

                        newMessages.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
                    }
                }

                await Task.Run(() =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(new Action(() =>
                        {
                            mainEditorModel.BattleMessages = newMessages;
                            LoadBattleMessages();
                            loadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                            MessageBox.Show("Battle messages imported successfully!", "Success");
                        }));
                    }
                    else
                    {
                        mainEditorModel.BattleMessages = newMessages;
                        LoadBattleMessages();
                        loadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                        MessageBox.Show("Battle messages imported successfully!", "Success");
                    }
                });
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("An error occurred while reading the file:\n" + ex.Message, "Unable to Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                else
                {
                    MessageBox.Show("An error occurred while reading the file:\n" + ex.Message, "Unable to Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void BeginSaveBattleMessages(IProgress<int> progress)
        {
            var messageTriggers = MessageTrigger.MessageTriggers.ToDictionary(mt => mt.ListName, mt => mt.MessageTriggerId);
            List<BattleMessage> messageData = new List<BattleMessage>(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString().Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()];
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                string messageText = row.Cells[3].Value.ToString();

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }

            messageData = messageData.OrderBy(x => x.MessageId).ToList();
            SaveBattleMessages(messageData, progress);
        }

        public void BeginSortRepointTrainerText(IProgress<int> progress, int max)
        {
            var messageTriggers = MessageTrigger.MessageTriggers.ToDictionary(mt => mt.ListName, mt => mt.MessageTriggerId);
            List<BattleMessage> messageData = new List<BattleMessage>(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString().Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()];
                int messageId = int.Parse(row.Cells[0].Value.ToString());
                string messageText = row.Cells[3].Value.ToString();

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }
            messageData = messageData.OrderBy(x => x.TrainerId).ThenBy(x => x.MessageTriggerId).ToList();
            SaveBattleMessages(messageData, progress);
            RepointBattleMessageOffsets(messageData, progress);
            progress?.Report(max);
        }
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

        private void battleMessage_InsertE_Btn_Click(object sender, EventArgs e) => AppendBattleMessage(battleMessages_MessageTextBox, "é");

        private void battleMessage_InsertF_btn_Click(object sender, EventArgs e) => AppendBattleMessage(battleMessages_MessageTextBox, "\\f");

        private void battleMessage_InsertN_btn_Click(object sender, EventArgs e) => AppendBattleMessage(battleMessages_MessageTextBox, "\\n");

        private void battleMessage_InsertR_btn_Click(object sender, EventArgs e) => AppendBattleMessage(battleMessages_MessageTextBox, "\\r");

        private void battleMessage_MessageTableDataGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!isLoadingData)
            {
                EditedBattleMessage(true);
            }
        }

        private void battleMessage_MessageTableDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (!isLoadingData)
            {
                mainEditorModel.SelectedBattleMessageRowIndex = battleMessage_MessageTableDataGrid.CurrentCell.RowIndex;
                battleMessages_MessageTextBox.Enabled = true;
                battleMessages_MessageTextBox.Text = battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString();
                battleMessages_RemoveBtn.Enabled = true;

                undoStack.Clear();
                redoStack.Clear();
                UpdateUndoRedoButtons();
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
            string[] currentTrainers = new string[mainEditorModel.Trainers.Count];
            string[] currentMessageTriggers = new string[MessageTrigger.MessageTriggers.Count];

            for (int i = 0; i < mainEditorModel.Trainers.Count; i++)
            {
                currentTrainers[i] = mainEditorModel.Trainers[i].ListName;
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

        private void battleMessages_ExportBtn_Click(object sender, EventArgs e) => ExportBattleMessagesAsCsv();

        private void battleMessages_ImportBtn_Click(object sender, EventArgs e) => ImportBattleMessageCsv();

        private void battleMessages_MessageDownBtn_Click(object sender, EventArgs e) => MessagePreviewNavigate(true, battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);

        private void battleMessages_MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadingData && !isUndoRedo) // Skip if undo/redo is in progress
            {
                // Push the current text to the undo stack and clear the redo stack
                undoStack.Push(battleMessages_MessageTextBox.Text);
                redoStack.Clear();

                // Update Undo/Redo button states
                UpdateUndoRedoButtons();

                // Compare the text to the DataGridView value and mark it as edited if different
                if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
                {
                    EditedBattleMessage(true);
                }

                // Update the DataGridView with the new text
                battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;

                // Update text preview
                UpdateTextPreview(battleMessages_MessageTextBox.Text, battleMessage_PreviewText, battleMessages_MessageUpBtn, battleMessages_MessageDownBtn);
            }
        }

        private void battleMessages_MessageUpBtn_Click(object sender, EventArgs e) => MessagePreviewNavigate(false, battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);

        private void battleMessages_RedoMessageBtn_Click(object sender, EventArgs e)
        {
            if (redoStack.Count > 0)
            {
                isUndoRedo = true;

                undoStack.Push(battleMessages_MessageTextBox.Text);

                battleMessages_MessageTextBox.Text = redoStack.Pop();

                UpdateDataGridViewAndPreview(battleMessages_MessageTextBox.Text);

                isUndoRedo = false;

                UpdateUndoRedoButtons();
            }
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
            isLoadingData = true;
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
                    mainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);

                    undoStack.Clear();
                    redoStack.Clear();
                    UpdateUndoRedoButtons();
                }
            }
            isLoadingData = false;
        }

        private async void battleMessages_SortBtn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;

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
                    mainEditorModel.BattleMessages = await battleMessageEditorMethods.GetBattleMessagesAsync(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                }
            }
            isLoadingData = false;
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
            if (undoStack.Count > 1) // Ensure there's something to undo
            {
                // Set flag to indicate undo/redo is in progress
                isUndoRedo = true;

                // Push the current text to the redo stack
                redoStack.Push(battleMessages_MessageTextBox.Text);

                // Pop the current state from the undo stack and apply the last saved state
                undoStack.Pop();
                battleMessages_MessageTextBox.Text = undoStack.Peek(); // Apply the previous state

                // Update DataGridView and Preview
                UpdateDataGridViewAndPreview(battleMessages_MessageTextBox.Text);

                // Reset flag after undo is completed
                isUndoRedo = false;

                // Update buttons after undo
                UpdateUndoRedoButtons();
            }
        }

        private void battleMessages_UpdateText_Click(object sender, EventArgs e)
        {
            if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
            {
                EditedBattleMessage(true);
            }
            battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;
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

        private async void LoadBattleMessages() => await BeginPopulateBattleMessages();

        private void MessagePreviewNavigate(bool isNext, Button nextButton, Button backButton, Label previewText)
        {
            if (isNext)
            {
                mainEditorModel.BattleMessageDisplayIndex++;
            }
            else
            {
                mainEditorModel.BattleMessageDisplayIndex--;
            }

            backButton.Enabled = mainEditorModel.BattleMessageDisplayIndex > 0;
            backButton.Visible = backButton.Enabled;

            nextButton.Enabled = mainEditorModel.BattleMessageDisplayIndex < mainEditorModel.DisplayBattleMessageText.Count - 1;
            nextButton.Visible = nextButton.Enabled;

            if (mainEditorModel.BattleMessageDisplayIndex >= 0 &&
                mainEditorModel.BattleMessageDisplayIndex < mainEditorModel.DisplayBattleMessageText.Count)
            {
                previewText.Text = mainEditorModel.DisplayBattleMessageText[mainEditorModel.BattleMessageDisplayIndex];
            }
            else
            {
                // If index is out of range, revert the index change
                mainEditorModel.BattleMessageDisplayIndex = isNext ? mainEditorModel.BattleMessageDisplayIndex - 1 : mainEditorModel.BattleMessageDisplayIndex + 1;
            }
        }

        private async Task PopulateBattleMessages()
        {
            string[] currentTrainers = mainEditorModel.Trainers.Select(x => x.ListName).ToArray();
            string[] messageTriggers = MessageTrigger.MessageTriggers.Select(x => x.ListName).ToArray();

            // Suspend layout logic to prevent flickering
            battleMessage_MessageTableDataGrid.SuspendLayout();

            // Clear existing rows if necessary
            battleMessage_MessageTableDataGrid.Rows.Clear();

            // Use a loop to add the battle messages
            for (int i = 0; i < mainEditorModel.BattleMessages.Count; i++)
            {
                // Access the current message data
                int trainerId = mainEditorModel.BattleMessages[i].TrainerId - 1;
                int messageTriggerIndex = mainEditorModel.BattleMessages[i].MessageTriggerId;

                // Clone the row and set its values
                DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
                row.Cells[0].Value = i;
                row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[trainerId] };
                row.Cells[2] = new DataGridViewComboBoxCell { DataSource = messageTriggers, Value = messageTriggers[messageTriggerIndex] };
                row.Cells[3].Value = mainEditorModel.BattleMessages[i].MessageText;

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
            for (int i = 1; i < mainEditorModel.Trainers.Count + 1; i++)
            {
                int index = 0;
                var trainer = mainEditorModel.Trainers[i - 1];
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
            isLoadingData = true;

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
            isLoadingData = false;
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
            isLoadingData = true;
            battleMessage_MessageTableDataGrid.Rows.Clear();
            if (repopulate)
            {
                SetupBattleMessageEditor();
            }
            isLoadingData = false;
        }

        private void UpdateDataGridViewAndPreview(string text)
        {
            battleMessage_MessageTableDataGrid.Rows[mainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = text;
            UpdateTextPreview(text, battleMessage_PreviewText, battleMessages_MessageUpBtn, battleMessages_MessageDownBtn);
        }

        private void UpdateTextPreview(string battleMessage, Label previewText, Button nextButton, Button backButton)
        {
            mainEditorModel.BattleMessageDisplayIndex = 0;
            mainEditorModel.DisplayBattleMessageText = [];

            battleMessage = battleMessage.Replace("\\n", Environment.NewLine);
            battleMessage = battleMessage.Replace("\\f", Environment.NewLine);

            foreach (var item in battleMessage.Split(new string[] { Seperator }, StringSplitOptions.None))
            {
                int numLines = item.Split('\n').Length;
                if (numLines >= 3 && !string.IsNullOrEmpty(ReadLine(item, 3)))
                {
                    string text1 = ReadLine(item, 1) + Environment.NewLine + ReadLine(item, 2);
                    string text2 = ReadLine(item, 2) + Environment.NewLine + ReadLine(item, 3);

                    mainEditorModel.DisplayBattleMessageText.Add(text1);
                    mainEditorModel.DisplayBattleMessageText.Add(text2);
                }
                else
                {
                    mainEditorModel.DisplayBattleMessageText.Add(item);
                }
            }

            // Remove last item if blank line - is the case as trainer text formatted as ending with \n.
            if (mainEditorModel.DisplayBattleMessageText.Count > 1 && string.IsNullOrEmpty(mainEditorModel.DisplayBattleMessageText.Last()))
            {
                mainEditorModel.DisplayBattleMessageText.Remove(mainEditorModel.DisplayBattleMessageText.Last());
            }
            //  trainer_Message.Font = new Font(vsMakerFont.VsMakerFontCollection.Families[0], trainer_Message.Font.Size);
            previewText.Text = mainEditorModel.DisplayBattleMessageText[0];
            backButton.Enabled = mainEditorModel.DisplayBattleMessageText.Count > 1;
            backButton.Visible = backButton.Enabled;
            nextButton.Enabled = false;
            nextButton.Visible = nextButton.Enabled;
        }

        private void UpdateUndoRedoButtons()
        {
            battleMessages_UndoMessageBtn.Enabled = undoStack.Count > 1;
            battleMessages_RedoMessageBtn.Enabled = redoStack.Count > 0;
        }

        private (bool Valid, int Row) VerifyBattleMessageTable()
        {
            foreach (var trainer in mainEditorModel.Trainers)
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