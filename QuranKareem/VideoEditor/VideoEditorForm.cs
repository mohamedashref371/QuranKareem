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
        private static int width = 1920, height = 1080, x = 100, y = 588, lWidth = 1720, lHeight = 200;
        private static float frames = 25f;

        public VideoEditorForm(int quranClass)
        {
            InitializeComponent();
            this.quranClass = quranClass;
            videoWidth.Value = width; videoHeight.Value = height;
            frameRate.Value = (decimal)frames;
            locX.Value = x; locY.Value = y;
            lineWidth.Value = lWidth; lineHeight.Value = lHeight;
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
            width = (int)videoWidth.Value;
            VidiotXmlBuilder.VideoWidth = width;
        }

        private void VideoHeight_ValueChanged(object sender, EventArgs e)
        {
            height = (int)videoHeight.Value;
            VidiotXmlBuilder.VideoHeight = height;
        }

        private void FrameRate_ValueChanged(object sender, EventArgs e)
        {
            frames = (float)frameRate.Value;
            VidiotXmlBuilder.FrameRate = frames;
        }

        private void AudioBitRate_ValueChanged(object sender, EventArgs e)
        {
            VidiotXmlBuilder.AudioBitRate = (int)audioBitRate.Value;
        }

        private void VidiotLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://sourceforge.net/projects/vidiot");
        }

        private void YEditCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (yEditCheck.Visible)
                lineHeight.Enabled = yEditCheck.Checked;
        }

        private void LineHeightAuto_CheckedChanged(object sender, EventArgs e)
        {
            lineHeight.Enabled = !lineHeightAuto.Checked;
            yEditCheck.Visible = lineHeightAuto.Checked;
            if (!yEditCheck.Visible)
                yEditCheck.Checked = false;
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
                lineHeightAuto.Checked, yEditCheck.Checked,
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
                lineHeightAuto.Checked, yEditCheck.Checked,
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

        private void LocX_ValueChanged(object sender, EventArgs e)
        {
            x = (int)locX.Value;
        }

        private void LocY_ValueChanged(object sender, EventArgs e)
        {
            y = (int)locY.Value;
        }

        private void LineWidth_ValueChanged(object sender, EventArgs e)
        {
            lWidth = (int)lineWidth.Value;
        }

        private void LineHeight_ValueChanged(object sender, EventArgs e)
        {
            lHeight = (int)lineHeight.Value;
        }
    }
}
