using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace SharpLogger
{
	public class LogPanel : Panel
	{
		private ulong moves;
		private Point click;
		private ulong updates;
		private bool executing;
		private Queue<Action> actions;
		private LogModel.SelectionState select;
		private LogModel.OutputState output;
		private LogModel.InputState input;
		private Brush selectionBrush;
		private Brush selectingBrush;
		private LogModel model;
		private Color color;
		private Font font;

		//design time only
		[Category("Logging")]
		public float FontSize { get; set; } = 12;
		[Category("Logging")]
		public Color SelectionBack { get; set; } = Color.SteelBlue;
		[Category("Logging")]
		public Color SelectingBack { get; set; } = Color.LightSteelBlue;
		[Category("Logging")]
		public Color SelectionFront { get; set; } = Color.White;

		public LogPanel()
		{
			BackColor = Color.Black;
			DoubleBuffered = true;
			AutoScroll = true;
		}

		public void SetLines(params LogLine[] lines)
		{
			LogDebug.WriteLine("LogPanel.SetLines {0}", lines.Length);
			QueueInputChange(() => { input.Lines = new LogModel.Lines(lines); });
		}

		public string SelectedText()
        {
			InitializeModel();
			var sb = new StringBuilder();
			var sd = model.Selection.Selected;
			if (sd != null)
            {
				foreach (var line in input.Lines.Array)
				{
					VisitSelection(sd, line, (start, length) => {
						if (sb.Length > 0) sb.AppendLine();
						sb.Append(line.Line.Substring(start, length));
					});
				}
				if (sb.Length > 0) return sb.ToString();
			}
			foreach (var line in input.Lines.Array)
			{
				sb.AppendLine(line.Line);
			}
			return sb.ToString();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			LogDebug.WriteLine("LogPanel.OnPaint {0}", ViewPort());
			var si = model.Selection.Selecting;
			var sd = model.Selection.Selected;
			var cs = model.Input.CharSize;
			foreach (var line in model.Output.Visibles.Array)
			{
				TextRenderer.DrawText(e.Graphics, line.Line, font, line.View, line.Color);
				if (sd != null) Render(e.Graphics, sd, line, selectionBrush, cs);
				if (si != null) Render(e.Graphics, si, line, selectingBrush, cs);
			}
		}

		private void Render(Graphics g, LogModel.Region r, LogLine l, Brush b, Size cs)
        {
			VisitSelection(r, l, (start, length) => {
				var v = l.View;
				v.X += start * cs.Width + cs.Width / 2;
				v.Width = length * cs.Width;
				g.FillRectangle(b, v);
				v.X -= cs.Width / 2;
				var ss = l.Line.Substring(start, length);
				TextRenderer.DrawText(g, ss, font, v.Location, color);
			});
		}

		private void VisitSelection(LogModel.Region r, LogLine l, Action<int, int> callback)
		{
			if (l.Index < r.Start.Line) return;
			if (l.Index > r.End.Line) return;
			var start = 0;
			var length = l.Line.Length;
			if (l.Index == r.Start.Line && l.Index == r.End.Line)
			{
				if (r.Start.Index < 0 && r.End.Index < 0) return;
				if (r.Start.Index >= l.Line.Length && r.End.Index >= l.Line.Length) return;
				start = Math.Max(0, r.Start.Index);
				length = Math.Min(l.Line.Length - 1, r.End.Index) - start + 1;
			}
			else if (l.Index == r.Start.Line)
			{
				if (r.Start.Index >= l.Line.Length) return;
				start = Math.Max(0, r.Start.Index);
				length = l.Line.Length - start;
			}
			else if (l.Index == r.End.Line)
			{
				if (r.End.Index < 0) return;
				start = 0;
				length = Math.Min(l.Line.Length - 1, r.End.Index) + 1;
			}
			callback(start, length);
		}

		protected override void OnCreateControl()
        {
			LogDebug.WriteLine("LogPanel.OnCreateControl");
			base.OnCreateControl();
			QueueInputChange(() => { input.ViewPort = ViewPort(); });
		}

        protected override void OnClientSizeChanged(EventArgs e)
		{
			LogDebug.WriteLine("LogPanel.OnClientSizeChanged");
			base.OnClientSizeChanged(e);
			QueueInputChange(() => { input.ViewPort = ViewPort(); });
		}

		//https://stackoverflow.com/questions/1851620/handling-scroll-event-on-listview-in-c-sharp
		//no practical way to catch scrolling events
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == 0x115 || m.Msg == 0x114 || m.Msg == 0x020A)
			{
				LogDebug.WriteLine("LogPanel.OnScrollChanged");
				QueueInputChange(() => { input.ViewPort = ViewPort(); });
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
        {
            //base.OnMouseDown(e); //prevent click event
			if (e.Button == MouseButtons.Left)
            {
				Capture = true;
				moves = 0;
				click = e.Location;
				ExecuteMouseChange(() => { model.ProcessMouse("down", e.Location, Shift()); });
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			//base.OnMouseDown(e); //prevent click event
			if (Capture && e.Button == MouseButtons.Left)
			{
				moves++;
				ExecuteMouseChange(() => { model.ProcessMouse("move", e.Location, Shift()); });
				var cs = model.Input.CharSize;
				var vp = model.Input.ViewPort;
				if (e.Location.Y < 0)
                {
					VerticalScroll.Value = Math.Max(VerticalScroll.Minimum, VerticalScroll.Value - cs.Height);
					QueueInputChange(() => { input.ViewPort = ViewPort(); });
				}
				if (e.Location.Y > vp.Height)
				{
					VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, VerticalScroll.Value + cs.Height);
					QueueInputChange(() => { input.ViewPort = ViewPort(); });
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
        {
			//base.OnMouseDown(e); //prevent click event
			if (Capture && e.Button == MouseButtons.Left)
            {
				Capture = false;
				//1 move event generated if when not moving the mouse
				var isClick = moves<=1 && click==e.Location;
				if (isClick) ExecuteMouseChange(() => { model.ProcessMouse("click", e.Location, Shift()); });
				else ExecuteMouseChange(() => { model.ProcessMouse("up", e.Location, Shift()); });
			}
		}

		private void ExecuteMouseChange(Action action)
		{
			//received: down, move, click, up
			InitializeModel();
			updates++;
			action();
			LogDebug.WriteLine("LogPanel.ExecuteMouseChange {0} S:{1}", updates, model.Selection);
			var previous = select;
			var current = new LogModel.SelectionState()
			{
				Caret = model.Selection.Caret,
				Selected = model.Selection.Selected,
				Selecting = model.Selection.Selecting,
			};
			if (LogModel.NotEqual(previous, current))
			{
				select = current;
				Invalidate();
			}
		}

		private void QueueInputChange(Action action)
		{
			InitializeModel();
			actions.Enqueue(action);
			if (!executing)
            {
				executing = true;
				while (actions.Count > 0) ExecuteInputChange(actions.Dequeue());
				executing = false;
			}
		}

		private void ExecuteInputChange(Action action)
		{
			updates++;
			action();
			LogDebug.WriteLine("LogPanel.ExecuteInputChange {0} I:{1}", updates, input);
			model.ProcessInput(input);
			LogDebug.WriteLine("LogPanel.ExecuteInputChange {0} O:{1}", updates, model.Output);
			LogDebug.WriteLine("LogPanel.ExecuteInputChange {0} S:{1}", updates, model.Selection);
			var previous = output;
			var current = new LogModel.OutputState()
			{
				Lines = model.Output.Lines,
				Visibles = model.Output.Visibles,
				ScrollSize = model.Output.ScrollSize,
			};
			if (LogModel.NotEqual(previous, current))
			{
				output = current; //cache new output
				var scrollSizeChanged = LogModel.NotEqual(current.ScrollSize, previous.ScrollSize);
				var linesChanged = LogModel.NotEqual(current.Lines, previous.Lines);
				if (scrollSizeChanged)
				{
					LogDebug.WriteLine("LogPanel.scrollSizeChanged");
					AutoScrollMinSize = current.ScrollSize;
					VerticalScroll.Value = VerticalScroll.Maximum;
					HorizontalScroll.Value = 0;
					//redraw vertical scroll bar
					//issues client size change on first call
					//wont issue scroll event
					PerformLayout();
					QueueInputChange(() => { input.ViewPort = ViewPort(); });
				}
				else if (linesChanged)
				{
					LogDebug.WriteLine("LogPanel.linesChanged");
					//wont issue scroll event
					VerticalScroll.Value = VerticalScroll.Maximum;
					HorizontalScroll.Value = 0;
					PerformLayout();
					QueueInputChange(() => { input.ViewPort = ViewPort(); });
				}
				Invalidate();
			}
		}

		private void InitializeModel()
		{
			if (model == null)
			{
				LogDebug.WriteLine("LogPanel.InitializeModel");
				model = new LogModel();
				input = new LogModel.InputState();
				output = new LogModel.OutputState();
				selectionBrush = new SolidBrush(SelectionBack);
				selectingBrush = new SolidBrush(SelectingBack);
				color = SelectionFront;
				actions = new Queue<Action>();
				//Monospace font required because MeasureText is expensive
				//Ubiquitous monospace fonts: Consolas, Courier
				//FontFamily.GenericMonospace creates Courier New
				font = new Font(
					FontFamily.GenericMonospace,
					FontSize,
					FontStyle.Regular,
					GraphicsUnit.Pixel);
				//returns width of an extra character 1=14, 2=21, ...
				var cs = TextRenderer.MeasureText("-", font);
				cs.Width /= 2; //returns 14=2*7 for asigned font size = 12
				input.CharSize = cs;
				input.Lines = new LogModel.Lines(new LogLine[0]);
			}
		}

		private Rectangle ViewPort()
		{
			var offset = AutoScrollPosition;
			offset.X *= -1;
			offset.Y *= -1;
			return new Rectangle(offset, ClientSize);
		}

		private bool Shift()
		{
			return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
		}
	}
}
