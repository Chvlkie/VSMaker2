namespace Main.Forms
{
    partial class ViewVsTrainerFile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            trainerData = new DataGridView();
            importExportLbl = new Label();
            importExportVsTrainersBtn = new Button();
            importExportXcelBtn = new Button();
            label4 = new Label();
            label3 = new Label();
            gameFamilyTextBox = new TextBox();
            label2 = new Label();
            messagesCountTextBox = new TextBox();
            label1 = new Label();
            classesCountTextBox = new TextBox();
            trainersCountLbl = new Label();
            trainerCountTextBox = new TextBox();
            importExportCsvBtn = new Button();
            tarinerId = new DataGridViewTextBoxColumn();
            trainerName = new DataGridViewTextBoxColumn();
            trClass = new DataGridViewTextBoxColumn();
            isDouble = new DataGridViewCheckBoxColumn();
            poke = new DataGridViewImageColumn();
            items = new DataGridViewImageColumn();
            aiFlags = new DataGridViewImageColumn();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trainerData).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(trainerData);
            panel1.Controls.Add(importExportLbl);
            panel1.Controls.Add(importExportVsTrainersBtn);
            panel1.Controls.Add(importExportXcelBtn);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(gameFamilyTextBox);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(messagesCountTextBox);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(classesCountTextBox);
            panel1.Controls.Add(trainersCountLbl);
            panel1.Controls.Add(trainerCountTextBox);
            panel1.Controls.Add(importExportCsvBtn);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(884, 601);
            panel1.TabIndex = 0;
            // 
            // trainerData
            // 
            trainerData.AllowUserToAddRows = false;
            trainerData.AllowUserToDeleteRows = false;
            trainerData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            trainerData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            trainerData.Columns.AddRange(new DataGridViewColumn[] { tarinerId, trainerName, trClass, isDouble, poke, items, aiFlags });
            trainerData.Enabled = false;
            trainerData.Location = new Point(12, 42);
            trainerData.Name = "trainerData";
            trainerData.Size = new Size(860, 475);
            trainerData.TabIndex = 16;
            // 
            // importExportLbl
            // 
            importExportLbl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            importExportLbl.AutoSize = true;
            importExportLbl.Enabled = false;
            importExportLbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            importExportLbl.Location = new Point(517, 546);
            importExportLbl.Name = "importExportLbl";
            importExportLbl.Size = new Size(69, 15);
            importExportLbl.TabIndex = 15;
            importExportLbl.Text = "Export As...";
            importExportLbl.Visible = false;
            // 
            // importExportVsTrainersBtn
            // 
            importExportVsTrainersBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            importExportVsTrainersBtn.Enabled = false;
            importExportVsTrainersBtn.Location = new Point(517, 563);
            importExportVsTrainersBtn.Name = "importExportVsTrainersBtn";
            importExportVsTrainersBtn.Size = new Size(151, 23);
            importExportVsTrainersBtn.TabIndex = 14;
            importExportVsTrainersBtn.Text = "VS Trainers (.vstrainers)";
            importExportVsTrainersBtn.UseVisualStyleBackColor = true;
            importExportVsTrainersBtn.Visible = false;
            // 
            // importExportXcelBtn
            // 
            importExportXcelBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            importExportXcelBtn.Enabled = false;
            importExportXcelBtn.Location = new Point(674, 563);
            importExportXcelBtn.Name = "importExportXcelBtn";
            importExportXcelBtn.Size = new Size(126, 23);
            importExportXcelBtn.TabIndex = 13;
            importExportXcelBtn.Text = "Spreadsheet (.xlsx)";
            importExportXcelBtn.UseVisualStyleBackColor = true;
            importExportXcelBtn.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(12, 24);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 12;
            label4.Text = "Trainer Data";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(340, 546);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 11;
            label3.Text = "Game Family";
            // 
            // gameFamilyTextBox
            // 
            gameFamilyTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            gameFamilyTextBox.Location = new Point(340, 564);
            gameFamilyTextBox.Name = "gameFamilyTextBox";
            gameFamilyTextBox.ReadOnly = true;
            gameFamilyTextBox.Size = new Size(145, 23);
            gameFamilyTextBox.TabIndex = 10;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(224, 546);
            label2.Name = "label2";
            label2.Size = new Size(107, 15);
            label2.TabIndex = 9;
            label2.Text = "# Battle Messages";
            // 
            // messagesCountTextBox
            // 
            messagesCountTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            messagesCountTextBox.Location = new Point(224, 564);
            messagesCountTextBox.Name = "messagesCountTextBox";
            messagesCountTextBox.ReadOnly = true;
            messagesCountTextBox.Size = new Size(100, 23);
            messagesCountTextBox.TabIndex = 8;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(118, 546);
            label1.Name = "label1";
            label1.Size = new Size(55, 15);
            label1.TabIndex = 7;
            label1.Text = "# Classes";
            // 
            // classesCountTextBox
            // 
            classesCountTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            classesCountTextBox.Location = new Point(118, 564);
            classesCountTextBox.Name = "classesCountTextBox";
            classesCountTextBox.ReadOnly = true;
            classesCountTextBox.Size = new Size(100, 23);
            classesCountTextBox.TabIndex = 6;
            // 
            // trainersCountLbl
            // 
            trainersCountLbl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            trainersCountLbl.AutoSize = true;
            trainersCountLbl.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            trainersCountLbl.Location = new Point(12, 546);
            trainersCountLbl.Name = "trainersCountLbl";
            trainersCountLbl.Size = new Size(61, 15);
            trainersCountLbl.TabIndex = 5;
            trainersCountLbl.Text = "# Trainers";
            // 
            // trainerCountTextBox
            // 
            trainerCountTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            trainerCountTextBox.Location = new Point(12, 564);
            trainerCountTextBox.Name = "trainerCountTextBox";
            trainerCountTextBox.ReadOnly = true;
            trainerCountTextBox.Size = new Size(100, 23);
            trainerCountTextBox.TabIndex = 2;
            // 
            // importExportCsvBtn
            // 
            importExportCsvBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            importExportCsvBtn.Enabled = false;
            importExportCsvBtn.Location = new Point(806, 563);
            importExportCsvBtn.Name = "importExportCsvBtn";
            importExportCsvBtn.Size = new Size(66, 23);
            importExportCsvBtn.TabIndex = 1;
            importExportCsvBtn.Text = ".csv";
            importExportCsvBtn.UseVisualStyleBackColor = true;
            importExportCsvBtn.Visible = false;
            // 
            // tarinerId
            // 
            tarinerId.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            tarinerId.FillWeight = 10F;
            tarinerId.Frozen = true;
            tarinerId.HeaderText = "ID";
            tarinerId.MinimumWidth = 25;
            tarinerId.Name = "tarinerId";
            tarinerId.ReadOnly = true;
            tarinerId.Width = 25;
            // 
            // trainerName
            // 
            trainerName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            trainerName.FillWeight = 58.9261551F;
            trainerName.HeaderText = "Name";
            trainerName.MinimumWidth = 30;
            trainerName.Name = "trainerName";
            trainerName.ReadOnly = true;
            // 
            // trClass
            // 
            trClass.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            trClass.FillWeight = 66.69883F;
            trClass.HeaderText = "Class";
            trClass.MinimumWidth = 30;
            trClass.Name = "trClass";
            trClass.ReadOnly = true;
            // 
            // isDouble
            // 
            isDouble.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            isDouble.FillWeight = 11.8088694F;
            isDouble.HeaderText = "Dbl Battle";
            isDouble.MinimumWidth = 50;
            isDouble.Name = "isDouble";
            isDouble.ReadOnly = true;
            // 
            // poke
            // 
            poke.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            poke.FillWeight = 73.78501F;
            poke.HeaderText = "Pokemon";
            poke.MinimumWidth = 175;
            poke.Name = "poke";
            poke.ReadOnly = true;
            // 
            // items
            // 
            items.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            items.FillWeight = 73.78501F;
            items.HeaderText = "Items";
            items.MinimumWidth = 30;
            items.Name = "items";
            items.ReadOnly = true;
            // 
            // aiFlags
            // 
            aiFlags.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            aiFlags.FillWeight = 73.78501F;
            aiFlags.HeaderText = "AI Flags";
            aiFlags.MinimumWidth = 50;
            aiFlags.Name = "aiFlags";
            aiFlags.ReadOnly = true;
            // 
            // ViewVsTrainerFile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(884, 601);
            Controls.Add(panel1);
            MinimumSize = new Size(900, 640);
            Name = "ViewVsTrainerFile";
            Text = "VS Maker Trainers File Viewer";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trainerData).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button importExportCsvBtn;
        private TextBox trainerCountTextBox;
        private Label label3;
        private TextBox gameFamilyTextBox;
        private Label label2;
        private TextBox messagesCountTextBox;
        private Label label1;
        private TextBox classesCountTextBox;
        private Label trainersCountLbl;
        private Label label4;
        private Button importExportVsTrainersBtn;
        private Button importExportXcelBtn;
        private Label importExportLbl;
        private DataGridView trainerData;
        private DataGridViewTextBoxColumn tarinerId;
        private DataGridViewTextBoxColumn trainerName;
        private DataGridViewTextBoxColumn trClass;
        private DataGridViewCheckBoxColumn isDouble;
        private DataGridViewImageColumn poke;
        private DataGridViewImageColumn items;
        private DataGridViewImageColumn aiFlags;
    }
}