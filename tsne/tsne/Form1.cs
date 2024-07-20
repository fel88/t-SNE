using Newtonsoft.Json;
using static System.Windows.Forms.AxHost;

namespace tsneDemo
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			//Sample();

			var json = File.ReadAllText("wordvecs50dtop1000.json");
			data = JsonConvert.DeserializeObject(json);

			var vecs = data.vecs.ToObject<double[][]>();
			pictureBox1.Paint += PictureBox1_Paint;

			pictureBox1.MouseWheel += PictureBox1_MouseWheel;
			pictureBox1.MouseDown += PictureBox1_MouseDown;
			pictureBox1.MouseUp += PictureBox1_MouseUp;
			tsne.initDataRaw(vecs); // init embedding
		}

		public void UpdateDrag()
		{
			if (isDrag)
			{
				var p = pictureBox1.PointToClient(Cursor.Position);

				sx = origsx + ((p.X - startx) / zoom);
				sy = origsy + (-(p.Y - starty) / zoom);
			}
		}
		public float startx, starty;
		public float origsx, origsy;

		public bool isDrag = false;
		private bool isMiddleDrag = false;
		private void PictureBox1_MouseDown(object? sender, MouseEventArgs e)
		{
			var pos = pictureBox1.PointToClient(Cursor.Position);

			if (e.Button == MouseButtons.Right)
			{
				isDrag = true;

				startx = pos.X;
				starty = pos.Y;
				origsx = sx;
				origsy = sy;
			}
			if (e.Button == MouseButtons.Middle)
			{
				isMiddleDrag = true;

				startx = pos.X;
				starty = pos.Y;
				origsx = sx;
				origsy = sy;

			}


		}

		public float sx, sy;
		public float zoom = 1;
		public float ZoomFactor = 1.5f;

		private void PictureBox1_MouseWheel(object? sender, MouseEventArgs e)
		{
			//zoom *= Math.Sign(e.Delta) * 1.3f;
			//zoom += Math.Sign(e.Delta) * 0.31f;
			autoFit = false;
			toolStripButton5.Checked = autoFit;

			var pos = pictureBox1.PointToClient(Cursor.Position);
			if (!pictureBox1.ClientRectangle.IntersectsWith(new Rectangle(pos.X, pos.Y, 1, 1)))
			{
				return;
			}

			float zold = zoom;

			if (e.Delta > 0) { zoom *= ZoomFactor; } else { zoom /= ZoomFactor; }

			if (zoom < 0.0008) { zoom = 0.0008f; }
			if (zoom > 10000) { zoom = 10000f; }

			sx = -(pos.X / zold - sx - pos.X / zoom);
			sy = (pos.Y / zold + sy - pos.Y / zoom);
		}
		public virtual PointF Transform(PointF p1)
		{
			return new PointF((p1.X + sx) * zoom, -(p1.Y + sy) * zoom);
		}
		public virtual PointF Transform(float x, float y)
		{
			return new PointF((x + sx) * zoom, -(y + sy) * zoom);
		}
		public virtual PointF Transform(double x, double y)
		{
			return new PointF((float)((x + sx) * zoom), (float)(-(y + sy) * zoom));
		}

		public void PictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			isDrag = false;
			isMiddleDrag = false;
		}

		dynamic data;


		private void PictureBox1_Paint(object? sender, PaintEventArgs e)
		{
			UpdateDrag();
			e.Graphics.Clear(Color.White);
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			if (points == null)
				return;

			for (int i = 0; i < points.Length; i++)
			{
				PointF item = points[i];
				var word = (string)data.words[i];
				var tr = Transform(item);
				if (!pictureBox1.ClientRectangle.Contains((int)tr.X, (int)tr.Y))
					continue;

				e.Graphics.DrawEllipse(Pens.Black, tr.X - 3, tr.Y - 3, 6, 6);
			}

			var curp = pictureBox1.PointToClient(Cursor.Position);

			for (int i = 0; i < points.Length; i++)
			{
				PointF item = points[i];
				var word = (string)data.words[i];
				var tr = Transform(item);
				if (!pictureBox1.ClientRectangle.Contains((int)tr.X, (int)tr.Y))
					continue;

				var dist = Math.Sqrt(Math.Pow(tr.X - curp.X, 2) + Math.Pow(tr.Y - curp.Y, 2));
				if (dist < wordRadius)
					e.Graphics.DrawString(word, SystemFonts.DefaultFont, Brushes.Blue, tr.X, tr.Y);

			}

			e.Graphics.DrawEllipse(Pens.Red, curp.X - wordRadius, curp.Y - wordRadius, wordRadius * 2, wordRadius * 2);
		}
		int wordRadius = 20;

		int tx = 0, ty = 0;
		int ss = 1;
		PointF[] points = null;
		public void updateEmbedding()
		{
			var Y = tsne.getSolution();
			if (points == null || points.Length != Y.Length)
			{
				points = new PointF[Y.Length];
			}

			for (int i = 0; i < Y.Length; i++)
			{
				var xx = Y[i][0] * 20 * ss + tx + 400;
				var yy = Y[i][1] * 20 * ss + ty + 400;
				points[i] = new PointF((float)xx, (float)yy);
			}
		}

		tSNE tsne = new tSNE(new tsneSettings() { });

		private void Sample()
		{
			var opt = new tsneSettings();
			opt.epsilon = 10; // epsilon is learning rate (10 = default)
			opt.perplexity = 30; // roughly how many neighbors each point influences (30 = default)
			opt.dim = 2; // dimensionality of the embedding (2 = default)

			var tsne = new tSNE(opt); // create a tSNE instance

			// initialize data. Here we have 3 points and some example pairwise dissimilarities
			var dists = new double[][] { new[] { 1.0, 0.1, 0.2 }, new[] { 0.1, 1.0, 0.3 }, new[] { 0.2, 0.1, 1.0 } };
			tsne.initDataDist(dists);

			for (var k = 0; k < 500; k++)
			{
				tsne.step(); // every time you call this, solution gets better
			}

			var Y = tsne.getSolution(); // Y is an array of 2-D points that you can plot
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!pause)
			{
				tsne.step();
				updateEmbedding();
			}

			if (autoFit)
				FitAll();

			pictureBox1.Invalidate();
		}


		bool pause = false;
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			pause = !pause;
			toolStripButton2.Checked = pause;
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			wordRadius *= 2;
		}

		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			wordRadius /= 2;
		}
		public void FitToPoints(PointF[] points, int gap = 0)
		{
			var maxx = points.Max(z => z.X) + gap;
			var minx = points.Min(z => z.X) - gap;
			var maxy = points.Max(z => z.Y) + gap;
			var miny = points.Min(z => z.Y) - gap;

			var w = pictureBox1.Width;
			var h = pictureBox1.Height;

			var dx = maxx - minx;
			var kx = w / dx;
			var dy = maxy - miny;
			var ky = h / dy;

			var oz = zoom;
			var sz1 = new Size((int)(dx * kx), (int)(dy * kx));
			var sz2 = new Size((int)(dx * ky), (int)(dy * ky));
			zoom = kx;
			if (sz1.Width > w || sz1.Height > h) zoom = ky;

			var x = dx / 2 + minx;
			var y = dy / 2 + miny;

			sx = ((w / 2f) / zoom - x);
			sy = -((h / 2f) / zoom + y);



		}
		bool autoFit = true;
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			FitAll();
		}
		void FitAll()
		{
			FitToPoints(points);
		}

		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			autoFit = !autoFit;
			toolStripButton5.Checked = autoFit;
		}
	}
}