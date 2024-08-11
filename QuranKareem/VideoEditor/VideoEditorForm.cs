using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace QuranKareem
{
    public partial class VideoEditorForm : Form
    {
        private int quranClass;
        public VideoEditorForm(int quranClass)
        {
            InitializeComponent();
            this.quranClass = quranClass;
        }

        private void VideoPath_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                VidiotXmlBuilder.VideoPath = openFileDialog.FileName;
                videoNameLabel.Text = openFileDialog.FileName;
            }
        }

        private void VideoOutputPath_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                VidiotXmlBuilder.OutputPath = saveFileDialog.FileName;
                videoOutputNameLabel.Text = saveFileDialog.FileName;
            }
        }

        private readonly List<int> ayahword = new List<int>();
        private Bitmap bitmap;
        private int surah, page;
        private void VideoEditorForm_Load(object sender, EventArgs e)
        {
            int[] minmax;
            if (quranClass == 2)
                minmax = TrueTypeFontQuran.Instance.GetStartAndEndOfPage();
            else
                minmax = PictureQuran.Instance.GetStartAndEndOfPage();
            
            float[] minmax2 = AudioQuran.Instance.WordsList(minmax[0], minmax[1], out string mp3Url, ayahword, VidiotXmlBuilder.AudioTimestamps);

            if (minmax2 == null)
            {
                MessageBox.Show("لم نستطع إمساك سورة أو غير متوفر تحديد الكلمات لهذا الشيخ.");
                Close();
                return;
            }
            if (quranClass == 2)
            {
                surah = TrueTypeFontQuran.Instance.SurahNumber;
                page = TrueTypeFontQuran.Instance.PageNumber;
            }
            else
            {
                surah = PictureQuran.Instance.SurahNumber;
                page = PictureQuran.Instance.PageNumber;
                bitmap = (Bitmap)PictureQuran.Instance.PagePicture.Clone();
            }

            VidiotXmlBuilder.AudioPath = mp3Url;
            VidiotXmlBuilder.AudioOffsetInSecond = minmax2[0];
            VidiotXmlBuilder.LengthInSecond = minmax2[1] - minmax2[0];
            
            audioLength.Text = AudioQuran.Instance.GetPositionOf((int)((minmax2[1] - minmax2[0]) * 1000));
        }

        private void VideoWidth_ValueChanged(object sender, EventArgs e)
        {
            VidiotXmlBuilder.VideoWidth = (int)videoWidth.Value;
        }

        private void VideoHeight_ValueChanged(object sender, EventArgs e)
        {
            VidiotXmlBuilder.VideoHeight = (int)videoHeight.Value;
        }

        private void FrameRate_ValueChanged(object sender, EventArgs e)
        {
            VidiotXmlBuilder.FrameRate = (int)frameRate.Value;
        }

        private void AudioBitRate_ValueChanged(object sender, EventArgs e)
        {
            VidiotXmlBuilder.AudioBitRate = (int)audioBitRate.Value;
        }

        private void VidiotLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://sourceforge.net/projects/vidiot");
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            string path = "videos\\" + DateTime.Now.Ticks.ToString();
            if (quranClass == 2)
            {
                TrueTypeFontQuran.Instance.GetAyatInLinesWithWordsMarks(
                ayahword,
                VidiotXmlBuilder.VideoWidth, VidiotXmlBuilder.VideoHeight,
                (int)locX.Value, (int)locY.Value, (int)lineWidth.Value, (int)lineHeight.Value,
                path,
                VidiotXmlBuilder.ImagesPaths,
                surah, page
                );
            }
            else
            {
                PictureQuran.Instance.GetAyatInLinesWithWordsMarks(
                ayahword,
                VidiotXmlBuilder.VideoWidth, VidiotXmlBuilder.VideoHeight,
                (int)locX.Value, (int)locY.Value, (int)lineWidth.Value, (int)lineHeight.Value,
                path,
                VidiotXmlBuilder.ImagesPaths,
                bitmap, surah, page
                );
            }
                
            if (VidiotXmlBuilder.ImagesPaths.Count != 0)
            {
                File.WriteAllText(path + $"\\QuranKareem.vid", VidiotXmlBuilder.Build());
                Process.Start(path);
            }
            else
                MessageBox.Show("هناك خطأ، يحتمل أن المصحف لا يدعم تحديد الكلمات");
        }
    }
}
