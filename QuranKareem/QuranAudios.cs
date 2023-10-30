using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    class QuranAudios
    {
        private string path; // المسار
        private bool success = false; // نجح استدعاء ال QuranAudio ? :(

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount;
        public string Extension { get; private set; } // example: .mp3
        public string Comment { get; private set; }

        private int ayahId;

        int version;
        public int Narration { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }

        public int SurahNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public int AyatCount { get; private set; }

        public int CurrentPosition { get; private set; }

        private readonly Timer timer = new Timer(); private bool ok = true;
        private readonly AxWMPLib.AxWindowsMediaPlayer mp3 = new AxWMPLib.AxWindowsMediaPlayer();

        private readonly Timer wordsTimer = new Timer();
        public int CurrentWord { get; private set; } = -1;
        public bool WordMode { get; set; } = false;

        private bool added = false;
        public void AddInControls(Control.ControlCollection Controls)
        {
            if (!added) try
                {
                    Controls.Add(mp3);
                    mp3.Visible = false;
                    added = true;
                }
                catch { }
        }

        public static QuranAudios Instance { get; private set; } = new QuranAudios();

        public bool CapturedAudio = false;

        private QuranAudios()
        {
            timer.Tick += Timer_Tick;
            wordsTimer.Tick += WordsTimer_Tick;
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        public void QuranAudio(string path, int sura = 1, int aya = 0)
        {
            if (!added || path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }

            try { path = Path.GetFullPath(path); } catch { return; }

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;

            this.path = path; success = false;

            SurahNumber = 0;
            timer.Stop(); ok = true;

            try
            {
                quran.Close();
                if (File.Exists(path + "000.db"))
                    quran.ConnectionString = "Data Source=" + path + "000.db;Version=3;";
                else
                    quran.ConnectionString = "Data Source=" + path + "0.db;Version=3;";
                quran.Open();
                command.CommandText = $"SELECT * FROM description";
                reader = command.ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();
                version = reader.GetInt32(1);
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3:audios*/ != 3 || (version != 1 && version != 2)) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                Extension = reader.GetString(4);
                Comment = reader.GetString(5);
                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;

                Ayah(sura, aya);
            }
            catch { }
        }

        public void Surah(int i) { Ayah(i, 0); }

        public void AyahPlus()
        {
            if (!success) return;
            if (AyahRepeatCounter < AyahRepeat - 1 && AyahNumber != 0)
            {
                AyahRepeatCounter += 1;
                ok = true;
                Ayah(SurahNumber, AyahNumber);
            }
            else if (AyahNumber == AyatCount && SurahRepeatCounter < SurahRepeat - 1)
            {
                SurahRepeatCounter += 1;
                ok = true;
                Ayah(SurahNumber, 0);
            }
            else
            {
                AyahRepeatCounter = 0;
                if (AyahNumber == AyatCount && SurahNumber < surahsCount) Ayah(SurahNumber + 1, 0);
                else if (AyahNumber == AyatCount) Surah(1);
                else Ayah(SurahNumber, AyahNumber + 1);
            }

        }

        public void Ayah() { Ayah(SurahNumber, AyahNumber); }
        public void Ayah(int aya) { Ayah(SurahNumber, aya); }

        public void Ayah(int sura, int aya)
        {
            if (!timer.Enabled) ok = true;
            timer.Stop(); wordsTimer.Stop();
            if (!success) return;

            sura = Math.Abs(sura);
            if (sura == 0) sura = 1;
            else if (sura > surahsCount) sura = surahsCount;

            if (aya == -1) aya = 0;
            else aya = Math.Abs(aya);

            quran.Open();
            if (sura != SurahNumber || !CapturedAudio || mp3.Ctlcontrols.currentPosition == 0)
            {
                // /*if(!*/ Check(sura); /*) return;*/
                surahArray = null; start = -1;
                if (!CaptureAudio(sura))
                {
                    mp3.URL = "";
                    quran.Close();
                    CapturedAudio = false;
                    return;
                }

                CapturedAudio = true;
                command.CommandText = $"SELECT * FROM surahs WHERE id={sura}";
                reader = command.ExecuteReader();
                reader.Read();
                SurahNumber = sura;
                AyatCount = reader.GetInt32(2);
                reader.Close();
                command.Cancel();
            }

            if (aya > AyatCount) aya = AyatCount;

            command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            if (!reader.HasRows && aya == 0)
            {
                reader.Close();
                command.Cancel();
                command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}";
                reader = command.ExecuteReader();
                aya = 1;
            }
            reader.Read();
            To = reader.GetInt32(3);

            ayahId = reader.GetInt32(0);
            reader.Close();
            command.Cancel();
            command.CommandText = $"SELECT * FROM ayat WHERE id={ayahId - 1}";
            reader = command.ExecuteReader();
            reader.Read();
            From = Math.Abs(reader.GetInt32(3));
            reader.Close();
            command.Cancel();
            AyahNumber = aya;
            CurrentPosition = From;

            if (To <= 0 && aya > 0)
            {
                quran.Close();
                if (ok && From > 0) mp3.Ctlcontrols.currentPosition = From / 1000.0;
                ok = true;
                return;
            }

            if ((int)((To - From) / /*mp3.settings.rate*/ rate) > 0)
                timer.Interval = (int)((To - From) / /*mp3.settings.rate*/ rate);
            else timer.Interval = 1;

            if (ok) mp3.Ctlcontrols.currentPosition = From / 1000.0;

            words.Clear(); CurrentWord = -1; idWord = 0;
            if (WordMode)
            {
                int wordCount;
                command.CommandText = $"SELECT word FROM words WHERE ayah_id={ayahId} ORDER BY word DESC";
                reader = command.ExecuteReader();
                reader.Read();
                wordCount = reader.GetInt32(0);
                reader.Close();
                command.CommandText = $"SELECT word,timestamp_from FROM words WHERE ayah_id={ayahId} GROUP BY word";
                reader = command.ExecuteReader();
                for (int i = 0; i < wordCount; i++) words.Add(-1);
                while (reader.Read()) words[reader.GetInt32(0) - 1] = reader.GetInt32(1);
                reader.Close();
                command.Cancel();

                command.CommandText = $"SELECT word,timestamp_from,timestamp_to FROM words WHERE ayah_id={ayahId}";
                reader = command.ExecuteReader();
                while (reader.Read()) FullWords.AddRange(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2) });
                reader.Close();
                command.Cancel();
            }
            quran.Close();
            ok = true;
            timer.Start();
            if (WordMode) Words();
        }

        private readonly List<int> words = new List<int>();
        public void WordOf(int word)
        {
            if (success && WordMode && timer.Enabled && word > 0 && word <= words.Count && words[word - 1]>=0 && To - words[word - 1] > 0)
            {
                wordsTimer.Stop();
                timer.Interval = (int)((To - words[word - 1]) / rate);
                mp3.Ctlcontrols.currentPosition = words[word - 1] / 1000.0;
                idWord = 0;
                Words();
            }

        }
        private readonly List<int> FullWords = new List<int>(); int idWord = 0;
        private void Words()
        {
            wordsTimer.Stop();
            CurrentWord = -1;
            wordsTimer.Interval = 100;
            int num = (int)(mp3.Ctlcontrols.currentPosition * 1000);
            for (int i = idWord; i < FullWords.Count / 3; i++)
            {
                if (num >= FullWords[i * 3 + 1] && num <= FullWords[i * 3 + 2])
                {
                    wordsTimer.Interval = FullWords[i * 3 + 2] - num > 0 ? FullWords[i * 3 + 2] - num : 1;
                    CurrentWord = FullWords[i * 3];
                    idWord++;
                    break;
                }
            }
            wordsTimer.Start();
        }

        bool Check(int surah)
        {
            if (!CaptureAudio(surah)) return false;
            //quran.Open();
            command.CommandText = $"SELECT duration FROM surahs WHERE id={surah}";
            reader = command.ExecuteReader();
            reader.Read();
            if (reader.IsDBNull(0)) { /*quran.Close();*/ return false; }
            int i = reader.GetInt32(0);
            reader.Close();
            command.Cancel();
            //quran.Close();
            if (mp3.currentMedia.duration == i) return true;
            return false;
        }

        bool CaptureAudio(int sura)
        {
            string s = sura + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += Extension;
            bool hold = true;
            if (File.Exists(path + s)) mp3.URL = path + s;
            else if (File.Exists(path + sura + Extension)) mp3.URL = path + sura + Extension;
            else
            {
                hold = false;
                string[] filesName = Directory.GetFiles(path);
                for (int i = 0; i < filesName.Length; i++)
                {
                    if (filesName[i].Split('\\').Last().Contains(sura.ToString().PadLeft(3, '0')))
                    {
                        mp3.URL = filesName[i];
                        hold = true;
                    }
                }
            }
            mp3.settings.rate = rate;
            return hold;
        }

        public void Pause() { timer.Stop(); wordsTimer.Stop(); mp3.URL = ""; CapturedAudio = false; }
        public void Stop() { success = false; Pause(); }

        double rate = 1;
        public void Rate(double i = 1.0)
        {
            if (i > 1.5) rate = 1.5;
            else if (i < 0.6) rate = 0.6;
            else rate = i;
            mp3.settings.rate = rate;
            if (timer.Enabled && To - mp3.Ctlcontrols.currentPosition * 1000 > 0) timer.Interval = (int)((To - mp3.Ctlcontrols.currentPosition * 1000) / rate);
            mp3.Ctlcontrols.currentPosition = mp3.Ctlcontrols.currentPosition;
        }

        public void Volume(int i)
        {
            if (i < 10) i = 10;
            else if (i > 100) i = 100;
            mp3.settings.volume = i;
        }

        void Timer_Tick(object sender, EventArgs e) { ok = false; AyahPlus(); }
        void WordsTimer_Tick(object sender, EventArgs e) { Words(); }

        public void AddEventHandlerOfAyah(EventHandler eH) => timer.Tick += eH;
        public void AddEventHandlerOfWord(EventHandler eH) => wordsTimer.Tick += eH;

        int SurahRepeat = 1, AyahRepeat = 1, SurahRepeatCounter = 0, AyahRepeatCounter = 0;
        public void Repeat(int SurahRepeat = 1, int AyahRepeat = 1)
        {
            this.SurahRepeat = (SurahRepeat >= 1) ? SurahRepeat : 1;
            this.AyahRepeat = (AyahRepeat >= 1) ? AyahRepeat : 1;
        }

        byte[] surahArray = null; int start = -1;
        // ليست جيدة في بعض المقاطع الصوتية التي تحتوي على وصف وصورة في بداية الملف
        public void SurahSplitter()
        {
            if (!success) return;
            if (!Directory.Exists("splits")) Directory.CreateDirectory("splits");
            if (mp3.URL == "") return;
            if (To <= From) return;
            if (surahArray == null) surahArray = File.ReadAllBytes(mp3.URL);
            if (surahArray.Length < 21000 || mp3.currentMedia.duration == 0) return;
            if (start == -1)
            {
                start = 0;
                int j = 0;
                for (int i = 0; i < 20000; i++)
                {
                    if (surahArray[i] == 0) j += 1;
                    else
                    {
                        if (j > 1000) { start = i; break; }
                        j = 0;
                    }
                }
            }
            double unit = (surahArray.Length - start) / (mp3.currentMedia.duration * 1000);
            byte[] ayah = new byte[(int)(unit * (To - From))];
            Array.Copy(surahArray, (int)(unit * From) + start, ayah, 0, ayah.Length);
            File.WriteAllBytes($@"splits\S{SurahNumber.ToString().PadLeft(3, '0')}A{AyahNumber.ToString().PadLeft(3, '0')}{Extension}", ayah);
        }

        // Mp3 Current Position String
        public string GetCurrentPosition() { return GetPositionOf(CurrentPosition); }
        string s;
        public string GetPositionOf(int num)
        {
            int temp = num / 3600000;
            s = temp.ToString().PadLeft(2, '0') + ":";

            num -= temp * 3600000;
            temp = num / 60000;
            s += temp.ToString().PadLeft(2, '0') + ":";

            num -= temp * 60000;
            temp = num / 1000;
            s += temp.ToString().PadLeft(2, '0') + ".";

            num -= temp * 1000;
            s += num.ToString().PadLeft(3, '0');

            return s;
        }

        public string[] GetPositionsOf(int surah)
        {
            if (!success) return null;
            int[] timestampsT = GetTimestamps(surah);
            string[] timestamps = new string[timestampsT.Length];

            for (int i = 0; i < timestamps.Length; i++)
                timestamps[i] = GetPositionOf(timestampsT[i]);

            return timestamps;
        }

        #region إضافة شيخ جديد
        public bool NewQuranAudio(string path)
        {
            if (!added || path == null || path.Trim().Length == 0) return false;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }
            this.path = path; success = false;

            CapturedAudio = false;
            timer.Stop(); ok = true;
            try
            {
                if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db"))
                {
                    if (!File.Exists(@"audios\database for audios.db")) return false;
                    File.Copy(@"audios\database for audios.db", path + "000.db");
                }

                if (File.Exists(path + "000.db"))
                    quran.ConnectionString = "Data Source=" + path + "000.db;Version=3;";
                else
                    quran.ConnectionString = "Data Source=" + path + "0.db;Version=3;";

                quran.Open();
                command.CommandText = $"SELECT * FROM description";
                reader = command.ExecuteReader();
                if (!reader.HasRows) return false;
                reader.Read();
                if (reader.GetInt32(0) != 3 || reader.GetInt32(1) != 1) return false;
                Narration = reader.GetInt32(2);
                surahsCount = reader.GetInt32(3);
                Extension = reader.GetString(4);
                Comment = reader.GetString(5);
                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;
                return true;
            }
            catch { return false; }

        }

        public int GetTimestamp(int sura, int aya)
        {
            if (!success) return 0;
            int temp;
            quran.Open();
            command.CommandText = $"SELECT timestamp_to FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            reader.Read();
            temp = reader.GetInt32(0);
            reader.Close();
            command.Cancel();
            quran.Close();
            return temp;
        }

        public int[] GetTimestamps(int sura)
        {
            if (!success) return null;
            List<int> list = new List<int>();
            quran.Open();
            command.CommandText = $"SELECT timestamp_to FROM ayat WHERE surah={sura}";
            reader = command.ExecuteReader();
            while (reader.Read()) list.Add(reader.GetInt32(0));
            reader.Close();
            command.Cancel();
            quran.Close();
            return list.ToArray();
        }

        public void SetSurah(int sura, int duration)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE surahs SET duration={duration} WHERE id={sura}";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
        }

        public void SetAyah(int sura, int aya, int timestampTo)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE ayat SET timestamp_to={timestampTo} WHERE surah={sura} AND ayah={aya}";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
            if (aya == AyatCount && timestampTo != 0) SetSurah(sura, timestampTo);
        }

        public void SetDescription(string extension, string comment)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE description SET extension='{extension}', comment='{comment}'; VACUUM;";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
            Extension = extension; Comment = comment;
        }

        //public string[] GetDescription() /* لم يعد مستعملاً */
        //{
        //    if (!success) return null;
        //    string[] desc = new string[2];
        //    quran.Open();
        //    command.CommandText = $"SELECT extension,comment FROM description";
        //    reader = command.ExecuteReader();
        //    reader.Read();
        //    desc[0] = reader.GetString(0);
        //    desc[1] = reader.GetString(1);
        //    reader.Close();
        //    command.Cancel();
        //    quran.Close();
        //    return desc;
        //}

        public double Mp3CurrentPosition() { return mp3.Ctlcontrols.currentPosition; }
        public double Mp3Duration() { return mp3.currentMedia != null ? mp3.currentMedia.duration : 0; }
        #endregion
    }
}
