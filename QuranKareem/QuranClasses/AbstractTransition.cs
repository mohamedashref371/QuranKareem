using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranKareem
{
    internal abstract class AbstractTransition : AbstractBase
    {

        protected int surahsCount;

        public int SurahNumber { get; protected set; }
        public int AyahNumber { get; protected set; }
        public int AyatCount { get; protected set; }

        protected AbstractTransition() : base()
        {
        }

        public void Surah(int i) => Ayah(i, 0);
        public abstract void AyahPlus();
        public void Ayah() => Ayah(SurahNumber, AyahNumber);
        public void Ayah(int aya) => Ayah(SurahNumber, aya);
        public abstract void Ayah(int sura, int aya);
    }
}
