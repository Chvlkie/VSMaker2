namespace Main.Forms
{
    partial class EditStatsForm
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
            groupBox1 = new GroupBox();
            label6 = new Label();
            num_StatSpDef = new NumericUpDown();
            label5 = new Label();
            num_StatSpAtk = new NumericUpDown();
            label4 = new Label();
            num_StatSpd = new NumericUpDown();
            label2 = new Label();
            num_StatDef = new NumericUpDown();
            label3 = new Label();
            num_StatAtk = new NumericUpDown();
            label1 = new Label();
            num_StatHp = new NumericUpDown();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)num_StatSpDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_StatSpAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_StatSpd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_StatDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_StatAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_StatHp).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(num_StatSpDef);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(num_StatSpAtk);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(num_StatSpd);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(num_StatDef);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(num_StatAtk);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(num_StatHp);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(278, 140);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Stats";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(192, 80);
            label6.Name = "label6";
            label6.Size = new Size(42, 15);
            label6.TabIndex = 15;
            label6.Text = "SpDEF";
            // 
            // num_StatSpDef
            // 
            num_StatSpDef.Location = new Point(192, 98);
            num_StatSpDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatSpDef.Name = "num_StatSpDef";
            num_StatSpDef.Size = new Size(77, 23);
            num_StatSpDef.TabIndex = 14;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(99, 80);
            label5.Name = "label5";
            label5.Size = new Size(43, 15);
            label5.TabIndex = 13;
            label5.Text = "SpATK";
            // 
            // num_StatSpAtk
            // 
            num_StatSpAtk.Location = new Point(99, 98);
            num_StatSpAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatSpAtk.Name = "num_StatSpAtk";
            num_StatSpAtk.Size = new Size(77, 23);
            num_StatSpAtk.TabIndex = 12;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 80);
            label4.Name = "label4";
            label4.Size = new Size(30, 15);
            label4.TabIndex = 11;
            label4.Text = "SPD";
            // 
            // num_StatSpd
            // 
            num_StatSpd.Location = new Point(6, 98);
            num_StatSpd.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatSpd.Name = "num_StatSpd";
            num_StatSpd.Size = new Size(77, 23);
            num_StatSpd.TabIndex = 10;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(192, 24);
            label2.Name = "label2";
            label2.Size = new Size(28, 15);
            label2.TabIndex = 9;
            label2.Text = "DEF";
            // 
            // num_StatDef
            // 
            num_StatDef.Location = new Point(192, 42);
            num_StatDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatDef.Name = "num_StatDef";
            num_StatDef.Size = new Size(77, 23);
            num_StatDef.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(99, 24);
            label3.Name = "label3";
            label3.Size = new Size(29, 15);
            label3.TabIndex = 7;
            label3.Text = "ATK";
            // 
            // num_StatAtk
            // 
            num_StatAtk.Location = new Point(99, 42);
            num_StatAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatAtk.Name = "num_StatAtk";
            num_StatAtk.Size = new Size(77, 23);
            num_StatAtk.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 24);
            label1.Name = "label1";
            label1.Size = new Size(23, 15);
            label1.TabIndex = 3;
            label1.Text = "HP";
            // 
            // num_StatHp
            // 
            num_StatHp.Location = new Point(6, 42);
            num_StatHp.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_StatHp.Name = "num_StatHp";
            num_StatHp.Size = new Size(77, 23);
            num_StatHp.TabIndex = 0;
            // 
            // EditStatsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 139);
            Controls.Add(groupBox1);
            Name = "EditStatsForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Set Stats";
            FormClosing += EditStatsForm_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)num_StatSpDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_StatSpAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_StatSpd).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_StatDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_StatAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_StatHp).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private NumericUpDown num_StatHp;
        private Label label3;
        private NumericUpDown num_StatAtk;
        private Label label1;
        private Label label6;
        private NumericUpDown num_StatSpDef;
        private Label label5;
        private NumericUpDown num_StatSpAtk;
        private Label label4;
        private NumericUpDown num_StatSpd;
        private Label label2;
        private NumericUpDown num_StatDef;
    }
}