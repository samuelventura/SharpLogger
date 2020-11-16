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
		private Font font;
		private Pen debug;

		public Color SelectionBack { get; set; } = Color.DodgerBlue;
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

		protected override void OnPaint(PaintEventArgs e)
		{
			var client = ClientSize;
			var offset = AutoScrollPosition;
			var back = new SolidBrush(SelectionBack);
			foreach (var line in lines)
			{
				var rect = new Rectangle(offset, line.Size);
				//draw full row for selected items
				rect.Width = Math.Max(client.Width, HorizontalScroll.Maximum);
				if (e.ClipRectangle.IntersectsWith(rect))
				{
					var color = line.Color;
					if (line.Selected)
                    {
						e.Graphics.FillRectangle(back, rect);
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
			var clicked = FindItem(e.Location);
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
			if (e.Button != MouseButtons.Left) return;
			var captured = Capture;
			Capture = false;
			var captureEnd = e.Location;
			if (captured && captureEnd != captureStart)
			{
				var control = IsControlDown();
				if (!control) ClearSelection();
				var rect = new Rectangle(
					Math.Min(captureStart.X, captureEnd.X),
					Math.Min(captureStart.Y, captureEnd.Y),
					Math.Abs(captureStart.X - captureEnd.X),
					Math.Abs(captureStart.Y - captureEnd.Y));
				foreach (var line in FindItems(rect))
				{
					line.Selected = true;
				}
				Invalidate();
			}
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

		private LogLine[] FindItems(Rectangle selection)
		{
			var list = new List<LogLine>();
			foreach (var line in lines)
			{
				//struct are copied on assigment
				var offset = AutoScrollPosition;
				offset.Offset(line.Location);
				var rect = new Rectangle(offset, line.Size);
				//needs to touch text to select item
				//rect.Width = HorizontalScroll.Maximum;
				if (rect.IntersectsWith(selection))
				{
					list.Add(line);
				}
			}
			return list.ToArray();
		}

		private LogLine FindItem(Point click)
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
	}
}
