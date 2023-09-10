using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    class QuranAudios
    {
        private string path; // المسار
        private SQLiteConnection quran; // SQLiteConnection
        private bool success = false; // نجح استدعاء ال QuranAudio ? :(
        private SQLiteDataReader reader; // قارئ لتنفيذ ال 'select' sql

        private int surahsCount;
        private string extension; // example: .mp3
        private string comment;

        private int ayahId;

        public int Narration { get; private set; }
        public int From { get; private set; }
        public int To { get; private set; }

        public int Surah { get; private set; }
        public int Ayah { get; private set; }
        public int AyatCount { get; private set; }

        public int CurrentPosition { get; private set; }

        private Timer timer = new Timer(); private bool ok = true;
        private AxWMPLib.AxWindowsMediaPlayer mp3 = new AxWMPLib.AxWindowsMediaPlayer();

        private bool added = false;
        public void AddInControls(Control.ControlCollection Controls) {
            if (!added) try {
                    timer.Tick += new EventHandler(timer_Tick);
                    Controls.Add(mp3);
                    mp3.Visible = false;
                    added = true;
                } catch { }
        }

        public static QuranAudios Instance { get; private set; } = new QuranAudios();

        public bool CapturedAudio = false;

        public void QuranAudio(string path, int sura = 1, int aya = 0) {
            if (!added || path == null || path.Trim().Length == 0) return;
            if (path.Substring(path.Length - 1) != "\\") { path += "\\"; }
            if (!File.Exists(path + "000.db") && !File.Exists(path + "0.db")) return;

            this.path = path; success = false;

            Surah = 0;
            timer.Stop(); ok = true;

            try {
                if (File.Exists(path + "000.db"))
                    quran = new SQLiteConnection("Data Source=" + path + "000.db;Version=3;"); // SQLite Connection
                else
                    quran = new SQLiteConnection("Data Source=" + path + "0.db;Version=3;"); // SQLite Connection
                quran.Open();
                reader = new SQLiteCommand($"SELECT * FROM description", quran).ExecuteReader();
                if (!reader.HasRows) return;
                reader.Read();
                if (reader.GetInt32(0)/*type 1:text, 2:picture, 3: audios*/ != 3 || reader.GetInt32(1)/*version*/ != 1) return;
                Narration = reader.GetInt32(2); // العمود الثالث
                surahsCount = reader.GetInt32(3);
                extension = reader.GetString(4);
                comment = reader.GetString(5);
                quran.Close();
                success = true;

                ayah(sura, aya);
            } catch { return; }
        }

        public void surah(int i) { ayah(i, 0); }

        public void AyahPlus() {
            if (!success) return;
            if (AyahRepeatCounter < AyahRepeat - 1 && Ayah != 0) { AyahRepeatCounter += 1; ok = true; ayah(Surah, Ayah); }
            else if (Ayah == AyatCount && SurahRepeatCounter < SurahRepeat - 1) { SurahRepeatCounter += 1; ok = true; ayah(Surah, 0); }
            else
            {
                AyahRepeatCounter = 0;
                if (Ayah == AyatCount && Surah < surahsCount) ayah(Surah + 1, 0);
                else if (Ayah == AyatCount) surah(1);
                else ayah(Surah, Ayah + 1);
            }

        }

        public void ayah() { ayah(Surah, Ayah); }
        public void ayah(int aya) { ayah(Surah, aya); }

        public void ayah(int sura, int aya) {
            if (!success) return;
            timer.Stop();

            sura = Math.Abs(sura);
            if (sura == 0) sura = 1;
            else if (sura > surahsCount) sura = surahsCount;

            if (aya == -1) aya = 0;
            else aya = Math.Abs(aya);

            quran.Open();
            if (sura != Surah || !CapturedAudio || mp3.Ctlcontrols.currentPosition==0) {
                // /*if(!*/ Check(sura); /*) return;*/

                if (!CaptureAudio(sura)) { 
                    
                    mp3.URL = ""; quran.Close(); CapturedAudio = false; return;
                }

                CapturedAudio = true;
                reader = new SQLiteCommand($"SELECT * FROM surahs WHERE id={sura}", quran).ExecuteReader();
                reader.Read();
                Surah = sura;
                AyatCount = reader.GetInt32(2);
            }

            if (aya > AyatCount) aya = AyatCount;
            
            // لم أقم بتصليح مشكلة لو لا يوجد بسملة في اول المقطع
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={aya}", quran).ExecuteReader();
            if (!reader.HasRows && aya == 0) { reader = new SQLiteCommand($"SELECT * FROM ayat WHERE surah={sura} AND ayah={1}", quran).ExecuteReader(); aya = 1; }
            reader.Read();
            To = reader.GetInt32(3);
            //if (To == 0 && aya == 0) To = 1; // هذا ليس الحل الأمثل
            ayahId = reader.GetInt32(0);
            reader = new SQLiteCommand($"SELECT * FROM ayat WHERE id={ayahId - 1}", quran).ExecuteReader();
            reader.Read();
            From = Math.Abs(reader.GetInt32(3));
            Ayah = aya;

            if ((int)((To - From) / mp3.settings.rate) > 0)
                timer.Interval = (int)((To - From) / mp3.settings.rate);
            else timer.Interval = 1;

            if (ok) mp3.Ctlcontrols.currentPosition = From / 1000.0;
            CurrentPosition = From;
            quran.Close();
            ok = true;
            timer.Start();
        }

         bool Check(int surah) {
            if (!CaptureAudio(surah)) return false;
            //quran.Open();
            reader = new SQLiteCommand($"SELECT id,duration FROM surahs WHERE id={surah}", quran).ExecuteReader();
            reader.Read();
            if (reader.IsDBNull(1)) { /*quran.Close();*/ return false; }
            int i = reader.GetInt32(1);
            //quran.Close();
            if (mp3.currentMedia.duration == i) return true;
            return false;
        }

        bool CaptureAudio(int surah){
            string s = surah + "";
            if (s.Length == 1) s = "00" + s;
            else if (s.Length == 2) s = "0" + s;
            s += extension;

            if (File.Exists(path + s)) mp3.URL = path + s;
            else if (File.Exists(path + surah + extension)) mp3.URL = path + surah + extension;
            else return false;
            mp3.settings.rate = rate;
            return true;
        }

        public void Pause() { timer.Stop(); mp3.URL=""; CapturedAudio = false; }
        public void Stop() { success = false; Pause(); }

        double rate=1;
        public void Rate(double i =1.0) {
            if (i > 1.4) rate = 1.4;
            else if (i < 0.6) rate = 0.6;
            else rate=i;
            mp3.settings.rate = rate;
            ayah(Surah, Ayah);
        }

        void timer_Tick(object sender, EventArgs e) { ok = false; AyahPlus(); }

        public void AddEventHandler(EventHandler eh) { timer.Tick += eh; }

        int SurahRepeat=1, AyahRepeat=1, SurahRepeatCounter =0, AyahRepeatCounter =0;
        public void Repeat( int SurahRepeat = 1, int AyahRepeat=1) {
            this.SurahRepeat = (SurahRepeat >= 1) ? SurahRepeat : 1;
            this.AyahRepeat = (AyahRepeat >= 1) ? AyahRepeat : 1;
        }

    }
}
