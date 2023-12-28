using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace radioZiner
{
    public class Recorder
    {
        public string streamingFolder = "";
        public DateTime recordStartTime = DateTime.Now;
        public string name = "";
        public string shortName = "";
        public string url = "";
        public string recordingToFile = "";
        public double lastPlayPos = 0;

        public PictureBox pictureBox = new PictureBox();
        private MpvPlayer Player { get; } = new MpvPlayer();
        private Timer ChkStreamTimer = new Timer();
        public delegate void TitleAddedEvent(string s);
        public event TitleAddedEvent TitleAdded;

        private bool stopped = false;

        public bool hasIcyTitles = false;

        public Recorder()
        {
            Player.Init(pictureBox.Handle);
            ChkStreamTimer.Interval = 1000;
            ChkStreamTimer.Tick += new System.EventHandler(ChkStreamTimer_Tick);
        }

        ~Recorder()
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
                recordingToFile = streamingFolder + shortName + ".mkv";
                Player.SetPropertyBool("mute", true);
                Player.SetPropertyString("stream-record", recordingToFile);
                Player.CommandV("loadfile", url, "replace");
                recordStartTime = DateTime.Now;
                ChkStreamTimer.Start();
            }
        }

        public double GetRecordLength ()
        {
            return (Player.GetPropertyDouble("time-pos"));
        }

        public List<string> icyTitles = new List<string>();
        public List<string> icyPosTitles = new List<string>();


        public void Clear()
        {
            icyTitles.Clear();
            icyPosTitles.Clear();
        }

        private bool AddTitle(double pos, string title = "")
        {
            bool changed = false;

            if (title != "" && !icyTitles.Contains(title))
            {
                icyTitles.Add(title);
                TimeSpan tPos = TimeSpan.FromSeconds(pos);
                string sPos = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                string sPosTitle = sPos + " - " + title;
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
            if (streamingFolder != "" && Player.GetPropertyString("path").StartsWith("http"))
            {
                if (AddTitle(Player.GetPropertyDouble("time-pos", false), Player.GetPropertyString("media-title")))
                {
                    File.WriteAllLines(streamingFolder + shortName + ".txt", icyPosTitles);
                    TitleAdded?.Invoke(shortName);
                }
            }
        }
    }
}
