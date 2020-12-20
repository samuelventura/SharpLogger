using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Text;

namespace SharpLogger
{
    public partial class LogControl : UserControl, ILogAppender, ILogHandler
    {
		public const int LIMIT = 1000;

		private readonly Dictionary<string, Color> colors = new Dictionary<string, Color>();
		private readonly LinkedList<LogLine> shown = new LinkedList<LogLine>();
		private readonly LinkedList<LogDto> queue = new LinkedList<LogDto>();

		private LogFormatter formatter = LogFormatter.TIMEONLY_MESSAGE;
		private int limit = LIMIT;
		private Logger logger;
		private bool dirty;

		public LogControl()
        {
			colors.Add(LogDto.DEBUG, Color.Gray);
			colors.Add(LogDto.INFO, Color.White);
			colors.Add(LogDto.WARN, Color.Yellow);
			colors.Add(LogDto.ERROR, Color.Tomato);
			colors.Add(LogDto.SUCCESS, Color.PaleGreen);
			logger = Create(typeof(LogControl).Name);

			InitializeComponent();
        }

		//DESIGN TIME
		[Category("Logging")]
		public float FontSize
		{
			get { return logPanel.FontSize; }
			set { logPanel.FontSize = value; }
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
		public Color SelectingBack
		{
			get { return logPanel.SelectingBack; }
			set { logPanel.SelectingBack = value; }
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
		public int PollPeriod
		{
			get { return timer.Interval; }
			set { timer.Interval = value; }
		}
		[Category("Logging")]
		public string LogFormat
		{
			get { return formatter.Format; }
			set { formatter = new LogFormatter(value); }
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

		public void AddColor(string level, Color color)
        {
			colors.Add(level, color);
		}

		//Should not switch to UI to prevent thread
		//deathlock when disposing from UI events
		public void Append(params LogDto[] dtos)
		{
			lock (queue)
			{
				//memory leak if disposed clear timer
				foreach(var dto in dtos) queue.AddLast(dto);
			}
		}

		//Should not switch to UI to prevent thread
		//deathlock when disposing from UI events
		public void Append(LogDto dto)
		{
			lock (queue)
			{
				//memory leak if disposed clear timer
				queue.AddLast(dto);
			}
		}

		public Logger Create(string source)
        {
			return new Logger(this, source);
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
			logPanel.SetLines();
		}

        private void CopyButton_Click(object sender, EventArgs e)
        {
			var sb = new StringBuilder();
			var list = logPanel.GetSelected();
			if (list.Length == 0) list = logPanel.GetAll();
			foreach (var line in list)
			{
				if (line.Partial)
                {
					sb.AppendLine(line.Substring);
				}
				else sb.AppendLine(line.Line);
			}
			Clipboard.SetText(sb.ToString());
		}

		private void Lines(string text, Action<string> callback)
        {
			if (text.Contains("\n"))
            {
				var sb = new StringBuilder();
				foreach (var c in text)
				{
					switch (c)
					{
						case '\r':
							break;
						case '\n':
							callback(sb.ToString());
							sb.Clear();
							break;
						case '\t':
							sb.Append("    ");
							break;
						default:
							sb.Append(c);
							break;
					}
				}
				callback(sb.ToString());
			} 
			else
            {
				callback(text);
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
        {
			timer.Enabled = false;
			var debug = ShowDebug;
			var freeze = freezeViewCheckBox.Checked;
			foreach (var dto in Pop())
			{
				if (debug || dto.Level != LogDto.DEBUG)
				{
					var text = formatter.Convert(dto);
					var color = ToColor(dto.Level);
					Lines(text, (single) => {
						var line = new LogLine();
						line.Dto = dto;
						line.Color = color;
						line.Line = single;
						line.Message = text;
						shown.AddLast(line);
					});
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
				logPanel.SetLines(Shown());
				dirty = false;
			}
			timer.Enabled = true;
		}

		private LogLine[] Shown()
        {
			var list = new List<LogLine>();
			foreach (var line in shown) list.Add(line);
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

	public partial class LogControl : ILogger
	{
		public void Log(string level, string format, params object[] args)
		{
			logger.Log(level, format, args);
		}

		public void Debug(string format, params object[] args)
		{
			logger.Debug(format, args);
		}

		public void Info(string format, params object[] args)
		{
			logger.Info(format, args);
		}

		public void Warn(string format, params object[] args)
		{
			logger.Warn(format, args);
		}

		public void Error(string format, params object[] args)
		{
			logger.Error(format, args);
		}

		public void Success(string format, params object[] args)
		{
			logger.Success(format, args);
		}
	}
}
