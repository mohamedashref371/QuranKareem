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
        private List<Discriminator> list;
        public DiscriminatorsForm(List<Discriminator> lst)
        {
            if (lst == null) { Close(); return; }
            list = lst;
            InitializeComponent();
        }

        private void DiscriminatorsForm_Load(object sender, EventArgs e)
        {
            Discriminator dis;
            DiscriminatorControl d; int counter = 0;
            foreach (var desc in Discriminators.Descriptions)
            {
                d = new DiscriminatorControl(desc.Key, desc.Value)
                {
                    Location = new Point(12, counter * (DiscriminatorControl.ControlSize.Height + 20)),
                };
                panel.Controls.Add(d);
                counter++;
                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        dis = list.FirstOrDefault(x => x.Id == desc.Key && (x.Lighting == i || x.Lighting == 2) && x.Condition == j);
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
                list.Clear();
                foreach (DiscriminatorControl d in panel.Controls)
                {
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
}
