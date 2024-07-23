namespace QuranKareem
{
    partial class VideoEditorForm
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
            this.vidiotRadio = new System.Windows.Forms.RadioButton();
            this.vidiotLink = new System.Windows.Forms.LinkLabel();
            this.videoPathLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.videoNameLabel = new System.Windows.Forms.Label();
            this.videoPath = new System.Windows.Forms.Button();
            this.videoOutputPath = new System.Windows.Forms.Button();
            this.videoOutputPathLabel = new System.Windows.Forms.Label();
            this.videoOutputNameLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // vidiotRadio
            // 
            this.vidiotRadio.AutoSize = true;
            this.vidiotRadio.Checked = true;
            this.vidiotRadio.Font = new System.Drawing.Font("Tahoma", 16F);
            this.vidiotRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(45)))), ((int)(((byte)(200)))));
            this.vidiotRadio.Location = new System.Drawing.Point(12, 12);
            this.vidiotRadio.Name = "vidiotRadio";
            this.vidiotRadio.Size = new System.Drawing.Size(164, 31);
            this.vidiotRadio.TabIndex = 1;
            this.vidiotRadio.TabStop = true;
            this.vidiotRadio.Text = "Vidiot v0.3.39";
            this.vidiotRadio.UseVisualStyleBackColor = true;
            // 
            // vidiotLink
            // 
            this.vidiotLink.AutoSize = true;
            this.vidiotLink.Font = new System.Drawing.Font("Tahoma", 14F);
            this.vidiotLink.Location = new System.Drawing.Point(25, 50);
            this.vidiotLink.Name = "vidiotLink";
            this.vidiotLink.Size = new System.Drawing.Size(329, 23);
            this.vidiotLink.TabIndex = 2;
            this.vidiotLink.TabStop = true;
            this.vidiotLink.Text = "https://sourceforge.net/projects/vidiot";
            // 
            // videoPathLabel
            // 
            this.videoPathLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoPathLabel.Location = new System.Drawing.Point(620, 127);
            this.videoPathLabel.Name = "videoPathLabel";
            this.videoPathLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoPathLabel.Size = new System.Drawing.Size(168, 23);
            this.videoPathLabel.TabIndex = 3;
            this.videoPathLabel.Text = "مسار الفيديو:";
            this.videoPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "VideoInput";
            this.openFileDialog.Filter = "*.mp4|Video Files";
            // 
            // videoNameLabel
            // 
            this.videoNameLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.videoNameLabel.Location = new System.Drawing.Point(9, 120);
            this.videoNameLabel.Name = "videoNameLabel";
            this.videoNameLabel.Size = new System.Drawing.Size(493, 36);
            this.videoNameLabel.TabIndex = 4;
            this.videoNameLabel.Text = "Video.mp4";
            this.videoNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoPath
            // 
            this.videoPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoPath.Location = new System.Drawing.Point(510, 120);
            this.videoPath.Name = "videoPath";
            this.videoPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoPath.Size = new System.Drawing.Size(104, 36);
            this.videoPath.TabIndex = 5;
            this.videoPath.Text = "إختيار ...";
            this.videoPath.UseVisualStyleBackColor = true;
            this.videoPath.Click += new System.EventHandler(this.VideoPath_Click);
            // 
            // videoOutputPath
            // 
            this.videoOutputPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoOutputPath.Location = new System.Drawing.Point(510, 166);
            this.videoOutputPath.Name = "videoOutputPath";
            this.videoOutputPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoOutputPath.Size = new System.Drawing.Size(104, 36);
            this.videoOutputPath.TabIndex = 8;
            this.videoOutputPath.Text = "إختيار ...";
            this.videoOutputPath.UseVisualStyleBackColor = true;
            this.videoOutputPath.Click += new System.EventHandler(this.VideoOutputPath_Click);
            // 
            // videoOutputPathLabel
            // 
            this.videoOutputPathLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoOutputPathLabel.Location = new System.Drawing.Point(620, 173);
            this.videoOutputPathLabel.Name = "videoOutputPathLabel";
            this.videoOutputPathLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoOutputPathLabel.Size = new System.Drawing.Size(168, 23);
            this.videoOutputPathLabel.TabIndex = 6;
            this.videoOutputPathLabel.Text = "مسار إخراج الفيديو:";
            this.videoOutputPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoOutputNameLabel
            // 
            this.videoOutputNameLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.videoOutputNameLabel.Location = new System.Drawing.Point(11, 166);
            this.videoOutputNameLabel.Name = "videoOutputNameLabel";
            this.videoOutputNameLabel.Size = new System.Drawing.Size(493, 36);
            this.videoOutputNameLabel.TabIndex = 7;
            this.videoOutputNameLabel.Text = "Output.mp4";
            this.videoOutputNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "VideoOutput";
            this.saveFileDialog.Filter = ".mp4|Video Files";
            // 
            // VideoEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 569);
            this.Controls.Add(this.videoOutputPath);
            this.Controls.Add(this.videoOutputPathLabel);
            this.Controls.Add(this.videoOutputNameLabel);
            this.Controls.Add(this.videoPath);
            this.Controls.Add(this.videoPathLabel);
            this.Controls.Add(this.vidiotLink);
            this.Controls.Add(this.vidiotRadio);
            this.Controls.Add(this.videoNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "VideoEditorForm";
            this.Text = "Video Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton vidiotRadio;
        private System.Windows.Forms.LinkLabel vidiotLink;
        private System.Windows.Forms.Label videoPathLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label videoNameLabel;
        private System.Windows.Forms.Button videoPath;
        private System.Windows.Forms.Button videoOutputPath;
        private System.Windows.Forms.Label videoOutputPathLabel;
        private System.Windows.Forms.Label videoOutputNameLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}