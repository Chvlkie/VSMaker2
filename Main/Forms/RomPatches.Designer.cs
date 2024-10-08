namespace Main.Forms
{
    partial class RomPatches
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RomPatches));
            patchArm9Btn = new Button();
            label1 = new Label();
            arm9PatchCheckBox = new CheckBox();
            label4 = new Label();
            btn_TrainerName = new Button();
            checkBox_TrainerNames = new CheckBox();
            label6 = new Label();
            btn_expandTrainerClass = new Button();
            checkBox2 = new CheckBox();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // patchArm9Btn
            // 
            patchArm9Btn.Font = new Font("Segoe UI", 9F);
            patchArm9Btn.Location = new Point(311, 19);
            patchArm9Btn.Name = "patchArm9Btn";
            patchArm9Btn.Size = new Size(108, 50);
            patchArm9Btn.TabIndex = 0;
            patchArm9Btn.Text = "Expand ARM9";
            patchArm9Btn.UseVisualStyleBackColor = true;
            patchArm9Btn.Click += patchArm9Btn_Click;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 9F);
            label1.Location = new Point(6, 19);
            label1.Name = "label1";
            label1.Size = new Size(246, 81);
            label1.TabIndex = 1;
            label1.Text = resources.GetString("label1.Text");
            // 
            // arm9PatchCheckBox
            // 
            arm9PatchCheckBox.Enabled = false;
            arm9PatchCheckBox.Location = new Point(311, 75);
            arm9PatchCheckBox.Name = "arm9PatchCheckBox";
            arm9PatchCheckBox.Size = new Size(108, 19);
            arm9PatchCheckBox.TabIndex = 3;
            arm9PatchCheckBox.Text = "Applied";
            arm9PatchCheckBox.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI", 9F);
            label4.Location = new Point(6, 19);
            label4.Name = "label4";
            label4.Size = new Size(246, 59);
            label4.TabIndex = 5;
            label4.Text = "This patch expands the character limit for Trainer Names from 10 to 16.";
            // 
            // btn_TrainerName
            // 
            btn_TrainerName.Font = new Font("Segoe UI", 9F);
            btn_TrainerName.Location = new Point(311, 19);
            btn_TrainerName.Name = "btn_TrainerName";
            btn_TrainerName.Size = new Size(108, 45);
            btn_TrainerName.TabIndex = 6;
            btn_TrainerName.Text = "Patch";
            btn_TrainerName.UseVisualStyleBackColor = true;
            btn_TrainerName.Click += btn_TrainerName_Click;
            // 
            // checkBox_TrainerNames
            // 
            checkBox_TrainerNames.Enabled = false;
            checkBox_TrainerNames.Location = new Point(311, 70);
            checkBox_TrainerNames.Name = "checkBox_TrainerNames";
            checkBox_TrainerNames.Size = new Size(108, 19);
            checkBox_TrainerNames.TabIndex = 7;
            checkBox_TrainerNames.Text = "Applied";
            checkBox_TrainerNames.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 9F);
            label6.Location = new Point(6, 19);
            label6.Name = "label6";
            label6.Size = new Size(285, 140);
            label6.TabIndex = 9;
            label6.Text = resources.GetString("label6.Text");
            // 
            // btn_expandTrainerClass
            // 
            btn_expandTrainerClass.Font = new Font("Segoe UI", 9F);
            btn_expandTrainerClass.Location = new Point(311, 19);
            btn_expandTrainerClass.Name = "btn_expandTrainerClass";
            btn_expandTrainerClass.Size = new Size(108, 45);
            btn_expandTrainerClass.TabIndex = 10;
            btn_expandTrainerClass.Text = "Patch";
            btn_expandTrainerClass.UseVisualStyleBackColor = true;
            btn_expandTrainerClass.Click += btn_expandTrainerClass_Click;
            // 
            // checkBox2
            // 
            checkBox2.Enabled = false;
            checkBox2.Location = new Point(311, 70);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(108, 19);
            checkBox2.TabIndex = 11;
            checkBox2.Text = "Applied";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox3);
            panel1.Controls.Add(groupBox2);
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(450, 403);
            panel1.TabIndex = 20;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(patchArm9Btn);
            groupBox1.Controls.Add(arm9PatchCheckBox);
            groupBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(426, 113);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "ARM 9 Expansion";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(btn_TrainerName);
            groupBox2.Controls.Add(checkBox_TrainerNames);
            groupBox2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox2.Location = new Point(12, 131);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(426, 100);
            groupBox2.TabIndex = 13;
            groupBox2.TabStop = false;
            groupBox2.Text = "Expand Trainer Names";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(btn_expandTrainerClass);
            groupBox3.Controls.Add(checkBox2);
            groupBox3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            groupBox3.Location = new Point(12, 241);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(426, 153);
            groupBox3.TabIndex = 14;
            groupBox3.TabStop = false;
            groupBox3.Text = "Expand Trainer Classes";
            // 
            // RomPatches
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 403);
            Controls.Add(panel1);
            MaximizeBox = false;
            MaximumSize = new Size(466, 442);
            MinimizeBox = false;
            MinimumSize = new Size(466, 442);
            Name = "RomPatches";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "ROM Patches";
            TopMost = true;
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button patchArm9Btn;
        private Label label1;
        private CheckBox arm9PatchCheckBox;
        private Label label4;
        private Button btn_TrainerName;
        private CheckBox checkBox_TrainerNames;
        private Label label6;
        private Button btn_expandTrainerClass;
        private CheckBox checkBox2;
        private Panel panel1;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
    }
}