using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranKareem
{
    class QuranTexts
    {
        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranText ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال sql select

        private int surahsCount, quartersCount, pagesCount;
        private string font, comment;
        private int pageStartId, ayahId;

        public int Narration { get; private set; }
        public int Surah { get; private set; }
        public int Quarter { get; private set; }
        public int Page { get; private set; }
        public int Ayah { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }

        public string PageText { get; private set; }

        private int temp, temp1;

        public static QuranTexts Instance { get; private set; } = new QuranTexts();

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
            font = reader.GetString(6);
            comment= reader.GetString(7);
            success = true;


        }

        public string[] GetSurahNames()
        {
            if (!success) return new string[] { "" };
            string[] names = new string[surahsCount];
            quran.Open();
            reader = new SQLiteCommand($"SELECT name FROM surahs", quran).ExecuteReader();
            int i = 0;
            while (reader.Read())
            {
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
            temp = reader.GetInt32(1)/*surah*/; temp1 = reader.GetInt32(2)/*ayah*/;
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
            temp = reader.GetInt32(1); temp1 = reader.GetInt32(2);
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

            if (Page != reader.GetInt32(3)){
                Page = reader.GetInt32(3);
                reader = new SQLiteCommand($"SELECT * FROM pages WHERE id={Page}", quran).ExecuteReader();
                reader.Read();
                pageStartId = reader.GetInt32(1);
                text(Page);
            }


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

        private StringBuilder sb = new StringBuilder();
        private void text(int i) {
            sb.Clear();
            quran.Open();
            reader = new SQLiteCommand($"SELECT id,page,the_text FROM ayat WHERE page={i}", quran).ExecuteReader();

            while (reader.Read()) {
                sb.Append(reader.GetString(2));
            }

            PageText = sb.ToString();
            quran.Close();
        }

        public void setCursor(int i) {

        }
    }
}
