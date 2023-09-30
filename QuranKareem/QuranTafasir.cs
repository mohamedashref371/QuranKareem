using System;
using System.Data.SQLite;
using System.Text;

namespace QuranKareem
{
    class QuranTafasir
    {
        private bool success = false; // نجح استدعاء ال QuranTafseer ? :(

        private SQLiteConnection quran; // SQLite Connection
        private readonly SQLiteCommand command; // SQLite Command
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        public string Comment { get; private set; }

        public static QuranTafasir Instance { get; private set; } = new QuranTafasir();

        private QuranTafasir() {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        public void QuranTafseer(string file) {
            success = false;
            if (file == null || file.Trim().Length == 0) return;
            try {
                quran.ConnectionString = $"Data Source={file}; Version=3;";
                quran.Open();
                command.CommandText = $"SELECT * FROM description";

                reader = command.ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();

                if (reader.GetInt32(0) != 4 || reader.GetInt32(1) /*version*/ != 1) return;
                Comment = reader.GetString(6);

                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;
            } catch { }
        }

        private string tempString;
        public string AyahTafseerText(int sura, int aya) {
            if (!success) return "";
            quran.Open();
            command.CommandText = $"SELECT text FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            if (reader.Read()) tempString = reader.GetString(0);
            reader.Close();
            command.Cancel();
            quran.Close();
            return tempString;
        }

    }
}
