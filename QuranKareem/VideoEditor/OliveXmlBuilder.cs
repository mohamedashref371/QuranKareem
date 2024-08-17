﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using static QuranKareem.VideoXMLProperties;

namespace QuranKareem
{
    internal class OliveXmlBuilder
    {
        private static readonly StringBuilder sb = new StringBuilder();

        public static string Build()
        {
            sb.Clear();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><project>");

            AddMedia();

            Addsequence();

            sb.Append("</project>");

            return sb.ToString();
        }

        public static void AddMedia()
        {
            sb.Append($"<media><footage id=\"1\" folder=\"0\" name=\"QuranTemplate\" url=\"{VideoPath}\"></footage><footage id=\"2\" folder=\"0\" name=\"QuranAudio\" url=\"{AudioPath}\"></footage>");

            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                sb.Append($"<footage id=\"{3 + i}\" folder=\"0\" name=\"{Path.GetFileName(ImagesPaths[i])}\" url=\"{ImagesPaths[i]}\"></footage>");
            }

            sb.Append("</media>");
        }

        public static void Addsequence()
        {
            sb.Append($"<sequences><sequence id=\"1\" folder=\"0\" name=\"{ProjectName}\" width=\"{VideoWidth}\" height=\"{VideoHeight}\" framerate=\"{FrameRate}\" afreq=\"{AudioSampleRate}\" alayout=\"3\" open=\"1\" workarea=\"0\" workareaIn=\"0\" workareaOut=\"0\">");

            sb.Append($"<clip id=\"1\" enabled=\"1\" name=\"QuranTemplate\" clipin=\"{(int)(VideoOffsetInSecond * FrameRate)}\" in=\"0\" out=\"{(int)(LengthInSecond * FrameRate)}\" track=\"-2\" media=\"1\" stream=\"0\"></clip><clip id=\"2\" enabled=\"1\" name=\"QuranAudio\" clipin=\"{(int)(AudioOffsetInSecond * FrameRate)}\" in=\"0\" out=\"{(int)(LengthInSecond * FrameRate)}\" track=\"0\" media=\"2\" stream=\"0\"></clip>");

            float sumIn = 0, sumOut;
            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                sumOut = sumIn + AudioTimestamps[i];
                sb.Append($"<clip id=\"{3 + i}\" enabled=\"1\" name=\"{Path.GetFileName(ImagesPaths[i])}\" clipin=\"0\" in=\"{(int)(sumIn * FrameRate)}\" out=\"{(int)(sumOut * FrameRate)}\" track=\"-4\" media=\"{3 + i}\" stream=\"0\"></clip>");
                sumIn = sumOut;
            }

            sb.Append("</sequence></sequences>");
        }

    }
}
