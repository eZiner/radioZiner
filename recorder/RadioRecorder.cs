using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace radioZiner
{
    public class RadioRecorder
    {
        public string streamingFolder = "";
        public DateTime recordStartTime = DateTime.Now;
        public string recTimeStamp = "";
        public string name = "";
        public string shortName = "";
        public string url = "";
        public string recordingToFile = "";
        public double lastPlayPos = 0;
        public double recLength = 0;

        public PictureBox pictureBox = new PictureBox();
        public MpvPlayer Player { get; } = new MpvPlayer();
        private Timer ChkStreamTimer = new Timer();
        public delegate void TitleAddedEvent(string s);
        public event TitleAddedEvent TitleAdded;

        private bool stopped = false;

        public bool hasIcyTitles = false;

        public int extRecorder = 0;

        public RadioRecorder()
        {
            Player.Init(pictureBox.Handle);
            ChkStreamTimer.Interval = 1000;
            ChkStreamTimer.Tick += new System.EventHandler(ChkStreamTimer_Tick);
        }

        ~RadioRecorder()
        {
            if (!stopped)
            {
                Player.Destroy();
            }
        }

        public void Play()
        {
            Player.CommandV("loadfile", url, "replace");
        }

        public void Stop()
        {
            ChkStreamTimer.Stop();
            Player.Destroy();
            stopped = true;
        }

        public void Record()
        {
            if (streamingFolder != "")
            {
                recordStartTime = DateTime.Now;
                recTimeStamp = string.Format(
                    "{0:D4}-{1:D2}-{2:D2}-{3:D2}-{4:D2}-{5:D2}",
                    recordStartTime.Year,
                    recordStartTime.Month,
                    recordStartTime.Day,
                    recordStartTime.Hour,
                    recordStartTime.Minute,
                    recordStartTime.Second
                );
                recordingToFile = Path.Combine(streamingFolder, shortName + "_" + recTimeStamp + ".ts"); //.mkv
                Player.SetPropertyBool("mute", true);
                if (extRecorder==0)
                {
                    Player.SetPropertyString("stream-record", recordingToFile);
                }
                Player.CommandV("loadfile", url, "replace");
                ChkStreamTimer.Start();
            }
        }

        public double GetRecordLength ()
        {
            double l = shortName.EndsWith("@") ? recLength : Player.GetPropertyDouble("time-pos");
            return (l);
        }

        public List<string> icyTitles = new List<string>();
        public List<string> icyPosTitles = new List<string>();


        public void Clear()
        {
            icyTitles.Clear();
            icyPosTitles.Clear();
        }

        public void DeleteTitleAtPos(string pos, string title)
        {
            ChkStreamTimer.Enabled = false;

            foreach (var line in icyPosTitles)
            {
                if (line.StartsWith(pos + " " + title.Trim()) || line.StartsWith(pos.Split('.')[0] + " " + title.Trim())) // // " - " + 
                {
                    icyTitles.Remove(title);
                    icyPosTitles.Remove(line);
                    break;
                }
            }

            if (streamingFolder != "" && Player.GetPropertyString("path").StartsWith("http"))
            {
                File.WriteAllLines(Path.Combine(streamingFolder, shortName + "_" + recTimeStamp + ".txt"), icyPosTitles);
                TitleAdded?.Invoke(shortName);
            }

            ChkStreamTimer.Enabled = true;

            if (!(icyTitles.Count() > 1))
            {
                hasIcyTitles = false;
            }
        }

        public void AddTitlePos(string pos, string title = "", string delim = " ") //" - "
        {
            pos = pos.Trim();
            title = title.Trim();
            if (title != "")
            {
                ChkStreamTimer.Enabled = false;
                icyTitles.Add(title);
                string sPosTitle = pos + delim + title;
                icyPosTitles.Add(sPosTitle);
                icyPosTitles.Sort();

                if (streamingFolder != "" && Player.GetPropertyString("path").StartsWith("http"))
                {
                    List<string> titles = new List<string>();
                    for (int i = 0; i < icyPosTitles.Count; i++)
                    {
                        var a = icyPosTitles[i].Split('|');
                        for (int j = 0; j < a.Count(); j++)
                        {
                            titles.Add(a[j].Trim());
                        }
                    }
                    File.WriteAllLines(Path.Combine(streamingFolder, shortName + "_" + recTimeStamp + ".txt"), titles);
                    //File.WriteAllLines(Path.Combine(streamingFolder, shortName + "_" + recTimeStamp + ".txt"), icyPosTitles);
                    TitleAdded?.Invoke(shortName);
                }

                ChkStreamTimer.Enabled = true;
            }

            if (icyTitles.Count() > 1)
            {
                hasIcyTitles = true;
            }
        }

        private bool AddTitle(double pos, string title = "")
        {
            bool changed = false;

            title = title.Trim();
            if (title != "" && !icyTitles.Contains(title))
            {
                icyTitles.Add(title);
                TimeSpan tPos = TimeSpan.FromSeconds(pos);
                string sPos = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                string sPosTitle = sPos + " " + title; // " - "
                icyPosTitles.Add(sPosTitle);

                icyPosTitles.Sort();
                changed = true;
            }

            if (icyTitles.Count()>1)
            {
                hasIcyTitles = true;
            }

            return (changed);
        }

        private void ChkStreamTimer_Tick(object sender, EventArgs e)
        {
            recLength++;
            if (streamingFolder != "" && Player.GetPropertyString("path").StartsWith("http"))
            {
                if (AddTitle(Player.GetPropertyDouble("time-pos", false), Player.GetPropertyString("media-title")))
                {
                    File.WriteAllLines(Path.Combine(streamingFolder, shortName + "_" + recTimeStamp + ".txt"), icyPosTitles);
                    TitleAdded?.Invoke(shortName);
                }
            }
        }
    }
}
