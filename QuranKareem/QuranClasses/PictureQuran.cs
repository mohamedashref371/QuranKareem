using System;
using System.Data.SQLite;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using static QuranKareem.Coloring;
using System.Text;
using System.Runtime.ConstrainedExecution;

namespace QuranKareem
{
    internal class PictureQuran
    {
        #region Create The Class
        private bool success = false;
        private string path;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        public static readonly PictureQuran Instance = new PictureQuran();

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

        #region Bitmaps
        private SharpPixelQK spQuranPicture;
        public Bitmap CurrentPicture { get; private set; }
        public Bitmap PagePicture { get; private set; }
        #endregion

        #region Words
        public int CurrentWord { get; private set; } = -1;
        private int wordsCount = 0;
        private bool isWordTableEmpty = true;
        private bool isWordsDiscriminatorEmpty = false;
        #endregion

        private readonly StringBuilder str = new StringBuilder();

        private bool darkMode = false;
        public bool DarkMode
        {
            get => darkMode;
            set
            {
                if (success && value != darkMode)
                {
                    darkMode = value;
                    PageNumber = 0;
                    if (background.A != 0) background = Color.FromArgb(background.A, 255 - background.R, 255 - background.G, 255 - background.B);
                    if (!textColor.IsEmpty) textColor = Color.FromArgb(255 - background.R, 255 - background.G, 255 - background.B);
                    isWordsDiscriminatorEmpty = !Discriminators.ActiveDiscriminators(darkMode);
                    Set();
                }
            }
        }

        // الدالة البداية
        public bool Start(string path, int sura = 1, int aya = 0)
        {

            if (path == null || path.Trim().Length == 0) return false;
            if (path.Substring(path.Length - 1) != "\\") path += "\\";

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return false;
            this.path = path; success = false;

            try
            {
                if (File.Exists(path + "000.db"))
                    quran.ConnectionString = $"Data Source={path}000.db;Version=3;";
                else
                    quran.ConnectionString = $"Data Source={path}0.db;Version=3;";

                quran.Open();
                command.CommandText = $"SELECT * FROM description";
                reader = command.ExecuteReader();

                if (!reader.HasRows) return false;
                reader.Read();
                version = reader.GetInt32(1);
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 2 || version != 4) return false;
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
                reader.Close();

                PageNumber = 0;

                command.CommandText = $"SELECT * FROM words LIMIT 1";
                reader = command.ExecuteReader();
                isWordTableEmpty = !reader.HasRows;

                success = true;
            }
            catch { }
            finally
            {
                reader?.Close();
                quran.Close();
            }
            if (success)
            {
                DiscriminatorsReader();
                Discriminators.GetDiscriminators(path + "Colors.txt");
                isWordsDiscriminatorEmpty = !Discriminators.ActiveDiscriminators(darkMode);
                CurrentPicture = new Bitmap(Width, Height);
                PagePicture = CurrentPicture;
                GetInitialColors();
                Set(sura, aya);
            }
            return success;
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
        /// <summary>
        /// Implementation Priority:<br/>
        ///   1- Surah (1:114), its Ayah (0:{AyatCount}).<br/>
        ///   1- Ayah (0:{AyatCount}) only.<br/>
        ///   2- Next parameter (false,true).<br/>
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
            str.Length = 0;
            str.Append("SELECT id,surah,quarter,page,ayah FROM ayat WHERE ");
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

                if (ayah < 0) ayah = 0;
                str.Append($"surah >= {surah} AND ayah >= {ayah}");
            }

            // Surah and Ayah
            else if (surah >= 1 && surah <= SurahsCount + 1)
            {
                if (surah == SurahsCount + 1) surah = 1;
                if (ayah < 0) ayah = 0;
                str.Append($"surah >= {surah} AND ayah >= {ayah}");
            }

