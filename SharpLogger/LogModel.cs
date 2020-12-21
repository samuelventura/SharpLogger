using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpLogger
{
    public class LogModel
    {
        private readonly InnerState inner = new InnerState();

        public InputState Input { get; } = new InputState();
        public OutputState Output { get; } = new OutputState();
        public SelectionState Selection { get; } = new SelectionState();

        public void ProcessInput(InputState input)
        {
            LogDebug.WriteLine("LogModel.ProcessInput");
            var linesChanged = NotEqual(Input.Lines, input.Lines);
            var viewPortChanged = NotEqual(Input.ViewPort, input.ViewPort);
            var charSizeChanged = NotEqual(Input.CharSize, input.CharSize);
            Input.Lines = input.Lines;
            Input.ViewPort = input.ViewPort;
            Input.CharSize = input.CharSize;
            if (linesChanged) InitializeLines();
            if (linesChanged || charSizeChanged) RecalculateScrollSize();
            if (linesChanged || charSizeChanged || viewPortChanged) RecalculateVisibles();
        }

        public void ProcessMouse(string name, Point pointer, bool shift)
        {
            LogDebug.WriteLine("LogModel.ProcessMouse {0} {1} {2}", name, pointer, shift);
            var caret = FindCaret(pointer);
            switch (name)
            {
                case "down":
                    Selection.Caret = caret;
                    inner.Last = caret;
                    return;
                case "move":
                    Selection.Caret = caret;
                    if (inner.Last != null)
                    {
                        var last = inner.Last;
                        Selection.Selecting = new Region()
                        {
                            End = caret,
                            Start = last,
                        };
                    }
                    return;
                case "up":
                    Selection.Caret = caret;
                    if (inner.Last != null)
                    {
                        var last = inner.Last;
                        Selection.Selected = new Region()
                        {
                            End = caret,
                            Start = last,
                        };
                    }
                    return;
                case "click":
                    Selection.Caret = caret;
                    if (inner.Last != null)
                    {
                        var last = inner.Last;
                        Selection.Selected = new Region()
                        {
                            End = caret,
                            Start = last,
                        };
                    }
                    inner.Last = caret;
                    return;
            }
            throw new Exception($"Invalid mouser event {name}");
        }

        private void InitializeLines()
        {
            LogDebug.WriteLine("LogModel.InitializeLines");
            var index = 0;
            foreach (var line in Input.Lines.Array)
            {
                line.Index = index++;
            }
            inner.Last = null;
            Output.Lines = Input.Lines;
            Output.Visibles = new Lines();
            Output.ScrollSize = new Size();
            Selection.Selected = null;
            Selection.Selecting = null;
            Output.Visibles = null;
            Selection.Caret = new Caret()
            {
                Line = index,
            };
        }

        private void RecalculateScrollSize()
        {
            LogDebug.WriteLine("LogModel.RecalculateScrollSize");
            var cs = Input.CharSize;
            var ss = new Size(0, 0);
            foreach (var l in Input.Lines.Array)
            {
                var width = (l.Line.Length + 1) * cs.Width;
                if (width > ss.Width) ss.Width = width;
                ss.Height += cs.Height;
            }
            ss.Height += cs.Height; //extra row 
            Output.ScrollSize = ss;
        }

        private void RecalculateVisibles()
        {
            LogDebug.WriteLine("LogModel.RecalculateVisibles");
            var cs = Input.CharSize;
            var vp = Input.ViewPort;
            var start = vp.Y / cs.Height;
            var end = vp.Bottom / cs.Height;
            var visibles = new List<LogLine>();
            foreach (var l in Input.Lines.Array)
            {
                var index = l.Index;
                if ((index >= start && index <= end))
                {
                    visibles.Add(l);
                    var width = (l.Line.Length + 1) * cs.Width;
                    var y = index * cs.Height;
                    var location = new Point(0, y);
                    var size = new Size(width, cs.Height);
                    l.Rect = new Rectangle(location, size);
                    location.X -= vp.X;
                    location.Y -= vp.Y;
                    l.View = new Rectangle(location, size);
                    location.X = 0;
                    size.Width = vp.Width;
                    l.Row = new Rectangle(location, size);
                }
            }
            Output.Visibles = new Lines(visibles.ToArray());
        }

        private Caret FindCaret(Point pointer)
        {
            var line = FindLine(pointer);
            if (line != null)
            {
                var s = Input.CharSize;
                var r = line.View;
                var p = pointer;
                var d = p.X - r.X - s.Width / 2.0;
                var i = (int)Math.Floor(d / s.Width);
                return new Caret()
                {
                    Line = line.Index,
                    Index = i,
                };
            }
            return new Caret()
            {
                Line = Input.Lines.Array.Length,
                Index = 0,
            };
        }

        private LogLine FindLine(Point pointer)
        {
            foreach (var line in Output.Visibles.Array)
            {
                if (line.Row.Contains(pointer)) return line;
            }
            return null;
        }

        public static bool NotEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null) return false;
            if (obj1 == null) return true;
            if (obj2 == null) return true;
            if (obj1.ToString() == obj2.ToString()) return false;
            return true;
        }

        public class Lines
        {
            public LogLine[] Array { get; }
            public Lines(params LogLine[] array) { Array = array; }
            public override string ToString()
            {
                return $"{Array.Length}:{GetHashCode()}";
            }
        }
        public class InputState
        {
            public Lines Lines { get; set; }
            public Rectangle ViewPort { get; set; }
            public Size CharSize { get; set; }
            public override string ToString()
            {
                return $"{Lines}:{ViewPort}:{CharSize}";
            }
        }
        public class OutputState
        {
            public Lines Lines { get; set; }
            public Size ScrollSize { get; set; }
            public Lines Visibles { get; set; }
            public override string ToString()
            {
                return $"{Visibles}:{ScrollSize}:{Lines}";
            }
        }
        public class SelectionState
        {
            public Caret Caret { get; set; }
            public Region Selected { get; set; }
            public Region Selecting { get; set; }
            public override string ToString()
            {
                return $"{Caret}:{Selected}:{Selecting}";
            }
        }
        public class InnerState
        {
            public Caret Last { get; set; }
        }
        public class Caret
        {
            public int Line;
            public int Index;
            public override string ToString()
            {
                return $"{Line}:{Index}";
            }
        }
        public class Region
        {
            public Caret Start;
            public Caret End;
            public override string ToString()
            {
                return $"{Start}:{End}";
            }
        }
    }
}