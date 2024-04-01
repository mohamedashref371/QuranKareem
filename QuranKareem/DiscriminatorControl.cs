using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class DiscriminatorControl : Control
    {
        public readonly int Id;
        public DiscriminatorControl(int id, string s)
        {
            Id = id;
            Label lbl = new Label()
            {
                Text = s
            };
        }
    }
}
