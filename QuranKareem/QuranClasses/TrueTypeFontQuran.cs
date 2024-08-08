using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuranKareem.Coloring;

namespace QuranKareem
{
    internal class TrueTypeFontQuran
    {
        private bool success = false;
        private string path;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        #region Description
        public int Narration { get; private set; }
        public int SurahsCount { get; private set; }
        public int PagesCount { get; private set; }
        public string Extension { get; private set; }
        public string Comment { get; private set; }
        #endregion

        public int SurahNumber { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyatCount { get; private set; }
        public int QuarterNumber { get; private set; }
        public int PageNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public int CurrentWord { get; private set; } = -1;

        private int wordsCount  = 0;
        private int glyphsCount = 0;
        private int ayahId;

        private bool isWordsDiscriminatorEmpty = false;

        private int width;
        private float fontSize;

        private FontFamily bsml, fPage;

        public static readonly TrueTypeFontQuran Instance = new TrueTypeFontQuran();

        private string pageRtf, ayahRtf;

        private readonly RichTextBox pageRichText = new RichTextBox()
        {
            RightToLeft = RightToLeft.Yes,
            WordWrap = false,
            Font = new Font("Tahoma", 20F),
            BackColor = SystemColors.Control,
            BorderStyle = BorderStyle.None,
            ReadOnly = true,
            //ScrollBars = RichTextBoxScrollBars.None
        };

        private bool darkMode = false;
        public bool DarkMode
        {
            get => darkMode;
            set
            {
                if (success && value != darkMode)
                {
                    darkMode = value;
                    if (value)
                    {
                        pageRichText.ForeColor = Color.White;
                        pageRichText.BackColor = Color.Black;
                    }
                    else
                    {
                        pageRichText.ForeColor = Color.Black;
                        pageRichText.BackColor = Color.White;
                    }
                    PageNumber = 0;
                    Set(SurahNumber, AyahNumber);
                }
            }
        }

        private bool added = false;
        public void AddRichTextBoxInControls(Control.ControlCollection Controls, int locX, int locY, int width, int height)
        {
            if (!added) try
                {
                    pageRichText.Location = new Point(locX, locY);
                    pageRichText.Size = new Size(width, height);
                    pageRichText.SelectAll();
                    pageRichText.SelectionAlignment = HorizontalAlignment.Center;
                    pageRichText.DeselectAll();
                    Controls.Add(pageRichText);
                    added = true;
                }
                catch { }
        }

        private TrueTypeFontQuran()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        public void Start(string path, int sura = 1, int aya = 0)
        {
            if (path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") path += "\\";

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;
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

                if (!reader.HasRows) return;
                reader.Read();
                if (reader.GetInt32(0) != 5 || reader.GetInt32(1) != 1) return;
                Narration = reader.GetInt32(2);
                SurahsCount = reader.GetInt32(3);
                PagesCount = reader.GetInt32(5);
                width = reader.GetInt32(6);
                fontSize = reader.GetFloat(7);
                Extension = reader.GetString(8);
                Comment = reader.GetString(9);
                reader.Close();

                PageNumber = 0;

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
                bsml = CatchFontFile(0);
                if (bsml.Name == "Tahoma") BsmlNotFound();
                DiscriminatorsReader();
                Discriminators.GetDiscriminators(path + "Colors.txt");
                isWordsDiscriminatorEmpty = !Discriminators.ActiveDiscriminators(darkMode);
                GetInitialColors();
                Set(sura, aya);
            }
        }

        private void BsmlNotFound()
        {
            PrivateFontCollection collection = new PrivateFontCollection();
            string[] filesName = Directory.GetFiles(path);
            string name;
            for (int i = 0; i < filesName.Length; i++)
            {
                name = filesName[i].Split('\\').Last();
                if (name.IndexOf("bsml", StringComparison.OrdinalIgnoreCase) >= 0 && name.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                {
                    collection.AddFontFile(filesName[i]);
                    break;
                }
            }

            if (collection.Families.Length != 0)
                bsml = collection.Families[0];

            collection.Dispose();
        }

        public string[] GetSurahNames()
        {
            if (!success) return new string[] { "" };
            string[] names = new string[SurahsCount];
            quran.Open();
            command.CommandText = $"SELECT id,name FROM surahs";
            reader = command.ExecuteReader();

            while (reader.Read())
                names[reader.GetInt32(0) - 1] = reader.GetString(1);
           
            reader.Close(); quran.Close();
            return names;
        }

        private bool AyahAt(int id)
        {
            quran.Open();
            command.CommandText = $"SELECT id,surah,quarter,page,ayah,words_count,glyphs_count FROM ayat WHERE id >= {id} AND ayah >= 0 LIMIT 1";
            reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close(); quran.Close();
                return false;
            }
            ayahId = reader.GetInt32(0);
            int surah = reader.GetInt32(1);
            QuarterNumber = reader.GetInt32(2);
            AyahNumber = reader.GetInt32(4);
            wordsCount = reader.GetInt32(5);
            glyphsCount = reader.GetInt32(6);
            reader.Close(); quran.Close();

            if (surah != SurahNumber) SurahData(surah);
            AyahData();

            return true;
        }

        readonly StringBuilder str = new StringBuilder();
        public bool Set(int surah = 0, int ayah = -2, bool next = false, int juz = 0, int hizb = 0, int quarter = 0, int page = 0)
        {
            if (!success) return false;

            #region SQL Building
            str.Length = 0;
            str.Append("SELECT id,surah,quarter,page,ayah,words_count,glyphs_count FROM ayat WHERE ");
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
            else if (next && SurahNumber >= 1)
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
            wordsCount = reader.GetInt32(5);
            glyphsCount = reader.GetInt32(6);
            reader.Close(); quran.Close();
            #endregion

            if (surah != SurahNumber) SurahData(surah);
            if (page != PageNumber) PageData(page);
            AyahData();

            return true;
        }

        private void SurahData(int surah)
        {
            SurahNumber = surah;
            command.CommandText = $"SELECT makya_madanya,ayat_count FROM surahs WHERE id = {surah}";
            quran.Open();
            reader = command.ExecuteReader();
            reader.Read();
            Makya_Madanya = reader.GetBoolean(0);
            AyatCount = reader.GetInt32(1);
            reader.Close(); quran.Close();
        }

        private readonly List<int[]> pageWords = new List<int[]>();
        private void PageData(int page)
        {
            PageNumber = page;
            pageRichText.Text = "";
            pageWords.Clear();

            fPage = CatchFontFile(page);

            pageRichText.Font = new Font(fPage, fontSize, GraphicsUnit.Pixel);
            Font fBsml = new Font(bsml, fontSize, GraphicsUnit.Pixel);

            command.CommandText = $"SELECT ayah_id,ayah,line,word,discriminator,text FROM ayat JOIN words ON words.ayah_id = ayat.id WHERE page = {page}";
            int line = 1, index;
            string s; Color clr;
            int discri;
            quran.Open();
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                s = "";
                index = pageRichText.Text.Length;
                if (line != reader.GetInt32(2))
                {
                    s = "\n";
                    pageWords.Add(null);
                }
                discri = reader.GetInt32(4);
                pageWords.Add(new int[3] { reader.GetInt32(0), reader.GetInt32(3), discri });
                s += reader.GetString(5);
                pageRichText.AppendText(s);
                if (reader.GetInt32(1) <= 0)
                {
                    pageRichText.Select(index, s.Length);
                    pageRichText.SelectionFont = fBsml;
                    pageRichText.DeselectAll();
                }
                if (Discriminators.KeyExists(0, discri))
                {
                    clr = Discriminators.PageColors[discri];
                    if (!clr.IsEmpty)
                    {
                        pageRichText.Select(index, s.Length);
                        pageRichText.SelectionColor = clr;
                        pageRichText.DeselectAll();
                    }
                }
            }
            pageWords.Add(null);
            reader.Close(); quran.Close();
            pageRtf = pageRichText.Rtf;
        }

