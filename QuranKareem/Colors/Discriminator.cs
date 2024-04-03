using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    public class Discriminator
    {
        public int Id;
        public int Lighting; // Light=0, Night=1, Both=2
        public int Condition; // Page=0, Ayah=1, Word=2
        public Color Color;

        public Discriminator(int id, int light, int cond, Color color)
        {
            Id = id;
            Lighting = light;
            Condition = cond;
            Color = color;
        }

        public static void GetDiscriminators(List<Discriminator> list, string s)
        {
            list.Clear();
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
        }

        public static string GetText(List<Discriminator> list)
        {
            StringBuilder sb = new StringBuilder();
            string s;
            for (int i = 0; i < list?.Count; i++)
            {
                s = list[i].ToString();
                if (s != "")
                {
                    sb.Append(s);
                    if (i < list.Count - 1) sb.Append("*");
                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return !Color.IsEmpty ? $"{Id}|{Lighting}|{Condition}|{Coloring.GetString(Color, true)}" : "";
        }
    }
}
