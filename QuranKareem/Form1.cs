﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QuranKareem {
    public partial class Form1 : Form {

        public Form1() { InitializeComponent(); }

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace("1.0.0.0", "");

        readonly QuranTexts quranTexts = QuranTexts.Instance;
        readonly QuranPictures quranPictures = QuranPictures.Instance;
        readonly QuranAudios quranAudios = QuranAudios.Instance;
        readonly QuranTafasir quranTafasir = QuranTafasir.Instance;

        string moshafText = "-1";
        string moshafPic = "0";
        string moshafAudio = "-1";
        string Tafseer = "-1";

        bool textMode = false;

        static string newLine = @"
";

        private void Form1_Load(object sender, EventArgs e) {
            color.SelectedIndex = 1;
            string[] stringArray, textsFiles = null, picturesFolders = null, audiosFolders = null;

            try { textsFiles = Directory.GetFiles("texts"); /*البحث في مجلد المصاحف المكتوبة */ } catch { }

            if (textsFiles != null && textsFiles.Length > 0)
                quranTexts.QuranText(textsFiles[0], (int)Surah.Value, (int)Ayah.Value);

            try { picturesFolders = Directory.GetDirectories("pictures"); /* البحث في مجلد المصاحف المصورة */ } catch { }

            if (picturesFolders != null && picturesFolders.Length > 0)
                quranPictures.QuranPicture(picturesFolders[0], (int)Surah.Value, (int)Ayah.Value);

            else if (textsFiles != null && textsFiles.Length > 0) {
                textMode = true;
                quranTexts.AddRichTextBoxInControls(Controls, 240, 5, 510, 900);
                quranTexts.AddEventHandler(new EventHandler(PageRichText_Click));
                quranPic.Visible = false;
            }
            else Close();

            if (!textMode) quranPic.BackgroundImage = quranPictures.Picture; // اظهار الصورة التي سيعطيها لك

            Surahs.Items.Clear(); // إفراغ القائمة من ال ComboBox
            if (textMode) Surahs.Items.AddRange(quranTexts.GetSurahNames()); // ملأها بأسماء السور;
            else Surahs.Items.AddRange(quranPictures.GetSurahNames());
            Surahs.SelectedIndex = (int)Surah.Value - 1; // الإشارة على أول سورة

            quranAudios.AddInControls(Controls);
            quranAudios.AddEventHandler(Audio_);

            try { audiosFolders = Directory.GetDirectories("audios"); } catch { }

            Guna2Button b;
            int y = 5;
            Color clr; Random rand = new Random();
            if (audiosFolders != null)
                for (int i = 0; i < audiosFolders.Length; i++) {
                    clr = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 200), rand.Next(0, 256));
                    b = new Guna2Button();
                    stringArray = audiosFolders[i].Split('\\');
                    b.FillColor = clr;
                    b.Text = stringArray[stringArray.Length - 1];
                    b.Font = new Font("Segoe UI", 12F);
                    b.Location = new Point(5, y);
                    b.Size = new Size(230, 45);
                    b.Cursor = Cursors.Hand;
                    b.Tag = i;
                    b.Click += new EventHandler(Button_Click); // event
                    b.BorderRadius = 15;
                    guna2Panel1.Controls.Add(b);
                    y += 50;
                }

            textsFiles = null;
            try { textsFiles = Directory.GetFiles("tafasir"); } catch { }
            tafasir.Items.Clear();
            if (textsFiles != null && textsFiles.Length > 0) {
                foreach (string file in textsFiles) {
                    tafasir.Items.Add(file.Split('\\').Last().Replace(".db",""));
                }
                tafasir.SelectedIndex = 0;
            }

            try {
                if (File.Exists(save + "Surah")) {
                    stringArray = File.ReadAllText(save + "Surah").Split(',');
                    Surah.Value = Convert.ToInt32(stringArray[0]);
                    Ayah.Value = Convert.ToInt32(stringArray[1]);
                    if (stringArray[4] != "-1") {
                        moshafAudio = stringArray[4];
                        quranAudios.QuranAudio(audiosFolders[Convert.ToInt32(stringArray[4])], (int)Surah.Value, (int)Ayah.Value);
                        NewDateTime();
                        quranAudios.Pause();
                    }
                    if (stringArray[5] != "-1") {
                        Tafseer = stringArray[5];
                        tafasir.SelectedIndex = Convert.ToInt32(Tafseer);
                    }
                }
            } catch { }
                
        }

        private void Button_Click(object sender, EventArgs e) {
            moshafAudio = ((Guna2Button)sender).Tag + "";
            string s = @"audios\" + ((Guna2Button)sender).Text;
            quranAudios.QuranAudio(s, (int)Surah.Value, (int)Ayah.Value);
            NewDateTime();
            if (File.Exists(s + "\\download links.txt") && Directory.GetFiles(s).Length == 2) {
                if (MessageBox.Show("هل تريد تحميل المصحف لهذا الشيخ؟ .. سنطلعك بعد الانتهاء", "تحميل", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Task.Run(() => DownloadFiles(s));
                }
            }
        }

        // تحميل المصاحف المسموعة في الخلفية
        static void DownloadFiles(string s) {
            try {
                string[] links = File.ReadAllText(s + "\\download links.txt").Replace(newLine, "|").Split('|');
                string[] temp;
                System.Net.WebClient client = new System.Net.WebClient();
                if (links.Length >= 113) {
                    foreach (string link in links) {
                        if (link.Trim().Length > 0) {
                            try { temp = link.Split('/'); } catch { continue; }
                            try {
                                client.DownloadFile(link, s + "\\" + temp.Last());
                            } catch {
                                if (MessageBox.Show($"حدث خطأ في تحميل ملف {temp.Last()} .. {newLine}هل تريد استكمال تحميل الملفات الأخرى؟", "خطأ", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                            }
                        }
                    }
                    MessageBox.Show("انتهى تحميل المصحف");
                } else MessageBox.Show("حدث خطأ ما");
            } catch { MessageBox.Show("حدث خطأ ما"); }
        }

        // تفعيل التكرار
        private void Repeat_CheckedChanged(object sender, EventArgs e) {
            quranAudios.Repeat(SurahRepeatCheck.Checked ? (int)SurahRepeat.Value : 1, AyahRepeatCheck.Checked ? (int)AyahRepeat.Value : 1);
        }

        bool allow = true; // أداة للتعامل مع الإستدعاءات التلقائية غير المرغوب فيها
        private void Surahs_SelectedIndexChanged(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false; // قفل الباب
            if (textMode) {
                quranTexts.surah(Surahs.SelectedIndex + 1);
                Surah.Value = quranTexts.Surah; // وضع البيانات في ال NumericUpDown
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
            }

            else {
                quranPictures.surah(Surahs.SelectedIndex + 1); // استدعاء الميثود
                Surah.Value = quranPictures.Surah; // وضع البيانات في ال NumericUpDown
                Quarter.Value = quranPictures.Quarter;
                Page.Value = quranPictures.Page;
                Ayah.Minimum = quranPictures.AyahStart;
                Ayah.Maximum = quranPictures.AyatCount;
                Ayah.Value = quranPictures.Ayah;
                quranPic.BackgroundImage = quranPictures.Picture; // اظهار الصورة التي سيعطيها لك
            }

            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah); NewDateTime();
            allow = true; // فتح الباب
        }

        private void Surah_ValueChanged(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false;
            if (textMode) {
                quranTexts.surah((int)Surah.Value);
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
            }

            else {
                quranPictures.surah((int)Surah.Value);
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranPictures.Quarter;
                Page.Value = quranPictures.Page;
                Ayah.Minimum = quranPictures.AyahStart;
                Ayah.Maximum = quranPictures.AyatCount;
                Ayah.Value = quranPictures.Ayah;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah); NewDateTime();
            allow = true;
        }

        bool allowJuz = true;
        private void Juz_ValueChanged(object sender, EventArgs e) /* فهرس بالأجزاء */ {
            if (!allowJuz) { return; }
            Quarter.Value = Juz.Value * 8 - 7;
        }

        private void Hizb_ValueChanged(object sender, EventArgs e) /* فهرس بالحزب */ {
            if (!allowJuz) { return; }
            Quarter.Value = Hizb.Value * 4 - 3;
        }

        private void Quarter_ValueChanged(object sender, EventArgs e) /* فهرس بالربع */ {
            allowJuz = false;
            Juz.Value = Math.Ceiling(Quarter.Value / 8);
            Hizb.Value = Math.Ceiling(Quarter.Value / 4);
            allowJuz = true;
            if (!allow) { return; }
            allow = false;

            if (textMode) {
                quranTexts.quarter((int)Quarter.Value);
                Surah.Value = quranTexts.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
            }

            else {
                quranPictures.quarter((int)Quarter.Value);
                Surah.Value = quranPictures.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Page.Value = quranPictures.Page;
                Ayah.Minimum = quranPictures.AyahStart;
                Ayah.Maximum = quranPictures.AyatCount;
                Ayah.Value = quranPictures.Ayah;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah); NewDateTime();
            allow = true;
        }

        private void Page_ValueChanged(object sender, EventArgs e) /* فهرس بالصفحات */ {
            if (!allow) { return; }
            allow = false;

            if (textMode) {
                quranTexts.page((int)Page.Value);
                Surah.Value = quranTexts.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranTexts.Quarter;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
            }

            else {
                quranPictures.page((int)Page.Value);
                Surah.Value = quranPictures.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranPictures.Quarter;
                Ayah.Minimum = quranPictures.AyahStart;
                Ayah.Maximum = quranPictures.AyatCount;
                Ayah.Value = quranPictures.Ayah;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah); NewDateTime();
            allow = true;
        }

        // فهرس بالآية داخل السورة
        private void Ayah_ValueChanged(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false;
            if (textMode) {
                quranTexts.ayah((int)Surah.Value, (int)Ayah.Value);
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
            }

            else {
                quranPictures.ayah((int)Surah.Value, (int)Ayah.Value);
                Quarter.Value = quranPictures.Quarter;
                Page.Value = quranPictures.Page;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah); NewDateTime();
            allow = true;
        }

        // بديل عن المصحف المصور، ربما لن تراه في حياتك
        private void PageRichText_Click(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false;
            quranTexts.setCursor();
            Surah.Value = quranTexts.Surah;
            Surahs.SelectedIndex = (int)Surah.Value - 1;
            Quarter.Value = quranTexts.Quarter;
            Page.Value = quranTexts.Page;
            Ayah.Minimum = quranTexts.AyahStart;
            Ayah.Maximum = quranTexts.AyatCount;
            Ayah.Value = quranTexts.Ayah;
            quranAudios.ayah(quranTexts.Surah, quranTexts.Ayah);
            NewDateTime();
            allow = true;
        }

        // quranAudios -> AddEventHandler
        private void Audio_(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false;

            if (textMode) {
                quranTexts.ayah(quranAudios.Surah, quranAudios.Ayah);
                Surah.Value = quranTexts.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
            }

            else {
                quranPictures.ayah(quranAudios.Surah, quranAudios.Ayah);
                Surah.Value = quranPictures.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranPictures.Quarter;
                Page.Value = quranPictures.Page;
                Ayah.Minimum = quranPictures.AyahStart;
                Ayah.Maximum = quranPictures.AyatCount;
                Ayah.Value = quranPictures.Ayah;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            NewDateTime();
            allow = true;
        }

        int x, y; // مكان مؤشر الماوس
        private void QuranPic_MouseMove(object sender, MouseEventArgs e) { x = e.X; y = e.Y; } // يتم تحديث مؤشر الماوس باستمرار تحريك الماوس على الصورة
        private void QuranPic_Click(object sender, EventArgs e) {
            if (!allow) { return; }
            allow = false;
            quranPictures.setXY(x, y /*, pB1.Width, pB1.Height*/);
            Surah.Value = quranPictures.Surah;
            Surahs.SelectedIndex = (int)Surah.Value - 1;
            Quarter.Value = quranPictures.Quarter;
            Page.Value = quranPictures.Page;
            Ayah.Minimum = quranPictures.AyahStart;
            Ayah.Maximum = quranPictures.AyatCount;
            Ayah.Value = quranPictures.Ayah;
            quranPic.BackgroundImage = quranPictures.Picture;
            quranAudios.ayah(quranPictures.Surah, quranPictures.Ayah);
            NewDateTime();
            allow = true;
        }

        void NewDateTime() {
            int cp = quranAudios.CurrentPosition;
            int temp = cp / 3600000;
            time5.Text = temp + ":";

            cp -= temp * 3600000;
            temp = cp / 60000;
            time5.Text += temp.ToString().PadLeft(2, '0') + ":";

            cp -= temp * 60000;
            temp = cp / 1000;
            time5.Text += temp.ToString().PadLeft(2, '0') + ".";

            cp -= temp * 1000;
            time5.Text += cp.ToString().PadLeft(3, '0');
        }


        private void Color_SelectedIndexChanged(object sender, EventArgs e) {
            if (color.SelectedIndex == 1) { quranPictures.ayahColor = AyahColor.red; quranTexts.ayahColor = AyahColor.red; }
            else if (color.SelectedIndex == 2) { quranPictures.ayahColor = AyahColor.green; quranTexts.ayahColor = AyahColor.green; }
            else if (color.SelectedIndex == 3) { quranPictures.ayahColor = AyahColor.blue; quranTexts.ayahColor = AyahColor.blue; }
            else if (color.SelectedIndex == 4) { quranPictures.ayahColor = AyahColor.darkCyan; quranTexts.ayahColor = AyahColor.darkCyan; }
            else if (color.SelectedIndex == 5) { quranPictures.ayahColor = AyahColor.darkRed; quranTexts.ayahColor = AyahColor.darkRed; }
            else { quranPictures.ayahColor = AyahColor.nothing; quranTexts.ayahColor = AyahColor.nothing; }
        }

        private void copy_Click(object sender, EventArgs e) {
            try {
                if (normalText.Checked) Clipboard.SetText(quranTexts.ayahAbstractText((int)Surah.Value, (int)Ayah.Value));
                else Clipboard.SetText(quranTexts.ayahText((int)Surah.Value, (int)Ayah.Value));
            } catch { }
        }

        private void tafseerCopy_Click(object sender, EventArgs e) {
            try {
                Clipboard.SetText(quranTafasir.ayahTafseerText((int)Surah.Value, (int)Ayah.Value));
            } catch { }
        }

        private void tafasir_SelectedIndexChanged(object sender, EventArgs e) {
            quranTafasir.QuranTafseer(@"tafasir\" + tafasir.Text + ".db");
            Tafseer = tafasir.SelectedIndex+"";
        }

        private void saveRTF_Click(object sender, EventArgs e) {
            saveRichText.FileName = $"Tafseer Surah {Surah.Value} Ayah {Ayah.Value}";
            rtb.Text = quranTafasir.ayahTafseerText((int)Surah.Value, (int)Ayah.Value);
            if (rtb.Text!="" && saveRichText.ShowDialog() == DialogResult.OK)
                rtb.SaveFile(saveRichText.FileName);
        }

        private void search_Click(object sender, EventArgs e) {
            searchList.Items.Clear();
            searchList.Items.AddRange(quranTexts.Search(searchText.Text));
            searchList.Visible = true;
            searchClose.Visible = true;
        }

        private void searchList_SelectedIndexChanged(object sender, EventArgs e) {
            int[] sura_aya = quranTexts.SelectedSearchIndex(searchList.SelectedIndex);
            Surah.Value = sura_aya[0]; Ayah.Value = sura_aya[1]>= Ayah.Minimum ? sura_aya[1] : Ayah.Minimum;
        }

        private void searchClose_Click(object sender, EventArgs e) {
            searchList.Visible = false;
            searchClose.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(save + "Surah", Surah.Value + "," + Ayah.Value+","+ moshafText + "," + moshafPic + ","+ moshafAudio + "," + Tafseer);
        }

        private void Pause_Click(object sender, EventArgs e) { quranAudios.Pause(); }
        private void Stop_Click(object sender, EventArgs e) { quranAudios.Stop(); }

        private void Rate_ValueChanged(object sender, EventArgs e) { quranAudios.Rate((double)Rate.Value); }

        private void exitForm_Click(object sender, EventArgs e) { Close(); }

        private void minimize_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }

        private void latest_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://www.mediafire.com/folder/fwzq0xlpp9oys"); }

        private void about_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://facebook.com/Mohamed3713317"); }
    }
}
