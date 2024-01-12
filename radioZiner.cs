using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        private int extRecorder = 0;
        private VideoRecorder videoRecorder;

        private void ExecuteCommand(string cmd, string val = "")
        {
            {
                var a = cmd.Split('|');
                if (a.Count() > 1)
                {
                    cmd = a[0];
                    val = a[1];
                }
            }
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
                case "seek":
                    if (val.StartsWith("+") || val.StartsWith("-"))
                    {
                        Player.CommandV("seek", val, "exact");
                    }
                    else
                    {
                        Player.CommandV("seek", val, "absolute", "exact");
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
                    if (TextBox_ShortName.Visible)
                    {
                        TextBox_ShortName.Paste();
                    }
                    else if (TextBox_ChannelSet.Visible)
                    {
                        TextBox_ChannelSet.Paste();
                    }
                    else if (Clipboard.ContainsText())
                    {
                        string s = Clipboard.GetText();
                        if (s.Substring(0, 4).ToLower() == "http" || File.Exists(s) || Directory.Exists(s))
                        {
                            ExecuteCommand("pasteURL", s);
                        }
                        else if (s.Substring(0, 7).ToLower() == "#extinf")
                        {
                            ExecuteCommand("pasteChannel", s);
                        }
                    }
                    else if (Clipboard.ContainsFileDropList())
                    {
                        var a = Clipboard.GetFileDropList();
                        if (a.Count > 0)
                        {
                            ExecuteCommand("pasteURL", a[0]);
                        }
                    }
                    break;
                case "cut":
                    if (TextBox_ShortName.Visible)
                    {
                        TextBox_ShortName.Cut();
                    }
                    else if (TextBox_ChannelSet.Visible)
                    {
                        TextBox_ChannelSet.Cut();
                    }
                    else
                    {
                        ExecuteCommand("cutChannel");
                    }
                    break;
                case "copy":
                    if (TextBox_ShortName.Visible)
                    {
                        TextBox_ShortName.Copy();
                    }
                    else if (TextBox_ChannelSet.Visible)
                    {
                        TextBox_ChannelSet.Copy();
                    }
                    else
                    {
                        ExecuteCommand("copyChannel");
                    }
                    break;
                case "cutChannel":
                    if (channels.ContainsKey(Combo_ShortName.Text))
                    {
                        int selIndex = Combo_ShortName.SelectedIndex;
                        ExecuteCommand("copyChannel");
                        channels.Remove(Combo_ShortName.Text);
                        M3u.SaveChannelsToFile(channels, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                        ReadChannels();
                        ReadSelectedChannelSet();

                        if (selIndex >= Combo_ShortName.Items.Count)
                        {
                            selIndex--;
                        }

                        if (selIndex >= 0)
                        {
                            Combo_ShortName.SelectedIndex = selIndex;
                        }
                    }
                    break;
                case "copyChannel":
                    if (channels.ContainsKey(Combo_ShortName.Text))
                    {
                        Clipboard.SetText( M3u.CreateTvgRecord(channels[Combo_ShortName.Text]));
                    }
                    break;
                case "pasteChannel":
                    if (Combo_ChannelSet.Text != "")
                    {
                        M3u.TvgChannel channel = M3u.ParseTvgRecord(val);

                        for (var (i,s) = (1, channel.id); channels.ContainsKey(channel.id) && i < 10; i++) //allChannels
                        {
                            channel.id = s + " (" + i + ")";
                        }

                        if (!channels.ContainsKey(channel.id)) //allChannels
                        {
                            channels.Add(channel.id, channel);
                            M3u.SaveChannelsToFile(channels, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                            ReadChannels();
                            ReadSelectedChannelSet();
                        }
                    }
                    break;
                case "pasteURL":
                    TextBox_Url.Text = val;
                    TextBox_ShortName.Text = "";
                    LabelEnterChannelName.Show();
                    TextBox_ShortName.Show();
                    LabelEnterChannelName.BringToFront();

                    curChannelName = "";
                    PictureBox_Player.Show();
                    ListBox_Titles.Hide();
                    Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                    Button_Rec.Text = "Rec";
                    ClearChannelSelect();
                    TextBox_ShortName.Focus();
                    break;
                case "new":
                    TextBox_ChannelSet.Text = "";
                    LabelEnterGroupName.Show();
                    TextBox_ChannelSet.Show();
                    LabelEnterGroupName.BringToFront();
                    TextBox_ChannelSet.Focus();
                    break;
                case "delete":
                    if (Combo_ChannelSet.Text!="")
                    {
                        string channelSetFile = Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u");
                        if (File.Exists(channelSetFile))
                        {
                            DialogResult dr = MessageBox.Show("Delete Channel List \"" + Combo_ChannelSet.Text +  "\"?",
                                                  "Delete Channel List", MessageBoxButtons.YesNo);
                            switch (dr)
                            {
                                case DialogResult.Yes:
                                    File.Delete(channelSetFile);
                                    ReadChannelSets();
                                    break;
                                case DialogResult.No:
                                    break;
                            }
                        }
                    }
                    break;
                case "homeDir":
                    Process.Start("explorer.exe" , mainDir);
                    break;
                case "quit":
                    Application.Exit(); // Process.Start("explorer.exe" , @"C:\Users");
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
            AddMenuItem(mItem, "Home", "homeDir").ShortcutKeys = Keys.Control | Keys.H;
            AddMenuItem(mItem, "New", "new").ShortcutKeys = Keys.Control | Keys.N;
            AddMenuItem(mItem, "Delete", "delete").ShortcutKeys = Keys.Control | Keys.D;
            AddMenuItem(mItem, "Quit", "quit").ShortcutKeys = Keys.Control | Keys.Q;
            MenuBar.Items.Add(mItem);

            mItem = new ToolStripMenuItem("Edit");
            AddMenuItem(mItem, "Cut", "cut").ShortcutKeys = Keys.Control | Keys.X;
            AddMenuItem(mItem, "Copy", "copy").ShortcutKeys = Keys.Control | Keys.C;
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
            AddMenuItem(mItem, "- 1 Sec", "seek|-1").ShortcutKeys = Keys.Control | Keys.Left;
            AddMenuItem(mItem, "+ 1 Sec", "seek|+1").ShortcutKeys = Keys.Control | Keys.Right;
            AddMenuItem(mItem, "- 1 Min", "seek|-60").ShortcutKeys = Keys.Shift | Keys.Control | Keys.Left;
            AddMenuItem(mItem, "+ 1 Min", "seek|+60").ShortcutKeys = Keys.Shift | Keys.Control | Keys.Right;
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
            TextBox_ShortName.Hide();

            PictureBox_Player.MouseWheel += new MouseEventHandler(MouseWheelHandler);
            ListBox_Titles.MouseWheel += new MouseEventHandler(MouseWheelHandler);
            PictureBox_Player.MouseClick += new MouseEventHandler(PictureBox_Player_MouseClick);
            ListBox_Titles.MouseDown += new MouseEventHandler(ListBox_Titles_Click);

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

            ReadChannelSets();

            ReadChannels();

            //Player.SetPropertyBool("mute", true);
            btnPlayPause.Text = "⏸️";
            Button_Mute.Text = "🔈";

            if (extRecorder>0)
            {
                videoRecorder = new VideoRecorder(recordingFolder);
            }
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.Destroy();
            if (extRecorder > 0)
            {
                videoRecorder.StopAllRecordings();
            }
        }

        private void ReadChannelSets()
        {
            Combo_ChannelSet.Items.Clear();
            foreach (var sFile in Directory.GetFiles(channelFolder, "*.m3u"))
            {
                Combo_ChannelSet.Items.Add(Path.GetFileName(sFile).Split('.')[0]);
            }
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

        private bool UserEntersData ()
        {
            return (TextBox_ShortName.Visible || TextBox_ChannelSet.Visible);
        }

        private void Button_Rec_Click(object sender, EventArgs e)
        {
            if (UserEntersData())
            {
                return;
            }
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
                        r.extRecorder = extRecorder;
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

                        if (!channels.Keys.Contains(shortName) && Combo_ChannelSet.Text != "") //allChannels
                        {
                            M3u.TvgChannel c = new M3u.TvgChannel();
                            c.id = shortName;
                            c.url = url;
                            c.title = shortName;
                            channels.Add(shortName,c);
                            M3u.SaveChannelsToFile(channels, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                            ReadChannels();
                            ReadSelectedChannelSet();
                        }

                        if (extRecorder > 0)
                        {
                            videoRecorder.StartRecording(channels[shortName]);
                        }
                    }
                }
            }
            else
            {
                if (extRecorder > 0)
                {
                    videoRecorder.StopRecording(curChannelName);
                }

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

            //Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDownList;
            TextBox_ShortName.Hide();
        }

        int selTitleIndex = -1;

        private void ListBox_Titles_Click(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    
                    if (ListBox_Titles.SelectedItem != null && ListBox_Titles.SelectedIndex != selTitleIndex)
                    {
                        var a = ListBox_Titles.SelectedItem.ToString().Substring(0, 8).Split(':');
                        double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                        Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
                        selTitleIndex = ListBox_Titles.SelectedIndex;
                    }
                    
                    break;

                case MouseButtons.Middle:
                    break;

                case MouseButtons.Right:
                    ExecuteCommand("togglePlayerPause");
                    break;
            }
        }

        private void ReadSelectedChannelSet()
        {
            string s = Combo_ChannelSet.SelectedItem.ToString() + ".m3u";
            channels = M3u.GetTvgChannels(Path.Combine(channelFolder, s));

            Combo_ShortName.Items.Clear();
            //Combo_ShortName.Items.Add("");
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
            if (e.Data.GetDataPresent(typeof(string)) || e.Data.GetDataPresent(DataFormats.FileDrop))
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
                    ExecuteCommand("pasteURL", s);

                    LabelEnterChannelName.Show();
                    TextBox_ShortName.Text = "";
                    TextBox_ShortName.Show();
                    LabelEnterChannelName.BringToFront();
                    TextBox_Url.Text = s;
                    Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
                    Combo_ShortName.Text = "";
                    Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;

                    curChannelName = "";
                    PictureBox_Player.Show();
                    ListBox_Titles.Hide();
                    Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                    Button_Rec.Text = "Rec";
                    ClearChannelSelect();
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var a =  (string[])e.Data.GetData(DataFormats.FileDrop);
                if (a.Count() > 0)
                {
                    ExecuteCommand("pasteURL", a[0]);
                }
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
            TextBox_ShortName.Focus();
        }

        private void Combo_ShortName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Combo_ShortName.Text!="")
            {
                oldShortName = Combo_ShortName.Text;
                TextBox_ShortName.Text = oldShortName;
                TextBox_ShortName.Show();
                TextBox_ShortName.Focus();
            }
        }

        public string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        private string oldShortName = "";

        private void TextBox_ShortName_KeyDown(object sender, KeyEventArgs e)
        {
            LabelEnterChannelName.Hide();
            if (e.KeyCode == Keys.Enter)
            {
                string url = TextBox_Url.Text;
                string shortName = ReplaceInvalidChars(TextBox_ShortName.Text);

                if (!channels.Keys.Contains(shortName) && Combo_ChannelSet.Text != "") //allChannels
                {
                    M3u.TvgChannel c = new M3u.TvgChannel();
                    c.id = shortName;
                    c.url = url;
                    c.title = shortName;
                    channels.Add(shortName, c);
                    if (oldShortName!="")
                    {
                        channels.Remove(oldShortName);
                        oldShortName = "";
                    }
                    M3u.SaveChannelsToFile(channels, Path.Combine(channelFolder, Combo_ChannelSet.Text + ".m3u"));
                    ReadChannels();
                    ReadSelectedChannelSet();
                    Combo_ShortName.SelectedItem = shortName;
                    TextBox_ShortName.Hide();
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                TextBox_ShortName.Text = "";
                oldShortName = "";
                TextBox_ShortName.Hide();
                e.SuppressKeyPress = true;
            }
        }

        private string oldChannelSetName = "";

        private void TextBox_ChannelSet_KeyDown(object sender, KeyEventArgs e)
        {
            LabelEnterGroupName.Hide();
            if (e.KeyCode == Keys.Enter && TextBox_ChannelSet.Text != "")
            {
                string channelSetFile = Path.Combine(channelFolder, ReplaceInvalidChars(TextBox_ChannelSet.Text) + ".m3u");
                if (File.Exists(channelSetFile))
                {
                    return;
                }

                if (oldChannelSetName=="")
                {
                    try
                    {
                        File.WriteAllText(channelSetFile, "#EXTM3U" + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error creating file: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        string oldChannelSetFile = Path.Combine(channelFolder, oldChannelSetName + ".m3u");
                        Directory.Move(oldChannelSetFile, channelSetFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error renaming file: " + ex.Message);
                        return;
                    }
                }
                ReadChannelSets();
                Combo_ChannelSet.SelectedItem = TextBox_ChannelSet.Text;
                TextBox_ChannelSet.Text = "";
                TextBox_ChannelSet.Hide();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                TextBox_ChannelSet.Text = "";
                oldChannelSetName = "";
                TextBox_ChannelSet.Hide();
                e.SuppressKeyPress = true;
            }

        }

        private void Combo_ChannelSet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Combo_ChannelSet.Text != "")
            {
                oldChannelSetName = Combo_ChannelSet.Text;
                TextBox_ChannelSet.Text = oldChannelSetName;
                TextBox_ChannelSet.Show();
                TextBox_ChannelSet.Focus();
            }
        }

        private void LabelEnterGroupName_Click(object sender, EventArgs e)
        {
            LabelEnterGroupName.Visible = false;
            TextBox_ChannelSet.Focus();
        }


        private void PictureBox_Player_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    break;

                case MouseButtons.Middle:
                    break;

                case MouseButtons.Right:
                    ExecuteCommand("togglePlayerPause");
                    break;
            }
            PictureBox_Player.Focus();
        }

        public void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (e.Delta > 0)
                {
                    if (Player.GetPropertyBool("pause"))
                    {
                        Player.CommandV("frame-step");
                        return;
                    }

                    Player.CommandV("seek", "3", "relative");
                }
                else if (Player.GetPropertyBool("pause"))
                {
                    Player.CommandV("frame-back-step");
                }
                else
                {
                    Player.CommandV("seek", "-3", "relative");
                }
            }
            else if (e.Delta > 0)
            {
                if (Player.GetPropertyBool("pause"))
                {
                    Player.CommandV("seek", "1", "relative", "exact");
                }
                else
                {
                    Player.CommandV("seek", "5", "relative");
                }
            }
            else if (Player.GetPropertyBool("pause"))
            {
                Player.CommandV("seek", "-1", "relative", "exact");
            }
            else
            {
                Player.CommandV("seek", "-5", "relative");
            }
        }

        private void ListBox_Titles_DoubleClick(object sender, EventArgs e)
        {
            ExecuteCommand("fullScreen");
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                MenuBar.Visible = false;
                flowPanel.Visible = false;
                panel3.Visible = false;
            }
            else
            {
                MenuBar.Visible = true;
                flowPanel.Visible = true;
                panel3.Visible = true;
            }
        }

        private void ListBox_Titles_Click(object sender, EventArgs e)
        {
            /*
            if (ListBox_Titles.SelectedItem != null)
            {
                var a = ListBox_Titles.SelectedItem.ToString().Substring(0, 8).Split(':');
                double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
            }
            */
        }

        private void ListBox_Titles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListBox_Titles_SelectedValueChanged(object sender, EventArgs e)
        {
        }
    }
}
