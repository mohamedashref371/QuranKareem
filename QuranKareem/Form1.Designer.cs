namespace QuranKareem
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.quranPic = new System.Windows.Forms.PictureBox();
            this.Surah = new System.Windows.Forms.NumericUpDown();
            this.Page = new System.Windows.Forms.NumericUpDown();
            this.Ayah = new System.Windows.Forms.NumericUpDown();
            this.Quarter = new System.Windows.Forms.NumericUpDown();
            this.Surahs = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.color = new System.Windows.Forms.ComboBox();
            this.folder = new System.Windows.Forms.FolderBrowserDialog();
            this.AyahRepeat = new System.Windows.Forms.NumericUpDown();
            this.AyahRepeatCheck = new System.Windows.Forms.CheckBox();
            this.SurahRepeat = new System.Windows.Forms.NumericUpDown();
            this.time5 = new System.Windows.Forms.Label();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.SurahRepeatCheck = new System.Windows.Forms.CheckBox();
            this.pause = new System.Windows.Forms.Button();
            this.Rate = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.exitForm = new System.Windows.Forms.Button();
            this.minimize = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.Juz = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.Hizb = new System.Windows.Forms.NumericUpDown();
            this.quranHtmlText = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.copy = new System.Windows.Forms.Button();
            this.normalText = new System.Windows.Forms.CheckBox();
            this.searchList = new System.Windows.Forms.ListBox();
            this.search = new System.Windows.Forms.Button();
            this.searchText = new System.Windows.Forms.TextBox();
            this.searchClose = new System.Windows.Forms.Button();
            this.tafasir = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tafseerCopy = new System.Windows.Forms.Button();
            this.saveRTF = new System.Windows.Forms.Button();
            this.saveRichText = new System.Windows.Forms.SaveFileDialog();
            this.rtb = new System.Windows.Forms.RichTextBox();
            this.about = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.quranPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Surah)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Page)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Ayah)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Quarter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AyahRepeat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SurahRepeat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Juz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Hizb)).BeginInit();
            this.SuspendLayout();
            // 
            // quranPic
            // 
            this.quranPic.BackColor = System.Drawing.Color.Transparent;
            this.quranPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.quranPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.quranPic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.quranPic.Location = new System.Drawing.Point(240, 5);
            this.quranPic.Name = "quranPic";
            this.quranPic.Size = new System.Drawing.Size(510, 900);
            this.quranPic.TabIndex = 2;
            this.quranPic.TabStop = false;
            this.quranPic.Click += new System.EventHandler(this.QuranPic_Click);
            this.quranPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.QuranPic_MouseMove);
            // 
            // Surah
            // 
            this.Surah.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Surah.Location = new System.Drawing.Point(35, 47);
            this.Surah.Maximum = new decimal(new int[] {
            114,
            0,
            0,
            0});
            this.Surah.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Surah.Name = "Surah";
            this.Surah.Size = new System.Drawing.Size(95, 28);
            this.Surah.TabIndex = 9;
            this.Surah.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Surah.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Surah.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Surah.ValueChanged += new System.EventHandler(this.Surah_ValueChanged);
            // 
            // Page
            // 
            this.Page.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Page.Location = new System.Drawing.Point(35, 212);
            this.Page.Maximum = new decimal(new int[] {
            604,
            0,
            0,
            0});
            this.Page.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Page.Name = "Page";
            this.Page.Size = new System.Drawing.Size(95, 28);
            this.Page.TabIndex = 11;
            this.Page.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Page.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Page.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Page.ValueChanged += new System.EventHandler(this.Page_ValueChanged);
            // 
            // Ayah
            // 
            this.Ayah.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Ayah.Location = new System.Drawing.Point(35, 257);
            this.Ayah.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.Ayah.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Ayah.Name = "Ayah";
            this.Ayah.Size = new System.Drawing.Size(95, 28);
            this.Ayah.TabIndex = 12;
            this.Ayah.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Ayah.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Ayah.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Ayah.ValueChanged += new System.EventHandler(this.Ayah_ValueChanged);
            // 
            // Quarter
            // 
            this.Quarter.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Quarter.Location = new System.Drawing.Point(35, 164);
            this.Quarter.Maximum = new decimal(new int[] {
            240,
            0,
            0,
            0});
            this.Quarter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Quarter.Name = "Quarter";
            this.Quarter.Size = new System.Drawing.Size(95, 28);
            this.Quarter.TabIndex = 10;
            this.Quarter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Quarter.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Quarter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Quarter.ValueChanged += new System.EventHandler(this.Quarter_ValueChanged);
            // 
            // Surahs
            // 
            this.Surahs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Surahs.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Surahs.FormattingEnabled = true;
            this.Surahs.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.Surahs.Location = new System.Drawing.Point(35, 12);
            this.Surahs.Name = "Surahs";
            this.Surahs.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Surahs.Size = new System.Drawing.Size(95, 29);
            this.Surahs.TabIndex = 8;
            this.Surahs.SelectedIndexChanged += new System.EventHandler(this.Surahs_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label1.Location = new System.Drawing.Point(145, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 22);
            this.label1.TabIndex = 10;
            this.label1.Text = "السورة";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label2.Location = new System.Drawing.Point(145, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 22);
            this.label2.TabIndex = 11;
            this.label2.Text = "السورة";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label3.Location = new System.Drawing.Point(159, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 22);
            this.label3.TabIndex = 12;
            this.label3.Text = "الربع";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label4.Location = new System.Drawing.Point(145, 214);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 22);
            this.label4.TabIndex = 13;
            this.label4.Text = "الصفحة";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label5.Location = new System.Drawing.Point(159, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 22);
            this.label5.TabIndex = 14;
            this.label5.Text = "الآية";
            // 
            // color
            // 
            this.color.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.color.Font = new System.Drawing.Font("Tahoma", 13F);
            this.color.FormattingEnabled = true;
            this.color.Items.AddRange(new object[] {
            "لا شيء",
            "أحمر",
            "أخضر",
            "أزرق",
            "سماوي غامق",
            "أحمر غامق"});
            this.color.Location = new System.Drawing.Point(20, 332);
            this.color.Name = "color";
            this.color.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.color.Size = new System.Drawing.Size(109, 29);
            this.color.TabIndex = 1;
            this.color.SelectedIndexChanged += new System.EventHandler(this.Color_SelectedIndexChanged);
            // 
            // AyahRepeat
            // 
            this.AyahRepeat.Font = new System.Drawing.Font("Tahoma", 13F);
            this.AyahRepeat.Location = new System.Drawing.Point(20, 414);
            this.AyahRepeat.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.AyahRepeat.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.AyahRepeat.Name = "AyahRepeat";
            this.AyahRepeat.Size = new System.Drawing.Size(55, 28);
            this.AyahRepeat.TabIndex = 6;
            this.AyahRepeat.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // AyahRepeatCheck
            // 
            this.AyahRepeatCheck.AutoSize = true;
            this.AyahRepeatCheck.Font = new System.Drawing.Font("Tahoma", 13F);
            this.AyahRepeatCheck.Location = new System.Drawing.Point(102, 416);
            this.AyahRepeatCheck.Name = "AyahRepeatCheck";
            this.AyahRepeatCheck.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AyahRepeatCheck.Size = new System.Drawing.Size(99, 26);
            this.AyahRepeatCheck.TabIndex = 4;
            this.AyahRepeatCheck.Text = "تكرار الآية";
            this.AyahRepeatCheck.UseVisualStyleBackColor = true;
            this.AyahRepeatCheck.CheckedChanged += new System.EventHandler(this.Repeat_CheckedChanged);
            // 
            // SurahRepeat
            // 
            this.SurahRepeat.Font = new System.Drawing.Font("Tahoma", 13F);
            this.SurahRepeat.Location = new System.Drawing.Point(20, 380);
            this.SurahRepeat.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SurahRepeat.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.SurahRepeat.Name = "SurahRepeat";
            this.SurahRepeat.Size = new System.Drawing.Size(55, 28);
            this.SurahRepeat.TabIndex = 5;
            this.SurahRepeat.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // time5
            // 
            this.time5.AutoSize = true;
            this.time5.Font = new System.Drawing.Font("Tahoma", 13F);
            this.time5.Location = new System.Drawing.Point(15, 298);
            this.time5.Name = "time5";
            this.time5.Size = new System.Drawing.Size(117, 22);
            this.time5.TabIndex = 29;
            this.time5.Text = "00:00:00.000";
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.AutoScroll = true;
            this.guna2Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.guna2Panel1.BorderColor = System.Drawing.Color.Transparent;
            this.guna2Panel1.Location = new System.Drawing.Point(761, 46);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(255, 595);
            this.guna2Panel1.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label6.Location = new System.Drawing.Point(135, 335);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 22);
            this.label6.TabIndex = 31;
            this.label6.Text = "تلوين الآية";
            // 
            // SurahRepeatCheck
            // 
            this.SurahRepeatCheck.AutoSize = true;
            this.SurahRepeatCheck.Font = new System.Drawing.Font("Tahoma", 13F);
            this.SurahRepeatCheck.Location = new System.Drawing.Point(81, 380);
            this.SurahRepeatCheck.Name = "SurahRepeatCheck";
            this.SurahRepeatCheck.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SurahRepeatCheck.Size = new System.Drawing.Size(120, 26);
            this.SurahRepeatCheck.TabIndex = 3;
            this.SurahRepeatCheck.Text = "تكرار السورة";
            this.SurahRepeatCheck.UseVisualStyleBackColor = true;
            this.SurahRepeatCheck.CheckedChanged += new System.EventHandler(this.Repeat_CheckedChanged);
            // 
            // pause
            // 
            this.pause.Location = new System.Drawing.Point(184, 297);
            this.pause.Name = "pause";
            this.pause.Size = new System.Drawing.Size(45, 23);
            this.pause.TabIndex = 2;
            this.pause.Text = "||";
            this.pause.UseVisualStyleBackColor = true;
            this.pause.Click += new System.EventHandler(this.Pause_Click);
            // 
            // Rate
            // 
            this.Rate.DecimalPlaces = 3;
            this.Rate.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Rate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.Rate.Location = new System.Drawing.Point(34, 468);
            this.Rate.Maximum = new decimal(new int[] {
            14,
            0,
            0,
            65536});
            this.Rate.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            65536});
            this.Rate.Name = "Rate";
            this.Rate.Size = new System.Drawing.Size(95, 28);
            this.Rate.TabIndex = 6;
            this.Rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Rate.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Rate.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Rate.ValueChanged += new System.EventHandler(this.Rate_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label7.Location = new System.Drawing.Point(135, 470);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 22);
            this.label7.TabIndex = 36;
            this.label7.Text = "السرعة";
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 15;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.8D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // guna2HtmlLabel1
            // 
            this.guna2HtmlLabel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2HtmlLabel1.Font = new System.Drawing.Font("Times New Roman", 25F, System.Drawing.FontStyle.Bold);
            this.guna2HtmlLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.guna2HtmlLabel1.IsSelectionEnabled = false;
            this.guna2HtmlLabel1.Location = new System.Drawing.Point(12, 859);
            this.guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            this.guna2HtmlLabel1.Size = new System.Drawing.Size(217, 39);
            this.guna2HtmlLabel1.TabIndex = 37;
            this.guna2HtmlLabel1.Text = "<span style=\"color: red\">Q</span>uran <span style=\"color: blue\">K</span>areem";
            // 
            // exitForm
            // 
            this.exitForm.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.exitForm.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.exitForm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitForm.Font = new System.Drawing.Font("Tahoma", 12F);
            this.exitForm.Location = new System.Drawing.Point(980, 12);
            this.exitForm.Name = "exitForm";
            this.exitForm.Size = new System.Drawing.Size(36, 28);
            this.exitForm.TabIndex = 38;
            this.exitForm.Text = "x";
            this.exitForm.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exitForm.UseVisualStyleBackColor = false;
            this.exitForm.Click += new System.EventHandler(this.exitForm_Click);
            // 
            // minimize
            // 
            this.minimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.minimize.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.minimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize.Font = new System.Drawing.Font("Tahoma", 12F);
            this.minimize.Location = new System.Drawing.Point(935, 22);
            this.minimize.Name = "minimize";
            this.minimize.Size = new System.Drawing.Size(36, 10);
            this.minimize.TabIndex = 39;
            this.minimize.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.minimize.UseVisualStyleBackColor = false;
            this.minimize.Click += new System.EventHandler(this.minimize_Click);
            // 
            // stop
            // 
            this.stop.Image = global::QuranKareem.Properties.Resources.stop;
            this.stop.Location = new System.Drawing.Point(133, 297);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(45, 23);
            this.stop.TabIndex = 40;
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label8.Location = new System.Drawing.Point(156, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 22);
            this.label8.TabIndex = 42;
            this.label8.Text = "الجزء";
            // 
            // Juz
            // 
            this.Juz.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Juz.Location = new System.Drawing.Point(35, 96);
            this.Juz.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.Juz.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Juz.Name = "Juz";
            this.Juz.Size = new System.Drawing.Size(95, 28);
            this.Juz.TabIndex = 10;
            this.Juz.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Juz.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Juz.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Juz.ValueChanged += new System.EventHandler(this.Juz_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label9.Location = new System.Drawing.Point(148, 132);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(54, 22);
            this.label9.TabIndex = 44;
            this.label9.Text = "الحزب";
            // 
            // Hizb
            // 
            this.Hizb.Font = new System.Drawing.Font("Tahoma", 13F);
            this.Hizb.Location = new System.Drawing.Point(35, 130);
            this.Hizb.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.Hizb.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Hizb.Name = "Hizb";
            this.Hizb.Size = new System.Drawing.Size(95, 28);
            this.Hizb.TabIndex = 10;
            this.Hizb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Hizb.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.Hizb.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Hizb.ValueChanged += new System.EventHandler(this.Hizb_ValueChanged);
            // 
            // quranHtmlText
            // 
            this.quranHtmlText.BackColor = System.Drawing.Color.Transparent;
            this.quranHtmlText.Font = new System.Drawing.Font("KFGQPC HAFS Uthmanic Script", 20F);
            this.quranHtmlText.Location = new System.Drawing.Point(240, 5);
            this.quranHtmlText.Name = "quranHtmlText";
            this.quranHtmlText.Size = new System.Drawing.Size(3, 2);
            this.quranHtmlText.TabIndex = 45;
            this.quranHtmlText.Text = null;
            this.quranHtmlText.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.quranHtmlText.Visible = false;
            // 
            // copy
            // 
            this.copy.Font = new System.Drawing.Font("Tahoma", 12F);
            this.copy.Location = new System.Drawing.Point(893, 656);
            this.copy.Name = "copy";
            this.copy.Size = new System.Drawing.Size(90, 42);
            this.copy.TabIndex = 46;
            this.copy.Text = "نسخ الآية";
            this.copy.UseVisualStyleBackColor = true;
            this.copy.Click += new System.EventHandler(this.copy_Click);
            // 
            // normalText
            // 
            this.normalText.AutoSize = true;
            this.normalText.Font = new System.Drawing.Font("Tahoma", 12F);
            this.normalText.Location = new System.Drawing.Point(796, 667);
            this.normalText.Name = "normalText";
            this.normalText.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.normalText.Size = new System.Drawing.Size(91, 23);
            this.normalText.TabIndex = 47;
            this.normalText.Text = "نص عادي";
            this.normalText.UseVisualStyleBackColor = true;
            // 
            // searchList
            // 
            this.searchList.FormattingEnabled = true;
            this.searchList.Location = new System.Drawing.Point(12, 11);
            this.searchList.Name = "searchList";
            this.searchList.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.searchList.Size = new System.Drawing.Size(222, 511);
            this.searchList.TabIndex = 48;
            this.searchList.Visible = false;
            this.searchList.SelectedIndexChanged += new System.EventHandler(this.searchList_SelectedIndexChanged);
            // 
            // search
            // 
            this.search.Font = new System.Drawing.Font("Tahoma", 12F);
            this.search.Location = new System.Drawing.Point(129, 528);
            this.search.Name = "search";
            this.search.Size = new System.Drawing.Size(90, 32);
            this.search.TabIndex = 49;
            this.search.Text = "بحث";
            this.search.UseVisualStyleBackColor = true;
            this.search.Click += new System.EventHandler(this.search_Click);
            // 
            // searchText
            // 
            this.searchText.Font = new System.Drawing.Font("Tahoma", 12F);
            this.searchText.Location = new System.Drawing.Point(12, 566);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(222, 27);
            this.searchText.TabIndex = 51;
            // 
            // searchClose
            // 
            this.searchClose.Font = new System.Drawing.Font("Tahoma", 12F);
            this.searchClose.Location = new System.Drawing.Point(33, 528);
            this.searchClose.Name = "searchClose";
            this.searchClose.Size = new System.Drawing.Size(90, 32);
            this.searchClose.TabIndex = 52;
            this.searchClose.Text = "الغاء";
            this.searchClose.UseVisualStyleBackColor = true;
            this.searchClose.Visible = false;
            this.searchClose.Click += new System.EventHandler(this.searchClose_Click);
            // 
            // tafasir
            // 
            this.tafasir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tafasir.Font = new System.Drawing.Font("Tahoma", 13F);
            this.tafasir.FormattingEnabled = true;
            this.tafasir.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.tafasir.Location = new System.Drawing.Point(12, 656);
            this.tafasir.Name = "tafasir";
            this.tafasir.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tafasir.Size = new System.Drawing.Size(222, 29);
            this.tafasir.TabIndex = 53;
            this.tafasir.SelectedIndexChanged += new System.EventHandler(this.tafasir_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 13F);
            this.label10.Location = new System.Drawing.Point(86, 631);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 22);
            this.label10.TabIndex = 54;
            this.label10.Text = "التفاسير";
            // 
            // tafseerCopy
            // 
            this.tafseerCopy.Font = new System.Drawing.Font("Tahoma", 12F);
            this.tafseerCopy.Location = new System.Drawing.Point(144, 691);
            this.tafseerCopy.Name = "tafseerCopy";
            this.tafseerCopy.Size = new System.Drawing.Size(90, 51);
            this.tafseerCopy.TabIndex = 55;
            this.tafseerCopy.Text = "نسخ التفسير";
            this.tafseerCopy.UseVisualStyleBackColor = true;
            this.tafseerCopy.Click += new System.EventHandler(this.tafseerCopy_Click);
            // 
            // saveRTF
            // 
            this.saveRTF.Font = new System.Drawing.Font("Tahoma", 12F);
            this.saveRTF.Location = new System.Drawing.Point(12, 691);
            this.saveRTF.Name = "saveRTF";
            this.saveRTF.Size = new System.Drawing.Size(126, 51);
            this.saveRTF.TabIndex = 56;
            this.saveRTF.Text = "save .rtf";
            this.saveRTF.UseVisualStyleBackColor = true;
            this.saveRTF.Click += new System.EventHandler(this.saveRTF_Click);
            // 
            // saveRichText
            // 
            this.saveRichText.FileName = "tafseer";
            this.saveRichText.Filter = "Rich Text File|*.rtf";
            // 
            // rtb
            // 
            this.rtb.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtb.Location = new System.Drawing.Point(-44, 691);
            this.rtb.Name = "rtb";
            this.rtb.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.rtb.Size = new System.Drawing.Size(50, 51);
            this.rtb.TabIndex = 57;
            this.rtb.Text = "";
            this.rtb.Visible = false;
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(12, 830);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(75, 23);
            this.about.TabIndex = 58;
            this.about.Text = "About";
            this.about.UseVisualStyleBackColor = true;
            this.about.Click += new System.EventHandler(this.about_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 910);
            this.Controls.Add(this.about);
            this.Controls.Add(this.rtb);
            this.Controls.Add(this.saveRTF);
            this.Controls.Add(this.tafseerCopy);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.tafasir);
            this.Controls.Add(this.searchClose);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.search);
            this.Controls.Add(this.searchList);
            this.Controls.Add(this.normalText);
            this.Controls.Add(this.copy);
            this.Controls.Add(this.quranHtmlText);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Hizb);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Juz);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.minimize);
            this.Controls.Add(this.exitForm);
            this.Controls.Add(this.guna2HtmlLabel1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.Rate);
            this.Controls.Add(this.pause);
            this.Controls.Add(this.SurahRepeatCheck);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.guna2Panel1);
            this.Controls.Add(this.time5);
            this.Controls.Add(this.SurahRepeat);
            this.Controls.Add(this.AyahRepeatCheck);
            this.Controls.Add(this.AyahRepeat);
            this.Controls.Add(this.color);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Surahs);
            this.Controls.Add(this.Quarter);
            this.Controls.Add(this.Ayah);
            this.Controls.Add(this.Page);
            this.Controls.Add(this.Surah);
            this.Controls.Add(this.quranPic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quran Kareem";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.quranPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Surah)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Page)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Ayah)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Quarter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AyahRepeat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SurahRepeat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Rate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Juz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Hizb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox quranPic;
        private System.Windows.Forms.NumericUpDown Surah;
        private System.Windows.Forms.NumericUpDown Page;
        private System.Windows.Forms.NumericUpDown Ayah;
        private System.Windows.Forms.NumericUpDown Quarter;
        private System.Windows.Forms.ComboBox Surahs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox color;
        private System.Windows.Forms.FolderBrowserDialog folder;
        private System.Windows.Forms.NumericUpDown AyahRepeat;
        private System.Windows.Forms.CheckBox AyahRepeatCheck;
        private System.Windows.Forms.NumericUpDown SurahRepeat;
        private System.Windows.Forms.Label time5;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox SurahRepeatCheck;
        private System.Windows.Forms.Button pause;
        private System.Windows.Forms.NumericUpDown Rate;
        private System.Windows.Forms.Label label7;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private System.Windows.Forms.Button exitForm;
        private System.Windows.Forms.Button minimize;
        private System.Windows.Forms.Button stop;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown Hizb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown Juz;
        private Guna.UI2.WinForms.Guna2HtmlLabel quranHtmlText;
        private System.Windows.Forms.Button copy;
        private System.Windows.Forms.CheckBox normalText;
        private System.Windows.Forms.ListBox searchList;
        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Button search;
        private System.Windows.Forms.Button searchClose;
        private System.Windows.Forms.Button tafseerCopy;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox tafasir;
        private System.Windows.Forms.Button saveRTF;
        private System.Windows.Forms.SaveFileDialog saveRichText;
        private System.Windows.Forms.RichTextBox rtb;
        private System.Windows.Forms.Button about;
    }
}

