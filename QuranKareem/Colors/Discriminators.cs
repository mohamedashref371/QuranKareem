using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using static QuranKareem.Constants;

namespace QuranKareem
{
    internal static class Discriminators
    {
        public static readonly Dictionary<int, string> Descriptions = new Dictionary<int, string>();
        public static readonly Dictionary<int, Color> PageColors = new Dictionary<int, Color>();
        public static readonly Dictionary<int, Color> AyahColors = new Dictionary<int, Color>();
        public static readonly Dictionary<int, Color> WordColors = new Dictionary<int, Color>();

        public static readonly List<Discriminator> WordsDiscriminators = new List<Discriminator>();

        public static bool ActiveDiscriminators(bool darkMode = false)
        {
            PageColors.Clear();
            AyahColors.Clear();
            WordColors.Clear();
            foreach (var d in WordsDiscriminators)
            {
                if ((d.Lighting == 0 && !darkMode || d.Lighting == 1 && darkMode || d.Lighting == 2) && !d.Color.IsEmpty)
                {
                    if (d.Condition == 0)
                        PageColors[d.Id] = d.Color;
                    else if (d.Condition == 1)
                        AyahColors[d.Id] = d.Color;
                    else if (d.Condition == 2)
                        WordColors[d.Id] = d.Color;
                }
            }
            PageColorsKeys = PageColors.Keys.ToArray();
            AyahColorsKeys = AyahColors.Keys.ToArray();
            WordColorsKeys = WordColors.Keys.ToArray();
            return WordColors.Count != 0;
        }

        public static void GetDiscriminators(string file)
            => Discriminator.GetDiscriminators(WordsDiscriminators, File.Exists(file) ? File.ReadAllText(file) : "");

        public static void SetDiscriminators(string file)
            => File.WriteAllText(file, Discriminator.GetText(WordsDiscriminators));

        public static string GetPageKeysAsString()
            => GetKeysAsString(PageColorsKeys);

        public static string GetAyahKeysAsString()
            => GetKeysAsString(AyahColorsKeys);

        public static string GetWordKeysAsString()
            => GetKeysAsString(WordColorsKeys);

        private static string GetKeysAsString(int[] keys)
        {
            StrBuilder.Length = 0;
            if (keys.Length > 0) StrBuilder.Append(keys[0].ToString());
            for (int i = 1; i < keys.Length; i++)
                StrBuilder.Append(',').Append(keys[i].ToString());
            return StrBuilder.ToString();
        }

        private static int[] PageColorsKeys, AyahColorsKeys, WordColorsKeys;
        public static bool KeyExists(int level, int key)
        {
            if (level == 0)
                return PageColorsKeys.Contains(key);
            if (level == 1)
                return AyahColorsKeys.Contains(key);
            if (level == 2)
                return WordColorsKeys.Contains(key);
            return false;
        }
    }
}
