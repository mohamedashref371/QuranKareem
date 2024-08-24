using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QuranKareem
{
    public partial class Form1 : Form
    {

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace(Application.ProductVersion, "");

        readonly TextQuran quranText = TextQuran.Instance;
        readonly PictureQuran quranPicture = PictureQuran.Instance;
        readonly AudioQuran quranAudio = AudioQuran.Instance;
        readonly TafseerQuran quranTafseer = TafseerQuran.Instance;
        readonly TrueTypeFontQuran quranTtf = TrueTypeFontQuran.Instance;

        bool textMode = false;
        string moshafText = "", moshafAudio = "", tafseer = "";
        string[] stringArray;

        #region Form
        private readonly int SizeX = 1100, SizeY = 910;

        public Form1() => InitializeComponent();
        bool success = false;
        #endregion

        #region Form EventArgs
        FormSize fs;
        private void Form1_Load(object sender, EventArgs e)
        {

            try { rtb.SaveFile(save + "XXX"); /* حل مؤقت لمشكلة ال rtb.SaveFile() */ } catch { }

            string[] textsFiles = null, picturesFolders = null;

            if (Directory.Exists("texts")) textsFiles = Directory.GetFiles("texts"); /*البحث في مجلد المصاحف المكتوبة */

            if (Directory.Exists("pictures")) picturesFolders = Directory.GetDirectories("pictures"); /* البحث في مجلد المصاحف المصورة */

            if (picturesFolders != null && picturesFolders.Length > 0)
            {
                for (int i = 0; i < picturesFolders.Length; i++)
                    moshaf.Items.Add(picturesFolders[i].Split('\\').Last());
                moshaf.Items.Add("تحميل ...");
                string s = @"pictures\TheChosenMoshaf.txt";
                if (File.Exists(s) && (s = File.ReadAllText(s, Encoding.UTF8)).Trim() != "" && Directory.Exists($@"pictures\{s}"))
                    moshaf.SelectedItem = s;
                else
                    moshaf.SelectedItem = picturesFolders[0].Split('\\').Last();
            }

            // مجلد الصور غير موجود ؟
            else if (textsFiles != null && textsFiles.Length > 0)
            {
                moshaf.SelectedIndex = 0;
                moshaf.Items.Add("تحميل ...");
                textMode = true; // التبديل إلى RichTextBox
                pageZoom.Enabled = false; // تعطيل خاصية تكبير الصفحة
                quranText.AddRichTextBoxInControls(Controls, quranPic.Location.X, quranPic.Location.Y, quranPic.Width, quranPic.Height); // اظهاره في النافذة
                quranText.AddEventHandler(PageQuranText_Click); // اضافة دالة تُنفذ عند الضغط بالماوس
                quranPic.Visible = false;
                //discri.Visible = false;
            }

            else
            {
                MessageBox.Show($"مجلدات المصاحف غير موجودة،\nسيتم إغلاق البرنامج.");
                Close();
            }

            if (textsFiles != null && textsFiles.Length > 0)
                quranText.Start(textsFiles[0], (int)Surah.Value, (int)Ayah.Value);

            Surahs.Items.Clear(); // إفراغ قائمة ال ComboBox
            if (textMode) Surahs.Items.AddRange(quranText.GetSurahNames()); // ملأها بأسماء السور;
            else if (qPicture == 2) Surahs.Items.AddRange(quranTtf.GetSurahNames());
            else Surahs.Items.AddRange(quranPicture.GetSurahNames());
            Surahs.SelectedIndex = (int)Surah.Value - 1; // الإشارة على أول سورة

            quranAudio.AddInControls(Controls); // اضافة المشغل الصوتي إلى خلفية النافذة
            quranAudio.AddEventHandlerOfAyah(AyahAudio); // اضافة تنبيه لإنتهاء الآية حين يقرأ الشيخ
            quranAudio.AddEventHandlerOfWord(WordAudio);

            // اضافة التفاسير
            textsFiles = null;
            if (Directory.Exists("tafasir")) textsFiles = Directory.GetFiles("tafasir");
            tafasir.Items.Clear();
            if (textsFiles != null && textsFiles.Length > 0)
            {
                tafasir.Items.Add("* التفاسير ...");
                for (int i = 0; i < textsFiles.Length; i++)
                {
                    tafasir.Items.Add(textsFiles[i].Split('\\').Last().Replace(".db", ""));
                }
                tafasir.SelectedIndex = 0;
            }
            // اضافة التراجم
            textsFiles = null;
            if (Directory.Exists("translations")) textsFiles = Directory.GetFiles("translations");
            if (textsFiles != null && textsFiles.Length > 0)
            {
                tafasir.Items.Add("* التراجم ...");
                for (int i = 0; i < textsFiles.Length; i++)
                {
                    tafasir.Items.Add(textsFiles[i].Split('\\').Last().Replace(".db", ""));
                }
                tafasir.SelectedIndex = 0;
            }

            // الوقوف عند الآية التي كنت فيها قبل اغلاق البرنامج آخر مرة
            try
            {
                if (File.Exists(save + "AyahNumberCurrent"))
                {
                    stringArray = File.ReadAllText(save + "AyahNumberCurrent").Split(',');
                    Surah.Value = Convert.ToInt32(stringArray[0]);
                    Ayah.Value = Convert.ToInt32(stringArray[1]);
                }
            }
            catch { }

            // تشغيل الشيخ الذي كنت تسمعه آخر مرة
            try
            {
                if (File.Exists(save + "MoshafAudioCurrent"))
                {
                    moshafAudio = File.ReadAllText(save + "MoshafAudioCurrent");
                    quranAudio.Start(moshafAudio, (int)Surah.Value, (int)Ayah.Value);
                    time5.Text = quranAudio.GetCurrentPosition();
                    quranAudio.OneTimePause(); // ثم إيقاف صوته :)
                }
            }
            catch { }

            // ذاكرتك ضعيفة ولا تتذكر كتاب التفسير الذي كنت عليه قبلاً؟
            try
            {
                if (File.Exists(save + "TafseerCurrent"))
                {
                    tafseer = File.ReadAllText(save + "TafseerCurrent");
                    tafasir.SelectedItem = tafseer;
                }
            }
            catch { }

            try
            {
                if (File.Exists(save + "Volume"))
                    volume.Value = Convert.ToInt32(File.ReadAllText(save + "Volume"));
            }
            catch { }

            // FormWindowState -> Maximized
            fs = new FormSize(SizeX, SizeY, Size.Width, Size.Height);
            fs.SetControls(Controls);
            AddMashaykhButtons();

            // The Controls which backcolor is not subject to the form's backcolor.
            ControlsList.AddRange(new List<Control> { Surahs, Surah, Juz, Hizb, Quarter, Page, Ayah, pause, stop, Rate, SurahRepeat, AyahRepeat, copy, search, searchClose, searchText, searchList, srtFile, extension, comment, tafasir, tafseerCopy, saveRTF, descSave, latest, moshaf, addNewMoqrea, splitAll, splitter, videoEditor, youtubeCaptions });
            success = true;
        }

        private int qPicture;
        private void Moshaf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (moshaf.Items.Count != 1 && moshaf.SelectedIndex == moshaf.Items.Count - 1)
            {
                Process.Start("https://github.com/mohamedashref371/QuranKareem/tree/master/Quran%20Kareem/pictures");
            }
            else if (moshaf.SelectedIndex > 0)
            {
                if (qPicture == 2)
                    quranTtf.PageRichText.Visible = false;

                qPicture = 0;
                if (quranPicture.Start($@"pictures\{moshaf.SelectedItem}", (int)Surah.Value, (int)Ayah.Value))
                    qPicture = 1;
                else if (quranTtf.Start($@"pictures\{moshaf.SelectedItem}", (int)Surah.Value, (int)Ayah.Value))
                    qPicture = 2;

                if (qPicture == 1)
                {
                    quranPic.Visible = true;
                    quranPic.BackgroundImage = quranPicture.CurrentPicture;
                    quranPic.BackColor = dark.Text == "Dark" ? Coloring.Light.BackColor : Coloring.Night.BackColor;
                }
                else if (qPicture == 2)
                {
                    quranPic.Visible = false;
                    quranTtf.PageRichText.Location = new Point(quranPic.Location.X, quranPic.Location.Y);
                    quranTtf.PageRichText.BackColor = dark.Text == "Dark" ? Coloring.Light.BackColor : Coloring.Night.BackColor;
                    if (!Controls.Contains(quranTtf.PageRichText))
                    {
                        quranTtf.PageRichText.Click += PageFontQuranText_Click;
                        quranTtf.PageRichText.MouseWheel += QuranPic_MouseWheel;
                        Controls.Add(quranTtf.PageRichText);
                    }
                    quranTtf.PageRichText.Size = new Size(quranPic.Width, quranPic.Height);
                    quranTtf.SetWidth();
                    quranTtf.PageRichText.Visible = true;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!success) return;
            // حفظ الإعدادات الحالية عند إغلاق البرنامج لاستعادتها لاحقاً
            File.WriteAllText(save + "AyahNumberCurrent", Surah.Value + "," + Ayah.Value);
            File.WriteAllText(save + "MoshafTextCurrent", moshafText);

            if (!textMode && moshaf.SelectedIndex > 0 && moshaf.SelectedIndex < moshaf.Items.Count - 1)
                File.WriteAllText(@"pictures\TheChosenMoshaf.txt", (string)moshaf.SelectedItem, Encoding.UTF8);
            
            File.WriteAllText(save + "MoshafAudioCurrent", moshafAudio);
            File.WriteAllText(save + "TafseerCurrent", tafseer);
            File.WriteAllText(save + "Volume", volume.Value + "");
        }

        private void ExitForm_Click(object sender, EventArgs e) // زر الإغلاق
        {
            if (zoom) // البرنامج في وضعية الزوم
            {
                zoom = !zoom; // الخروج من وضعية الزوم
                for (int i = 0; i < Controls.Count; i++)
                {
                    if ((string)Controls[i].Tag == "Visible == true")
                    {
                        Controls[i].Visible = true;
                        Controls[i].Tag = null;
                    }
                }
                exitForm.TabStop = true;
                minimize.TabStop = true;
                if (qPicture == 2)
                {
                    quranTtf.PageRichText.Location = new Point(quranPictureLocation.X, quranPictureLocation.Y);
                    quranTtf.PageRichText.Size = new Size(quranPicSize.Width, quranPicSize.Height);
                    quranTtf.SetWidth();
                }
                else
                {
                    quranPic.Size = quranPicSize;
                    quranPic.Location = quranPictureLocation;
                }
            }
            else
            {
                Close();
            }
        }

        private void Minimize_Click(object sender, EventArgs e) => WindowState = FormWindowState.Minimized;

        Point quranPictureLocation;
        Size quranPicSize;
        bool zoom = false;
        // تكبير صفحة القرآن المصور
        private void PageZoom_Click(object sender, EventArgs e)
        {
            if (!zoom)
            {
                zoom = !zoom;
                for (int i = 0; i < Controls.Count; i++)
                {
                    if (Controls[i].Visible == true)
                    {
                        Controls[i].Tag = "Visible == true";
                        Controls[i].Visible = false;
                    }
                }
                exitForm.Visible = true; exitForm.TabStop = false;
                minimize.Visible = true; minimize.TabStop = false;
                quranPicSize = quranPic.Size;
                quranPictureLocation = quranPic.Location;
                if (qPicture == 2)
                {
                    quranTtf.PageRichText.Visible = true;
                    quranTtf.PageRichText.Location = new Point(30, exitForm.Location.Y + exitForm.Size.Height + 5);
                    quranTtf.PageRichText.Size = new Size(Width - 60, (int)(quranTtf.PageRichText.Height * (Width - 60.0) / quranTtf.PageRichText.Width));
                    quranTtf.SetWidth();
                }
                else
                {
                    quranPic.Visible = true;
                    quranPic.Location = new Point(30, exitForm.Location.Y + exitForm.Size.Height + 5);
                    quranPic.Size = new Size(Width - 60, (int)(quranPicture.Height * (Width - 60.0) / quranPicture.Width));
                    quranPic.Select();
                }
            }
            NumberOfTimesPictureRise = 0;
        }

        // تشغيل بكرة الماوس في وضعية تكبير الصفحة
        int NumberOfTimesPictureRise = 0;
        private void QuranPic_MouseWheel(object sender, MouseEventArgs e)
        {
            if (zoom)
            {
                if (e.Delta < 0 && NumberOfTimesPictureRise < 3)
                {
                    NumberOfTimesPictureRise++;
                    if (qPicture == 2)
                        quranTtf.PageRichText.Location = new Point(quranTtf.PageRichText.Location.X, (int)(quranTtf.PageRichText.Location.Y - quranTtf.PageRichText.Size.Height * 0.25));
                    else
                        quranPic.Location = new Point(quranPic.Location.X, (int)(quranPic.Location.Y - quranPic.Size.Height * 0.25));
                }
                else if (e.Delta > 0 && NumberOfTimesPictureRise >= 0)
                {
                    NumberOfTimesPictureRise--;
                    if (qPicture == 2)
                        quranTtf.PageRichText.Location = new Point(quranTtf.PageRichText.Location.X, (int)(quranTtf.PageRichText.Location.Y + quranTtf.PageRichText.Size.Height * 0.25));
                    else
                        quranPic.Location = new Point(quranPic.Location.X, (int)(quranPic.Location.Y + quranPic.Size.Height * 0.25));

                }
            }
        }

        // List of special controls which backcolor is not subject to the form's backcolor.
        readonly List<Control> ControlsList = new List<Control>();

        private void Dark_Click(object sender, EventArgs e)
        {
            if (dark.Text == "Dark")
            {
                dark.Text = "Light";

                BackColor = Color.Black; ForeColor = Color.White;
                dark.FillColor = Color.FromArgb(191, 191, 191);
                dark.ForeColor = Color.Black;
                quranPic.BackColor = Coloring.Night.BackColor;
                quranPicture.DarkMode = true;
                quranTtf.DarkMode = true;
                quranText.DarkMode = true;
                panel.BackColor = Color.FromArgb(0, 31, 63);
                for (int i = 0; i < ControlsList.Count; i++)
                {
                    if (ControlsList[i].BackColor != Color.Transparent)
                        ControlsList[i].BackColor = BackColor;
                    ControlsList[i].ForeColor = ForeColor;
                }
                ShaykhDesc.BackColor = Color.Gray;
            }
            else
            {
                dark.Text = "Dark";

                BackColor = SystemColors.Control; ForeColor = Color.Black;
                dark.FillColor = Color.FromArgb(64, 64, 64);
                dark.ForeColor = Color.White;
                quranPic.BackColor = Coloring.Light.BackColor;
                quranPicture.DarkMode = false;
                quranTtf.DarkMode = false;
                quranText.DarkMode = false;
                panel.BackColor = Color.FromArgb(255, 224, 192);
                for (int i = 0; i < ControlsList.Count; i++)
                {
                    if (ControlsList[i].BackColor != Color.Transparent)
                        ControlsList[i].BackColor = SystemColors.Window;
                    ControlsList[i].ForeColor = ForeColor;
                    if (ControlsList[i] is Button button) button.UseVisualStyleBackColor = true;
                }
                ShaykhDesc.BackColor = BackColor;
                ShaykhDesc.UseVisualStyleBackColor = true;
            }
            if (!textMode && qPicture == 1) quranPic.BackgroundImage = quranPicture.CurrentPicture;
        }
        #endregion

        #region أزرار المشايخ
        // اضافة ازرار المشايخ
        void AddMashaykhButtons() /* ليست أفضل شيئ */
        {
            List<string> audiosFolders = new List<string>();
            bool distinct = false;
            if (File.Exists("audios\\favourite.txt") && File.ReadAllText("audios\\favourite.txt").Trim() != "")
            {
                audiosFolders.AddRange(File.ReadAllText("audios\\favourite.txt", Encoding.UTF8).Replace("\r\n", "\n").Replace('\n', '*').Split('*').ToList());
                for (int i = 0; i < audiosFolders.Count; i++)
                {
                    stringArray = audiosFolders[i].Split('|');
                    if (stringArray[0].Trim() != "" && Directory.Exists("audios\\" + stringArray[0])) audiosFolders[i] = "audios\\" + audiosFolders[i];
                    else if (Directory.Exists(stringArray[0]) || stringArray[0].Trim() == ":line:" || stringArray[0].Trim() == ":space:") { }
                    else if (stringArray[0].Trim() == ":distinct:") { audiosFolders[i] = ""; distinct = true; }
                    else audiosFolders[i] = "";
                }
            }
            panel.Controls.Clear();
            try { audiosFolders.AddRange(Directory.GetDirectories("audios").ToList()); } catch { } // البحث في مجلد الصوتيات


            if (distinct)
            {
                string temp, temp2;
                for (int i = 0; i < audiosFolders.Count; i++)
                {
                    temp = audiosFolders[i].Split('|').First().Trim('\\', '/');
                    try { temp = Path.GetFullPath(temp); } catch { continue; }
                    for (int j = i + 1; j < audiosFolders.Count; j++)
                    {
                        temp2 = audiosFolders[j].Split('|').First().Trim('\\', '/');
                        try { temp2 = Path.GetFullPath(temp2); } catch { continue; }
                        if (temp == temp2) audiosFolders[j] = "";
                    }
                }
            }

            Guna2Button b;
            int y = -45;
            Color clr; Random rand = new Random();
            for (int i = 0; i < audiosFolders.Count; i++)
            {
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
                    Location = new Point(fs.GetNewX(3), fs.GetNewY(y)),
                    BorderRadius = 15
                };
                if (stringArray != null && stringArray.Length > 2 && stringArray[2].Trim() != "") b.ForeColor = Color.FromName(stringArray[2].Trim());
                if (stringArray != null && stringArray.Length > 3 && stringArray[3].Trim() != "") b.Text = stringArray[3];
                if (audiosFolders[i].Trim() == ":line:")
                {
                    b.Size = new Size(fs.GetNewX(243), fs.GetNewY(10));
                    y -= 35;
                }
                else
                {
                    if (b.Text == "") b.Text = audiosFolders[i].Trim('\\', '/').Split('\\').Last();
                    b.Font = new Font("Segoe UI", fs.GetNewX(12));
                    b.Size = new Size(fs.GetNewX(243), fs.GetNewY(45));
                    b.Cursor = Cursors.Hand;
                    b.Tag = audiosFolders[i];
                    b.Click += Button_Click; // اضافة تنبيه عند الضغط على الزر
                }
                panel.Controls.Add(b);
            }
        }

        // دالة عامة لجميع الأزرار عند الضغط عليها
        private void Button_Click(object sender /* الزر الذي ضغطت عليه */, EventArgs e)
        {
            string s = (string)((Guna2Button)sender).Tag;
            moshafAudio = s;
            quranAudio.Start(s, (int)Surah.Value, (int)Ayah.Value);
            time5.Text = quranAudio.GetCurrentPosition();
            folder.SelectedPath = Path.GetFullPath(s);
            if (File.Exists(s + "\\download links.txt") && Directory.GetFiles(s).Length <= 2)
            {
                if (MessageBox.Show("هل تريد تحميل المصحف لهذا الشيخ؟ .. سنطلعك بعد الانتهاء", "تحميل", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Task.Run(() => DownloadFiles(s));
                }
            }
        }

        // تحميل المصاحف المسموعة في الخلفية
        static void DownloadFiles(string s)
        {
            try
            {
                string[] links = File.ReadAllText(s + "\\download links.txt", Encoding.UTF8).Replace("\r\n", "\n").Replace('\n', '|').Split('|');
                string[] temp;
                System.Net.WebClient client = new System.Net.WebClient();
                if (links.Length > 0)
                {
                    for (int i = 0; i < links.Length; i++)
                    {
                        if (links[i].Trim().Length > 0)
                        {
                            try { temp = links[i].Split('/'); } catch { continue; }
                            try
                            {
                                client.DownloadFile(links[i], s + "\\" + temp.Last());
                            }
                            catch
                            {
                                if (MessageBox.Show($"حدث خطأ في تحميل ملف {temp.Last()} ..\nهل تريد استكمال تحميل الملفات الأخرى؟", "خطأ o_O", MessageBoxButtons.YesNo) == DialogResult.No) return;
                            }
                        }
                    }
                    MessageBox.Show("انتهى تحميل المصحف", ":)");
                }
                else MessageBox.Show("ملف تحميل المصحف فارغ", ":(");
            }
            catch { MessageBox.Show("حدث خطأ ما", "-_-"); }
        }
        #endregion

        #region التعامل مع التنقلات في المصحف
        bool allow = true; // أداة للتعامل مع الإستدعاءات التلقائية غير المرغوب فيها

        // إلقاء المسؤولية على Surah_ValueChanged
        private void Surahs_SelectedIndexChanged(object sender, EventArgs e) => Surah.Value = Surahs.SelectedIndex + 1;

        // فهرس برقم السورة
        private void Surah_ValueChanged(object sender, EventArgs e)
        {
            Surahs.SelectedIndex = (int)Surah.Value - 1;

            if (textMode)
            {
                if (allow) quranText.Set((int)Surah.Value);
                //Ayah.Minimum = quranText.AyahStart;
                ayahMaximumIsChanged = true;
                Ayah.Maximum = quranText.AyatCount;
                ayahMaximumIsChanged = false;
                SetAyah();
            }
            else if (qPicture == 2)
            {
                if (allow) quranTtf.Set((int)Surah.Value);
                ayahMaximumIsChanged = true;
                Ayah.Maximum = quranTtf.AyatCount;
                ayahMaximumIsChanged = false;
                SetAyah();
            }
            else
            {
                if (allow) quranPicture.Set((int)Surah.Value); //
                //Ayah.Minimum = quranPicture.AyahStart;
                ayahMaximumIsChanged = true;
                Ayah.Maximum = quranPicture.AyatCount;
                ayahMaximumIsChanged = false;
                SetAyah(); //
            }
            if (addNewMoqrea.Text == "إلغاء") EditMoqreaSurah((int)Surah.Value);
        }

        bool allowJuz = true;
        private void Juz_ValueChanged(object sender, EventArgs e) /* فهرس بالأجزاء */
        {
            if (!allowJuz) return;
            Quarter.Value = Juz.Value * 8 - 7; // إلقاء المسؤولية على Quarter_ValueChanged
        }

        private void Hizb_ValueChanged(object sender, EventArgs e) /* فهرس بالحزب */
        {
            if (!allowJuz) return;
            Quarter.Value = Hizb.Value * 4 - 3; // إلقاء المسؤولية على Quarter_ValueChanged
        }

        private void Quarter_ValueChanged(object sender, EventArgs e) /* فهرس بالربع */
        {
            allowJuz = false; // إسكات فهرس الجزء والحزب
            Juz.Value = Math.Ceiling(Quarter.Value / 8);
            Hizb.Value = Math.Ceiling(Quarter.Value / 4);
            allowJuz = true;

            if (textMode && allow)
            {
                quranText.Set(quarter: (int)Quarter.Value);
                SetAyah();
            }
            else if (allow)
            {
                if (qPicture == 2)
                    quranTtf.Set(quarter: (int)Quarter.Value);
                else
                    quranPicture.Set(quarter: (int)Quarter.Value);
                SetAyah();
            }
        }

        private void Page_ValueChanged(object sender, EventArgs e) /* فهرس بالصفحات */
        {

            if (textMode && allow)
            {
                quranText.Set(page: (int)Page.Value);
                SetAyah();
            }
            else if (allow)
            {
                if (qPicture == 2)
                    quranTtf.Set(page: (int)Page.Value);
                else
                    quranPicture.Set(page: (int)Page.Value);
                SetAyah();
            }
        }

        void SetAyah() // الجزء المتكرر في تنقلات المصحف
        {
            allow = false;
            if (textMode)
            {
                Surah.Value = quranText.SurahNumber;
                if (Ayah.Value == quranText.AyahNumber) Ayah_ValueChanged(null, null);
                else Ayah.Value = quranText.AyahNumber;
            }
            else if (qPicture == 2)
            {
                Surah.Value = quranTtf.SurahNumber;
                if (Ayah.Value == quranTtf.AyahNumber) Ayah_ValueChanged(null, null);
                else Ayah.Value = quranTtf.AyahNumber;
            }
            else
            {
                Surah.Value = quranPicture.SurahNumber;
                if (Ayah.Value == quranPicture.AyahNumber) Ayah_ValueChanged(null, null);
                else Ayah.Value = quranPicture.AyahNumber;
            }
            allow = true;
        }

        // فهرس بالآية داخل السورة
        bool ayahMaximumIsChanged = false;
        private void Ayah_ValueChanged(object sender, EventArgs e)
        {
            if (ayahMaximumIsChanged) return;
            if (textMode)
            {
                if (allow)
                {
                    quranText.Set((int)Surah.Value, (int)Ayah.Value);
                    allow = false;
                }
                Quarter.Value = quranText.QuarterNumber;
                Page.Value = quranText.PageNumber;
            }
            else
            {
                if (allow)
                {
                    if (qPicture == 2)
                        quranTtf.Set((int)Surah.Value, (int)Ayah.Value);
                    else
                        quranPicture.Set((int)Surah.Value, (int)Ayah.Value);
                    allow = false;
                }

                if (qPicture == 2)
                {
                    Quarter.Value = quranTtf.QuarterNumber;
                    Page.Value = quranTtf.PageNumber;
                }
                else if (qPicture == 1)
                {
                    Quarter.Value = quranPicture.QuarterNumber;
                    Page.Value = quranPicture.PageNumber;
                    quranPic.BackgroundImage = null;
                    quranPic.BackgroundImage = quranPicture.CurrentPicture;
                }
            }

            if (isAllow && textMode)
            {
                quranAudio.Set(quranText.SurahNumber, quranText.AyahNumber);
                if (quranText.CurrentWord > 0)
                    quranAudio.WordOf(quranText.CurrentWord);
            }
            else if (isAllow)
            {
                if (qPicture == 2)
                {
                    quranAudio.Set(quranTtf.SurahNumber, quranTtf.AyahNumber);
                    if (quranTtf.CurrentWord > 0)
                        quranAudio.WordOf(quranTtf.CurrentWord);
                }
                else
                {
                    quranAudio.Set(quranPicture.SurahNumber, quranPicture.AyahNumber);
                    if (quranPicture.CurrentWord > 0)
                        quranAudio.WordOf(quranPicture.CurrentWord);
                }
            }

            time5.Text = quranAudio.GetCurrentPosition();
            allow = true;
        }

        // quranAudios -> AddEventHandlerOfAyah
        bool isAllow = true;
        private void AyahAudio(object sender, EventArgs e)
        {
            isAllow = false;
            if (textMode && allow)
            {
                quranText.Set(quranAudio.SurahNumber, quranAudio.AyahNumber);
                SetAyah();
            }
            else if (allow)
            {
                if (qPicture == 2)
                    quranTtf.Set(quranAudio.SurahNumber, quranAudio.AyahNumber);
                else
                    quranPicture.Set(quranAudio.SurahNumber, quranAudio.AyahNumber);
                SetAyah();
            }
            isAllow = true;
        }

        // quranAudios -> AddEventHandlerOfWord
        private void WordAudio(object sender, EventArgs e)
        {
            if (textMode)
            {
                quranText.WordOf(quranAudio.CurrentWord);
            }
            else
            {
                if (quranAudio.CurrentWord > 0)
                    if (qPicture == 2)
                        quranTtf.WordOf(quranAudio.CurrentWord);
                    else
                        quranPicture.WordOf(quranAudio.CurrentWord);

                if (qPicture == 1)
                {
                    quranPic.BackgroundImage = null;
                    quranPic.BackgroundImage = quranPicture.CurrentPicture;
                }
            }
        }

        private void QuranPic_Click(object sender, EventArgs e)
        {
            if (allow
                && quranPicture.SetXY(MousePosition.X - quranPic.Location.X - Location.X,
                                    MousePosition.Y - quranPic.Location.Y - Location.Y,
                                     quranPic.Width, quranPic.Height)
                ) SetAyah();
        }

        // بديل عن المصحف المصور، ربما لن تراه في حياتك
        // quranTexts -> AddEventHandler
        private void PageQuranText_Click(object sender, EventArgs e)
        {
            if (allow && quranText.SetCursor()) SetAyah();
        }

        private void PageFontQuranText_Click(object sender, EventArgs e)
        {
            if (allow && quranTtf.SetCursor()) SetAyah();
        }
        #endregion

        #region نسخ القرآن وتفسيره والبحث فيه
        // نسخ الآية
        private void Copy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                if (normalText.Checked) Clipboard.SetText(quranText.AyahAbstractText((int)Surah.Value, (int)Ayah.Value));
                else Clipboard.SetText(quranText.AyahText((int)Surah.Value, (int)Ayah.Value));
            }
            catch { }
        }

        // نسخ تفسير الآية
        private void TafseerCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(quranTafseer.AyahTafseerText((int)Surah.Value, (int)Ayah.Value)); // كود النسخ
            }
            catch { }
        }

        // إختيار كتاب تفسير
        private void Tafasir_SelectedIndexChanged(object sender, EventArgs e)
        {
            tafseer = tafasir.Text;
            if (!tafseer.Contains("*"))
                if (File.Exists(@"tafasir\" + tafseer + ".db")) quranTafseer.Start(@"tafasir\" + tafseer + ".db");
                else quranTafseer.Start(@"translations\" + tafseer + ".db");
        }

        // إنشاء تنسيق نص منسق لتفسير الآية 
        private void SaveRTF_Click(object sender, EventArgs e)
        {
            saveRichText.FileName = $"Tafseer Surah {Surah.Value} Ayah {Ayah.Value}";
            rtb.Text = quranTafseer.AyahTafseerText((int)Surah.Value, (int)Ayah.Value);
            if (rtb.Text != "" && saveRichText.ShowDialog() == DialogResult.OK)
                rtb.SaveFile(saveRichText.FileName);
        }

        // البحث بالكلمات في القرآن
        private void Search_Click(object sender, EventArgs e)
        {
            searchList.Items.Clear();
            searchList.Items.AddRange(quranText.Search(searchText.Text, spellingErrors.Checked));
            searchList.Visible = true;
            searchClose.Visible = true;
        }

        // إختيار آية من قائمة البحث
        private void SearchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int[] sura_aya = quranText.SelectedSearchIndex(searchList.SelectedIndex);
            if (sura_aya == null) return;
            Surah.Value = sura_aya[0];
            Ayah.Value = sura_aya[1] >= Ayah.Minimum ? sura_aya[1] : Ayah.Minimum;
        }

        // إغلاق قائمة البحث
        private void SearchClose_Click(object sender, EventArgs e)
        {
            searchList.Visible = false;
            searchClose.Visible = false;
        }

        // إنشاء ملف ترجمة
        private void SrtFile_Click(object sender, EventArgs e)
        {
            saveSRTFile.FileName = $"Surah {Surah.Value.ToString().PadLeft(3, '0')}.srt";
            if (saveSRTFile.ShowDialog() == DialogResult.OK)
            {
                if (pageSrtCheck.Checked)
                {
                    int[] minmax = PictureQuran.Instance.GetStartAndEndOfPage();
                    File.WriteAllText(saveSRTFile.FileName, quranTafseer.SubRipText(quranAudio.GetPositionsOf((int)Surah.Value, minmax[0], minmax[1]), (int)Surah.Value, minmax[0], minmax[1]), Encoding.UTF8);
                }
                else
                    File.WriteAllText(saveSRTFile.FileName, quranTafseer.SubRipText(quranAudio.GetPositionsOf((int)Surah.Value), (int)Surah.Value), Encoding.UTF8);
            }
        }
        #endregion

        #region التعامل مع الصوت
        // تفعيل التكرار
        private void Repeat_CheckedChanged(object sender, EventArgs e)
            => quranAudio.Repeat(SurahRepeatCheck.Checked ? (int)SurahRepeat.Value : 1, AyahRepeatCheck.Checked ? (int)AyahRepeat.Value : 1);


        private void Pause_Click(object sender, EventArgs e) => quranAudio.Pause(); // إيقاف مؤقت للصوت
        private void Stop_Click(object sender, EventArgs e) => quranAudio.Stop(); // إيقاف دائم للصوت

        // سرعة الصوت
        private void Rate_ValueChanged(object sender, EventArgs e) => quranAudio.Rate = (double)Rate.Value;

        // مستوى إرتفاع الصوت
        private void Volume_ValueChanged(object sender, EventArgs e) => quranAudio.Volume = volume.Value;

        // حفظ الآية
        private void Splitter_Click(object sender, EventArgs e) => quranAudio.SurahSplitter();

        // تقطيع السورة لآيات
        private void SplitAll_Click(object sender, EventArgs e)
        {
            for (int i = 1; i <= quranAudio.AyatCount; i++)
            {
                quranAudio.Set(ayah: i);
                quranAudio.SurahSplitter();
            }
            Ayah_ValueChanged(null, null);
        }
        #endregion

        #region إضافة شيخ جديد
        // إضافة أدوات إضافة الشيخ الجديد
        void EditMoqreaSurah(int sura)
        {
            panel.Controls.Clear();
            int[] ayat = quranAudio.GetTimestamps(sura);
            string[] ayatS = quranText.SurahAbstractTexts(sura, 5, endAyatCheck.Checked);
            if (ayat == null) return;

            Label label; NumericUpDown num; Button btn;
            Font font = new Font("Tahoma", fs.GetNewX(13));
            Size labelSize = new Size(fs.GetNewX(257), fs.GetNewY(33));
            Size numSize = new Size(fs.GetNewX(120), fs.GetNewY(24));
            Size btnSize = new Size(fs.GetNewX(31), fs.GetNewY(23));
            Color clr = dark.Text == "Dark" ? Color.FromArgb(255, 204, 172) : Color.FromArgb(0, 51, 83);
            Color Fclr = dark.Text == "Dark" ? Color.Black : Color.White;
            int y = 28;
            int k;
            for (int i = 0; i < ayat.Length; i++)
            {

                if (Ayah.Minimum == 0 || i == 0) k = i - 1;
                else k = i;

                label = new Label
                {
                    Font = font,
                    Location = new Point(fs.GetNewX(3), fs.GetNewY(y)),
                    RightToLeft = RightToLeft.Yes,
                    Size = labelSize,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = endAyatCheck.Checked ? k : Ayah.Minimum == 0 ? i : i + 1
                };
                if (endAyatCheck.Checked)
                {
                    if (i != 0) label.Text = ayatS[i];
                    else label.Text = "الإستعاذة";
                }
                else
                {
                    if (i != ayat.Length - 1) label.Text = ayatS[i + 1];
                    else label.Text = "نهاية آخر آية";
                }
                if (endAyatCheck.Checked && i != 0 || !endAyatCheck.Checked && i != ayat.Length - 1) label.Click += AyahLabel_Click;
                panel.Controls.Add(label);

                num = new NumericUpDown
                {
                    BackColor = clr,
                    ForeColor = Fclr,
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
                num.MouseWheel += Timestamp_MouseWheel;
                num.ValueChanged += Timestamp_ValueChanged;
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

        // استدعاء الآية عند الضغط عليها
        void AyahLabel_Click(object sender, EventArgs e) => Ayah.Value = (int)((Label)sender).Tag;

        int tempInt;
        void Timestamp_ValueChanged(object sender, EventArgs e)
        {
            tempInt = (int)((NumericUpDown)sender).Tag;
            quranAudio.SetAyah((int)Surah.Value, tempInt, (int)(((NumericUpDown)sender).Value * 1000));
            if (tempInt >= Ayah.Minimum && tempInt != Ayah.Maximum && timestampChangeEventCheck.Checked)
                if (Ayah.Value != tempInt + 1) Ayah.Value = tempInt + 1;
                else Ayah_ValueChanged(sender, e);
        }

        private void Timestamp_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            var num = (NumericUpDown)sender;
            tempInt = (int)num.Tag;
            if (tempInt >= Ayah.Minimum && tempInt != Ayah.Maximum)
            {
                decimal decml = ((NumericUpDown)panel.Controls[3 * (tempInt - (int)Ayah.Minimum) + 1]).Value + 0.5M;
                if (decml > 0.5M || tempInt <= 1)
                {
                    if (num.Value < decml) num.Value = decml;
                    if (e.Delta > 0) num.Value += 3;
                    else if (num.Value >= decml + 3) num.Value -= 3;
                }
            }
        }

        void Timestamp_record(object sender, EventArgs e)
        {
            double temp = quranAudio.Mp3CurrentPosition();
            if (temp == 0) return;
            ((NumericUpDown)panel.Controls[(int)((Button)sender).Tag]).Value = (decimal)temp;
        }

        void Mp3Duration_record(object sender, EventArgs e)
        {
            double temp = quranAudio.Mp3Duration();
            if (temp == 0) return;
            ((NumericUpDown)panel.Controls[(int)((Button)sender).Tag]).Value = (decimal)temp;
        }

        // زر تشغيل وضعية إضافة مقرئ جديد
        private void AddNewMoqrea_Click(object sender, EventArgs e)
        {
            if (addNewMoqrea.Text != "إلغاء" && folder.ShowDialog() == DialogResult.OK && quranAudio.NewQuranAudio(folder.SelectedPath))
            {
                addNewMoqrea.Text = "إلغاء"; stop.Enabled = false;
                moshafAudio = folder.SelectedPath;
                splitter.Visible = false; splitAll.Visible = false;
                videoEditor.Visible = false;
                youtubeCaptions.Visible = false;
                timestampChangeEventCheck.Visible = true;
                endAyatCheck.Visible = true;
                ShaykhDesc.Enabled = true; addShaykhInfo.Enabled = true;
                EditMoqreaSurah((int)Surah.Value);
            }
            else
            {
                AddMashaykhButtons();
                addNewMoqrea.Text = "إضافة شيخ جديد"; stop.Enabled = true;
                timestampChangeEventCheck.Visible = false;
                endAyatCheck.Visible = false;
                ShaykhDesc.Enabled = false; addShaykhInfo.Enabled = false;
                splitter.Visible = true; splitAll.Visible = true;
                videoEditor.Visible = true;
                youtubeCaptions.Visible = true;
            }
        }

        // عند التفعيل سوف ترى نهاية الآيات بدل بدايتها
        private void EndAyatCheck_CheckedChanged(object sender, EventArgs e) => EditMoqreaSurah((int)Surah.Value);

        private void AddShaykhInfo_Click(object sender, EventArgs e)
        {
            if (endAyatCheck.Checked) MessageBox.Show("هذه التوقيتات هي لنهاية الآيات وليس بدايتها");
            else MessageBox.Show($"هذه التوقيتات هي لبداية الآيات إلا آخر توقيت");
        }

        // زر تنظيف الداتابيز من الشوائب
        private void DescSave_Click(object sender, EventArgs e) => quranAudio.SetDescription(extension.Text, comment.Text);

        // إضافة وصف للشيخ
        private void ShaykhDesc_(object sender, EventArgs e)
        {
            if (ShaykhDesc.Text == "الوصف" && ShaykhDesc.Enabled == true)
            {
                ShaykhDesc.Text = "إلغاء";
                lTafseer.Visible = false;
                tafasir.Visible = false;
                tafseerCopy.Visible = false;
                saveRTF.Visible = false;
                srtFile.Visible = false;
                lExt.Visible = true;
                extension.Text = quranAudio.Extension;
                extension.Visible = true;
                lComment.Visible = true;
                comment.Text = quranAudio.Comment;
                comment.Visible = true;
                descSave.Visible = true;
            }
            else if (ShaykhDesc.Text == "إلغاء")
            {
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

        private void Discri_Click(object sender, EventArgs e)
        {
            if (new DiscriminatorsForm().ShowDialog() == DialogResult.Yes)
                if (qPicture == 1)
                {
                    quranPicture.SetInitialColors();
                    quranPicture.SetDiscriminators();
                    if (dark.Text == "Dark")
                        quranPic.BackColor = Coloring.Light.BackColor;
                    else
                        quranPic.BackColor = Coloring.Night.BackColor;
                }
                else if (qPicture == 2)
                {
                    quranTtf.SetInitialColors();
                    quranTtf.SetDiscriminators();
                    if (dark.Text == "Dark")
                        quranTtf.PageRichText.BackColor = Coloring.Light.BackColor;
                    else
                        quranTtf.PageRichText.BackColor = Coloring.Night.BackColor;
                }
                else
                {
                    quranText.DarkMode = !quranText.DarkMode;
                    quranText.DarkMode = !quranText.DarkMode;
                }
        }

        private void VideoEditor_Click(object sender, EventArgs e)
        {
            new VideoEditorForm(qPicture).ShowDialog();
        }

        private void YoutubeCaptions_Click(object sender, EventArgs e)
        {
            new YoutubeCaptionsForm(qPicture).ShowDialog();
        }

        private void Latest_Click(object sender, EventArgs e) => Process.Start("https://github.com/mohamedashref371/QuranKareem");

        private void About_Click(object sender, EventArgs e) => Process.Start("https://facebook.com/Mohamed3713317");

        private void Guna2HtmlLabel1_Click(object sender, EventArgs e) { }

    }
}
