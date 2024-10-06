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

        public event EventHandler FormClosedWithYesResult;

        private int dCount;
        private DiscriminatorControl d;
        public void InitializeDiscriminator()
        {
            shouldClose = false;
            if (panel.Controls.Count == 0)
            {
                d = new DiscriminatorControl(-3713317, "ألوان أولية", false)
                {
                    Location = new Point(12, 12),
                    BorderStyle = BorderStyle.FixedSingle
                };
                panel.Controls.Add(d);
            }
            else d = (DiscriminatorControl)panel.Controls[0];

            d.LightPageColor = Coloring.Light.BackColor;
            d.LightAyahColor = Coloring.Light.AyahColor;
            d.LightWordColor = Coloring.Light.WordColor;
            d.NightPageColor = Coloring.Night.BackColor;
            d.NightAyahColor = Coloring.Night.AyahColor;
            d.NightWordColor = Coloring.Night.WordColor;
            //---------------

            Discriminator dis;
            int counter = 0;
            Color clr;

            foreach (var desc in Discriminators.Descriptions)
            {
                counter++;
                if (counter > dCount)
                {
                    d = new DiscriminatorControl(desc.Key, desc.Value)
                    {
                        Location = new Point(12, counter * (DiscriminatorControl.ControlSize.Height + 20) + 32),
                    };
                    panel.Controls.Add(d);
                    dCount = counter;
                }
                else
                {
                    d = (DiscriminatorControl)panel.Controls[counter];
                    d.Id = desc.Key;
                    d.WordText = desc.Value;
                    d.Visible = true;
                }

                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        dis = Discriminators.WordsDiscriminators.FirstOrDefault(x => x.Id == desc.Key && (x.Lighting == i || x.Lighting == 2) && x.Condition == j);
                        clr = dis == null ? Color.Empty : dis.Color;

                        if (i == 0 && j == 0) d.LightPageColor = clr;
                        else if (i == 0 && j == 1) d.LightAyahColor = clr;
                        else if (i == 0 && j == 2) d.LightWordColor = clr;
                        else if (i == 1 && j == 0) d.NightPageColor = clr;
                        else if (i == 1 && j == 1) d.NightAyahColor = clr;
                        else if (i == 1 && j == 2) d.NightWordColor = clr;
                    }
                }
            }

            while (counter < dCount)
            {
                counter++;
                d = (DiscriminatorControl)panel.Controls[counter];
                d.Visible = false;
            }
        }

        private bool shouldClose = false;
        private void DiscriminatorsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (shouldClose) return;
            DialogResult result = MessageBox.Show("هل تريد حفظ التغييرات؟", "رسالة", MessageBoxButtons.YesNoCancel);
            e.Cancel = true;

            if (result == DialogResult.Yes)
            {
                AddIn(Discriminators.WordsDiscriminators);
                FormClosedWithYesResult?.Invoke(this, e);
            }
            if (result != DialogResult.Cancel)
            {
                shouldClose = true;
                Owner = null;
                this.Hide();
            }
        }

        private void AddIn(List<Discriminator> list)
        {
            list.Clear();
            DiscriminatorControl d = (DiscriminatorControl)panel.Controls[0];
            Coloring.Light.BackColor = d.LightPageColor;
            Coloring.Light.AyahColor = d.LightAyahColor;
            Coloring.Light.WordColor = d.LightWordColor;
            Coloring.Night.BackColor = d.NightPageColor;
            Coloring.Night.AyahColor = d.NightAyahColor;
            Coloring.Night.WordColor = d.NightWordColor;
            for (int i = 1; i < panel.Controls.Count; i++)
            {
                d = (DiscriminatorControl)panel.Controls[i];
                if (!d.Visible) break;

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
