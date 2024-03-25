namespace Main.Forms
{
    partial class LoadingData
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
            progressBar = new ProgressBar();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 12);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(400, 23);
            progressBar.TabIndex = 0;
            // 
            // LoadingData
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(424, 50);
            ControlBox = false;
            Controls.Add(progressBar);
            MaximizeBox = false;
            MaximumSize = new Size(440, 89);
            MinimizeBox = false;
            MinimumSize = new Size(440, 89);
            Name = "LoadingData";
            StartPosition = FormStartPosition.CenterParent;
            Text = "LoadingData";
            TopMost = true;
            UseWaitCursor = true;
            ResumeLayout(false);
        }

        #endregion

        private ProgressBar progressBar;
    }
}