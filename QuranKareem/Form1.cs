//using Microsoft.Data.Sqlite;
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

        public Form1() { InitializeComponent(); }

        readonly string save = Microsoft.VisualBasic.FileIO.SpecialDirectories.AllUsersApplicationData.Replace("1.0.0.0","");

        readonly QuranTexts quranTexts = QuranTexts.Instance;
        readonly QuranPictures quranPictures = QuranPictures.Instance;
        readonly QuranAudios quranAudios = QuranAudios.Instance;

        string moshaf = "0";
        string author="-1";

        bool textMode =false;

        private void Form1_Load(object sender, EventArgs e) {
            color.SelectedIndex = 1;
            string[] stringArray, textsFiles=null, picturesFolders=null, audiosFolders=null;

            try { textsFiles = Directory.GetFiles("texts"); /*البحث في مجلد المصاحف المكتوبة */ } catch { }

            if (textsFiles != null && textsFiles.Length > 0)
                quranTexts.QuranText(textsFiles[0], (int)Surah.Value, (int)Ayah.Value);


            try { picturesFolders = Directory.GetDirectories("pictures"); /* البحث في مجلد المصاحف المصورة */ } catch { }

            if (picturesFolders != null && picturesFolders.Length > 0)
                quranPictures.QuranPicture(picturesFolders[0], (int)Surah.Value, (int)Ayah.Value);

            else if (textsFiles != null && textsFiles.Length > 0) {
                textMode = true;
                quranTxt.Visible = true;
                quranPic.Visible = false;
                try { quranTxt.Font = new Font(quranTexts.fontFamily, 20F); } catch { }
            }
            else Close();

            if (textMode) quranTxt.Text = quranTexts.PageText; // اظهار النص الذي سيعطيه لك
            else quranPic.BackgroundImage = quranPictures.Picture; // اظهار الصورة التي سيعطيها لك

            Surahs.Items.Clear(); // إفراغ القائمة من ال ComboBox
            if (textMode) Surahs.Items.AddRange(quranTexts.GetSurahNames()); // ملأها بأسماء السور;
            else Surahs.Items.AddRange(quranPictures.GetSurahNames());
            Surahs.SelectedIndex = (int)Surah.Value - 1; // الإشارة على أول سورة

            quranAudios.AddInControls(Controls); // 
            quranAudios.AddEventHandler(Audio_);

            try { audiosFolders = Directory.GetDirectories("audios"); } catch { }

            //Button b;
            Guna2Button b;
            
            int y = 5;
            Color clr; Random rand = new Random();
            if (audiosFolders!=null)
            for (int i=0; i< audiosFolders.Length;i++) {
                clr = Color.FromArgb(rand.Next(0, 256), rand.Next(0, 200), rand.Next(0, 256));
                b = new Guna2Button(); // new Button();
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
                //panel1.Controls.Add(b);
                y += 50;
            }

            try {
                if (File.Exists(save + "Surah")){
                    stringArray = File.ReadAllText(save + "Surah").Split(',');
                    Surah.Value = Convert.ToInt32(stringArray[0]);
                    Ayah.Value = Convert.ToInt32(stringArray[1]);
                    if (stringArray[3] != "-1") {
                        author = stringArray[3];
                        quranAudios.QuranAudio(audiosFolders[Convert.ToInt32(stringArray[3])], (int)Surah.Value, (int)Ayah.Value);
                        NewDateTime();
                        quranAudios.Pause();
                    }
                }
            } catch { }
            
        }

        private void Button_Click(object sender, EventArgs e) {
            author = ((Guna2Button)sender).Tag+"";
            quranAudios.QuranAudio(@"audios\" + ((Guna2Button)sender).Text, (int)Surah.Value, (int)Ayah.Value);
            NewDateTime();
        }

        private void Repeat_CheckedChanged(object sender, EventArgs e){
            quranAudios.Repeat(SurahRepeatCheck.Checked ? (int)SurahRepeat.Value : 1, AyahRepeatCheck.Checked ? (int)AyahRepeat.Value : 1);
        }

        bool allow = true; // أداة للتعامل مع الإستدعاءات التلقائية غير المرغوب فيها
        private void Surahs_SelectedIndexChanged(object sender, EventArgs e) {
            if ( !allow) { return; }
            allow = false; // قفل الباب
            if (textMode) {
                quranTexts.surah(Surahs.SelectedIndex + 1);
                Surah.Value = quranTexts.Surah; // وضع البيانات في ال NumericUpDown
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
                quranTxt.Text = quranTexts.PageText;
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
            if ( !allow) { return; }
            allow = false;
            if (textMode) {
                quranTexts.surah((int)Surah.Value);
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
                quranTxt.Text = quranTexts.PageText;
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

        bool allowJuz=true;
        private void Juz_ValueChanged(object sender, EventArgs e) {
            if (!allowJuz) { return; }
            Quarter.Value = Juz.Value * 8 -7;
        }

        private void Hizb_ValueChanged(object sender, EventArgs e) {
            if (!allowJuz) { return; }
            Quarter.Value = Hizb.Value * 4 -3;
        }

        private void Quarter_ValueChanged(object sender, EventArgs e) {
            allowJuz = false;
            Juz.Value = Math.Ceiling(Quarter.Value / 8);
            Hizb.Value = Math.Ceiling(Quarter.Value / 4);
            allowJuz = true;
            if ( !allow) { return; }
            allow = false;

            if (textMode) {
                quranTexts.quarter((int)Quarter.Value);
                Surah.Value = quranTexts.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Page.Value = quranTexts.Page;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
                quranTxt.Text = quranTexts.PageText;
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

        private void Page_ValueChanged(object sender, EventArgs e) {
            if ( !allow) { return; }
            allow = false;

            if (textMode) {
                quranTexts.page((int)Page.Value);
                Surah.Value = quranTexts.Surah;
                Surahs.SelectedIndex = (int)Surah.Value - 1;
                Quarter.Value = quranTexts.Quarter;
                Ayah.Minimum = quranTexts.AyahStart;
                Ayah.Maximum = quranTexts.AyatCount;
                Ayah.Value = quranTexts.Ayah;
                quranTxt.Text = quranTexts.PageText;
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

        private void Ayah_ValueChanged(object sender, EventArgs e) {
            if ( !allow) { return; }
            allow = false;
            if (textMode) {
                quranTexts.ayah((int)Surah.Value, (int)Ayah.Value);
                Quarter.Value = quranTexts.Quarter;
                Page.Value = quranTexts.Page;
                quranTxt.Text = quranTexts.PageText;
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
                quranTxt.Text = quranTexts.PageText;
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
            time5.Text += temp + ":";

            cp -= temp * 60000;
            temp = cp / 1000;
            time5.Text += temp + ".";

            cp -= temp * 1000;
            time5.Text += cp;
        }

        private void Color_SelectedIndexChanged(object sender, EventArgs e) {
            if (color.SelectedIndex == 1) { quranPictures.textColor = QuranPictures.TextColor.red; quranTexts.textColor = QuranTexts.TextColor.red; }
            else if (color.SelectedIndex == 2) { quranPictures.textColor = QuranPictures.TextColor.green; quranTexts.textColor = QuranTexts.TextColor.green; }
            else if (color.SelectedIndex == 3) { quranPictures.textColor = QuranPictures.TextColor.blue; quranTexts.textColor = QuranTexts.TextColor.blue; }
            else if (color.SelectedIndex == 4) { quranPictures.textColor = QuranPictures.TextColor.darkCyan; quranTexts.textColor = QuranTexts.TextColor.darkCyan; }
            else if (color.SelectedIndex == 5) { quranPictures.textColor = QuranPictures.TextColor.darkRed; quranTexts.textColor = QuranTexts.TextColor.darkRed; }
            else { quranPictures.textColor = QuranPictures.TextColor.nothing; quranTexts.textColor = QuranTexts.TextColor.nothing; }
        }

        private void copy_Click(object sender, EventArgs e) {
            try {
                if (normalText.Checked) Clipboard.SetText(quranTexts.ayahAbstractText((int)Surah.Value, (int)Ayah.Value));
                else Clipboard.SetText(quranTexts.ayahText((int)Surah.Value, (int)Ayah.Value));
            } catch { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            File.WriteAllText(save + "Surah", Surah.Value + "," + Ayah.Value+","+ moshaf + ","+author);
        }

        private void Pause_Click(object sender, EventArgs e) { quranAudios.Pause(); }
        private void Stop_Click(object sender, EventArgs e) { quranAudios.Stop(); }

        private void Rate_ValueChanged(object sender, EventArgs e) { quranAudios.Rate((double)Rate.Value); }

        private void exitForm_Click(object sender, EventArgs e) { Close(); }

        private void minimize_Click(object sender, EventArgs e) { WindowState = FormWindowState.Minimized; }

    }
}
