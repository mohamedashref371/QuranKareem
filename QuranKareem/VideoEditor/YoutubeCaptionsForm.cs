using System;
using System.Windows.Forms;

namespace QuranKareem
{
    public partial class YoutubeCaptionsForm : Form
    {
        private int quranClass;
        public YoutubeCaptionsForm(int quranClass)
        {
            InitializeComponent();
            this.quranClass = quranClass;
        }

        private void VttPath_Click(object sender, EventArgs e)
        {
            if (openVttDialog.ShowDialog() == DialogResult.OK)
            {
                if (saveTextDialog.ShowDialog() == DialogResult.OK)
                {
                    YoutubeCaptions.SetCaptionsVTT(openVttDialog.FileName, saveTextDialog.FileName);
                    txtNameLabel.Text = saveTextDialog.FileName;
                }
            }
        }

        private void TextPath_Click(object sender, EventArgs e)
        {
            if (openTextDialog.ShowDialog() == DialogResult.OK)
            {
                txtNameLabel.Text = openTextDialog.FileName;
                YoutubeCaptions.TextPath = txtNameLabel.Text;
            }
        }

        private void Mp3Path_Click(object sender, EventArgs e)
        {
            if (openMp3Dialog.ShowDialog() == DialogResult.OK)
            {
                YoutubeCaptions.Mp3Url = openMp3Dialog.FileName;
                mp3NameLabel.Text = YoutubeCaptions.Mp3Url;
            }
        }

        private void VideoEditor_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(YoutubeCaptions.TextPath))
                new VideoEditorForm(quranClass, true).ShowDialog();
        }

    }
}
