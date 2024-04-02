using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranKareem
{
    internal class Discriminator
    {
        public int Id;
        public int Light_Night_Both;
        public int Page_Ayah_Word;
        public Color Color;

        public Discriminator(int id, int light, int cond, Color color)
        {
            Id = id;
            Light_Night_Both = light;
            Page_Ayah_Word = cond;
            Color = color;
        }

        public static Discriminator[] GetDiscriminators(string s)
        {
            var list = new List<Discriminator>();
            string[] arr = s.Replace(" ", "").Split('*');
            string[] childs;
            for (int i = 0; i < arr.Length; i++)
            {
                childs = arr[i].Split('|');
                if (childs.Length == 4 && int.TryParse(childs[0], out int id) && int.TryParse(childs[1], out int lighting) && int.TryParse(childs[2], out int condition))
                {
                    Color clr = Coloring.GetColor(childs[3]);
                    if (!clr.IsEmpty)
                        list.Add(new Discriminator(id, lighting, condition, clr));
                }
            }
            return list.ToArray();
        }

        public static string GetText(Discriminator[] dis)
        {
            StringBuilder sb = new StringBuilder();
            string s;
            for (int i = 0; i < dis?.Length; i++)
            {
                s = dis[i].ToString();
                if (s != "")
                {
                    sb.Append(s);
                    if (i < dis.Length - 1) sb.Append("*");
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return !Color.IsEmpty ? $"{Id}|{Light_Night_Both}|{Page_Ayah_Word}|{Coloring.GetString(Color, true)}" : "";
        }
    }
}
