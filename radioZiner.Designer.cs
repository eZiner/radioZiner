namespace radioZiner
{
    partial class radioZiner
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextBox_Url = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Button_Rec = new System.Windows.Forms.Button();
            this.Combo_ShortName = new System.Windows.Forms.ComboBox();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.ListBox_Titles = new System.Windows.Forms.ListBox();
            this.PictureBox_Player = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.LabelEnterGroupName = new System.Windows.Forms.Label();
            this.TextBox_ChannelSet = new System.Windows.Forms.TextBox();
            this.LabelEnterChannelName = new System.Windows.Forms.Label();
            this.TextBox_ShortName = new System.Windows.Forms.TextBox();
            this.Button_Mute = new System.Windows.Forms.Button();
            this.Combo_ChannelSet = new System.Windows.Forms.ComboBox();
            this.lblRecordLength = new System.Windows.Forms.Label();
            this.lblPlayerPos = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Player)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBox_Url
            // 
            this.TextBox_Url.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox_Url.Location = new System.Drawing.Point(6, 18);
            this.TextBox_Url.Name = "TextBox_Url";
            this.TextBox_Url.Size = new System.Drawing.Size(68, 20);
            this.TextBox_Url.TabIndex = 0;
            this.TextBox_Url.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "URL:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Button_Rec
            // 
            this.Button_Rec.AutoSize = true;
            this.Button_Rec.BackColor = System.Drawing.Color.Maroon;
            this.Button_Rec.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Rec.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Rec.Location = new System.Drawing.Point(594, 6);
            this.Button_Rec.Name = "Button_Rec";
            this.Button_Rec.Size = new System.Drawing.Size(69, 28);
            this.Button_Rec.TabIndex = 5;
            this.Button_Rec.Text = "Rec";
            this.Button_Rec.UseVisualStyleBackColor = false;
            this.Button_Rec.Click += new System.EventHandler(this.Button_Rec_Click);
            // 
            // Combo_ShortName
            // 
            this.Combo_ShortName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_ShortName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Combo_ShortName.FormattingEnabled = true;
            this.Combo_ShortName.Location = new System.Drawing.Point(291, 6);
            this.Combo_ShortName.Name = "Combo_ShortName";
            this.Combo_ShortName.Size = new System.Drawing.Size(286, 28);
            this.Combo_ShortName.TabIndex = 4;
            this.Combo_ShortName.SelectedIndexChanged += new System.EventHandler(this.Combo_ShortName_SelectedIndexChanged);
            this.Combo_ShortName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Combo_ShortName_KeyDown);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.BackColor = System.Drawing.Color.Black;
            this.btnPlayPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayPause.Location = new System.Drawing.Point(0, 0);
            this.btnPlayPause.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(38, 38);
            this.btnPlayPause.TabIndex = 1;
            this.btnPlayPause.UseVisualStyleBackColor = false;
            this.btnPlayPause.Click += new System.EventHandler(this.Button_PlayPause_Click);
            // 
            // ListBox_Titles
            // 
            this.ListBox_Titles.BackColor = System.Drawing.Color.Black;
            this.ListBox_Titles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListBox_Titles.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBox_Titles.ForeColor = System.Drawing.Color.White;
            this.ListBox_Titles.FormattingEnabled = true;
            this.ListBox_Titles.ItemHeight = 29;
            this.ListBox_Titles.Location = new System.Drawing.Point(0, 45);
            this.ListBox_Titles.Name = "ListBox_Titles";
            this.ListBox_Titles.Size = new System.Drawing.Size(507, 232);
            this.ListBox_Titles.TabIndex = 7;
            this.ListBox_Titles.Click += new System.EventHandler(this.ListBox_Titles_Click);
            this.ListBox_Titles.SelectedIndexChanged += new System.EventHandler(this.ListBox_Titles_SelectedIndexChanged);
            this.ListBox_Titles.SelectedValueChanged += new System.EventHandler(this.ListBox_Titles_SelectedValueChanged);
            this.ListBox_Titles.DoubleClick += new System.EventHandler(this.ListBox_Titles_DoubleClick);
            // 
            // PictureBox_Player
            // 
            this.PictureBox_Player.Location = new System.Drawing.Point(592, 156);
            this.PictureBox_Player.Name = "PictureBox_Player";
            this.PictureBox_Player.Size = new System.Drawing.Size(100, 50);
            this.PictureBox_Player.TabIndex = 2;
            this.PictureBox_Player.TabStop = false;
            this.PictureBox_Player.DoubleClick += new System.EventHandler(this.ListBox_Titles_DoubleClick);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.trackBar1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 664);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1008, 65);
            this.panel3.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DimGray;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.lblRecordLength);
            this.panel2.Controls.Add(this.lblPlayerPos);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1008, 41);
            this.panel2.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.LabelEnterGroupName);
            this.panel1.Controls.Add(this.TextBox_ChannelSet);
            this.panel1.Controls.Add(this.LabelEnterChannelName);
            this.panel1.Controls.Add(this.TextBox_ShortName);
            this.panel1.Controls.Add(this.Button_Mute);
            this.panel1.Controls.Add(this.btnPlayPause);
            this.panel1.Controls.Add(this.Button_Rec);
            this.panel1.Controls.Add(this.Combo_ShortName);
            this.panel1.Controls.Add(this.Combo_ChannelSet);
            this.panel1.Location = new System.Drawing.Point(175, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(667, 38);
            this.panel1.TabIndex = 7;
            // 
            // LabelEnterGroupName
            // 
            this.LabelEnterGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelEnterGroupName.Location = new System.Drawing.Point(89, 0);
            this.LabelEnterGroupName.Name = "LabelEnterGroupName";
            this.LabelEnterGroupName.Size = new System.Drawing.Size(196, 33);
            this.LabelEnterGroupName.TabIndex = 11;
            this.LabelEnterGroupName.Text = "New group name ...";
            this.LabelEnterGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LabelEnterGroupName.Visible = false;
            this.LabelEnterGroupName.Click += new System.EventHandler(this.LabelEnterGroupName_Click);
            // 
            // TextBox_ChannelSet
            // 
            this.TextBox_ChannelSet.Location = new System.Drawing.Point(93, 5);
            this.TextBox_ChannelSet.Name = "TextBox_ChannelSet";
            this.TextBox_ChannelSet.Size = new System.Drawing.Size(192, 29);
            this.TextBox_ChannelSet.TabIndex = 12;
            this.TextBox_ChannelSet.Visible = false;
            this.TextBox_ChannelSet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ChannelSet_KeyDown);
            // 
            // LabelEnterChannelName
            // 
            this.LabelEnterChannelName.Location = new System.Drawing.Point(287, 3);
            this.LabelEnterChannelName.Name = "LabelEnterChannelName";
            this.LabelEnterChannelName.Size = new System.Drawing.Size(290, 33);
            this.LabelEnterChannelName.TabIndex = 10;
            this.LabelEnterChannelName.Text = "Enter channel name here ...";
            this.LabelEnterChannelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LabelEnterChannelName.Visible = false;
            this.LabelEnterChannelName.Click += new System.EventHandler(this.LabelEnterChannelName_Click);
            // 
            // TextBox_ShortName
            // 
            this.TextBox_ShortName.Location = new System.Drawing.Point(291, 6);
            this.TextBox_ShortName.Name = "TextBox_ShortName";
            this.TextBox_ShortName.Size = new System.Drawing.Size(286, 29);
            this.TextBox_ShortName.TabIndex = 12;
            this.TextBox_ShortName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ShortName_KeyDown);
            // 
            // Button_Mute
            // 
            this.Button_Mute.BackColor = System.Drawing.Color.Black;
            this.Button_Mute.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Mute.Location = new System.Drawing.Point(38, 0);
            this.Button_Mute.Margin = new System.Windows.Forms.Padding(0);
            this.Button_Mute.Name = "Button_Mute";
            this.Button_Mute.Size = new System.Drawing.Size(38, 38);
            this.Button_Mute.TabIndex = 2;
            this.Button_Mute.UseVisualStyleBackColor = false;
            this.Button_Mute.Click += new System.EventHandler(this.Button_Mute_Click);
            // 
            // Combo_ChannelSet
            // 
            this.Combo_ChannelSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_ChannelSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Combo_ChannelSet.FormattingEnabled = true;
            this.Combo_ChannelSet.Location = new System.Drawing.Point(93, 6);
            this.Combo_ChannelSet.Name = "Combo_ChannelSet";
            this.Combo_ChannelSet.Size = new System.Drawing.Size(192, 28);
            this.Combo_ChannelSet.TabIndex = 3;
            this.Combo_ChannelSet.SelectedIndexChanged += new System.EventHandler(this.Combo_ChannelSet_SelectedIndexChanged);
            this.Combo_ChannelSet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Combo_ChannelSet_KeyDown);
            // 
            // lblRecordLength
            // 
            this.lblRecordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecordLength.AutoSize = true;
            this.lblRecordLength.Location = new System.Drawing.Point(925, 8);
            this.lblRecordLength.Name = "lblRecordLength";
            this.lblRecordLength.Size = new System.Drawing.Size(80, 24);
            this.lblRecordLength.TabIndex = 0;
            this.lblRecordLength.Text = "00:00:00";
            this.lblRecordLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.AutoSize = true;
            this.lblPlayerPos.Location = new System.Drawing.Point(3, 8);
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.Size = new System.Drawing.Size(80, 24);
            this.lblPlayerPos.TabIndex = 0;
            this.lblPlayerPos.Text = "00:00:00";
            this.lblPlayerPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel4.Controls.Add(this.TextBox_Url);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(845, -4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(83, 38);
            this.panel4.TabIndex = 9;
            this.panel4.Visible = false;
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.Color.DimGray;
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.Location = new System.Drawing.Point(0, 0);
            this.trackBar1.Maximum = 7200;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1008, 65);
            this.trackBar1.TabIndex = 6;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.TrackBar1_Scroll);
            // 
            // flowPanel
            // 
            this.flowPanel.AutoSize = true;
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowPanel.Location = new System.Drawing.Point(0, 664);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(1008, 0);
            this.flowPanel.TabIndex = 2;
            // 
            // radioZiner
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.PictureBox_Player);
            this.Controls.Add(this.ListBox_Titles);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.panel3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "radioZiner";
            this.Text = "radioZiner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RadioZiner_FormClosing);
            this.Load += new System.EventHandler(this.RadioZiner_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RadioZiner_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RadioZiner_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Player)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.ListBox ListBox_Titles;
        private System.Windows.Forms.TextBox TextBox_Url;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Button_Rec;
        private System.Windows.Forms.ComboBox Combo_ShortName;
        private System.Windows.Forms.PictureBox PictureBox_Player;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Label lblPlayerPos;
        private System.Windows.Forms.Label lblRecordLength;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox Combo_ChannelSet;
        private System.Windows.Forms.Button Button_Mute;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label LabelEnterChannelName;
        private System.Windows.Forms.Label LabelEnterGroupName;
        private System.Windows.Forms.TextBox TextBox_ShortName;
        private System.Windows.Forms.TextBox TextBox_ChannelSet;
    }
}

