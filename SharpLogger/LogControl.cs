using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;

namespace SharpLogger
{
    public partial class LogControl : UserControl
    {
        public LogControl()
        {
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
		public Color ErrorColor
		{
			get { return logPanel.ErrorColor; }
			set { logPanel.ErrorColor = value; }
		}
		[Category("Logging")]
		public Color WarnColor
		{
			get { return logPanel.WarnColor; }
			set { logPanel.WarnColor = value; }
		}
		[Category("Logging")]
		public Color InfoColor
		{
			get { return logPanel.InfoColor; }
			set { logPanel.InfoColor = value; }
		}
		[Category("Logging")]
		public Color DebugColor
		{
			get { return logPanel.DebugColor; }
			set { logPanel.DebugColor = value; }
		}
		[Category("Logging")]
		public int LineLimit
		{
			get { return limit; }
			set { limit = value; }
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

		private LinkedList<LogDto> shown = new LinkedList<LogDto>();
		private LinkedList<LogDto> queue = new LinkedList<LogDto>();
		private int limit = 100;
		private bool dirty;

		public void Push(params LogDto[] dtos)
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
				var items = queue.ToArray();
				queue.Clear();
				return items;
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
			foreach (var dto in list)
			{
				sb.AppendLine(dto.ToString());
			}
			Clipboard.SetText(sb.ToString());
		}

        private void Timer_Tick(object sender, EventArgs e)
        {
			timer.Enabled = false;
			var debug = ShowDebug;
			var freeze = freezeViewCheckBox.Checked;
			foreach (var item in Pop())
			{
				if (item.Level != LogLevel.DEBUG || debug)
				{
					shown.AddLast(item);
					dirty = true;
				}
			}
			if (limit <= 0) limit = 100;
			while (shown.Count > limit)
			{
				shown.RemoveFirst();
				dirty = true;
			}
			if (dirty && !freeze)
			{
				logPanel.SetItems(shown.ToArray());
				dirty = false;
			}
			timer.Enabled = true;
		}
	}
}
