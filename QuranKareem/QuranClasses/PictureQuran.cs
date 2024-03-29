﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using static QuranKareem.Coloring;
using System.Text;

namespace QuranKareem
{
    internal class PictureQuran
    {
        #region Create The Class
        private bool success = false;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        public static PictureQuran Instance => new PictureQuran();

        private PictureQuran()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }
        #endregion

        #region Ayat Table
        private int ayahId;

        #region Surahs Table
        public int SurahNumber { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyatCount { get; private set; }
        #endregion

        public int QuarterNumber { get; private set; }
        public int PageNumber { get; private set; }
        public int AyahNumber { get; private set; }
        #endregion

        #region Description Table
        private int version;
        public int Narration { get; private set; }
        public int SurahsCount { get; private set; }
        //public int QuartersCount { get; private set; }
        public int PagesCount { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        private Color textColor = Color.Empty, background;
        public string Extension { get; private set; } // example: .png
        public string Comment { get; private set; }
        #endregion

        private string path;

        #region Bitmaps and Words
        private Bitmap oPic;
        public Bitmap Picture { get; private set; }
        public Bitmap WordPicture { get; private set; }
        private FastPixel fp;

        public int CurrentWord { get; private set; } = -1;
        public bool WordMode { get; set; } = false;
        #endregion

        public bool IsDark { get; private set; } = false;
        public void ChangeDark()
        {
            if (!success) return;
            IsDark = !IsDark;
            PageNumber = 0;
            if (background.A != 0) background = Color.FromArgb(background.A, 255 - background.R, 255 - background.G, 255 - background.B);
            if (textColor != Color.Empty) textColor = Color.FromArgb(255 - background.R, 255 - background.G, 255 - background.B);
            Set();
        }

        // الدالة البداية
        public void Start(string path, int sura = 1, int aya = 0)
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
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || version < 2 || version > 4) return;
                Narration = reader.GetInt32(2);
                SurahsCount = reader.GetInt32(3);
                //QuartersCount = reader.GetInt32(4);
                PagesCount = reader.GetInt32(5);
                Width = reader.GetInt32(6);
                Height = reader.GetInt32(7);

                string[] clr;
                clr = reader.GetString(8).Split(',');
                if (clr.Length == 3) textColor = Color.FromArgb(Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                else textColor = Color.Empty;
                clr = reader.GetString(9).Split(',');
                if (clr.Length >= 3) background = Color.FromArgb(clr.Length > 3 ? Convert.ToInt32(clr[3]) : 255, Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));

                Extension = reader.GetString(10);
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
                Set(sura, aya);
            }
        }

        #region التنقلات في المصحف
        public string[] GetSurahNames()
        {
            if (!success) return new string[] { "" };
            string[] names = new string[SurahsCount];
            quran.Open();
            command.CommandText = $"SELECT name FROM surahs";
            reader = command.ExecuteReader();

            int i = 0;
            while (reader.Read())
            {
                names[i] = reader.GetString(0); // عمود واحد
                i++;
            }
            reader.Close(); quran.Close();
            return names;
        }

        #region Set Ayah Method
        private readonly StringBuilder sql = new StringBuilder();

        /// <summary>
        /// Implementation Priority:<br/>
        ///   1- Surah (1:114), its Ayah (0:{AyatCount}).<br/>
        ///   1- Ayah (0:{AyatCount}) only.
        ///   2- Next parameter (false,true).
        ///   3- Juz (1:30), Hizb (1:2), Quarter (1:4).<br/>
        ///   3- Juz (1:30), Quarter (1:8).<br/>
        ///   3- Juz (1:30) only.<br/>
        ///   4- Hizb (1:60), Quarter (1:4).<br/>
        ///   4- Hizb (1:60) only.<br/>
        ///   5- Quarter (1:240) only.<br/>
        ///   6- Page (1:{PagesCount}).<br/>
        /// </summary>
        /// <param name="surah">Quran Surahs Count is 114.</param>
        /// <param name="ayah">Each Surah contains Ayat.</param>
        /// <param name="next">Go to the next Ayah.</param>
        /// <param name="juz">Quran Juz Count is 30 and one Juz contains 8 Quarters.</param>
        /// <param name="hizb">Quran Hizb Count is 60 and one Hizb contains 4 Quarters.</param>
        /// <param name="quarter">Quran Quarters Count is 240.</param>
        /// <param name="page">Usually the 'Quran Pages Count' is 604.</param>
        /// <returns>true if everything goes well.</returns>
        public bool Set(int surah = 0, int ayah = -2, bool next = false, int juz = 0, int hizb = 0, int quarter = 0, int page = 0)
        {
            if (!success) return false;

            #region SQL Building
            sql.Length = 0;
            sql.Append("SELECT id,surah,quarter,page,ayah FROM ayat WHERE ");
            #region Surah and Ayah
            // Ayah
            if ((surah <= 0 || surah == SurahNumber) && ayah >= -1)
            {
                if (ayah == AyatCount + 1)
                {
                    surah = SurahNumber != SurahsCount ? SurahNumber + 1 : 1;
                    ayah = 0;
                }
                else if (ayah > AyatCount + 1)
                {
                    surah = SurahNumber;
                    ayah = AyatCount;
                }
                else
                    surah = SurahNumber;

                if (ayah > 0)
                    sql.Append($"surah = {surah} AND ayah = {ayah}");
                else
                    sql.Append($"surah = {surah} AND (ayah = 0 OR ayah = 1) ORDER BY ayah");
            }

            // Surah and Ayah
            else if (surah >= 1 && surah <= SurahsCount + 1)
            {
                if (surah == SurahsCount + 1) surah = 1;
                if (ayah < 0) ayah = 0;
                if (ayah > 0)
                    sql.Append($"surah = {surah} AND ayah = {ayah}");
                else
                    sql.Append($"surah = {surah} AND (ayah = 0 OR ayah = 1) ORDER BY ayah");
            }

            // Ayah Plus
            else if (next && SurahNumber > 0)
            {
                if (SurahNumber == SurahsCount && AyahNumber == AyatCount)
                    sql.Append("ayah >= 0");
                else
                    sql.Append($"id > {ayahId} AND ayah >= 0");
            }
            #endregion
            #region Juz, Hizb, Quarter and Page
            // Juz then Hizb and Quarter
            else if (juz >= 1 && juz <= 31)
            {
                if (juz == 31) juz = 1;
                if (quarter <= 0 || quarter > 8) quarter = 1;
                if (hizb == 2 && quarter >= 1 && quarter <= 4) quarter += 4;

                sql.Append($"quarter = {juz * 8 - 8 + quarter} AND ayah>=0");
            }

            // Hizb then Quarter
            else if (hizb >= 1 && hizb <= 61)
            {
                if (hizb == 61) hizb = 1;
                if (quarter <= 0 || quarter > 4) quarter = 1;

                sql.Append($"quarter = {hizb * 4 - 4 + quarter} AND ayah>=0");
            }

            // Quarter only
            else if (quarter >= 1 && quarter <= 241)
            {
                if (quarter == 241) quarter = 1;
                sql.Append($"quarter = {quarter} AND ayah>=0");
            }
            
            // Page
            else if (page >= 1 && page <= PagesCount + 1)
            {
                if (page == PagesCount + 1) page = 1;
                sql.Append($"page = {page} AND ayah>=0");
            }
            #endregion
            #region Otherwise
            else if (SurahNumber > 0)
            {
                if (AyahNumber > 0)
                    sql.Append($"surah = {SurahNumber} AND ayah = {AyahNumber}");
                else
                    sql.Append($"surah = {SurahNumber} AND (ayah = 0 OR ayah = 1) ORDER BY ayah");
            }
            else
            {
                sql.Append("ayah >= 0");
            }
            #endregion
            sql.Append(" LIMIT 1");
            #endregion

            #region SQL Execution
            quran.Open();
            command.CommandText = sql.ToString();
            reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close(); quran.Close();
                return false;
            }
            ayahId = reader.GetInt32(0);
            surah = reader.GetInt32(1);
            QuarterNumber = reader.GetInt32(2);
            page = reader.GetInt32(3);
            AyahNumber = reader.GetInt32(4);
            reader.Close(); quran.Close();
            #endregion

            if (surah != SurahNumber) SurahData(surah);
            if (page != PageNumber) PictureAt(page);
            AyahData();

            return true;
        }

        private void SurahData(int surah)
        {
            SurahNumber = surah;
            command.CommandText = $"SELECT makya_madanya,ayat_count FROM surahs WHERE id={surah}";
            quran.Open();
            reader = command.ExecuteReader();
            reader.Read();
            Makya_Madanya = reader.GetBoolean(0);
            AyatCount = reader.GetInt32(1);
            reader.Close(); quran.Close();
        }

        private void AyahData()
        {
            CurrentWord = -1; words.Clear(); WordPicture = null;
            Picture = (Bitmap)oPic.Clone();
            fp = new FastPixel(Picture);
            fp.Lock();

            quran.Open();
            // التلوين
            if (AyahColor.A != 0)
            {
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM parts WHERE ayah_id={ayahId}";
                reader = command.ExecuteReader();
                while (reader.Read()) Coloring(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), AyahColor);
                reader.Close();
            }
            fp.Unlock(true);
            if (WordMode)
            {
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y,word FROM words WHERE ayah_id={ayahId} AND word>=1 AND word<=599 ORDER BY word";
                reader = command.ExecuteReader();
                int w = 1; ints.Clear();
                while (reader.Read()) // بفرض أن الكلمة ستبدأ برقم 1 وستكون السلسلة متصلة
                {
                    if (w != reader.GetInt32(4))
                    {
                        words.Add(ints.ToArray());
                        ints.Clear();
                        w = reader.GetInt32(4);
                    }
                    ints.Add(reader.GetInt32(0)); ints.Add(reader.GetInt32(1)); ints.Add(reader.GetInt32(2)); ints.Add(reader.GetInt32(3));
                }
                words.Add(ints.ToArray());
                reader.Close();
            }
            quran.Close();
        }
        private readonly List<int> ints = new List<int>();

        private void PictureAt(int page) // Part Of Ayah Function // الصورة الحالية
        {
            PageNumber = page;
            string s = page + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += Extension;

            if (File.Exists(path + s)) oPic = new Bitmap(path + s);
            else if (File.Exists($"{path}{page}{Extension}")) oPic = new Bitmap($"{path}{page}{Extension}");
            else
            {
                string[] filesName = Directory.GetFiles(path);
                for (int i = 0; i < filesName.Length; i++)
                {
                    if (filesName[i].Split('\\').Last().Contains(page.ToString().PadLeft(3, '0')))
                        oPic = new Bitmap(filesName[i]);
                }
            }

            fp = new FastPixel(oPic);
            fp.Lock();
            if (IsDark) ReverseColors(0, Width - 1, 0, Height - 1);
            PageDecorations();
            fp.Unlock(true);
        }
        private void PageDecorations()
        {
            quran.Open();
            command.CommandText = $"SELECT word,min_x,max_x,min_y,max_y FROM words JOIN ayat ON words.ayah_id = ayat.id WHERE (word<=0 OR word>=600) AND page={PageNumber}";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) == 0)
                {
                    Coloring(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), QuarterStartColor);
                }
                else if (reader.GetInt32(0) == 998)
                {
                    Coloring(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), SajdaColor);
                }
                else if (reader.GetInt32(0) == 999)
                {
                    Coloring(reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4), AyahEndColor);
                }
            }
            reader.Close(); quran.Close();
        }
        #endregion

        private readonly List<int[]> words = new List<int[]>();
        public void WordOf(int word)
        {
            if (word <= 0 || word > words.Count || !WordMode) { WordPicture = null; return; }
            WordPicture = (Bitmap)Picture.Clone();
            fp = new FastPixel(WordPicture);
            fp.Lock();
            if (WordColor.A != 0)
                for (int i = 0; i < words[word - 1].Length / 4; i++)
                    Coloring(words[word - 1][i * 4], words[word - 1][i * 4 + 1], words[word - 1][i * 4 + 2], words[word - 1][i * 4 + 3], WordColor);
            CurrentWord = word;
            fp.Unlock(true);
        }

        public bool SetXY(int xMouse, int yMouse) => SetXY(xMouse, yMouse, Width, Height);
        public bool SetXY(int xMouse, int yMouse, int width, int height)
        { // مؤشر الماوس
            if (!success) return false;
            xMouse = (int)(xMouse * (Width / (decimal)width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)(yMouse * (Height / (decimal)height)) + 1;
            int word = -1; int tempInt = -371, tempInt2 = 0;
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
                    reader.Close();
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

            reader.Close(); quran.Close();
            if (tempInt != -371) Set(tempInt, tempInt2); // استدعاء الملك
            if (words && tempInt != -371) WordOf(word);
            return tempInt != -371;
        }
        #endregion

        #region Coloring
        private void Coloring(int x5, int x9, int y5, int y9, Color color)
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
                                fp.SetPixel(x1, y1, Color.FromArgb(p4.A, color.R, color.G, color.B));
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
            var coords = new int[2];
            Bitmap bmap = GetLine(line, coords);
            if (bmap == null) return null;
            List<Bitmap> bitmaps = new List<Bitmap> { bmap };
            if (WordColor.A == 0) return bitmaps;
            var list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y,ayah_id,word FROM words JOIN ayat ON words.ayah_id=ayat.id WHERE page={PageNumber} AND line={line} AND word>=1 AND word<=599 ORDER BY ayah_id,word";
            quran.Open();
            reader = command.ExecuteReader();

            int a,w; ints.Clear();
            if (!reader.Read()) return bitmaps;
            a = reader.GetInt32(4);
            w = reader.GetInt32(5);

            do
            {
                if (a != reader.GetInt32(4) || w != reader.GetInt32(5))
                {
                    list.Add(ints.ToArray());
                    ints.Clear();
                    a = reader.GetInt32(4);
                    w = reader.GetInt32(5);
                }
                ints.Add(reader.GetInt32(0)); ints.Add(reader.GetInt32(1)); ints.Add(reader.GetInt32(2)); ints.Add(reader.GetInt32(3));
            } while (reader.Read());
            list.Add(ints.ToArray());

            reader.Close(); quran.Close();

            Bitmap bitmap;
            for (int i = 0; i < list.Count; i++)
            {
                bitmap = (Bitmap)bmap.Clone();
                fp = new FastPixel(bitmap);
                fp.Lock();
                for (int j = 0; j < list[i].Length / 4; j++)
                {
                    Coloring(
                    (list[i][j * 4] - coords[0]) >= 0 ? list[i][j * 4] - coords[0] : 0,
                    (list[i][j * 4 + 1] - coords[0]) < bitmap.Width ? list[i][j * 4 + 1] - coords[0] : bitmap.Width - 1,
                    (list[i][j * 4 + 2] - coords[1]) >= 0 ? list[i][j * 4 + 2] - coords[1] : 0,
                    (list[i][j * 4 + 3] - coords[1]) < bitmap.Height ? list[i][j * 4 + 3] - coords[1] : bitmap.Height - 1,
                    WordColor
                    );
                }
                fp.Unlock(true);
                bitmaps.Add(bitmap);
            }
            return bitmaps;
        }

        public Bitmap GetLine(int line, int[] lineCoordinates = null)
        {
            if (!success || version != 4) return null;
            List<int[]> list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM lines JOIN ayat ON lines.ayah_id=ayat.id WHERE page={PageNumber} AND line={line}";
            quran.Open();
            reader = command.ExecuteReader();
            while(reader.Read())
                list.Add(new int[4] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3) });
            reader.Close(); quran.Close();
            if (list.Count == 0) return null;

            int[] final = list[0].ToArray(); // copy
            for(int i = 1; i < list.Count; i++)
            {
                if (list[i][0] < final[0]) final[0] = list[i][0];
                if (list[i][1] > final[1]) final[1] = list[i][1];
                if (list[i][2] < final[2]) final[2] = list[i][2];
                if (list[i][3] > final[3]) final[3] = list[i][3];
            }
            if (lineCoordinates?.Length >= 2)
            {
                lineCoordinates[0] = final[0];
                lineCoordinates[1] = final[2];
            }
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
