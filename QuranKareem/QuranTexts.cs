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

        private int pageStartId, ayahId;

        public int Narration { get; private set; }
        public int Surah { get; private set; }
        public int Quarter { get; private set; }
        public int Page { get; private set; }
        public int Ayah { get; private set; }
        public bool Makya_Madanya { get; private set; }
        public int AyahStart { get; private set; }
        public int AyatCount { get; private set; }

        public static QuranTexts Instance { get; private set; } = new QuranTexts();

        public void QuranText(string path, int sura = 1, int aya = 0){
            
            if (path == null || path.Trim().Length == 0) return;

            quran = new SQLiteConnection($"Data Source={path}; Version=3;"); // SQLite Connection
            success = false;

            reader = new SQLiteCommand($"SELECT * FROM description", quran).ExecuteReader();
            if (!reader.HasRows) return;
            reader.Read();

            if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 1 || reader.GetInt32(1)/*version*/ != 1) return;

        }
    }
}
