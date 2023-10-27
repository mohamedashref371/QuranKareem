using System;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace QuranKareem
{
    class QuranPictures
    {
        private string path; // المسار
        private bool success = false; // نجح استدعاء ال QuranPicture ? :(

        private readonly SQLiteConnection quran; // SQLite Connection
        private readonly SQLiteCommand command; // SQLite Command
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount, quartersCount, pagesCount /*,linesCount*/;
        int width, height;
        private string extension; // example: .png
        private Color background, textColor = Color.Empty;
        //private string linesHelp; // setXY function help
        private int ayahId;
        private Bitmap oPic;
        public Bitmap Picture { get; private set; }
        public Bitmap WordPicture { get; private set; }
        FastPixel fp;

        public int Narration { get; private set; }
        public int SurahNumber { get; private set; }
        public int QuarterNumber { get; private set; }
        public int PageNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }
        public int CurrentWord { get; set; } = -1;

        public bool WordMode { get; set; } = true;
        public bool IsDark { get; private set; } = false;
        public void ChangeDark()
        {
            if (!success) return;
            IsDark = !IsDark;
            PageNumber = 0;
            background = Color.FromArgb(background.A, 255 - background.R, 255 - background.G, 255 - background.B);
            textColor = Color.FromArgb(255 - background.R, 255 - background.G, 255 - background.B);
            Ayah();
        }

        //private int lineHeight;
        private int tempInt, tempInt2;

        public static QuranPictures Instance { get; private set; } = new QuranPictures();

        private QuranPictures()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        // الدالة البداية
        public void QuranPicture(string path, int sura = 1, int aya = 0)
        {

            if (path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;
            this.path = path; success = false;

            try
            {
                if (File.Exists(path + "000.db"))
                    quran.ConnectionString = "Data Source=" + path + "000.db;Version=3;";
                else
                    quran.ConnectionString = "Data Source=" + path + "0.db;Version=3;";

                quran.Open();
                command.CommandText = $"SELECT * FROM description";
                reader = command.ExecuteReader();

                if (!reader.HasRows) return;
                reader.Read();
                /* قررت التخلي عن الإصدار الأول من الداتابيز تماماً*/
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || reader.GetInt32(1)/*version*/ != 2) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                quartersCount = reader.GetInt32(4);
                pagesCount = reader.GetInt32(5);
                //linesCount = reader.GetInt32(6);
                width = reader.GetInt32(6);
                height = reader.GetInt32(7);

                string[] clr;
                clr = reader.GetString(8).Split(',');
                if (clr.Length == 3) textColor = Color.FromArgb(Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                clr = reader.GetString(9).Split(',');
                if (clr.Length >= 3) background = Color.FromArgb(clr.Length > 3 ? Convert.ToInt32(clr[3]) : 255, Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                
                extension = reader.GetString(10);
                //lineHeight = height / linesCount;

                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;

                Ayah(sura, aya);
            }
            catch { }
        }

        public string[] GetSurahNames()
        {
            if (!success) return new string[] { "" };
            string[] names = new string[surahsCount];
            quran.Open();
            command.CommandText = $"SELECT name FROM surahs";
            reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                names[i] = reader.GetString(0); // عمود واحد
                i++;
            }
            reader.Close();
            command.Cancel();
            quran.Close();
            return names;
        }

        public void Surah(int i) { Ayah(i, 0); }

        public void Quarter(int i)
        {
            if (!success) return;
            i = Math.Abs(i);
            if (i == 0) i = 1;
            else if (i > quartersCount) i = quartersCount;

            quran.Open();
            command.CommandText = $"SELECT ayat_id_start FROM quarters WHERE id={i}";
            reader = command.ExecuteReader();
            if (!reader.Read()) return;
            tempInt = reader.GetInt32(0);
            reader.Close();
            command.Cancel();
            quran.Close();
            AyahAt(tempInt);
        }

        public void Page(int i)
        {
            if (!success) return;
            i = Math.Abs(i);
            if (i == 0) i = 1;
            else if (i > pagesCount) i = pagesCount;

            quran.Open();
            command.CommandText = $"SELECT ayat_id_start FROM pages WHERE id={i}";
            reader = command.ExecuteReader();
            if (!reader.Read()) return;
            tempInt = reader.GetInt32(0);
            reader.Close();
            command.Cancel();
            quran.Close();
            AyahAt(tempInt);
        }

        public void AyahPlus()
        {
            if (!success) return;
            if (AyahNumber == AyatCount && SurahNumber < surahsCount) Ayah(SurahNumber + 1, 0);
            else if (AyahNumber == AyatCount) Surah(1);
            else Ayah(SurahNumber, AyahNumber + 1);
        }

        private void AyahAt(int id)
        {
            quran.Open();
            command.CommandText = $"SELECT surah,ayah FROM ayat WHERE id={id}";
            reader = command.ExecuteReader();
            reader.Read();
            tempInt = reader.GetInt32(0)/*surah*/; tempInt2 = reader.GetInt32(1)/*ayah*/;
            reader.Close();
            command.Cancel();
            quran.Close();
            Ayah(tempInt, tempInt2); // أحاول تركيز كل المجهود على دالة واحدة
        }

        public void Ayah() { Ayah(SurahNumber, AyahNumber); }
        public void Ayah(int aya) { Ayah(SurahNumber, aya); }

        public void Ayah(int sura, int aya)
        { // كما ترى .. المجهود كله عليها
            if (!success) return;

            sura = Math.Abs(sura); // تصحيح رقم السورة
            if (sura == 0) sura = 1;
            else if (sura > surahsCount) sura = surahsCount;

            if (aya == -1) aya = 0;
            else aya = Math.Abs(aya);

            quran.Open();
            if (sura != SurahNumber)
            {
                command.CommandText = $"SELECT * FROM surahs WHERE id={sura}";
                reader = command.ExecuteReader();
                reader.Read();
                SurahNumber = sura;
                Makya_Madanya = reader.GetBoolean(2);
                AyatCount = reader.GetInt32(3);
                reader.Close();
                command.Cancel();
            }
            if (aya > AyatCount) aya = AyatCount;
            AyahNumber = aya;

            command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            AyahStart = 0;
            if (!reader.HasRows && aya == 0)
            { // لا يوجد بسملة
                reader.Close();
                command.Cancel();
                command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}";
                reader = command.ExecuteReader();
                AyahNumber = 1;
                AyahStart = 1;
            }
            reader.Read();
            ayahId = reader.GetInt32(0);
            QuarterNumber = reader.GetInt32(2); // رقم الربع 

            if (PageNumber != reader.GetInt32(3))
            {
                PageNumber = reader.GetInt32(3);
                //reader.Close();
                //command.Cancel();
                //command.CommandText = $"SELECT * FROM pages WHERE id={PageNumber}";
                //reader = command.ExecuteReader();
                //reader.Read();
                //pageStartId = reader.GetInt32(1);
                PictureAt(PageNumber);
            }

            reader.Close();
            command.Cancel();

            CurrentWord = -1; words.Clear();
            Picture = (Bitmap)oPic.Clone();
            fp = new FastPixel(Picture);
            fp.Lock();

            // التلوين
            if (ayahColor != AyahColor.nothing)
            {
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM parts WHERE ayah_id={ayahId}";
                reader = command.ExecuteReader();
                while (reader.Read()) Fun(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                reader.Close();
                command.Cancel();
            }
            fp.Unlock(true);
            if (WordMode)
            {
                command.CommandText = $"SELECT DISTINCT(word),min_x,max_x,min_y,max_y FROM words WHERE ayah_id={ayahId} AND word>=1 AND word IS NOT NULL ORDER BY word ASC"; // AND word={word}
                reader = command.ExecuteReader();
                while (reader.Read()) words.AddRange(new int[] { reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4) });
                reader.Close();
                command.Cancel();
            }
            quran.Close();
        }

        private readonly List<int> words = new List<int>();
        public void WordPictureOf(int word)
        {
            if (word <= 0 || word * 4 > words.Count) { WordPicture = null; return; }
            WordPicture = (Bitmap)Picture.Clone();
            fp = new FastPixel(WordPicture);
            fp.Lock();
            FunWord(words[word * 4 - 4], words[word * 4 - 3], words[word * 4 - 2], words[word * 4 - 1]);
            CurrentWord = word;
            fp.Unlock(true);
        }

        private void PictureAt(int sura)
        { // الصورة الحالية
            string s = sura + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += extension;

            if (File.Exists(path + s)) oPic = new Bitmap(path + s);
            else if (File.Exists($"{path}{sura}{extension}")) oPic = new Bitmap($"{path}{sura}{extension}");
            if (IsDark)
            {
                fp = new FastPixel(oPic);
                fp.Lock();
                FunWhite(0, width - 1, 0, height - 1);
                fp.Unlock(true);
            } 
        }
        public void SetXY(int xMouse, int yMouse) => SetXY(xMouse, yMouse, width, height);
        public void SetXY(int xMouse, int yMouse, int width, int height, bool words=false)
        { // مؤشر الماوس
            if (!success) return;
            xMouse = (int)(xMouse * (this.width / (decimal)width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)(yMouse * (this.height / (decimal)height)) + 1;
            int word = -1; tempInt = -371;
            quran.Open();
            
            if (words)
            {
                command.CommandText = $"SELECT surah,ayah,word FROM (SELECT * FROM ayat WHERE page={PageNumber}) as ayats INNER JOIN (SELECT * FROM words WHERE min_x<={xMouse} AND max_x>={xMouse} AND min_y<={yMouse} AND max_y>={yMouse}) as wordss on wordss.ayah_id = ayats.id";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    tempInt = reader.GetInt32(0);
                    tempInt2 = reader.GetInt32(1);
                    word = !reader.IsDBNull(2)? reader.GetInt32(2) : -1;
                }
                else {
                    words = false;
                    reader.Close();
                    command.Cancel();
                }
            }
            if (!words)
            {
                command.CommandText = $"SELECT surah,ayah FROM (SELECT * FROM ayat WHERE page={PageNumber}) as ayats INNER JOIN (SELECT * FROM parts WHERE min_x<={xMouse} AND max_x>={xMouse} AND min_y<={yMouse} AND max_y>={yMouse}) as partss on partss.ayah_id = ayats.id";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    tempInt = reader.GetInt32(0);
                    tempInt2 = reader.GetInt32(1);
                }
            }
            
            reader.Close();
            command.Cancel();
            quran.Close();
            if (tempInt != -371) Ayah(tempInt, tempInt2); // استدعاء الملك
            if (words) WordPictureOf(word);
        }

        public AyahColor ayahColor = AyahColor.red;
        private void Fun(int x5, int x9, int y5, int y9)
        { // كود التلوين
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                {
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0 /*البكسل ليس شفافا*/ && background != p4 /*البكسل ليس الخلفية*/)
                        {
                            if (!Equal2Color(p4, background, 30) && (textColor == Color.Empty || Equal2Color(p4, textColor, 30)))
                            {
                                if (ayahColor == AyahColor.blue) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, p4.G < 128 ? p4.G : 255 - p4.G, 255));
                                else if (ayahColor == AyahColor.green) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, 128, p4.B < 128 ? p4.B : 255 - p4.B));
                                else if (ayahColor == AyahColor.darkCyan) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, 100, 100));
                                else if (ayahColor == AyahColor.darkRed) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 128, p4.G < 128 ? p4.G : 255 - p4.G, p4.B < 128 ? p4.B : 255 - p4.B));
                                else fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 255, p4.G < 128 ? p4.G : 255 - p4.G, p4.B < 128 ? p4.B : 255 - p4.B)); // غير لونها الى الأحمر
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void FunWord(int x5, int x9, int y5, int y9)
        {
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                {
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0 /*البكسل ليس شفافا*/ && background != p4 /*البكسل ليس الخلفية*/)
                        {
                            if (!Equal2Color(p4, background, 30) && (textColor == Color.Empty || Equal2Color(p4, textColor, 30))) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 121, 255, 225));
                        }
                    }
                }
            }
            catch { }
        }

        private void FunWhite(int x5, int x9, int y5, int y9)
        {
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                {
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 255 - p4.R, 255 - p4.G, 255 - p4.B));
                    }
                }
            }
            catch { }
        }

        private bool Equal2Color(Color clr1, Color clr2, int delta=0)
        {
            return clr1.A==255 && clr2.A==255 && Math.Abs(clr1.R - clr2.R) <= delta && Math.Abs(clr1.G - clr2.G) <= delta && Math.Abs(clr1.B - clr2.B) <= delta;
        }
    }
}
