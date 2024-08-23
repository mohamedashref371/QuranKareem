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

        private static readonly string VideoColor = "r=\"128\" g=\"128\" b=\"192\"";
        private static readonly string AudioColor = "r=\"128\" g=\"192\" b=\"128\"";

        private static readonly string ImageColor = "r=\"192\" g=\"160\" b=\"128\"";
        private static readonly string ImageColor2 = "r=\"192\" g=\"128\" b=\"128\"";

        public static string Build()
        {
            sb.Clear();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><project><version>190219</version>");

            AddMedia();

            Addsequence();

            sb.Append("</project>");

            return sb.ToString();
        }

        public static void AddMedia()
        {
            sb.Append($"<media><footage id=\"1\" folder=\"0\" name=\"QuranTemplate\" url=\"{VideoPath}\"></footage>");
            sb.Append($"<footage id=\"2\" folder=\"0\" name=\"QuranAudio\" url=\"{AudioPath}\"></footage>");

            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                sb.Append($"<footage id=\"{3 + i}\" folder=\"0\" name=\"{Path.GetFileName(ImagesPaths[i])}\" url=\"{ImagesPaths[i]}\"></footage>");
            }

            sb.Append("</media>");
        }

        public static void Addsequence()
        {
            sb.Append($"<sequences><sequence id=\"1\" folder=\"0\" name=\"{ProjectName}\" width=\"{VideoWidth}\" height=\"{VideoHeight}\" framerate=\"{FrameRate}\" afreq=\"{AudioSampleRate}\" alayout=\"3\" open=\"1\" workarea=\"0\" workareaIn=\"0\" workareaOut=\"0\">");

            sb.Append($"<clip id=\"1\" enabled=\"1\" name=\"QuranTemplate\" clipin=\"{(int)(VideoOffsetInSecond * FrameRate)}\" in=\"0\" out=\"{(int)(LengthInSecond * FrameRate)}\" {VideoColor} track=\"-2\" media=\"1\" stream=\"0\"></clip>");
            sb.Append($"<clip id=\"2\" enabled=\"1\" name=\"QuranAudio\" clipin=\"{(int)(AudioOffsetInSecond * FrameRate)}\" in=\"0\" out=\"{(int)(LengthInSecond * FrameRate)}\" {AudioColor} track=\"0\" media=\"2\" stream=\"0\"></clip>");

            float sumIn = 0, sumOut;
            for (int i = 0; i < ImagesPaths.Count; i++)
            {
                sumOut = sumIn + AudioTimestamps[i];
                sb.Append($"<clip id=\"{3 + i}\" enabled=\"1\" name=\"{Path.GetFileName(ImagesPaths[i])}\" clipin=\"0\" in=\"{(int)(sumIn * FrameRate)}\" out=\"{(int)(sumOut * FrameRate)}\" {(i % 2 == 0 ? ImageColor : ImageColor2)} track=\"-4\" media=\"{3 + i}\" stream=\"0\"></clip>");
                sumIn = sumOut;
            }

            sb.Append("</sequence></sequences>");
        }

    }
}