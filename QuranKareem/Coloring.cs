using System.Drawing;

namespace QuranKareem
{
    public static class Coloring
    {
        public static Color AyahColor = Color.Red;
        public static void SetAyahColor(string color) => AyahColor = Color.FromName(color);

        public static Color WordColor = Color.LightSeaGreen;
        public static void SetWordColor(string color) => WordColor = Color.FromName(color);
        
    }
}
