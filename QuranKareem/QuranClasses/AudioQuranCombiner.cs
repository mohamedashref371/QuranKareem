using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuranKareem
{
    internal class AudioQuranCombiner
    {
        public static string[] MushafCombine(string folderPath)
        {
            try { folderPath = Path.GetFullPath(folderPath); } catch { return null; }
            if (folderPath[folderPath.Length - 1] != '\\') folderPath += "\\";
            if (!Directory.Exists(folderPath) || File.Exists(folderPath + "000.db") || !File.Exists("audios\\database for audios.db")) return null;

            string newFolder = $"{folderPath}{DateTime.Now.Ticks}\\";
            Directory.CreateDirectory(newFolder);
            File.Copy("audios\\database for audios.db", newFolder + "000.db");

            Regex regex = new Regex(@"^(s(\d{1,3})a(\d{1,3})|s?(\d{3})a?(\d{3}))\.(mp3|wav)$", RegexOptions.IgnoreCase);
            List<AudioFile> audioFiles = Directory.GetFiles(folderPath)
                .Where(file => regex.IsMatch(Path.GetFileName(file)))
                .Select(file => GetAudioFile(file, regex))
                .OrderBy(file => file.Surah).ThenBy(file => file.Ayah)
                .ToList();
            if (audioFiles.Count == 0) return null;
            bool isMp3 = audioFiles[0].FilePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase);

            List<string> list = new List<string>();
            int index = 0, timestamp_to, bytesRead;
            string outputFilePath;
            Mp3Frame frame;
            byte[] buffer;
            
            using (var conn = new SQLiteConnection($"Data Source={newFolder}000.db;Version=3;"))
            {
                conn.Open();
                using (var comm = new SQLiteCommand(conn))
                {
                    if (!CheckAudioDatabase(comm)) return new string[] { "audios\\database for audios.db" };
                    for (int surah = 1; surah <= 114; surah++)
                    {
                        if (index >= audioFiles.Count) break;
                        if (audioFiles[index].Surah != surah) continue;
                        timestamp_to = 0;
                        outputFilePath = newFolder + surah.ToString().PadLeft(3, '0') + (isMp3 ? ".mp3" : ".wav");
                        using (var outputFile = new FileStream(outputFilePath, FileMode.CreateNew))
                        {
                            while (index < audioFiles.Count && audioFiles[index].Surah == surah)
                            {
                                try
                                {
                                    if (isMp3)
                                    {
                                        using (var reader = new Mp3FileReader(audioFiles[index].FilePath))
                                        {
                                            if (outputFile.Position == 0 && reader.Id3v2Tag != null)
                                                outputFile.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);

                                            while ((frame = reader.ReadNextFrame()) != null)
                                                outputFile.Write(frame.RawData, 0, frame.RawData.Length);

                                            timestamp_to += (int)reader.TotalTime.TotalMilliseconds;
                                        }
                                    }
                                    else // isWav
                                    {
                                        using (var reader = new WaveFileReader(audioFiles[index].FilePath))
                                        {
                                            buffer = new byte[reader.WaveFormat.AverageBytesPerSecond];
                                            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                                                outputFile.Write(buffer, 0, bytesRead);

                                            timestamp_to += (int)reader.TotalTime.TotalMilliseconds;
                                        }
                                    }
                                    comm.CommandText = $"UPDATE ayat SET timestamp_to={timestamp_to} WHERE surah={audioFiles[index].Surah} AND ayah={audioFiles[index].Ayah};";
                                    comm.CommandText += $"UPDATE ayat SET timestamp_from={timestamp_to} WHERE surah={audioFiles[index].Surah} AND ayah={audioFiles[index].Ayah + 1}";
                                    comm.ExecuteNonQuery();
                                }
                                catch
                                {
                                    list.Add(Path.GetFileName(audioFiles[index].FilePath));
                                    if (list.Count >= 50) return list.ToArray();
                                }
                                index++;
                            }
                        }
                    }
                    return list.ToArray();
                }
            }
        }

        private static bool CheckAudioDatabase(SQLiteCommand comm)
        {
            SQLiteDataReader sqlReader;
            comm.CommandText = $"SELECT type,version FROM description";
            sqlReader = comm.ExecuteReader();
            if (!sqlReader.Read()) return false;
            int type = sqlReader.GetInt32(0),
                version = sqlReader.GetInt32(1);
            sqlReader.Close();
            comm.Cancel();
            return type == 3 && version == 5;

        }

        private static AudioFile GetAudioFile(string file, Regex regex)
        {
            GroupCollection group = regex.Match(Path.GetFileName(file)).Groups;
            return new AudioFile
            {
                Surah = int.Parse(group[2].Success ? group[2].Value : group[4].Value),
                Ayah = int.Parse(group[3].Success ? group[3].Value : group[5].Value),
                FilePath = file
            };
        }

        private struct AudioFile
        {
            public int Surah;
            public int Ayah;
            public string FilePath;
        }
    }
}
