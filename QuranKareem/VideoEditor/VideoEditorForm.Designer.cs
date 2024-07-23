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
            this.videoWidthLabel = new System.Windows.Forms.Label();
            this.videoWidth = new System.Windows.Forms.NumericUpDown();
            this.videoHeight = new System.Windows.Forms.NumericUpDown();
            this.videoHeightLabel = new System.Windows.Forms.Label();
            this.frameRate = new System.Windows.Forms.NumericUpDown();
            this.frameRateLabel = new System.Windows.Forms.Label();
            this.audioBitRate = new System.Windows.Forms.NumericUpDown();
            this.audioBitRateLabel = new System.Windows.Forms.Label();
            this.audioLengthLabel = new System.Windows.Forms.Label();
            this.audioLength = new System.Windows.Forms.Label();
            this.generate = new System.Windows.Forms.Button();
            this.locY = new System.Windows.Forms.NumericUpDown();
            this.locX = new System.Windows.Forms.NumericUpDown();
            this.widthHeight = new System.Windows.Forms.Label();
            this.locationLabel = new System.Windows.Forms.Label();
            this.lineHeight = new System.Windows.Forms.NumericUpDown();
            this.lineWidth = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.videoWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioBitRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.locX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineWidth)).BeginInit();
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
            this.videoPathLabel.Location = new System.Drawing.Point(620, 113);
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
            this.videoNameLabel.Location = new System.Drawing.Point(12, 106);
            this.videoNameLabel.Name = "videoNameLabel";
            this.videoNameLabel.Size = new System.Drawing.Size(483, 36);
            this.videoNameLabel.TabIndex = 4;
            this.videoNameLabel.Text = "Video.mp4";
            this.videoNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoPath
            // 
            this.videoPath.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoPath.Location = new System.Drawing.Point(501, 106);
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
            this.videoOutputPath.Location = new System.Drawing.Point(501, 160);
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
            this.videoOutputPathLabel.Location = new System.Drawing.Point(620, 167);
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
            this.videoOutputNameLabel.Location = new System.Drawing.Point(12, 160);
            this.videoOutputNameLabel.Name = "videoOutputNameLabel";
            this.videoOutputNameLabel.Size = new System.Drawing.Size(483, 36);
            this.videoOutputNameLabel.TabIndex = 7;
            this.videoOutputNameLabel.Text = "Output.mp4";
            this.videoOutputNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "VideoOutput";
            this.saveFileDialog.Filter = ".mp4|Video Files";
            // 
            // videoWidthLabel
            // 
            this.videoWidthLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoWidthLabel.Location = new System.Drawing.Point(604, 229);
            this.videoWidthLabel.Name = "videoWidthLabel";
            this.videoWidthLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoWidthLabel.Size = new System.Drawing.Size(168, 23);
            this.videoWidthLabel.TabIndex = 9;
            this.videoWidthLabel.Text = "عرض الفيديو:";
            this.videoWidthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // videoWidth
            // 
            this.videoWidth.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.videoWidth.Location = new System.Drawing.Point(445, 227);
            this.videoWidth.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.videoWidth.Minimum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.videoWidth.Name = "videoWidth";
            this.videoWidth.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.videoWidth.Size = new System.Drawing.Size(120, 30);
            this.videoWidth.TabIndex = 10;
            this.videoWidth.Value = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            this.videoWidth.ValueChanged += new System.EventHandler(this.VideoWidth_ValueChanged);
            // 
            // videoHeight
            // 
            this.videoHeight.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoHeight.Location = new System.Drawing.Point(54, 227);
            this.videoHeight.Maximum = new decimal(new int[] {
            4320,
            0,
            0,
            0});
            this.videoHeight.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.videoHeight.Name = "videoHeight";
            this.videoHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.videoHeight.Size = new System.Drawing.Size(120, 30);
            this.videoHeight.TabIndex = 12;
            this.videoHeight.Value = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            this.videoHeight.ValueChanged += new System.EventHandler(this.VideoHeight_ValueChanged);
            // 
            // videoHeightLabel
            // 
            this.videoHeightLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.videoHeightLabel.Location = new System.Drawing.Point(202, 229);
            this.videoHeightLabel.Name = "videoHeightLabel";
            this.videoHeightLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.videoHeightLabel.Size = new System.Drawing.Size(168, 23);
            this.videoHeightLabel.TabIndex = 11;
            this.videoHeightLabel.Text = "إرتفاع الفيديو:";
            this.videoHeightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frameRate
            // 
            this.frameRate.Font = new System.Drawing.Font("Tahoma", 14F);
            this.frameRate.Location = new System.Drawing.Point(445, 276);
            this.frameRate.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.frameRate.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.frameRate.Name = "frameRate";
            this.frameRate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.frameRate.Size = new System.Drawing.Size(120, 30);
            this.frameRate.TabIndex = 14;
            this.frameRate.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.frameRate.ValueChanged += new System.EventHandler(this.FrameRate_ValueChanged);
            // 
            // frameRateLabel
            // 
            this.frameRateLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.frameRateLabel.Location = new System.Drawing.Point(604, 278);
            this.frameRateLabel.Name = "frameRateLabel";
            this.frameRateLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.frameRateLabel.Size = new System.Drawing.Size(168, 23);
            this.frameRateLabel.TabIndex = 13;
            this.frameRateLabel.Text = "عدد الإطارات\\ثانية:";
            this.frameRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // audioBitRate
            // 
            this.audioBitRate.Font = new System.Drawing.Font("Tahoma", 14F);
            this.audioBitRate.Increment = new decimal(new int[] {
            32000,
            0,
            0,
            0});
            this.audioBitRate.Location = new System.Drawing.Point(54, 276);
            this.audioBitRate.Maximum = new decimal(new int[] {
            256000,
            0,
            0,
            0});
            this.audioBitRate.Minimum = new decimal(new int[] {
            96000,
            0,
            0,
            0});
            this.audioBitRate.Name = "audioBitRate";
            this.audioBitRate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.audioBitRate.Size = new System.Drawing.Size(120, 30);
            this.audioBitRate.TabIndex = 16;
            this.audioBitRate.Value = new decimal(new int[] {
            128000,
            0,
            0,
            0});
            this.audioBitRate.ValueChanged += new System.EventHandler(this.AudioBitRate_ValueChanged);
            // 
            // audioBitRateLabel
            // 
            this.audioBitRateLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.audioBitRateLabel.Location = new System.Drawing.Point(202, 280);
            this.audioBitRateLabel.Name = "audioBitRateLabel";
            this.audioBitRateLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.audioBitRateLabel.Size = new System.Drawing.Size(168, 23);
            this.audioBitRateLabel.TabIndex = 15;
            this.audioBitRateLabel.Text = "جودة الصوت:";
            this.audioBitRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // audioLengthLabel
            // 
            this.audioLengthLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.audioLengthLabel.Location = new System.Drawing.Point(342, 459);
            this.audioLengthLabel.Name = "audioLengthLabel";
            this.audioLengthLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.audioLengthLabel.Size = new System.Drawing.Size(348, 36);
            this.audioLengthLabel.TabIndex = 17;
            this.audioLengthLabel.Text = "طول الصفحة القرآنية للشيخ الحالي: ";
            this.audioLengthLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // audioLength
            // 
            this.audioLength.Font = new System.Drawing.Font("Tahoma", 14F);
            this.audioLength.Location = new System.Drawing.Point(106, 459);
            this.audioLength.Name = "audioLength";
            this.audioLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.audioLength.Size = new System.Drawing.Size(242, 36);
            this.audioLength.TabIndex = 18;
            this.audioLength.Text = "0";
            this.audioLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // generate
            // 
            this.generate.Font = new System.Drawing.Font("Tahoma", 14F);
            this.generate.Location = new System.Drawing.Point(292, 508);
            this.generate.Name = "generate";
            this.generate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.generate.Size = new System.Drawing.Size(203, 49);
            this.generate.TabIndex = 19;
            this.generate.Text = "إنشاء ملف المشروع";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // locY
            // 
            this.locY.Font = new System.Drawing.Font("Tahoma", 14F);
            this.locY.Location = new System.Drawing.Point(54, 346);
            this.locY.Maximum = new decimal(new int[] {
            1080,
            0,
            0,
            0});
            this.locY.Name = "locY";
            this.locY.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.locY.Size = new System.Drawing.Size(120, 30);
            this.locY.TabIndex = 23;
            this.locY.Value = new decimal(new int[] {
            885,
            0,
            0,
            0});
            // 
            // locX
            // 
            this.locX.Font = new System.Drawing.Font("Tahoma", 14F);
            this.locX.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.locX.Location = new System.Drawing.Point(445, 351);
            this.locX.Maximum = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            this.locX.Name = "locX";
            this.locX.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.locX.Size = new System.Drawing.Size(120, 30);
            this.locX.TabIndex = 21;
            this.locX.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // widthHeight
            // 
            this.widthHeight.Font = new System.Drawing.Font("Tahoma", 14F);
            this.widthHeight.Location = new System.Drawing.Point(101, 403);
            this.widthHeight.Name = "widthHeight";
            this.widthHeight.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.widthHeight.Size = new System.Drawing.Size(687, 23);
            this.widthHeight.TabIndex = 28;
            this.widthHeight.Text = "العرض =                                                    ،                  الإ" +
    "رتفاع =                                       ،";
            this.widthHeight.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // locationLabel
            // 
            this.locationLabel.Font = new System.Drawing.Font("Tahoma", 14F);
            this.locationLabel.Location = new System.Drawing.Point(337, 353);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.locationLabel.Size = new System.Drawing.Size(451, 23);
            this.locationLabel.TabIndex = 22;
            this.locationLabel.Text = "نقطة بداية سطر القرآن =                             ،";
            this.locationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lineHeight
            // 
            this.lineHeight.Font = new System.Drawing.Font("Tahoma", 14F);
            this.lineHeight.Location = new System.Drawing.Point(54, 400);
            this.lineHeight.Maximum = new decimal(new int[] {
            4320,
            0,
            0,
            0});
            this.lineHeight.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.lineHeight.Name = "lineHeight";
            this.lineHeight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lineHeight.Size = new System.Drawing.Size(120, 30);
            this.lineHeight.TabIndex = 30;
            this.lineHeight.Value = new decimal(new int[] {
            181,
            0,
            0,
            0});
            // 
            // lineWidth
            // 
            this.lineWidth.Font = new System.Drawing.Font("Tahoma", 14F);
            this.lineWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.lineWidth.Location = new System.Drawing.Point(445, 400);
            this.lineWidth.Maximum = new decimal(new int[] {
            7680,
            0,
            0,
            0});
            this.lineWidth.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.lineWidth.Name = "lineWidth";
            this.lineWidth.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lineWidth.Size = new System.Drawing.Size(120, 30);
            this.lineWidth.TabIndex = 29;
            this.lineWidth.Value = new decimal(new int[] {
            1421,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F);
            this.label2.Location = new System.Drawing.Point(421, 357);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 19);
            this.label2.TabIndex = 31;
            this.label2.Text = "X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F);
            this.label1.Location = new System.Drawing.Point(180, 351);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 19);
            this.label1.TabIndex = 32;
            this.label1.Text = "Y";
            // 
            // VideoEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 569);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lineHeight);
            this.Controls.Add(this.lineWidth);
            this.Controls.Add(this.widthHeight);
            this.Controls.Add(this.locY);
            this.Controls.Add(this.locX);
            this.Controls.Add(this.generate);
            this.Controls.Add(this.audioLength);
            this.Controls.Add(this.audioLengthLabel);
            this.Controls.Add(this.audioBitRate);
            this.Controls.Add(this.audioBitRateLabel);
            this.Controls.Add(this.frameRate);
            this.Controls.Add(this.frameRateLabel);
            this.Controls.Add(this.videoHeight);
            this.Controls.Add(this.videoHeightLabel);
            this.Controls.Add(this.videoWidth);
            this.Controls.Add(this.videoWidthLabel);
            this.Controls.Add(this.videoOutputPath);
            this.Controls.Add(this.videoOutputPathLabel);
            this.Controls.Add(this.videoOutputNameLabel);
            this.Controls.Add(this.videoPath);
            this.Controls.Add(this.videoPathLabel);
            this.Controls.Add(this.vidiotLink);
            this.Controls.Add(this.vidiotRadio);
            this.Controls.Add(this.videoNameLabel);
            this.Controls.Add(this.locationLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "VideoEditorForm";
            this.Text = "Video Editor";
            this.Load += new System.EventHandler(this.VideoEditorForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.videoWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.videoHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.frameRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.audioBitRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.locX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineWidth)).EndInit();
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
        private System.Windows.Forms.Label videoWidthLabel;
        private System.Windows.Forms.NumericUpDown videoWidth;
        private System.Windows.Forms.NumericUpDown videoHeight;
        private System.Windows.Forms.Label videoHeightLabel;
        private System.Windows.Forms.NumericUpDown frameRate;
        private System.Windows.Forms.Label frameRateLabel;
        private System.Windows.Forms.NumericUpDown audioBitRate;
        private System.Windows.Forms.Label audioBitRateLabel;
        private System.Windows.Forms.Label audioLengthLabel;
        private System.Windows.Forms.Label audioLength;
        private System.Windows.Forms.Button generate;
        private System.Windows.Forms.NumericUpDown locY;
        private System.Windows.Forms.NumericUpDown locX;
        private System.Windows.Forms.Label widthHeight;
        private System.Windows.Forms.Label locationLabel;
        private System.Windows.Forms.NumericUpDown lineHeight;
        private System.Windows.Forms.NumericUpDown lineWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}