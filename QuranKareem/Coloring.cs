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

        public static string GetString(Color clr, bool isHexa = false)
        {
            if (clr.IsEmpty)
                return "EmptyColor";
            else if (clr.IsKnownColor)
                return clr.Name;
            else if (!isHexa)
                return $"{clr.R},{clr.G},{clr.B}";
            else
                return clr.Name;
        }

        public static Color GetColor(string s)
        {
            s = s.Replace(" ", "");

            if (s.Contains(","))
            {
                string[] arr = s.Split(',');
                if (arr.Length == 3  && int.TryParse(arr[0], out int n1) && int.TryParse(arr[1], out int n2) && int.TryParse(arr[2], out int n3))
                    return Color.FromArgb(n1, n2, n3);
                
                else if (arr.Length == 4 && int.TryParse(arr[0], out int m1) && int.TryParse(arr[1], out int m2) && int.TryParse(arr[2], out int m3) && int.TryParse(arr[3], out int m4))
                    return Color.FromArgb(m1, m2, m3, m4);
                
                else
                    return Color.Empty;
            }
            else if (IsHexColor(s))
                return GetHexColor(s);
            
            else if (int.TryParse(s, out int num))
                return Color.FromArgb(num);
            else
            {
                Color clr = Color.FromName(s);
                return clr.IsKnownColor || clr.Name == "AyahColor" || clr.Name == "WordColor" ? clr : Color.Empty;
            }
        }
        
        static bool IsHexColor(string s)
        {
            if (s.Length == 3 || s.Length == 4 || s.Length == 6 || s.Length == 8)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (!(s[i] >= '0' && s[i] <= '9' || s[i] >= 'A' && s[i] <= 'F' || s[i] >= 'a' && s[i] <= 'f'))
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        static Color GetHexColor(string s)
        {
            if (s.Length == 3)
                return Color.FromArgb(HexToInt(s[0] + "" + s[0]), HexToInt(s[1] + "" + s[1]), HexToInt(s[2] + "" + s[2]));

            else if (s.Length == 4)
                return Color.FromArgb(HexToInt(s[0] + "" + s[0]), HexToInt(s[1] + "" + s[1]), HexToInt(s[2] + "" + s[2]), HexToInt(s[3] + "" + s[3]));

            else if (s.Length == 6)
                return Color.FromArgb(HexToInt(s[0] + "" + s[1]), HexToInt(s[2] + "" + s[3]), HexToInt(s[4] + "" + s[5]));

            else if (s.Length == 8)
                return Color.FromArgb(HexToInt(s[0] + "" + s[1]), HexToInt(s[2] + "" + s[3]), HexToInt(s[4] + "" + s[5]), HexToInt(s[6] + "" + s[7]));
            else
                return Color.Empty;
        }

        static int HexToInt(string s)
        {
            int num = 0; int n = 0;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                num += HexToInt(s[i]) * (int)Math.Pow(16, n);
                n++;
            }
            return num;
        }

        static int HexToInt(char c)
        {
            if (c >= '0' && c <= '9')
                return int.Parse(c.ToString());

            if (c >= 'A' && c >= 'F')
                c = char.ToLower(c);

            if (c >= 'a')
                return 10;
            else if (c >= 'b')
                return 11;
            else if (c >= 'c')
                return 12;
            else if (c >= 'd')
                return 13;
            else if (c >= 'e')
                return 14;
            else if (c >= 'f')
                return 15;
            else
                return 0;
        }
    }
}
