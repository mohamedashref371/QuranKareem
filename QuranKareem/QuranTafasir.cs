using System;
using System.Data.SQLite;
using System.Text;

namespace QuranKareem
{
    class QuranTafasir
    {
        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranTafseer ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        public string Comment { get; private set; }

        public static QuranTafasir Instance { get; private set; } = new QuranTafasir();

        public void QuranTafseer(string file) {
            success = false;
            if (file == null || file.Trim().Length == 0) return;
            try {
                quran = new SQLiteConnection($"Data Source={file}; Version=3;"); // SQLite Connection
                quran.Open();

                reader = new SQLiteCommand($"SELECT * FROM description", quran).ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();

                if (reader.GetInt32(0) != 4 || reader.GetInt32(1)/*version*/ != 1) return;
                Comment = reader.GetString(6);

                success = true;
                quran.Close();
            } catch { }
        }

        private string tempString;
        public string AyahTafseerText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            reader = new SQLiteCommand($"SELECT text FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            reader.Read();
            tempString = reader.GetString(0);
            quran.Close();
            return tempString;
        }

    }
}
