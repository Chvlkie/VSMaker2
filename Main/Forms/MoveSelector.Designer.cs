namespace Main.Forms
{
    partial class MoveSelector
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
            pokeName = new Label();
            label6 = new Label();
            label4 = new Label();
            move04ComboBox = new ComboBox();
            label3 = new Label();
            move03ComboBox = new ComboBox();
            label2 = new Label();
            move02ComboBox = new ComboBox();
            label1 = new Label();
            move01ComboBox = new ComboBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pokeName);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(move04ComboBox);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(move03ComboBox);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(move02ComboBox);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(move01ComboBox);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(248, 300);
            panel1.TabIndex = 0;
            // 
            // pokeName
            // 
            pokeName.AutoSize = true;
            pokeName.Location = new Point(12, 35);
            pokeName.Name = "pokeName";
            pokeName.Size = new Size(0, 15);
            pokeName.TabIndex = 11;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label6.Location = new Point(12, 9);
            label6.Name = "label6";
            label6.Size = new Size(60, 15);
            label6.TabIndex = 10;
            label6.Text = "Pokemon";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(12, 246);
            label4.Name = "label4";
            label4.Size = new Size(56, 15);
            label4.TabIndex = 8;
            label4.Text = "Move 04";
            // 
            // move04ComboBox
            // 
            move04ComboBox.FormattingEnabled = true;
            move04ComboBox.Location = new Point(12, 264);
            move04ComboBox.Name = "move04ComboBox";
            move04ComboBox.Size = new Size(223, 23);
            move04ComboBox.TabIndex = 7;
            move04ComboBox.SelectedIndexChanged += move04ComboBox_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(12, 190);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 6;
            label3.Text = "Move 03";
            // 
            // move03ComboBox
            // 
            move03ComboBox.FormattingEnabled = true;
            move03ComboBox.Location = new Point(12, 208);
            move03ComboBox.Name = "move03ComboBox";
            move03ComboBox.Size = new Size(223, 23);
            move03ComboBox.TabIndex = 5;
            move03ComboBox.SelectedIndexChanged += move03ComboBox_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(12, 134);
            label2.Name = "label2";
            label2.Size = new Size(56, 15);
            label2.TabIndex = 4;
            label2.Text = "Move 02";
            // 
            // move02ComboBox
            // 
            move02ComboBox.FormattingEnabled = true;
            move02ComboBox.Location = new Point(12, 152);
            move02ComboBox.Name = "move02ComboBox";
            move02ComboBox.Size = new Size(223, 23);
            move02ComboBox.TabIndex = 3;
            move02ComboBox.SelectedIndexChanged += move02ComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(12, 78);
            label1.Name = "label1";
            label1.Size = new Size(56, 15);
            label1.TabIndex = 2;
            label1.Text = "Move 01";
            // 
            // move01ComboBox
            // 
            move01ComboBox.FormattingEnabled = true;
            move01ComboBox.Location = new Point(12, 96);
            move01ComboBox.Name = "move01ComboBox";
            move01ComboBox.Size = new Size(223, 23);
            move01ComboBox.TabIndex = 0;
            move01ComboBox.SelectedIndexChanged += move01ComboBox_SelectedIndexChanged;
            // 
            // MoveSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(248, 300);
            Controls.Add(panel1);
            MaximizeBox = false;
            MaximumSize = new Size(264, 339);
            MinimizeBox = false;
            MinimumSize = new Size(264, 339);
            Name = "MoveSelector";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Choose Moves";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label2;
        private ComboBox move02ComboBox;
        private Label label1;
        private ComboBox move01ComboBox;
        private Label label4;
        private ComboBox move04ComboBox;
        private Label label3;
        private ComboBox move03ComboBox;
        private Label label6;
        private Label pokeName;
    }
}