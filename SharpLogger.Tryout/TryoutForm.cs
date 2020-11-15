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
        private LogRunner logRunner;
        private ILogger logger;
        private LogDao dao;

        private volatile bool asyncFeed;
        private volatile bool userLogger;

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

        private void TryoutForm_Load(object sender, EventArgs e)
        {
            dao = new LogDao();
            logRunner = new LogRunner();
            logRunner.AddAppender(dao);
            logRunner.AddAppender(logControl);
            logRunner.Run(() => throw new Exception("Test Exception"));
            logger = logRunner.Create("DEFAULT");

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
                    if (userLogger) logger.Log(dto.Level, dto.Message);
                    else logControl.Append(dto);
                    if (i%10==0) Thread.Sleep(1);
                    i++;
                }
            });
        }

        private void button100_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            var list = new List<LogItem>();
            for (var i = 0; i < 100; i++)
            {
                var item = new LogItem();
                var sb = new StringBuilder();
                sb.Append($" Line {i}");
                for (var j = 0; j < i; j++)
                {
                    sb.Append($" {j}");
                }
                item.Line = sb.ToString();
                item.Color = ToColor(i);
                list.Add(item);
            }
            logPanel.SetItems(list.ToArray());
        }

        private void checkBoxAsyncFeed_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            asyncFeed = checkBoxAsyncFeed.Checked;
        }

        private void checkBoxUseLogger_CheckedChanged(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            userLogger = checkBoxUseLogger.Checked;
        }
    }
}
