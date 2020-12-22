using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class PanelTryoutForm : Form
    {
        public PanelTryoutForm()
        {
            InitializeComponent();
        }

        private Color ToColor(int index)
        {
            switch (index % 5)
            {
                case 0:
                    return Color.Gray;
                case 1:
                    return Color.White;
                case 2:
                    return Color.Yellow;
                case 3:
                    return Color.Tomato;
                case 4:
                    return Color.PaleGreen;
            }
            throw new Exception($"Invalid index {index}");
        }

        private void SetLines(int count)
        {
            var list = new List<LogLine>();
            for (var i = 0; i < count; i++)
            {
                var line = new LogLine();
                var sb = new StringBuilder();
                sb.Append($"Line {i}");
                for (var j = 0; j < i; j++)
                {
                    sb.Append($" {j}");
                }
                line.Line = sb.ToString();
                line.Color = ToColor(i);
                list.Add(line);
            }
            logPanel.SetLines(list.ToArray());
        }

        private void button1000_Click(object sender, EventArgs e)
        {
            SetLines(1000);
        }

        private void button100_Click(object sender, EventArgs e)
        {
            SetLines(100);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetLines(10);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetLines(0);
        }
    }
}
