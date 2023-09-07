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
    class QuranTexts
    {
        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranText ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال sql select

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

        private bool isHTML=true;
        public string PageText { get; private set; }

        private List<int> finishedPosition = new List<int>();

        public static QuranTexts Instance { get; private set; } = new QuranTexts();

        public FontFamily fontFamily { get; private set; } = null;

        public void QuranText(string path, int sura = 1, int aya = 0){
            
            if (path == null || path.Trim().Length == 0) return;

            quran = new SQLiteConnection($"Data Source={path}; Version=3;"); // SQLite Connection
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
            fontFile = reader.GetString(6);
            fontName = reader.GetString(7);
            comment = reader.GetString(8);
            success = true;
            quran.Close();

            try {
                PrivateFontCollection collection = new PrivateFontCollection();
                collection.AddFontFile(@"fonts\" + fontFile);
                fontFamily = new FontFamily(fontName, collection);
            } catch { }

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
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            AyahStart = 0;
            if (!reader.HasRows && aya == 0) { reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}", quran).ExecuteReader(); Ayah = 1; AyahStart = 1; }
            reader.Read();
            ayahId = reader.GetInt32(0);
            Quarter = reader.GetInt32(2);

            if (Page != reader.GetInt32(3)) {
                Page = reader.GetInt32(3);
                reader = new SQLiteCommand($"SELECT * FROM pages WHERE id={Page}", quran).ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                pageText(Page);
            }
            quran.Close();

            PageText = OriginalPageText.ToString();

            // التلوين
            if (isHTML && textColor != TextColor.nothing) {
                int k = ayahId - pageStartId;
                int start = 0, finish;
                if (k > 0) start = finishedPosition[k - 1];
                finish = finishedPosition[k];
                PageText = (start>0? PageText.Substring(0, start):"") + GetHtmlText() + PageText.Substring(start, finish-start)+"</span>"+ PageText.Substring(finish);
            }
            if (isHTML) { PageText = "<span dir=\"rtl\">" + PageText.Replace(@"
", "<br>") + "</span>"; }

        }

        private StringBuilder OriginalPageText = new StringBuilder();
        private void pageText(int i) {
            OriginalPageText.Clear();
            finishedPosition.Clear();
            reader = new SQLiteCommand($"SELECT id,page,finished_position,the_text FROM ayat WHERE page={i}", quran).ExecuteReader();

            while (reader.Read()) {
                OriginalPageText.Append(reader.GetString(3));
                finishedPosition.Add(reader.GetInt32(2));
            }
        }

        public void setCursor(int position) {
            if (!success) return;
            
            int temp=-1, temp1=0;
            for (int i=0; i< finishedPosition.Count; i++) {
                if (position< finishedPosition[i]) {
                    quran.Open();
                    reader = new SQLiteCommand($"SELECT id,surah,ayah FROM ayat WHERE id={i + pageStartId}", quran).ExecuteReader();
                    reader.Read();
                    temp = reader.GetInt32(1); temp1 = reader.GetInt32(2);
                    quran.Close();
                }
            }
            if (temp!=-1) ayah(temp, temp1);
        }

        private string tempString;
        public string ayahText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            reader = new SQLiteCommand($"SELECT id,surah,ayah,the_text FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            reader.Read();
            tempString = reader.GetString(3);
            quran.Close();
            return tempString;
        }

        public string ayahAbstractText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            reader = new SQLiteCommand($"SELECT id,surah,ayah,abstract_text FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            reader.Read();
            tempString = reader.GetString(3);
            quran.Close();
            return tempString;
        }

        private List<int> SearchIDs = new List<int>();
        public string[] Search(string words) {
            SearchIDs.Clear();
            if (!success || words==null || words.Trim()=="") return new string[] { };
            words = words.Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا");
            List<string> lst = new List<string>();
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

        private string GetHtmlText() {
            string s = "<span";
            if (textColor != TextColor.nothing) s+= " style=\"color: ";
            if (textColor == TextColor.red) s+="red\"";
            else if (textColor == TextColor.green) s += "green\"";
            else if (textColor == TextColor.blue) s += "blue\"";
            else if (textColor == TextColor.darkCyan) s += "darkcyan\"";
            else if (textColor == TextColor.darkRed) s += "darkred\"";
            s += ">";
            return s;
        }
        public TextColor textColor = TextColor.nothing;
        public enum TextColor {
           nothing=0, red = 1, green = 2, blue = 3, darkCyan = 4, darkRed = 5
        }
    }
}
