using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Text.RegularExpressions;

using Timer = System.Windows.Forms.Timer;

namespace radioZiner
{
    public partial class radioZiner : Form
    {
        private string mainDir = "";
        private string recordingFolder = "";
        private string exportFolder = "";
        private string channelFolder = "";
        private string streamFolder = "";

        private string curRecChannelName = "";
        private string CurSource = "";

        private static MpvPlayer Player;

        Dictionary<string, RadioRecorder> recorders = new Dictionary<string, RadioRecorder>();
        SortedDictionary<string, M3u.TvgChannel> channels = new SortedDictionary<string, M3u.TvgChannel>();
        Dictionary<string, M3u.TvgChannel> allChannels = new Dictionary<string, M3u.TvgChannel>();
        SortedDictionary<string, M3u.TvgChannel> allStreams = new SortedDictionary<string, M3u.TvgChannel>();
        SortedDictionary<string, string> TvgGroups = new SortedDictionary<string, string>();
        SortedDictionary<string, string> TvgCountries = new SortedDictionary<string, string>();

        //SortedDictionary<string, M3u.TvgChannel> RadioChannels = new SortedDictionary<string, M3u.TvgChannel>();

        private int extRecorder = 0;
        private VideoRecorder videoRecorder;

        private string curChannelName = "";

