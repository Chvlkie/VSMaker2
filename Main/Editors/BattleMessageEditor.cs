using Microsoft.VisualBasic.FileIO;
using System.Text;
using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace Main
{
    // BATTLE MESSAGES EDITOR
    public partial class MainForm : Form
    {
        private readonly string seperator = @"\r";
        private bool isUndoRedo = false;
        private readonly Stack<string> redoStack = new();
        private readonly Stack<string> undoStack = new();
        private bool unsavedBattleMessageChanges;
        public int BattleMessageCount => battleMessage_MessageTableDataGrid.RowCount;

        public async Task BeginExportBattleMessagesAsync(IProgress<int> progress, string filePath)
        {
            Console.WriteLine("Exporting battle messages...");
            try
            {
                await using StreamWriter outputFile = new(filePath, false, new UTF8Encoding(true));
                var export = new StringBuilder();
                var headers = battleMessage_MessageTableDataGrid.Columns.Cast<DataGridViewColumn>();

                export.AppendJoin(",", headers.Select(column => $"\"{column.HeaderText}\"")).AppendLine();
                await outputFile.WriteLineAsync(export.ToString());

                export.Clear();

                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    DataGridViewRow row = battleMessage_MessageTableDataGrid.Rows[i];
                    string messageId = row.Index.ToString();
                    string trainerId = Trainer.ListNameToTrainerId(row.Cells[1].Value.ToString()!).ToString();
                    string messageTriggerId = MessageTrigger.ListNameToMessageTriggerId(row.Cells[2].Value.ToString()!).ToString();
                    string messageText = row.Cells[3].Value.ToString()!;

                    export.AppendLine($@"""{messageId}"",""{trainerId}"",""{messageTriggerId}"",""{messageText}""");

                    await outputFile.WriteLineAsync(export.ToString());
                    export.Clear();

                    progress?.Report(i + 1);

                    await Task.Yield();
                }

                progress?.Report(battleMessage_MessageTableDataGrid.Rows.Count + 50);

                await Task.Delay(250);

                MessageBox.Show("Battle Message exported successfully.", "Success!");
                Console.WriteLine($"Exported {BattleMessageCount} battle messages.");
                Console.WriteLine("Exporting battle messages | Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while exporting: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task BeginImportBattleMessagesAsync(string filePath)
        {
            Console.WriteLine("Importing battle messages...");
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
                        string[]? fields = parser.ReadFields();

                        if (fields!.Length != 4)
                        {
                            Console.WriteLine("Unable to import | Incorrect number of columns");
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
                            mainDataModel.BattleMessages = newMessages;
                            LoadBattleMessages();
                            loadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                            MessageBox.Show("Battle messages imported successfully!", "Success");
                        }));
                    }
                    else
                    {
                        mainDataModel.BattleMessages = newMessages;
                        LoadBattleMessages();
                        loadingData.UpdateProgressBarStyle(ProgressBarStyle.Blocks);
                        MessageBox.Show("Battle messages imported successfully!", "Success");
                    }
                });
                Console.WriteLine($"Imported battle messages {newMessages.Count} battle messages.");
                Console.WriteLine("Import battle messages | Success");
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => MessageBox.Show("An error occurred while reading the file:\n" + ex.Message, "Unable to Import CSV", MessageBoxButtons.OK, MessageBoxIcon.Error)));
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
            List<BattleMessage> messageData = new(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString()!.Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()!];
                int messageId = int.Parse(row.Cells[0].Value.ToString()!);
                string messageText = row.Cells[3].Value.ToString()!;

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }

            messageData = [.. messageData.OrderBy(x => x.MessageId)];
            SaveBattleMessages(messageData, progress);
        }

        public void BeginSortRepointTrainerText(IProgress<int> progress, int max)
        {
            var messageTriggers = MessageTrigger.MessageTriggers.ToDictionary(mt => mt.ListName, mt => mt.MessageTriggerId);
            List<BattleMessage> messageData = new(battleMessage_MessageTableDataGrid.Rows.Count);

            foreach (DataGridViewRow row in battleMessage_MessageTableDataGrid.Rows)
            {
                int trainerId = int.Parse(row.Cells[1].Value.ToString()!.Substring(1, 4));
                int messageTriggerId = messageTriggers[row.Cells[2].Value.ToString()!];
                int messageId = int.Parse(row.Cells[0].Value.ToString()!);
                string messageText = row.Cells[3].Value.ToString()!;

                messageData.Add(new BattleMessage(trainerId, messageId, messageTriggerId, messageText));
            }
            messageData = [.. messageData.OrderBy(x => x.TrainerId)
                .ThenBy(static x => x.MessageTriggerId switch {
                    15 => 0,
                    16 => 1,
                    _ => 2
                }).ThenBy(x => x.MessageTriggerId)];

            SaveBattleMessages(messageData, progress);
            RepointBattleMessageOffsets(messageData, progress);
            progress?.Report(max);
        }

        private static string? ReadLine(string text, int lineNumber)
        {
            var reader = new StringReader(text);

            string? line;
            int currentLineNumber = 0;

            do
            {
                currentLineNumber++;
                line = reader.ReadLine();
            }
            while (line != null && currentLineNumber < lineNumber);

            return (currentLineNumber == lineNumber) ? line : string.Empty;
        }

        private static void AppendBattleMessage(RichTextBox messageText, string appendText)
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
                mainDataModel.SelectedBattleMessageRowIndex = battleMessage_MessageTableDataGrid.CurrentCell.RowIndex;
                battleMessages_MessageTextBox.Enabled = true;
                battleMessages_MessageTextBox.Text = battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString();
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
            string[] currentTrainers = new string[mainDataModel.Trainers.Count];
            string[] currentMessageTriggers = new string[MessageTrigger.MessageTriggers.Count];

            for (int i = 0; i < mainDataModel.Trainers.Count; i++)
            {
                currentTrainers[i] = mainDataModel.Trainers[i].ListName;
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
            if (!isLoadingData && !isUndoRedo)
            {
                undoStack.Push(battleMessages_MessageTextBox.Text);
                redoStack.Clear();

                UpdateUndoRedoButtons();

                if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
                {
                    EditedBattleMessage(true);
                }

                battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;

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
            var (Valid, Row) = VerifyBattleMessageTable();
            if (!Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("Save all changes to Battle Messsage Table?\nThis might take some time...", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult != DialogResult.No && dialogResult == DialogResult.Yes)
                {
                    OpenLoadingDialog(LoadType.SaveTrainerTextTable);
                    RomFile.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
                    RomFile.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
                    mainDataModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);

                    undoStack.Clear();
                    redoStack.Clear();
                    CompileTrainerTableChannges();
                    UpdateUndoRedoButtons();
                }
            }
            isLoadingData = false;
        }

        private void battleMessages_SortBtn_Click(object sender, EventArgs e)
        {
            isLoadingData = true;

            battleMessage_MessageTableDataGrid.EndEdit();
            var (Valid, Row) = VerifyBattleMessageTable();
            if (!Valid)
            {
                MessageBox.Show("You must only use each Message Trigger once per Trainer.\n\nPlease review entry " + Row, "Unable to Save Changes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                battleMessage_MessageTableDataGrid.FirstDisplayedScrollingRowIndex = Row;
                battleMessage_MessageTableDataGrid.ClearSelection();
                battleMessage_MessageTableDataGrid.Rows[Row].Selected = true;
            }
            else
            {
                var dialogResult = MessageBox.Show("This will sort the Battle Message table to group Trainers." +
                    "\n\nThe Battle Messagex lookup table will also be sorted.\nThis allows for more efficient loading in-game." +
                    "\n\nAll changes will be saved.", "Sort and Repoint", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.OK)
                {
                    OpenLoadingDialog(LoadType.RepointTextTable);
                    RomFile.BattleMessageTableData = romFileMethods.GetBattleMessageTableData(RomFile.BattleMessageTablePath);
                    RomFile.BattleMessageOffsetData = romFileMethods.GetBattleMessageOffsetData(RomFile.BattleMessageOffsetPath);
                    mainDataModel.BattleMessages = battleMessageEditorMethods.GetBattleMessages(RomFile.BattleMessageTableData, RomFile.BattleMessageTextNumber);
                    battleMessage_MessageTableDataGrid.Rows.Clear();
                    LoadBattleMessages();
                    EditedBattleMessage(false);
                    CompileTrainerTableChannges();
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

        public void ImportBattleMessageCsv()
        {
            var confirmImport = MessageBox.Show(
                "Importing a CSV will overwrite existing data.\n\nAre you sure?",
                "Import CSV",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmImport != DialogResult.Yes) return;

            using OpenFileDialog importFile = new()
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Open CSV File"
            };

            if (importFile.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    OpenLoadingDialog(LoadType.ImportTextTable, importFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error importing CSV file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void battleMessages_UndoMessageBtn_Click(object sender, EventArgs e)
        {
            if (undoStack.Count > 1)
            {
                isUndoRedo = true;

                redoStack.Push(battleMessages_MessageTextBox.Text);

                undoStack.Pop();
                battleMessages_MessageTextBox.Text = undoStack.Peek();

                UpdateDataGridViewAndPreview(battleMessages_MessageTextBox.Text);

                isUndoRedo = false;

                UpdateUndoRedoButtons();
            }
        }

        private void battleMessages_UpdateText_Click(object sender, EventArgs e)
        {
            if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
            {
                EditedBattleMessage(true);
            }
            battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;
        }

        private void BeginPopulateBattleMessages()
        {
            battleMessage_MessageTableDataGrid.AllowUserToAddRows = true;
            battleMessage_MessageTableDataGrid.Enabled = false;

            PopulateBattleMessages();

            battleMessage_MessageTableDataGrid.AllowUserToAddRows = false;
            battleMessage_MessageTableDataGrid.Enabled = true;
        }

        private void EditedBattleMessage(bool hasChanges)
        {
            unsavedBattleMessageChanges = hasChanges;
            main_MainTable_BattleMessageTab.Text = unsavedBattleMessageChanges ? "Battle Messages *" : "Battle Messages";
            battleMessages_UndoAllBtn.Enabled = unsavedBattleMessageChanges;
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

        private void LoadBattleMessages() => BeginPopulateBattleMessages();

        private void MessagePreviewNavigate(bool isNext, Button nextButton, Button backButton, Label previewText)
        {
            if (isNext)
            {
                mainDataModel.BattleMessageDisplayIndex++;
            }
            else
            {
                mainDataModel.BattleMessageDisplayIndex--;
            }

            backButton.Enabled = mainDataModel.BattleMessageDisplayIndex > 0;
            backButton.Visible = backButton.Enabled;

            nextButton.Enabled = mainDataModel.BattleMessageDisplayIndex < mainDataModel.DisplayBattleMessageText.Count - 1;
            nextButton.Visible = nextButton.Enabled;

            if (mainDataModel.BattleMessageDisplayIndex >= 0 &&
                mainDataModel.BattleMessageDisplayIndex < mainDataModel.DisplayBattleMessageText.Count)
            {
                previewText.Text = mainDataModel.DisplayBattleMessageText[mainDataModel.BattleMessageDisplayIndex];
            }
            else
            {
                mainDataModel.BattleMessageDisplayIndex = isNext ? mainDataModel.BattleMessageDisplayIndex - 1 : mainDataModel.BattleMessageDisplayIndex + 1;
            }
        }

        private void PopulateBattleMessages()
        {
            string[] currentTrainers = mainDataModel.Trainers.Select(x => x.ListName).ToArray();
            string[] messageTriggers = MessageTrigger.MessageTriggers.Select(x => x.ListName).ToArray();

            battleMessage_MessageTableDataGrid.SuspendLayout();

            battleMessage_MessageTableDataGrid.Rows.Clear();

            for (int i = 0; i < mainDataModel.BattleMessages.Count; i++)
            {
                int trainerId = mainDataModel.BattleMessages[i].TrainerId;
                int messageTriggerIndex = mainDataModel.BattleMessages[i].MessageTriggerId;

                DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
                row.Cells[0].Value = i;
                row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[trainerId] };
                row.Cells[2] = new DataGridViewComboBoxCell { DataSource = messageTriggers, Value = messageTriggers[messageTriggerIndex] };
                row.Cells[3].Value = mainDataModel.BattleMessages[i].MessageText;

                ThreadSafeDataTable(row);
            }

            battleMessage_MessageTableDataGrid.ResumeLayout();
        }

        private void RepointBattleMessageOffsets(List<BattleMessage> messageData, IProgress<int> progress)
        {
            progress?.Report(0);
            List<ushort> offsets = [];
            offsets.Add(0);
            for (int i = 1; i < mainDataModel.Trainers.Count; i++)
            {
                int index = 0;
                var trainer = mainDataModel.Trainers[i];
                List<BattleMessage> trainerMessages = messageData.Where(x => x.TrainerId == trainer.TrainerId).ToList();
                if (trainerMessages.Count > 0)
                {
                    var firstMessage = trainerMessages[0];
                    index = messageData.FindIndex(x => x.TrainerId == firstMessage.TrainerId && x.MessageTriggerId == firstMessage.MessageTriggerId);
                    index *= 4;
                }

                offsets.Add((ushort)index);
            }
            var (Success, ErrorMessage) = fileSystemMethods.WriteBattleMessageOffsetData(offsets, progress!);
            if (!Success)
            {
                MessageBox.Show(ErrorMessage, "Unable to Save Battle Offset Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progress?.Report(100);
        }

        private void SaveBattleMessages(List<BattleMessage> messageData, IProgress<int> progress)
        {
            List<string> messageTexts = messageData.ConvertAll(x => x.MessageText);
            var (Success, ErrorMessage) = fileSystemMethods.WriteBattleMessageTableData(messageData, progress);
            if (!Success)
            {
                MessageBox.Show(ErrorMessage, "Unable to Save Battle Message Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            battleMessage_MessageTableDataGrid.Rows[mainDataModel.SelectedBattleMessageRowIndex].Cells[3].Value = text;
            UpdateTextPreview(text, battleMessage_PreviewText, battleMessages_MessageUpBtn, battleMessages_MessageDownBtn);
        }

        private void UpdateTextPreview(string battleMessage, Label previewText, Button nextButton, Button backButton)
        {
            mainDataModel.BattleMessageDisplayIndex = 0;
            mainDataModel.DisplayBattleMessageText = [];

            battleMessage = battleMessage.Replace("\\n", Environment.NewLine);
            battleMessage = battleMessage.Replace("\\f", Environment.NewLine);

            foreach (var item in battleMessage.Split(new string[] { seperator }, StringSplitOptions.None))
            {
                int numLines = item.Split('\n').Length;
                if (numLines >= 3 && !string.IsNullOrEmpty(ReadLine(item, 3)))
                {
                    string text1 = ReadLine(item, 1) + Environment.NewLine + ReadLine(item, 2);
                    string text2 = ReadLine(item, 2) + Environment.NewLine + ReadLine(item, 3);

                    mainDataModel.DisplayBattleMessageText.Add(text1);
                    mainDataModel.DisplayBattleMessageText.Add(text2);
                }
                else
                {
                    mainDataModel.DisplayBattleMessageText.Add(item);
                }
            }

            // Remove last item if blank line - is the case as trainer text formatted as ending with \n.
            if (mainDataModel.DisplayBattleMessageText.Count > 1 && string.IsNullOrEmpty(mainDataModel.DisplayBattleMessageText[^1]))
            {
                mainDataModel.DisplayBattleMessageText.Remove(mainDataModel.DisplayBattleMessageText[^1]);
            }
            //  trainer_Message.Font = new Font(vsMakerFont.VsMakerFontCollection.Families[0], trainer_Message.Font.Size);
            previewText.Text = mainDataModel.DisplayBattleMessageText[0];
            backButton.Enabled = mainDataModel.DisplayBattleMessageText.Count > 1;
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
            foreach (var trainer in mainDataModel.Trainers)
            {
                var checkMessages = new List<string>();
                for (int i = 0; i < battleMessage_MessageTableDataGrid.Rows.Count; i++)
                {
                    var selectedMessageTrigger = battleMessage_MessageTableDataGrid.Rows[i].Cells[2].Value.ToString();
                    var selectedTrainer = battleMessage_MessageTableDataGrid.Rows[i].Cells[1].Value.ToString();
                    int trainerId = int.Parse(selectedTrainer!.Remove(0, 1).Remove(4));
                    if (trainerId == trainer.TrainerId)
                    {
                        if (checkMessages.Contains(selectedMessageTrigger!))
                        {
                            return (false, i);
                        }
                        else
                        {
                            checkMessages.Add(selectedMessageTrigger!);
                        }
                    }
                }
            }

            return (true, -1);
        }
    }
}