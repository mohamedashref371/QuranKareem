using System.Drawing;

namespace QuranKareem
{
    public static class Coloring
    {
        public static readonly byte[] AyahColor = { 255, 0, 0, 255 };

        public static readonly byte[] WordColor = { 44, 164, 171, 255 };


        public static void SetAyahColor(string color)
        {
            Color ayahColor = Color.FromName(color);
            AyahColor[0] = ayahColor.R; AyahColor[1] = ayahColor.G; AyahColor[2] = ayahColor.B;
            AyahColor[3] = ayahColor.A;
        }

        public static Color GetAyahColor()
        {
            return Color.FromArgb(AyahColor[3], AyahColor[0], AyahColor[1], AyahColor[2]);
        }

        public static void SetWordColor(string color)
        {
            Color wordColor = Color.FromName(color);
            WordColor[0] = wordColor.R; WordColor[1] = wordColor.G; WordColor[2] = wordColor.B;
            WordColor[3] = wordColor.A;
        }

        public static Color GetWordColor()
        {
            return Color.FromArgb(WordColor[3], WordColor[0], WordColor[1], WordColor[2]);
        }
    }
}