            // Ayah Plus
            else if (next && SurahNumber > 0)
            {
                if (SurahNumber == SurahsCount && AyahNumber == AyatCount)
                    str.Append("ayah >= 0");
                else
                    str.Append($"id > {ayahId} AND ayah >= 0");
            }
            #endregion
            #region Juz, Hizb, Quarter and Page
            // Juz then Hizb and Quarter
            else if (juz >= 1 && juz <= 31)
            {
                if (juz == 31) juz = 1;
                if (quarter <= 0 || quarter > 8) quarter = 1;
                if (hizb == 2 && quarter >= 1 && quarter <= 4) quarter += 4;

                str.Append($"quarter >= {juz * 8 - 8 + quarter} AND ayah >= 0");
            }

            // Hizb then Quarter
            else if (hizb >= 1 && hizb <= 61)
            {
                if (hizb == 61) hizb = 1;
                if (quarter <= 0 || quarter > 4) quarter = 1;

                str.Append($"quarter >= {hizb * 4 - 4 + quarter} AND ayah >= 0");
            }

            // Quarter only
            else if (quarter >= 1 && quarter <= 241)
            {
                if (quarter == 241) quarter = 1;
                str.Append($"quarter >= {quarter} AND ayah >= 0");
            }

            // Page
            else if (page >= 1 && page <= PagesCount + 1)
            {
                if (page == PagesCount + 1) page = 1;
                str.Append($"page >= {page} AND ayah >= 0");
            }
            #endregion
            else
            {
                str.Append($"surah >= {SurahNumber} AND ayah >= {AyahNumber}");
            }
            str.Append(" LIMIT 1");
            #endregion

            #region SQL Execution
            quran.Open();
            command.CommandText = str.ToString();
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
            CurrentWord = -1;

            AyahDecorations(Discriminators.AyahColors.Count > 0 || !isWordTableEmpty);

