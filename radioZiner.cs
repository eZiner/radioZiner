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

        private static MpvPlayer Player;

        Dictionary<string, Recorder> recorders = new Dictionary<string, Recorder>();
        SortedDictionary<string, M3u.TvgChannel> channels = new SortedDictionary<string, M3u.TvgChannel>();
        Dictionary<string, M3u.TvgChannel> allChannels = new Dictionary<string, M3u.TvgChannel>();

        private void ExecuteCommand(string cmd, string val = "")
        {
            switch (cmd)
            {
                case "togglePlayerPause":
                    if (Player.GetPropertyBool("pause"))
                    {
                        btnPlayPause.Text = "⏸️";
                        Player.SetPropertyBool("pause", false);
                    }
                    else
                    {
                        btnPlayPause.Text = "▶️";
                        Player.SetPropertyBool("pause", true);
                    }
                    break;
                case "togglePlayerMute":
                    if (Player.GetPropertyBool("mute"))
                    {
                        Button_Mute.Text = "🔈";
                        Player.SetPropertyBool("mute", false);
                    }
                    else
                    {
                        Button_Mute.Text = "🔊";
                        Player.SetPropertyBool("mute", true);
                    }
                    break;
                case "fullScreen":
                    if (this.FormBorderStyle != FormBorderStyle.None)
                    {
                        this.WindowState = FormWindowState.Normal;
                        this.FormBorderStyle = FormBorderStyle.None;
                        this.WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        this.FormBorderStyle = FormBorderStyle.Sizable;
                        this.WindowState = FormWindowState.Normal;
                    }
                    break;
                case "menuBar":
                    MenuBar.Visible = !MenuBar.Visible;
                    break;
                case "controlBar":
                    flowPanel.Visible = !flowPanel.Visible;
                    panel3.Visible = !panel3.Visible;
                    break;
                case "paste":
                    if (Clipboard.ContainsText())
                    {
                        string s = Clipboard.GetText();
                        if (s.Substring(0, 4).ToLower() == "http")
                        {
                            ExecuteCommand("pasteChannel",s);
                        }
                        else
                        {
                            Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
                            Combo_ShortName.Text = s;
                            Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;
                        }
                    }
                    break;
                case "pasteChannel":
                    TextBox_Url.Text = val;
                    Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDown;
                    Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
                    Combo_ShortName.Text = "";
                    Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;
                    LabelEnterChannelName.Visible = true;

                    curChannelName = "";
                    PictureBox_Player.Show();
                    ListBox_Titles.Hide();
                    Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                    Button_Rec.Text = "Rec";
                    ClearChannelSelect();
                    break;
                case "quit":
                    Application.Exit();
                    break;
            }
        }

        private ToolStripMenuItem AddMenuItem(ToolStripMenuItem mItem, String itemText, String itemID)
        {
            var sItem = new ToolStripMenuItem(itemText);
            sItem.Tag = itemID;
            sItem.Click += new System.EventHandler(MenuClick);
            sItem.ForeColor = Color.White;
            sItem.BackColor = Color.DimGray;
            mItem.DropDownItems.Add(sItem);
            return (sItem);
        }

        private void MenuClick(object sender, EventArgs e)
        {
            ExecuteCommand((String)((ToolStripMenuItem)sender).Tag);
        }

        MenuStrip MenuBar = new MenuStrip();

        public radioZiner()
        {
            InitializeComponent();

            MenuBar.Renderer = new ToolStripProfessionalRenderer(new CustomMenuColors());
            MenuBar.Dock = DockStyle.Top;
            MenuBar.ForeColor = Color.White;
            MenuBar.Font = new Font("Arial", 14.0f);

            var mItem = new ToolStripMenuItem("File");
            AddMenuItem(mItem, "New", "newChannel").ShortcutKeys = Keys.Control | Keys.N;
            AddMenuItem(mItem, "Quit", "quit").ShortcutKeys = Keys.Control | Keys.Q;
            MenuBar.Items.Add(mItem);

            mItem = new ToolStripMenuItem("Edit");
            AddMenuItem(mItem, "Cut", "cutChannel").ShortcutKeys = Keys.Control | Keys.X;
            AddMenuItem(mItem, "Copy", "copyChannel").ShortcutKeys = Keys.Control | Keys.C;
            AddMenuItem(mItem, "Paste", "paste").ShortcutKeys = Keys.Control | Keys.V;
            MenuBar.Items.Add(mItem);

            mItem = new ToolStripMenuItem("View");
            AddMenuItem(mItem, "Controlpanels", "controlBar").ShortcutKeys = Keys.F9;
            AddMenuItem(mItem, "Menubar", "menuBar").ShortcutKeys = Keys.F10;
            AddMenuItem(mItem, "Fullscreen", "fullScreen").ShortcutKeys = Keys.F11;
            MenuBar.Items.Add(mItem);

            mItem = new ToolStripMenuItem("Player");
            AddMenuItem(mItem, "Pause", "togglePlayerPause").ShortcutKeys = Keys.Control | Keys.Space;
            AddMenuItem(mItem, "Mute", "togglePlayerMute").ShortcutKeys = Keys.Control | Keys.M;
            MenuBar.Items.Add(mItem);

            Controls.Add(MenuBar);

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
            Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDownList;
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

            ReadChannels();

            //Player.SetPropertyBool("mute", true);
            btnPlayPause.Text = "⏸️";
            Button_Mute.Text = "🔈";
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.Destroy();
        }

        private void ReadChannels ()
        {
            allChannels.Clear();
            foreach (var f in Directory.GetFiles(channelFolder,"*.m3u"))
            {
                foreach (var c in M3u.GetTvgChannels(Path.Combine(channelFolder, f)))
                {
                    var tvgChannel = c.Value;
                    tvgChannel.file = f;
                    if (!allChannels.Keys.Contains(c.Key))
                    {
                        allChannels.Add(c.Key, tvgChannel);
                    }
                }
            }
        }

        private void ClearChannelSelect()
        {
            foreach (Button c in flowPanel.Controls)
            {
                c.BackColor = Color.Black;
            }
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

        private void Button_PlayPause_Click(object sender, EventArgs e)
        {
            ExecuteCommand("togglePlayerPause");
        }

        private void Button_Mute_Click(object sender, EventArgs e)
        {
            ExecuteCommand("togglePlayerMute");
        }

        private void Button_Rec_Click(object sender, EventArgs e)
        {
            if (curChannelName == "")
            {
                string url = TextBox_Url.Text;
                string shortName = Combo_ShortName.Text;

                // ToDo: Verify shortName is valid filename

                if (!recorders.Keys.Contains(Combo_ShortName.Text) && url != "")
                {
                    if (shortName!="")
                    {
                        Recorder r = new Recorder();
                        r.TitleAdded += UpdateTitleList;
                        r.url = url;
                        r.streamingFolder = recordingFolder;
                        r.shortName = shortName;
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

                        if (!allChannels.Keys.Contains(shortName) && Combo_ChannelSet.Text != "")
                        {
                            M3u.TvgChannel c = new M3u.TvgChannel();
                            c.id = shortName;
                            c.url = url;
                            c.title = shortName;
                            channels.Add(shortName,c);
                            //var sortedChannels = new SortedDictionary<string, M3u.TvgChannel>(channels);
                            M3u.SaveChannelsToFile(channels, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                            //M3u.AppendChannelToFile(c, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                            ReadChannels();
                            ReadSelectedChannelSet();
                        }
                    }
                }
            }
            else
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
                Combo_ShortName.Text = "";
                r.Stop();
                Player.CommandV("stop");
                Button_Rec.Text = "Rec";
            }
            Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void Button_ChangeChannel_Click(object sender, EventArgs e)
        {
            if (curChannelName != "")
            {
                recorders[curChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
                ClearChannelSelect();
            }

            Button btn = (Button)sender;

            curChannelName = btn.Text;
            btn.BackColor = Color.Blue;

            if (!Combo_ShortName.Items.Contains(curChannelName))
            {
                if (allChannels.Keys.Contains(curChannelName))
                {
                    string sFile = Path.GetFileNameWithoutExtension(allChannels[curChannelName].file);
                    Combo_ChannelSet.Text = sFile;
                }
            }

            Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
            Combo_ShortName.Text = curChannelName;
            Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;

            TextBox_Url.Text = recorders[curChannelName].url;
            UpdateTitleList(curChannelName);
            Button_Rec.Text = "Stop";

            Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDownList;
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

        private void ReadSelectedChannelSet()
        {
            string s = Combo_ChannelSet.SelectedItem.ToString() + ".m3u";
            channels = M3u.GetTvgChannels(Path.Combine(channelFolder, s));

            Combo_ShortName.Items.Clear();
            Combo_ShortName.Items.Add("");
            foreach (var c in channels)
            {
                Combo_ShortName.Items.Add(c.Key);
            }
        }

        private void Combo_ChannelSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReadSelectedChannelSet();
        }

        private void Combo_ShortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (curChannelName != "")
            {
                recorders[curChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
                ClearChannelSelect();
            }

            string s = Combo_ShortName.Text;
            TextBox_Url.Text = channels.Keys.Contains(s) ? channels[s].url : "";
            Player.CommandV("stop");

            curChannelName = "";
            PictureBox_Player.Show();
            ListBox_Titles.Hide();
            Player.CommandV("loadfile", TextBox_Url.Text, "replace");
            Button_Rec.Text = "Rec";
            //ClearChannelSelect();
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
                if (s.Substring(0, 4).ToLower() == "http")
                {
                    ExecuteCommand("pasteChannel", s);
                }
                
                Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDown;
                TextBox_Url.Text = s;
                Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
                Combo_ShortName.Text = ""; // "NEW-CHANNEL-" + channelID++.ToString();
                Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;

                curChannelName = "";
                PictureBox_Player.Show();
                ListBox_Titles.Hide();
                Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                Button_Rec.Text = "Rec";
                ClearChannelSelect();
                
            }
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            int seconds = trackBar1.Value;
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (curChannelName=="")
            {
                double curPos = Player.GetPropertyDouble("time-pos");
                if (curPos >= 0)
                {
                    TimeSpan tPos = TimeSpan.FromSeconds(curPos);
                    lblPlayerPos.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                }
                else
                {
                    lblPlayerPos.Text = "--:--:--";
                }
                lblRecordLength.Text = "--:--:--";

                double recLen = Player.GetPropertyDouble("time-remaining") + curPos;
                if (recLen >= 0)
                {
                    TimeSpan tPos = TimeSpan.FromSeconds(recLen);
                    lblRecordLength.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                }
                else
                {
                    lblRecordLength.Text = "--:--:--";
                }
                trackBar1.Minimum = 0;
                trackBar1.Maximum = (int)recLen > 0 ? (int)recLen : 7200;
                trackBar1.Value = (int)curPos > 0 && (int)curPos <= recLen ? (int)curPos : 0;
            }
            else if (recorders.ContainsKey(curChannelName) && recorders[curChannelName]?.recordingToFile != "")
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
                    Player.CommandV("loadfile", recorders[curChannelName].recordingToFile, "replace");
                }

                double curPos = Player.GetPropertyDouble("time-pos");
                double lastPos = recorders[curChannelName].lastPlayPos;
                double recLen = recorders[curChannelName].GetRecordLength();

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

                if (curPos >= 0)
                {
                    TimeSpan tPos = TimeSpan.FromSeconds(curPos);
                    lblPlayerPos.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                }
                else
                {
                    lblPlayerPos.Text = "--:--:--";
                }

                if (recLen >= 0)
                {
                    TimeSpan tPos = TimeSpan.FromSeconds(recLen);
                    lblRecordLength.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                }
                else
                {
                    lblRecordLength.Text = "--:--:--";
                }

                trackBar1.Minimum = 0;
                trackBar1.Maximum = (int)recLen > 0 ? (int)recLen : 7200;
                trackBar1.Value = (int)curPos > 0 && (int)curPos <= recLen ? (int)curPos : 0;

                for (int i = ListBox_Titles.Items.Count - 1; i >= 0; i--)
                {
                    var a = ListBox_Titles.Items[i].ToString().Substring(0, 8).Split(':');
                    double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                    if (seconds <= curPos)
                    {
                        ListBox_Titles.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void LabelEnterChannelName_Click(object sender, EventArgs e)
        {
            LabelEnterChannelName.Visible = false;
        }
    }
}
