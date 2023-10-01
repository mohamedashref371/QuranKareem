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

        private readonly SQLiteConnection quran; // SQLite Connection
        private readonly SQLiteCommand command; // SQLite Command
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount;
        public string Extension { get; private set; } // example: .mp3
        public string Comment { get; private set; }

        private int ayahId;

        public int Narration { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }

        public int SurahNumber { get; private set; }
        public int AyahNumber { get; private set; }
        public int AyatCount { get; private set; }

        public int CurrentPosition { get; private set; }

        private readonly Timer timer = new Timer(); private bool ok = true;
        private readonly AxWMPLib.AxWindowsMediaPlayer mp3 = new AxWMPLib.AxWindowsMediaPlayer();

        private bool added = false;
        public void AddInControls(Control.ControlCollection Controls) {
            if (!added) try {
                    Controls.Add(mp3);
                    mp3.Visible = false;
                    added = true;
                } catch { }
        }

        public static QuranAudios Instance { get; private set; } = new QuranAudios();

        public bool CapturedAudio = false;

        private QuranAudios() {
            timer.Tick += Timer_Tick;
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        public void QuranAudio(string path, int sura = 1, int aya = 0) {
            if (!added || path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }

            try { path = Path.GetFullPath(path); } catch { return; }

            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;

            this.path = path; success = false;

            SurahNumber = 0;
            timer.Stop(); ok = true;

            try {
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
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 3 || reader.GetInt32(1)/*version*/ != 1) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                Extension = reader.GetString(4);
                Comment = reader.GetString(5);
                reader.Close();
                command.Cancel();
                quran.Close();
                success = true;

                Ayah(sura, aya);
            } catch {}
        }

        public void Surah(int i) { Ayah(i, 0); }

        public void AyahPlus() {
            if (!success) return;
            if (AyahRepeatCounter < AyahRepeat - 1 && AyahNumber != 0) { 
                AyahRepeatCounter += 1;
                ok = true; 
                Ayah(SurahNumber, AyahNumber);
            }
            else if (AyahNumber == AyatCount && SurahRepeatCounter < SurahRepeat - 1) { 
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

        public void Ayah(int sura, int aya) {
            timer.Stop();
            if (!success) return;

            sura = Math.Abs(sura);
            if (sura == 0) sura = 1;
            else if (sura > surahsCount) sura = surahsCount;

            if (aya == -1) aya = 0;
            else aya = Math.Abs(aya);

            quran.Open();
            if (sura != SurahNumber || !CapturedAudio || mp3.Ctlcontrols.currentPosition==0) {
                // /*if(!*/ Check(sura); /*) return;*/
                surahArray = null;
                if (!CaptureAudio(sura)) { 
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

            // لم أقم بتصليح مشكلة لو لا يوجد بسملة في اول المقطع
            command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}";
            reader = command.ExecuteReader();
            if (!reader.HasRows && aya == 0) {
                reader.Close();
                command.Cancel();
                command.CommandText = $"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}";
                reader = command.ExecuteReader();
                aya = 1;
            }
            reader.Read();
            To = reader.GetInt32(3);
            //if (To == 0 && aya == 0) To = 1; // هذا ليس الحل الأمثل

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

            if (To <= 0 && aya > 0) { 
                quran.Close();
                if (ok && From > 0) mp3.Ctlcontrols.currentPosition = From / 1000.0;
                ok = true;
                return; 
            }

            if ((int)((To - From) / /*mp3.settings.rate*/ rate) > 0)
                timer.Interval = (int)((To - From) / /*mp3.settings.rate*/ rate);
            else timer.Interval = 1;

            if (ok) mp3.Ctlcontrols.currentPosition = From / 1000.0;
            quran.Close();
            ok = true;
            timer.Start();
        }

         bool Check(int surah) {
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

        bool CaptureAudio(int sura){
            string s = sura + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += Extension;
            bool hold=true;
            if (File.Exists(path + s)) mp3.URL = path + s;
            else if (File.Exists(path + sura + Extension)) mp3.URL = path + sura + Extension;
            else {
                hold = false;
                string[] filesName = Directory.GetFiles(path);
                for (int i=0; i< filesName.Length;i++) {
                    if (filesName[i].Split('\\').Last().Contains(sura.ToString().PadLeft(3, '0'))) {
                        mp3.URL = filesName[i];
                        hold = true;
                    }
                }
            }
            mp3.settings.rate = rate;
            return hold;
        }

        public void Pause() { timer.Stop(); mp3.URL=""; CapturedAudio = false; }
        public void Stop() { success = false; Pause(); }

        double rate=1;
        public void Rate(double i =1.0) {
            if (i > 1.5) rate = 1.5;
            else if (i < 0.6) rate = 0.6;
            else rate=i;
            mp3.settings.rate = rate;
            timer.Interval = (int)((To - mp3.Ctlcontrols.currentPosition*1000) / rate);
            mp3.Ctlcontrols.currentPosition = mp3.Ctlcontrols.currentPosition;
        }

        public void Volume(int i) {
            if (i < 10) i = 10;
            else if (i > 100) i = 100;
            mp3.settings.volume = i;
        }

        void Timer_Tick(object sender, EventArgs e) { ok = false; AyahPlus(); }

        public void AddEventHandler(EventHandler eh) { timer.Tick += eh; }

        int SurahRepeat=1, AyahRepeat=1, SurahRepeatCounter =0, AyahRepeatCounter =0;
        public void Repeat( int SurahRepeat = 1, int AyahRepeat=1) {
            this.SurahRepeat = (SurahRepeat >= 1) ? SurahRepeat : 1;
            this.AyahRepeat = (AyahRepeat >= 1) ? AyahRepeat : 1;
        }
        //------------------------------------------------

        public bool NewQuranAudio(string path) {
            if (!added || path == null || path.Trim().Length == 0) return false;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }
            this.path = path; success = false;

            CapturedAudio = false;
            timer.Stop(); ok = true;
            try {
                if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) {
                    if (!File.Exists(@"audios\database for audios.db")) return false;
                    File.Copy(@"audios\database for audios.db", path+"000.db");
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
            } catch { return false; }

        }
        
        public int GetTimestamp( int sura , int aya) {
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

        public int[] GetTimestamps(int sura){
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

        public void Surah(int sura, int duration){
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE surahs SET duration={duration} WHERE id={sura}";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
        }

        public void Ayah(int sura, int aya, int timestampTo) {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE ayat SET timestamp_to={timestampTo} WHERE surah={sura} AND ayah={aya}";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
            if (aya == AyatCount && timestampTo!=0) Surah(sura, timestampTo);
        }

        public void SetDescription(string extension, string comment) {
            if (!success) return;
            quran.Open();
            command.CommandText = $"UPDATE description SET extension='{extension}'; UPDATE description SET comment='{comment}'; VACUUM;";
            command.ExecuteNonQuery();
            command.Cancel();
            quran.Close();
        }

        public string[] GetDescription() /* لم يعد مستعملاً */{
            if (!success) return null;
            string[] desc = new string[2];
            quran.Open();
            command.CommandText = $"SELECT extension,comment FROM description";
            reader = command.ExecuteReader();
            reader.Read();
            desc[0] = reader.GetString(0);
            desc[1] = reader.GetString(1);
            reader.Close();
            command.Cancel();
            quran.Close();
            return desc;
        }

        public double Mp3CurrentPosition() { return mp3.Ctlcontrols.currentPosition; }
        public double Mp3Duration() { return mp3.currentMedia != null ? mp3.currentMedia.duration : 0; }

        byte[] surahArray=null;
        public void SurahSplitter() /* ليست جيدة في بعض المقاطع الصوتية */ {
            if (!success) return;
            if (!Directory.Exists("splits")) Directory.CreateDirectory("splits");
            if (mp3.URL == "") return;
            if (To <= From) return;
            if (surahArray == null) surahArray = File.ReadAllBytes(mp3.URL);
            if (surahArray.Length == 0 || mp3.currentMedia.duration == 0) return;
            double unit = surahArray.Length / (mp3.currentMedia.duration * 1000);
            byte[] ayah = new byte[(int)(unit * (To - From))];
            Array.Copy(surahArray, (int)(unit * From), ayah, 0, ayah.Length);
            File.WriteAllBytes($@"splits\S{SurahNumber.ToString().PadLeft(3, '0')}A{AyahNumber.ToString().PadLeft(3, '0')}{Extension}", ayah);
        }
    }
}
