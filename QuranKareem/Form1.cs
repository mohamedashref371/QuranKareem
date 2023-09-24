using System;
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

        private readonly int SizeX=1100, SizeY=910;

        public Form1() { InitializeComponent(); }

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace("1.0.1.0", "");

        readonly QuranTexts quranTexts = QuranTexts.Instance;
        readonly QuranPictures quranPictures = QuranPictures.Instance;
        readonly QuranAudios quranAudios = QuranAudios.Instance;
        readonly QuranTafasir quranTafasir = QuranTafasir.Instance;

        string moshafText = "-1"; bool textMode = false;
        string moshafPic = "0";
        string moshafAudio = "-1";
        string Tafseer = "-1";

        readonly static string newLine = @"
";

        FormSize fs;
        string[] stringArray;

        private void Form1_Load(object sender, EventArgs e) {
            
            try { rtb.SaveFile(save+"XXX"); /* حل مؤقت لمشكلة ال rtb.SaveFile() */ } catch { }

            color.SelectedIndex = 1; // اللون الأحمر
            string[] textsFiles = null, picturesFolders = null, audiosFolders = null;

            try { textsFiles = Directory.GetFiles("texts"); /*البحث في مجلد المصاحف المكتوبة */ } catch { }

            if (textsFiles != null && textsFiles.Length > 0)
                quranTexts.QuranText(textsFiles[0], (int)Surah.Value, (int)Ayah.Value);

            try { picturesFolders = Directory.GetDirectories("pictures"); /* البحث في مجلد المصاحف المصورة */ } catch { }

            if (picturesFolders != null && picturesFolders.Length > 0)
                quranPictures.QuranPicture(picturesFolders[0], (int)Surah.Value, (int)Ayah.Value);

            // مجلد الصور غير موجود ؟
            else if (textsFiles != null && textsFiles.Length > 0) {
                textMode = true; // التبديل إلى RichTextBox
                quranTexts.AddRichTextBoxInControls(Controls, quranPic.Location.X, quranPic.Location.Y, quranPic.Width, quranPic.Height); // اظهاره في النافذة
                quranTexts.AddEventHandler(new EventHandler(PageRichText_Click)); // اضافة دالة تُنفذ عند الضغط بالماوس
                quranPic.Visible = false;
            }

            else {
                MessageBox.Show($"مجلدات المصاحف غير موجودة،{newLine}سيتم إغلاق البرنامج.");
                Close();
            }

            if (!textMode) quranPic.BackgroundImage = quranPictures.Picture; // اظهار الصورة التي سيعطيها لك

            Surahs.Items.Clear(); // إفراغ قائمة ال ComboBox
            if (textMode) Surahs.Items.AddRange(quranTexts.GetSurahNames()); // ملأها بأسماء السور;
            else Surahs.Items.AddRange(quranPictures.GetSurahNames());
            Surahs.SelectedIndex = (int)Surah.Value - 1; // الإشارة على أول سورة

            quranAudios.AddInControls(Controls); // اضافة المشغل الصوتي إلى خلفية النافذة
            quranAudios.AddEventHandler(Audio_); // اضافة تنبيه لإنتهاء الآية حين يقرأ الشيخ

            try { audiosFolders = Directory.GetDirectories("audios"); } catch { } // البحث في مجلد الصوتيات

            // اضافة التفاسير
            textsFiles = null;
            try { textsFiles = Directory.GetFiles("tafasir"); } catch { }
            tafasir.Items.Clear();
            if (textsFiles != null && textsFiles.Length > 0) {
                foreach (string file in textsFiles) {
                    tafasir.Items.Add(file.Split('\\').Last().Replace(".db",""));
                }
                tafasir.SelectedIndex = 0;
            }

            // الوقوف عند الآية التي كنت فيها قبل اغلاق البرنامج آخر مرة
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

            try { if (File.Exists(save + "Volume")) { volume.Value = Convert.ToInt32(File.ReadAllText(save + "Volume")); } } catch { }

            // FormWindowState -> Maximized
            fs = new FormSize(SizeX, SizeY, Size.Width, Size.Height);
            fs.SetControls(Controls);
            AddMashaykhButtons(audiosFolders);
        }

        // اضافة ازرار المشايخ
        void AddMashaykhButtons(string[] audiosFolders) {
            panel.Controls.Clear();
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
                    b.Font = new Font("Segoe UI", fs.getNewX(12));
                    b.Location = new Point(fs.getNewX(5), fs.getNewY(y));
                    b.Size = new Size(fs.getNewX(230), fs.getNewY(45));
                    b.Cursor = Cursors.Hand;
                    b.Tag = i; // اضافة رقم للشيخ
                    b.Click += new EventHandler(Button_Click); // اضافة تنبيه عند الضغط على الزر
                    b.BorderRadius = 15;
                    panel.Controls.Add(b);
                    y += 50;
                }
        }
        
        // دالة عامة لجميع الأزرار عند الضغط عليها
        private void Button_Click(object sender /* الزر الذي ضغطت عليه */, EventArgs e) {
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
                if (links.Length > 0) {
                    foreach (string link in links) {
                        if (link.Trim().Length > 0) {
                            try { temp = link.Split('/'); } catch { continue; }
                            try {
                                client.DownloadFile(link, s + "\\" + temp.Last());
                            } catch {
                                if (MessageBox.Show($"حدث خطأ في تحميل ملف {temp.Last()} .. {newLine}هل تريد استكمال تحميل الملفات الأخرى؟", "خطأ o_O", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                            }
                        }
                    }
                    MessageBox.Show("انتهى تحميل المصحف" , ":)");
                } else MessageBox.Show("مجلد تحميل المصحف فارغ", ":(");
            } catch { MessageBox.Show("حدث خطأ ما", "-_-"); }
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

        // فهرس برقم السورة
        private void Surah_ValueChanged(object sender, EventArgs e) {
            if (addNewMoqrea.Text == "إلغاء") EditMoqreaSurah((int)Surah.Value);
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
        // quranTexts -> AddEventHandler
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
            quranPictures.setXY(x, y, quranPic.Width, quranPic.Height);
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
                Clipboard.SetText(quranTafasir.ayahTafseerText((int)Surah.Value, (int)Ayah.Value)); // كود النسخ
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
            if (sura_aya == null) return;
            Surah.Value = sura_aya[0];
            Ayah.Value = sura_aya[1]>= Ayah.Minimum ? sura_aya[1] : Ayah.Minimum;
        }

        private void searchClose_Click(object sender, EventArgs e) {
            searchList.Visible = false;
            searchClose.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(save + "Surah", Surah.Value + "," + Ayah.Value+","+ moshafText + "," + moshafPic + ","+ moshafAudio + "," + Tafseer);
            File.WriteAllText(save + "Volume", volume.Value + "");
        }

        private void Pause_Click(object sender, EventArgs e) { quranAudios.Pause(); }
        private void Stop_Click(object sender, EventArgs e) { quranAudios.Stop(); }

        private void Rate_ValueChanged(object sender, EventArgs e) { quranAudios.Rate((double)Rate.Value); }

        private void volume_ValueChanged(object sender, EventArgs e) { quranAudios.Volume(volume.Value); }

        private void exitForm_Click(object sender, EventArgs e) { Close(); }

        private void minimize_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }

        private void latest_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://www.mediafire.com/folder/fwzq0xlpp9oys"); }

        private void about_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://facebook.com/Mohamed3713317"); }



        void EditMoqreaSurah(int sura) {
            panel.Controls.Clear();
            int[] ayat = quranAudios.getTimestamps(sura);
            string[] ayatS = quranTexts.surahAbstractTexts(sura, 40);
            if (ayat == null) return;

            Label label; NumericUpDown num; Button btn;
            Font font = new Font("Tahoma", fs.getNewX(13));
            Size labelSize = new Size(fs.getNewX(257), fs.getNewY(33));
            Size numSize = new Size(fs.getNewX(120), fs.getNewY(24));
            Size btnSize = new Size(fs.getNewX(31), fs.getNewY(23));
            Color clr = Color.FromArgb(255, 204, 172);

            int y = 28;
            int k;
            for (int i = 0; i < ayat.Length; i++) {
                label = new Label();
                label.Font = font;
                label.Location = new Point(fs.getNewX(3), fs.getNewY(y));
                label.RightToLeft = RightToLeft.Yes;
                label.Size = labelSize;
                label.TextAlign = ContentAlignment.MiddleCenter;
                if (i != 0) label.Text = ayatS[i];
                else label.Text = "الإستعاذة";
                panel.Controls.Add(label);

                num = new NumericUpDown();
                num.BackColor = clr;
                num.BorderStyle = BorderStyle.None;
                num.DecimalPlaces = 3;
                num.Font = font;
                num.Location = new Point(fs.getNewX(35), fs.getNewY(y+37));
                num.Size = numSize;
                if (Ayah.Minimum == 0) k = i - 1;
                else if (i == 0) k = -1;
                else k = i;
                num.Tag = k;
                num.TextAlign = HorizontalAlignment.Center;
                num.Maximum = 99999;
                num.Increment = 0.1M;
                num.Value = ayat[i]/1000M;
                num.ValueChanged+= Timestamp_ValueChanged;
                panel.Controls.Add(num);

                btn = new Button();
                btn.FlatStyle = FlatStyle.Flat;
                btn.Size = btnSize;
                btn.Tag = i*3+1;
                btn.Location = new Point(fs.getNewX(165), fs.getNewY(y + 37));
                btn.ForeColor = Color.Red;
                btn.Text = "O";
                btn.Click += Timestamp_record;
                panel.Controls.Add(btn);

                y += 86;
            }
        }

        void Timestamp_ValueChanged(object sender, EventArgs e) {
            quranAudios.ayah((int)Surah.Value,(int)((NumericUpDown)sender).Tag, (int)(((NumericUpDown)sender).Value*1000));
        }

        void Timestamp_record(object sender, EventArgs e) {
          ((NumericUpDown) panel.Controls[(int)((Button)sender).Tag]).Value = (decimal)quranAudios.Mp3CurrentPosition();
        }

        private void addNewMoqrea_Click(object sender, EventArgs e) {
            if(addNewMoqrea.Text != "إلغاء" && folder.ShowDialog()== DialogResult.OK && quranAudios.NewQuranAudio(folder.SelectedPath)) {
                addNewMoqrea.Text = "إلغاء"; stop.Enabled = false;
                EditMoqreaSurah((int)Surah.Value);
            }
            else {
                string[] audiosFolders=null;
                try { audiosFolders = Directory.GetDirectories("audios"); } catch { } // البحث في مجلد الصوتيات
                AddMashaykhButtons(audiosFolders);
                addNewMoqrea.Text = "إضافة شيخ جديد"; stop.Enabled = true;
            }
        }
    }
}
