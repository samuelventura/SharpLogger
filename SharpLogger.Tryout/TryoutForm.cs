using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class TryoutForm : Form
    {
        private volatile bool feed;

        public TryoutForm()
        {
            InitializeComponent();
        }

        private LogLevel ToLevel(int index)
        {
            switch(index % 4)
            {
                case 0:
                    return LogLevel.DEBUG;
                case 1:
                    return LogLevel.INFO;
                case 2:
                    return LogLevel.WARN;
                case 3:
                    return LogLevel.ERROR;
            }
            throw new Exception($"Invalid index {index}");
        }

        private void TryoutForm_Load(object sender, EventArgs e)
        {
            Task.Run(() => {
                var i = 0;
                var dts = DateTime.Now;
                while (true)
                {
                    if (!feed)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    var dto = new LogDto();
                    var sb = new StringBuilder();
                    var ts = DateTime.Now - dts;
                    sb.Append($"Line {i} {ts.TotalSeconds:0.000}");
                    for (var j = 0; j < i % 100; j++)
                    {
                        sb.Append($" {j}");
                    }
                    dto.Message = sb.ToString();
                    dto.Level = ToLevel(i);
                    logControl.Push(dto);
                    if (i%10==0) Thread.Sleep(1);
                    i++;
                }
            });
        }

        private void button100_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            var list = new List<LogDto>();
            for (var i = 0; i < 100; i++)
            {
                var dto = new LogDto();
                var sb = new StringBuilder();
                sb.Append($" Line {i}");
                for (var j = 0; j < i; j++)
                {
                    sb.Append($" {j}");
                }
                dto.Message = sb.ToString();
                dto.Level = ToLevel(i);
                list.Add(dto);
            }
            logPanel.SetItems(list.ToArray());
        }

        private void checkBoxAsyncFee_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            feed = checkBoxAsyncFee.Checked;
        }
    }
}
