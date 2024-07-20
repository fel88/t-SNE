using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using tsneDemo;

namespace tsne
{
	public partial class mdi2 : Form
	{
		public mdi2()
		{
			InitializeComponent();
		}

		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			Form1 f = new Form1();
			f.MdiParent = this;
			f.Show();
		}
	}
}
