using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chart
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.Load += Form1_Load;
		}

		void Form1_Load(object sender, EventArgs e)
		{
			scoreChart.Series[0].Points.AddXY(1, 2);
			scoreChart.Series[0].Points.AddXY(3, 4);
			scoreChart.Series[0].Points.AddXY(5, 6);
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}
	}
}
