using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuranKareem
{
    internal static class Coloring
    {
        public static Color AyahColor = Color.Red;

        public static Color QuarterStartColor = Color.DarkBlue; // 0 // is unused
        public static Color WordColor = Color.LightSeaGreen; // 1:599
        public static Color SajdaColor = Color.DarkMagenta; // 998 // is unused
        public static Color AyahEndColor = Color.DarkGreen; // 999

        public static Color GetColor(string s)
        {
            s = s.Replace(" ", "");
            //if (s == "AyahColor") return AyahColor;
            //if (s == "WordColor") return WordColor;

            if (s.Contains(","))
            {
                string[] arr = s.Split(',');
                if ((arr.Length == 3 || arr.Length == 4) && int.TryParse(arr[0], out int n1) && int.TryParse(arr[1], out int n2) && int.TryParse(arr[2], out int n3))
                {
                    if (arr.Length == 4 && int.TryParse(arr[0], out int n4))
                        return Color.FromArgb(n4, n1, n2, n3);
                    return Color.FromArgb(n1, n2, n3);
                }
                else
                    return Color.Empty;
            }
            else if (int.TryParse(s, out int num))
                return Color.FromArgb(num, num, num);
            else
            {
                Color clr = Color.FromName(s);
                return clr.IsKnownColor  || clr.Name == "AyahColor" || clr.Name == "WordColor" ? clr : Color.Empty;
            }
        }
    }
}
