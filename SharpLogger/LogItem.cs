using System;
using System.Drawing;

namespace SharpLogger
{
    public class LogItem
    {
        public int Index { get; set; }
        public LogDto Dto { get; set; } = new LogDto();
        public Point Location { get; set; } = new Point();
        public Size Size { get; set; } = new Size();
        public Color Color { get; set; } = Color.Black;
        public bool Selected { get; set; }
    }
}
