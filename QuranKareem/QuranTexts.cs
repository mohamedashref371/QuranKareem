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

namespace QuranKareem
{
    class QuranTexts {

        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranText ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount, quartersCount, pagesCount;
        private int pageStartId, ayahId;

        public int Narration { get; private set; }
        public int Surah { get; private set; }
        public int Quarter { get; private set; }
        public int Page { get; private set; }
        public int Ayah { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }
        string fontFile,fontName, comment;

        public string PageText { get; private set; }
        public string PageTextHTML { get; private set; }
        public RichTextBox PageRichText { get; private set; } = new RichTextBox();

        private List<int> finishedPosition = new List<int>();

        public static QuranTexts Instance { get; private set; } = new QuranTexts();

        private bool added = false;
        public void AddRichTextBoxInControls(Control.ControlCollection Controls, int locX, int locY, int width, int height) {
            if (!added) try {
                    PageRichText.Location = new Point(locX, locY);
                    PageRichText.Size = new Size(width > 0 ? width : 10, height > 0 ? height : 10);
                    PageRichText.RightToLeft = RightToLeft.Yes;
                    PageRichText.WordWrap = false;
                    textType = TextType.rich;
                    Controls.Add(PageRichText);
                    added = true;
            } catch { }
        }

        public void QuranText(string file, int sura = 1, int aya = 0){
            
            if (file == null || file.Trim().Length == 0) return;

            quran = new SQLiteConnection($"Data Source={file}; Version=3;"); // SQLite Connection
            success = false;
            quran.Open();

            reader = new SQLiteCommand($"SELECT * FROM description", quran).ExecuteReader();
            if (!reader.HasRows) return;
            reader.Read();

            if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 1 || reader.GetInt32(1)/*version*/ != 1) return;
            Narration = reader.GetInt32(2); // العمود الثالث
            surahsCount = reader.GetInt32(3);
            quartersCount = reader.GetInt32(4);
            pagesCount = reader.GetInt32(5);
            fontFile = reader.GetString(7);
            fontName = reader.GetString(8);
            comment = reader.GetString(9);
            success = true;
            quran.Close();

            try {
                var collection = new PrivateFontCollection();
                collection.AddFontFile(@"fonts\" + fontFile);
                PageRichText.Font = new Font(new FontFamily(fontName, collection), 20F);
            } catch { }

            if(added) ayah(sura, aya);
        }

