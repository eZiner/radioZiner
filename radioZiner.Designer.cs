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
            this.panel1 = new System.Windows.Forms.Panel();
            this.LabelEnterGroupName = new System.Windows.Forms.Label();
            this.TextBox_ChannelSet = new System.Windows.Forms.TextBox();
            this.Combo_ChannelSet = new System.Windows.Forms.ComboBox();
            this.LabelEnterChannelName = new System.Windows.Forms.Label();
            this.TextBox_ShortName = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Button_Toggle_Channels_Export = new System.Windows.Forms.Button();
            this.lblRecordLength = new System.Windows.Forms.Label();
            this.lblPlayerPos = new System.Windows.Forms.Label();
            this.Button_Mute = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.Label_EndTime = new System.Windows.Forms.Label();
            this.TextBox_ExportFileName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Label_StartTime = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Button_ExportSave = new System.Windows.Forms.Button();
            this.Combo_ExportFileExtension = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Player)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextBox_Url
            // 
            this.TextBox_Url.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox_Url.Location = new System.Drawing.Point(3, 18);
            this.TextBox_Url.Name = "TextBox_Url";
            this.TextBox_Url.Size = new System.Drawing.Size(68, 20);
            this.TextBox_Url.TabIndex = 0;
            this.TextBox_Url.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
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
            this.Button_Rec.Location = new System.Drawing.Point(492, 4);
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
            this.Combo_ShortName.Location = new System.Drawing.Point(200, 4);
            this.Combo_ShortName.Name = "Combo_ShortName";
            this.Combo_ShortName.Size = new System.Drawing.Size(286, 28);
            this.Combo_ShortName.TabIndex = 4;
            this.Combo_ShortName.SelectedIndexChanged += new System.EventHandler(this.Combo_ShortName_SelectedIndexChanged);
            this.Combo_ShortName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Combo_ShortName_KeyDown);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.BackColor = System.Drawing.Color.Black;
            this.btnPlayPause.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnPlayPause.FlatAppearance.BorderSize = 0;
            this.btnPlayPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayPause.Location = new System.Drawing.Point(0, 0);
            this.btnPlayPause.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(38, 40);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.TabStop = false;
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
            this.ListBox_Titles.Location = new System.Drawing.Point(12, 25);
            this.ListBox_Titles.Name = "ListBox_Titles";
            this.ListBox_Titles.Size = new System.Drawing.Size(473, 174);
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
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 664);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1008, 65);
            this.panel3.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel1.Controls.Add(this.LabelEnterGroupName);
            this.panel1.Controls.Add(this.TextBox_ChannelSet);
            this.panel1.Controls.Add(this.Combo_ChannelSet);
            this.panel1.Controls.Add(this.LabelEnterChannelName);
            this.panel1.Controls.Add(this.TextBox_ShortName);
            this.panel1.Controls.Add(this.Combo_ShortName);
            this.panel1.Controls.Add(this.Button_Rec);
            this.panel1.Location = new System.Drawing.Point(263, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(572, 38);
            this.panel1.TabIndex = 7;
            // 
            // LabelEnterGroupName
            // 
            this.LabelEnterGroupName.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelEnterGroupName.Location = new System.Drawing.Point(3, 0);
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
            this.TextBox_ChannelSet.Location = new System.Drawing.Point(3, 4);
            this.TextBox_ChannelSet.Name = "TextBox_ChannelSet";
            this.TextBox_ChannelSet.Size = new System.Drawing.Size(192, 29);
            this.TextBox_ChannelSet.TabIndex = 12;
            this.TextBox_ChannelSet.Visible = false;
            this.TextBox_ChannelSet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ChannelSet_KeyDown);
            // 
            // Combo_ChannelSet
            // 
            this.Combo_ChannelSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_ChannelSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Combo_ChannelSet.FormattingEnabled = true;
            this.Combo_ChannelSet.Location = new System.Drawing.Point(3, 4);
            this.Combo_ChannelSet.Name = "Combo_ChannelSet";
            this.Combo_ChannelSet.Size = new System.Drawing.Size(192, 28);
            this.Combo_ChannelSet.TabIndex = 3;
            this.Combo_ChannelSet.SelectedIndexChanged += new System.EventHandler(this.Combo_ChannelSet_SelectedIndexChanged);
            this.Combo_ChannelSet.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Combo_ChannelSet_KeyDown);
            // 
            // LabelEnterChannelName
            // 
            this.LabelEnterChannelName.Location = new System.Drawing.Point(200, 0);
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
            this.TextBox_ShortName.Location = new System.Drawing.Point(200, 4);
            this.TextBox_ShortName.Name = "TextBox_ShortName";
            this.TextBox_ShortName.Size = new System.Drawing.Size(286, 29);
            this.TextBox_ShortName.TabIndex = 12;
            this.TextBox_ShortName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_ShortName_KeyDown);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Black;
            this.panel2.Controls.Add(this.Button_Toggle_Channels_Export);
            this.panel2.Controls.Add(this.lblRecordLength);
            this.panel2.Controls.Add(this.lblPlayerPos);
            this.panel2.Controls.Add(this.Button_Mute);
            this.panel2.Controls.Add(this.btnPlayPause);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1008, 40);
            this.panel2.TabIndex = 6;
            // 
            // Button_Toggle_Channels_Export
            // 
            this.Button_Toggle_Channels_Export.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Button_Toggle_Channels_Export.AutoSize = true;
            this.Button_Toggle_Channels_Export.BackColor = System.Drawing.Color.DarkGreen;
            this.Button_Toggle_Channels_Export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_Toggle_Channels_Export.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Toggle_Channels_Export.Location = new System.Drawing.Point(178, 3);
            this.Button_Toggle_Channels_Export.Name = "Button_Toggle_Channels_Export";
            this.Button_Toggle_Channels_Export.Size = new System.Drawing.Size(82, 28);
            this.Button_Toggle_Channels_Export.TabIndex = 6;
            this.Button_Toggle_Channels_Export.Text = "Export";
            this.Button_Toggle_Channels_Export.UseVisualStyleBackColor = false;
            this.Button_Toggle_Channels_Export.Click += new System.EventHandler(this.Button_Toggle_Channels_Export_Click);
            // 
            // lblRecordLength
            // 
            this.lblRecordLength.AutoSize = true;
            this.lblRecordLength.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRecordLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecordLength.Location = new System.Drawing.Point(850, 0);
            this.lblRecordLength.Name = "lblRecordLength";
            this.lblRecordLength.Size = new System.Drawing.Size(120, 31);
            this.lblRecordLength.TabIndex = 0;
            this.lblRecordLength.Text = "00:00:00";
            this.lblRecordLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.AutoSize = true;
            this.lblPlayerPos.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblPlayerPos.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayerPos.Location = new System.Drawing.Point(38, 0);
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.Size = new System.Drawing.Size(120, 31);
            this.lblPlayerPos.TabIndex = 0;
            this.lblPlayerPos.Text = "00:00:00";
            this.lblPlayerPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Button_Mute
            // 
            this.Button_Mute.BackColor = System.Drawing.Color.Black;
            this.Button_Mute.Dock = System.Windows.Forms.DockStyle.Right;
            this.Button_Mute.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_Mute.Location = new System.Drawing.Point(970, 0);
            this.Button_Mute.Margin = new System.Windows.Forms.Padding(0);
            this.Button_Mute.Name = "Button_Mute";
            this.Button_Mute.Size = new System.Drawing.Size(38, 40);
            this.Button_Mute.TabIndex = 0;
            this.Button_Mute.TabStop = false;
            this.Button_Mute.UseVisualStyleBackColor = false;
            this.Button_Mute.Click += new System.EventHandler(this.Button_Mute_Click);
            // 
            // panel4
            // 
            this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel4.Controls.Add(this.TextBox_Url);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(668, 415);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(83, 38);
            this.panel4.TabIndex = 9;
            this.panel4.Visible = false;
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
            // panel5
            // 
            this.panel5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel5.Controls.Add(this.Label_EndTime);
            this.panel5.Controls.Add(this.TextBox_ExportFileName);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.Label_StartTime);
            this.panel5.Controls.Add(this.Button_ExportSave);
            this.panel5.Controls.Add(this.Combo_ExportFileExtension);
            this.panel5.Location = new System.Drawing.Point(218, 345);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(572, 38);
            this.panel5.TabIndex = 10;
            this.panel5.Visible = false;
            // 
            // Label_EndTime
            // 
            this.Label_EndTime.AutoSize = true;
            this.Label_EndTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_EndTime.Location = new System.Drawing.Point(64, 12);
            this.Label_EndTime.Name = "Label_EndTime";
            this.Label_EndTime.Size = new System.Drawing.Size(55, 16);
            this.Label_EndTime.TabIndex = 16;
            this.Label_EndTime.Text = "00:00:00";
            this.Label_EndTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_EndTime.Click += new System.EventHandler(this.Label_EndTime_Click);
            // 
            // TextBox_ExportFileName
            // 
            this.TextBox_ExportFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextBox_ExportFileName.Location = new System.Drawing.Point(122, 6);
            this.TextBox_ExportFileName.Name = "TextBox_ExportFileName";
            this.TextBox_ExportFileName.Size = new System.Drawing.Size(292, 26);
            this.TextBox_ExportFileName.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(55, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "-";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label_StartTime
            // 
            this.Label_StartTime.AutoSize = true;
            this.Label_StartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_StartTime.Location = new System.Drawing.Point(3, 12);
            this.Label_StartTime.Name = "Label_StartTime";
            this.Label_StartTime.Size = new System.Drawing.Size(55, 16);
            this.Label_StartTime.TabIndex = 13;
            this.Label_StartTime.Text = "00:00:00";
            this.Label_StartTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label_StartTime.Click += new System.EventHandler(this.Label_StartTime_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(270, 288);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(68, 26);
            this.textBox1.TabIndex = 12;
            this.textBox1.Text = "00:00:00";
            // 
            // Button_ExportSave
            // 
            this.Button_ExportSave.AutoSize = true;
            this.Button_ExportSave.BackColor = System.Drawing.Color.DarkBlue;
            this.Button_ExportSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Button_ExportSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Button_ExportSave.Location = new System.Drawing.Point(492, 4);
            this.Button_ExportSave.Name = "Button_ExportSave";
            this.Button_ExportSave.Size = new System.Drawing.Size(69, 28);
            this.Button_ExportSave.TabIndex = 5;
            this.Button_ExportSave.Text = "Save";
            this.Button_ExportSave.UseVisualStyleBackColor = false;
            this.Button_ExportSave.Click += new System.EventHandler(this.Button_ExportSave_Click);
            // 
            // Combo_ExportFileExtension
            // 
            this.Combo_ExportFileExtension.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_ExportFileExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Combo_ExportFileExtension.FormattingEnabled = true;
            this.Combo_ExportFileExtension.Items.AddRange(new object[] {
            ".mp4",
            ".mp3",
            ".ts"});
            this.Combo_ExportFileExtension.Location = new System.Drawing.Point(422, 4);
            this.Combo_ExportFileExtension.Name = "Combo_ExportFileExtension";
            this.Combo_ExportFileExtension.Size = new System.Drawing.Size(64, 28);
            this.Combo_ExportFileExtension.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(360, 288);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(68, 26);
            this.textBox2.TabIndex = 12;
            this.textBox2.Text = "00:00:00";
            // 
            // radioZiner
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1008, 729);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.PictureBox_Player);
            this.Controls.Add(this.ListBox_Titles);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.textBox2);
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
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
        private System.Windows.Forms.Button Button_Toggle_Channels_Export;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label Label_EndTime;
        private System.Windows.Forms.TextBox TextBox_ExportFileName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label Label_StartTime;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Button_ExportSave;
        private System.Windows.Forms.ComboBox Combo_ExportFileExtension;
        private System.Windows.Forms.TextBox textBox2;
    }
}

