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

		public Color SelectionBack { get; set; } = Color.SteelBlue;
		public Color SelectingBack { get; set; } = Color.LightSteelBlue;
		public Color SelectionFront { get; set; } = Color.White;

		//Monospace font required because MeasureText is expensive
		//Ubiquitous monospace fonts: Consolas, Courier
		//FontFamily.GenericMonospace creates Courier New
		public float FontSize { 
			get { return font.Size; }
			set { font = new Font(FontFamily.GenericMonospace, 
				value, FontStyle.Regular, GraphicsUnit.Pixel); }
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
			//returns width of an extra character 1=14, 2=21, ...
			var single = TextRenderer.MeasureText("-", font);
			single.Width /= 2; //returns 14 for asigned font size = 12
			foreach (var line in lines)
			{
				line.Selecting = false;
				line.Location = new Point(0, scroll.Height);
				//Expects single text line per log line
				//Monospace font required because MeasureText is expensive
				//line.Size = TextRenderer.MeasureText(line.Line, font);
				line.Size = new Size((line.Line.Length + 1) * single.Width, single.Height);
				if (line.Size.Width > scroll.Width) scroll.Width = line.Size.Width;
				scroll.Height += line.Size.Height;
				line.Index = index++;
			}
			scroll.Height += single.Height; //renglon extra
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
			foreach (var line in lines)
			{
				//struct are copied on assigment
				var offset = AutoScrollPosition;
				offset.Offset(line.Location);
				var rect = new Rectangle(offset, line.Size);
				//select even from trailing empty space or
				//unexpected selection pops up on triangle text
				rect.Width = HorizontalScroll.Maximum;
				callback(line, rect.IntersectsWith(selection));
			}
		}

		private LogLine Contains(Point click)
		{
			foreach (var line in lines)
			{
				//struct are copied on assigment
				var offset = AutoScrollPosition;
				offset.Offset(line.Location);
				var rect = new Rectangle(offset, line.Size);
				//needs to click text to select line
				//rect.Width = HorizontalScroll.Maximum;
				if (rect.Contains(click))
				{
					return line;
				}
			}
			return null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var client = ClientSize;
			var offset = AutoScrollPosition;
			var selection = new SolidBrush(SelectionBack);
			var selecting = new SolidBrush(SelectingBack);
			foreach (var line in lines)
			{
				var rect = new Rectangle(offset, line.Size);
				//draw full row for selected items
				rect.Width = Math.Max(client.Width, HorizontalScroll.Maximum);
				if (e.ClipRectangle.IntersectsWith(rect))
				{
					var color = line.Color;
					if (Capture && line.Selecting)
                    {
						e.Graphics.FillRectangle(selecting, rect);
						color = SelectionFront;
					}
					else if (line.Selected)
                    {
						e.Graphics.FillRectangle(selection, rect);
						color = SelectionFront;
					}
					TextRenderer.DrawText(e.Graphics, line.Line, font, offset, color);
					if (debug != null)
                    {
						e.Graphics.DrawRectangle(debug, offset.X, offset.Y, line.Size.Width, line.Size.Height);
					}
				}
				offset.Y += line.Size.Height;
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
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!Capture) return;
			var rect = new Rectangle(
				Math.Min(captureStart.X, e.Location.X),
				Math.Min(captureStart.Y, e.Location.Y),
				Math.Abs(captureStart.X - e.Location.X),
				Math.Abs(captureStart.Y - e.Location.Y));
			Intersect(rect, (line, intersect) => {
				line.Selecting = intersect;
			});
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
			if (!Capture) return;
			Capture = false;
			if (e.Button != MouseButtons.Left) return;
			if (e.Location == captureStart) return;
			var control = IsControlDown();
			var rect = new Rectangle(
				Math.Min(captureStart.X, e.Location.X),
				Math.Min(captureStart.Y, e.Location.Y),
				Math.Abs(captureStart.X - e.Location.X),
				Math.Abs(captureStart.Y - e.Location.Y));
			Intersect(rect, (line, intersect) => {
				if (!control) line.Selected = false;
				if (intersect) line.Selected = true;
				line.Selecting = false;
			});
			Invalidate();
		}
	}
}