        private FontFamily CatchFontFile(int page)
        {
            string s = page.ToString().PadLeft(3, '0');
            PrivateFontCollection collection = new PrivateFontCollection();

            if (File.Exists($"{path}{s}{Extension}")) collection.AddFontFile($"{path}{s}{Extension}");
            else if (File.Exists($"{path}{page}{Extension}")) collection.AddFontFile($"{path}{page}{Extension}");
            else
            {
                string[] filesName = Directory.GetFiles(path);
                string name;
                for (int i = 0; i < filesName.Length; i++)
                {
                    name = filesName[i].Split('\\').Last();
                    if (name.Contains(s) && name.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        collection.AddFontFile(filesName[i]);
                        break;
                    }
                }
            }

            FontFamily ff;
            if (collection.Families.Length != 0)
                ff = collection.Families[0];
            else if (fPage != null)
                ff = fPage;
            else
                ff = new FontFamily("Tahoma");

            collection.Dispose();
            return ff;
        }

        private int ayahIdIndex = 0;
        private void AyahData()
        {
            pageRichText.Rtf = pageRtf;
            CurrentWord = -1;
            int index = pageWords.FindIndex(arr => arr[0] == ayahId);
            ayahIdIndex = index;
            Color clr;
            for (; index < pageWords.Count; index++)
            {
                if (pageWords[index] == null)
                    continue;
                else if (pageWords[index][0] != ayahId)
                    break;
                else if (!Discriminators.KeyExists(1, pageWords[index][2]))
                    continue;
                clr = Discriminators.AyahColors[pageWords[index][2]];
                if (clr.Name == "AyahColor") clr = AyahColor;
                if (!clr.IsEmpty)
                {
                    pageRichText.Select(index, 1);
                    pageRichText.SelectionColor = clr;
                    pageRichText.DeselectAll();
                }
            }
            ayahRtf = pageRichText.Rtf;
        }

        public bool WordOf(int word)
        {
            pageRichText.Rtf = ayahRtf;
            CurrentWord = -1;
            if (isWordsDiscriminatorEmpty && word > 0 && word <= wordsCount)
            {
                int index = pageWords.FindIndex(ayahIdIndex, arr => arr[1] == word);
                if (index == -1) return false;
                CurrentWord = word;
                Color clr;
                for (; index < pageWords.Count; index++)
                {
                    if (pageWords[index] == null)
                        continue;
                    else if (pageWords[index][0] != ayahId || pageWords[index][1] != word)
                        break;
                    else if (!Discriminators.KeyExists(2, pageWords[index][2]))
                        continue;

                    clr = Discriminators.WordColors[pageWords[index][2]];
                    if (clr.Name == "WordColor") clr = WordColor;
                    if (!clr.IsEmpty)
                    {
                        pageRichText.Select(index, 1);
                        pageRichText.SelectionColor = clr;
                        pageRichText.DeselectAll();
                    }
                }
                return true;
            }
            return false;
        }

        public bool SetCursor(int position = -1)
        {
            if (!success) return false;
            if (position < 0) position = pageRichText.SelectionStart;
            if (position > pageWords.Count) return false;

            int[] current = pageWords[position] ?? pageWords[position - 1];
            if (!AyahAt(current[0]))
                return false;

            return WordOf(current[1]);
        }

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
