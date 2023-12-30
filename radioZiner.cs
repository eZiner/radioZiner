﻿using System;
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

            listBox1.Dock = DockStyle.Fill;
            listBox1.Visible = false;

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Visible = true;

            cbChannelSet.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void RadioZiner_Load(object sender, EventArgs e)
        {
            Timer timer1 = new Timer();
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(Timer1_Tick);

            Player.Init(pictureBox1.Handle);

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
                cbChannelSet.Items.Add(Path.GetFileName(sFile).Split('.')[0]);
            }
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.Destroy();
        }

        private void UpdateListBox(string recShortName)
        {
            if (recShortName == curChannelName)
            {
                listBox1.Items.Clear();
                foreach (var t in recorders[recShortName].icyPosTitles)
                {
                    listBox1.Items.Add(t);
                }
            }
        }

        private void AddRecorder(string url, string streamingFolder, string shortName = "")
        {
            if ((shortName == "" || !recorders.Keys.Contains(cbShortName.Text)) && url != "")
            {
                Recorder r = new Recorder();
                r.TitleAdded += UpdateListBox;
                r.url = url;
                r.streamingFolder = streamingFolder;
                r.shortName = shortName != "" ? shortName : (recorders.Count() + 1).ToString();
                r.Record();

                recorders.Add(r.shortName, r);

                Button btn = new Button();
                btn.Text = r.shortName;
                btn.Click += BtnChangeChannel_Click;
                btn.Dock = DockStyle.Left;
                btn.BackColor = Color.Black;
                btn.ForeColor = Color.White;
                btn.AutoSize = true;
                btn.FlatStyle = FlatStyle.Standard;

                flowPanel.Controls.Add(btn);
            }
        }

        private void BtnAddChannel_Click(object sender, EventArgs e)
        {
            if (cbShortName.Text != curChannelName)
            {
                AddRecorder(tbURL.Text, recordingFolder, cbShortName.Text);
            }
            else
            {
                if (curChannelName != "")
                {
                    foreach (var c in flowPanel.Controls)
                    {
                        if (c is Button button && button.Text == curChannelName)
                        {
                            flowPanel.Controls.Remove(button);
                            break;
                        }
                    }
                    listBox1.Items.Clear();
                    var r = recorders[curChannelName];
                    recorders.Remove(curChannelName);
                    curChannelName = "";
                    r.Stop();
                    Player.CommandV("stop");
                    btnAddChannel.Text = "Rec";
                }
            }
        }

        private void BtnChangeChannel_Click(object sender, EventArgs e)
        {
            channelSwitched = true;
            if (curChannelName!="")
            {
                recorders[curChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
            }
            curChannelName = ((Button)sender).Text;
            cbShortName.Text = curChannelName;
            tbURL.Text = recorders[curChannelName].url;
            UpdateListBox(curChannelName);
            btnAddChannel.Text = "Stop";
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Player.SetPropertyBool("pause", !Player.GetPropertyBool("pause"));
        }

        private void ListBox1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string title = listBox1.SelectedItem.ToString().Substring(11);
                var a = listBox1.SelectedItem.ToString().Substring(0, 8).Split(':');
                double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (recorders.ContainsKey(curChannelName) && recorders[curChannelName]?.recordingToFile != "")
            {
                if (recorders[curChannelName].hasIcyTitles)
                {
                    if (!listBox1.Visible) listBox1.Show();
                    if (pictureBox1.Visible) pictureBox1.Hide();
                }
                else
                {
                    if (!pictureBox1.Visible) pictureBox1.Show();
                    if (listBox1.Visible) listBox1.Hide();
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

                for (int i = listBox1.Items.Count - 1; i >= 0; i--)
                {
                    var a = listBox1.Items[i].ToString().Substring(0, 8).Split(':');
                    double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                    if (seconds <= pos)
                    {
                        listBox1.SelectedIndex = i;
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

        private void CbShortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!channelSwitched)
            {
                var a = cbShortName.Text.Split('|');
                string s = a.Count() > 1 ? cbShortName.Text.Split('|')[1].Trim() : cbShortName.Text;
                tbURL.Text = channels.Keys.Contains(s) ? channels[s].url : "";
                cbShortName.Text = s;
                Player.CommandV("stop");
                curChannelName = "";
                pictureBox1.Show();
                listBox1.Hide();
                Player.CommandV("loadfile", tbURL.Text, "replace");
                btnAddChannel.Text = "Rec";
            }
            channelSwitched = false;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int seconds = trackBar1.Value;
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
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
                tbURL.Text = s;
                cbShortName.Text = "";
            }
        }

        private void CbChannelSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = cbChannelSet.SelectedItem.ToString() + ".m3u";
            channels = M3u.GetTvgChannels(Path.Combine(channelFolder,s));

            cbShortName.Items.Clear();
            foreach (var c in channels)
            {
                cbShortName.Items.Add(c.Key);
            }
        }
    }
}