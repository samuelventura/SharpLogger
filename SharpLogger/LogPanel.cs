using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace SharpLogger
{
	public class LogPanel : Panel
	{
		private ulong updates;
		private LogModel.OutputState output;
		private LogModel.InputState input;
		private LogModel model;
		private Brush selection;
		private Brush selecting;
		private Font font;

		//design time only
		public float FontSize { get; set; } = 12;
		public Color SelectionBack { get; set; } = Color.SteelBlue;
		public Color SelectingBack { get; set; } = Color.LightSteelBlue;
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
			UpdateModel(() => {
				input.Lines = new LogModel.Lines(lines);
			});
		}

		public string SelectedText()
        {
			InitializeModel();
			var sb = new StringBuilder();
			foreach (var line in input.Lines.Array)
			{
				sb.AppendLine(line.Line);
			}
			return sb.ToString();
		}

        protected override void OnCreateControl()
        {
			LogDebug.WriteLine("LogPanel.OnCreateControl");
			base.OnCreateControl();
			UpdateModel(() => {
				input.ViewPort = ViewPort();
			});
		}

        protected override void OnClientSizeChanged(EventArgs e)
		{
			LogDebug.WriteLine("LogPanel.OnClientSizeChanged");
			base.OnClientSizeChanged(e);
			UpdateModel(() => {
				input.ViewPort = ViewPort();
			});
		}

		//https://stackoverflow.com/questions/1851620/handling-scroll-event-on-listview-in-c-sharp
		//no practical way to catch scrolling events
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (m.Msg == 0x115 || m.Msg == 0x114 || m.Msg == 0x020A)
			{
				LogDebug.WriteLine("LogPanel.OnScrollChanged");
				UpdateModel(() => {
					input.ViewPort = ViewPort();
				});
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			LogDebug.WriteLine("LogPanel.OnPaint {0}", ViewPort());
			foreach (var line in model.Output.Visibles.Array)
			{
				e.Graphics.FillRectangle(Brushes.Black, line.Row);
				TextRenderer.DrawText(e.Graphics, line.Line, font, line.View, Color.White);
			}
		}

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
			if (e.Button == MouseButtons.Left)
			{
				UpdateModel(() => {
					model.ProcessMouse("click", e.Location, Shift());
				});
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
			if (e.Button == MouseButtons.Left)
            {
				Capture = true;
				UpdateModel(() => {
					model.ProcessMouse("down", e.Location, Shift());
				});
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (Capture)
			{
				UpdateModel(() => {
					model.ProcessMouse("move", e.Location, Shift());
				});
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
        {
			base.OnMouseUp(e);
			if (Capture && e.Button != MouseButtons.Left)
            {
				Capture = false;
				UpdateModel(() => {
					model.ProcessMouse("up", e.Location, Shift());
				});
			}
		}

		private Rectangle ViewPort()
		{
			var offset = AutoScrollPosition;
			offset.X *= -1;
			offset.Y *= -1;
			return new Rectangle(offset, ClientSize);
		}

		private void UpdateModel(Action action)
		{
			//reentrant code, ensure local references to fields
			//reentrant is good to minimize redraws
			var index = updates++;
			InitializeModel();
			action();
			LogDebug.WriteLine("LogPanel.UpdateModel {0} I:{1}", index, input);
			model.ProcessInput(input);
			LogDebug.WriteLine("LogPanel.UpdateModel {0} O:{1}", index, model.Output);
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
					AutoScrollMinSize = current.ScrollSize;
					VerticalScroll.Value = VerticalScroll.Maximum;
					HorizontalScroll.Value = 0;
					//redraw vertical scroll bar
					//issues client size change on first call
					//wont issue scroll event
					PerformLayout(); 
					UpdateModel(() => {
						input.ViewPort = ViewPort();
					});
				}
				else if(linesChanged)
                {
					LogDebug.WriteLine("LogPanel.linesChanged");
					//wont issue scroll event
					VerticalScroll.Value = VerticalScroll.Maximum;
					HorizontalScroll.Value = 0;
					UpdateModel(() => {
						input.ViewPort = ViewPort();
					});
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
				selection = new SolidBrush(SelectionBack);
				selecting = new SolidBrush(SelectingBack);
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

		private bool Shift()
		{
			return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
		}
	}
}
