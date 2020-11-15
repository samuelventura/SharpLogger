using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace SharpLogger
{
    public partial class LogControl : UserControl, ILogAppender
    {
		public const int LIMIT = 100;

		public LogControl()
        {
			colors.Add(LogDto.DEBUG, Color.Gray);
			colors.Add(LogDto.INFO, Color.White);
			colors.Add(LogDto.WARN, Color.Yellow);
			colors.Add(LogDto.ERROR, Color.Tomato);
			colors.Add(LogDto.SUCCESS, Color.PaleGreen);

			InitializeComponent();
        }

		//DESIGN TIME
		[Category("Logging")]
		public Font LogFont
		{
			get { return logPanel.Font; }
			set { logPanel.Font = value; }
		}
		[Category("Logging")]
		public Color LogBackColor
		{
			get { return logPanel.BackColor; }
			set { logPanel.BackColor = value; }
		}
		[Category("Logging")]
		public Color SelectionBack
		{
			get { return logPanel.SelectionBack; }
			set { logPanel.SelectionBack = value; }
		}
		[Category("Logging")]
		public Color SelectionFront
		{
			get { return logPanel.SelectionFront; }
			set { logPanel.SelectionFront = value; }
		}
		[Category("Logging")]
		public Color SuccessColor
		{
			get { return colors[LogDto.SUCCESS]; }
			set { colors[LogDto.SUCCESS] = value; }
		}
		[Category("Logging")]
		public Color ErrorColor
		{
			get { return colors[LogDto.ERROR]; }
			set { colors[LogDto.ERROR] = value; }
		}
		[Category("Logging")]
		public Color WarnColor
		{
			get { return colors[LogDto.WARN]; }
			set { colors[LogDto.WARN] = value; }
		}
		[Category("Logging")]
		public Color InfoColor
		{
			get { return colors[LogDto.INFO]; }
			set { colors[LogDto.INFO] = value; }
		}
		[Category("Logging")]
		public Color DebugColor
		{
			get { return colors[LogDto.DEBUG]; }
			set { colors[LogDto.DEBUG] = value; }
		}
		[Category("Logging")]
		public int LineLimit
		{
			get { return limit; }
			set { limit = value; }
		}
		[Category("Logging")]
		public string LogFormat
		{
			get { return formatter.Format; }
			set { formatter = new PatternLogFormatter(value); }
		}
		//DESIGN & RUNTIME
		[Category("Logging")]
		public bool ShowDebug
		{
			get { return showDebugCheckBox.Checked; }
			set { showDebugCheckBox.Checked = value; }
		}
		[Category("Logging")]
		public bool FreezeView
		{
			get { return freezeViewCheckBox.Checked; }
			set { freezeViewCheckBox.Checked = value; }
		}

		private PatternLogFormatter formatter = PatternLogFormatter.TIMEONLY_MESSAGE;
		private Dictionary<string, Color> colors = new Dictionary<string, Color>();
		private LinkedList<LogItem> shown = new LinkedList<LogItem>();
		private LinkedList<LogDto> queue = new LinkedList<LogDto>();
		private int limit = LIMIT;
		private bool dirty;

		public void AddColor(string level, Color color)
        {
			colors.Add(level, color);
		}

		public void Append(params LogDto[] dtos)
		{
			lock (queue)
			{
				//memory leak if disposed clear timer
				foreach(var dto in dtos) queue.AddLast(dto);
			}
		}

		private LogDto[] Pop()
		{
			lock (queue)
			{
				var list = new List<LogDto>();
				while (queue.Count > 0)
				{
					list.Add(queue.First.Value);
					queue.RemoveFirst();
				}
				return list.ToArray();
			}
		}

		private void ClearButton_Click(object sender, EventArgs e)
        {
			shown.Clear();
			logPanel.SetItems();
		}

        private void CopyButton_Click(object sender, EventArgs e)
        {
			var sb = new StringBuilder();
			var list = logPanel.GetSelected();
			if (list.Length == 0) list = logPanel.GetAll();
			foreach (var item in list)
			{
				sb.AppendLine(item.Line);
			}
			Clipboard.SetText(sb.ToString());
		}

        private void Timer_Tick(object sender, EventArgs e)
        {
			timer.Enabled = false;
			var debug = ShowDebug;
			var freeze = freezeViewCheckBox.Checked;
			foreach (var dto in Pop())
			{
				if (dto.Level != LogDto.DEBUG || debug)
				{
					var item = new LogItem();
					item.Dto = dto;
					item.Color = ToColor(dto.Level);
					item.Line = formatter.Convert(dto);
					shown.AddLast(item);
					dirty = true;
				}
			}
			if (limit <= 0) limit = LIMIT;
			while (shown.Count > limit)
			{
				shown.RemoveFirst();
				dirty = true;
			}
			if (dirty && !freeze)
			{
				logPanel.SetItems(Shown());
				dirty = false;
			}
			timer.Enabled = true;
		}

		private LogItem[] Shown()
        {
			var list = new List<LogItem>();
			foreach (var item in shown) list.Add(item);
			return list.ToArray();
		}

		private Color ToColor(string level)
		{
			if (colors.TryGetValue(level, out var color))
            {
				return color;
            }
			throw new Exception($"Invalid level {level}");
		}
	}
}
