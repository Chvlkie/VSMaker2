namespace Main.Forms
{
    partial class Settings
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
            components = new System.ComponentModel.Container();
            label1 = new Label();
            btn_ClearRecent = new Button();
            checkBox_OpenLast = new CheckBox();
            panel1 = new Panel();
            panel2 = new Panel();
            btn_Apply = new Button();
            tabMenu_Settings = new TabControl();
            tab_General = new TabPage();
            groupBox2 = new GroupBox();
            lbl_DefaultRomFolder = new Label();
            tb_RomFolder = new TextBox();
            btn_BrowseRomFolder = new Button();
            groupBox1 = new GroupBox();
            lbl_AutoLoadHelp = new Label();
            label2 = new Label();
            tab_Display = new TabPage();
            tab_Advanced = new TabPage();
            groupBox3 = new GroupBox();
            label4 = new Label();
            btn_ExportLogs = new Button();
            label5 = new Label();
            checkBox_ShowConsole = new CheckBox();
            toolTip_AutoLoad = new ToolTip(components);
            toolTip_DefaultRomFolder = new ToolTip(components);
            btn_Close = new Button();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            tabMenu_Settings.SuspendLayout();
            tab_General.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tab_Advanced.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 57);
            label1.Name = "label1";
            label1.Size = new Size(153, 15);
            label1.TabIndex = 2;
            label1.Text = "Clear Recently Opened Files";
            // 
            // btn_ClearRecent
            // 
            btn_ClearRecent.Location = new Point(226, 53);
            btn_ClearRecent.Name = "btn_ClearRecent";
            btn_ClearRecent.Size = new Size(75, 23);
            btn_ClearRecent.TabIndex = 3;
            btn_ClearRecent.Text = "Clear";
            btn_ClearRecent.UseVisualStyleBackColor = true;
            btn_ClearRecent.Click += btn_ClearRecent_Click;
            // 
            // checkBox_OpenLast
            // 
            checkBox_OpenLast.Appearance = Appearance.Button;
            checkBox_OpenLast.CheckAlign = ContentAlignment.MiddleCenter;
            checkBox_OpenLast.Location = new Point(226, 22);
            checkBox_OpenLast.Name = "checkBox_OpenLast";
            checkBox_OpenLast.Size = new Size(75, 25);
            checkBox_OpenLast.TabIndex = 4;
            checkBox_OpenLast.Text = "No";
            checkBox_OpenLast.TextAlign = ContentAlignment.MiddleCenter;
            checkBox_OpenLast.UseVisualStyleBackColor = true;
            checkBox_OpenLast.CheckedChanged += checkBox_OpenLast_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(tabMenu_Settings);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(320, 219);
            panel1.TabIndex = 2;
            // 
            // panel2
            // 
            panel2.Controls.Add(btn_Close);
            panel2.Controls.Add(btn_Apply);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 184);
            panel2.Name = "panel2";
            panel2.Size = new Size(320, 35);
            panel2.TabIndex = 7;
            // 
            // btn_Apply
            // 
            btn_Apply.Location = new Point(151, 3);
            btn_Apply.Name = "btn_Apply";
            btn_Apply.Size = new Size(75, 23);
            btn_Apply.TabIndex = 0;
            btn_Apply.Text = "Apply";
            btn_Apply.UseVisualStyleBackColor = true;
            btn_Apply.Click += btn_Apply_Click;
            // 
            // tabMenu_Settings
            // 
            tabMenu_Settings.Controls.Add(tab_General);
            tabMenu_Settings.Controls.Add(tab_Display);
            tabMenu_Settings.Controls.Add(tab_Advanced);
            tabMenu_Settings.Dock = DockStyle.Top;
            tabMenu_Settings.Location = new Point(0, 0);
            tabMenu_Settings.Multiline = true;
            tabMenu_Settings.Name = "tabMenu_Settings";
            tabMenu_Settings.SelectedIndex = 0;
            tabMenu_Settings.Size = new Size(320, 175);
            tabMenu_Settings.TabIndex = 6;
            // 
            // tab_General
            // 
            tab_General.Controls.Add(groupBox2);
            tab_General.Controls.Add(groupBox1);
            tab_General.Location = new Point(4, 24);
            tab_General.Name = "tab_General";
            tab_General.Padding = new Padding(3);
            tab_General.Size = new Size(312, 147);
            tab_General.TabIndex = 0;
            tab_General.Text = "General";
            tab_General.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(lbl_DefaultRomFolder);
            groupBox2.Controls.Add(tb_RomFolder);
            groupBox2.Controls.Add(btn_BrowseRomFolder);
            groupBox2.Dock = DockStyle.Bottom;
            groupBox2.Location = new Point(3, 91);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(306, 53);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Default ROM Location";
            // 
            // lbl_DefaultRomFolder
            // 
            lbl_DefaultRomFolder.AutoSize = true;
            lbl_DefaultRomFolder.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lbl_DefaultRomFolder.Location = new Point(207, 25);
            lbl_DefaultRomFolder.Name = "lbl_DefaultRomFolder";
            lbl_DefaultRomFolder.Size = new Size(12, 15);
            lbl_DefaultRomFolder.TabIndex = 7;
            lbl_DefaultRomFolder.Text = "?";
            // 
            // tb_RomFolder
            // 
            tb_RomFolder.Location = new Point(6, 22);
            tb_RomFolder.Name = "tb_RomFolder";
            tb_RomFolder.Size = new Size(196, 23);
            tb_RomFolder.TabIndex = 6;
            // 
            // btn_BrowseRomFolder
            // 
            btn_BrowseRomFolder.Location = new Point(225, 21);
            btn_BrowseRomFolder.Name = "btn_BrowseRomFolder";
            btn_BrowseRomFolder.Size = new Size(75, 23);
            btn_BrowseRomFolder.TabIndex = 7;
            btn_BrowseRomFolder.Text = "Browse";
            btn_BrowseRomFolder.UseVisualStyleBackColor = true;
            btn_BrowseRomFolder.Click += btn_BrowseRomFolder_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbl_AutoLoadHelp);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btn_ClearRecent);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBox_OpenLast);
            groupBox1.Dock = DockStyle.Top;
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(306, 82);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Recent Files";
            // 
            // lbl_AutoLoadHelp
            // 
            lbl_AutoLoadHelp.AutoSize = true;
            lbl_AutoLoadHelp.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lbl_AutoLoadHelp.Location = new Point(207, 27);
            lbl_AutoLoadHelp.Name = "lbl_AutoLoadHelp";
            lbl_AutoLoadHelp.Size = new Size(12, 15);
            lbl_AutoLoadHelp.TabIndex = 6;
            lbl_AutoLoadHelp.Text = "?";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 27);
            label2.Name = "label2";
            label2.Size = new Size(160, 15);
            label2.TabIndex = 5;
            label2.Text = "Load Last Opened on Startup";
            // 
            // tab_Display
            // 
            tab_Display.Location = new Point(4, 24);
            tab_Display.Name = "tab_Display";
            tab_Display.Padding = new Padding(3);
            tab_Display.Size = new Size(312, 147);
            tab_Display.TabIndex = 1;
            tab_Display.Text = "Appearance";
            tab_Display.UseVisualStyleBackColor = true;
            // 
            // tab_Advanced
            // 
            tab_Advanced.Controls.Add(groupBox3);
            tab_Advanced.Location = new Point(4, 24);
            tab_Advanced.Name = "tab_Advanced";
            tab_Advanced.Padding = new Padding(3);
            tab_Advanced.Size = new Size(312, 147);
            tab_Advanced.TabIndex = 2;
            tab_Advanced.Text = "Advanced";
            tab_Advanced.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label4);
            groupBox3.Controls.Add(btn_ExportLogs);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(checkBox_ShowConsole);
            groupBox3.Dock = DockStyle.Top;
            groupBox3.Location = new Point(3, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(306, 82);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Logs";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 27);
            label4.Name = "label4";
            label4.Size = new Size(129, 15);
            label4.TabIndex = 5;
            label4.Text = "Show Console Window";
            // 
            // btn_ExportLogs
            // 
            btn_ExportLogs.Location = new Point(226, 53);
            btn_ExportLogs.Name = "btn_ExportLogs";
            btn_ExportLogs.Size = new Size(75, 23);
            btn_ExportLogs.TabIndex = 3;
            btn_ExportLogs.Text = "Export";
            btn_ExportLogs.UseVisualStyleBackColor = true;
            btn_ExportLogs.Click += btn_ExportLogs_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 57);
            label5.Name = "label5";
            label5.Size = new Size(69, 15);
            label5.TabIndex = 2;
            label5.Text = "Export Logs";
            // 
            // checkBox_ShowConsole
            // 
            checkBox_ShowConsole.Appearance = Appearance.Button;
            checkBox_ShowConsole.CheckAlign = ContentAlignment.MiddleCenter;
            checkBox_ShowConsole.Location = new Point(226, 22);
            checkBox_ShowConsole.Name = "checkBox_ShowConsole";
            checkBox_ShowConsole.Size = new Size(75, 25);
            checkBox_ShowConsole.TabIndex = 4;
            checkBox_ShowConsole.Text = "Hide";
            checkBox_ShowConsole.TextAlign = ContentAlignment.MiddleCenter;
            checkBox_ShowConsole.UseVisualStyleBackColor = true;
            checkBox_ShowConsole.CheckedChanged += checkBox_ShowConsole_CheckedChanged;
            // 
            // btn_Close
            // 
            btn_Close.Location = new Point(233, 3);
            btn_Close.Name = "btn_Close";
            btn_Close.Size = new Size(75, 23);
            btn_Close.TabIndex = 1;
            btn_Close.Text = "Close";
            btn_Close.UseVisualStyleBackColor = true;
            btn_Close.Click += btn_Close_Click;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 219);
            Controls.Add(panel1);
            MaximizeBox = false;
            MaximumSize = new Size(336, 258);
            MinimizeBox = false;
            MinimumSize = new Size(336, 258);
            Name = "Settings";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "Settings";
            TopMost = true;
            Load += Settings_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            tabMenu_Settings.ResumeLayout(false);
            tab_General.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tab_Advanced.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btn_ClearRecent;
        private Label label1;
        private CheckBox checkBox_OpenLast;
        private Panel panel1;
        private TabControl tabMenu_Settings;
        private TabPage tab_General;
        private TabPage tab_Display;
        private Button btn_BrowseRomFolder;
        private TextBox tb_RomFolder;
        private TabPage tab_Advanced;
        private Panel panel2;
        private Button btn_Apply;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private Label label2;
        private Label lbl_AutoLoadHelp;
        private ToolTip toolTip_AutoLoad;
        private Label lbl_DefaultRomFolder;
        private ToolTip toolTip_DefaultRomFolder;
        private GroupBox groupBox3;
        private Label label3;
        private Label label4;
        private Button btn_ExportLogs;
        private Label label5;
        private CheckBox checkBox_ShowConsole;
        private Button btn_Close;
    }
}