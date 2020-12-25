using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class ControlTryoutForm : Form
    {
        private LogRunner log;

        private volatile bool asyncFeed;
        private volatile bool useLogger;

        public ControlTryoutForm()
        {
            InitializeComponent();
        }

        private string ToLevel(int index)
        {
            switch (index % 5)
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

        private string StackTrace()
        {
            return "Environment.StackTrace\n" + Environment.StackTrace;
        }

        private void TryoutForm_Load(object sender, EventArgs e)
        {
            log = new LogRunner();
            log.AddAppender(new LogFile());
            log.AddAppender(logControl);

            Task.Factory.StartNew(() =>
            {
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
                    else logControl.HandleLog(dto);
                    if (i % 10 == 0) Thread.Sleep(1);
                    i++;
                }
            });
        }

        private void TryoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Dispose();
        }

        private void button1000_Click(object sender, EventArgs e)
        {
            logControl.LineLimit = 1000;
        }

        private void button100_Click(object sender, EventArgs e)
        {
            logControl.LineLimit = 100;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            logControl.LineLimit = 10;
        }

        private void checkBoxAsyncFeed_CheckedChanged(object sender, EventArgs e)
        {
            asyncFeed = checkBoxAsyncFeed.Checked;
        }

        private void checkBoxUseLogger_CheckedChanged(object sender, EventArgs e)
        {
            useLogger = checkBoxUseLogger.Checked;
        }

        private void buttonRefresh10_Click(object sender, EventArgs e)
        {
            logControl.PollPeriod = 10;
        }

        private void buttonRefresh100_Click(object sender, EventArgs e)
        {
            logControl.PollPeriod = 100;
        }

        private void buttonException_Click(object sender, EventArgs e)
        {
            log.Run(() => throw new Exception("Test Exception"));
        }
    }
}
