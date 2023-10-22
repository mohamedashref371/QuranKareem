using System;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
namespace QuranKareem
{
    class QuranPictures
    {
        private string path; // المسار
        private bool success = false; // نجح استدعاء ال QuranPicture ? :(

        private readonly SQLiteConnection quran; // SQLite Connection
        private readonly SQLiteCommand command; // SQLite Command
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount, quartersCount, pagesCount, linesCount, width, height;
        private string extension; // example: .png
        private Color background;
        private string linesHelp; // setXY function help
        private int pageStartId, ayahId;
        private Bitmap oPic;
        public Bitmap Picture { get; private set; }
        FastPixel fp;

        public int Narration { get; private set; }
        public int SurahNumber { get; private set; }
        public int QuarterNumber { get; private set; }
        public int PageNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }
        public bool IsDark { get; set; } = false;

        private int lineHeight; private int tempInt, tempInt2;

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
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || reader.GetInt32(1)/*version*/ != 1) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                quartersCount = reader.GetInt32(4);
                pagesCount = reader.GetInt32(5);
                linesCount = reader.GetInt32(6);
                width = reader.GetInt32(7);
                height = reader.GetInt32(8);
                var clr = reader.GetString(10).Split(',');
                background = Color.FromArgb(clr.Length > 3 ? Convert.ToInt32(clr[3]) : 255, Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                extension = reader.GetString(11);
                lineHeight = height / linesCount;

                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;

                Ayah(sura, aya);
            }
            catch { return; }
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

            int x5, x9, y5, y9; // متغيرات التلوين

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
            y9 = reader.GetInt32(5);
            x5 = reader.GetInt32(6);

            if (PageNumber != reader.GetInt32(3))
            {
                PageNumber = reader.GetInt32(3);
                reader.Close();
                command.Cancel();
                command.CommandText = $"SELECT * FROM pages WHERE id={PageNumber}";
                reader = command.ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                linesHelp = reader.GetString(2);
                PictureAt(PageNumber);
            }

            reader.Close();
            command.Cancel();
            quran.Close();


            Picture = (Bitmap)oPic.Clone();
            fp = new FastPixel(Picture);
            fp.Lock();

            // التلوين
            if (ayahColor != AyahColor.nothing)
            {
                if (ayahId - pageStartId == 0) { x9 = width - 1; y5 = 1; }
                else
                {
                    quran.Open();
                    command.CommandText = $"SELECT * FROM ayat WHERE id={ayahId - 1}";
                    reader = command.ExecuteReader();
                    reader.Read();
                    y5 = reader.GetInt32(5);
                    x9 = reader.GetInt32(6);
                    reader.Close();
                    command.Cancel();
                    quran.Close();
                    if (x9 == 0) { x9 = width - 1; y5 += 1; }
                }

                if (y5 == 1 && y9 == linesCount && x5 == 0 && x9 == width - 1) { fp.Unlock(true); return; }
                if (y5 == y9)
                {
                    y5 = lineHeight * y5 - lineHeight; y9 = lineHeight * y9 - 1;
                    Fun(x5, x9, y5, y9);
                }
                else
                {
                    if (y9 - y5 > 1) Fun(0, width - 1, lineHeight * y5, lineHeight * y9 - lineHeight - 1);
                    Fun(0, x9, lineHeight * y5 - lineHeight, lineHeight * y5 - 1);
                    Fun(x5, width - 1, lineHeight * y9 - lineHeight, lineHeight * y9 - 1);
                }
            }
            fp.Unlock(true);
        }

        private void PictureAt(int i)
        { // الصورة الحالية
            string s = i + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += extension;

            if (File.Exists(path + s)) oPic = new Bitmap(path + s);

            fp = new FastPixel(oPic);
            fp.Lock();
            if (IsDark) FunWhite(0, width - 1, 0, height - 1);
            fp.Unlock(true);
        }
        public void SetXY(int xMouse, int yMouse) { SetXY(xMouse, yMouse, width, height); }
        public void SetXY(int xMouse, int yMouse, int width, int height)
        { // مؤشر الماوس
            if (!success) return;
            xMouse = (int)(xMouse * (this.width / (decimal)width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)(yMouse * (this.height / (decimal)height)) + 1;
            int cLine = (int)Math.Ceiling((decimal)yMouse / lineHeight) - 1;
            int position = Convert.ToInt32(linesHelp.Substring(cLine * 2, 2)); // الإستفادة من مساعد السطور التي تسهل عليا البحث عن الآية
            int x;

            if (position >= 0 /* صفحة سورة الفاتحة وأول سورة البقرة بها فراغ من الأسفل */)
            {
                quran.Open();
                command.CommandText = $"SELECT * FROM ayat WHERE id={position + pageStartId}";
                reader = command.ExecuteReader();
                reader.Read();
                x = reader.GetInt32(6);
                if (cLine < 14 && position == Convert.ToInt32(linesHelp.Substring(cLine * 2 + 2, 2)) || x == 0)
                {
                    tempInt = reader.GetInt32(1);
                    tempInt2 = reader.GetInt32(4);
                }
                else
                {
                    while (true)
                    { // القليل من البحث الضئيل
                        if (xMouse >= x || reader.GetInt32(5) > cLine + 1) break;
                        position++;
                        reader.Close();
                        command.Cancel();
                        command.CommandText = $"SELECT * FROM ayat WHERE id={position + pageStartId}";
                        reader = command.ExecuteReader();
                        reader.Read();
                        x = reader.GetInt32(6);
                    }
                    tempInt = reader.GetInt32(1);
                    tempInt2 = reader.GetInt32(4);
                }
                reader.Close();
                command.Cancel();
                quran.Close();
                Ayah(tempInt, tempInt2); // استدعاء الملك
            }
        }

        public void RefreshPage()
        {
            PageNumber = 0;
            Ayah();
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
                            if (ayahColor == AyahColor.blue) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, p4.G < 128 ? p4.G : 255 - p4.G, 255));
                            else if (ayahColor == AyahColor.green) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, 128, p4.B < 128 ? p4.B : 255 - p4.B));
                            else if (ayahColor == AyahColor.darkCyan) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, p4.R < 128 ? p4.R : 255 - p4.R, 100, 100));
                            else if (ayahColor == AyahColor.darkRed) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 128, p4.G < 128 ? p4.G : 255 - p4.G, p4.B < 128 ? p4.B : 255 - p4.B));
                            else fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 255, p4.G < 128 ? p4.G : 255 - p4.G, p4.B < 128 ? p4.B : 255 - p4.B)); // غير لونها الى الأحمر
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
                        if (p4.A != 0 /*البكسل ليس شفافا*/ && background != p4 /*البكسل ليس الخلفية*/)
                        {
                            fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 255 - p4.R, 255 - p4.G, 255 - p4.B));
                        }
                    }
                }
            }
            catch { }
        }
    }
}
