using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace radioZiner
{
    public class M3u
    {

        public struct TvgChannel
        {
            public string id;
            public string url;
            public string group;
            public string title;
            public string logo;
            public string file;
        }

        public static TvgChannel ParseTvgRecord (string sRecord)
        {
            var tvg = new TvgChannel();
            var lines = sRecord.Replace('~','_').Split('\n');

            var line = lines[0].Trim();

            var a = line.Replace("tvg-id=", "~").Split('~');

            if (a.Length > 1)
            {
                a = a[1].Replace("tvg", "~").Split('~');
                tvg.id = a[0].Replace("\"", "").Trim(' ');
            }

            a = line.Replace("tvg-logo=", "~").Split('~');
            if (a.Length > 1)
            {
                line = a[1];
                a = line.Replace("group-", "~").Split('~');
                tvg.logo = a[0].Replace("\"", "").Trim(' ');
            }

            a = line.Replace("group-title=", "~").Split('~');
            if (a.Length > 1)
            {
                line = a[1];
                a = line.Replace("\",", "~").Split('~');
                tvg.group = a[0].Replace("\"", "").Trim(' ');
                tvg.title = (a.Length > 1) ? a[1] : line;
            }

            if (lines.Length > 1)
            {
                line = lines[1];
                if (line.Substring(0, 1).ToLower() != "#")
                {
                    tvg.url = line;
                }
            }

            return tvg;
        }

        public static string CreateTvgRecord (TvgChannel channel)
        {
            return
            (
                "#EXTINF:-1 tvg-id=\"" + channel.id
                + "\" tvg-logo=\"" + channel.logo
                + "\" group-title=\"" + channel.group
                + "\"," + channel.title
                + Environment.NewLine
                + channel.url
            );
        }

        public static void SaveChannelsToFile(SortedDictionary<string, M3u.TvgChannel> channels, string file)
        {
            List<string> lines = new List<string>();
            lines.Add("#EXTM3U");
            foreach (var channel in channels)
            {
                lines.Add(CreateTvgRecord(channel.Value));
            }

            File.WriteAllLines(file, lines);
        }

        public static SortedDictionary<string, TvgChannel> GetTvgChannels(string url)
        {
            SortedDictionary<string, TvgChannel> channels = new SortedDictionary<string, TvgChannel>();

            Stream stream = null;

            try
            {

                if (url.StartsWith("http"))
                {
                    WebClient client = new WebClient();
                    stream = client.OpenRead(url);
                }
                else
                {
                    stream = File.OpenRead(url);
                }

                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("UTF-8"));

                string line;
                string lines = "";
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim(' ');
                    if (line.Length > 0 && line.Substring(0, 7) != "#EXTM3U")
                    {
                        if (line.Substring(0, 7) == "#EXTINF")
                        {
                            lines = line + Environment.NewLine;
                        }
                        else if (line.Substring(0, 1).ToLower() != "#")
                        {
                            lines += line;
                            TvgChannel tvg = ParseTvgRecord(lines);
                            //Console.WriteLine("tvg.id: " + tvg.id);
                            if (tvg.id!="" && !channels.ContainsKey(tvg.id))
                            {
                                channels.Add(tvg.id, tvg);
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                Console.WriteLine("AddTvgChannels: " + url + " " + statusCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTvgChannels: " + url + " " + ex.Message);
            }
            finally
            {
                stream?.Close();
            }
            return channels;
        }
    }
}
