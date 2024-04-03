using System;
using System.Collections.Generic;
using System.Drawing;
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
    }
}
