using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuranKareem
{
    internal static class VideoXMLProperties
    {
        public static readonly string ProjectName = Application.ProductName + Application.ProductVersion;

        public static int VideoWidth { get; set; } = 1920;
        public static int VideoHeight { get; set; } = 1080;
        public static float FrameRate { get; set; } = 30f;
        public static int AudioChannels { get; set; } = 2;
        public static int AudioSampleRate { get; set; } = 44100;
        public static int AudioBitRate { get; set; } = 128000;

        private static string videoPath = "Video.mp4";
        public static string VideoPath
        {
            get => videoPath;
            set { if (value != null && value != "") videoPath = value; }
        }

        public static float VideoOffsetInSecond { get; set; } = 0;

        private static string audioPath = "Audio.mp4";
        public static string AudioPath
        {
            get => audioPath;
            set { if (value != null && value != "") audioPath = value; }
        }

        public static float AudioOffsetInSecond { get; set; } = 0;
        public static float LengthInSecond { get; set; } = 1000;

        public static readonly List<string> ImagesPaths = new List<string>();
        public static readonly List<float> AudioTimestamps = new List<float>();

        private static string outputPath = "Output.mp4";
        public static string OutputPath
        {
            get => outputPath;
            set { if (value != null && value != "") outputPath = value; }
        }
    }
}
