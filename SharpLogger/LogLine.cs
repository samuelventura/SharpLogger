using System;
using System.Drawing;

namespace SharpLogger
{
    public class LogLine
    {
        public LogDto Dto { get; set; } = new LogDto();
        public int Index { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Line { get; set; } = string.Empty;
        public Point Location { get; set; } = new Point();
        public Size Size { get; set; } = new Size();
        public Color Color { get; set; } = Color.Black;
        public bool Selected { get; set; }
        public bool Selecting { get; set; }
        public bool Partialing { get; set; }
        public bool Partial { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Substring { get; set; }
    }
}
