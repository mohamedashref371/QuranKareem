using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace(Application.ProductVersion, "");

        readonly QuranTexts quranTexts = QuranTexts.Instance;
        readonly QuranPictures quranPictures = QuranPictures.Instance;
        readonly QuranAudios quranAudios = QuranAudios.Instance;
        readonly QuranTafasir quranTafasir = QuranTafasir.Instance;

        string moshafText = ""; bool textMode = false;
        string moshafPic = "";
        string moshafAudio = "";
        string Tafseer = "";

        readonly static string newLine = @"
";

        FormSize fs;
        string[] stringArray;

        private void Form1_Load(object sender, EventArgs e) {
            
            try { rtb.SaveFile(save+"XXX"); /* حل مؤقت لمشكلة ال rtb.SaveFile() */ } catch { }
            color.SelectedIndex = 1; // اللون الأحمر
            string[] textsFiles = null, picturesFolders = null;

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
                if (File.Exists(save + "AyahNumberCurrent")) {
                    stringArray = File.ReadAllText(save + "AyahNumberCurrent").Split(',');
                    Surah.Value = Convert.ToInt32(stringArray[0]);
                    Ayah.Value = Convert.ToInt32(stringArray[1]);
                }
            } catch { }

            try {
                if (File.Exists(save + "MoshafAudioCurrent")) {
                    moshafAudio = File.ReadAllText(save + "MoshafAudioCurrent");
                    quranAudios.QuranAudio(moshafAudio, (int)Surah.Value, (int)Ayah.Value);
                    NewDateTime();
                    quranAudios.Pause();
                }
            } catch { }
            
            try {
                if (File.Exists(save + "TafseerCurrent")){
                    Tafseer = File.ReadAllText(save + "TafseerCurrent");
                    tafasir.SelectedItem = Tafseer;
                }
            } catch { }

            try { if (File.Exists(save + "Volume")) { volume.Value = Convert.ToInt32(File.ReadAllText(save + "Volume")); } } catch { }

            // FormWindowState -> Maximized
            fs = new FormSize(SizeX, SizeY, Size.Width, Size.Height);
            fs.SetControls(Controls);
            AddMashaykhButtons();
        }

        // اضافة ازرار المشايخ
        void AddMashaykhButtons() {
            List<string> audiosFolders = new List<string>();
            
            if (File.Exists("audios\\favourite.txt") && File.ReadAllText("audios\\favourite.txt").Trim() != "") {
                audiosFolders.AddRange(File.ReadAllText("audios\\favourite.txt").Replace(newLine,"*").Split('*').ToList());
                for (int i=0; i< audiosFolders.Count; i++) {
                    stringArray = audiosFolders[i].Split('|');
                    if (Directory.Exists("audios\\" + stringArray[0])) audiosFolders[i] = "audios\\" + audiosFolders[i];
                    else if (Directory.Exists(stringArray[0]) || stringArray[0] == ":line:") { }
                    else audiosFolders[i]="";
                }
            }

            panel.Controls.Clear();
            try { audiosFolders.AddRange(Directory.GetDirectories("audios").ToList()); } catch { } // البحث في مجلد الصوتيات
            Guna2Button b;
            int y = -45;
            Color clr; Random rand = new Random();
            for (int i = 0; i < audiosFolders.Count; i++) {
                if (audiosFolders[i].Trim() == "" || audiosFolders[i].Trim() == "audios\\") continue;
                stringArray = null;
                y += 50;
                if (audiosFolders[i].Contains('|')) {
                    stringArray = audiosFolders[i].Split('|');
                    clr = Color.FromName(stringArray[1]);
                    audiosFolders[i] = stringArray[0];
                }
                else clr = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 200), rand.Next(0, 256));

                b = new Guna2Button
                {
                    FillColor = clr,
                    Location = new Point(fs.GetNewX(5), fs.GetNewY(y)),
                    BorderRadius = 15
                };
                if (stringArray!=null && stringArray.Length > 2) b.ForeColor = Color.FromName(stringArray[2]);

                if (audiosFolders[i]== ":line:") {
                    b.Size = new Size(fs.GetNewX(230), fs.GetNewY(10));
                    y -= 30;
                }
                else {
                    stringArray = audiosFolders[i].Split('\\');
                    b.Text = stringArray[stringArray.Length - 1];
                    b.Font = new Font("Segoe UI", fs.GetNewX(12));
                    b.Size = new Size(fs.GetNewX(230), fs.GetNewY(45));
                    b.Cursor = Cursors.Hand;
                    b.Tag = audiosFolders[i];
                    b.Click += new EventHandler(Button_Click); // اضافة تنبيه عند الضغط على الزر
                }
                panel.Controls.Add(b);
            }
        }
        
        // دالة عامة لجميع الأزرار عند الضغط عليها
        private void Button_Click(object sender /* الزر الذي ضغطت عليه */, EventArgs e) {
            string s = (string)((Guna2Button)sender).Tag;
            moshafAudio = s;
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
            quranTexts.SetCursor();
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
            quranPictures.SetXY(x, y, quranPic.Width, quranPic.Height);
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

        private void Copy_Click(object sender, EventArgs e) {
            try {
                if (normalText.Checked) Clipboard.SetText(quranTexts.AyahAbstractText((int)Surah.Value, (int)Ayah.Value));
                else Clipboard.SetText(quranTexts.AyahText((int)Surah.Value, (int)Ayah.Value));
            } catch { }
        }

        private void TafseerCopy_Click(object sender, EventArgs e) {
            try {
                Clipboard.SetText(quranTafasir.AyahTafseerText((int)Surah.Value, (int)Ayah.Value)); // كود النسخ
            } catch { }
        }

        private void Tafasir_SelectedIndexChanged(object sender, EventArgs e) {
            Tafseer = tafasir.Text;
            quranTafasir.QuranTafseer(@"tafasir\" + Tafseer + ".db");
        }

        private void SaveRTF_Click(object sender, EventArgs e) {
            saveRichText.FileName = $"Tafseer Surah {Surah.Value} Ayah {Ayah.Value}";
            rtb.Text = quranTafasir.AyahTafseerText((int)Surah.Value, (int)Ayah.Value);
            if (rtb.Text!="" && saveRichText.ShowDialog() == DialogResult.OK)
                rtb.SaveFile(saveRichText.FileName);
        }

        private void Search_Click(object sender, EventArgs e) {
            searchList.Items.Clear();
            searchList.Items.AddRange(quranTexts.Search(searchText.Text));
            searchList.Visible = true;
            searchClose.Visible = true;
        }

        private void SearchList_SelectedIndexChanged(object sender, EventArgs e) {
            int[] sura_aya = quranTexts.SelectedSearchIndex(searchList.SelectedIndex);
            if (sura_aya == null) return;
            Surah.Value = sura_aya[0];
            Ayah.Value = sura_aya[1]>= Ayah.Minimum ? sura_aya[1] : Ayah.Minimum;
        }

        private void SearchClose_Click(object sender, EventArgs e) {
            searchList.Visible = false;
            searchClose.Visible = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(save + "AyahNumberCurrent", Surah.Value + "," + Ayah.Value);
            File.WriteAllText(save + "MoshafTextCurrent", moshafText);
            File.WriteAllText(save + "MoshafPictureCurrent", moshafPic);
            File.WriteAllText(save + "MoshafAudioCurrent", moshafAudio);
            File.WriteAllText(save + "TafseerCurrent", Tafseer);
            File.WriteAllText(save + "Volume", volume.Value + "");
        }

        private void Pause_Click(object sender, EventArgs e) { quranAudios.Pause(); }
        private void Stop_Click(object sender, EventArgs e) { quranAudios.Stop(); }

        private void Rate_ValueChanged(object sender, EventArgs e) { quranAudios.Rate((double)Rate.Value); }

        private void Volume_ValueChanged(object sender, EventArgs e) { quranAudios.Volume(volume.Value); }

        private void ExitForm_Click(object sender, EventArgs e) { Close(); }

        private void Minimize_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }

        private void Latest_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://www.mediafire.com/folder/fwzq0xlpp9oys"); }

        private void About_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://facebook.com/Mohamed3713317"); }



        void EditMoqreaSurah(int sura) {
            panel.Controls.Clear();
            int[] ayat = quranAudios.GetTimestamps(sura);
            string[] ayatS = quranTexts.SurahAbstractTexts(sura, 5, true);
            if (ayat == null) return;

            Label label; NumericUpDown num; Button btn;
            Font font = new Font("Tahoma", fs.GetNewX(13));
            Size labelSize = new Size(fs.GetNewX(257), fs.GetNewY(33));
            Size numSize = new Size(fs.GetNewX(120), fs.GetNewY(24));
            Size btnSize = new Size(fs.GetNewX(31), fs.GetNewY(23));
            Color clr = Color.FromArgb(255, 204, 172);

            int y = 28;
            int k;
            for (int i = 0; i < ayat.Length; i++) {
                if (Ayah.Minimum == 0) k = i - 1;
                else if (i == 0) k = -1;
                else k = i;

                label = new Label
                {
                    Font = font,
                    Location = new Point(fs.GetNewX(3), fs.GetNewY(y)),
                    RightToLeft = RightToLeft.Yes,
                    Size = labelSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = k
                };
                if (i != 0) label.Text = ayatS[i];
                else label.Text = "الإستعاذة";
                label.Click += Label_Click;
                panel.Controls.Add(label);

                num = new NumericUpDown
                {
                    BackColor = clr,
                    BorderStyle = BorderStyle.None,
                    DecimalPlaces = 3,
                    Font = font,
                    Location = new Point(fs.GetNewX(35), fs.GetNewY(y + 37)),
                    Size = numSize,
                    Tag = k,
                    TextAlign = HorizontalAlignment.Center,
                    Maximum = 99999,
                    Increment = 0.1M,
                    Value = ayat[i] / 1000M
                };
                num.ValueChanged+= Timestamp_ValueChanged;
                panel.Controls.Add(num);

                btn = new Button
                {
                    FlatStyle = FlatStyle.Flat,
                    Size = btnSize,
                    Tag = i * 3 + 1,
                    Location = new Point(fs.GetNewX(165), fs.GetNewY(y + 37)),
                    ForeColor = Color.Red,
                    Text = "O"
                };
                btn.Click += Timestamp_record;
                panel.Controls.Add(btn);

                y += 86;
            }

            btn = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Size = btnSize,
                Tag = ayat.Length * 3 - 2,
                Location = new Point(fs.GetNewX(200), fs.GetNewY(y - 49)),
                ForeColor = Color.Blue,
                Text = "O"
            };
            btn.Click += Mp3Duration_record;
            panel.Controls.Add(btn);
        }

        void Label_Click(object sender, EventArgs e){
            if ((int)((Label)sender).Tag >= 0) Ayah.Value = (int)((Label)sender).Tag;
            else Ayah.Value = 0;
        }

        int tempInt;
        void Timestamp_ValueChanged(object sender, EventArgs e) {
            tempInt = (int)((NumericUpDown)sender).Tag;
            quranAudios.ayah((int)Surah.Value, tempInt, (int)(((NumericUpDown)sender).Value*1000));
            if (tempInt != Ayah.Maximum && timestampChangeEventCheck.Checked)
                if (Ayah.Value != tempInt + 1) Ayah.Value = tempInt + 1;
                else Ayah_ValueChanged(sender, e);
        }

        double temp;
        void Timestamp_record(object sender, EventArgs e) {
            temp = quranAudios.Mp3CurrentPosition();
            if (temp == 0) return;
            ((NumericUpDown) panel.Controls[(int)((Button)sender).Tag]).Value = (decimal)temp;
        }

        void Mp3Duration_record(object sender, EventArgs e){
            temp = quranAudios.Mp3Duration();
            if (temp == 0) return;
            ((NumericUpDown)panel.Controls[(int)((Button)sender).Tag]).Value = (decimal)temp;
        }

        private void AddNewMoqrea_Click(object sender, EventArgs e) {
            if (addNewMoqrea.Text != "إلغاء" && folder.ShowDialog()== DialogResult.OK && quranAudios.NewQuranAudio(folder.SelectedPath)) {
                addNewMoqrea.Text = "إلغاء"; stop.Enabled = false;
                moshafAudio = folder.SelectedPath;
                timestampChangeEventCheck.Visible = true;
                ShaykhDesc.Enabled = true; addShaykhInfo.Enabled = true;
                EditMoqreaSurah((int)Surah.Value);
            }
            else {
                AddMashaykhButtons();
                addNewMoqrea.Text = "إضافة شيخ جديد"; stop.Enabled = true;
                timestampChangeEventCheck.Visible = false;
                ShaykhDesc.Enabled = false; addShaykhInfo.Enabled = false;
            }
        }

        private void AddShaykhInfo_Click(object sender, EventArgs e) { MessageBox.Show("هذه التوقيتات هي لنهاية الآيات وليس بدايتها"); }

        private void DescSave_Click(object sender, EventArgs e) { quranAudios.SetDescription(extension.Text, comment.Text); }

        private void ShaykhDesc_(object sender, EventArgs e) {
            if (ShaykhDesc.Text == "الوصف" && ShaykhDesc.Enabled==true) {
                string[] desc = quranAudios.GetDescription();
                if (desc == null) return;
                ShaykhDesc.Text = "إلغاء";
                lTafseer.Visible = false;
                tafasir.Visible = false;
                tafseerCopy.Visible = false;
                saveRTF.Visible = false;
                lExt.Visible = true;
                extension.Text = desc[0];
                extension.Visible = true;
                lComment.Visible = true;
                comment.Text = desc[1];
                comment.Visible = true;
                descSave.Visible = true;
            }
            else if (ShaykhDesc.Text == "إلغاء") {
                ShaykhDesc.Text = "الوصف";
                lExt.Visible = false;
                extension.Visible = false;
                lComment.Visible = false;
                comment.Visible = false;
                descSave.Visible = false;
                lTafseer.Visible = true;
                tafasir.Visible = true;
                tafseerCopy.Visible = true;
                saveRTF.Visible = true;
            }
            
        }

    }
}
