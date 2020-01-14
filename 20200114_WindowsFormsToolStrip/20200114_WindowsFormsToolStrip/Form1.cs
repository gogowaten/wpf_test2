using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//ToolStripコントロールを使用してツールバーを作成する - .NET Tips(VB.NET, C#...)
//https://dobon.net/vb/dotnet/control/toolstrip.html

namespace _20200114_WindowsFormsToolStrip
{
    public partial class Form1 : Form
    {
        private ToolStrip ToolStrip1;
        private ToolStripButton ToolStripButton1;
        private ToolStripButton ToolStripButton2;

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolStrip1 = new ToolStrip();
            this.SuspendLayout();
            ToolStrip1.SuspendLayout();

            ToolStripButton1 = new ToolStripButton
            {
                Text = "開く(&O)",
                //DisplayStyle = ToolStripItemDisplayStyle.Image,
            };
            ToolStripButton1.Click += ToolStripButton1_Click;
            ToolStripButton2 = new ToolStripButton
            {
                Text = "保存(&S)",
                //DisplayStyle = ToolStripItemDisplayStyle.Image,
            };
            ToolStripButton2.Click += ToolStripButton2_Click;

            ToolStrip1.Items.Add(ToolStripButton1);
            ToolStrip1.Items.Add(ToolStripButton2);

            this.Controls.Add(ToolStrip1);

            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("button2");
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("button1");
        }
    }
}
