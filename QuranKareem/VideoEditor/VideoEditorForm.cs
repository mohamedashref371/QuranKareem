﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        private readonly List<int> surahayah = new List<int>();
        private void VideoEditorForm_Load(object sender, EventArgs e)
        {
            int[] minmax = PictureQuran.Instance.GetStartAndEndOfPage();
            float[] minmax2 = AudioQuran.Instance.WordsList(minmax[0], minmax[1], out string mp3Url, surahayah, VidiotXmlBuilder.AudioTimestamps);

            VidiotXmlBuilder.AudioOffsetInSecond = minmax2[0];
            VidiotXmlBuilder.LengthInSecond = minmax2[1] - minmax2[0];
            VidiotXmlBuilder.AudioPath = mp3Url;

            audioLength.Text = AudioQuran.Instance.GetPositionOf((int)((minmax2[1] - minmax[0]) * 1000));
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

        private void Generate_Click(object sender, EventArgs e)
        {
            string path = "videos\\" + DateTime.Now.Ticks.ToString();
            PictureQuran.Instance.GetAyatInLinesWithWordsMarks(
                surahayah,
                VidiotXmlBuilder.VideoWidth, VidiotXmlBuilder.VideoHeight,
                (int)locX.Value, (int)locY.Value, (int)lineWidth.Value, (int)lineHeight.Value,
                path,
                VidiotXmlBuilder.ImagesPaths
                );

            File.WriteAllText(path + $"\\QuranKareem.vid", VidiotXmlBuilder.Build());
            Process.Start(path);
        }
    }
}
