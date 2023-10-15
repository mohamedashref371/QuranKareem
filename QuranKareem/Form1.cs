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
        // إن شاء الله ستكون الخطوة القادمة هي تحسين واجهة البرنامج

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace(Application.ProductVersion, "");

        readonly QuranTexts quranTexts = QuranTexts.Instance;
        readonly QuranPictures quranPictures = QuranPictures.Instance;
        readonly QuranAudios quranAudios = QuranAudios.Instance;
        readonly QuranTafasir quranTafasir = QuranTafasir.Instance;

        bool textMode = false;
        string moshafText = "", moshafPic = "", moshafAudio = "", Tafseer = "";
        string[] stringArray;

        #region Form EventArgs
        FormSize fs;
        private void Form1_Load(object sender, EventArgs e) {
           
            try { rtb.SaveFile(save+"XXX"); /* حل مؤقت لمشكلة ال rtb.SaveFile() */ } catch { }
            color.SelectedIndex = 1; // اللون الأحمر
            string[] textsFiles = null, picturesFolders = null;

            try { textsFiles = Directory.GetFiles("texts"); /*البحث في مجلد المصاحف المكتوبة */ } catch { }

            try { picturesFolders = Directory.GetDirectories("pictures"); /* البحث في مجلد المصاحف المصورة */ } catch { }

            if (picturesFolders != null && picturesFolders.Length > 0)
                quranPictures.QuranPicture(picturesFolders[0], (int)Surah.Value, (int)Ayah.Value);

            // مجلد الصور غير موجود ؟
            else if (textsFiles != null && textsFiles.Length > 0) {
                textMode = true; // التبديل إلى RichTextBox
                quranTexts.AddRichTextBoxInControls(Controls, quranPic.Location.X, quranPic.Location.Y, quranPic.Width, quranPic.Height); // اظهاره في النافذة
                quranTexts.AddEventHandler(PageRichText_Click); // اضافة دالة تُنفذ عند الضغط بالماوس
                quranPic.Visible = false;
            }

            else {
                MessageBox.Show($"مجلدات المصاحف غير موجودة،\nسيتم إغلاق البرنامج.");
                Close();
            }

            if (textsFiles != null && textsFiles.Length > 0)
                quranTexts.QuranText(textsFiles[0], (int)Surah.Value, (int)Ayah.Value);

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
                tafasir.Items.Add("* التفاسير ...");
                foreach (string file in textsFiles) {
                    tafasir.Items.Add(file.Split('\\').Last().Replace(".db",""));
                }
                tafasir.SelectedIndex = 0;
            }
            // اضافة التراجم
            textsFiles = null;
            try { textsFiles = Directory.GetFiles("translations"); } catch { }
            if (textsFiles != null && textsFiles.Length > 0) {
                tafasir.Items.Add("* التراجم ...");
                foreach (string file in textsFiles) {
                    tafasir.Items.Add(file.Split('\\').Last().Replace(".db", ""));
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
                    time5.Text = quranAudios.GetCurrentPosition();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(save + "AyahNumberCurrent", Surah.Value + "," + Ayah.Value);
            File.WriteAllText(save + "MoshafTextCurrent", moshafText);
            File.WriteAllText(save + "MoshafPictureCurrent", moshafPic);
            File.WriteAllText(save + "MoshafAudioCurrent", moshafAudio);
            File.WriteAllText(save + "TafseerCurrent", Tafseer);
            File.WriteAllText(save + "Volume", volume.Value + "");
        }

        private void ExitForm_Click(object sender, EventArgs e) { Close(); }

        private void Minimize_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }
        #endregion

        #region أزرار المشايخ
        // اضافة ازرار المشايخ
        void AddMashaykhButtons() /* ليست أفضل شيئ */ {
            List<string> audiosFolders = new List<string>();
            bool distinct=false;
            if (File.Exists("audios\\favourite.txt") && File.ReadAllText("audios\\favourite.txt").Trim() != "") {
                audiosFolders.AddRange(File.ReadAllText("audios\\favourite.txt", Encoding.UTF8).Replace(Environment.NewLine, "*").Split('*').ToList());
                for (int i=0; i< audiosFolders.Count; i++) {
                    stringArray = audiosFolders[i].Split('|');
                    if (stringArray[0].Trim() != "" && Directory.Exists("audios\\" + stringArray[0])) audiosFolders[i] = "audios\\" + audiosFolders[i];
                    else if (Directory.Exists(stringArray[0]) || stringArray[0].Trim() == ":line:" || stringArray[0].Trim() == ":space:") { }
                    else if (stringArray[0].Trim() == ":distinct:") { audiosFolders[i] = ""; distinct = true; }
                    else audiosFolders[i]="";
                }
            }
            panel.Controls.Clear();
            try { audiosFolders.AddRange(Directory.GetDirectories("audios").ToList()); } catch { } // البحث في مجلد الصوتيات

            
            if (distinct) {
                string temp,temp2;
                for (int i = 0; i < audiosFolders.Count; i++) {
                    temp = audiosFolders[i].Split('|').First().Trim('\\', '/');
                    try { temp = Path.GetFullPath(temp); } catch { continue; }
                    for (int j = i + 1; j < audiosFolders.Count; j++) {
                        temp2 = audiosFolders[j].Split('|').First().Trim('\\', '/');
                        try { temp2 = Path.GetFullPath(temp2); } catch { continue; }
                        if (temp == temp2) audiosFolders[j] = "";
                    }
                }
            }
            
            Guna2Button b;
            int y = -45;
            Color clr; Random rand = new Random();
            for (int i = 0; i < audiosFolders.Count; i++) {
                if (audiosFolders[i].Trim() == "") continue;
                stringArray = audiosFolders[i].Split('|');
                if (stringArray[0].Trim() == ":space:") { y += 5; continue; }
                y += 50;
                if (stringArray.Length > 1 && stringArray[1].Trim() != "") clr = Color.FromName(stringArray[1].Trim());
                else clr = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 200), rand.Next(0, 256));
                audiosFolders[i] = stringArray[0];
                b = new Guna2Button
                {
                    FillColor = clr,
                    Location = new Point(fs.GetNewX(5), fs.GetNewY(y)),
                    BorderRadius = 15
                };
                if (stringArray != null && stringArray.Length > 2 && stringArray[2].Trim() != "") b.ForeColor = Color.FromName(stringArray[2].Trim());
                if (stringArray != null && stringArray.Length > 3 && stringArray[3].Trim() != "") b.Text = stringArray[3];
                if (audiosFolders[i].Trim() == ":line:") {
                    b.Size = new Size(fs.GetNewX(230), fs.GetNewY(10));
                    y -= 35;
                }
                else {
                    if (b.Text=="") b.Text = audiosFolders[i].Trim('\\', '/').Split('\\').Last();
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
            time5.Text = quranAudios.GetCurrentPosition();
            if (File.Exists(s + "\\download links.txt") && Directory.GetFiles(s).Length == 2) {
                if (MessageBox.Show("هل تريد تحميل المصحف لهذا الشيخ؟ .. سنطلعك بعد الانتهاء", "تحميل", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    Task.Run(() => DownloadFiles(s));
                }
            }
        }

        // تحميل المصاحف المسموعة في الخلفية
        static void DownloadFiles(string s) {
            try {
                string[] links = File.ReadAllText(s + "\\download links.txt").Replace("\n", "|").Split('|');
                string[] temp;
                System.Net.WebClient client = new System.Net.WebClient();
                if (links.Length > 0) {
                    foreach (string link in links) {
                        if (link.Trim().Length > 0) {
                            try { temp = link.Split('/'); } catch { continue; }
                            try {
                                client.DownloadFile(link, s + "\\" + temp.Last());
                            } catch {
                                if (MessageBox.Show($"حدث خطأ في تحميل ملف {temp.Last()} ..\nهل تريد استكمال تحميل الملفات الأخرى؟", "خطأ o_O", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                            }
                        }
                    }
                    MessageBox.Show("انتهى تحميل المصحف" , ":)");
                } else MessageBox.Show("مجلد تحميل المصحف فارغ", ":(");
            } catch { MessageBox.Show("حدث خطأ ما", "-_-"); }
        }
        #endregion

        #region التعامل مع التنقلات في المصحف
        bool allow = true; // أداة للتعامل مع الإستدعاءات التلقائية غير المرغوب فيها

        // إلقاء المسؤولية على Surah_ValueChanged
        private void Surahs_SelectedIndexChanged(object sender, EventArgs e) { Surah.Value = Surahs.SelectedIndex + 1; }

        // فهرس برقم السورة
        private void Surah_ValueChanged(object sender, EventArgs e) {
            Surahs.SelectedIndex = (int)Surah.Value - 1;
            
            if (textMode) {
                if (allow) quranTexts.Surah((int)Surah.Value);
                Ayah.Minimum = quranTexts.AyahStart;
                ayahMaximumIsChanged = true;
                Ayah.Maximum = quranTexts.AyatCount;
                ayahMaximumIsChanged = false;
                SetAyah();
            }
            else {
                if (allow) quranPictures.Surah((int)Surah.Value);
                Ayah.Minimum = quranPictures.AyahStart;
                ayahMaximumIsChanged = true;
                Ayah.Maximum = quranPictures.AyatCount;
                ayahMaximumIsChanged = false;
                SetAyah();
            }
            if (addNewMoqrea.Text == "إلغاء") EditMoqreaSurah((int)Surah.Value);
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

            if (textMode && allow) {
                quranTexts.Quarter((int)Quarter.Value);
                SetAyah();
            }
            else if (allow) {
                quranPictures.Quarter((int)Quarter.Value);
                SetAyah();
            }
        }

        private void Page_ValueChanged(object sender, EventArgs e) /* فهرس بالصفحات */ {

            if (textMode && allow) {
                quranTexts.Page((int)Page.Value);
                SetAyah();
            }
            else if (allow) {
                quranPictures.Page((int)Page.Value);
                SetAyah();
            }
        }

        void SetAyah() {
            allow = false;
            if (textMode) {
                Surah.Value = quranTexts.SurahNumber;
                if (Ayah.Value == quranTexts.AyahNumber) Ayah_ValueChanged(null, null);
                else Ayah.Value = quranTexts.AyahNumber;
            }
            else {
                Surah.Value = quranPictures.SurahNumber;
                if (Ayah.Value == quranPictures.AyahNumber) Ayah_ValueChanged(null, null);
                else Ayah.Value = quranPictures.AyahNumber;
            }
            allow = true;
        }

        // فهرس بالآية داخل السورة
        bool ayahMaximumIsChanged = false;
        private void Ayah_ValueChanged(object sender, EventArgs e) {
            if (ayahMaximumIsChanged) return;
            if (textMode) {
                if (allow) {
                    quranTexts.Ayah((int)Surah.Value, (int)Ayah.Value);
                    allow = false;
                }
                Quarter.Value = quranTexts.QuarterNumber;
                Page.Value = quranTexts.PageNumber;
            }
            else {
                if (allow) {
                    quranPictures.Ayah((int)Surah.Value, (int)Ayah.Value);
                    allow = false;
                }
                Quarter.Value = quranPictures.QuarterNumber;
                Page.Value = quranPictures.PageNumber;
                quranPic.BackgroundImage = quranPictures.Picture;
            }

            if (isAllow) quranAudios.Ayah(quranPictures.SurahNumber, quranPictures.AyahNumber);
            time5.Text = quranAudios.GetCurrentPosition();
            allow = true;
        }

        // quranAudios -> AddEventHandler
        bool isAllow=true;
        private void Audio_(object sender, EventArgs e) {
            isAllow = false;
            if (textMode && allow) {
                quranTexts.Ayah(quranAudios.SurahNumber, quranAudios.AyahNumber);
                SetAyah();
            }
            else if (allow) {
                quranPictures.Ayah(quranAudios.SurahNumber, quranAudios.AyahNumber);
                SetAyah();
            }
            isAllow = true;
        }

        private void QuranPic_Click(object sender, EventArgs e) {
            if (allow) {
                quranPictures.SetXY( MousePosition.X - quranPic.Location.X - Location.X,
                                     MousePosition.Y - quranPic.Location.Y - Location.Y,
                                     quranPic.Width, quranPic.Height);
                SetAyah();
            }
        }

        // بديل عن المصحف المصور، ربما لن تراه في حياتك
        // quranTexts -> AddEventHandler
        private void PageRichText_Click(object sender, EventArgs e) {
            if (allow) {
                quranTexts.SetCursor();
                SetAyah();
            }
        }

        private void Color_SelectedIndexChanged(object sender, EventArgs e) {
            if (color.SelectedIndex == 1) { quranPictures.ayahColor = AyahColor.red; quranTexts.ayahColor = AyahColor.red; }
            else if (color.SelectedIndex == 2) { quranPictures.ayahColor = AyahColor.green; quranTexts.ayahColor = AyahColor.green; }
            else if (color.SelectedIndex == 3) { quranPictures.ayahColor = AyahColor.blue; quranTexts.ayahColor = AyahColor.blue; }
            else if (color.SelectedIndex == 4) { quranPictures.ayahColor = AyahColor.darkCyan; quranTexts.ayahColor = AyahColor.darkCyan; }
            else if (color.SelectedIndex == 5) { quranPictures.ayahColor = AyahColor.darkRed; quranTexts.ayahColor = AyahColor.darkRed; }
            else { quranPictures.ayahColor = AyahColor.nothing; quranTexts.ayahColor = AyahColor.nothing; }
        }
        #endregion

        #region نسخ القرآن وتفسيره والبحث فيه
        // نسخ الآية
        private void Copy_Click(object sender, EventArgs e) {
            try {
                if (normalText.Checked) Clipboard.SetText(quranTexts.AyahAbstractText((int)Surah.Value, (int)Ayah.Value));
                else Clipboard.SetText(quranTexts.AyahText((int)Surah.Value, (int)Ayah.Value));
            } catch { }
        }

        // نسخ تفسير الآية
        private void TafseerCopy_Click(object sender, EventArgs e) {
            try {
                Clipboard.SetText(quranTafasir.AyahTafseerText((int)Surah.Value, (int)Ayah.Value)); // كود النسخ
            } catch { }
        }

        // إختيار كتاب تفسير
        private void Tafasir_SelectedIndexChanged(object sender, EventArgs e) {
            Tafseer = tafasir.Text;
            if (!Tafseer.Contains("*"))
                if (File.Exists(@"tafasir\" + Tafseer + ".db")) quranTafasir.QuranTafseer(@"tafasir\" + Tafseer + ".db");
                else quranTafasir.QuranTafseer(@"translations\" + Tafseer + ".db");
        }

        // إنشاء تنسيق نص منسق لتفسير الآية 
        private void SaveRTF_Click(object sender, EventArgs e) {
            saveRichText.FileName = $"Tafseer Surah {Surah.Value} Ayah {Ayah.Value}";
            rtb.Text = quranTafasir.AyahTafseerText((int)Surah.Value, (int)Ayah.Value);
            if (rtb.Text!="" && saveRichText.ShowDialog() == DialogResult.OK)
                rtb.SaveFile(saveRichText.FileName);
        }

        // البحث بالكلمات في القرآن
        private void Search_Click(object sender, EventArgs e) {
            searchList.Items.Clear();
            searchList.Items.AddRange(quranTexts.Search(searchText.Text));
            searchList.Visible = true;
            searchClose.Visible = true;
        }

        // إختيار آية من قائمة البحث
        private void SearchList_SelectedIndexChanged(object sender, EventArgs e) {
            int[] sura_aya = quranTexts.SelectedSearchIndex(searchList.SelectedIndex);
            if (sura_aya == null) return;
            Surah.Value = sura_aya[0];
            Ayah.Value = sura_aya[1]>= Ayah.Minimum ? sura_aya[1] : Ayah.Minimum;
        }

        // إغلاق قائمة البحث
        private void SearchClose_Click(object sender, EventArgs e) {
            searchList.Visible = false;
            searchClose.Visible = false;
        }

        // إنشاء ملف ترجمة
        private void SrtFile_Click(object sender, EventArgs e) {
            saveSRTFile.FileName = $"Surah {Surah.Value.ToString().PadLeft(3, '0')}.srt";
            if (saveSRTFile.ShowDialog() == DialogResult.OK) {
                File.WriteAllText(saveSRTFile.FileName, quranTafasir.SubRipText((int)Surah.Value, quranAudios.GetPositionsOf((int)Surah.Value)), Encoding.UTF8);
            }
        }
        #endregion

        #region التعامل مع الصوت
        // تفعيل التكرار
        private void Repeat_CheckedChanged(object sender, EventArgs e) {
            quranAudios.Repeat(SurahRepeatCheck.Checked ? (int)SurahRepeat.Value : 1, AyahRepeatCheck.Checked ? (int)AyahRepeat.Value : 1);
        }

        private void Pause_Click(object sender, EventArgs e) { quranAudios.Pause(); } // إيقاف مؤقت للصوت
        private void Stop_Click(object sender, EventArgs e) { quranAudios.Stop(); } // إيقاف دائم للصوت

        // سرعة الصوت
        private void Rate_ValueChanged(object sender, EventArgs e) { quranAudios.Rate((double)Rate.Value); }

        // مستوى إرتفاع الصوت
        private void Volume_ValueChanged(object sender, EventArgs e) { quranAudios.Volume(volume.Value); }

        // حفظ الآية
        private void Splitter_Click(object sender, EventArgs e) { quranAudios.SurahSplitter(); }

        // تقطيع السورة لآيات
        private void SplitAll_Click(object sender, EventArgs e) {
            for (int i = 1; i <= quranAudios.AyatCount; i++) {
                quranAudios.Ayah(i);
                quranAudios.SurahSplitter();
            }
            Ayah_ValueChanged(null, null);
        }
        #endregion

        #region إضافة شيخ جديد
        // إضافة أدوات إضافة الشيخ الجديد
        void EditMoqreaSurah(int sura) {
            panel.Controls.Clear();
            int[] ayat = quranAudios.GetTimestamps(sura);
            string[] ayatS = quranTexts.SurahAbstractTexts(sura, 5, endAyatCheck.Checked);
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
                    Tag = endAyatCheck.Checked ? k : Ayah.Minimum == 0 ? i : i+1
                };
                if (endAyatCheck.Checked) {
                    if (i != 0) label.Text = ayatS[i];
                    else label.Text = "الإستعاذة";
                }
                else {
                    if (i != ayat.Length - 1) label.Text = ayatS[i + 1];
                    else label.Text = "نهاية آخر آية";
                }
                if (endAyatCheck.Checked && i != 0 || !endAyatCheck.Checked && i != ayat.Length - 1) label.Click += Label_Click;
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
                    Minimum = -99999,
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

        void Label_Click(object sender, EventArgs e) { Ayah.Value = (int)((Label)sender).Tag; }

        int tempInt;
        void Timestamp_ValueChanged(object sender, EventArgs e) {
            tempInt = (int)((NumericUpDown)sender).Tag;
            quranAudios.Ayah((int)Surah.Value, tempInt, (int)(((NumericUpDown)sender).Value*1000));
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

        // زر تشغيل وضعية إضافة مقرئ جديد
        private void AddNewMoqrea_Click(object sender, EventArgs e) {
            if (addNewMoqrea.Text != "إلغاء" && folder.ShowDialog()== DialogResult.OK && quranAudios.NewQuranAudio(folder.SelectedPath)) {
                addNewMoqrea.Text = "إلغاء"; stop.Enabled = false;
                moshafAudio = folder.SelectedPath;
                splitter.Visible = false; splitAll.Visible = false;
                timestampChangeEventCheck.Visible = true;
                endAyatCheck.Visible = true;
                ShaykhDesc.Enabled = true; addShaykhInfo.Enabled = true;
                EditMoqreaSurah((int)Surah.Value);
            }
            else {
                AddMashaykhButtons();
                addNewMoqrea.Text = "إضافة شيخ جديد"; stop.Enabled = true;
                timestampChangeEventCheck.Visible = false;
                endAyatCheck.Visible = false;
                ShaykhDesc.Enabled = false; addShaykhInfo.Enabled = false;
                splitter.Visible = true; splitAll.Visible = true;
            }
        }

        // عند التفعيل سوف ترى نهاية الآيات بدل بدايتها
        private void EndAyatCheck_CheckedChanged(object sender, EventArgs e) { EditMoqreaSurah((int)Surah.Value); }

        private void AddShaykhInfo_Click(object sender, EventArgs e) {
            if (endAyatCheck.Checked) MessageBox.Show("هذه التوقيتات هي لنهاية الآيات وليس بدايتها");
            else MessageBox.Show($"هذه التوقيتات هي لبداية الآيات إلا آخر توقيت");
        }

        // زر تنظيف الداتابيز من الشوائب
        private void DescSave_Click(object sender, EventArgs e) { quranAudios.SetDescription(extension.Text, comment.Text); }

        // إضافة وصف للشيخ
        private void ShaykhDesc_(object sender, EventArgs e) {
            if (ShaykhDesc.Text == "الوصف" && ShaykhDesc.Enabled==true) {
                ShaykhDesc.Text = "إلغاء";
                lTafseer.Visible = false;
                tafasir.Visible = false;
                tafseerCopy.Visible = false;
                saveRTF.Visible = false;
                srtFile.Visible = false;
                lExt.Visible = true;
                extension.Text = quranAudios.Extension;
                extension.Visible = true;
                lComment.Visible = true;
                comment.Text = quranAudios.Comment;
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
                srtFile.Visible = true;
            }
            
        }
        #endregion


        private void Latest_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://www.mediafire.com/folder/fwzq0xlpp9oys"); }

        private void About_Click(object sender, EventArgs e) { System.Diagnostics.Process.Start("https://facebook.com/Mohamed3713317"); }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e) { }
    }
}
