using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SharpLogger
{
	public class LogPanel : Panel
	{
		private LogItem[] items = new LogItem[0];
		private LogItem lastClicked;
		private Point captureStart;

		public Color DebugColor { get; set; } = Color.Gray;
		public Color InfoColor { get; set; } = Color.White;
		public Color WarnColor { get; set; } = Color.Yellow;
		public Color ErrorColor { get; set; } = Color.Tomato;
		public Color SelectionBack { get; set; } = Color.DodgerBlue;
		public Color SelectionFront { get; set; } = Color.White;

		public LogPanel()
		{
			BackColor = Color.Black;
			DoubleBuffered = true;
			AutoScroll = true;
		}

		public void SetItems(LogDto[] dtos)
		{
			var items = new List<LogItem>();
			var scroll = new Size(0, 0);
			var client = ClientSize;
			var index = 0;
			foreach (var dto in dtos)
			{
				var item = new LogItem();
				items.Add(item);
				item.Dto = dto;
				item.Location = new Point(0, scroll.Height);
				item.Size = TextRenderer.MeasureText(item.Dto.Message, Font, client);
				if (item.Size.Width > scroll.Width) scroll.Width = item.Size.Width;
				scroll.Height += item.Size.Height;
				item.Color = ToColor(item.Dto.Level);
				item.Index = index++;
			}
			scroll.Height += Font.Height; //renglon extra
			this.items = items.ToArray();
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
			foreach (var item in items)
			{
				if (item.Selected) count++;
			}
			return count;
		}

		public LogDto[] GetAll()
		{
			var list = new List<LogDto>();
			foreach (var item in items)
			{
				list.Add(item.Dto);
			}
			return list.ToArray();
		}

		public LogDto[] GetSelected()
        {
			var list = new List<LogDto>();
			foreach (var item in items)
			{
				if (item.Selected) list.Add(item.Dto);
			}
			return list.ToArray();
        }

		protected override void OnPaint(PaintEventArgs e)
		{
			var client = ClientSize;
			var offset = AutoScrollPosition;
			var back = new SolidBrush(SelectionBack);
			foreach (var item in items)
			{
				var rect = new Rectangle(offset, item.Size);
				//draw full row for selected items
				rect.Width = Math.Max(client.Width, HorizontalScroll.Maximum);
				if (e.ClipRectangle.IntersectsWith(rect))
				{
					var color = item.Color;
					if (item.Selected)
                    {
						e.Graphics.FillRectangle(back, rect);
						color = SelectionFront;
					}
					TextRenderer.DrawText(e.Graphics, item.Dto.Message, 
						Font, offset, color);
				}
				offset.Y += item.Size.Height;
			}
		}

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);			
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
						items[i].Selected = true;
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
			Capture = true;
			captureStart = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
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
				foreach (var item in FindItems(rect))
				{
					item.Selected = true;
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
			foreach (var item in items)
			{
				if (item.Selected) count++;
				item.Selected = false;
			}
			return count;
		}

		private LogItem[] FindItems(Rectangle selection)
		{
			var list = new List<LogItem>();
			foreach (var item in items)
			{
				//struct are copied on assigment
				var offset = AutoScrollPosition;
				offset.Offset(item.Location);
				var rect = new Rectangle(offset, item.Size);
				//needs to touch text to select item
				//rect.Width = HorizontalScroll.Maximum;
				if (rect.IntersectsWith(selection))
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		private LogItem FindItem(Point click)
        {
			foreach (var item in items)
			{
				//struct are copied on assigment
				var offset = AutoScrollPosition;
				offset.Offset(item.Location);
				var rect = new Rectangle(offset, item.Size);
				//needs to click text to select item
				//rect.Width = HorizontalScroll.Maximum;
				if (rect.Contains(click))
                {
					return item;
                }
			}
			return null;
		}

		private Color ToColor(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.DEBUG:
					return DebugColor;
				case LogLevel.INFO:
					return InfoColor;
				case LogLevel.WARN:
					return WarnColor;
				case LogLevel.ERROR:
					return ErrorColor;
			}
			throw new Exception($"Invalid level {level}");
		}
	}
}
