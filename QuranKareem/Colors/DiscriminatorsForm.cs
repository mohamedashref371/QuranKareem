using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
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
            DiscriminatorControl d;

            d = new DiscriminatorControl(-3713317, "ألوان أولية", false)
            {
                Location = new Point(12, 12),
            };
            panel.Controls.Add(d);
            d.LightPageColor = Coloring.Light.BackColor;
            d.LightAyahColor = Coloring.Light.AyahColor;
            d.LightAyahColor = Coloring.Light.WordColor;
            d.NightPageColor = Coloring.Night.BackColor;
            d.NightAyahColor = Coloring.Night.AyahColor;
            d.NightAyahColor = Coloring.Night.WordColor;
            //---------------

            Discriminator dis;
            int counter = 1;
            foreach (var desc in Discriminators.Descriptions)
            {
                d = new DiscriminatorControl(desc.Key, desc.Value)
                {
                    Location = new Point(12, counter * (DiscriminatorControl.ControlSize.Height + 20) + 32),
                };
                panel.Controls.Add(d);
                counter++;
                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        dis = Discriminators.WordsDiscriminators.FirstOrDefault(x => x.Id == desc.Key && (x.Lighting == i || x.Lighting == 2) && x.Condition == j);
                        if (dis != null)
                        {
                            if (i == 0 && j == 0) d.LightPageColor = dis.Color;
                            else if (i == 0 && j == 1) d.LightAyahColor = dis.Color;
                            else if (i == 0 && j == 2) d.LightWordColor = dis.Color;
                            else if (i == 1 && j == 0) d.NightPageColor = dis.Color;
                            else if (i == 1 && j == 1) d.NightAyahColor = dis.Color;
                            else if (i == 1 && j == 2) d.NightWordColor = dis.Color;
                        }
                    }
                }
            }
        }

        private void DiscriminatorsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("هل تريد حفظ التغييرات؟", "رسالة", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }

            else if (result == DialogResult.Yes)
            {
                AddIn(Discriminators.WordsDiscriminators);
                DialogResult = DialogResult.Yes;
            }

            else
            {
                DialogResult = DialogResult.No;
            }
        }

        private void AddIn(List<Discriminator> list)
        {
            list.Clear();
            DiscriminatorControl d = (DiscriminatorControl)Controls[0];
            Coloring.Light.BackColor = d.LightPageColor;
            Coloring.Light.AyahColor = d.LightAyahColor;
            Coloring.Light.WordColor = d.LightWordColor;
            Coloring.Night.BackColor = d.NightPageColor;
            Coloring.Night.AyahColor = d.NightAyahColor;
            Coloring.Night.WordColor = d.NightWordColor;
            for (int i = 1; i < panel.Controls.Count; i++)
            {
                d = (DiscriminatorControl)Controls[i];
                if (d.LightPageColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 0, 0, d.LightPageColor));
                if (d.LightAyahColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 0, 1, d.LightAyahColor));
                if (d.LightWordColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 0, 2, d.LightWordColor));
                if (d.NightPageColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 1, 0, d.NightPageColor));
                if (d.NightAyahColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 1, 1, d.NightAyahColor));
                if (d.NightWordColor != Color.Empty)
                    list.Add(new Discriminator(d.Id, 1, 2, d.NightWordColor));
            }
        }

    }
}
