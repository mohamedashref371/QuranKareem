﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace QuranKareem
{
    internal class TafseerQuran
    {
        #region Create The Class
        private bool success = false;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        public static readonly TafseerQuran Instance = new TafseerQuran();

        private TafseerQuran()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }
        #endregion

        private int version;

        public int Narration { get; private set; }
        public string Comment { get; private set; }

        public void Start(string file, int sura = 1, int aya = 0)
        {
            success = false;
            if (file == null || file.Trim().Length == 0) return;
            try
            {
                quran.ConnectionString = $"Data Source={file}; Version=3;";
                quran.Open();
                command.CommandText = $"SELECT * FROM description";

                reader = command.ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();

                if (reader.GetInt32(0) != 4 || reader.GetInt32(1) /*version*/ != 1) return;
                Comment = reader.GetString(6);

                success = true;
            }
            catch { }
            finally
            {
                reader?.Close();
                quran.Close();
            }
        }

        private string tempString;
        public string AyahTafseerText(int sura, int aya)
        {
            if (!success) return "";
            quran.Open();
            command.CommandText = $"SELECT text FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            tempString = reader.Read() && !reader.IsDBNull(0) ? reader.GetString(0) : "";
            reader.Close();
            quran.Close();
            return tempString;
        }

        string[] SurahTafseerText(int sura, int ayahStart = -1, int ayahEnd = 287)
        {
            if (!success) return null;
            quran.Open();
            command.CommandText = $"SELECT text FROM ayat WHERE surah={sura} AND ayah>={ayahStart} AND ayah<={ayahEnd}";
            reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read())
            {
                list.Add(!reader.IsDBNull(0) ? reader.GetString(0) : "");
            }
            reader.Close();
            quran.Close();
            return list.ToArray();
        }

        readonly string n = Environment.NewLine;
        public string SubRipText(string[] timestamps, int surah, int ayahStart = -1, int ayahEnd = 287)
        {
            if (!success) return "";
            string[] tafseer = SurahTafseerText(surah, ayahStart, ayahEnd);
            if (timestamps == null || tafseer.Length != timestamps.Length) return "";

            string s = $"0{n}00:00:00.000 --> {timestamps[0]}{n}{tafseer[0]}{n}{n}";
            for (int i = 0; i < timestamps.Length - 1; i++)
                s += $"{i + 1}{n}{timestamps[i]} --> {timestamps[i + 1]}{n}{tafseer[i + 1]}{n}{n}";

            return s;
        }
    }
}
