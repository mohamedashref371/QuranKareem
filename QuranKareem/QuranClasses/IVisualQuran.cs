namespace QuranKareem
{
    internal interface IVisualQuran
    {
        int SurahNumber { get; }
        int QuarterNumber { get; }
        int PageNumber { get; }
        int AyahNumber { get; }
        int AyatCount { get; }
        int CurrentWord { get; }

        bool Start(string path, int sura = 1, int aya = 0);

        string[] GetSurahNames();

        bool Set(int surah = 0, int ayah = -2, bool next = false, int juz = 0, int hizb = 0, int quarter = 0, int page = 0);

        bool WordOf(int word);
    }
}
