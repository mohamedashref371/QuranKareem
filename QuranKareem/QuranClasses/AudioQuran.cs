using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace QuranKareem
{
    internal class AudioQuran
    {
        #region Create The Class
        private bool success = false;

        private readonly SQLiteConnection quran;
        private readonly SQLiteCommand command;
        private SQLiteDataReader reader;

        public static readonly AudioQuran Instance = new AudioQuran();

        private AudioQuran()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
            timer.Tick += Timer_Tick;
            wordsTimer.Tick += WordsTimer_Tick;
        }
        #endregion

        private string path; // المسار
        public string Extension { get; private set; } // example: .mp3
        private int ayahId;

        public int SurahsCount { get; private set; }

        public int SurahNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public int AyatCount { get; private set; }

        private int version;

        public int Narration { get; private set; }
        public string Comment { get; private set; }

        public int From { get; private set; }
        public int To { get; private set; }

        double rate = 1;
        public double Rate
        {
            set
            {
                if (value > 1.5) rate = 1.5;
                else if (value < 0.6) rate = 0.6;
                else rate = value;
                mp3.settings.rate = rate;
                if (timer.Enabled && To - mp3.Ctlcontrols.currentPosition * 1000 > 0) timer.Interval = (int)((To - mp3.Ctlcontrols.currentPosition * 1000) / rate);
                mp3.Ctlcontrols.currentPosition = mp3.Ctlcontrols.currentPosition;
            }
        }

        public int Volume
        {
            set
            {
                if (value < 1) value = 1;
                else if (value > 100) value = 100;
                mp3.settings.volume = value;
            }
        }

        public int CurrentPosition { get; private set; }

        private readonly Timer timer = new Timer(); private bool ok = true;
        private readonly AxWMPLib.AxWindowsMediaPlayer mp3 = new AxWMPLib.AxWindowsMediaPlayer();

        private readonly Timer wordsTimer = new Timer();
        public int CurrentWord { get; private set; } = -1;
        private bool isWordTableEmpty = true;

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

        public void Start(string path, int sura = 1, int aya = 0)
        {
            if (!added || !Directory.Exists(path)) return;

            try { path = Path.GetFullPath(path); } catch { return; }
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;

            this.path = path; success = false;

            SurahNumber = 0;
            timer.Stop(); ok = true;

            try
            {
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
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3:audios*/ != 3 || version < 1 || version > 4) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                SurahsCount = reader.GetInt32(3);
                Extension = reader.GetString(4);
                Comment = reader.GetString(5);
                reader.Close();

                isWordTableEmpty = true;
                if (version != 1)
                {
                    command.CommandText = $"SELECT * FROM words LIMIT 1";
                    reader = command.ExecuteReader();
                    isWordTableEmpty = !reader.HasRows;
                }

                success = true;
            }
            catch { }
            finally
            {
                reader?.Close();
                quran.Close();
            }
            if (success) Set(sura, aya);
        }

        #region التنقلات في المصحف

        readonly StringBuilder sql = new StringBuilder();
        /// <summary>
        /// Implementation Priority:<br/>
        ///   - Surah (1:114), its Ayah (0:{AyatCount}).<br/>
        ///   - Ayah (0:{AyatCount}) only.<br/>
        ///   - Next parameter (false,true).<br/>
        /// </summary>
        /// <param name="surah">Quran Surahs Count is 114.</param>
        /// <param name="ayah">Each Surah contains Ayat.</param>
        /// <param name="next">Go to the next Ayah.</param>
        /// <returns>true if everything goes well.</returns>
        public bool Set(int surah = 0, int ayah = -2, bool next = false)
        {
            if (!timer.Enabled) ok = true;
            timer.Stop(); wordsTimer.Stop();
            if (!success) return false;

            #region SQL Building
            sql.Length = 0;
            sql.Append("SELECT id,surah,ayah,timestamp_to FROM ayat WHERE ");
            #region Surah and Ayah
            // Ayah
            if ((surah <= 0 || surah == SurahNumber) && ayah >= -1)
            {
                if (ayah == AyatCount + 1)
                {
                    surah = SurahNumber != SurahsCount ? SurahNumber + 1 : 1;
                    ayah = 0;
                }
                else if (ayah > AyatCount + 1)
                {
                    surah = SurahNumber;
                    ayah = AyatCount;
                }
                else
                    surah = SurahNumber;

                if (ayah < 0) ayah = 0;
                sql.Append($"surah >= {surah} AND ayah >= {ayah}");
            }

            // Surah and Ayah
            else if (surah >= 1 && surah <= SurahsCount + 1)
            {
                if (surah == SurahsCount + 1) surah = 1;
                if (ayah < 0) ayah = 0;
                sql.Append($"surah >= {surah} AND ayah >= {ayah}");
            }

            // Ayah Plus
            else if (next && SurahNumber >= 1)
            {
                if (AyahRepeatCounter < AyahRepeat - 1 && AyahNumber != 0)
                {
                    AyahRepeatCounter += 1;
                    ok = true;
                    sql.Append($"id = {ayahId}");
                }
                else if (AyahNumber == AyatCount && SurahRepeatCounter < SurahRepeat - 1)
                {
                    SurahRepeatCounter += 1;
                    AyahRepeatCounter = 0;
                    ok = true;
                    sql.Append($"surah = {SurahNumber} AND ayah >= 0");
                }
                else
                {
                    AyahRepeatCounter = 0;
                    if (SurahNumber == SurahsCount && AyahNumber == AyatCount)
                        sql.Append("ayah >= 0");
                    else
                        sql.Append($"id > {ayahId} AND ayah >= 0");
                }
            }
            #endregion
            else
            {
                sql.Append($"surah >= {SurahNumber} AND ayah >= {AyahNumber}");
            }
            sql.Append(" LIMIT 1");
            #endregion

            #region SQL Execution
            quran.Open();
            command.CommandText = sql.ToString();
            reader = command.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close(); quran.Close();
                return false;
            }
            int id = reader.GetInt32(0);
            surah = reader.GetInt32(1);
            ayah = reader.GetInt32(2);
            To = reader.GetInt32(3);
            reader.Close(); quran.Close();
            #endregion

            if (surah != SurahNumber || !CapturedAudio || mp3.Ctlcontrols.currentPosition == 0)
            {
                surahArray = null; start = -1;
                if (!CaptureAudio(surah))
                {
                    mp3.URL = "";
                    return false;
                }
                SurahData(surah);
            }
            ayahId = id;
            AyahNumber = ayah;

            AyahData();
            return true;
        }

        private void SurahData(int surah)
        {
            SurahNumber = surah;
            SurahRepeatCounter = 0;
            command.CommandText = $"SELECT ayat_count FROM surahs WHERE id={surah}";
            quran.Open();
            reader = command.ExecuteReader();
            if (reader.Read())
                AyatCount = reader.GetInt32(0);
            reader.Close(); quran.Close();
        }

        private void AyahData()
        {
            quran.Open();

            command.CommandText = $"SELECT * FROM ayat WHERE id={ayahId - 1}";
            reader = command.ExecuteReader();
            reader.Read();
            From = Math.Abs(reader.GetInt32(3));
            reader.Close();
            command.Cancel();
            CurrentPosition = From;

            if (To <= 0 && AyahNumber > 0)
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
            if (!isWordTableEmpty)
            {
                int wordCount;
                command.CommandText = $"SELECT word FROM words WHERE ayah_id={ayahId} ORDER BY word DESC";
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    wordCount = reader.GetInt32(0);
                    reader.Close();
                    command.Cancel();
                    command.CommandText = $"SELECT word,timestamp_from FROM words WHERE ayah_id={ayahId} GROUP BY word";
                    reader = command.ExecuteReader();
                    for (int i = 0; i < wordCount; i++) words.Add(-1);
                    while (reader.Read()) words[reader.GetInt32(0) - 1] = reader.GetInt32(1);
                    reader.Close();
                    command.Cancel();

                    command.CommandText = $"SELECT word,timestamp_from,timestamp_to FROM words WHERE ayah_id={ayahId}";
                    reader = command.ExecuteReader();
                    while (reader.Read()) FullWords.AddRange(new int[] { reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2) });
                }
                reader.Close();
            }
            quran.Close();
            ok = true;
            timer.Start();
            if (!isWordTableEmpty) Words();
        }

        private readonly List<int> words = new List<int>();
        public void WordOf(int word)
        {
            if (success && !isWordTableEmpty && timer.Enabled && word > 0 && word <= words.Count && words[word - 1] >= 0 && To - words[word - 1] > 0)
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
            int num = (int)(mp3.Ctlcontrols.currentPosition * 1000); // Error At Program Closing
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

        public bool Check(int delta = 0)
        {
            if (success && CaptureAudio(SurahNumber))
            {
                quran.Open();
                command.CommandText = $"SELECT duration FROM surahs WHERE id={SurahNumber}";
                reader = command.ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0)) { quran.Close(); return false; }
                int i = reader.GetInt32(0);
                reader.Close();
                quran.Close();
                return Math.Abs(mp3.currentMedia.duration - i) <= delta;
            }
            return false;
        }

        public bool CapturedAudio = false;
        bool CaptureAudio(int sura)
        {
            string s = sura + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += Extension;
            CapturedAudio = true;
            if (File.Exists(path + s)) mp3.URL = path + s;
            else if (File.Exists(path + sura + Extension)) mp3.URL = path + sura + Extension;
            else
            {
                CapturedAudio = false;
                string[] filesName = Directory.GetFiles(path);
                for (int i = 0; i < filesName.Length; i++)
                {
                    if (filesName[i].Split('\\').Last().Contains(sura.ToString().PadLeft(3, '0')))
                    {
                        mp3.URL = filesName[i];
                        CapturedAudio = true;
                    }
                }
            }
            mp3.settings.rate = rate;
            return CapturedAudio;
        }
        #endregion

        public void OneTimePause()
        {
            timer.Stop(); wordsTimer.Stop();
            mp3.URL = ""; CapturedAudio = false;
        }

        public void Pause()
        {
            timer.Stop(); wordsTimer.Stop();
            mp3.Ctlcontrols.pause(); CapturedAudio = false;
        }
        public void Stop()
        {
            success = false;
            timer.Stop(); wordsTimer.Stop();
            mp3.URL = ""; CapturedAudio = false;
        }

        void Timer_Tick(object sender, EventArgs e) { ok = false; Set(next: true); }
        void WordsTimer_Tick(object sender, EventArgs e) => Words();

        public void AddEventHandlerOfAyah(EventHandler eH) => timer.Tick += eH;
        public void AddEventHandlerOfWord(EventHandler eH) => wordsTimer.Tick += eH;

        int SurahRepeat = 1, AyahRepeat = 1, SurahRepeatCounter = 0, AyahRepeatCounter = 0;
        public void Repeat(int SurahRepeat = 1, int AyahRepeat = 1)
        {
            this.SurahRepeat = (SurahRepeat >= 1) ? SurahRepeat : 1;
            this.AyahRepeat = (AyahRepeat >= 1) ? AyahRepeat : 1;
        }

        byte[] surahArray = null; int start = -1, unit;
        public void SurahSplitter()
        {
            if (!success || mp3.URL == "" || To <= From || mp3.currentMedia.duration == 0) return;
            if (!Directory.Exists("splits")) Directory.CreateDirectory("splits");
            if (surahArray == null) surahArray = File.ReadAllBytes(mp3.URL);

            if (start == -1)
            {
                unit = Convert.ToInt32(mp3.currentMedia.getItemInfo("Bitrate")) / 8000;
                start = (int)(surahArray.Length - unit * mp3.currentMedia.duration * 1000);
                if (start < 0) start = 0;
            }

            byte[] ayah = new byte[unit * (To - From)];
            Array.Copy(surahArray, start + unit * From, ayah, 0, ayah.Length);
            File.WriteAllBytes($@"splits\S{SurahNumber.ToString().PadLeft(3, '0')}A{AyahNumber.ToString().PadLeft(3, '0')}{Extension}", ayah);
        }

        public float[] WordsList(int ayahStart, int ayahEnd, out string mp3Url, List<int> ayahword, List<float> timestamps)
        {
            mp3Url = mp3.URL;
            ayahword.Clear();
            timestamps.Clear();

            if (!success || isWordTableEmpty || mp3.URL == "" || ayahword == null || timestamps == null) return null;

            int tStart = 0, tEnd = 0, to ;
            quran.Open();

            command.CommandText = $"SELECT MIN(id),MAX(id) FROM ayat WHERE surah={SurahNumber} AND ayah>={ayahStart} AND ayah<={ayahEnd}";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                tStart = reader.GetInt32(0);
                tEnd = reader.GetInt32(1);
            }
            reader.Close(); command.Cancel();

            command.CommandText = $"SELECT MIN(timestamp_to),MAX(timestamp_to) FROM ayat WHERE id>={tStart-1} AND id<={tEnd}";
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                tStart = reader.GetInt32(0);
                tEnd = reader.GetInt32(1);
            }
            reader.Close(); command.Cancel();

            command.CommandText = $"SELECT ayah,word,words.timestamp_from,words.timestamp_to FROM ayat JOIN words ON ayat.id = words.ayah_id WHERE surah={SurahNumber} AND ayah>={ayahStart} AND ayah<={ayahEnd} ORDER BY words.timestamp_from";
            reader = command.ExecuteReader();

            to = tStart;

            while (reader.Read())
            {
                if (reader.GetInt32(2) > to)
                {
                    if (reader.GetInt32(2) - to <= 12)
                    {
                        timestamps[timestamps.Count - 1] += (reader.GetInt32(2) - to) / 1000f;
                    }
                    else
                    {
                        ayahword.Add(reader.GetInt32(0));
                        ayahword.Add(-1);
                        timestamps.Add((reader.GetInt32(2) - to) / 1000f);
                    }
                }
                ayahword.Add(reader.GetInt32(0));
                ayahword.Add(reader.GetInt32(1));
                to = reader.GetInt32(3);
                timestamps.Add((to - reader.GetInt32(2)) / 1000f);
            }
            reader.Close(); quran.Close();

            if (tEnd - to > 1)
            {
                ayahword.Add(-1);
                ayahword.Add(-1);
                timestamps.Add((tEnd - to) / 1000f);
            }
            return new float[2] { tStart / 1000f, tEnd / 1000f };
        }

        // Mp3 Current Position String
        public string GetCurrentPosition() => GetPositionOf(CurrentPosition);
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

        public string[] GetPositionsOf(int surah, int ayahStart = -1, int ayahEnd = 287)
        {
            if (!success) return null;
            int[] timestampsT = GetTimestamps(surah, ayahStart, ayahEnd);
            string[] timestamps = new string[timestampsT.Length];

            for (int i = 0; i < timestamps.Length; i++)
                timestamps[i] = GetPositionOf(timestampsT[i]);

            return timestamps;
        }

        #region إضافة شيخ جديد
        public bool NewQuranAudio(string path)
        {
            if (!added || !Directory.Exists(path)) return false;

            try { path = Path.GetFullPath(path); } catch { return false; }
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
                version = reader.GetInt32(1);
                if (reader.GetInt32(0) != 3 || version < 1 || version > 4) return false;
                Narration = reader.GetInt32(2);
                SurahsCount = reader.GetInt32(3);
                Extension = reader.GetString(4);
                Comment = reader.GetString(5);

                isWordTableEmpty = true;
                if (version != 1)
                {
                    reader.Close();
                    command.Cancel();
                    command.CommandText = $"SELECT * FROM words LIMIT 1";
                    reader = command.ExecuteReader();
                    isWordTableEmpty = !reader.HasRows;
                }

                success = true;
                return true;
            }
            catch { return false; }
            finally
            {
                reader?.Close();
                quran.Close();
            }
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
            quran.Close();
            return temp;
        }

        public int[] GetTimestamps(int sura, int ayahStart = -1, int ayahEnd = 287)
        {
            if (!success) return null;
            int shift = 0;
            List<int> list = new List<int>();
            quran.Open();
            command.CommandText = $"SELECT timestamp_to FROM ayat WHERE surah={sura} AND ayah>={ayahStart - 1} AND ayah<={ayahEnd}";
            reader = command.ExecuteReader();
            if (ayahStart >= 0)
            {
                if (reader.Read())
                    shift = reader.GetInt32(0);
            }
            while (reader.Read()) list.Add(reader.GetInt32(0) - shift);
            reader.Close();
            quran.Close();
            return list.ToArray();
        }

        public void SetSurah(int sura, int duration)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE surahs SET duration={duration} WHERE id={sura}";
            command.ExecuteNonQuery();
            quran.Close();
        }

        public void SetAyah(int sura, int aya, int timestampTo)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE ayat SET timestamp_to={timestampTo} WHERE surah={sura} AND ayah={aya}";
            command.ExecuteNonQuery();
            quran.Close();
            if (aya == AyatCount && timestampTo != 0) SetSurah(sura, timestampTo);
        }

        public void SetDescription(string extension, string comment)
        {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE description SET extension='{extension}', comment='{comment}'; VACUUM;";
            command.ExecuteNonQuery();
            quran.Close();
            Extension = extension; Comment = comment;
        }

        public double Mp3CurrentPosition() => mp3.Ctlcontrols.currentPosition;
        public double Mp3Duration() => mp3.currentMedia != null ? mp3.currentMedia.duration : 0;
        #endregion
    }
}
