using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranKareem
{
    public abstract class AbsQuranBase
    {
        protected bool success = false;

        protected readonly SQLiteConnection quran;
        protected readonly SQLiteCommand command;
        protected SQLiteDataReader reader;

        protected int version;

        public int Narration { get; protected set; }
        public string Comment { get; protected set; }

        protected AbsQuranBase()
        {
            quran = new SQLiteConnection();
            command = new SQLiteCommand(quran);
        }

        public abstract void Start(string path, int sura = 1, int aya = 0);

        protected void Close()
        {
            reader.Close();
            command.Cancel();
        }
        protected void CloseConn()
        {
            reader.Close();
            quran.Close();
        }
    }
}
