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
            this.tbURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddChannel = new System.Windows.Forms.Button();
            this.cbShortName = new System.Windows.Forms.ComboBox();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbChannelSet = new System.Windows.Forms.ComboBox();
            this.lblPlayerPos = new System.Windows.Forms.Label();
            this.lblRecordLength = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbURL
            // 
            this.tbURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbURL.Location = new System.Drawing.Point(671, 8);
            this.tbURL.Name = "tbURL";
            this.tbURL.Size = new System.Drawing.Size(127, 20);
            this.tbURL.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(635, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "URL:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnAddChannel
            // 
            this.btnAddChannel.AutoSize = true;
            this.btnAddChannel.BackColor = System.Drawing.Color.Maroon;
            this.btnAddChannel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddChannel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddChannel.Location = new System.Drawing.Point(806, 4);
            this.btnAddChannel.Name = "btnAddChannel";
            this.btnAddChannel.Size = new System.Drawing.Size(55, 28);
            this.btnAddChannel.TabIndex = 3;
            this.btnAddChannel.Text = "Rec";
            this.btnAddChannel.UseVisualStyleBackColor = false;
            this.btnAddChannel.Click += new System.EventHandler(this.BtnAddChannel_Click);
            // 
            // cbShortName
            // 
            this.cbShortName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbShortName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbShortName.FormattingEnabled = true;
            this.cbShortName.Location = new System.Drawing.Point(368, 4);
            this.cbShortName.Name = "cbShortName";
            this.cbShortName.Size = new System.Drawing.Size(263, 28);
            this.cbShortName.TabIndex = 1;
            this.cbShortName.SelectedIndexChanged += new System.EventHandler(this.CbShortName_SelectedIndexChanged);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.AutoSize = true;
            this.btnPlayPause.BackColor = System.Drawing.Color.Black;
            this.btnPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlayPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPlayPause.Location = new System.Drawing.Point(83, 5);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(58, 28);
            this.btnPlayPause.TabIndex = 1;
            this.btnPlayPause.Text = "Pause";
            this.btnPlayPause.UseVisualStyleBackColor = false;
            this.btnPlayPause.Click += new System.EventHandler(this.Button3_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.ForeColor = System.Drawing.Color.White;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 29;
            this.listBox1.Location = new System.Drawing.Point(0, 45);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(507, 232);
            this.listBox1.TabIndex = 1;
            this.listBox1.Click += new System.EventHandler(this.ListBox1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(592, 156);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
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
            this.panel2.Controls.Add(this.cbChannelSet);
            this.panel2.Controls.Add(this.btnPlayPause);
            this.panel2.Controls.Add(this.cbShortName);
            this.panel2.Controls.Add(this.lblPlayerPos);
            this.panel2.Controls.Add(this.lblRecordLength);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnAddChannel);
            this.panel2.Controls.Add(this.tbURL);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1008, 41);
            this.panel2.TabIndex = 6;
            // 
            // cbChannelSet
            // 
            this.cbChannelSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbChannelSet.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbChannelSet.FormattingEnabled = true;
            this.cbChannelSet.Location = new System.Drawing.Point(149, 4);
            this.cbChannelSet.Name = "cbChannelSet";
            this.cbChannelSet.Size = new System.Drawing.Size(211, 28);
            this.cbChannelSet.TabIndex = 6;
            this.cbChannelSet.SelectedIndexChanged += new System.EventHandler(this.CbChannelSet_SelectedIndexChanged);
            // 
            // lblPlayerPos
            // 
            this.lblPlayerPos.AutoSize = true;
            this.lblPlayerPos.Location = new System.Drawing.Point(3, 8);
            this.lblPlayerPos.Name = "lblPlayerPos";
            this.lblPlayerPos.Size = new System.Drawing.Size(80, 24);
            this.lblPlayerPos.TabIndex = 3;
            this.lblPlayerPos.Text = "00:00:00";
            this.lblPlayerPos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRecordLength
            // 
            this.lblRecordLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecordLength.AutoSize = true;
            this.lblRecordLength.Location = new System.Drawing.Point(928, 7);
            this.lblRecordLength.Name = "lblRecordLength";
            this.lblRecordLength.Size = new System.Drawing.Size(80, 24);
            this.lblRecordLength.TabIndex = 4;
            this.lblRecordLength.Text = "00:00:00";
            this.lblRecordLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.Color.DimGray;
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.Location = new System.Drawing.Point(0, 0);
            this.trackBar1.Maximum = 7200;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(1008, 65);
            this.trackBar1.TabIndex = 2;
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
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.panel3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "radioZiner";
            this.Text = "radioZiner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RadioZiner_FormClosing);
            this.Load += new System.EventHandler(this.RadioZiner_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RadioZiner_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RadioZiner_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox tbURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddChannel;
        private System.Windows.Forms.ComboBox cbShortName;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.Label lblPlayerPos;
        private System.Windows.Forms.Label lblRecordLength;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cbChannelSet;
    }
}