        public string[] GetSurahNames() {
            if (!success) return new string[] { "" };
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
            reader = new SQLiteCommand($"SELECT id,surah,ayah FROM ayat WHERE id={reader.GetInt32(1)}", quran).ExecuteReader();
            reader.Read();
            int temp = reader.GetInt32(1)/*surah*/, temp1 = reader.GetInt32(2)/*ayah*/;
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
            reader = new SQLiteCommand($"SELECT id,surah,ayah FROM ayat WHERE id={reader.GetInt32(1)}", quran).ExecuteReader();
            reader.Read();
            int temp = reader.GetInt32(1), temp1 = reader.GetInt32(2);
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
            reader = new SQLiteCommand($"SELECT id,quarter,page FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            AyahStart = 0;
            if (!reader.HasRows && aya == 0) { reader = new SQLiteCommand($"SELECT id,quarter,page FROM ayat WHERE surah={sura} AND ayah={1}", quran).ExecuteReader(); Ayah = 1; AyahStart = 1; }
            reader.Read();
            ayahId = reader.GetInt32(0);
            Quarter = reader.GetInt32(1);

            if (Page != reader.GetInt32(2)) {
                Page = reader.GetInt32(2);
                reader = new SQLiteCommand($"SELECT * FROM pages WHERE id={Page}", quran).ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                pageText(Page);
            }
            quran.Close();

            PageText = OriginalPageText.ToString();

            // التلوين
            int k = ayahId - pageStartId;
            int start = 0, finish;
            if (k > 0) start = finishedPosition[k - 1];
            finish = finishedPosition[k];

            if (textType == TextType.html) {
                if (ayahColor != AyahColor.nothing) {
                    PageTextHTML = (start > 0 ? PageText.Substring(0, start) : "") + GetHtmlTextColor() + PageText.Substring(start, finish - start) + "</span>" + PageText.Substring(finish);
                }
                PageTextHTML = "<span dir=\"rtl\">" + PageText.Replace(newLine, "<br>") + "</span>";
            }
            else if (textType == TextType.rich) {
                PageRichText.Text = OriginalPageText.ToString();
                if (ayahColor != AyahColor.nothing) {
                    PageRichText.Select(start, finish-start);
                    PageRichText.SelectionColor = GetRichTextColor();
                    PageRichText.SelectionStart = start;
                }
            }

        }

        public static readonly string newLine = @"
";
        private StringBuilder OriginalPageText = new StringBuilder();
        private void pageText(int i) {
            OriginalPageText.Clear();
            finishedPosition.Clear();
            reader = new SQLiteCommand($"SELECT id,line,finished_position,text FROM ayat WHERE page={i}", quran).ExecuteReader();

            while (reader.Read()) {
                OriginalPageText.Append(reader.GetString(3));
                finishedPosition.Add(reader.GetInt32(2) - (textType == TextType.rich? reader.GetInt32(1)+1 :0));

            }
        }

        public void setCursor(int position=-1) {
            if (!success) return;
            if (position < 0) position = PageRichText.SelectionStart;
            int temp=-1, temp1=0;
            for (int i=0; i< finishedPosition.Count; i++) {
                if (position< finishedPosition[i]) {
                    quran.Open();
                    reader = new SQLiteCommand($"SELECT id,surah,ayah FROM ayat WHERE id={i + pageStartId}", quran).ExecuteReader();
                    reader.Read();
                    temp = reader.GetInt32(1); temp1 = reader.GetInt32(2);
                    quran.Close();
                    break;
                }
            }
            if (temp!=-1) ayah(temp, temp1);
            PageRichText.DeselectAll();
        }

        private string tempString;
        public string ayahText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            reader = new SQLiteCommand($"SELECT text FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            reader.Read();
            tempString = reader.GetString(0);
            quran.Close();
            return tempString;
        }

        public string ayahAbstractText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            reader = new SQLiteCommand($"SELECT abstract_text FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            reader.Read();
            tempString = reader.GetString(0);
            quran.Close();
            return tempString;
        }

        public string[] surahAbstractTexts(int sura, int length=-1){
            if (!success) return null;
            lst.Clear();
            quran.Open();
            reader = new SQLiteCommand($"SELECT abstract_text FROM ayat WHERE surah={sura}", quran).ExecuteReader();
            while (reader.Read()) {
                tempString = reader.GetString(0);
                lst.Add((length==-1 || tempString.Length< length)? tempString : tempString.Substring(0, length));
            }
            quran.Close();
            return lst.ToArray();
        }

        private List<int> SearchIDs = new List<int>();
        List<string> lst = new List<string>();
        public string[] Search(string words) {
            SearchIDs.Clear();
            if (!success || words==null || words.Trim()=="") return new string[] { };
            words = words.Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا");
            lst.Clear();
            quran.Open();
            reader = new SQLiteCommand("SELECT id,abstract_text FROM ayat", quran).ExecuteReader();
            string s;
            while (reader.Read()) {
                s = reader.GetString(1).Replace("أ","ا").Replace("إ", "ا").Replace("آ", "ا");
                if (s.Contains(words)) {
                    s = reader.GetString(1);
                    lst.Add($"{(s.Length>50? s.Substring(0,50)+"..." : s)}");
                    SearchIDs.Add(reader.GetInt32(0));
                }
            }
            quran.Close();
            return lst.ToArray();
        }
        public int[] SelectedSearchIndex(int i) {
            if (!success || SearchIDs.Count == 0 || i<0 || i>= SearchIDs.Count) return null;
            int[] sura_aya = new int[2];
            quran.Open();
            reader = new SQLiteCommand($"SELECT id,surah,ayah FROM ayat WHERE id={SearchIDs[i]}", quran).ExecuteReader();
            reader.Read();
            sura_aya[0] = reader.GetInt32(1);
            sura_aya[1] = reader.GetInt32(2);
            quran.Close();
            return sura_aya;
        }

        public AyahColor ayahColor = AyahColor.nothing;

        private Color GetRichTextColor() {
            if (ayahColor == AyahColor.red) return Color.Red;
            else if (ayahColor == AyahColor.green) return Color.Green;
            else if (ayahColor == AyahColor.blue) return Color.Blue;
            else if (ayahColor == AyahColor.darkCyan) return Color.DarkCyan;
            else if (ayahColor == AyahColor.darkRed) return Color.DarkRed;
            else return Color.Black;
        }

        private string GetHtmlTextColor() {
            string s = "<span";
            if (ayahColor != AyahColor.nothing) s+= " style=\"color: ";
            if (ayahColor == AyahColor.red) s+="red\"";
            else if (ayahColor == AyahColor.green) s += "green\"";
            else if (ayahColor == AyahColor.blue) s += "blue\"";
            else if (ayahColor == AyahColor.darkCyan) s += "darkcyan\"";
            else if (ayahColor == AyahColor.darkRed) s += "darkred\"";
            s += ">";
            return s;
        }

        public void AddEventHandler(EventHandler eh) { PageRichText.Click += eh; }

        public TextType textType = TextType.plain;
        public enum TextType {
            plain = 0, html = 1, rich = 2
        }
    }
}