            if (!isWordTableEmpty)
            {
                quran.Open();
                command.CommandText = $"SELECT word FROM words WHERE ayah_id={ayahId} AND word<=599 ORDER BY word DESC LIMIT 1";
                reader = command.ExecuteReader();
                if (reader.Read())
                    wordsCount = reader.GetInt32(0);
                else
                    wordsCount = 0;
                reader.Close(); quran.Close();
            }
        }

        private void PictureAt(int page) // Part Of Ayah Function // الصورة الحالية
        {
            PageNumber = page;

            CurrentPicture = CatchPicture(page);
            spQuranPicture = new SharpPixelQK(CurrentPicture, true);

            if (Discriminators.PageColors.Count > 0 && !isWordTableEmpty)
                PageDecorations();
            else if (darkMode && isWordTableEmpty)
            {
                spQuranPicture.Lock();
                spQuranPicture.ReverseColors();
                spQuranPicture.Unlock(true, true);
            }

            PagePicture = (Bitmap)CurrentPicture.Clone();
        }

        private Bitmap CatchPicture(int page)
        {
            string s = page + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += Extension;

            if (File.Exists(path + s)) return new Bitmap(path + s);
            else if (File.Exists($"{path}{page}{Extension}")) return new Bitmap($"{path}{page}{Extension}");
            else
            {
                string[] filesName = Directory.GetFiles(path);
                for (int i = 0; i < filesName.Length; i++)
                {
                    if (filesName[i].Split('\\').Last().Contains(page.ToString().PadLeft(3, '0')))
                        return new Bitmap(filesName[i]);
                }
            }
            return CurrentPicture;
        }
        #endregion

        public void WordOf(int word)// سيتم تعديله ان شاء الله
        {
            if (isWordTableEmpty || isWordsDiscriminatorEmpty || word <= 0 || word > wordsCount)
            {
                CurrentWord = -1;
                return;
            }
            CurrentWord = word;
            WordDecorations();
        }

        public bool SetXY(int xMouse, int yMouse) => SetXY(xMouse, yMouse, Width, Height);
        public bool SetXY(int xMouse, int yMouse, int width, int height)
        { // مؤشر الماوس
            if (!success) return false;
            xMouse = (int)(xMouse * (Width / (decimal)width)); // تصحيح المؤشر إذا كان عارض الصورة ليس بنفس عرض الصورة نفسها
            yMouse = (int)(yMouse * (Height / (decimal)height)) + 1;
            int word = -1; int tempInt = -371, tempInt2 = 0;
            quran.Open();
            bool words = !isWordTableEmpty;
            if (words)
            {
                command.CommandText = $"SELECT surah,ayah,word FROM ayat JOIN words ON ayat.id = words.ayah_id WHERE page={PageNumber} AND min_x<={xMouse} AND max_x>={xMouse} AND min_y<={yMouse} AND max_y>={yMouse}";
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
                command.CommandText = $"SELECT surah,ayah FROM ayat JOIN lines ON ayat.id = lines.ayah_id WHERE page={PageNumber} AND min_x<={xMouse} AND max_x>={xMouse} AND min_y<={yMouse} AND max_y>={yMouse}";
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

        #region Discriminators Methods
        private void PageDecorations()
        {
            spQuranPicture.Lock();

            quran.Open();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y,discriminator FROM words JOIN ayat ON words.ayah_id = ayat.id WHERE discriminator IN ({Discriminators.GetPageKeysAsString()}) AND page={PageNumber}";
            reader = command.ExecuteReader();

            while (reader.Read())
                spQuranPicture.Clear(Discriminators.PageColors[reader.GetInt32(4)], reader.GetInt32(0), reader.GetInt32(2), reader.GetInt32(1), reader.GetInt32(3), textColor, 30, false, true);

            reader.Close(); quran.Close();
            spQuranPicture.Unlock(true, true);
        }

        private void AyahDecorations(bool discri = true)
        {
            spQuranPicture.Lock();
            spQuranPicture.SetOriginal();

            quran.Open();
            if (discri)
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y,discriminator FROM words WHERE discriminator IN ({Discriminators.GetAyahKeysAsString()}) AND ayah_id={ayahId}";
            else
                command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM lines WHERE ayah_id={ayahId}";
            reader = command.ExecuteReader();

            Color clr;
            while (reader.Read())
            {
                if (discri)
                {
                    clr = Discriminators.AyahColors[reader.GetInt32(4)];
                    clr = clr.Name != "AyahColor" ? clr : AyahColor;
                }
                else
                    clr = AyahColor;

                if (!clr.IsEmpty)
                    spQuranPicture.Clear(clr, reader.GetInt32(0), reader.GetInt32(2), reader.GetInt32(1), reader.GetInt32(3), textColor, 30, false, true);
            }

            reader.Close(); quran.Close();
            spQuranPicture.Unlock(true);
        }

        private void WordDecorations()
        {
            spQuranPicture.Lock();
            spQuranPicture.SetOriginal(true);

            quran.Open();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y,discriminator FROM words WHERE discriminator IN ({Discriminators.GetWordKeysAsString()}) AND ayah_id={ayahId} AND word={CurrentWord}";
            reader = command.ExecuteReader();

            Color clr;
            while (reader.Read())
            {
                clr = Discriminators.WordColors[reader.GetInt32(4)];
                if (clr.Name == "WordColor") clr = WordColor;
                if (!clr.IsEmpty)
                    spQuranPicture.Clear(clr, reader.GetInt32(0), reader.GetInt32(2), reader.GetInt32(1), reader.GetInt32(3), textColor, 30, false, true, true);
            }

            reader.Close(); quran.Close();
            spQuranPicture.Unlock(true);
        }
        #endregion

        #region lines images
        public List<Bitmap> GetLines()
        {
            List<Bitmap> bitmaps = new List<Bitmap>();
            for (int i = 1; i <= 15; i++)
                bitmaps.Add(GetLine(PageNumber, PagePicture, i));
            return bitmaps;
        }

        public List<List<Bitmap>> GetLinesWithWordsMarks()
        {
            var bitmaps = new List<List<Bitmap>>();
            for (int i = 1; i <= 15; i++)
                bitmaps.Add(GetLineWithWordsMarks(PageNumber, PagePicture, i));
            return bitmaps;
        }

        private readonly List<int> ints = new List<int>();
        private List<Bitmap> GetLineWithWordsMarks(int page, Bitmap pagePic, int line = -1, int ayah = -1, int word = -2)
        {
            var coords = new int[2];
            Bitmap bmap = GetLine(page, pagePic, line, coords);
            if (bmap == null) return null;
            List<Bitmap> bitmaps = new List<Bitmap> { bmap };
            if (WordColor.IsEmpty || isWordsDiscriminatorEmpty) return bitmaps;
            var list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y,ayah,word,discriminator FROM words JOIN ayat ON words.ayah_id=ayat.id WHERE page={page} AND line={line} AND " + (word == -2 ? "word>=1 AND word<=599 ORDER BY ayah_id,word" : $"ayah={ayah} AND word={word}");
            quran.Open();
            reader = command.ExecuteReader();

            int a, w; ints.Clear();
            if (!reader.Read())
            {
                reader.Close(); quran.Close();
                return bitmaps;
            }
            a = reader.GetInt32(4);
            w = reader.GetInt32(5);

            do
            {
                if (a != reader.GetInt32(4) || w != reader.GetInt32(5))
                {
                    bitmaps.Add((Bitmap)bmap.Clone());
                    bitmaps.Last().Tag = $"{a},{w}";
                    list.Add(ints.ToArray());
                    ints.Clear();
                    a = reader.GetInt32(4);
                    w = reader.GetInt32(5);
                }
                ints.Add(reader.GetInt32(0)); ints.Add(reader.GetInt32(1)); ints.Add(reader.GetInt32(2)); ints.Add(reader.GetInt32(3)); ints.Add(reader.GetInt32(6));
            } while (reader.Read());
            bitmaps.Add((Bitmap)bmap.Clone());
            bitmaps.Last().Tag = $"{a},{w}";
            list.Add(ints.ToArray());

            reader.Close(); quran.Close();

            SharpPixelQK sp;
            Color clr;
            for (int i = 0; i < list.Count; i++)
            {
                sp = new SharpPixelQK(bitmaps[i + 1]);
                sp.Lock();
                for (int j = 0; j < list[i].Length / 5; j++)
                {
                    clr = Discriminators.WordColors[list[i][j * 5 + 4]];
                    clr = clr.Name != "WordColor" && !clr.IsEmpty ? clr : WordColor;
                    sp.Clear(clr,
                        (list[i][j * 5] - coords[0]) >= 0 ? list[i][j * 5] - coords[0] : 0,
                        (list[i][j * 5 + 2] - coords[1]) >= 0 ? list[i][j * 5 + 2] - coords[1] : 0,
                        (list[i][j * 5 + 1] - coords[0]) < bmap.Width ? list[i][j * 5 + 1] - coords[0] : bmap.Width - 1,
                        (list[i][j * 5 + 3] - coords[1]) < bmap.Height ? list[i][j * 5 + 3] - coords[1] : bmap.Height - 1,
                        textColor, 30, false, true);
                }
                sp.Unlock(true);
            }
            return bitmaps;
        }

        private Bitmap GetLine(int page, Bitmap pagePic, int line, int[] lineCoordinates = null)
        {
            if (!success) return null;
            List<int[]> list = new List<int[]>();
            command.CommandText = $"SELECT min_x,max_x,min_y,max_y FROM lines JOIN ayat ON lines.ayah_id=ayat.id WHERE page={page} AND line={line}";
            quran.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
                list.Add(new int[4] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3) });
            reader.Close(); quran.Close();
            if (list.Count == 0) return null;

            int[] final = list[0].ToArray(); // copy
            for (int i = 1; i < list.Count; i++)
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
                gr.DrawImage(pagePic, new Rectangle(list[i][0] - final[0], list[i][2] - final[2], list[i][1] - list[i][0], list[i][3] - list[i][2]), new Rectangle(list[i][0], list[i][2], list[i][1] - list[i][0], list[i][3] - list[i][2]), GraphicsUnit.Pixel);

            return bitmap;
        }

        public int[] GetStartAndEndOfPage()
        {
            command.CommandText = $"SELECT MIN(ayah), MAX(ayah) FROM ayat WHERE surah={SurahNumber} AND page = {PageNumber}";
            int[] ints = new int[2];
            quran.Open();
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                ints[0] = reader.GetInt32(0);
                ints[1] = reader.GetInt32(1);
            }
            reader.Close();
            quran.Close();
            return ints;
        }

