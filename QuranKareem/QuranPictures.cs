using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
namespace QuranKareem
{
    class QuranPictures {
        private string path; // المسار
        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranPicture ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال sql select

        private int surahsCount, quartersCount, pagesCount, linesCount, width, height;
        private string extension; // example: .png
        private Color background;
        private string linesHelp;
        private int pageStartId, ayahId;
        private Bitmap oPic;
        public Bitmap Picture { get; private set; }

        public int Narration { get; private set; }
        public int Surah { get; private set; }
        public int Quarter { get; private set; }
        public int Page { get; private set; }
        public int Ayah { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }

        private int lineHeight; private int temp,temp1;

        public static QuranPictures Instance { get; private set; } = new QuranPictures();

        public void QuranPicture(string path, int sura = 1, int aya = 0){
            if (path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }
            
            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;
            this.path = path; success = false;

            try {
                if (File.Exists(path + "000.db"))
                    quran = new SQLiteConnection("Data Source=" + path + "000.db;Version=3;"); // SQLite Connection
                else
                    quran = new SQLiteConnection("Data Source=" + path + "0.db;Version=3;"); // SQLite Connection
                quran.Open();
                reader = new SQLiteCommand($"SELECT * FROM description", quran).ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || reader.GetInt32(1)/*version*/ != 1) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                quartersCount = reader.GetInt32(4);
                pagesCount = reader.GetInt32(5);
                linesCount = reader.GetInt32(6);
                width = reader.GetInt32(7);
                height = reader.GetInt32(8);
                var clr = reader.GetString(10).Split(',');
                background = Color.FromArgb(Convert.ToInt32(clr[3]), Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                extension = reader.GetString(11);
                lineHeight = height / linesCount;
                quran.Close();
                success = true;

                ayah(sura, aya);
            } catch { return; }
        }
        
        public string[] GetSurahNames() {
            if (!success) return new string[]{""};
            string[] names = new string[surahsCount];
            quran.Open();
            reader = new SQLiteCommand($"SELECT name FROM surahs", quran).ExecuteReader();
            int i = 0;
            while (reader.Read()) {
                names[i] = reader.GetString(0); // عمود واحد
                i++;
            }
            quran.Close();
            return names;
        }

        public void surah(int i) { ayah(i, 0); }

        public void quarter(int i) {
            if (!success) return;
            i = Math.Abs(i);
            if (i == 0) i = 1;
            else if (i > quartersCount) i = quartersCount;

            quran.Open();
            reader = new SQLiteCommand($"SELECT * FROM quarters WHERE id={i}", quran).ExecuteReader();
            reader.Read();
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={reader.GetInt32(1)}", quran).ExecuteReader();
            reader.Read();
            temp = reader.GetInt32(1)/*surah*/; temp1 = reader.GetInt32(4)/*ayah*/;
            quran.Close();
            ayah(temp, temp1); // أحاول تركيز كل المجهود على دالة واحدة
        }

        public void page(int i) {
            if (!success) return;
            i = Math.Abs(i);
            if (i == 0) i = 1;
            else if (i > pagesCount) i = pagesCount;

            quran.Open();
            reader = new SQLiteCommand($"SELECT * FROM pages WHERE id={i}", quran).ExecuteReader();
            reader.Read();
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={reader.GetInt32(1)}", quran).ExecuteReader();
            reader.Read();
            temp = reader.GetInt32(1); temp1 = reader.GetInt32(4);
            quran.Close();
            ayah(temp, temp1);
        }

        public void AyahPlus() {
            if (!success) return;
            if (Ayah == AyatCount && Surah < surahsCount) ayah(Surah + 1, 0);
            else if (Ayah == AyatCount) surah(1);
            else ayah(Surah, Ayah + 1);
        }

