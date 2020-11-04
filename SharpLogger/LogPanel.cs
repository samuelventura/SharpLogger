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
			var back = new SolidBrush(Color.DodgerBlue);
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
						color = Color.White;
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
				if (shift)
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
					var count = ClearSelection();
					if (count > 0) Invalidate();
				}
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
				case LogLevel.TRACE:
					return Color.DarkGray;
				case LogLevel.DEBUG:
					return Color.LightGray;
				case LogLevel.INFO:
					return Color.White;
				case LogLevel.WARN:
					return Color.Yellow;
				case LogLevel.ERROR:
					return Color.Tomato;
			}
			throw new Exception($"Invalid level {level}");
		}
	}
}
