using VsMaker2Core.DataModels;

namespace Main
{
    // BATTLE MESSAGES EDITOR
    public partial class Mainform : Form
    {
        private readonly string Seperator = @"\r";
        private bool UnsavedBattleMessageChanges;
        public int BattleMessageCount;

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

        private void battleMessages_MessageDOwnBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewNext(battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);
        }

        private void battleMessages_MessageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!IsLoadingData)
            {
                UpdateTextPreview(battleMessages_MessageTextBox.Text, battleMessage_PreviewText, battleMessages_MessageUpBtn, battleMessages_MessageDownBtn);
            }
        }

        private void battleMessages_MessageUpBtn_Click(object sender, EventArgs e)
        {
            MessagePreviewBack(battleMessages_MessageDownBtn, battleMessages_MessageUpBtn, battleMessage_PreviewText);
        }

        private void battleMessages_UndoAllBtn_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("This will undo all unsaved changes!", "Confirm Undo Changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirm == DialogResult.OK)
            {
                UndoBattleMessageChanges(true);
            }
        }

        private void battleMessages_UpdateText_Click(object sender, EventArgs e)
        {
            if (battleMessages_MessageTextBox.Text != battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value.ToString())
            {
                EditedBattleMessage(true);
            }
            battleMessage_MessageTableDataGrid.Rows[MainEditorModel.SelectedBattleMessageRowIndex].Cells[3].Value = battleMessages_MessageTextBox.Text;
        }

        private async void BeginPopulateBattleMessages()
        {
            battleMessage_MessageTableDataGrid.AllowUserToAddRows = true;
            battleMessage_MessageTableDataGrid.Enabled = false;
            await Task.Run(() => PopulateBattleMessages().Wait());
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
            battleMessages_CopyBtn.Enabled = false;
            battleMessages_ExportBtn.Enabled = false;
            battleMessages_ImportBtn.Enabled = false;
            battleMessages_MessageDownBtn.Enabled = false;
            battleMessages_MessageTextBox.Enabled = false;
            battleMessages_MessageUpBtn.Enabled = false;
            battleMessages_PasteBtn.Enabled = false;
            battleMessages_RedoMessageBtn.Enabled = false;
            battleMessages_RemoveBtn.Enabled = false;
            battleMessages_SaveBtn.Enabled = false;
            battleMessages_SortBtn.Enabled = false;
            battleMessages_UndoAllBtn.Enabled = false;
            battleMessages_UndoMessageBtn.Enabled = false;
        }

        private void MessagePreviewBack(Button nextButton, Button backButton, Label previewText)
        {
            MainEditorModel.BattleMessageDisplayIndex--;
            backButton.Enabled = MainEditorModel.BattleMessageDisplayIndex > 0;
            backButton.Visible = backButton.Enabled;

            nextButton.Enabled = MainEditorModel.BattleMessageDisplayIndex >= 0;
            nextButton.Visible = nextButton.Enabled;

            if (MainEditorModel.BattleMessageDisplayIndex >= 0)
            {
                previewText.Text = MainEditorModel.DisplayBattleMessageText[MainEditorModel.BattleMessageDisplayIndex];
            }
            else
            {
                MainEditorModel.BattleMessageDisplayIndex++;
            }
        }

        private void MessagePreviewNext(Button nextButton, Button backButton, Label previewText)
        {
            MainEditorModel.BattleMessageDisplayIndex++;
            backButton.Enabled = MainEditorModel.BattleMessageDisplayIndex > 0;
            backButton.Visible = backButton.Enabled;

            nextButton.Enabled = MainEditorModel.BattleMessageDisplayIndex < MainEditorModel.DisplayBattleMessageText.Count - 1;
            nextButton.Visible = nextButton.Enabled;

            if (MainEditorModel.BattleMessageDisplayIndex < MainEditorModel.DisplayBattleMessageText.Count)
            {
                previewText.Text = MainEditorModel.DisplayBattleMessageText[MainEditorModel.BattleMessageDisplayIndex];
            }
            else
            {
                MainEditorModel.BattleMessageDisplayIndex++;
            }
        }

        private Task PopulateBattleMessages()
        {
            string[] currentTrainers = MainEditorModel.Trainers.Select(x => x.ListName).ToArray();
            string[] messageTriggers = MessageTrigger.MessageTriggers.Select(x => x.ListName).ToArray();

            for (int i = 0; i < MainEditorModel.BattleMessages.Count; i++)
            {
                int trainerId = MainEditorModel.BattleMessages[i].TrainerId - 1;
                int messageTriggerIndex = MainEditorModel.BattleMessages[i].MessageTriggerId;
                DataGridViewRow row = (DataGridViewRow)battleMessage_MessageTableDataGrid.Rows[0].Clone();
                row.Cells[0].Value = i;
                row.Cells[1] = new DataGridViewComboBoxCell { DataSource = currentTrainers, Value = currentTrainers[trainerId] };
                row.Cells[2] = new DataGridViewComboBoxCell { DataSource = messageTriggers, Value = messageTriggers[messageTriggerIndex] };
                row.Cells[3].Value = MainEditorModel.BattleMessages[i].MessageText;
                ThreadSafeDataTable(row);
            }
            return Task.CompletedTask;
        }

        private bool SaveBattleMessageTexts()
        {
            string[] battleMessages = new string[battleMessage_MessageTableDataGrid.RowCount];
            for (int i = 0; i < battleMessage_MessageTableDataGrid.RowCount; i++)
            {
                var row = battleMessage_MessageTableDataGrid.Rows[i];
                // Set by message ID to not change sorting.
                battleMessages[(int)row.Cells[0].Value] = (string)row.Cells[3].Value;
            }
            var saveBattleMessages = fileSystemMethods.WriteBattleMessageTexts([.. battleMessages], LoadedRom.BattleMessageTextNumber);

            if (!saveBattleMessages.Success)
            {
                MessageBox.Show(saveBattleMessages.ErrorMessage, "Unable to Save Message");
                return false;
            }
            else
            {
                MessageBox.Show("Battle Message texts updated!", "Success");
                return true;
            }
        }

        private void LoadBattleMessages()
        {
            battleMessage_MessageTableDataGrid.Rows.Clear();
            battleMessage_MessageTableDataGrid.SuspendLayout();
            BeginPopulateBattleMessages();
            battleMessage_MessageTableDataGrid.ResumeLayout();
            BattleMessageCount = battleMessage_MessageTableDataGrid.RowCount;
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
    }
}