        private void setChannelName (string s)
        {
            curChannelName = s;
            TextBox_ShortName.Text = s;
        }

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
                case "titleList":
                    Toggle_ListBox_Titles();
                    break;
                case "fileBrowser":
                    Toggle_ListBox_Files();
                    break;
                case "menuBar":
                    MenuBar.Visible = !MenuBar.Visible;
                    break;
                case "controlBar":
                    FlowPanel_Recording_Buttons.Visible = !FlowPanel_Recording_Buttons.Visible;
                    panel3.Visible = !panel3.Visible;
                    break;
                case "toggleControls":
                    ToggleControls();
                    break;
                case "paste":
                    {
                        if (this.ActiveControl is TextBox box)
                        {
                            box.Paste();
                        }
                        else if (Clipboard.ContainsText())
                        {
                            string s = Clipboard.GetText();
                            if (s.Length > 4 && s.Substring(0, 4).ToLower() == "http" || File.Exists(s) || Directory.Exists(s))
                            {
                                ExecuteCommand("pasteURL", s);
                            }
                            else if (s.Length > 7 && s.Substring(0, 7).ToLower() == "#extinf")
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
                    }
                    break;
                case "cut":
                    {
                        if (this.ActiveControl is TextBox box)
                        {
                            box.Cut();
                        }
                        else
                        {
                            ExecuteCommand("cutChannel");
                        }
                    }
                    break;
                case "copy":
                    {
                        if (this.ActiveControl is TextBox box)
                        {
                            box.Copy();
                        }
                        else
                        {
                            ExecuteCommand("copyChannel");
                        }
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

                        for (var (i,s) = (1, channel.id); channels.ContainsKey(channel.id) && i < 10; i++)
                        {
                            channel.id = s + " (" + i + ")";
                        }

                        if (!channels.ContainsKey(channel.id))
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
                    setChannelName("NEW CHANNEL");
                    CurSource = "live";
                    icyTitles.Clear();
                    icyPosTitles.Clear();
                    ListBox_Titles.Items.Clear();

                    curRecChannelName = "";

                    Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                    Button_Rec.Text = "Rec";
                    ClearChannelSelect();
                    TextBox_ShortName.Focus();

                    if (Directory.Exists(val))
                    {
                        ListBox_Files_LoadFromDir(val);
                    }

                    ListBox_Titles_LoadFromFile(val);
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

        private ColorSlider slider;

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
            AddMenuItem(mItem, "Titles", "titleList").ShortcutKeys = Keys.F7;
            AddMenuItem(mItem, "Files", "fileBrowser").ShortcutKeys = Keys.F8;
            AddMenuItem(mItem, "Controlpanels", "controlBar").ShortcutKeys = Keys.F9;
            AddMenuItem(mItem, "Menubar", "menuBar").ShortcutKeys = Keys.F10;
            AddMenuItem(mItem, "Fullscreen", "fullScreen").ShortcutKeys = Keys.F11;
            AddMenuItem(mItem, "Hide All", "toggleControls").ShortcutKeys = Keys.F12;
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
            ListBox_Titles.BackColor = ColorTranslator.FromHtml("#000000");

            PictureBox_Player.Dock = DockStyle.Fill;
            PictureBox_Player.Visible = true;

            Combo_ChannelSet.DropDownStyle = ComboBoxStyle.DropDownList;
            Combo_ShortName.DropDownStyle = ComboBoxStyle.DropDownList;

            panel3.Controls.Add(panel5);
            panel3.Controls.Add(panel8);

            panel5.Location = panel1.Location;
            panel5.BringToFront();

            panel8.Location = panel1.Location;
            panel8.BringToFront();

            PictureBox_Player.MouseWheel += new MouseEventHandler(MouseWheelHandler);
            Label_PlayerPos.MouseWheel += new MouseEventHandler(MouseWheelHandler);
            Label_PlayerPosFrac.MouseWheel += new MouseEventHandler(MouseWheelTicHandler);
            ListBox_Titles.MouseWheel += new MouseEventHandler(MouseWheelHandler);
            PictureBox_Player.MouseClick += new MouseEventHandler(PictureBox_Player_MouseClick);
            ListBox_Titles.MouseDown += new MouseEventHandler(ListBox_Titles_Click);

            TextBox_ShortName.Multiline = true;
            TextBox_ShortName.MinimumSize = new Size(0, 40);
            TextBox_ShortName.Size = new Size(TextBox_ShortName.Size.Width, 40);
            TextBox_ShortName.Multiline = false;

            TextBox_SearchFilter.Multiline = true;
            TextBox_SearchFilter.MinimumSize = new Size(0, 25);
            TextBox_SearchFilter.Size = new Size(TextBox_SearchFilter.Size.Width, 25);
            TextBox_SearchFilter.Multiline = false;
        }

        Timer timer1 = new System.Windows.Forms.Timer();

        private async void RadioZiner_Load(object sender, EventArgs e)
        {
            Panel_Files_Show();
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(Timer1_Tick);

            Player.Init(PictureBox_Player.Handle);
            Player.SetPropertyBool("deinterlace", true);

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

            exportFolder = Path.Combine(mainDir, "exports");
            if (!Directory.Exists(exportFolder))
            {
                Directory.CreateDirectory(exportFolder);
            }

            channelFolder = Path.Combine(mainDir, "channels");
            if (!Directory.Exists(channelFolder))
            {
                Directory.CreateDirectory(channelFolder);
            }

            streamFolder = Path.Combine(mainDir, "streams");
            if (!Directory.Exists(streamFolder))
            {
                Directory.CreateDirectory(streamFolder);
            }

            btnPlayPause.Text = "⏸️";
            Button_Mute.Text = "🔈";

            videoRecorder = new VideoRecorder(recordingFolder);

            slider = new ColorSlider
            {
                Dock = DockStyle.Top,
                Visible = true,
                ForeColor = Color.Red,
                BackColor = Color.Black,
                ElapsedInnerColor = Color.Red,
                ElapsedPenColorTop = Color.Red,
                ElapsedPenColorBottom = Color.Red,
                ThumbInnerColor = Color.Red,
                ThumbOuterColor = Color.Red,
                ThumbPenColor = Color.Red,
                TickStyle = TickStyle.None,

                ThumbRoundRectSize = new Size(0, 0),
                ThumbSize = new Size(1, 10),
                Width = panel3.Width,
                Height = 20,
                MouseWheelBarPartitions = 1000,

                SmallChange = 10,
                LargeChange = 60
            };
            slider.Scroll += Slider_Scroll;
            slider.MouseWheel += Slider_MouseWheel;
            slider.ValueChanged += Slider_ValueChanged;

            panel3.Controls.Add(slider);

            await Task.Delay(100);
            ReadChannelSets();
            await Task.Delay(100);
            ReadChannels();
            await Task.Delay(100);

            
            await ReadStreams();
            Set_SearchButton_Text();
        }

        private void Slider_ValueChanged(object sender, EventArgs e)
        {
        }

        private void Slider_MouseWheel(object sender, EventArgs e)
        {
            int seconds = Convert.ToInt32(((ColorSlider)sender).Value);
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
        }

        private void Slider_Scroll(object sender, EventArgs e)
        {
            int seconds = Convert.ToInt32(((ColorSlider)sender).Value);
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.Destroy();
            videoRecorder.StopAllRecordings();
        }

        private void ReadChannelSets()
        {
            Combo_ChannelSet.Items.Clear();
            foreach (var sFile in Directory.GetFiles(channelFolder, "*.m3u"))
            {
                Combo_ChannelSet.Items.Add(Path.GetFileName(sFile).Split('.')[0]);
            }
        }

        private char[] trimChars = new char[] { ' ', '\'', '#', '[', ']', '(', ')', '{', '}' };

        private async Task ReadSqlDump (string fPath)
        {
            Console.WriteLine("Load ...");

            Dictionary<string, List<string>> sqlDump = SqlDumpReader.Convert(fPath);

            Dispatcher dispatcherUI = Dispatcher.CurrentDispatcher;
            Label_Status.Text = "Milliseconds: ";
            var watch = Stopwatch.StartNew();
            List<Task> tasks = new List<Task>();

            int ListCounter = 0;

            if (sqlDump.Keys.Contains("Station"))
            {
                Console.WriteLine("Radiostations: " + sqlDump["Station"].Count);

                foreach (var station in sqlDump["Station"])
                {

                    var _allStreams = allStreams;

                    var _trimChars = trimChars;

                    ListCounter++;
                    if (ListCounter % 100 == 0)
                    {
                        await Task.Delay(1);
                    }

                    var t = Task.Run
                        (() =>
                        {
                            dispatcherUI.BeginInvoke
                            (new Action
                                (() =>
                                {
                                    M3u.TvgChannel c = new M3u.TvgChannel();
                                    using (var streamRdr = new StringReader(station.Replace(@"\'", "~!")))
                                    {
                                        char[] trimChars = new char[] { ' ', '\'', '#', '[', ']', '(', ')', '{', '}' };
                                        var csvReader = new CsvReader(streamRdr, ",");
                                        csvReader.Read();

                                        string name = csvReader[1].Trim(trimChars).Replace("~!", "´").Replace("\\\"", "\'").Replace('.', '_');
                                        string country = csvReader[23].ToLower().Trim();
                                        if (country == "gb")
                                        {
                                            country = "uk";
                                        }

                                        c.id = name + "." + country;
                                        c.url = csvReader[2];
                                        c.group = ".rdb " + csvReader[8].Trim('\'').Replace("~!", "´").Replace("\\\"", "\'");
                                        c.country = country;
                                    }

                                    if (!allStreams.ContainsKey(c.id))
                                    {
                                        allStreams.Add(c.id, c);
                                    }

                                    if (!Combo_FilterCountry.Items.Contains(c.country))
                                    {
                                        Combo_FilterCountry.Items.Add(c.country);
                                    }
                                }
                                )
                            , null
                            );
                        }
                        );
                    tasks.Add(t);
                }
                try
                {
                    await Task.Factory.ContinueWhenAll
                         (tasks.ToArray()
                            , result =>
                            {
                                var time = watch.ElapsedMilliseconds;
                                dispatcherUI.BeginInvoke
                              (new Action
                                    (() =>
                                    {
                                        Label_Status.Text += time.ToString();
                                    }
                                     )
                               );
                            }
                        );
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("... loaded");
            }
        }

        private async Task ReadStreams()
        {
            if (allStreams.Count() == 0)
            {
                
                foreach (var f in Directory.GetFiles(streamFolder, "*.m3u"))
                {
                    foreach (var c in M3u.GetTvgChannels(Path.Combine(streamFolder, f), out TvgGroups, out TvgCountries))
                    {
                        var tvgChannel = c.Value;
                        tvgChannel.file = f;
                        tvgChannel.group = ".tvg " + tvgChannel.group;
                        if (!allStreams.Keys.Contains(c.Key))
                        {
                            allStreams.Add(c.Key, tvgChannel);
                        }
                    }
                }
                
                
                foreach (var f in Directory.GetFiles(streamFolder, "*.sql"))
                {
                    await ReadSqlDump(Path.Combine(streamFolder, f));
                }
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
            foreach (Button c in FlowPanel_Recording_Buttons.Controls)
            {
                c.BackColor = Color.Black;
            }
            foreach (Button c in FlowPanel_FileStream_Buttons.Controls)
            {
                c.BackColor = Color.Black;
            }
            Combo_ShortName.BackColor = Color.Black;

        }

        private void UpdateTitleList(string recShortName)
        {
            if (recShortName == curRecChannelName)
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
            if (curRecChannelName == "")
            {
                string url = TextBox_Url.Text;
                string shortName = ReplaceInvalidChars (TextBox_ShortName.Text);

                if (!recorders.Keys.Contains(shortName) && url != "")
                {
                    if (shortName!="")
                    {
                        RadioRecorder r = new RadioRecorder();
                        r.TitleAdded += UpdateTitleList;
                        r.url = url;
                        r.streamingFolder = recordingFolder;
                        r.shortName = shortName;
                        r.extRecorder = shortName.EndsWith("@") ? 1 : extRecorder;
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

                        FlowPanel_Recording_Buttons.Controls.Add(btn);

                        if (!allChannels.Keys.Contains(shortName) && Combo_ChannelSet.Text != "") //allChannels
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

                        if (r.extRecorder > 0)
                        {
                            videoRecorder.StartRecording(channels[shortName]);
                        }
                    }
                }
            }
            else
            {
                videoRecorder.StopRecording(curRecChannelName);

                foreach (var c in FlowPanel_Recording_Buttons.Controls)
                {
                    if (c is Button button && button.Text == curRecChannelName)
                    {
                        FlowPanel_Recording_Buttons.Controls.Remove(button);
                        break;
                    }
                }
                ListBox_Titles.Items.Clear();
                var r = recorders[curRecChannelName];
                recorders.Remove(curRecChannelName);
                curRecChannelName = "";
                Combo_ShortName.Text = "";
                r.Stop();
                Player.CommandV("stop");
                Button_Rec.Text = "Rec";
            }
        }

        private void Button_ChangeChannel_Click(object sender, EventArgs e)
        {
            ClearChannelSelect();
            if (curRecChannelName != "")
            {
                recorders[curRecChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
            }

            Button btn = (Button)sender;

            curRecChannelName = btn.Text;
            btn.BackColor = Color.Blue;

            if (!Combo_ShortName.Items.Contains(curRecChannelName))
            {
                if (allChannels.Keys.Contains(curRecChannelName))
                {
                    string sFile = Path.GetFileNameWithoutExtension(allChannels[curRecChannelName].file);
                    Combo_ChannelSet.Text = sFile;
                }
            }

            Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
            Combo_ShortName.Text = curRecChannelName;
            Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;

            CurSource = "timeshift";
            icyTitles.Clear();
            icyPosTitles.Clear();
            ListBox_Titles.Items.Clear();
            UpdateTitleList(curRecChannelName);

            TextBox_Url.Text = recorders[curRecChannelName].url;
            Button_Rec.Text = "Stop";

            setChannelName(curRecChannelName);

            Label_StartTime.Text = "00:00:00";
            Label_EndTime.Text = "00:00:00";
            TextBox_ExportFileName.Text = curRecChannelName;
            // ToDo - hasIcyTitles is not pricise - should check for video steam instead
            if (recorders[curRecChannelName].hasIcyTitles)
            {
                Combo_ExportFileExtension.Text = ".mp3";
            }
            else
            {
                Combo_ExportFileExtension.Text = ".mp4";
            }

            Panel_Files_Hide();
        }

        private void ListBox_Titles_SaveToFile(string sTitlesFile)
        {
            if (File.Exists(sTitlesFile))
            {
                string fName = sTitlesFile;
                int pos = sTitlesFile.LastIndexOf('.');
                if (pos >= 0)
                {
                    fName = fName.Substring(0, pos);
                }
                fName += ".txt";
                List<string> titles = new List<string>();
                foreach (var item in ListBox_Titles.Items)
                {
                    titles.Add((string)item);
                }
                File.WriteAllLines(fName, titles);
            }
        }

        private readonly Regex Rgx_TitlePos = new Regex(@"(?<hours>[0-9][0-9]):(?<minutes>[0-5][0-9]):(?<seconds>[0-5][0-9])((\s)|(.(?<milliseconds>[0-9]{3})))");

        private string TitlePos2TimeString (string line)
        {
            Match m = Rgx_TitlePos.Match(line);
            if (m.Success)
            {
                string s = m.Groups["hours"].Value + ":" + m.Groups["minutes"].Value + ":" + m.Groups["seconds"].Value;
                if (m.Groups["milliseconds"].Value != "")
                {
                    s += "." + m.Groups["milliseconds"].Value;
                }
                return s;
            }
            return "";
        }
       
        private string TitlePos2secs(string line)
        {
            Match m = Rgx_TitlePos.Match(line);
            if (m.Success)
            {
                int i = int.Parse(m.Groups["hours"].Value) * 3600;
                i += int.Parse(m.Groups["minutes"].Value) * 60;
                i += int.Parse(m.Groups["seconds"].Value);

                string s = i.ToString();
                if (m.Groups["milliseconds"].Value != "")
                {
                    s += "."  + m.Groups["milliseconds"].Value;
                }
                return s;
            }
            return "";
        }

        private void ListBox_Titles_LoadFromFile(string sTitlesFile)
        {
            if (File.Exists(sTitlesFile))
            {
                string fName = sTitlesFile;

                int pos = sTitlesFile.LastIndexOf('.');
                if (pos >= 0)
                {
                    fName = fName.Substring(0, pos);
                }
                fName += ".txt";
                ListBox_Titles.Items.Clear();
                if (File.Exists(fName))
                {
                    foreach (var line in File.ReadAllLines(fName))
                    {
                        if (Rgx_TitlePos.IsMatch(line))
                        {
                            ListBox_Titles.Items.Add(line);
                        }
                    }
                }

                Label_TitleTime.Text = "00:00:00";
                Label_TitleTimeFrac.Text = ".000";
                Label_StartTime.Text = "00:00:00";
                Label_StartTimeFrac.Text = ".000";
                Label_EndTime.Text = "00:00:00";
                Label_EndTimeFrac.Text = ".000";

                TextBox_ExportFileName.Text = Path.GetFileNameWithoutExtension(sTitlesFile);
                Combo_ExportFileExtension.Text = ".mp4";
            }
        }

        private void ListBox_Titles_Click(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    
                    if (ListBox_Titles.SelectedItem != null)
                    {
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            string sPos = TitlePos2TimeString(ListBox_Titles.SelectedItem.ToString());
                            if (sPos!="")
                            {
                                var a = sPos.Split('.');
                                Label_TitleTime.Text = a[0];
                                if (a.Count()>1)
                                {
                                    Label_TitleTimeFrac.Text = "." + a[1];
                                }
                                else
                                {
                                    Label_TitleTimeFrac.Text = ".000";
                                }
                                
                                TextBox_TitleEdit.Text = ListBox_Titles.SelectedItem.ToString().Substring(sPos.Length);

                                Label_StartTime.Text = Label_TitleTime.Text;
                                Label_StartTimeFrac.Text = Label_TitleTimeFrac.Text;
                                TextBox_ExportFileName.Text = TextBox_TitleEdit.Text;
                                Combo_ExportFileExtension.Text = ".mp3";
                                int index = ListBox_Titles.SelectedIndex + 1;
                                if (index < ListBox_Titles.Items.Count)
                                {
                                    sPos = TitlePos2TimeString(ListBox_Titles.Items[index].ToString());
                                    if (sPos != "")
                                    {
                                        a = sPos.Split('.');
                                        Label_EndTime.Text = a[0];
                                        if (a.Count() > 1)
                                        {
                                            Label_EndTimeFrac.Text = "." + a[1];
                                        }
                                        else
                                        {
                                            Label_EndTimeFrac.Text = ".000";
                                        }
                                    }
                                }
                                else
                                {
                                    Label_EndTime.Text = Label_RecLen.Text;
                                    Label_EndTimeFrac.Text = ".000";
                                }
                            }
                        }
                        else
                        {
                            Player.CommandV("seek", TitlePos2secs(ListBox_Titles.SelectedItem.ToString()), "absolute"); // seconds.ToString(CultureInfo.InvariantCulture) + ".260"
                        }
                    }

                    break;

                case MouseButtons.Middle:
                    ExecuteCommand("titleList");
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
            foreach (var c in channels)
            {
                Combo_ShortName.Items.Add(c.Key);
            }
        }

        private void Combo_ChannelSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReadSelectedChannelSet();
            PictureBox_Player.Focus();
        }

        private void ListBox_Files_LoadFromDir(string val, string source = "files")
        {
            if (!Directory.Exists(val))
            {
                return;
            }
            ListView listView;
            switch (source)
            {
                case "recordings":
                    listView = ListView_Recordings;
                    break;
                case "exports":
                    listView = ListView_Exports;
                    break;
                default:
                    listView = ListView_Files;
                    break;

            }
            listView.Clear();
            listView.Columns.Add("Filename", 450, HorizontalAlignment.Left);
            listView.Tag = val;


            var allowedExtensions = new[] { ".mp4", ".mp3", ".aac", ".ts", ".avi", ".mpg", ".mpeg",  ".mkv"};
            foreach (var sFile in Directory.EnumerateFiles(val, "*.*",
                System.IO.SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .OrderByDescending(file => new FileInfo(file).CreationTime)
                )
            {
                ListViewItem lvItem = new ListViewItem(sFile.Substring(val.Length + (val.EndsWith("\\") ? 0 : 1)), 0);
                lvItem.SubItems.Add("Test");
                lvItem.Tag = new M3u.TvgChannel();
                listView.Items.Add(lvItem);
            }

            Panel_Files_Show();

        }

        private static ListViewItem ListBox_Streams_AddFiltered (M3u.TvgChannel channel, string country, List<string> words)
        {
            if (!words.All(w => (channel.group + " " + channel.id).ToLower().Contains(w)))
            {
                return null;
            }
            if (country == "" || channel.id.ToLower().Contains("." + country.ToLower()))
            {
                var a = channel.id.Split('.'); // was Key
                ListViewItem lvItem = new ListViewItem(a[0], 0);
                lvItem.SubItems.Add(a.Count() > 1 ? "." + a[1] : "");
                lvItem.SubItems.Add(channel.group);
                lvItem.Tag = channel;
                return lvItem;
            }
            else
            {
                return null;
            }
        }

        private void ListBox_Files_LoadFromChannels()
        {
            ListView_Files.Clear();
            ListView_Files.Columns.Add("Filename", 450, HorizontalAlignment.Left);
            ListView_Files.Tag = "channels";
            foreach (var channel in allChannels)
            {
                if (!channel.Value.url.StartsWith("http"))
                {
                    string channelKey = channel.Key;
                    if (!File.Exists(channel.Value.url))
                    {
                        channelKey = "[" + channelKey + "]";
                    }

                    ListViewItem lvItem = new ListViewItem(channelKey, 0);
                    lvItem.SubItems.Add("Test");
                    lvItem.Tag = channel.Value;
                    ListView_Files.Items.Add(lvItem);
                }
            }

            Panel_Files_Show();

        }

        private void Combo_ShortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearChannelSelect();
            Combo_ShortName.BackColor = Color.Blue;
            if (curRecChannelName != "")
            {
                recorders[curRecChannelName].lastPlayPos = Player.GetPropertyDouble("time-pos");
            }

            string s = Combo_ShortName.Text;
            TextBox_Url.Text = channels.Keys.Contains(s) ? channels[s].url : "";
            Player.CommandV("stop");

            curRecChannelName = "";

            Button_Rec.Text = "Rec";

            if (Directory.Exists(TextBox_Url.Text))
            {
                ListBox_Files_LoadFromDir(TextBox_Url.Text);
            }
            else
            {
                Panel_Files_Hide();
                Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                setChannelName(s);
                CurSource = "live";
                icyTitles.Clear();
                icyPosTitles.Clear();
                ListBox_Titles.Items.Clear();
            }

            PictureBox_Player.Focus();
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
                if (s.Length>4 && s.Substring(0, 4).ToLower() == "http")
                {
                    ExecuteCommand("pasteURL", s);

                    setChannelName("NEW CHANNEL");
                    CurSource = "live";
                    icyTitles.Clear();
                    icyPosTitles.Clear();
                    ListBox_Titles.Items.Clear();

                    TextBox_Url.Text = s;
                    Combo_ShortName.SelectedIndexChanged -= Combo_ShortName_SelectedIndexChanged;
                    Combo_ShortName.Text = "";
                    Combo_ShortName.SelectedIndexChanged += Combo_ShortName_SelectedIndexChanged;

                    curRecChannelName = "";

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

        private static string GetFormatedPlayerPos (double pos)
        {

            if (pos >= 0)
            {
                TimeSpan tPos = TimeSpan.FromSeconds(pos);
                return string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
            }
            else
            {
                return "--:--:--";
            }
        }

        private static string GetPlayerPosFractionalPart(double pos, int digits = 3)
        {

            if (pos >= 0)
            {
                pos = pos - Math.Truncate(pos);
                string formatString = "{0:0." + "".PadRight(digits, '0') + "}";
                string s = String.Format(formatString, pos).TrimStart('0').TrimStart('1').TrimStart('.').TrimStart(',');
                return "." + s;
            }
            else
            {
                return "-.-";
            }
        }

        private static void UpdateSlider (ColorSlider slider, double min, double max, double val)
        {
            slider.Minimum = 0;
            slider.Maximum = (int)max > 0 ? (int)max : 7200;
            slider.Value = (int)val > 0 && (int)val <= max ? (int)val : 0;
        }

        private void UpdateTitlesPos (double pos)
        {
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
        }

        private void SeekToLastRecorderPos ()
        {
            double lastPos = recorders[curRecChannelName].lastPlayPos;
            if (lastPos > 0)
            {
                if (!recorders[curRecChannelName].hasIcyTitles)
                {
                    Player.SetPropertyBool("pause", true);
                }
                Player.CommandV("seek", lastPos.ToString(CultureInfo.InvariantCulture), "absolute");
                double pos = Player.GetPropertyDouble("time-pos");
                if (pos >= lastPos)
                {
                    recorders[curRecChannelName].lastPlayPos = 0;
                    if (!recorders[curRecChannelName].hasIcyTitles)
                    {
                        Player.SetPropertyBool("pause", false);
                    }
                }
            }
        }


        private List<string> icyTitles = new List<string>();
        private List<string> icyPosTitles = new List<string>();
        private bool hasIcyTitles = false;

        private bool UpdateTitles()
        {
            bool changed = false;
            double pos = Player.GetPropertyDouble("time-pos", false);
            string title = Player.GetPropertyString("media-title");
            if (title != "" && !icyTitles.Contains(title))
            {
                icyTitles.Add(title);
                TimeSpan tPos = TimeSpan.FromSeconds(pos);
                string sPos = string.Format("{0:D2}:{1:D2}:{2:D2}", tPos.Hours, tPos.Minutes, tPos.Seconds);
                string sPosTitle = sPos + " - " + title;
                icyPosTitles.Add(sPosTitle);

                icyPosTitles.Sort();

                ListBox_Titles.Items.Clear();
                foreach (var s in icyPosTitles)
                {
                    ListBox_Titles.Items.Add(s);
                }
                changed = true;
            }

            if (icyTitles.Count() > 1)
            {
                hasIcyTitles = true;
            }

            return (changed);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (Player.GetPropertyString("video-codec") != "")
            {
                ListBox_Titles.Dock = DockStyle.Top;
                ListBox_Titles.Height = 250;
                PictureBox_Player.Show();
                PictureBox_Player.BringToFront();
            }
            else if (Player.GetPropertyString("audio-codec") != "")
            {
                ListBox_Titles.Dock = DockStyle.Fill;
                PictureBox_Player.Hide();
                ListBox_Titles_Show();
            }

            if (curRecChannelName=="")
            {
                double curPos = Player.GetPropertyDouble("time-pos");
                double recLen = Player.GetPropertyDouble("time-remaining") + curPos;

                Label_PlayerPos.Text = GetFormatedPlayerPos(curPos);
                Label_PlayerPosFrac.Text = GetPlayerPosFractionalPart(curPos);
                Label_RecLen.Text = GetFormatedPlayerPos(recLen);

                UpdateSlider(slider, 0, recLen, curPos);
                if (CurSource == "live")
                {
                    UpdateTitles();
                }
                UpdateTitlesPos(curPos);
            }
            else if (recorders.ContainsKey(curRecChannelName) && recorders[curRecChannelName]?.recordingToFile != "")
            {
                if (Player.GetPropertyString("path") != recorders[curRecChannelName]?.recordingToFile)
                {
                    Player.CommandV("loadfile", recorders[curRecChannelName].recordingToFile, "replace");
                }

                double curPos = Player.GetPropertyDouble("time-pos");
                double recLen = recorders[curRecChannelName].GetRecordLength();

                UpdateSlider(slider, 0, recLen, curPos);
                UpdateTitlesPos(curPos);

                Label_PlayerPos.Text = GetFormatedPlayerPos(curPos);
                Label_PlayerPosFrac.Text = GetPlayerPosFractionalPart(curPos);
                Label_RecLen.Text = GetFormatedPlayerPos(recLen);

                SeekToLastRecorderPos();
           }
        }

        public string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }

        private string oldShortName = "";

        private void TextBox_ShortName_KeyDown(object sender, KeyEventArgs e)
        {
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
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                TextBox_ShortName.Text = curChannelName;
                oldShortName = curChannelName;
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
                    ExecuteCommand("titleList");
                    break;

                case MouseButtons.Right:
                    ExecuteCommand("togglePlayerPause");
                    break;
            }
            PictureBox_Player.Focus();
        }

        public void MouseWheelTicHandler(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (e.Delta > 0)
                {
                    Player.CommandV("seek", "0.01", "relative");
                }
                else
                {
                    Player.CommandV("seek", "-0.01", "relative");
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    Player.CommandV("seek", "0.1", "relative");
                }
                else
                {
                    Player.CommandV("seek", "-0.1", "relative");
                }
            }
        }

        public void MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (Player.GetPropertyString("video-codec") != "" && !(sender is PictureBox))
            {
                return;
            }
            if (e.Location.X > ListBox_Titles.Width - 100 && !(sender is PictureBox))
            {
                return;
            }
            ((HandledMouseEventArgs)e).Handled = true;
            if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
            {
                if (e.Delta > 0)
                {
                    if (Player.GetPropertyBool("pause"))
                    {
                        if (Player.GetPropertyString("video-codec") != "")
                        {
                            Player.CommandV("frame-step");
                        }
                        else
                        {
                            Player.CommandV("seek", "0.1", "relative");
                        }
                        return;
                    }

                    Player.CommandV("seek", "3", "relative");
                }
                else if (Player.GetPropertyBool("pause"))
                {
                    if (Player.GetPropertyString("video-codec") != "")
                    {
                        Player.CommandV("frame-back-step");
                    }
                    else
                    {
                        Player.CommandV("seek", "-0.1", "relative");
                    }
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

        private bool controlsVisible = true;
        private void ToggleControls(int setState = -1)
        {
            bool setVisible = controlsVisible;
            if (setState>=0)
            {
                setVisible = setState == 1;
            }
            if (setVisible)
            {
                controlsVisible = false;
                MenuBar.Visible = false;
                FlowPanel_Recording_Buttons.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                TextBox_Url.Visible = false;
                panel6.Hide();
                Panel_Files_Hide();
            }
            else
            {
                controlsVisible = true;
                MenuBar.Visible = true;
                FlowPanel_Recording_Buttons.Visible = true;
                panel3.Visible = true;
                panel4.Visible = true;
                TextBox_Url.Visible = true;
                panel6.Show();
            }
        }

        private void ListBox_Titles_DoubleClick(object sender, EventArgs e)
        {
            ExecuteCommand("fullScreen");

            ToggleControls(FormBorderStyle == FormBorderStyle.None ? 1 : 0);
        }

        private void Button_ExportSave_Click(object sender, EventArgs e)
        {
            string ExportFileName = ReplaceInvalidChars(TextBox_ExportFileName.Text);
            string ExportFileExtension = ReplaceInvalidChars(Combo_ExportFileExtension.Text);
            string destFile = Path.Combine(exportFolder, ExportFileName + ExportFileExtension);

            int i = 1;
            while (File.Exists(destFile) && i<100)
            {
                destFile = Path.Combine(exportFolder, ReplaceInvalidChars(ExportFileName + " (" + i + ")" + ExportFileExtension));
                i++;
            }

            string param = "-ss "
                         + Label_StartTime.Text + Label_StartTimeFrac.Text
                         + " -to "
                         + Label_EndTime.Text + Label_EndTimeFrac.Text
                         + " -i \""
                         + Player.GetPropertyString("path")
                         + "\" -c copy \""
                         + destFile
                         + "\"";

            string ffMpegExeFile = Application.StartupPath + @"\ffmpeg.exe";

            ConsoleUtil.StartProgram(ffMpegExeFile, param);
        }

        private void Label_StartTime_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                Label_StartTime.Text = Label_PlayerPos.Text;
                Label_StartTimeFrac.Text = Label_PlayerPosFrac.Text;
            }
            else
            {
                Player.CommandV("seek", TitlePos2secs(Label_StartTime.Text + Label_StartTimeFrac.Text), "absolute", "exact");
            }
        }

        private void Label_TitleTime_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                Label_TitleTime.Text = Label_PlayerPos.Text;
                Label_TitleTimeFrac.Text = Label_PlayerPosFrac.Text;
            }
            else
            {
                Player.CommandV("seek", TitlePos2secs(Label_TitleTime.Text + Label_TitleTimeFrac.Text), "absolute", "exact");
            }
        }

