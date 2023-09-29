using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuranKareem
{
    class FormSize
    {
        private readonly double xDiv, yDiv;

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

        public int GetNewX(int x)
        {
            return Round(x * xDiv);
        }

        public int GetNewY(int y)
        {
            return Round(y * yDiv);
        }

        public static int Round(double num)
        {
            return (int)Math.Round(num);
        }
    }
}