        public void ayah(int sura, int aya) { // كما ترى .. المجهود كله عليها
            if (!success) return;
            sura = Math.Abs(sura);
            if (sura == 0) sura = 1;
            else if (sura > surahsCount) sura = surahsCount;

            if (aya == -1) aya = 0;
            else aya = Math.Abs(aya);

            int x5, x9, y5, y9;

            quran.Open();
            if (sura != Surah) {
                reader = new SQLiteCommand($"SELECT * FROM surahs WHERE id={sura}", quran).ExecuteReader();
                reader.Read();
                Surah = sura;
                Makya_Madanya = reader.GetBoolean(2);
                AyatCount = reader.GetInt32(3);
            }
            if (aya > AyatCount) aya = AyatCount;
            Ayah = aya;
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            AyahStart = 0;
            if (!reader.HasRows && aya == 0) { reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}", quran).ExecuteReader(); Ayah = 1; AyahStart = 1; }
            reader.Read();
            ayahId= reader.GetInt32(0);
            Quarter = reader.GetInt32(2);
            y9 = reader.GetInt32(5);
            x5= reader.GetInt32(6);
            if (Page != reader.GetInt32(3)) {
                Page = reader.GetInt32(3);
                reader = new SQLiteCommand($"SELECT * FROM pages WHERE id={Page}", quran).ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                linesHelp = reader.GetString(2);
                picture(Page);
            }

            // التلوين
            Picture = (Bitmap)oPic.Clone();
            
            if (textColor != TextColor.nothing) {
                if (ayahId - pageStartId == 0) { x9 = width - 1; y5 = 1; }
                else {
                    reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={ayahId - 1}", quran).ExecuteReader();
                    reader.Read();
                    y5 = reader.GetInt32(5);
                    x9 = reader.GetInt32(6);
                    if (x9 == 0) { x9 = width - 1; y5 += 1; }
                }

                if (y5 == 1 && y9 == linesCount && x5 == 0 && x9 == width - 1) return;
                if (y5 == y9) {
                    y5 = lineHeight * y5 - lineHeight; y9 = lineHeight * y9 - 1;
                    fun(x5, x9, y5, y9);
                }
                else {
                    if (y9 - y5 > 1) fun(0, width - 1, lineHeight * y5, lineHeight * y9 - lineHeight - 1);
                    fun(0, x9, lineHeight * y5 - lineHeight, lineHeight * y5 - 1);
                    fun(x5, width - 1, lineHeight * y9 - lineHeight, lineHeight * y9 - 1);
                }
            }
            quran.Close();
        }

        private void picture(int i) { // الصورة الحالية
            string s = i+"";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += extension;

            if (File.Exists(path + s)) oPic = new Bitmap(path + s);

        }
        public void setXY(int xMouse, int yMouse) { setXY(xMouse, yMouse, width, height); }
        public void setXY(int xMouse, int yMouse, int width, int height) { // مؤشر الماوس
            if (!success) return;
            xMouse = (int)( xMouse * (this.width / (decimal) width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)( yMouse * (this.height / (decimal) height));
            int cLine= (int)Math.Ceiling((decimal)yMouse / lineHeight) - 1;
            int position = Convert.ToInt32(linesHelp.Substring(cLine * 2, 2)); // الإستفادة من مساعد السطور التي تسهل عليا البحث عن الآية
            int x;
            
            if (position >= 0) { // صفحة سورة الفاتحة وأول سورة البقرة بها فراغ من الأسفل
                quran.Open();
                reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={position + pageStartId}", quran).ExecuteReader();
                reader.Read();
                x = reader.GetInt32(6);
                if (cLine<14 && position == Convert.ToInt32(linesHelp.Substring(cLine*2 +2, 2)) || x==0) {
                    temp = reader.GetInt32(1);
                    temp1 = reader.GetInt32(4);
                }
                else {
                    while (true) { // القليل من البحث الضئيل
                        if (xMouse>x || reader.GetInt32(5)> cLine+1) break;
                        position++;
                        reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={position + pageStartId}", quran).ExecuteReader();
                        reader.Read();
                        x = reader.GetInt32(6);
                    }
                    temp = reader.GetInt32(1);
                    temp1 = reader.GetInt32(4);
                }
                quran.Close();
                ayah(temp, temp1);
            }
        }

        void fun(int x5, int x9, int y5, int y9) { // كود التلوين
            if (!success) return;
            try {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++) {
                    for (int x1 = x5; x1 <= x9; x1++) {
                        p4 = Picture.GetPixel(x1, y1);
                        if (p4.A != 0 /*البكسل ليس شفافا*/ && background != p4 /*البكسل ليس الخلفية*/) {
                            if (textColor == TextColor.blue) Picture.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R, p4.G, 255));
                            else if (textColor == TextColor.green) Picture.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R, 128, p4.B));
                            else if (textColor == TextColor.darkCyan) Picture.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R, 100, 100));
                            else if (textColor == TextColor.darkRed) Picture.SetPixel(x1, y1, Color.FromArgb(p4.A, 128, p4.G, p4.B));
                            else Picture.SetPixel(x1, y1, Color.FromArgb(p4.A, 255, p4.G, p4.B)); // غير لونها الى الأحمر
                        }
                    }
                }
            } catch { }
        }

        public TextColor textColor = TextColor.red;
        public enum TextColor {
            nothing = 0, red = 1, green=2, blue=3, darkCyan=4, darkRed=5
        }
    }
}
