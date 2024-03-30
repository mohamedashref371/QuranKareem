using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static QuranKareem.Coloring;

namespace QuranKareem
{
    internal class TextQuran
    {
        #region Create The Class
        private bool success = false;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        public static readonly TextQuran Instance = new TextQuran();

        private TextQuran()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
            try
            {
                if (Directory.Exists("fonts"))
                {
                    string[] fontsFiles = Directory.GetFiles("fonts");
                    for (int i = 0; i < fontsFiles.Length; i++)
                        collection.AddFontFile(fontsFiles[i]);
                }
            }
            catch { }
        }
        #endregion

        private int quartersCount, pagesCount;
        private int tempInt, tempInt2;

        private int surahsCount;

        public int SurahNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public int AyatCount { get; private set; }

        private int version;

        public int Narration { get; private set; }

        public int QuarterNumber { get; private set; }
        public int PageNumber { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }

        public string Comment { get; private set; }

        private int pageStartId, ayahId;

        string /*fontFile,*/ fontName;

        public int CurrentWord { get; private set; } = -1;
        public bool WordMode { get; set; } = false;

        public bool IsDark { get; private set; } = false;
        public void ChangeDark()
        {
            if (!success) return;
            IsDark = !IsDark;
            if (IsDark)
            {
                pageRichText.ForeColor = Color.White;
                pageRichText.BackColor = Color.Black;
            }
            else
            {
                pageRichText.ForeColor = Color.Black;
                pageRichText.BackColor = Color.White;
            }
            Ayah();
        }

        public string PageText { get; private set; }
        public string PageTextHTML { get; private set; }

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

        private readonly PrivateFontCollection collection = new PrivateFontCollection();
        private readonly MemoryStream stream = new MemoryStream();

        private bool added = false;
        public void AddRichTextBoxInControls(Control.ControlCollection Controls, int locX, int locY, int width, int height)
        {
            if (!added) try
                {
                    pageRichText.Location = new Point(locX, locY);
                    pageRichText.Size = new Size(width > 0 ? width : 10, height > 0 ? height : 10);
                    textType = TextType.rich;
                    pageRichText.SelectAll();
                    pageRichText.SelectionAlignment = HorizontalAlignment.Center;
                    pageRichText.DeselectAll();
                    Controls.Add(pageRichText);
                    added = true;
                }
                catch { }
        }

