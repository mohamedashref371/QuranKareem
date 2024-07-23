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
    public partial class VideoEditorForm : Form
    {
        public VideoEditorForm()
        {
            InitializeComponent();
        }

        private void VideoPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                VidiotXmlBuilder.VideoPath = openFileDialog.FileName;
        }

        private void VideoOutputPath_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                VidiotXmlBuilder.OutputPath = saveFileDialog.FileName;
        }
    }
}
