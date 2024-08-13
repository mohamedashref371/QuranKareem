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
            this.saveTextDialog = new System.Windows.Forms.SaveFileDialog();
            this.openVttDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.youtubeLink = new System.Windows.Forms.LinkLabel();
            this.openMp3Dialog = new System.Windows.Forms.OpenFileDialog();
            this.mp3Path = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.mp3NameLabel = new System.Windows.Forms.Label();
            this.videoEditor = new System.Windows.Forms.Button();
            this.textPath = new System.Windows.Forms.Button();
            this.openTextDialog = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.arrow1 = new System.Windows.Forms.Label();
            this.arrow2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.arrow4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.arrow3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // vttPath
            // 
            this.vttPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.vttPath.Location = new System.Drawing.Point(462, 120);
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
            this.vttPathLabel.Location = new System.Drawing.Point(593, 127);
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
            this.txtNameLabel.Location = new System.Drawing.Point(12, 180);
            this.txtNameLabel.Name = "txtNameLabel";
            this.txtNameLabel.Size = new System.Drawing.Size(444, 36);
            this.txtNameLabel.TabIndex = 7;
            this.txtNameLabel.Text = "captions.txt";
            this.txtNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveTextDialog
            // 
            this.saveTextDialog.FileName = "captions.txt";
            this.saveTextDialog.Filter = "TEXT Files|*.txt";
            // 
            // openVttDialog
            // 
            this.openVttDialog.FileName = "captions.vtt";
            this.openVttDialog.Filter = "VTT Files|*.vtt";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label1.Location = new System.Drawing.Point(218, 244);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label1.Size = new System.Drawing.Size(364, 69);
            this.label1.TabIndex = 9;
            this.label1.Text = "قم بإصلاح ملف الـ captions.txt الناتج\r\nبحيث تكون كلماته صحيحة\r\nويبدأ من بداية الص" +
    "فحة الحالية وينتهي بنهايتها";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // youtubeLink
            // 
            this.youtubeLink.AutoSize = true;
            this.youtubeLink.Font = new System.Drawing.Font("Tahoma", 14F);
            this.youtubeLink.Location = new System.Drawing.Point(40, 14);
            this.youtubeLink.Name = "youtubeLink";
            this.youtubeLink.Size = new System.Drawing.Size(238, 23);
            this.youtubeLink.TabIndex = 10;
            this.youtubeLink.TabStop = true;
            this.youtubeLink.Text = "https://studio.youtube.com";
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
            // textPath
            // 
            this.textPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.textPath.Location = new System.Drawing.Point(462, 181);
            this.textPath.Name = "textPath";
            this.textPath.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.textPath.Size = new System.Drawing.Size(104, 36);
            this.textPath.TabIndex = 90;
            this.textPath.Text = "إختيار ...";
            this.textPath.UseVisualStyleBackColor = true;
            this.textPath.Click += new System.EventHandler(this.TextPath_Click);
            // 
            // openTextDialog
            // 
            this.openTextDialog.FileName = "captions.txt";
            this.openTextDialog.Filter = "TEXT Files|*.txt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label3.Location = new System.Drawing.Point(619, 172);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label3.Size = new System.Drawing.Size(158, 46);
            this.label3.TabIndex = 91;
            this.label3.Text = "لدي ملف مستخرج\r\nمسبقا بالفعل :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label4.Location = new System.Drawing.Point(328, 14);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label4.Size = new System.Drawing.Size(157, 23);
            this.label4.TabIndex = 92;
            this.label4.Text = "اختر قسم الترجمة";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // arrow1
            // 
            this.arrow1.AutoSize = true;
            this.arrow1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.arrow1.Location = new System.Drawing.Point(284, 14);
            this.arrow1.Name = "arrow1";
            this.arrow1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.arrow1.Size = new System.Drawing.Size(38, 23);
            this.arrow1.TabIndex = 93;
            this.arrow1.Text = "-->";
            this.arrow1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // arrow2
            // 
            this.arrow2.AutoSize = true;
            this.arrow2.Font = new System.Drawing.Font("Tahoma", 14F);
            this.arrow2.Location = new System.Drawing.Point(491, 14);
            this.arrow2.Name = "arrow2";
            this.arrow2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.arrow2.Size = new System.Drawing.Size(38, 23);
            this.arrow2.TabIndex = 95;
            this.arrow2.Text = "-->";
            this.arrow2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label7.Location = new System.Drawing.Point(535, 14);
            this.label7.Name = "label7";
            this.label7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label7.Size = new System.Drawing.Size(191, 23);
            this.label7.TabIndex = 94;
            this.label7.Text = "اختر فيديو القرآن الكريم";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label8.Location = new System.Drawing.Point(392, 52);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label8.Size = new System.Drawing.Size(186, 23);
            this.label8.TabIndex = 96;
            this.label8.Text = "العربية (ترجمة تلقائية)";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // arrow4
            // 
            this.arrow4.AutoSize = true;
            this.arrow4.Font = new System.Drawing.Font("Tahoma", 14F);
            this.arrow4.Location = new System.Drawing.Point(306, 52);
            this.arrow4.Name = "arrow4";
            this.arrow4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.arrow4.Size = new System.Drawing.Size(38, 23);
            this.arrow4.TabIndex = 97;
            this.arrow4.Text = "-->";
            this.arrow4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label10.Location = new System.Drawing.Point(185, 52);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label10.Size = new System.Drawing.Size(82, 23);
            this.label10.TabIndex = 98;
            this.label10.Text = "تنزيل .vtt";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // arrow3
            // 
            this.arrow3.AutoSize = true;
            this.arrow3.Font = new System.Drawing.Font("Tahoma", 14F);
            this.arrow3.Location = new System.Drawing.Point(611, 52);
            this.arrow3.Name = "arrow3";
            this.arrow3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.arrow3.Size = new System.Drawing.Size(38, 23);
            this.arrow3.TabIndex = 99;
            this.arrow3.Text = "-->";
            this.arrow3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label5.Location = new System.Drawing.Point(218, 127);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label5.Size = new System.Drawing.Size(228, 23);
            this.label5.TabIndex = 100;
            this.label5.Text = "لاستخراج ملف captions.txt";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // YoutubeCaptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 569);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.arrow3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.arrow4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.arrow2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.arrow1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textPath);
            this.Controls.Add(this.videoEditor);
            this.Controls.Add(this.mp3NameLabel);
            this.Controls.Add(this.mp3Path);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.youtubeLink);
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
        private System.Windows.Forms.SaveFileDialog saveTextDialog;
        private System.Windows.Forms.OpenFileDialog openVttDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel youtubeLink;
        private System.Windows.Forms.OpenFileDialog openMp3Dialog;
        private System.Windows.Forms.Button mp3Path;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label mp3NameLabel;
        private System.Windows.Forms.Button videoEditor;
        private System.Windows.Forms.Button textPath;
        private System.Windows.Forms.OpenFileDialog openTextDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label arrow1;
        private System.Windows.Forms.Label arrow2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label arrow4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label arrow3;
        private System.Windows.Forms.Label label5;
    }
}