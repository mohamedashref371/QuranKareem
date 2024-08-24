using System;
using System.Drawing;
using System.IO;

namespace QuranKareem
{
    internal static class Coloring
    {
        public static class Light
        {
            public static Color BackColor = Color.Transparent;
            public static Color AyahColor = Color.Red;
            public static Color WordColor = Color.LightSeaGreen;
        }
        public static class Night
        {
            public static Color BackColor = Color.Transparent;
            public static Color AyahColor = Color.Red;
            public static Color WordColor = Color.LightSeaGreen;
        }

        public static Color QuarterStartColor = Color.DarkBlue; // 0 // is unused
        public static Color SajdaColor = Color.DarkMagenta; // 998 // is unused
        public static Color AyahEndColor = Color.DarkGreen; // 999 // used in TextQuran

        public static void GetInitialColors(string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] arr = File.ReadAllText(filePath).Replace(" ", "").Split('*');
                if (arr.Length == 1 || arr.Length == 2 || arr.Length == 4)
                {
                    Light.BackColor = Color.Transparent;
                    Light.AyahColor = GetColor(arr[0]);

                    if (arr.Length == 1)
                        Light.WordColor = Color.Empty;
                    else
                        Light.WordColor = GetColor(arr[1]);


                    Night.BackColor = Color.Transparent;

                    if (arr.Length < 4)
                        Night.AyahColor = GetColor(arr[0]);
                    else
                        Night.AyahColor = GetColor(arr[2]);

                    if (arr.Length == 1)
                        Night.WordColor = Color.Empty;
                    else if (arr.Length == 2)
                        Night.WordColor = GetColor(arr[1]);
                    else
                        Night.WordColor = GetColor(arr[3]);
                }

                else if (arr.Length == 3 || arr.Length == 5)
                {
                    Light.BackColor = GetColor(arr[0]);
                    Light.AyahColor = GetColor(arr[1]);
                    Light.WordColor = GetColor(arr[2]);


                    Night.BackColor = GetColor(arr[0]);

                    if (arr.Length == 3)
                    {
                        Night.AyahColor = GetColor(arr[1]);
                        Night.WordColor = GetColor(arr[2]);
                    }
                    else
                    {
                        Night.AyahColor = GetColor(arr[3]);
                        Night.WordColor = GetColor(arr[4]);
                    }
                }

                else if (arr.Length >= 6)
                {
                    Light.BackColor = GetColor(arr[0]);
                    Light.AyahColor = GetColor(arr[1]);
                    Light.WordColor = GetColor(arr[2]);

                    Night.BackColor = GetColor(arr[3]);
                    Night.AyahColor = GetColor(arr[4]);
                    Night.WordColor = GetColor(arr[5]);
                }
            }
        }

        public static void SetInitialColors(string filePath)
        {
            Constants.StringBuilder.Length = 0;

            Constants.StringBuilder
                .Append(GetString(Light.BackColor, true))
                .Append("*")
                .Append(GetString(Light.AyahColor, true))
                .Append("*")
                .Append(GetString(Light.WordColor, true))
                .Append("*")
                .Append(GetString(Night.BackColor, true))
                .Append("*")
                .Append(GetString(Night.AyahColor, true))
                .Append("*")
                .Append(GetString(Night.WordColor, true))
                ;

            string s = Constants.StringBuilder.ToString();
            if (!File.Exists(filePath) || File.ReadAllText(filePath) != s)
                File.WriteAllText(filePath, s);
        }

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
                if (arr.Length == 3  && byte.TryParse(arr[0], out byte n1) && byte.TryParse(arr[1], out byte n2) && byte.TryParse(arr[2], out byte n3))
                    return Color.FromArgb(n1, n2, n3);
                
                else if (arr.Length == 4 && byte.TryParse(arr[0], out byte m1) && byte.TryParse(arr[1], out byte m2) && byte.TryParse(arr[2], out byte m3) && byte.TryParse(arr[3], out byte m4))
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
                return c - '0';

            else if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            
            else if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;
            
            else
                return 0;
        }
    }
}
