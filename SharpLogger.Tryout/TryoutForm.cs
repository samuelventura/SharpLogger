using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpLogger.Tryout
{
    public partial class TryoutForm : Form
    {
        public TryoutForm()
        {
            InitializeComponent();
        }

        private LogLevel ToLevel(int index)
        {
            switch(index % 5)
            {
                case 0:
                    return LogLevel.TRACE;
                case 1:
                    return LogLevel.DEBUG;
                case 2:
                    return LogLevel.INFO;
                case 3:
                    return LogLevel.WARN;
                case 4:
                    return LogLevel.ERROR;
            }
            throw new Exception($"Invalid index {index}");
        }

        private void button100_Click(object sender, EventArgs e)
        {
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

        private void button100k_Click(object sender, EventArgs e)
        {
            Task.Run(() => { 

            });
        }
    }
}
