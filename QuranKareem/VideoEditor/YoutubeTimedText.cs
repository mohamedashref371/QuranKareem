using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace QuranKareem
{
    public class YoutubeTimedText
    {
        class Event
        {
            public int tStartMs { get; set; }
            public int dDurationMs { get; set; }
            public List<Seg> segs { get; set; }
        }

        class Time
        {
            public List<Event> events { get; set; }
        }

        class Seg
        {
            public string utf8 { get; set; }
            public int? tOffsetMs { get; set; }
        }

        public static void JsonDeserializing(string path, string savePath)
        {
            Constants.StringBuilder.Length = 0;
            string jsonText = File.ReadAllText(path);
            Time json = JsonConvert.DeserializeObject<Time>(jsonText);
            int start, end;
            Event currentEvent;
            for (int i = 0; i < json.events?.Count; i++)
            {
                currentEvent = json.events[i];
                end = currentEvent.tStartMs;
                for (int j = 0; j < currentEvent.segs?.Count; j++)
                {
                    start = end;
                    if (j == currentEvent.segs.Count - 1)
                        if (i == json.events.Count - 1)
                            end = json.events[0].tStartMs + json.events[0].dDurationMs;
                        else
                            end = json.events[i + 1].tStartMs;
                    else
                        end = currentEvent.tStartMs + (currentEvent.segs[j + 1].tOffsetMs ?? 0);
                    if (currentEvent.segs[j].utf8.Trim() != "" && !currentEvent.segs[j].utf8.Contains('['))
                        Constants.StringBuilder.Append(start).Append("|").Append(end).Append("|").Append(currentEvent.segs[j].utf8.Trim()).AppendLine();
                }
            }
            File.WriteAllText(savePath, Constants.StringBuilder.ToString());
        }
    }
}
