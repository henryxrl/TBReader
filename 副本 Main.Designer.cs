namespace TBReader
{
	partial class Main
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.selectTXT_button = new System.Windows.Forms.Button();
            this.hotkeys_button = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.apt_tbar = new System.Windows.Forms.TrackBar();
            this.draghere_pbox = new System.Windows.Forms.PictureBox();
            this.dropTXTHere_label = new System.Windows.Forms.Label();
            this.or_label = new System.Windows.Forms.Label();
            this.apt_0sec_label = new System.Windows.Forms.Label();
            this.apt_0secManual_label = new System.Windows.Forms.Label();
            this.apt_20sec_label = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.apt_main_label = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.checkMark_pbox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.apt_tbar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.draghere_pbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMark_pbox)).BeginInit();
            this.SuspendLayout();
            // 
            // selectTXT_button
            // 
            this.selectTXT_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.selectTXT_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.selectTXT_button.Location = new System.Drawing.Point(367, 12);
            this.selectTXT_button.Name = "selectTXT_button";
            this.selectTXT_button.Size = new System.Drawing.Size(75, 123);
            this.selectTXT_button.TabIndex = 0;
            this.selectTXT_button.Text = "Select TXT";
            this.selectTXT_button.UseVisualStyleBackColor = true;
            this.selectTXT_button.Click += new System.EventHandler(this.button1_Click);
            // 
            // hotkeys_button
            // 
            this.hotkeys_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hotkeys_button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.hotkeys_button.Location = new System.Drawing.Point(367, 147);
            this.hotkeys_button.Name = "hotkeys_button";
            this.hotkeys_button.Size = new System.Drawing.Size(75, 63);
            this.hotkeys_button.TabIndex = 1;
            this.hotkeys_button.Text = "Hot Keys";
            this.hotkeys_button.UseVisualStyleBackColor = true;
            this.hotkeys_button.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(135, 526);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(220, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Start reading!";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(93, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "TBReader";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // apt_tbar
            // 
            this.apt_tbar.Cursor = System.Windows.Forms.Cursors.NoMoveHoriz;
            this.apt_tbar.Location = new System.Drawing.Point(12, 147);
            this.apt_tbar.Maximum = 20;
            this.apt_tbar.Name = "apt_tbar";
            this.apt_tbar.Size = new System.Drawing.Size(343, 45);
            this.apt_tbar.TabIndex = 6;
            this.apt_tbar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // draghere_pbox
            // 
            this.draghere_pbox.Image = ((System.Drawing.Image)(resources.GetObject("draghere_pbox.Image")));
            this.draghere_pbox.Location = new System.Drawing.Point(12, 12);
            this.draghere_pbox.Name = "draghere_pbox";
            this.draghere_pbox.Size = new System.Drawing.Size(344, 121);
            this.draghere_pbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.draghere_pbox.TabIndex = 7;
            this.draghere_pbox.TabStop = false;
            this.draghere_pbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragDrop);
            this.draghere_pbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox1_DragEnter);
            // 
            // dropTXTHere_label
            // 
            this.dropTXTHere_label.AllowDrop = true;
            this.dropTXTHere_label.AutoSize = true;
            this.dropTXTHere_label.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dropTXTHere_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dropTXTHere_label.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dropTXTHere_label.Location = new System.Drawing.Point(53, 37);
            this.dropTXTHere_label.Name = "dropTXTHere_label";
            this.dropTXTHere_label.Size = new System.Drawing.Size(87, 39);
            this.dropTXTHere_label.TabIndex = 8;
            this.dropTXTHere_label.Text = "TBA";
            this.dropTXTHere_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dropTXTHere_label.DragDrop += new System.Windows.Forms.DragEventHandler(this.dropTXTHere_label_DragDrop);
            this.dropTXTHere_label.DragEnter += new System.Windows.Forms.DragEventHandler(this.dropTXTHere_label_DragEnter);
            // 
            // or_label
            // 
            this.or_label.AllowDrop = true;
            this.or_label.AutoSize = true;
            this.or_label.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.or_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.or_label.ForeColor = System.Drawing.SystemColors.GrayText;
            this.or_label.Location = new System.Drawing.Point(270, 89);
            this.or_label.Name = "or_label";
            this.or_label.Size = new System.Drawing.Size(60, 20);
            this.or_label.TabIndex = 9;
            this.or_label.Text = "Or...→";
            this.or_label.DragDrop += new System.Windows.Forms.DragEventHandler(this.or_label_DragDrop);
            this.or_label.DragEnter += new System.Windows.Forms.DragEventHandler(this.or_label_DragEnter);
            // 
            // apt_0sec_label
            // 
            this.apt_0sec_label.AutoSize = true;
            this.apt_0sec_label.Location = new System.Drawing.Point(9, 182);
            this.apt_0sec_label.Name = "apt_0sec_label";
            this.apt_0sec_label.Size = new System.Drawing.Size(39, 13);
            this.apt_0sec_label.TabIndex = 10;
            this.apt_0sec_label.Text = "0 (sec)";
            // 
            // apt_0secManual_label
            // 
            this.apt_0secManual_label.AutoSize = true;
            this.apt_0secManual_label.Location = new System.Drawing.Point(9, 197);
            this.apt_0secManual_label.Name = "apt_0secManual_label";
            this.apt_0secManual_label.Size = new System.Drawing.Size(42, 13);
            this.apt_0secManual_label.TabIndex = 11;
            this.apt_0secManual_label.Text = "Manual";
            // 
            // apt_20sec_label
            // 
            this.apt_20sec_label.AutoSize = true;
            this.apt_20sec_label.Location = new System.Drawing.Point(320, 182);
            this.apt_20sec_label.Name = "apt_20sec_label";
            this.apt_20sec_label.Size = new System.Drawing.Size(45, 13);
            this.apt_20sec_label.TabIndex = 12;
            this.apt_20sec_label.Text = "20 (sec)";
            // 
            // apt_main_label
            // 
            this.apt_main_label.AutoSize = true;
            this.apt_main_label.Location = new System.Drawing.Point(138, 179);
            this.apt_main_label.Name = "apt_main_label";
            this.apt_main_label.Size = new System.Drawing.Size(28, 13);
            this.apt_main_label.TabIndex = 13;
            this.apt_main_label.Text = "APT";
            this.apt_main_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // checkMark_pbox
            // 
            this.checkMark_pbox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.checkMark_pbox.Image = ((System.Drawing.Image)(resources.GetObject("checkMark_pbox.Image")));
            this.checkMark_pbox.Location = new System.Drawing.Point(60, 89);
            this.checkMark_pbox.Name = "checkMark_pbox";
            this.checkMark_pbox.Size = new System.Drawing.Size(24, 24);
            this.checkMark_pbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.checkMark_pbox.TabIndex = 14;
            this.checkMark_pbox.TabStop = false;
            this.checkMark_pbox.Visible = false;
            // 
            // Main
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 222);
            this.Controls.Add(this.checkMark_pbox);
            this.Controls.Add(this.or_label);
            this.Controls.Add(this.dropTXTHere_label);
            this.Controls.Add(this.apt_20sec_label);
            this.Controls.Add(this.apt_0secManual_label);
            this.Controls.Add(this.apt_0sec_label);
            this.Controls.Add(this.apt_main_label);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.hotkeys_button);
            this.Controls.Add(this.selectTXT_button);
            this.Controls.Add(this.apt_tbar);
            this.Controls.Add(this.draghere_pbox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TBReader";
            this.Load += new System.EventHandler(this.Main_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.apt_tbar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.draghere_pbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkMark_pbox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button selectTXT_button;
		private System.Windows.Forms.Button hotkeys_button;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.NotifyIcon notifyIcon1;
		private System.Windows.Forms.TrackBar apt_tbar;
		private System.Windows.Forms.PictureBox draghere_pbox;
		private System.Windows.Forms.Label dropTXTHere_label;
		private System.Windows.Forms.Label or_label;
		private System.Windows.Forms.Label apt_0sec_label;
		private System.Windows.Forms.Label apt_0secManual_label;
		private System.Windows.Forms.Label apt_20sec_label;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label apt_main_label;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.PictureBox checkMark_pbox;
	}
}

