﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace SharpLogger
{
    public class LogModel
    {
        private static readonly LogDebug debugger = new LogDebug(typeof(LogModel).Name);

        private readonly InnerState inner = new InnerState();

        public OutputState Output { get; } = new OutputState();

        public void ProcessInput(InputState input)
        {
            debugger.WriteLine("LogModel.ProcessInput");
            var linesChanged = NotEqual(Output.Lines, input.Lines);
            var viewPortChanged = NotEqual(Output.ViewPort, input.ViewPort);
            var charSizeChanged = NotEqual(Output.CharSize, input.CharSize);
            Output.Lines = input.Lines;
            Output.ViewPort = input.ViewPort;
            Output.CharSize = input.CharSize;
            if (linesChanged) InitializeLines();
            if (linesChanged || charSizeChanged) RecalculateScrollSize();
            if (linesChanged || charSizeChanged || viewPortChanged) RecalculateVisibles();
        }

        public void ProcessMouse(string name, Point pointer, bool shift, bool control)
        {
            debugger.WriteLine("LogModel.ProcessMouse {0} {1} {2} {3}", name, pointer, shift, control);
            var caret = FindCaret(pointer);
            switch (name)
            {
                case "down":
                    inner.Last = caret;
                    return;
                case "move":
                    if (inner.Last != null)
                    {
                        var last = inner.Last;
                        Output.Selecting = new Region()
                        {
                            End = Max(caret, last),
                            Start = Min(caret, last),
                        };
                    }
                    return;
                case "up":
                    if (inner.Last != null)
                    {
                        var last = inner.Last;
                        Output.Selecting = null;
                        Output.Selected = new Region()
                        {
                            End = Max(caret, last),
                            Start = Min(caret, last),
                        };
                    }
                    return;
                case "click":
                    inner.Last = null;
                    Output.Selecting = null;
                    Output.Selected = null;
                    return;
                case "dclick":
                    inner.Last = null;
                    Output.Selecting = null;
                    Output.Selected = Word(caret, shift, control);
                    return;
            }
            throw new Exception($"Invalid mouse event {name}");
        }

        private void InitializeLines()
        {
            debugger.WriteLine("LogModel.InitializeLines");
            var index = 0;
            foreach (var line in Output.Lines.Array)
            {
                line.Index = index++;
            }
            inner.Last = null;
            Output.Lines = Output.Lines;
            Output.Visibles = new Lines();
            Output.ViewPort = new Rectangle(new Point(), Output.ViewPort.Size);
            Output.ScrollSize = new Size();
            Output.Selected = null;
            Output.Selecting = null;
            Output.Visibles = null;
        }

        private void RecalculateScrollSize()
        {
            debugger.WriteLine("LogModel.RecalculateScrollSize");
            var cs = Output.CharSize;
            var ss = new Size(0, 0);
            foreach (var l in Output.Lines.Array)
            {
                var width = (l.Line.Length + 1) * cs.Width;
                if (width > ss.Width) ss.Width = width;
                ss.Height += cs.Height;
            }
            //extra row only if not empty
            if (ss.Height > 0) ss.Height += cs.Height;
            Output.ScrollSize = ss;
            //scroll to botton by default, few lines should show on top
            var y = Math.Max(0, ss.Height - Output.ViewPort.Height);
            Output.ViewPort = new Rectangle(new Point(0, y), Output.ViewPort.Size);
        }

        private void RecalculateVisibles()
        {
            debugger.WriteLine("LogModel.RecalculateVisibles");
            var cs = Output.CharSize;
            var vp = Output.ViewPort;
            var visibles = new List<LogLine>();
            var lines = Output.Lines.Array;
            var start = Math.Max(0, vp.Y / cs.Height);
            var end = Math.Min(lines.Length - 1, vp.Bottom / cs.Height);
            for (var i = start; i <= end; i++)
            {
                var l = lines[i];
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

        private Region Word(Caret caret, bool shift, bool control)
        {
            var lines = Output.Lines.Array;
            if (caret.Line >= 0 && caret.Line < lines.Length)
            {
                var line = lines[caret.Line].Line;
                //shift -> line selection
                //control -> ignore punctuation
                if (!shift && caret.Index >= 0 && caret.Index < line.Length)
                {
                    if (!char.IsWhiteSpace(line[caret.Index]))
                    {
                        var start = caret.Index - 1;
                        while (true)
                        {
                            if (start < 0) break;
                            var cs = line[start];
                            if (char.IsWhiteSpace(cs)) break;
                            if (!control && char.IsPunctuation(cs)) break;
                            start--;
                        }
                        var end = caret.Index + 1;
                        while (true)
                        {
                            if (end > line.Length - 1) break;
                            var cs = line[end];
                            if (char.IsWhiteSpace(cs)) break;
                            if (!control && char.IsPunctuation(cs)) break;
                            end++;
                        }
                        return new Region()
                        {
                            Start = new Caret()
                            {
                                Line = caret.Line,
                                Index = start + 1,
                            },
                            End = new Caret()
                            {
                                Line = caret.Line,
                                Index = end - 1,
                            },
                        };
                    }
                }
                return new Region()
                {
                    Start = new Caret()
                    {
                        Line = caret.Line,
                        Index = 0,
                    },
                    End = new Caret()
                    {
                        Line = caret.Line,
                        Index = line.Length - 1,
                    },
                };
            }
            return new Region() { Start = caret, End = caret, };
        }

        private Caret FindCaret(Point pointer)
        {
            var cs = Output.CharSize;
            var vp = Output.ViewPort;
            var start = vp.Y / cs.Height;
            var end = vp.Bottom / cs.Height;
            var line = FindLine(pointer);
            if (line != null)
            {
                var s = Output.CharSize;
                var r = line.View;
                var p = pointer;
                var d = p.X - r.X - s.Width / 2.0;
                var i = (int)Math.Floor(d / s.Width);
                return new Caret()
                {
                    Line = line.Index,
                    Index = Math.Min(line.Line.Length, Math.Max(i, -1)),
                };
            }
            var visibles = Output.Visibles.Array;
            if (visibles.Length > 0)
            {
                var first = visibles[0];
                var last = visibles[visibles.Length - 1];
                if (pointer.Y < 0) return new Caret()
                {
                    Line = first.Index,
                    Index = 0,
                };
                if (pointer.Y >= vp.Height) return new Caret()
                {
                    Line = last.Index,
                    Index = last.Line.Length,
                };
            }
            return new Caret()
            {
                Line = Output.Lines.Array.Length,
                Index = 0,
            };
        }

        private LogLine FindLine(Point pointer)
        {
            foreach (var line in Output.Visibles.Array)
            {
                pointer.X = line.Row.X; //keep it inside
                if (line.Row.Contains(pointer)) return line;
            }
            return null;
        }

        private Caret Min(Caret c1, Caret c2)
        {
            if (c1.Line > c2.Line) return c2;
            if (c1.Line < c2.Line) return c1;
            if (c1.Index > c2.Index) return c2;
            return c1;
        }

        private Caret Max(Caret c1, Caret c2)
        {
            if (c1.Line > c2.Line) return c1;
            if (c1.Line < c2.Line) return c2;
            if (c1.Index > c2.Index) return c1;
            return c2;
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
            private static ulong index;
            public ulong Index { get; }
            public LogLine[] Array { get; }
            public Lines(params LogLine[] array) { Array = array; Index = index++; }
            public override string ToString()
            {
                return $"{Array.Length}:{Index}";
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
            public Lines Lines { get; set; } = new Lines();
            public Size ScrollSize { get; set; }
            public Lines Visibles { get; set; }
            public Rectangle ViewPort { get; set; }
            public Size CharSize { get; set; }
            public Region Selected { get; set; }
            public Region Selecting { get; set; }
            public override string ToString()
            {
                return $"ls:{Lines},vs:{Visibles},ss:{ScrollSize},vp:{ViewPort},cs:{CharSize},sd:{Selected},si:{Selecting}";
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
                return $"{{{Line},{Index}}}";
            }
        }
        public class Region
        {
            public Caret Start;
            public Caret End;
            public override string ToString()
            {
                return $"{{{Start},{End}}}";
            }
        }
    }
}