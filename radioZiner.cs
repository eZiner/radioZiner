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

        List<Font> LV_FontSizes = new List<Font>();
        int LV_Font_Index = 4;


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
                    Toggle_ListView_Titles();
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
                    ListView_Titles_Clear();

                    curRecChannelName = "";

                    Player.CommandV("loadfile", TextBox_Url.Text, "replace");
                    Button_Rec.Text = "Rec";
                    ClearChannelSelect();
                    TextBox_ShortName.Focus();

                    if (Directory.Exists(val))
                    {
                        ListBox_Files_LoadFromDir(val);
                    }

                    ListView_Titles_LoadFromFile(val);
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

        private ColorSlider slider1;
        private ColorSlider slider2;

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

            ListView_Titles.Dock = DockStyle.Fill;
            ListView_Titles.Visible = false;
            ListView_Titles.BackColor = ColorTranslator.FromHtml("#000000");

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

            PictureBox_Player.MouseWheel += new MouseEventHandler(PictureBox_MouseWheelHandler);
            Label_PlayerPos.MouseWheel += new MouseEventHandler(PictureBox_MouseWheelHandler);
            Label_PlayerPosFrac.MouseWheel += new MouseEventHandler(MouseWheelTicHandler);
            ListView_Titles.MouseWheel += new MouseEventHandler(ListView_Titles_MouseWheelHandler);
            PictureBox_Player.MouseClick += new MouseEventHandler(PictureBox_Player_MouseClick);
            ListView_Titles.MouseUp += new MouseEventHandler(ListView_Titles_Click);

            ListView_Files.MouseWheel += new MouseEventHandler(ListView_MouseWheelHandler);
            ListView_Exports.MouseWheel += new MouseEventHandler(ListView_MouseWheelHandler);
            ListView_Recordings.MouseWheel += new MouseEventHandler(ListView_MouseWheelHandler);
            ListView_Streams.MouseWheel += new MouseEventHandler(ListView_MouseWheelHandler);


            TextBox_ShortName.Multiline = true;
            TextBox_ShortName.MinimumSize = new Size(0, 40);
            TextBox_ShortName.Size = new Size(TextBox_ShortName.Size.Width, 40);
            TextBox_ShortName.Multiline = false;

            TextBox_SearchFilter.Multiline = true;
            TextBox_SearchFilter.MinimumSize = new Size(0, 25);
            TextBox_SearchFilter.Size = new Size(TextBox_SearchFilter.Size.Width, 25);
            TextBox_SearchFilter.Multiline = false;

            TextBox_FileFilter.Multiline = true;
            TextBox_FileFilter.MinimumSize = new Size(0, 25);
            TextBox_FileFilter.Size = new Size(TextBox_FileFilter.Size.Width, 25);
            TextBox_FileFilter.Multiline = false;

            for (int i = 8; i <= 28; i++)
            {
                LV_FontSizes.Add(new Font(ListView_Titles.Font.FontFamily, i));
            }
            ListView_Titles.Font = LV_FontSizes[LV_Font_Index];

            ListView_Titles_Clear();
        }

        private void ListView_Titles_Clear()
        {
            ListView_Titles.Clear();
            ListView_Titles.Columns.Add("Time", -2, HorizontalAlignment.Left); // 80
            ListView_Titles.Columns.Add(" * ", -2, HorizontalAlignment.Right);    // 25
            ListView_Titles.Columns.Add("Title", -2, HorizontalAlignment.Left); // 400
        }

        private void ListView_Titles_AutoResizeColumns()
        {
            var resizeStyle = ListView_Titles.Items.Count > 0 ? ColumnHeaderAutoResizeStyle.ColumnContent : ColumnHeaderAutoResizeStyle.HeaderSize;
            for (int i = 0; i < ListView_Titles.Columns.Count; i++)
            {
                ListView_Titles.AutoResizeColumn(i, resizeStyle);
            }
        }

        Timer timer1 = new System.Windows.Forms.Timer();

        private async void RadioZiner_Load(object sender, EventArgs e)
        {
            if (mainDir == "")
            {
                mainDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "radioZiner");
                if (!Directory.Exists(mainDir))
                {
                    DialogResult dr = MessageBox.Show("Create new streaming directory in \"" + mainDir + "\"?",
                                          "No streaming directory", MessageBoxButtons.YesNo);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            Directory.CreateDirectory(mainDir);
                            break;
                        case DialogResult.No:
                            Application.Exit();
                            return;
                    }
                }
            }
            else if (!Directory.Exists(mainDir))
            {
                DialogResult dr = MessageBox.Show("Directory \"" + mainDir + "\" not found.",
                                      "No valid streaming directory", MessageBoxButtons.OK);
                Application.Exit();
                return;
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

            Panel_Files_Show();
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(Timer1_Tick);

            Player.Init(PictureBox_Player.Handle);
            Player.SetPropertyBool("deinterlace", true);

            btnPlayPause.Text = "⏸️";
            Button_Mute.Text = "🔈";
            Combo_ExportFileExtension.Text = ".mp4";

            videoRecorder = new VideoRecorder(recordingFolder);

            slider1 = new ColorSlider
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
                Height = 45,
                MouseWheelBarPartitions = 100,

                SmallChange = 1,
                LargeChange = 10
            };
            slider1.Scroll += Slider_Scroll;
            slider1.MouseWheel += Slider_MouseWheel;
            slider1.MouseUp += Slider_MouseUp;

            panel3.Controls.Add(slider1);



            slider2 = new ColorSlider
            {
                Dock = DockStyle.Top,
                Visible = true,
                ForeColor = Color.Blue,
                BackColor = Color.Black,
                ElapsedInnerColor = Color.Blue,
                ElapsedPenColorTop = Color.Blue,
                ElapsedPenColorBottom = Color.Blue,
                ThumbInnerColor = Color.Blue,
                ThumbOuterColor = Color.Blue,
                ThumbPenColor = Color.Blue,
                TickStyle = TickStyle.None,

                ThumbRoundRectSize = new Size(0, 0),
                ThumbSize = new Size(1, 10),
                Width = panel3.Width,
                Height = 45,
                MouseWheelBarPartitions = 100,

                SmallChange = 1,
                LargeChange = 10
            };
            slider2.Scroll += Slider_Scroll;
            slider2.MouseWheel += Slider_MouseWheel;
            slider2.MouseUp += Slider_MouseUp;

            panel9.Controls.Add(slider2);
            slider2.BringToFront();


            await Task.Delay(100);
            ReadChannelSets();
            await Task.Delay(100);
            ReadChannels();
            await Task.Delay(100);

            
            await ReadStreams();
            Set_SearchButton_Text();
        }

        private void Slider_MouseUp(object sender, EventArgs e)
        {
            ColorSlider slider = ((ColorSlider)sender);
            if (slider.SelectedLabelIndex >= 0)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    int index = -1;
                    double pos = double.Parse(slider.SelectedLabelPos, CultureInfo.InvariantCulture);
                    for (int i = ListView_Titles.Items.Count - 1; i >= 0; i--)
                    {
                        var a = GetTitle(i).Substring(0, 8).Split(':');
                        double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                        if (seconds <= pos)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index>=0)
                    {
                        Select_TitleToEdit(index);
                    }
                }
                else
                {
                    Player.CommandV("seek", slider.SelectedLabelPos, "absolute");
                    //UpdateTitlesPos(double.Parse(slider.SelectedLabelPos, CultureInfo.InvariantCulture));
                }
            }
        }

        private void Slider_MouseWheel(object sender, EventArgs e)
        {
            timer1.Stop();
            int seconds = Convert.ToInt32(((ColorSlider)sender).Value);
            Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
            timer1.Start();
        }

        private void Slider_Scroll(object sender, EventArgs e)
        {
            ColorSlider slider = ((ColorSlider)sender);
            if (!((Control.ModifierKeys & Keys.Control) == Keys.Control) && slider.SelectedLabelIndex < 0)
            {
                int seconds = Convert.ToInt32(slider.Value);
                Player.CommandV("seek", seconds.ToString(CultureInfo.InvariantCulture), "absolute");
            }
        }

        private void RadioZiner_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player?.Destroy();
            videoRecorder?.StopAllRecordings();
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
            if (recShortName!="" && recShortName == curRecChannelName)
            {
                ListView_Titles_Clear();
                Cur_LV_Titles_Item = null;
                foreach (var t in recorders[recShortName].icyPosTitles)
                {
                    AddTitleLine(t);
                }
                ListView_Titles_AutoResizeColumns();
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
                ListView_Titles_Clear();
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
            ListView_Titles_Clear();
            UpdateTitleList(curRecChannelName);

            TextBox_Url.Text = recorders[curRecChannelName].url;
            Button_Rec.Text = "Stop";

            setChannelName(curRecChannelName);

            Label_StartTime.Text = "00:00:00";
            Label_EndTime.Text = "00:00:00";
            TextBox_ExportFileName.Text = curRecChannelName;
            
            if (recorders[curRecChannelName].Player.GetPropertyString("video-codec") != "")
            {
                Combo_ExportFileExtension.Text = ".mp4";
            }
            else
            {
                Combo_ExportFileExtension.Text = ".mp3";
            }

            Panel_Files_Hide();
        }

        private void ListView_Titles_SaveToFile(string sTitlesFile)
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
                for (int i = 0; i < ListView_Titles.Items.Count; i++)
                {
                    var a = GetTitle(i).Split('|');
                    for (int j = 0; j < a.Count(); j++)
                    {
                        titles.Add(a[j].Trim());
                    }
                }
                File.WriteAllLines(fName, titles);
            }
        }

        private readonly Regex Rgx_TitlePos = new Regex(@"(?<hours>[0-9][0-9]):(?<minutes>[0-5][0-9]):(?<seconds>[0-5][0-9])((\s)|(.(?<milliseconds>[0-9]{3})))");
        private readonly Regex Rgx_TitleLabels = new Regex(@"(\S*@([0-9]|\.)+)\b"); //(\w+@([0-9]|\.)+)\b

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

        private int TitlePos2secsInt(string line)
        {
            Match m = Rgx_TitlePos.Match(line);
            if (m.Success)
            {
                int i = int.Parse(m.Groups["hours"].Value) * 3600;
                i += int.Parse(m.Groups["minutes"].Value) * 60;
                i += int.Parse(m.Groups["seconds"].Value);

                return i;
            }
            return -1;
        }

        private string LastLoadedTitleFile = "";
        private void ListView_Titles_LoadFromFile(string sTitlesFile="", bool refreshOnly = false)
        {
            if (sTitlesFile == "")
            {
                sTitlesFile = LastLoadedTitleFile;
            }
            LastLoadedTitleFile = sTitlesFile;
            if (File.Exists(sTitlesFile))
            {
                string fName = sTitlesFile;

                int pos = sTitlesFile.LastIndexOf('.');
                if (pos >= 0)
                {
                    fName = fName.Substring(0, pos);
                }
                fName += ".txt";

                ListView_Titles_Clear();

                if (File.Exists(fName))
                {
                    Cur_LV_Titles_Item = null;
                    foreach (var line in File.ReadAllLines(fName))
                    {
                        AddTitleLine(line);
                    }
                }
                ListView_Titles_AutoResizeColumns();

                if (!refreshOnly)
                {
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
        }

        private void SetSlider2 ()
        {
            slider2.Labels.Clear();
            int val = (int)slider1.Value;
            if (ListView_Titles.SelectedItems != null && ListView_Titles.SelectedItems.Count > 0)
            {
                string s = GetTitle(); ;
                var matches = Rgx_TitleLabels.Matches(s);

                foreach (var m in matches)
                {
                    slider2.Labels.Add(m.ToString().TrimStart('!'));
                }
                int min = TitlePos2secsInt(s);
                int max = (int)slider1.Maximum;
                int index = ListView_Titles.SelectedIndices[0] + 1;
                if (index < ListView_Titles.Items.Count)
                {
                    max = TitlePos2secsInt(GetTitle(index));
                }
                if (min>=0 && max>=0 && max>min && val>=min && val<=max)
                {
                    slider2.Maximum = max;
                    slider2.Minimum = min;
                    slider2.Value = val;
                    return;
                }
            }

            slider2.Maximum = slider1.Maximum;
            slider2.Minimum = 0;
            slider2.Value = val <= slider1.Maximum ? val : 0;
            
        }
        
        private void Select_TitleToEdit(int index = -1)
        {
            if (index < 0)
            {
                index = ListView_Titles.SelectedIndices != null ? ListView_Titles.SelectedIndices[0] : -1;
                if (index < 0)
                {
                    return;
                }
            }
 
            string sPos = TitlePos2TimeString(GetTitle(index));

            if (sPos != "")
            {
                var a = sPos.Split('.');
                Label_TitleTime.Text = a[0];
                if (a.Count() > 1)
                {
                    Label_TitleTimeFrac.Text = "." + a[1];
                }
                else
                {
                    Label_TitleTimeFrac.Text = ".000";
                }

                TextBox_TitleEdit.Text = GetTitle(index).Substring(sPos.Length).Trim();
                OldTitle = TextBox_TitleEdit.Text;
                OldPos = Label_TitleTime.Text + Label_TitleTimeFrac.Text;
                Label_StartTime.Text = Label_TitleTime.Text;
                Label_StartTimeFrac.Text = Label_TitleTimeFrac.Text;
                TextBox_ExportFileName.Text = TextBox_TitleEdit.Text;

                if (Player.GetPropertyString("video-codec") != "")
                {
                    Combo_ExportFileExtension.Text = ".mp4";
                }
                else
                {
                    Combo_ExportFileExtension.Text = ".mp3";
                }

                if ((index+1) < ListView_Titles.Items.Count)
                {
                    sPos = TitlePos2TimeString(GetTitle(index));
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

 
        private void ListView_Titles_Click(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (ListView_Titles.SelectedItems != null && ListView_Titles.SelectedItems.Count > 0)
                    {
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {
                            Select_TitleToEdit();
                        }
                        else
                        {
                            Player.CommandV("seek", TitlePos2secs(GetTitle()), "absolute"); // seconds.ToString(CultureInfo.InvariantCulture) + ".260"
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

        private int CountMatchingTitlesInFile (string strFile, string strSearch)
        {
            int iFound = 0;

            int pos = strFile.LastIndexOf('.');
            if (pos >= 0)
            {
                strFile = strFile.Substring(0, pos);
            }
            strFile += ".txt";

            if (File.Exists(strFile))
            {
                foreach (var line in File.ReadAllLines(strFile))
                {
                    if (line.ToLower().Contains(strSearch.ToLower()))
                    {
                        iFound++;
                    }
                }
            }
            else
            {
                iFound = -1;
            }

            return iFound;
        }

        private string LastDirLoaded = "";
        private void ListBox_Files_LoadFromDir(string val = "", string source = "files")
        {
            if (val == "")
            {
                val = LastDirLoaded;
            }
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
                    LastDirLoaded = val;
                    break;

            }
            listView.Clear();
            listView.Columns.Add("#", 35, HorizontalAlignment.Right);
            listView.Columns.Add("Filename", 300, HorizontalAlignment.Left);
            listView.Tag = val;


            var allowedExtensions = new[] { ".mp4", ".mp3", ".aac", ".ts", ".avi", ".mpg", ".mpeg",  ".mkv", ".vob", ".wmv", ".flv"};
            foreach (var sFile in Directory.EnumerateFiles(val, "*.*",
                System.IO.SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                .OrderByDescending(file => new FileInfo(file).CreationTime)
                )
            {
                int matches = CountMatchingTitlesInFile(sFile, TextBox_FileFilter.Text);
                if (TextBox_FileFilter.Text == "" ||
                    matches > 0 ||
                    sFile.ToLower().Contains(TextBox_FileFilter.Text.ToLower()))
                {
                    ListViewItem lvItem = new ListViewItem(matches >= 0 ? matches.ToString() : "", 0);
                    lvItem.SubItems.Add(sFile.Substring(val.Length + (val.EndsWith("\\") ? 0 : 1)));
                    lvItem.Tag = new M3u.TvgChannel();
                    listView.Items.Add(lvItem);
                }
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
            ListView_Files.Columns.Add("#", 35, HorizontalAlignment.Right);
            ListView_Files.Columns.Add("Filename", 300, HorizontalAlignment.Left);
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

                    ListViewItem lvItem = new ListViewItem("", 0);
                    lvItem.SubItems.Add(channelKey);
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
                ListView_Titles_Clear();
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
                    ListView_Titles_Clear();

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

        private void UpdateSlider (ColorSlider slider, double min, double max, double val)
        {
            slider.Minimum = 0;
            slider.Maximum = (int)max > 0 ? (int)max : 7200;
            slider.Value = (int)val > 0 && (int)val <= max ? (int)val : 0;

            SetSlider2();
        }
        
        private void UpdateTitlesPos (double pos)
        {
            for (int i = ListView_Titles.Items.Count - 1; i >= 0; i--)
            {
                var a = GetTitle(i).Substring(0, 8).Split(':');
                double seconds = int.Parse(a[0]) * 3600 + int.Parse(a[1]) * 60 + int.Parse(a[2]);
                if (seconds <= pos)
                {
                    if (!ListView_Titles.Items[i].Selected)
                    {
                        ListView_Titles.Items[i].Selected = true;
                        ListView_Titles.Items[i].EnsureVisible();
                    }
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
                string sPosTitle = sPos + " " + title; //" - "
                icyPosTitles.Add(sPosTitle);

                icyPosTitles.Sort();

                ListView_Titles_Clear();
                Cur_LV_Titles_Item = null;
                foreach (var line in icyPosTitles)
                {
                    AddTitleLine(line);
                }
                changed = true;
                ListView_Titles_AutoResizeColumns();
            }

            if (icyTitles.Count() > 1)
            {
                hasIcyTitles = true;
            }

            return (changed);
        }

        private void UpdateMainLabels ()
        {
            slider1.Labels.Clear();
            for (int i = 0; i < ListView_Titles.Items.Count; i++)
            {
                string line = GetTitle(i);
                string marker = GetMarker(i);
                string pos = TitlePos2secs(line);
                int leadLen = TitlePos2TimeString(line).Length;
                string title = leadLen < line.Length ? line.Substring(leadLen).Trim() : "";
                string firstWord = title.Trim().Split(' ')[0];
                //string[] endChars = { "^", "°" }; // , "*"
                if (firstWord.StartsWith("!")) // endChars.Any(x => firstWord.EndsWith(x)) || 
                {
                    slider1.Labels.Add(firstWord.TrimStart('!') + "@" + pos);
                }
                else if (!firstWord.StartsWith("."))
                {
                    slider1.Labels.Add(".@" + pos);
                }

                /*
                string tag = "";
                if (leadLen + 3 < line.Length)
                {
                    tag = line.Substring(leadLen, 3);
                }
                if (tag == " - ")
                {
                    slider1.Labels.Add(".@" + pos);
                }
                else
                {
                    if (leadLen < line.Length)
                    {
                        slider1.Labels.Add(line.Substring(leadLen).Trim().Split(' ')[0] + @"@" + pos);
                    }
                }
                */

                if (marker!="")
                {
                    slider1.Labels.Add(marker+ "°" + @"@" + pos);
                }

                var matches = Rgx_TitleLabels.Matches(line);

                foreach (var m in matches)
                {
                    string str = m.ToString();
                    if (str.StartsWith("!"))
                    {
                        slider1.Labels.Add(str.TrimStart('!'));
                    }
                }

            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (Player.GetPropertyString("video-codec") != "")
            {
                ListView_Titles.Dock = DockStyle.Top;
                ListView_Titles.Height = 250;
                PictureBox_Player.Show();
                PictureBox_Player.BringToFront();
            }
            else if (Player.GetPropertyString("audio-codec") != "")
            {
                ListView_Titles.Dock = DockStyle.Fill;
                PictureBox_Player.Hide();
                ListView_Titles_Show();
            }

            if (curRecChannelName=="")
            {
                double curPos = Player.GetPropertyDouble("time-pos");
                double recLen = Player.GetPropertyDouble("time-remaining") + curPos;

                Label_PlayerPos.Text = GetFormatedPlayerPos(curPos);
                Label_PlayerPosFrac.Text = GetPlayerPosFractionalPart(curPos);
                Label_RecLen.Text = GetFormatedPlayerPos(recLen);

                UpdateSlider(slider1, 0, recLen, curPos);
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

                UpdateSlider(slider1, 0, recLen, curPos);
                UpdateTitlesPos(curPos);

                Label_PlayerPos.Text = GetFormatedPlayerPos(curPos);
                Label_PlayerPosFrac.Text = GetPlayerPosFractionalPart(curPos);
                Label_RecLen.Text = GetFormatedPlayerPos(recLen);

                SeekToLastRecorderPos();
            }
            label1.Text = GetFormatedPlayerPos((double)(slider2.Maximum - slider2.Value)).TrimStart('0').TrimStart(':').TrimStart('0').TrimStart(':');
            UpdateMainLabels();
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

        public void ListView_MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (e.Location.X > 100)
            {
                return;
            }
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.Delta > 0)
            {
                if (LV_Font_Index + 1 < LV_FontSizes.Count)
                {
                    ((ListView)sender).Font = LV_FontSizes[++LV_Font_Index];
                }
            }
            else
            {
                if (LV_Font_Index > 0)
                {
                    ((ListView)sender).Font = LV_FontSizes[--LV_Font_Index];
                }
            }
            var resizeStyle = ((ListView)sender).Items.Count > 0 ? ColumnHeaderAutoResizeStyle.ColumnContent : ColumnHeaderAutoResizeStyle.HeaderSize;
            for (int i = 0; i < ((ListView)sender).Columns.Count; i++)
            {
                ((ListView)sender).AutoResizeColumn(i, resizeStyle);
            }

        }

        public void ListView_Titles_MouseWheelHandler(object sender, MouseEventArgs e)
        {
            if (e.Location.X > ListView_Titles.Width - 100 && !(sender is PictureBox))
            {
                return;
            }
            ((HandledMouseEventArgs)e).Handled = true;
            if (e.Location.X > 100 && (Control.ModifierKeys & Keys.Control) != Keys.Control)
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
                if (LV_Font_Index + 1 < LV_FontSizes.Count)
                {
                    ListView_Titles.Font = LV_FontSizes[++LV_Font_Index];
                    ListView_Titles_AutoResizeColumns();
                }
            }
            else
            {
                if (LV_Font_Index > 0)
                {
                    ListView_Titles.Font = LV_FontSizes[--LV_Font_Index];
                    ListView_Titles_AutoResizeColumns();
                }
            }
        }

        public void PictureBox_MouseWheelHandler(object sender, MouseEventArgs e)
        {
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
                panel9.Hide();
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
                panel9.Show();
            }
        }

        private void ListView_Titles_DoubleClick(object sender, EventArgs e)
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

        private void ListView_Titles_Hide()
        {
            PictureBox_Player.Show();
            ListView_Titles.Hide();
            Label_Toggle_ListView_Titles.Text = "◁";
        }

        private void ListView_Titles_Show()
        {
            ListView_Titles.Show();
            Label_Toggle_ListView_Titles.Text = "▼";
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
                    Panel_FileFilter.Hide();

                    ListView_Streams.Show();
                    ListView_Recordings.Hide();
                    ListView_Exports.Hide();
                    ListView_Files.Hide();
                    break;
                case "recordings":
                    Panel_StreamFilter.Hide();
                    Panel_FileFilter.Show();

                    ListView_Streams.Hide();
                    ListView_Recordings.Show();
                    ListView_Recordings.BringToFront();
                    ListView_Exports.Hide();
                    ListView_Files.Hide();
                    if (ListView_Recordings.Items.Count == 0 || Panel_Files.Visible && ((Button)sender).ForeColor == Color.Orange)
                    {
                        ListBox_Files_LoadFromDir(recordingFolder, "recordings");
                    }
                    break;
                case "exports":
                    Panel_StreamFilter.Hide();
                    Panel_FileFilter.Show();

                    ListView_Streams.Hide();
                    ListView_Recordings.Hide();
                    ListView_Exports.Show();
                    ListView_Exports.BringToFront();
                    ListView_Files.Hide();
                    if (ListView_Exports.Items.Count == 0 || Panel_Files.Visible && ((Button)sender).ForeColor == Color.Orange)
                    {
                        ListBox_Files_LoadFromDir(exportFolder, "exports");
                    }
                    break;
                case "files":
                    Panel_StreamFilter.Hide();
                    Panel_FileFilter.Show();

                    ListView_Streams.Hide();
                    ListView_Recordings.Hide();
                    ListView_Exports.Hide();
                    ListView_Files.Show();
                    ListView_Files.BringToFront();
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

        private void Toggle_ListView_Titles()
        {
            if (ListView_Titles.Visible)
            {
                ListView_Titles_Hide();
            }
            else
            {
                ListView_Titles_Show();
            }
        }

        private void Label_Toggle_ListBox_Files_Click(object sender, EventArgs e)
        {
            Toggle_ListBox_Files();
        }

        private void Label_Toggle_ListView_Titles_Click(object sender, EventArgs e)
        {
            Toggle_ListView_Titles();
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
            string selectedItem = listView.SelectedItems[0].SubItems[1].Text;//item.id.ToString();
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
            ListView_Titles_Clear();

            curRecChannelName = "";

            Player.CommandV("loadfile", TextBox_Url.Text, "replace");
            Button_Rec.Text = "Rec";

            ListView_Titles_LoadFromFile(sUrl);
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




        private ListViewItem Cur_LV_Titles_Item = null;
        private void AddTitleLine(string line)
        {
            Match m = Rgx_TitlePos.Match(line);
            if (m.Success)
            {
                string marker = "     ";
                if (TextBox_FileFilter.Text != "" && line.ToLower().Contains(TextBox_FileFilter.Text.ToLower()))
                {
                    marker = "⚫";
                }
                Cur_LV_Titles_Item = new ListViewItem(m.Value, 0);
                Cur_LV_Titles_Item.SubItems.Add(marker);
                //Cur_LV_Titles_Item.SubItems.Add(line.Substring(m.Length).Trim());
                var a = line.Substring(m.Length).Trim().Split('|');
                for (int j = 0; j < a.Count(); j++)
                {
                    int curIndex = Cur_LV_Titles_Item.SubItems.Count;
                    int curCols = ListView_Titles.Columns.Count;
                    int maxIndex = 2;
                    if (curIndex > maxIndex && curIndex >= curCols)
                    {
                        ListView_Titles.Columns.Add("*", -2, HorizontalAlignment.Left);
                    }
                    Cur_LV_Titles_Item.SubItems.Add(a[j].Trim());
                    //titles.Add(a[j].Trim());
                }

                ListView_Titles.Items.Add(Cur_LV_Titles_Item);
            }
            else if (Cur_LV_Titles_Item != null)
            {
                int curIndex = Cur_LV_Titles_Item.SubItems.Count;
                int curCols = ListView_Titles.Columns.Count;
                int maxIndex = 2;
                if (curIndex > maxIndex && curIndex >= curCols)
                {
                    ListView_Titles.Columns.Add("*", -2, HorizontalAlignment.Left);
                }

                Cur_LV_Titles_Item.SubItems.Add(line);
            }
            //ListView_Titles_AutoResizeColumns();
        }

        public void AddTitlePos(string pos, string title = "")
        {
            timer1.Enabled = false;

            pos = pos.Trim();
            title = title.Trim();

            string marker = "";
            if (TextBox_FileFilter.Text != "" && title.ToLower().Contains(TextBox_FileFilter.Text.ToLower()))
            {
               marker = "⚫";
            }

            ListViewItem lvItem = new ListViewItem(pos, 0);
            lvItem.SubItems.Add(marker);
            var a = title.Split('|');
            for (int j = 0; j < a.Count(); j++)
            {
                lvItem.SubItems.Add(a[j].Trim());
            }
            //lvItem.SubItems.Add(title);

            ListView_Titles.Items.Add(lvItem);

            ListView_Titles_SaveToFile(TextBox_Url.Text);
            ListView_Titles_LoadFromFile(TextBox_Url.Text, true);
            //RefreshTitles();
            //ListView_Titles_AutoResizeColumns();
            timer1.Enabled = true;
        }

        private string GetMarker (int index = -1)
        {
            string title = "";
            if (index < 0)
            {
                title = ListView_Titles.SelectedItems[0].SubItems[1].Text;
            }
            else
            {
                title = ListView_Titles.Items[index].SubItems[1].Text;
            }
            return title;

        }

        private string GetTitle(int index = -1)
        {
            if (index < 0)
            {
                index = ListView_Titles.SelectedIndices[0];
            }
            string title = ListView_Titles.Items[index].SubItems[0].Text.Trim();
            title += " " + ListView_Titles.Items[index].SubItems[2].Text.Trim();
            for (int i = 3; i < ListView_Titles.Items[index].SubItems.Count; i++)
            {
                if (ListView_Titles.Items[index]?.SubItems[i].Text.Trim() != "")
                {
                    title += " | " + ListView_Titles.Items[index].SubItems[i].Text.Trim();
                }
            }
            return title;
        }

        public void DeleteTitleAtPos(string pos, string title)
        {
            timer1.Enabled = false;

            string s = pos.Trim() + " " + title.Trim();
            
            for (int i=0; i < ListView_Titles.Items.Count; i++)
            {
                if (GetTitle(i) == s)
                {
                    ListView_Titles.Items.RemoveAt(i);
                    break;
                }
            }
            ListView_Titles_SaveToFile(TextBox_Url.Text);
            ListView_Titles_AutoResizeColumns();
            timer1.Enabled = true;
        }

        private void AddTitle(string title)
        {
            //string newTitle = " " + title.Trim();
            string newTitle = title.Trim();
            if (curRecChannelName != "" && recorders.ContainsKey(curRecChannelName))
            {
                recorders[curRecChannelName].AddTitlePos(Label_TitleTime.Text + Label_TitleTimeFrac.Text, newTitle);
            }
            else
            {
                AddTitlePos(Label_TitleTime.Text + Label_TitleTimeFrac.Text, newTitle);
            }
            OldTitle = newTitle;
            OldPos = Label_TitleTime.Text + Label_TitleTimeFrac.Text;
        }

        private void Button_TitleAdd_Click(object sender, EventArgs e)
        {
            Label_TitleTime.Text = Label_PlayerPos.Text;
            Label_TitleTimeFrac.Text = Label_PlayerPosFrac.Text;
            AddTitle(TextBox_TitleEdit.Text);
        }

        private void DeleteTitle (string title, string pos = "")
        {
            if (pos == "")
            {
                pos = Label_TitleTime.Text + Label_TitleTimeFrac.Text;
            }

            if (curRecChannelName != "" && recorders.ContainsKey(curRecChannelName))
            {
                recorders[curRecChannelName].DeleteTitleAtPos(pos, title);
                recorders[curRecChannelName].DeleteTitleAtPos(pos.Split('.')[0], title);
            }
            else
            {
                DeleteTitleAtPos(pos, title);
                DeleteTitleAtPos(pos.Split('.')[0], title);
            }
        }

        private void Button_Title_Delete_Click(object sender, EventArgs e)
        {
            DeleteTitle(TextBox_TitleEdit.Text);
        }

        /*
        private string InsertStringIntoTitle(string s, string title)
        {
            Match m = Rgx_TitlePos.Match(title);
            if (m.Success)
            {
                if (m.Groups["milliseconds"].Value != "")
                {
                }
                int pos = m.Groups["milliseconds"].Value == "" ? 8 : 12;
                title = title.Substring(0, pos) + s + title.Substring(pos, title.Length - pos);
            }
            return title;
        }

        private string MarkTitleIfSearchStringMatches (string s)
        {
            if (TextBox_FileFilter.Text != "" && s.ToLower().Contains(TextBox_FileFilter.Text.ToLower()))
            {
                s = InsertStringIntoTitle(" ⚫", s);
            }
            return s;
        }
        */


        private void RefreshTitles()
        {
            if (CurSource == "live")
            {
                ListView_Titles_Clear();
                Cur_LV_Titles_Item = null;
                foreach (var line in icyPosTitles)
                {
                    AddTitleLine(line);
                }
                ListView_Titles_AutoResizeColumns();
            }
        }

        private void UpdateFileSearch()
        {
            ListBox_Files_LoadFromDir(recordingFolder, "recordings");
            ListBox_Files_LoadFromDir(exportFolder, "exports");
            ListBox_Files_LoadFromDir();
            ListView_Titles_LoadFromFile();
            RefreshTitles();
            UpdateTitleList(curRecChannelName);
        }

        private void TextBox_FileFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                UpdateFileSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void TextBox_FileFilter_TextChanged(object sender, EventArgs e)
        {
            if (TextBox_FileFilter.Text.Length == 0 || TextBox_FileFilter.Text.Length > 1)
            {
                UpdateFileSearch();
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

        private void TextBox_TitleEdit_Click(object sender, EventArgs e)
        {
        }

        private string OldTitle = "";
        private string OldPos = "";
        private void TextBox_TitleEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (OldTitle != "")
                {
                    DeleteTitle(OldTitle, OldPos);
                }

                AddTitle(TextBox_TitleEdit.Text);
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                TextBox_TitleEdit.Text = OldTitle;
                var a = OldPos.Split('.');
                Label_TitleTime.Text = a[0];
                Label_TitleTimeFrac.Text = a.Count() > 1 ? "." + a[1] : "";
            }
        }

        private void TextBox_TitleEdit_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                decimal pos = 0;
                if (!Label_PlayerPos.Text.Contains("-"))
                {
                    pos = decimal.Parse(TitlePos2secs(Label_PlayerPos.Text + Label_PlayerPosFrac.Text), CultureInfo.InvariantCulture); // - slider2.Minimum
                }
                var insertText = "@" + pos.ToString(CultureInfo.InvariantCulture);
                var selectionIndex = TextBox_TitleEdit.SelectionStart;
                TextBox_TitleEdit.Text = TextBox_TitleEdit.Text.Insert(selectionIndex, insertText);
                TextBox_TitleEdit.SelectionStart = selectionIndex + insertText.Length;
            }
        }
   }
}
