using VsMaker2Core.DataModels;
using static VsMaker2Core.Enums;

namespace Main.Forms
{
    public partial class ViewVsTrainerFile : Form
    {
        private VsTrainersFile vsTrainersFile;

        private ViewVsMakerFileType type;

        public ViewVsTrainerFile(VsTrainersFile vsTrainersFile, ViewVsMakerFileType type)
        {
            this.vsTrainersFile = vsTrainersFile;
            this.type = type;
            InitializeComponent();

            switch (type)
            {
                case ViewVsMakerFileType.ViewOnly:
                    EnableImportExportButtons(false);
                    PopulateData(vsTrainersFile);
                    break;

                case ViewVsMakerFileType.Import:
                    importExportLbl.Text = "Import";
                    EnableImportExportButtons(true);
                    break;

                case ViewVsMakerFileType.Export:
                    importExportLbl.Text = "Export As...";
                    EnableImportExportButtons(true);
                    PopulateData(vsTrainersFile);
                    break;
            }

        }

        private void PopulateData(VsTrainersFile vsTrainersFile)
        {
            trainerData.Rows.Clear();
            trainerData.AllowUserToAddRows = true;
            trainerData.Enabled = false;
            trainerCountTextBox.Text = vsTrainersFile.TrainerData.Count.ToString();
            classesCountTextBox.Text = vsTrainersFile.ClassesCount.ToString();
            messagesCountTextBox.Text = vsTrainersFile.BattleMessagesCount.ToString();
            gameFamilyTextBox.Text = vsTrainersFile.GameFamily.ToString();

            foreach (var trainer in vsTrainersFile.TrainerData)
            {
                var row = (DataGridViewRow)trainerData.Rows[0].Clone();
                row.Cells[0].Value = trainer.TrainerId.ToString();
                row.Cells[1].Value = trainer.TrainerName;
                row.Cells[2].Value = "Class";
                row.Cells[3].Value = true;
                trainerData.Rows.Add(row);
            }

            trainerData.AllowUserToAddRows = false;
            trainerData.Enabled = true;
        }
        private void EnableImportExportButtons(bool enable)
        {
            importExportCsvBtn.Visible = enable;
            importExportXcelBtn.Visible = enable;
            importExportVsTrainersBtn.Visible = enable;
            importExportCsvBtn.Enabled = enable;
            importExportXcelBtn.Enabled = enable;
            importExportVsTrainersBtn.Enabled = enable;
            importExportLbl.Visible = enable;
            importExportLbl.Enabled = enable;
        }
    }
}