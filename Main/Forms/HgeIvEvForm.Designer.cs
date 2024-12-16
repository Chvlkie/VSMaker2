namespace Main.Forms
{
    partial class HgeIvEvForm
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
            num_IvSpDef = new NumericUpDown();
            label5 = new Label();
            num_IvSpAtk = new NumericUpDown();
            label4 = new Label();
            num_IvSpd = new NumericUpDown();
            label2 = new Label();
            num_IvDef = new NumericUpDown();
            label3 = new Label();
            num_IvAtk = new NumericUpDown();
            label1 = new Label();
            num_IvHp = new NumericUpDown();
            groupBox2 = new GroupBox();
            label7 = new Label();
            num_EvSpd = new NumericUpDown();
            num_EvSpDef = new NumericUpDown();
            num_EvHp = new NumericUpDown();
            label8 = new Label();
            label12 = new Label();
            num_EvSpAtk = new NumericUpDown();
            num_EvAtk = new NumericUpDown();
            label9 = new Label();
            label11 = new Label();
            num_EvDef = new NumericUpDown();
            label10 = new Label();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)num_IvSpDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_IvSpAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_IvSpd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_IvDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_IvAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_IvHp).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)num_EvSpd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EvSpDef).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EvHp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EvSpAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EvAtk).BeginInit();
            ((System.ComponentModel.ISupportInitialize)num_EvDef).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(num_IvSpDef);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(num_IvSpAtk);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(num_IvSpd);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(num_IvDef);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(num_IvAtk);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(num_IvHp);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(278, 140);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "IVs";
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
            // num_IvSpDef
            // 
            num_IvSpDef.Location = new Point(192, 98);
            num_IvSpDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvSpDef.Name = "num_IvSpDef";
            num_IvSpDef.Size = new Size(77, 23);
            num_IvSpDef.TabIndex = 14;
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
            // num_IvSpAtk
            // 
            num_IvSpAtk.Location = new Point(99, 98);
            num_IvSpAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvSpAtk.Name = "num_IvSpAtk";
            num_IvSpAtk.Size = new Size(77, 23);
            num_IvSpAtk.TabIndex = 12;
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
            // num_IvSpd
            // 
            num_IvSpd.Location = new Point(6, 98);
            num_IvSpd.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvSpd.Name = "num_IvSpd";
            num_IvSpd.Size = new Size(77, 23);
            num_IvSpd.TabIndex = 10;
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
            // num_IvDef
            // 
            num_IvDef.Location = new Point(192, 42);
            num_IvDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvDef.Name = "num_IvDef";
            num_IvDef.Size = new Size(77, 23);
            num_IvDef.TabIndex = 8;
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
            // num_IvAtk
            // 
            num_IvAtk.Location = new Point(99, 42);
            num_IvAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvAtk.Name = "num_IvAtk";
            num_IvAtk.Size = new Size(77, 23);
            num_IvAtk.TabIndex = 6;
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
            // num_IvHp
            // 
            num_IvHp.Location = new Point(6, 42);
            num_IvHp.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_IvHp.Name = "num_IvHp";
            num_IvHp.Size = new Size(77, 23);
            num_IvHp.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(num_EvSpd);
            groupBox2.Controls.Add(num_EvSpDef);
            groupBox2.Controls.Add(num_EvHp);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label12);
            groupBox2.Controls.Add(num_EvSpAtk);
            groupBox2.Controls.Add(num_EvAtk);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(num_EvDef);
            groupBox2.Controls.Add(label10);
            groupBox2.Dock = DockStyle.Bottom;
            groupBox2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox2.Location = new Point(0, 158);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(278, 140);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "EVs";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(192, 83);
            label7.Name = "label7";
            label7.Size = new Size(42, 15);
            label7.TabIndex = 27;
            label7.Text = "SpDEF";
            // 
            // num_EvSpd
            // 
            num_EvSpd.Location = new Point(6, 101);
            num_EvSpd.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvSpd.Name = "num_EvSpd";
            num_EvSpd.Size = new Size(77, 23);
            num_EvSpd.TabIndex = 22;
            // 
            // num_EvSpDef
            // 
            num_EvSpDef.Location = new Point(192, 101);
            num_EvSpDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvSpDef.Name = "num_EvSpDef";
            num_EvSpDef.Size = new Size(77, 23);
            num_EvSpDef.TabIndex = 26;
            // 
            // num_EvHp
            // 
            num_EvHp.Location = new Point(6, 45);
            num_EvHp.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvHp.Name = "num_EvHp";
            num_EvHp.Size = new Size(77, 23);
            num_EvHp.TabIndex = 16;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(99, 83);
            label8.Name = "label8";
            label8.Size = new Size(43, 15);
            label8.TabIndex = 25;
            label8.Text = "SpATK";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(6, 27);
            label12.Name = "label12";
            label12.Size = new Size(23, 15);
            label12.TabIndex = 17;
            label12.Text = "HP";
            // 
            // num_EvSpAtk
            // 
            num_EvSpAtk.Location = new Point(99, 101);
            num_EvSpAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvSpAtk.Name = "num_EvSpAtk";
            num_EvSpAtk.Size = new Size(77, 23);
            num_EvSpAtk.TabIndex = 24;
            // 
            // num_EvAtk
            // 
            num_EvAtk.Location = new Point(99, 45);
            num_EvAtk.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvAtk.Name = "num_EvAtk";
            num_EvAtk.Size = new Size(77, 23);
            num_EvAtk.TabIndex = 18;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 83);
            label9.Name = "label9";
            label9.Size = new Size(30, 15);
            label9.TabIndex = 23;
            label9.Text = "SPD";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(99, 27);
            label11.Name = "label11";
            label11.Size = new Size(29, 15);
            label11.TabIndex = 19;
            label11.Text = "ATK";
            // 
            // num_EvDef
            // 
            num_EvDef.Location = new Point(192, 45);
            num_EvDef.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            num_EvDef.Name = "num_EvDef";
            num_EvDef.Size = new Size(77, 23);
            num_EvDef.TabIndex = 20;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(192, 27);
            label10.Name = "label10";
            label10.Size = new Size(28, 15);
            label10.TabIndex = 21;
            label10.Text = "DEF";
            // 
            // HgeIvEvForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(278, 298);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "HgeIvEvForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Set IVs and EVs";
            FormClosing += HgeIvEvForm_FormClosing;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)num_IvSpDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_IvSpAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_IvSpd).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_IvDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_IvAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_IvHp).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)num_EvSpd).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EvSpDef).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EvHp).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EvSpAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EvAtk).EndInit();
            ((System.ComponentModel.ISupportInitialize)num_EvDef).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private NumericUpDown num_IvHp;
        private Label label3;
        private NumericUpDown num_IvAtk;
        private Label label1;
        private Label label6;
        private NumericUpDown num_IvSpDef;
        private Label label5;
        private NumericUpDown num_IvSpAtk;
        private Label label4;
        private NumericUpDown num_IvSpd;
        private Label label2;
        private NumericUpDown num_IvDef;
        private Label label7;
        private NumericUpDown num_EvSpd;
        private NumericUpDown num_EvSpDef;
        private NumericUpDown num_EvHp;
        private Label label8;
        private Label label12;
        private NumericUpDown num_EvSpAtk;
        private NumericUpDown num_EvAtk;
        private Label label9;
        private Label label11;
        private NumericUpDown num_EvDef;
        private Label label10;
    }
}