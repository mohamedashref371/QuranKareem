namespace QuranKareem
{
    partial class YoutubeCaptionsForm
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
            this.vttPath = new System.Windows.Forms.Button();
            this.vttPathLabel = new System.Windows.Forms.Label();
            this.txtNameLabel = new System.Windows.Forms.Label();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.vidiotLink = new System.Windows.Forms.LinkLabel();
            this.openMp3Dialog = new System.Windows.Forms.OpenFileDialog();
            this.mp3Path = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.mp3NameLabel = new System.Windows.Forms.Label();
            this.videoEditor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vttPath
            // 
            this.vttPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.vttPath.Location = new System.Drawing.Point(202, 120);
            this.vttPath.Name = "vttPath";
            this.vttPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.vttPath.Size = new System.Drawing.Size(104, 36);
            this.vttPath.TabIndex = 8;
            this.vttPath.Text = "إختيار ...";
            this.vttPath.UseVisualStyleBackColor = true;
            this.vttPath.Click += new System.EventHandler(this.VttPath_Click);
            // 
            // vttPathLabel
            // 
            this.vttPathLabel.AutoSize = true;
            this.vttPathLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.vttPathLabel.Location = new System.Drawing.Point(414, 127);
            this.vttPathLabel.Name = "vttPathLabel";
            this.vttPathLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.vttPathLabel.Size = new System.Drawing.Size(195, 23);
            this.vttPathLabel.TabIndex = 6;
            this.vttPathLabel.Text = "مسار الـ  captions.vtt :";
            this.vttPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtNameLabel
            // 
            this.txtNameLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtNameLabel.Location = new System.Drawing.Point(12, 170);
            this.txtNameLabel.Name = "txtNameLabel";
            this.txtNameLabel.Size = new System.Drawing.Size(776, 36);
            this.txtNameLabel.TabIndex = 7;
            this.txtNameLabel.Text = "captions.txt";
            this.txtNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "captions.txt";
            this.saveFileDialog.Filter = "TEXT Files|*.txt";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "captions.vtt";
            this.openFileDialog.Filter = "VTT Files|*.vtt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label1.Location = new System.Drawing.Point(218, 243);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(364, 69);
            this.label1.TabIndex = 9;
            this.label1.Text = "قم بإصلاح ملف الـ captions.txt الناتج\r\nبحيث تكون كلماته صحيحة\r\nويبدأ من بداية الص" +
    "فحة الحالية وينتهي بنهايتها";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // vidiotLink
            // 
            this.vidiotLink.AutoSize = true;
            this.vidiotLink.Font = new System.Drawing.Font("Tahoma", 14F);
            this.vidiotLink.Location = new System.Drawing.Point(12, 9);
            this.vidiotLink.Name = "vidiotLink";
            this.vidiotLink.Size = new System.Drawing.Size(391, 23);
            this.vidiotLink.TabIndex = 10;
            this.vidiotLink.TabStop = true;
            this.vidiotLink.Text = "https://studio.youtube.com/video/translations";
            // 
            // openMp3Dialog
            // 
            this.openMp3Dialog.FileName = "audio.mp3";
            this.openMp3Dialog.Filter = "Audio Files|*.mp3";
            // 
            // mp3Path
            // 
            this.mp3Path.Font = new System.Drawing.Font("Tahoma", 14F);
            this.mp3Path.Location = new System.Drawing.Point(202, 365);
            this.mp3Path.Name = "mp3Path";
            this.mp3Path.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.mp3Path.Size = new System.Drawing.Size(104, 36);
            this.mp3Path.TabIndex = 12;
            this.mp3Path.Text = "إختيار ...";
            this.mp3Path.UseVisualStyleBackColor = true;
            this.mp3Path.Click += new System.EventHandler(this.Mp3Path_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label2.Location = new System.Drawing.Point(414, 372);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label2.Size = new System.Drawing.Size(175, 23);
            this.label2.TabIndex = 11;
            this.label2.Text = "مسار السورة mp3 : ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mp3NameLabel
            // 
            this.mp3NameLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.mp3NameLabel.Location = new System.Drawing.Point(12, 412);
            this.mp3NameLabel.Name = "mp3NameLabel";
            this.mp3NameLabel.Size = new System.Drawing.Size(776, 36);
            this.mp3NameLabel.TabIndex = 13;
            this.mp3NameLabel.Text = "audio.mp3";
            this.mp3NameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoEditor
            // 
            this.videoEditor.Font = new System.Drawing.Font("Tahoma", 20F);
            this.videoEditor.Location = new System.Drawing.Point(244, 486);
            this.videoEditor.Name = "videoEditor";
            this.videoEditor.Size = new System.Drawing.Size(277, 71);
            this.videoEditor.TabIndex = 89;
            this.videoEditor.Text = "Quran Template";
            this.videoEditor.UseVisualStyleBackColor = true;
            this.videoEditor.Click += new System.EventHandler(this.VideoEditor_Click);
            // 
            // YoutubeCaptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 569);
            this.Controls.Add(this.videoEditor);
            this.Controls.Add(this.mp3NameLabel);
            this.Controls.Add(this.mp3Path);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vidiotLink);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.vttPath);
            this.Controls.Add(this.vttPathLabel);
            this.Controls.Add(this.txtNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "YoutubeCaptionsForm";
            this.Text = "Youtube Captions";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button vttPath;
        private System.Windows.Forms.Label vttPathLabel;
        private System.Windows.Forms.Label txtNameLabel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel vidiotLink;
        private System.Windows.Forms.OpenFileDialog openMp3Dialog;
        private System.Windows.Forms.Button mp3Path;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label mp3NameLabel;
        private System.Windows.Forms.Button videoEditor;
    }
}