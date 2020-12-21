using System;
using System.Drawing;

namespace SharpLogger
{
    public class LogLine
    {
        public string Line { get; set; }
        public Color Color { get; set; }
        public Rectangle Rect { get; set; }
        public Rectangle View { get; set; }
        public Rectangle Row { get; set; }
        public int Index { get; set; }
    }
}
