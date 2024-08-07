using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return WordColors.Count != 0;
        }

        public static void GetDiscriminators(string file)
            => Discriminator.GetDiscriminators(WordsDiscriminators, File.Exists(file) ? File.ReadAllText(file) : "");

        public static void SetDiscriminators(string file)
            => File.WriteAllText(file, Discriminator.GetText(WordsDiscriminators));

        public static string GetPageKeysAsString()
            => GetKeysAsString(PageColors.Keys.ToArray());

        public static string GetAyahKeysAsString()
            => GetKeysAsString(AyahColors.Keys.ToArray());

        public static string GetWordKeysAsString()
            => GetKeysAsString(WordColors.Keys.ToArray());

        public static StringBuilder sb = new StringBuilder();
        private static string GetKeysAsString(int[] keys)
        {
            sb.Length = 0;
            if (keys.Length > 0) sb.Append(keys[0].ToString());
            for (int i = 1; i < keys.Length; i++)
                sb.Append(',').Append(keys[i].ToString());
            return sb.ToString();
        }
    }
}
