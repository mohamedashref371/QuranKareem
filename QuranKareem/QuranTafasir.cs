using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    class QuranTafasir
    {
        private bool success = false; // نجح استدعاء ال QuranTafseer ? :(

        private readonly SQLiteConnection quran; // SQLite Connection
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
            tempString = reader.Read() && !reader.IsDBNull(0)? reader.GetString(0) : "";
            reader.Close();
            command.Cancel();
            quran.Close();
            return tempString;
        }

         string[] SurahTafseerText(int sura) {
            if (!success) return null;
            quran.Open();
            command.CommandText = $"SELECT text FROM ayat WHERE surah={sura}";
            reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read()) {
                list.Add(!reader.IsDBNull(0)? reader.GetString(0) : "");
            }
            reader.Close();
            command.Cancel();
            quran.Close();
            return list.ToArray();
        }

        string n = Environment.NewLine;
        public string SubRipText(int surah, string[] timestamps) {
            if (!success) return "";
            string[] tafseer = SurahTafseerText(surah);
            if (timestamps == null || tafseer.Length != timestamps.Length) return "";

            string s = $"0{n}00:00:00.000 --> {timestamps[0]}{n}{tafseer[0]}{n}{n}";
            for (int i=0; i< timestamps.Length-1; i++)
                s += $"{i+1}{n}{timestamps[i]} --> {timestamps[i+1]}{n}{tafseer[i+1]}{n}{n}";
            
            return s;
        }
    }
}
