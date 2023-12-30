using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace radioZiner
{
    public partial class radioZiner : Form
    {
        private string mainDir = "";
        private string recordingFolder = "";
        private string channelFolder = "";

        private string curChannelName = "";
        private bool channelSwitched = false;

        private static MpvPlayer Player;

        Dictionary<string, Recorder> recorders = new Dictionary<string, Recorder>();
        Dictionary<string, M3u.TvgChannel> channels = new Dictionary<string, M3u.TvgChannel>();

        public radioZiner()
        {
            InitializeComponent();

            mainDir = Properties.Settings.Default.mainDir.Trim();

            foreach (var sArg in Environment.GetCommandLineArgs())
            {
                var a = sArg.Split('=');
                if (a[0].Trim().ToLower() == "maindir")
                {
                    mainDir = a[1].Trim().Trim('"');
                }
            }

            Player = new MpvPlayer();

            ListBox_Titles.Dock = DockStyle.Fill;
            ListBox_Titles.Visible = false;

            PictureBox_Player.Dock = DockStyle.Fill;
            PictureBox_Player.Visible = true;

            Combo_ChannelSet.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void RadioZiner_Load(object sender, EventArgs e)
        {
            Timer timer1 = new Timer();
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(Timer1_Tick);

            Player.Init(PictureBox_Player.Handle);

            if (mainDir == "")
            {
                mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "radioZiner");
                if (!Directory.Exists(mainDir))
                {
                    Directory.CreateDirectory(mainDir);
                }
            }

            recordingFolder = Path.Combine(mainDir, "recordings");
            if (!Directory.Exists(recordingFolder))
            {
                Directory.CreateDirectory(recordingFolder);
            }

            channelFolder = Path.Combine(mainDir, "channels");
            if (!Directory.Exists(channelFolder))
            {
                Directory.CreateDirectory(channelFolder);
            }

            foreach (var sFile in Directory.GetFiles(channelFolder,"*.m3u"))
            {
                Combo_ChannelSet.Items.Add(Path.GetFileName(sFile).Split('.')[0]);
            }

            // Player.SetPropertyBool("mute", true);
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.Destroy();
        }

        private void UpdateTitleList(string recShortName)
        {
            if (recShortName == curChannelName)
            {
                ListBox_Titles.Items.Clear();
                foreach (var t in recorders[recShortName].icyPosTitles)
                {
                    ListBox_Titles.Items.Add(t);
                }
            }
        }

        private void Button_Rec_Click(object sender, EventArgs e)
        {
            if (Combo_ShortName.Text != curChannelName)
            {
                string url = TextBox_Url.Text;
                string shortName = Combo_ShortName.Text;

                if ((shortName == "" || !recorders.Keys.Contains(Combo_ShortName.Text)) && url != "")
                {
                    Recorder r = new Recorder();
                    r.TitleAdded += UpdateTitleList;
                    r.url = url;
                    r.streamingFolder = recordingFolder;
                    r.shortName = shortName != "" ? shortName : (recorders.Count() + 1).ToString();
                    r.Record();

                    recorders.Add(r.shortName, r);

                    Button btn = new Button();
                    btn.Text = r.shortName;
                    btn.Click += Button_ChangeChannel_Click;
                    btn.Dock = DockStyle.Left;
                    btn.BackColor = Color.Black;
                    btn.ForeColor = Color.White;
                    btn.AutoSize = true;
                    btn.FlatStyle = FlatStyle.Standard;

                    flowPanel.Controls.Add(btn);
                }
            }
            else if(curChannelName != "")
            {
                foreach (var c in flowPanel.Controls)
                {
                    if (c is Button button && button.Text == curChannelName)
                    {
                        flowPanel.Controls.Remove(button);
                        break;
                    }
                }
                ListBox_Titles.Items.Clear();
                var r = recorders[curChannelName];
                recorders.Remove(curChannelName);
                curChannelName = "";
                r.Stop();
                Player.CommandV("stop");
                Button_Rec.Text = "Rec";
            }
        }

        private void Button_ChangeChannel_Click(object sender, EventArgs e)
        {
            channelSwitched = true; // HACK - Avoid Combo_ShortName_SelectedIndexChanged
            if (curChannelName!="")
            {
                recorders[curChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
            }
            curChannelName = ((Button)sender).Text;
            Combo_ShortName.Text = curChannelName;
            TextBox_Url.Text = recorders[curChannelName].url;
            UpdateTitleList(curChannelName);
            Button_Rec.Text = "Stop";
        }

        private void Button_PlayPause_Click(object sender, EventArgs e)
        {
            Player.SetPropertyBool("pause", !Player.GetPropertyBool("pause"));
        }

        private void ListBox_Titles_Click(object sender, EventArgs e)
        {
            if (ListBox_Titles.SelectedItem != null)
            {
                var a = ListBox_Titles.SelectedItem.ToString().Substring(0, 8).Split(':');
                double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
            }
        }

        private void Combo_ShortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!channelSwitched) // HACK Ignore event if raised from ChangeChannel_Click
            {
                string s = Combo_ShortName.Text;
                TextBox_Url.Text = channels.Keys.Contains(s) ? channels[s].url : "";
                Combo_ShortName.Text = s;
                Player.CommandV("stop");
                curChannelName = "";
                PictureBox_Player.Show();
                ListBox_Titles.Hide();
                Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                Button_Rec.Text = "Rec";
            }
            channelSwitched = false;
        }

        private void RadioZiner_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void RadioZiner_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(string)))
            {
                string s = (string)e.Data.GetData(typeof(string));
                TextBox_Url.Text = s;
                Combo_ShortName.Text = "";
            }
        }

        private void Combo_ChannelSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = Combo_ChannelSet.SelectedItem.ToString() + ".m3u";
            channels = M3u.GetTvgChannels(Path.Combine(channelFolder,s));

            Combo_ShortName.Items.Clear();
            foreach (var c in channels)
            {
                Combo_ShortName.Items.Add(c.Key);
            }
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int seconds = trackBar1.Value;
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (recorders.ContainsKey(curChannelName) && recorders[curChannelName]?.recordingToFile != "")
            {
                if (recorders[curChannelName].hasIcyTitles)
                {
                    if (!ListBox_Titles.Visible) ListBox_Titles.Show();
                    if (PictureBox_Player.Visible) PictureBox_Player.Hide();
                }
                else
                {
                    if (!PictureBox_Player.Visible) PictureBox_Player.Show();
                    if (ListBox_Titles.Visible) ListBox_Titles.Hide();
                }

                if (Player.GetPropertyString("path") != recorders[curChannelName]?.recordingToFile)
                {
                    Console.WriteLine($"Loadfile: {recorders[curChannelName].recordingToFile}");
                    Player.CommandV("loadfile", recorders[curChannelName].recordingToFile, "replace");
                }

                double pos = Player.GetPropertyDouble("time-pos");

                TimeSpan tPos = TimeSpan.FromSeconds(pos);
                lblPlayerPos.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);

                tPos = TimeSpan.FromSeconds(recorders[curChannelName].GetRecordLength());
                lblRecordLength.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);

                for (int i = ListBox_Titles.Items.Count - 1; i >= 0; i--)
                {
                    var a = ListBox_Titles.Items[i].ToString().Substring(0, 8).Split(':');
                    double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                    if (seconds <= pos)
                    {
                        ListBox_Titles.SelectedIndex = i;
                        break;
                    }
                }

                try
                {
                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = (int)recorders[curChannelName].GetRecordLength();
                    double curPos = Player.GetPropertyDouble("time-pos");
                    double lastPos = recorders[curChannelName].lastPlayPos;
                    if (lastPos > 0)
                    {
                        if (!recorders[curChannelName].hasIcyTitles)
                        {
                            Player.SetPropertyBool("pause", true);
                        }
                        Player.CommandV("seek", lastPos.ToString(CultureInfo.InvariantCulture), "absolute");
                        curPos = Player.GetPropertyDouble("time-pos");
                        if (curPos >= lastPos)
                        {
                            recorders[curChannelName].lastPlayPos = 0;
                            if (!recorders[curChannelName].hasIcyTitles)
                            {
                                Player.SetPropertyBool("pause", false);
                            }
                        }
                    }
                    trackBar1.Value = (int)curPos >= 0 ? (int)curPos : 0;
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.Message);
                }
            }
        }
    }
}
