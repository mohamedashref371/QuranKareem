using System;
using static QuranKareem.VideoXMLProperties;
using static QuranKareem.Constants;

namespace QuranKareem
{
    // download from https://sourceforge.net/projects/vidiot
    internal static class VidiotXmlBuilder
    {
        private static int frameNum = 30;
        private static int frameDiv = 1;
        private static readonly string dataTime = "2023-10-07T06:00:00";
        private static int vOffset, aOffset, length;

        public static string Build()
        {
            StrBuilder.Clear();

            frameNum = (int)FrameRate;
            frameDiv = 1;
            if (FrameRate != frameNum)
            {
                frameNum = (int)Math.Round(FrameRate * 100);
                frameDiv = 100;
            }

            vOffset = (int)(VideoOffsetInSecond * FrameRate);
            aOffset = (int)(AudioOffsetInSecond * FrameRate);
            length = (int)(LengthInSecond * FrameRate);

            XmlStart();

            AddAudioFile();
            AddImageFiles();
            AddVideoFile();

            SequencesStart();

            VideoInitialize();
            ImagesInitialize();
            AudioInitialize();

            Output();

            return StrBuilder.ToString();
        }

        private static void XmlStart()
        {
            
        }

        private static void AddAudioFile()
        {

        }

        private static void AddImageFiles()
        {

        }

        private static void AddVideoFile()
        {
            
        }

        private static void SequencesStart()
        {
            
        }

        private static void VideoInitialize()
        {
            
        }

        private static void ImagesInitialize()
        {

        }

        private static void AudioInitialize()
        {
            
        }

        private static void Output()
        {
            
        }
    }
}
