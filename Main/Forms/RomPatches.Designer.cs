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
            label2 = new Label();
            arm9PatchCheckBox = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            btn_TrainerName = new Button();
            checkBox_TrainerNames = new CheckBox();
            label5 = new Label();
            label6 = new Label();
            btn_expandTrainerClass = new Button();
            checkBox2 = new CheckBox();
            panel1 = new Panel();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // patchArm9Btn
            // 
            patchArm9Btn.Location = new Point(277, 21);
            patchArm9Btn.Name = "patchArm9Btn";
            patchArm9Btn.Size = new Size(108, 50);
            patchArm9Btn.TabIndex = 0;
            patchArm9Btn.Text = "Expand ARM9";
            patchArm9Btn.UseVisualStyleBackColor = true;
            patchArm9Btn.Click += patchArm9Btn_Click;
            // 
            // label1
            // 
            label1.Location = new Point(12, 42);
            label1.Name = "label1";
            label1.Size = new Size(246, 81);
            label1.TabIndex = 1;
            label1.Text = resources.GetString("label1.Text");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label2.Location = new Point(12, 21);
            label2.Name = "label2";
            label2.Size = new Size(99, 15);
            label2.TabIndex = 2;
            label2.Text = "ARM9 Expansion";
            // 
            // arm9PatchCheckBox
            // 
            arm9PatchCheckBox.AutoSize = true;
            arm9PatchCheckBox.Enabled = false;
            arm9PatchCheckBox.Location = new Point(277, 77);
            arm9PatchCheckBox.Name = "arm9PatchCheckBox";
            arm9PatchCheckBox.Size = new Size(67, 19);
            arm9PatchCheckBox.TabIndex = 3;
            arm9PatchCheckBox.Text = "Applied";
            arm9PatchCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label3.Location = new Point(12, 137);
            label3.Name = "label3";
            label3.Size = new Size(130, 15);
            label3.TabIndex = 4;
            label3.Text = "Expand Trainer Names";
            // 
            // label4
            // 
            label4.Location = new Point(12, 152);
            label4.Name = "label4";
            label4.Size = new Size(246, 59);
            label4.TabIndex = 5;
            label4.Text = "This patch expands the character limit for Trainer Names from 10 to 16.";
            // 
            // btn_TrainerName
            // 
            btn_TrainerName.Location = new Point(277, 137);
            btn_TrainerName.Name = "btn_TrainerName";
            btn_TrainerName.Size = new Size(108, 45);
            btn_TrainerName.TabIndex = 6;
            btn_TrainerName.Text = "Patch";
            btn_TrainerName.UseVisualStyleBackColor = true;
            btn_TrainerName.Click += btn_TrainerName_Click;
            // 
            // checkBox_TrainerNames
            // 
            checkBox_TrainerNames.AutoSize = true;
            checkBox_TrainerNames.Enabled = false;
            checkBox_TrainerNames.Location = new Point(277, 188);
            checkBox_TrainerNames.Name = "checkBox_TrainerNames";
            checkBox_TrainerNames.Size = new Size(67, 19);
            checkBox_TrainerNames.TabIndex = 7;
            checkBox_TrainerNames.Text = "Applied";
            checkBox_TrainerNames.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label5.Location = new Point(12, 235);
            label5.Name = "label5";
            label5.Size = new Size(130, 15);
            label5.TabIndex = 8;
            label5.Text = "Expand Trainer Classes";
            // 
            // label6
            // 
            label6.Location = new Point(12, 256);
            label6.Name = "label6";
            label6.Size = new Size(246, 158);
            label6.TabIndex = 9;
            label6.Text = resources.GetString("label6.Text");
            // 
            // btn_expandTrainerClass
            // 
            btn_expandTrainerClass.Location = new Point(277, 235);
            btn_expandTrainerClass.Name = "btn_expandTrainerClass";
            btn_expandTrainerClass.Size = new Size(108, 45);
            btn_expandTrainerClass.TabIndex = 10;
            btn_expandTrainerClass.Text = "Patch";
            btn_expandTrainerClass.UseVisualStyleBackColor = true;
            btn_expandTrainerClass.Click += btn_expandTrainerClass_Click;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Enabled = false;
            checkBox2.Location = new Point(277, 286);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(67, 19);
            checkBox2.TabIndex = 11;
            checkBox2.Text = "Applied";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(patchArm9Btn);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(arm9PatchCheckBox);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(btn_TrainerName);
            panel1.Controls.Add(checkBox_TrainerNames);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(checkBox2);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(btn_expandTrainerClass);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(406, 423);
            panel1.TabIndex = 20;
            // 
            // RomPatches
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(406, 423);
            Controls.Add(panel1);
            MaximizeBox = false;
            MaximumSize = new Size(422, 462);
            MinimizeBox = false;
            MinimumSize = new Size(422, 462);
            Name = "RomPatches";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "ROM Patches";
            TopMost = true;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button patchArm9Btn;
        private Label label1;
        private Label label2;
        private CheckBox arm9PatchCheckBox;
        private Label label3;
        private Label label4;
        private Button btn_TrainerName;
        private CheckBox checkBox_TrainerNames;
        private Label label5;
        private Label label6;
        private Button btn_expandTrainerClass;
        private CheckBox checkBox2;
        private Panel panel1;
    }
}