using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    class FormSize
    {
        
        private double xDiv,yDiv;

        public FormSize(int oldSizeX, int oldSizeY, int newSizeX, int newSizeY)
        {
            xDiv = newSizeX / (double)oldSizeX;
            yDiv = newSizeY / (double)oldSizeY;
        }

        public void SetControl(Control control, bool font=true)
        {
            control.Location = new Point(Round(control.Location.X * xDiv), Round(control.Location.Y * yDiv));
            control.Size = new Size(Round(control.Size.Width * xDiv), Round(control.Size.Height * yDiv));
            if (font) control.Font = new Font(control.Font.FontFamily, Round(control.Font.Size * xDiv));
        }

        public void SetControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls) SetControl(control);
        }

        public static int Round(double num)
        {
            return (int)Math.Round(num);
        }
    }
}
