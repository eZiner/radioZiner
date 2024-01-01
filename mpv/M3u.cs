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
            public string file;
        }

        public static void AppendChannelToFile (TvgChannel channel, string file)
        {
            List<string> lines = new List<string>();

            lines.Add("#EXTINF:-1 tvg-id=\"" + channel.id
                     + "\" tvg-logo=\"\" group-title=\"" + channel.group
                     + "\"," + channel.title);

            lines.Add(channel.url);

            // should make sure last char in file is newline here

            File.AppendAllLines(file, lines);
        }

        public static Dictionary<string, TvgChannel> GetTvgChannels(string url)
        {
            Dictionary<string, TvgChannel> channels = new Dictionary<string, TvgChannel>();

            try
            {
                Stream stream;

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
                string tvg_url = "";
                string tvg_id = "";
                string tvg_group = "";
                string tvg_title = "";
                string line;
                string[] a;

                int idc = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim(' ');
                    if (line.Length > 0 && line.Substring(0, 7) != "#EXTM3U")
                    {
                        if (line.Substring(0, 7) == "#EXTINF")
                        {
                            a = line.Replace("tvg-id=", "~").Split('~');
                            if (a.Length > 1)
                            {
                                line = a[1];
                                a = line.Replace("tvg", "~").Split('~');
                                tvg_id = a[0].Replace("\"", "").Trim(' ');
                            }
                            else
                            {
                                tvg_id = "";
                            }

                            a = line.Replace("group-title=", "~").Split('~');
                            if (a.Length > 1)
                            {
                                line = a[1];
                                a = line.Replace("\",", "~").Split('~');
                                tvg_group = a[0].Replace("\"", "").Trim(' ');
                                tvg_title = (a.Length > 1) ? a[1] : line;
                            }
                            else
                            {
                                tvg_title = "";
                                tvg_group = "";
                            }
                        }
                        else if (line.Substring(0, 4).ToLower() == "http")
                        {
                            tvg_url = line;
                            string s = tvg_id.Trim() == "" ? "-" + idc + "-" : tvg_id;
                            var tvg = new TvgChannel();
                            tvg.url = tvg_url;
                            tvg.id = tvg_id;
                            tvg.title = tvg_title;
                            tvg.group = tvg_group;
                            channels.Add(s, tvg);
                            idc++;

                            tvg_url = "";
                            tvg_id = "";
                            tvg_title = "";
                            tvg_group = "";
                        }
                    }
                }
                stream.Close();
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
            return channels;
        }
    }
}