        public void Start(string file, int sura = 1, int aya = 0)
        {

            if (!File.Exists(file)) return;

            quran.ConnectionString = $"Data Source={file}; Version=3;";
            success = false;
            try
            {
                quran.Open();

                command.CommandText = $"SELECT * FROM description";
                reader = command.ExecuteReader();

                if (!reader.HasRows) return;
                reader.Read();

                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 1 || reader.GetInt32(1) < 2 || reader.GetInt32(1) > 3) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                quartersCount = reader.GetInt32(4);
                pagesCount = reader.GetInt32(5);
                //fontFile = reader.GetString(6);
                fontName = reader.GetString(7);
                Comment = reader.GetString(8);
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
                pageRichText.Font = new Font(collection.Families.FirstOrDefault(f => f.Name == fontName) ?? pageRichText.Font.FontFamily, pageRichText.Font.Size);
                Ayah(sura, aya);
            }
        }

        #region التنقلات في المصحف
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
            reader.Close(); quran.Close();
            return names;
        }

        public void Surah(int i) => Ayah(i, 0);

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
            reader.Close(); quran.Close();
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
            reader.Close(); quran.Close();
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
            reader.Close(); quran.Close();
            Ayah(tempInt, tempInt2); // أحاول تركيز كل المجهود على دالة واحدة
        }

        public void Ayah() => Ayah(SurahNumber, AyahNumber);
        public void Ayah(int aya) => Ayah(SurahNumber, aya);

        public void Ayah(int sura, int aya)
        { // كما ترى .. المجهود كله عليها
            if (!success) return;
            sura = Math.Abs(sura);
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
                reader.Close(); command.Cancel();
            }
            if (aya > AyatCount) aya = AyatCount;
            AyahNumber = aya;
            command.CommandText = $"SELECT id,quarter,page FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            AyahStart = 0;
            if (!reader.HasRows && aya == 0)
            {
                reader.Close(); command.Cancel();
                command.CommandText = $"SELECT id,quarter,page FROM ayat WHERE surah={sura} AND ayah={1}";
                reader = command.ExecuteReader();
                AyahNumber = 1;
                AyahStart = 1;
            }
            reader.Read();
            ayahId = reader.GetInt32(0);
            QuarterNumber = reader.GetInt32(1);

            if (PageNumber != reader.GetInt32(2))
            {
                PageNumber = reader.GetInt32(2);
                reader.Close(); command.Cancel();
                command.CommandText = $"SELECT * FROM pages WHERE id={PageNumber}";
                reader = command.ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                reader.Close(); command.Cancel();
                PageTextAt(PageNumber);
                pageRichText.Text = "";
            }

            reader.Close(); quran.Close();
            CurrentWord = -1;
            PageText = originalPageText.ToString();

            // التلوين
            int k = ayahId - pageStartId;
            int start = 0, finish;
            if (k > 0) start = finishedPosition[k - 1];
            finish = finishedPosition[k];

            if (textType == TextType.html)
            {
                if (AyahColor.A != 0)
                {
                    PageTextHTML = (start > 0 ? PageText.Substring(0, start) : "") + GetHtmlTextColor() + PageText.Substring(start, finish - start) + "</span>" + PageText.Substring(finish);
                }
                PageTextHTML = "<span dir=\"rtl\">" + PageText.Replace(Environment.NewLine, "<br>") + "</span>";
            }
            else if (textType == TextType.rich)
            {
                if (pageRichText.Text == "")
                {
                    pageRichText.Text = originalPageText.ToString();
                    for (int i = 0; i < pageRichText.Text.Length; i++)
                    {
                        if (others.Contains(pageRichText.Text[i]))
                        {
                            pageRichText.Select(i, 1);
                            pageRichText.SelectionColor = AyahEndColor;
                        }
                    }
                    stream.Position = 0;
                    pageRichText.SaveFile(stream, RichTextBoxStreamType.RichText);
                }
                else
                {
                    stream.Position = 0;
                    pageRichText.LoadFile(stream, RichTextBoxStreamType.RichText);
                }
                if (AyahColor.A != 0)
                {
                    pageRichText.Select(start, finish - start);
                    pageRichText.SelectionColor = AyahColor;
                    pageRichText.SelectionStart = start;
                }
                pageRichText.DeselectAll();
            }
        }

        private readonly List<int> finishedPosition = new List<int>();
        private readonly List<List<int>> wordsPosition = new List<List<int>>();

        #region Part Of Ayah Function
        private readonly StringBuilder originalPageText = new StringBuilder();
        private readonly char[] characters = { 'ا', 'ى', 'ٱ', 'آ', 'أ', 'إ', 'ء', 'ؤ', 'ئ', 'ب', 'ت', 'ة', 'ث', 'ج', 'ح', 'خ', 'د', 'ذ', 'ر', 'ز', 'س', 'ش', 'ص', 'ض', 'ط', 'ظ', 'ع', 'غ', 'ف', 'ق', 'ك', 'ل', 'م', 'ن', 'ه', 'و', 'ي' },
                                decorations = { 'ُ', 'َ', 'ِ', 'ۡ', 'ّ', 'ٰ', 'ٓ', 'ۛ', 'ٗ', 'ْ', 'ۖ', 'ٌ', 'ٞ', 'ۢ', 'ۗ', 'ۥ', 'ٖ', 'ۚ', 'ۦ', 'ۘ', 'ٍ', 'ـ', 'ٔ', 'ً', 'ۭ', 'ۧ', 'ۜ', '۠', 'ۤ', 'ٕ', '۪', '۬', 'ۨ', ' ', ' ', '\n' },
                                others = { '٠', '١', '٢', '٣', '٤', '٥', '٦', '٧', '٨', '٩', '۞', '۩' };

        private void PageTextAt(int i)
        {
            originalPageText.Clear();
            finishedPosition.Clear(); int length = 0;
            wordsPosition.Clear();
            string s;

            command.CommandText = $"SELECT text FROM ayat WHERE page={i}";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                s = reader.GetString(0).Replace(Environment.NewLine, "\n");
                originalPageText.Append(s);
                length += s.Length;
                finishedPosition.Add(length);
            }
            reader.Close(); command.Cancel();

            if (WordMode) AyatWords(i);
        }

        private void AyatWords(int page)
        {
            command.CommandText = $"SELECT aya.id,pure_text FROM (SELECT id FROM ayat WHERE page={page}) AS aya INNER JOIN words ON aya.id = words.ayah_id";
            int id = pageStartId - 1; int length = 0;
            string s;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) > id)
                {
                    for (int k = 1; k < reader.GetInt32(0) - id; k++) wordsPosition.Add(null);
                    id = reader.GetInt32(0);
                    if (wordsPosition.Count >= 1) length = finishedPosition[wordsPosition.Count - 1];
                    wordsPosition.Add(new List<int>());
                    if (originalPageText[length] == others[10]) length += 1;
                    wordsPosition.Last().Add(length);
                }
                s = reader.GetString(1);
                //مقارنة الكلمة
                for (int j = 0; j < s.Length;)
                {
                    if (!decorations.Contains(originalPageText[length]) && originalPageText[length] == s[j])
                    {
                        length++; j++;
                    }
                    else length++;
                }
                while (length < originalPageText.Length)
                    if (decorations.Contains(originalPageText[length])) length++;
                    else break;

                wordsPosition.Last().Add(length);
            }
            reader.Close(); command.Cancel();
        }
        #endregion

        public void WordOf(int word)
        {
            try
            {
                if (textType == TextType.rich && word > 0 && WordColor.A != 0 && ayahId - pageStartId < wordsPosition.Count && wordsPosition[ayahId - pageStartId] != null)
                {
                    pageRichText.Select(wordsPosition[ayahId - pageStartId][word - 1], wordsPosition[ayahId - pageStartId][word] - wordsPosition[ayahId - pageStartId][word - 1]);
                    pageRichText.SelectionColor = WordColor;
                    pageRichText.DeselectAll();
                }
                CurrentWord = word;
            }
            catch { }
        }

        public bool SetCursor(int position = -1)
        {
            if (!success) return false;
            if (position < 0) position = pageRichText.SelectionStart;
            tempInt = -1; int i = 0;
            while (i < finishedPosition.Count)
            {
                if (position < finishedPosition[i])
                {
                    AyahAt(i + pageStartId);
                    break;
                }
                i++;
            }
            if (tempInt != -1 && WordMode && i < wordsPosition.Count && wordsPosition[i] != null)
            {
                for (int j = 1; j < wordsPosition[i].Count; j++)
                {
                    if (position < wordsPosition[i][j])
                    {
                        WordOf(j); break;
                    }
                }
            }
            return tempInt != -1;
        }
        #endregion

        private string tempString;
        public string AyahText(int sura, int aya)
        {
            if (!success) return "";
            quran.Open();
            command.CommandText = $"SELECT text FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            reader.Read();
            tempString = reader.GetString(0);
            reader.Close(); quran.Close();
            return tempString.Replace(Environment.NewLine, " ").Trim();
        }

        public string AyahAbstractText(int sura, int aya)
        {
            if (!success) return "";
            quran.Open();
            command.CommandText = $"SELECT abstract_text FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            reader.Read();
            tempString = reader.GetString(0);
            reader.Close(); quran.Close();
            return tempString;
        }

        public string[] SurahAbstractTexts(int sura, int wordsCount = -1, bool end = false)
        {
            if (!success) return null;
            lst.Clear();
            quran.Open();
            command.CommandText = $"SELECT ayah,abstract_text FROM ayat WHERE surah={sura}";
            reader = command.ExecuteReader();
            string[] words;
            while (reader.Read())
            {
                tempString = reader.GetString(1);
                words = tempString.Split(' ');
                if (wordsCount >= 1 && wordsCount < words.Length)
                    if (end) tempString = string.Join(" ", words, words.Length - wordsCount, wordsCount);
                    else tempString = string.Join(" ", words, 0, wordsCount);
                tempString += " ";
                if (reader.GetInt32(0) > 0) tempString += reader.GetInt32(0);
                lst.Add(tempString);
            }
            reader.Close(); quran.Close();
            return lst.ToArray();
        }

        private readonly List<int> SearchIDs = new List<int>();
        readonly List<string> lst = new List<string>();
        public string[] Search(string words, bool spellingErrors = false)
        {
            SearchIDs.Clear();
            if (!success || words == null || words.Trim() == "") return new string[] { };
            if (spellingErrors) words = words.Replace(" ", "")
                    .Replace("ى", "ا").Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا").Replace("ئ", "ا").Replace("ء", "ا").Replace("ؤ", "ا")
                    .Replace("ذ", "ز").Replace("ظ", "ز")
                    .Replace("ة", "ت").Replace("ط", "ت")
                    .Replace("ث", "س").Replace("ص", "س")
                    .Replace("ض", "د")
                    .Replace("ق", "ك");
            lst.Clear();
            quran.Open();
            command.CommandText = "SELECT id,surah,ayah,abstract_text FROM ayat";
            reader = command.ExecuteReader();
            string s;
            while (reader.Read())
            {
                s = reader.GetString(3);
                if (spellingErrors) s = s.Replace(" ", "")
                        .Replace("ى", "ا").Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا").Replace("ئ", "ا").Replace("ء", "ا").Replace("ؤ", "ا")
                        .Replace("ذ", "ز").Replace("ظ", "ز")
                        .Replace("ة", "ت").Replace("ط", "ت")
                        .Replace("ث", "س").Replace("ص", "س")
                        .Replace("ض", "د")
                        .Replace("ق", "ك");
                if (s.Contains(words))
                {
                    s = reader.GetString(3);
                    lst.Add($"{(s.Length > 50 ? s.Substring(0, 50) + "..." : s)}");
                    SearchIDs.Add(reader.GetInt32(0));
                }
            }
            reader.Close(); quran.Close();
            return lst.ToArray();
        }
        public int[] SelectedSearchIndex(int i)
        {
            if (!success || SearchIDs.Count == 0 || i < 0 || i >= SearchIDs.Count) return null;
            int[] sura_aya = new int[2];
            quran.Open();
            command.CommandText = $"SELECT surah,ayah FROM ayat WHERE id={SearchIDs[i]}";
            reader = command.ExecuteReader();
            reader.Read();
            sura_aya[0] = reader.GetInt32(0);
            sura_aya[1] = reader.GetInt32(1);
            reader.Close(); quran.Close();
            return sura_aya;
        }

        private string GetHtmlTextColor()
        {
            string s = "<span";
            if (AyahColor.A != 0) s += $" style=\"color: rgb({AyahColor.R},{AyahColor.G},{AyahColor.B});\"";
            s += ">";
            return s;
        }

        public void AddEventHandler(EventHandler eh) { pageRichText.Click += eh; }

        public TextType textType = TextType.plain;
        public enum TextType
        {
            plain = 0, html = 1, rich = 2
        }
    }
}
