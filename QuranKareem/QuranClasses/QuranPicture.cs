using System;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using static QuranKareem.Coloring;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class QuranPicture : AbsQuranVisual
    {
        private string path; // المسار

        public int Width { get; private set; }
        public int Height { get; private set; }
        private string extension; // example: .png
        private Color background, textColor = Color.Empty;
        private Bitmap oPic;
        public Bitmap Picture { get; private set; }
        public Bitmap WordPicture { get; private set; }
        FastPixel fp;

        public int CurrentWord { get; private set; } = -1;
        public bool WordMode { get; set; } = false;

        public bool IsDark { get; private set; } = false;
        public void ChangeDark()
        {
            if (!success) return;
            IsDark = !IsDark;
            PageNumber = 0;
            if (background.A != 0) background = Color.FromArgb(background.A, 255 - background.R, 255 - background.G, 255 - background.B);
            if (textColor != Color.Empty) textColor = Color.FromArgb(255 - background.R, 255 - background.G, 255 - background.B);
            Ayah();
        }

        public static readonly QuranPicture instance = new QuranPicture();

        private QuranPicture():base()
        {
        }

        // الدالة البداية
        public override void Start(string path, int sura = 1, int aya = 0)
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
                version = reader.GetInt32(1);
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || version < 2 || version > 3) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                quartersCount = reader.GetInt32(4);
                pagesCount = reader.GetInt32(5);
                Width = reader.GetInt32(6);
                Height = reader.GetInt32(7);

                string[] clr;
                clr = reader.GetString(8).Split(',');
                if (clr.Length == 3) textColor = Color.FromArgb(Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                else textColor = Color.Empty;
                clr = reader.GetString(9).Split(',');
                if (clr.Length >= 3) background = Color.FromArgb(clr.Length > 3 ? Convert.ToInt32(clr[3]) : 255, Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));

                extension = reader.GetString(10);
                Comment = reader.GetString(11);
                success = true;

                PageNumber = 0;
            }
            catch { }
            finally
            {
                reader?.Close();
                quran.Close();
            }
            if (success)
            {
                oPic = new Bitmap(Width, Height);
                Ayah(sura, aya);
            }
        }

        #region التنقلات في المصحف

        public override void Ayah(int sura, int aya)
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
                Close();
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
            int ayahId = reader.GetInt32(0);
            QuarterNumber = reader.GetInt32(2); // رقم الربع 

            if (PageNumber != reader.GetInt32(3))
            {
                PageNumber = reader.GetInt32(3);
                PictureAt(PageNumber);
            }

            Close();

            CurrentWord = -1; words.Clear(); WordPicture = null;
            Picture = (Bitmap)oPic.Clone();
            fp = new FastPixel(Picture);
            fp.Lock();

            // التلوين
            if (AyahColor.A != 0)
            {
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM parts WHERE ayah_id={ayahId}";
                reader = command.ExecuteReader();
                while (reader.Read()) ColoringAyah(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                Close();
            }
            fp.Unlock(true);
            if (WordMode)
            {
                command.CommandText = $"SELECT word,min_x,max_x,min_y,max_y FROM words WHERE ayah_id={ayahId} AND word>=1 AND word<=599 GROUP BY word ORDER BY word ASC";
                reader = command.ExecuteReader();
                while (reader.Read()) words.AddRange(new int[] { reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4) });
                reader.Close();
            }
            quran.Close();
        }

        private void PictureAt(int page) // Part Of Ayah Function
        { // الصورة الحالية
            string s = page + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += extension;

            if (File.Exists(path + s)) oPic = new Bitmap(path + s);
            else if (File.Exists($"{path}{page}{extension}")) oPic = new Bitmap($"{path}{page}{extension}");
            else
            {
                string[] filesName = Directory.GetFiles(path);
                for (int i = 0; i < filesName.Length; i++)
                {
                    if (filesName[i].Split('\\').Last().Contains(page.ToString().PadLeft(3, '0')))
                        oPic = new Bitmap(filesName[i]);
                }
            }

            if (IsDark)
            {
                fp = new FastPixel(oPic);
                fp.Lock();
                ReverseColors(0, Width - 1, 0, Height - 1);
                fp.Unlock(true);
            }
        }

        private readonly List<int> words = new List<int>();
        public void WordOf(int word)
        {
            if (word <= 0 || word * 4 > words.Count || !WordMode) { WordPicture = null; return; }
            WordPicture = (Bitmap)Picture.Clone();
            fp = new FastPixel(WordPicture);
            fp.Lock();
            if (WordColor.A != 0) ColoringWord(words[word * 4 - 4], words[word * 4 - 3], words[word * 4 - 2], words[word * 4 - 1]);
            CurrentWord = word;
            fp.Unlock(true);
        }

        public bool SetXY(int xMouse, int yMouse) => SetXY(xMouse, yMouse, Width, Height);
        public bool SetXY(int xMouse, int yMouse, int width, int height)
        { // مؤشر الماوس
            if (!success) return false;
            xMouse = (int)(xMouse * (Width / (decimal)width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)(yMouse * (Height / (decimal)height)) + 1;
            int word = -1; tempInt = -371;
            quran.Open();
            bool words = WordMode;
            if (words)
            {
                command.CommandText = $"SELECT surah,ayah,word FROM (SELECT * FROM ayat WHERE page={PageNumber}) as ayats INNER JOIN (SELECT * FROM words WHERE min_x<={xMouse} AND max_x>={xMouse} AND min_y<={yMouse} AND max_y>={yMouse}) as wordss on wordss.ayah_id = ayats.id";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    tempInt = reader.GetInt32(0);
                    tempInt2 = reader.GetInt32(1);
                    word = !reader.IsDBNull(2) ? reader.GetInt32(2) : -1;
                }
                else
                {
                    words = false;
                    Close();
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

            CloseConn();
            if (tempInt != -371) Ayah(tempInt, tempInt2); // استدعاء الملك
            if (words && tempInt != -371) WordOf(word);
            return tempInt != -371;
        }
        #endregion

        #region Coloring
        private void ColoringAyah(int x5, int x9, int y5, int y9)
        { // كود التلوين
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0 /*البكسل ليس شفافا*/ && background != p4 /*البكسل ليس الخلفية*/)
                        {
                            if (!Equal2Color(p4, background, 30) && (textColor == Color.Empty || Equal2Color(p4, textColor, 30)))
                                fp.SetPixel(x1, y1, Color.FromArgb(p4.A, AyahColor.R, AyahColor.G, AyahColor.B));
                        }
                    }
            }
            catch { }
        }

        private void ColoringWord(int x5, int x9, int y5, int y9)
        {
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0 && background != p4)
                        {
                            if (!Equal2Color(p4, background, 30) && (textColor == Color.Empty || Equal2Color(p4, textColor, 30)))
                                fp.SetPixel(x1, y1, Color.FromArgb(p4.A, WordColor.R, WordColor.G, WordColor.B));
                        }
                    }
            }
            catch { }
        }

        private void ReverseColors(int x5, int x9, int y5, int y9)
        {
            try
            {
                Color p4;
                for (int y1 = y5; y1 <= y9; y1++)
                    for (int x1 = x5; x1 <= x9; x1++)
                    {
                        p4 = fp.GetPixel(x1, y1);
                        if (p4.A != 0) fp.SetPixel(x1, y1, Color.FromArgb(p4.A, 255 - p4.R, 255 - p4.G, 255 - p4.B));
                    }
            }
            catch { }
        }

        private bool Equal2Color(Color clr1, Color clr2, int delta = 0)
        {
            return clr1.A == 255 && clr2.A == 255 && Math.Abs(clr1.R - clr2.R) <= delta && Math.Abs(clr1.G - clr2.G) <= delta && Math.Abs(clr1.B - clr2.B) <= delta;
        }
        #endregion

        #region lines images
        public List<Bitmap> GetLines()
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            for (int i = 1; i <= 15; i++)
                bitmaps.Add(GetLine(i));
            return bitmaps;
        }

        public List<List<Bitmap>> GetLinesWithWordsMarks()
        {
            var bitmaps = new List<List<Bitmap>>();
            for (int i = 1; i <= 15; i++)
                bitmaps.Add(GetLineWithWordsMarks(i));
            return bitmaps;
        }

        public List<Bitmap> GetLineWithWordsMarks(int line)
        {
            var coords = new List<int>();
            Bitmap bmap = GetLine(line, coords);
            if (bmap == null) return null;
            List<Bitmap> bitmaps = new List<Bitmap> { bmap };
            var list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM words JOIN ayat ON words.ayah_id=ayat.id WHERE page={PageNumber} AND line={line}";
            quran.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
                list.Add(new int[4] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3) });
            CloseConn();
            if (list.Count == 0) return bitmaps;
            Bitmap bitmap;
            for (int i = 0; i < list.Count; i++)
            {
                bitmap = (Bitmap)bmap.Clone();
                fp = new FastPixel(bitmap);
                fp.Lock();
                ColoringWord(
                    (list[i][0] - coords[0]) >= 0 ? list[i][0] - coords[0] : 0,
                    (list[i][1] - coords[0]) < bitmap.Width ? list[i][1] - coords[0] : bitmap.Width - 1,
                    (list[i][2] - coords[2]) >= 0 ? list[i][2] - coords[2] : 0,
                    (list[i][3] - coords[2]) < bitmap.Height ? list[i][3] - coords[2] : bitmap.Height - 1
                    );
                fp.Unlock(true);
                bitmaps.Add(bitmap);
            }
            return bitmaps;
        }

        public Bitmap GetLine(int line, List<int> lineCoordinates = null)
        {
            if (!success && version == 4) return null;
            List<int[]> list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM lines JOIN ayat ON lines.ayah_id=ayat.id WHERE page={PageNumber} AND line={line}";
            quran.Open();
            reader = command.ExecuteReader();
            while(reader.Read())
                list.Add(new int[4] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3) });
            CloseConn();
            if (list.Count == 0) return null;

            int[] final = list[0].ToArray(); // copy
            for(int i = 1; i < list.Count; i++)
            {
                if (list[i][0] < final[0]) final[0] = list[i][0];
                if (list[i][1] > final[1]) final[1] = list[i][1];
                if (list[i][2] < final[2]) final[2] = list[i][2];
                if (list[i][3] > final[3]) final[3] = list[i][3];
            }
            lineCoordinates?.AddRange(final.ToList());
            Bitmap bitmap = new Bitmap(final[1] - final[0] + 1, final[3] - final[2] + 1);
            Graphics gr = Graphics.FromImage(bitmap);
            gr.Clear(Color.Empty);
            for (int i = 0; i < list.Count; i++)
                gr.DrawImage(oPic, new Rectangle(list[i][0] - final[0], list[i][2] - final[2], list[i][1] - list[i][0], list[i][3] - list[i][2]), new Rectangle(list[i][0], list[i][2], list[i][1] - list[i][0], list[i][3] - list[i][2]), GraphicsUnit.Pixel);

            return bitmap;
        }
        #endregion
    }
}
