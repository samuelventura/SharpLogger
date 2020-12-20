using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpLogger
{
	public class LogPanel : Panel
	{
		private LogLine[] lines = new LogLine[0];
		private LogLine lastClicked;
		private Point captureStart;
		private Font font = null;
		private Pen debug = null;
		private Size charSize;

		public Color SelectionBack { get; set; } = Color.SteelBlue;
		public Color SelectingBack { get; set; } = Color.LightSteelBlue;
		public Color SelectionFront { get; set; } = Color.White;

		//Monospace font required because MeasureText is expensive
		//Ubiquitous monospace fonts: Consolas, Courier
		//FontFamily.GenericMonospace creates Courier New
		public float FontSize { 
			get { return font.Size; }
			set { 
				font = new Font(FontFamily.GenericMonospace, 
				value, FontStyle.Regular, GraphicsUnit.Pixel);
				//returns width of an extra character 1=14, 2=21, ...
				charSize = TextRenderer.MeasureText("-", font);
				charSize.Width /= 2; //returns 14 for asigned font size = 12
			}
		}

		public LogPanel()
		{
			//debug = new Pen(Color.Purple);
			BackColor = Color.Black;
			DoubleBuffered = true;
			AutoScroll = true;
			FontSize = 12;
		}

		public void SetLines(params LogLine[] lines)
		{
			this.lines = lines;
			var scroll = new Size(0, 0);
			var index = 0;
			foreach (var line in lines)
			{
				line.Selecting = false;
				line.Location = new Point(0, scroll.Height);
				//Expects single text line per log line
				//Monospace font required because MeasureText is expensive
				//line.Size = TextRenderer.MeasureText(line.Line, font);
				line.Size = new Size((line.Line.Length + 1) * charSize.Width, charSize.Height);
				if (line.Size.Width > scroll.Width) scroll.Width = line.Size.Width;
				scroll.Height += line.Size.Height;
				line.Index = index++;
			}
			scroll.Height += charSize.Height; //renglon extra
			AutoScrollMinSize = scroll;
			VerticalScroll.Value = VerticalScroll.Maximum;
			HorizontalScroll.Value = 0;
			lastClicked = null;
			Capture = false;
			PerformLayout(); //redraw vertical scroll bar
			Invalidate(); //relayout does not invalidate
		}

		public int CountSelected()
		{
			var count = 0;
			foreach (var item in lines)
			{
				if (item.Selected) count++;
			}
			return count;
		}

		public LogLine[] GetAll()
		{
			var list = new List<LogLine>();
			foreach (var line in lines)
			{
				list.Add(line);
			}
			return list.ToArray();
		}

		public LogLine[] GetSelected()
        {
			var list = new List<LogLine>();
			foreach (var line in lines)
			{
				if (line.Selected) list.Add(line);
			}
			return list.ToArray();
		}

		private bool IsShiftDown()
		{
			return (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
		}

		private bool IsControlDown()
		{
			return (Control.ModifierKeys & Keys.Control) == Keys.Control;
		}

		private int ClearSelection()
		{
			var count = 0;
			foreach (var line in lines)
			{
				if (line.Selected) count++;
				line.Selected = false;
			}
			return count;
		}

		private void Intersect(Rectangle selection, Action<LogLine, bool> callback)
		{
			var client = ClientSize;
			foreach (var line in lines)
			{
				var rect = LineRect(line);
				//select even from trailing empty space or
				//unexpected selection pops up on triangle text
				rect.Width = Math.Max(client.Width, HorizontalScroll.Maximum);
				callback(line, rect.IntersectsWith(selection));
			}
		}

		private LogLine Contains(Point click)
		{
			foreach (var line in lines)
			{
				//needs to click text to select line
				if (LineRect(line).Contains(click))
				{
					return line;
				}
			}
			return null;
		}

		private Rectangle LineRect(LogLine line)
		{
			//structs are copied on assigment
			var offset = AutoScrollPosition;
			offset.Offset(line.Location);
			return new Rectangle(offset, line.Size);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var client = ClientSize;
			var selection = new SolidBrush(SelectionBack);
			var selecting = new SolidBrush(SelectingBack);
			foreach (var line in lines)
			{
				var rect = LineRect(line);
				//draw full row for selected items
				rect.Width = Math.Max(client.Width, HorizontalScroll.Maximum);
				//TextRenderer renders leaving width/2 at each side using length+1 spaces
				if (e.ClipRectangle.IntersectsWith(rect))
				{
					if (Capture && line.Selecting)
                    {
						if (line.Partialing)
                        {
							TextRenderer.DrawText(e.Graphics, line.Line, font, rect.Location, line.Color);
							rect.X += line.Start * charSize.Width + charSize.Width/2;
							rect.Width = line.Length * charSize.Width;
							e.Graphics.FillRectangle(selecting, rect);
							rect.X -= charSize.Width/2;
							TextRenderer.DrawText(e.Graphics, line.Substring, font, rect.Location, SelectionFront);
						}
						else
						{
							e.Graphics.FillRectangle(selecting, rect);
							TextRenderer.DrawText(e.Graphics, line.Line, font, rect.Location, SelectionFront);
						}
					}
					else if (line.Selected)
                    {
						if (line.Partial)
						{
							TextRenderer.DrawText(e.Graphics, line.Line, font, rect.Location, line.Color);
							rect.X += line.Start * charSize.Width + charSize.Width/2;
							rect.Width = line.Length * charSize.Width;
							e.Graphics.FillRectangle(selection, rect);
							rect.X -= charSize.Width/2;
							TextRenderer.DrawText(e.Graphics, line.Substring, font, rect.Location, SelectionFront);
						}
						else
                        {
							e.Graphics.FillRectangle(selection, rect);
							TextRenderer.DrawText(e.Graphics, line.Line, font, rect.Location, SelectionFront);
						}
					}
					else TextRenderer.DrawText(e.Graphics, line.Line, font, rect.Location, line.Color);

					if (debug != null)
                    {
						e.Graphics.DrawRectangle(debug, LineRect(line));
					}
				}
			}
		}

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
			if (e.Button != MouseButtons.Left) return;
			var clicked = Contains(e.Location);
			var control = IsControlDown();
			var shift = IsShiftDown();
			if (clicked != null)
			{
				if (shift && lastClicked != null)
                {
					var start = Math.Min(clicked.Index, lastClicked.Index);
					var end = Math.Max(clicked.Index, lastClicked.Index);
					for (var i = start; i <= end; i++)
					{
						lines[i].Selected = true;
					}
				}
				else if (control)
                {
					clicked.Selected = !clicked.Selected;
				}
				else
                {
					ClearSelection();
					clicked.Selected = true;
				}
				lastClicked = clicked;
				Invalidate();
			}
			else
			{
				//ctrl & shift means trying
				//to add so ignore clear 
				if (!shift && !control)
                {
					lastClicked = null;
					var count = ClearSelection();
					if (count > 0) Invalidate();
				}
			}
		}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left) return;
			Capture = true;
			captureStart = e.Location;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!Capture) return;
			var control = IsControlDown();
			var rect = new Rectangle(
				Math.Min(captureStart.X, e.Location.X),
				Math.Min(captureStart.Y, e.Location.Y),
				Math.Abs(captureStart.X - e.Location.X),
				Math.Abs(captureStart.Y - e.Location.Y));
			var count = 0;
			var single = null as LogLine;
			Intersect(rect, (line, intersect) => {
				line.Selecting = intersect;
				if (line.Selecting) single = line;
				if (line.Selecting) count++;
				line.Partialing = false;
			});
			if (count != 1) single = null;
			if (control) single = null;
			if (single != null) Partialing(single, captureStart, e.Location);
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
        {
			base.OnMouseUp(e);
			if (e.Button != MouseButtons.Left) return;
			if (!Capture) return;
			Capture = false;
			if (e.Location == captureStart) return;
			var control = IsControlDown();
			var rect = new Rectangle(
				Math.Min(captureStart.X, e.Location.X),
				Math.Min(captureStart.Y, e.Location.Y),
				Math.Abs(captureStart.X - e.Location.X),
				Math.Abs(captureStart.Y - e.Location.Y));
			var count = 0;
			var single = null as LogLine;
			Intersect(rect, (line, intersect) => {
				if (!control) line.Selected = false;
				if (intersect) line.Selected = true;
				line.Partialing = false;
				line.Selecting = false;
				if (line.Selected) count++;
				if (line.Selected) single = line;
			});
			if (count != 1) single = null;
			if (control) single = null;
			if (single != null) Partial(single, captureStart, e.Location);
			Invalidate();
		}

		private void Partialing(LogLine l, Point p1, Point p2)
		{
			l.Partialing = Calculate(l, p1, p2);
		}

		private void Partial(LogLine l, Point p1, Point p2)
		{
			l.Partial = Calculate(l, p1, p2);
		}

		private bool Calculate(LogLine l, Point p1, Point p2)
		{
			var s = charSize;
			var r = LineRect(l);
			if (p1.Y < r.Top || p1.Y > r.Bottom) return false;
			if (p2.Y < r.Top || p2.Y > r.Bottom) return false;
			var x1 = Math.Min(p1.X, p2.X);
			var x2 = Math.Max(p1.X, p2.X);
			var d1 = x1 - r.X - s.Width / 2.0;
			var d2 = x2 - r.X - s.Width / 2.0;
			var i1 = (int)Math.Floor(d1 / s.Width);
			var i2 = (int)Math.Floor(d2 / s.Width);
			var len = l.Line.Length;
			if (i1 < 0 && i2 < 0) return false;
			if (i1 >= len && i2 >= len) return false;
			l.Start = Math.Min(len - 1, Math.Max(0, i1));
			var end = Math.Min(len - 1, Math.Max(0, i2));
			l.Length = end - l.Start + 1;
			l.Substring = l.Line.Substring(l.Start, l.Length);
			return true;
		}
	}
}
