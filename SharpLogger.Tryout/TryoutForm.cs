using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class TryoutForm : Form
    {
        private LogRunner log;
        private LogDao dao;

        private volatile bool asyncFeed;
        private volatile bool useLogger;

        public TryoutForm()
        {
            InitializeComponent();
        }

        private string ToLevel(int index)
        {
            switch(index % 5)
            {
                case 0:
                    return LogDto.DEBUG;
                case 1:
                    return LogDto.INFO;
                case 2:
                    return LogDto.WARN;
                case 3:
                    return LogDto.ERROR;
                case 4:
                    return LogDto.SUCCESS;
            }
            throw new Exception($"Invalid index {index}");
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

        private string StackTrace()
        {
            return "Environment.StackTrace\n" + Environment.StackTrace;
        }

        private void SetLines(int count)
        {
            tabControl1.SelectedIndex = 0;
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

        private void TryoutForm_Load(object sender, EventArgs e)
        {
            dao = new LogDao();
            log = new LogRunner();
            log.AddAppender(dao);
            log.AddAppender(logControl);
            log.Run(() => throw new Exception("Test Exception"));

            Task.Run(() => {
                var i = 0;
                var dts = DateTime.Now;
                while (true)
                {
                    if (!asyncFeed)
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
                    if (i % 5 == 2) dto.Message = StackTrace();
                    if (useLogger) log.Log(dto.Level, dto.Message);
                    else logControl.Append(dto);
                    if (i%10==0) Thread.Sleep(1);
                    i++;
                }
            });
        }

        private void TryoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Dispose();
        }

        private void button100_Click(object sender, EventArgs e)
        {
            SetLines(100);
            logControl.LineLimit = 100;
        }

        private void button1000_Click(object sender, EventArgs e)
        {
            SetLines(1000);
            logControl.LineLimit = 1000;
        }

        private void checkBoxAsyncFeed_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            asyncFeed = checkBoxAsyncFeed.Checked;
        }

        private void checkBoxUseLogger_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            useLogger = checkBoxUseLogger.Checked;
        }

        private void buttonRefresh10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            logControl.PollPeriod = 10;
        }

        private void buttonRefresh100_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            logControl.PollPeriod = 100;
        }
    }
}