#warning very slow
        public void GetAyatInLinesWithWordsMarks(List<int> ayahword, int width, int height, int locx, int locy, int linWdth, int linHght, bool autoHeight, bool yEdit, string path, List<string> paths, Bitmap pageClone, int surah, int page)
        {
            paths.Clear();
            if (!success || isWordTableEmpty || ayahword == null || paths == null) return;

            Directory.CreateDirectory($"{path}\\img\\");
            
            int line = 0;
            Bitmap bmp, bmp0;
            Graphics gr; int h;
            for (int i = 0; i < ayahword.Count / 2; i++)
            {
                quran.Open();
                command.CommandText = $"SELECT line FROM ayat JOIN words ON ayat.id = words.ayah_id WHERE surah={surah} AND ayah = {ayahword[i * 2]} AND word = {ayahword[i * 2 + 1]} LIMIT 1";
                reader = command.ExecuteReader();
                if (reader.Read())
                    line = reader.GetInt32(0);

                reader.Close(); quran.Close();
                bmp = new Bitmap(width, height);
                gr = Graphics.FromImage(bmp);
                gr.Clear(Color.Empty);
                bmp0 = GetLineWithWordsMarks(page, pageClone, line, ayahword[i * 2], ayahword[i * 2 + 1]).Last();
                if (autoHeight)
                {
                    h = (int)(1f * bmp0.Height / bmp0.Width * linWdth);
                    if (yEdit) locy -= h - linHght;
                    linHght = h;
                }
                gr.DrawImage(bmp0, locx, locy, linWdth, linHght);
                bmp.Save($"{path}\\img\\{i}.png", System.Drawing.Imaging.ImageFormat.Png);
                paths.Add($"img\\{i}.png");
            }
        }
        #endregion

        #region Colors
        private void GetInitialColors()
        {
            if (!success) return;
            if (File.Exists(path + "Colors0.txt"))
            {
                string[] arr = File.ReadAllText(path + "Colors0.txt").Replace(" ", "").Split('*');
                AyahColor = GetColor(arr[0]);
                if (arr.Length > 1) WordColor = GetColor(arr[1]);
                else WordColor = Color.Empty;
            }
        }

        public void SetInitialColors()
        {
            if (!success) return;

            str.Length = 0;
            str.Append(GetString(AyahColor));
            str.Append("*");
            str.Append(GetString(WordColor));

            File.WriteAllText(path + "Colors0.txt", str.ToString());
        }

        public void SetDiscriminators()
        {
            if (!success) return;
            Discriminators.SetDiscriminators(path + "Colors.txt");
            Discriminators.ActiveDiscriminators(darkMode);
        }

        private void DiscriminatorsReader()
        {
            Discriminators.Descriptions.Clear();
            quran.Open();
            command.CommandText = $"SELECT id,comment FROM discriminators WHERE enabled=1";
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Discriminators.Descriptions.Add(reader.GetInt32(0), reader.GetString(1));
            }
            reader.Close(); quran.Close();
        }
        #endregion

    }
}
