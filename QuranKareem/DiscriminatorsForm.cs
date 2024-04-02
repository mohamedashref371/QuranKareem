using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    public partial class DiscriminatorsForm : Form
    {
        public DiscriminatorsForm()
        {
            InitializeComponent();
        }
        
        private void DiscriminatorsForm_Load(object sender, EventArgs e)
        {
            DiscriminatorControl d = new DiscriminatorControl(1, "test");
            d.Location = new Point(0, 0);
            Controls.Add(d);

            d.LightPageColor = Color.FromName("AyahColor");

            d.LightPageColor = Color.Red;

        }
    }
}