        private void Label_EndTime_Click(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                Label_EndTime.Text = Label_PlayerPos.Text;
                Label_EndTimeFrac.Text = Label_PlayerPosFrac.Text;
            }
            else
            {
                Player.CommandV("seek", TitlePos2secs(Label_EndTime.Text + Label_EndTimeFrac.Text), "absolute", "exact");
            }
        }

        private void Panel_Files_Hide ()
        {
            Panel_Files.Hide();
            Label_Toggle_ListBox_Files.Text = "▷";
        }

        private void Panel_Files_Show()
        {
            Panel_Files.Show();
            Label_Toggle_ListBox_Files.Text = "▼";
        }

        private void ListBox_Titles_Hide()
        {
            PictureBox_Player.Show();
            ListBox_Titles.Hide();
            Label_Toggle_ListBox_Titles.Text = "◁";
        }

        private void ListBox_Titles_Show()
        {
            ListBox_Titles.Show();
            Label_Toggle_ListBox_Titles.Text = "▼";
        }

        private void Select_ListFiles_Button (string sButton = "")
        {
            foreach (Control control in FlowPanel_FileStream_Buttons.Controls)
            {
                if (control is Button)
                {
                    if (control.Tag.ToString() == sButton)
                    {
                        control.ForeColor = Color.Orange;
                    }
                    else
                    {
                        control.ForeColor = Color.White;
                    }
                }
            }
        }

        private void Button_ListFiles_Click(object sender, EventArgs e)
        {
            string sButton = ((Button)sender).Tag.ToString();
            switch (sButton)
            {
                case "streams":
                    Panel_StreamFilter.Show();

                    ListView_Streams.Show();
                    ListView_Recordings.Hide();
                    ListView_Exports.Hide();
                    ListView_Files.Hide();
                    break;
                case "recordings":
                    Panel_StreamFilter.Hide();

                    ListView_Streams.Hide();
                    ListView_Recordings.Show();
                    ListView_Exports.Hide();
                    ListView_Files.Hide();
                    if (ListView_Recordings.Items.Count == 0 || Panel_Files.Visible && ((Button)sender).ForeColor == Color.Orange)
                    {
                        ListBox_Files_LoadFromDir(recordingFolder, "recordings");
                    }
                    break;
                case "exports":
                    Panel_StreamFilter.Hide();

                    ListView_Streams.Hide();
                    ListView_Recordings.Hide();
                    ListView_Exports.Show();
                    ListView_Files.Hide();
                    if (ListView_Exports.Items.Count == 0 || Panel_Files.Visible && ((Button)sender).ForeColor == Color.Orange)
                    {
                        ListBox_Files_LoadFromDir(exportFolder, "exports");
                    }
                    break;
                case "files":
                    Panel_StreamFilter.Hide();

                    ListView_Streams.Hide();
                    ListView_Recordings.Hide();
                    ListView_Exports.Hide();
                    ListView_Files.Show();
                    if (ListView_Files.Items.Count == 0 || Panel_Files.Visible && ((Button)sender).ForeColor == Color.Orange)
                    {
                        ListBox_Files_LoadFromChannels();
                    }
                    break;
            }
            Select_ListFiles_Button(sButton);
            Panel_Files_Show();
        }

        private void Toggle_ListBox_Files ()
        {
            if (Panel_Files.Visible)
            {
                Panel_Files_Hide();
            }
            else
            {
                Panel_Files_Show();
            }
        }

        private void Toggle_ListBox_Titles()
        {
            if (ListBox_Titles.Visible)
            {
                ListBox_Titles_Hide();
            }
            else
            {
                ListBox_Titles_Show();
            }
        }

        private void Label_Toggle_ListBox_Files_Click(object sender, EventArgs e)
        {
            Toggle_ListBox_Files();
        }

        private void Label_Toggle_ListBox_Titles_Click(object sender, EventArgs e)
        {
            Toggle_ListBox_Titles();
        }

        private void Combo_FilterCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private bool ListView_Url_Changed (ListView listView)
        {
            if (listView.SelectedItems.Count == 0)
            {
                return false;
            }
            M3u.TvgChannel item = (M3u.TvgChannel)(listView.SelectedItems[0].Tag);

            string sTag = listView.Tag.ToString();
            string selectedItem = listView.SelectedItems[0].Text;//item.id.ToString();
            string sUrl = "";
            string sTitle = "";

            switch (sTag)
            {
                case "channels":
                    CurSource = "file";
                    string sFile = allChannels[selectedItem.Trim('[').Trim(']')].url;
                    if (selectedItem.StartsWith("[") && selectedItem.EndsWith("]"))
                    {
                        ListBox_Files_LoadFromDir(sFile);
                        return false;
                    }
                    else
                    {
                        sUrl = sFile;
                    }
                    sTitle = ReplaceInvalidChars(selectedItem);
                    break;
                case "streams":
                    CurSource = "live";
                    sUrl = item.url;
                    sTitle = item.id;
                    break;
                default:
                    CurSource = "file";
                    sUrl = Path.Combine(sTag, selectedItem);
                    sTitle = ReplaceInvalidChars(selectedItem);
                    break;
            }

            TextBox_Url.Text = sUrl;
            setChannelName(sTitle);
            icyTitles.Clear();
            icyPosTitles.Clear();
            ListBox_Titles.Items.Clear();

            curRecChannelName = "";

            Player.CommandV("loadfile", TextBox_Url.Text, "replace");
            Button_Rec.Text = "Rec";

            ListBox_Titles_LoadFromFile(sUrl);
            return true;
        }

        private void ListView_Streams_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (ListView_Url_Changed(listView))
            {
                ClearChannelSelect();
                Button_ListStreams.BackColor = Color.Blue;
            }
        }


        private void ListView_Recordings_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (ListView_Url_Changed(listView))
            {
                ClearChannelSelect();
                Button_ListRecordings.BackColor = Color.Blue;
            }
        }

        private void ListView_Exports_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (ListView_Url_Changed(listView))
            {
                ClearChannelSelect();
                Button_ListExports.BackColor = Color.Blue;
            }
        }

        private void ListView_Files_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listView = (ListView)sender;
            if (ListView_Url_Changed(listView))
            {
                ClearChannelSelect();
                Button_ListFiles.BackColor = Color.Blue;
            }
        }

        private void ShowCounter(string s)
        {
            LabelCounter.Text = s;
        }

        private async void Search()
        {
            CancelSearch = false;
            Searching = true;
            Set_SearchButton_Text();
            ListView_Streams.Clear();
            ListView_Streams.Columns.Add("Streams", 150, HorizontalAlignment.Left);
            ListView_Streams.Columns.Add("Ctry", 35, HorizontalAlignment.Left);
            ListView_Streams.Columns.Add("Src Tags", 150, HorizontalAlignment.Left);
            ListView_Streams.Tag = "streams";
            int ListCounter = 0;
            int LoopCounter = 0;
            LabelCounter.Text = "";
            Dispatcher dispatcherUI = Dispatcher.CurrentDispatcher;
            Label_Status.Text = "Milliseconds: ";
            var watch = Stopwatch.StartNew();
            List<Task> tasks = new List<Task>();

            string country = Combo_FilterCountry.Text;
            List<string> words = new List<string>();
            words.AddRange(TextBox_SearchFilter.Text.ToLower().Split(' '));


            foreach (var c in allStreams)
            {
                if (CancelSearch)
                {
                    break;
                }
                M3u.TvgChannel channel = c.Value;

                var lvItem = ListBox_Streams_AddFiltered(channel, country, words);

                LoopCounter++;
                if (LoopCounter % 1000 == 0)
                {
                    await Task.Delay(1);
                }
                if (lvItem != null)
                {
                    ListCounter++;
                    if (ListCounter % 20 == 0)
                    {
                        await Task.Delay(1);
                    }

                    var t = Task.Run
                    (() =>
                        {
                            dispatcherUI.BeginInvoke
                            (new Action
                                 (() =>
                                 {
                                     ListView_Streams.Columns[0].Text = "Streams (" + ListCounter + ")";
                                     ListView_Streams.Items.Add(lvItem);
                                 }
                                 )
                             , null
                            );
                        }
                    );
                    tasks.Add(t);
                }
            }

            try
            {
                await Task.Factory.ContinueWhenAll
                     (tasks.ToArray()
                        , result =>
                        {
                            var time = watch.ElapsedMilliseconds;
                            dispatcherUI.BeginInvoke
                          (new Action
                                (() =>
                                {
                                    ShowCounter(ListCounter.ToString());
                                    Label_Status.Text += time.ToString();
                                    ListView_Streams.Columns[0].Text = "Channels (" + ListCounter + ")";
                                    if (CancelSearch)
                                    {
                                        ListView_Streams.Clear();
                                    }
                                    Searching = false;
                                    Set_SearchButton_Text();
                                }
                                 )
                           );
                        }
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (CancelSearch)
                {
                    ListView_Streams.Clear();
                }
                Searching = false;
                Set_SearchButton_Text();
            }
        }

        private bool CancelSearch = false;
        private bool Searching = false;

        private void Set_SearchButton_Text ()
        {
            Button_StreamSearch.Enabled = true;

            if (Searching)
            {
                Button_StreamSearch.Text = "Cancel";
            }
            else
            {
                Button_StreamSearch.Text = "Search";
            }
        }

        private void Button_StreamSearch_Click(object sender, EventArgs e)
        {
            if (Searching)
            {
                CancelSearch = true;
            }
            else
            {
                Search();
            }
        }

        private void Button_Channels_Click(object sender, EventArgs e)
        {
            Button_Channels.ForeColor = Color.Orange;
            Button_Export.ForeColor = Color.White;
            Button_Title.ForeColor = Color.White;
            panel1.Visible = true;
            panel5.Visible = false;
            panel8.Visible = false;
        }

        private void Button_Export_Click(object sender, EventArgs e)
        {
            Button_Channels.ForeColor = Color.White;
            Button_Export.ForeColor = Color.Orange;
            Button_Title.ForeColor = Color.White;
            panel1.Visible = false;
            panel5.Visible = true;
            panel8.Visible = false;
        }

        private void Button_Title_Click(object sender, EventArgs e)
        {
            Button_Channels.ForeColor = Color.White;
            Button_Export.ForeColor = Color.White;
            Button_Title.ForeColor = Color.Orange;
            panel1.Visible = false;
            panel5.Visible = false;
            panel8.Visible = true;
        }

        public void DeleteTitle(string pos, string title)
        {
            timer1.Enabled = false;

            ListBox_Titles.Items.Remove(pos + title);
            ListBox_Titles_SaveToFile(TextBox_Url.Text);

            timer1.Enabled = true;
        }

        public void AddTitle(string pos, string title = "")
        {
            timer1.Enabled = false;

            ListBox_Titles.Items.Add(pos + title);
            ListBox_Titles_SaveToFile(TextBox_Url.Text);

            timer1.Enabled = true;
        }

        private void Button_TitleAdd_Click(object sender, EventArgs e)
        {
            if (curRecChannelName != "" && recorders.ContainsKey(curRecChannelName))
            {
                recorders[curRecChannelName].AddTitle(Label_TitleTime.Text + Label_TitleTimeFrac.Text, TextBox_TitleEdit.Text, "");
            }
            else
            {
                AddTitle(Label_TitleTime.Text + Label_TitleTimeFrac.Text, TextBox_TitleEdit.Text);
            }
        }

        private void Button_Title_Delete_Click(object sender, EventArgs e)
        {
            if (curRecChannelName != "" && recorders.ContainsKey(curRecChannelName))
            {
                recorders[curRecChannelName].DeleteTitle(Label_TitleTime.Text + Label_TitleTimeFrac.Text, TextBox_TitleEdit.Text);
                recorders[curRecChannelName].DeleteTitle(Label_TitleTime.Text, TextBox_TitleEdit.Text);
            }
            else
            {
                DeleteTitle(Label_TitleTime.Text + Label_TitleTimeFrac.Text, TextBox_TitleEdit.Text);
                DeleteTitle(Label_TitleTime.Text, TextBox_TitleEdit.Text);
            }
        }

        private async void TextBox_SearchFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CancelSearch = true;
                if (Button_StreamSearch.Enabled)
                {
                    while (Searching)
                    {
                        await Task.Delay(1);
                    }
                    Search();
                }
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                CancelSearch = true;
            }
        }

        private async void TextBox_SearchFilter_TextChanged(object sender, EventArgs e)
        {
            CancelSearch = true;
            if (Button_StreamSearch.Enabled && TextBox_SearchFilter.Text.Length > 1)
            {
                while (Searching)
                {
                    await Task.Delay(1);
                }
                if (TextBox_SearchFilter.Text.Length > 1)
                {
                    Search();
                }
                else
                {
                    CancelSearch = true;
                }
            }
        }
    }
}
