namespace tsneDemo
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			pictureBox1 = new PictureBox();
			timer1 = new System.Windows.Forms.Timer(components);
			toolStrip1 = new ToolStrip();
			toolStripButton4 = new ToolStripButton();
			toolStripButton2 = new ToolStripButton();
			toolStripButton1 = new ToolStripButton();
			toolStripButton3 = new ToolStripButton();
			toolStripButton5 = new ToolStripButton();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			toolStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.Dock = DockStyle.Fill;
			pictureBox1.Location = new Point(0, 0);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new Size(942, 615);
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// timer1
			// 
			timer1.Enabled = true;
			timer1.Interval = 25;
			timer1.Tick += timer1_Tick;
			// 
			// toolStrip1
			// 
			toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButton4, toolStripButton2, toolStripButton1, toolStripButton3, toolStripButton5 });
			toolStrip1.Location = new Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new Size(942, 25);
			toolStrip1.TabIndex = 1;
			toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButton4
			// 
			toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton4.Image = (Image)resources.GetObject("toolStripButton4.Image");
			toolStripButton4.ImageTransparentColor = Color.Magenta;
			toolStripButton4.Name = "toolStripButton4";
			toolStripButton4.Size = new Size(37, 22);
			toolStripButton4.Text = "fit all";
			toolStripButton4.Click += toolStripButton4_Click;
			// 
			// toolStripButton2
			// 
			toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton2.Image = (Image)resources.GetObject("toolStripButton2.Image");
			toolStripButton2.ImageTransparentColor = Color.Magenta;
			toolStripButton2.Name = "toolStripButton2";
			toolStripButton2.Size = new Size(42, 22);
			toolStripButton2.Text = "pause";
			toolStripButton2.Click += toolStripButton2_Click;
			// 
			// toolStripButton1
			// 
			toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
			toolStripButton1.ImageTransparentColor = Color.Magenta;
			toolStripButton1.Name = "toolStripButton1";
			toolStripButton1.Size = new Size(38, 22);
			toolStripButton1.Text = "+size";
			toolStripButton1.Click += toolStripButton1_Click;
			// 
			// toolStripButton3
			// 
			toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
			toolStripButton3.ImageTransparentColor = Color.Magenta;
			toolStripButton3.Name = "toolStripButton3";
			toolStripButton3.Size = new Size(35, 22);
			toolStripButton3.Text = "-size";
			toolStripButton3.Click += toolStripButton3_Click;
			// 
			// toolStripButton5
			// 
			toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.Text;
			toolStripButton5.Image = (Image)resources.GetObject("toolStripButton5.Image");
			toolStripButton5.ImageTransparentColor = Color.Magenta;
			toolStripButton5.Name = "toolStripButton5";
			toolStripButton5.Size = new Size(49, 22);
			toolStripButton5.Text = "auto fit";
			toolStripButton5.Click += toolStripButton5_Click;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(942, 615);
			Controls.Add(toolStrip1);
			Controls.Add(pictureBox1);
			Name = "Form1";
			Text = " 50-dimensional word vectors demo";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private PictureBox pictureBox1;
		private System.Windows.Forms.Timer timer1;
		private ToolStrip toolStrip1;
		private ToolStripButton toolStripButton2;
		private ToolStripButton toolStripButton1;
		private ToolStripButton toolStripButton3;
		private ToolStripButton toolStripButton4;
		private ToolStripButton toolStripButton5;
	}
}