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
            Panel panel = new Panel()
            {
                Location = new Point(0, 0),
                Size = new Size(DiscriminatorControl.ControlSize.Width, (DiscriminatorControl.ControlSize.Height + 20) * Discriminators.Descriptions.Count)
            };
            Controls.Add(panel);


            DiscriminatorControl d; int counter = 0;
            foreach (var desc in Discriminators.Descriptions)
            {
                d = new DiscriminatorControl(desc.Key, desc.Value)
                {
                    Location = new Point(0, counter * (DiscriminatorControl.ControlSize.Height + 20)),
                    LightAyahColor = Color.FromName("AyahColor"),
                    LightWordColor = Color.FromName("WordColor"),
                    NightAyahColor = Color.FromName("AyahColor"),
                    NightWordColor = Color.FromName("WordColor")
                };
                panel.Controls.Add(d);

            }
        }

    }
}
