using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace SharpLogger
{
    public class LogPanel : Panel
    {
        private static readonly LogDebug debugger = new LogDebug(typeof(LogPanel).Name);

        private const int WS_VSCROLL = 0x00200000;
        private const int WS_HSCROLL = 0x00100000;
        private const int WM_VSCROLL = 0x115;
        private const int WM_HSCROLL = 0x114;
        private const int WM_MOUSEWHEEL = 0x020A;

        private bool dirty;
        private ulong moves;
        private Point click;
        private ulong updates;
        private bool executing;
        private Queue<Action> actions;
        private LogModel.OutputState output;
        private LogModel.InputState input;
        private Brush selectionBrush;
        private Brush selectingBrush;
        private LogModel model;
        private Color color;
        private Font font;

        //design time only
        [Category("Logging")]
        public float FontSize { get; set; } = 12;
        [Category("Logging")]
        public Color SelectionBack { get; set; } = Color.SteelBlue;
        [Category("Logging")]
        public Color SelectingBack { get; set; } = Color.LightSteelBlue;
        [Category("Logging")]
        public Color SelectionFront { get; set; } = Color.White;

        //https://stackoverflow.com/questions/3562235/panel-not-getting-focus
        public LogPanel()
        {
            SetStyle(ControlStyles.Selectable, true);
            BackColor = Color.Black;
            DoubleBuffered = true;
            AutoScroll = true;
            TabStop = true;
        }

        public void SetLines(params LogLine[] lines)
        {
            debugger.WriteLine("LogPanel.SetLines {0}", lines.Length);
            QueueModelChange(() =>
            {
                debugger.WriteLine("LogPanel.LinesUpdate {0}", lines.Length);
                input.Lines = new LogModel.Lines(lines);
                dirty = true;
            });
        }

        public string SelectedText()
        {
            InitializeModel();
            var sb = new StringBuilder();
            var sd = model.Output.Selected;
            if (sd != null)
            {
                foreach (var line in input.Lines.Array)
                {
                    VisitSelection(sd, line, (start, length) =>
                    {
                        if (sb.Length > 0) sb.AppendLine();
                        sb.Append(line.Line.Substring(start, length));
                    });
                }
                if (sb.Length > 0) return sb.ToString();
            }
            foreach (var line in input.Lines.Array)
            {
                sb.AppendLine(line.Line);
            }
            return sb.ToString();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            debugger.WriteLine("LogPanel.OnPaint {0} {1}", actions.Count == 0, actions.Count);
            var si = model.Output.Selecting;
            var sd = model.Output.Selected;
            var cs = model.Output.CharSize;
            foreach (var line in model.Output.Visibles.Array)
            {
                TextRenderer.DrawText(e.Graphics, line.Line, font, line.View, line.Color);
                if (sd != null) Render(e.Graphics, sd, line, selectionBrush, cs);
                if (si != null) Render(e.Graphics, si, line, selectingBrush, cs);
            }
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.Clear();
                var selectedText = SelectedText();
                if (!string.IsNullOrEmpty(selectedText))
                {
                    //throws even on empty strings
                    Clipboard.SetText(selectedText);
                }
            }
        }

        //generated twice on start because scrolls visibility change
        protected override void OnClientSizeChanged(EventArgs e)
        {
            //OnClientSizeChanged makes OnCreateControl redundant
            debugger.WriteLine("LogPanel.OnClientSizeChanged");
            base.OnClientSizeChanged(e);
            QueueViewportUpdate();
        }

        //leave both scrollbars visible to avoid recursing changes
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= WS_VSCROLL;
                cp.Style |= WS_HSCROLL;
                return cp;
            }
        }

        //https://stackoverflow.com/questions/1851620/handling-scroll-event-on-listview-in-c-sharp
        //no practical way to catch scrolling events
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_VSCROLL || m.Msg == WM_HSCROLL || m.Msg == WM_MOUSEWHEEL)
            {
                debugger.WriteLine("LogPanel.OnScrollChanged");
                QueueViewportUpdate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                Capture = true;
                moves = 0;
                click = e.Location;
                QueueMouseEvent("down", e.Location);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Capture && e.Button == MouseButtons.Left)
            {
                moves++;
                QueueMouseEvent("move", e.Location);
                var cs = model.Output.CharSize;
                var vp = model.Output.ViewPort;
                var y = e.Location.Y;
                if (y < 0)
                {
                    var increment = cs.Height * (int)Math.Pow(10, Math.Min(3, Math.Abs(y) / cs.Height));
                    VerticalScroll.Value = Math.Max(VerticalScroll.Minimum, VerticalScroll.Value - increment);
                    QueueViewportUpdate();
                }
                else if (y > vp.Height)
                {
                    var increment = cs.Height * (int)Math.Pow(10, Math.Min(3, Math.Abs(y - vp.Height) / cs.Height));
                    VerticalScroll.Value = Math.Min(VerticalScroll.Maximum, VerticalScroll.Value + increment);
                    QueueViewportUpdate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (Capture && e.Button == MouseButtons.Left)
            {
                Capture = false;
                var isClick = moves <= 1 && click == e.Location;
                if (!isClick) QueueMouseEvent("up", e.Location);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            //1 move event generated even when not moving the mouse
            var isClick = moves <= 1 && click == e.Location;
            if (isClick) QueueMouseEvent("click", e.Location);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            QueueMouseEvent("dclick", e.Location);
        }

        private void Render(Graphics g, LogModel.Region r, LogLine l, Brush b, Size cs)
        {
            VisitSelection(r, l, (start, length) =>
            {
                var v = l.View;
                v.X += start * cs.Width + cs.Width / 2;
                v.Width = length * cs.Width;
                g.FillRectangle(b, v);
                v.X -= cs.Width / 2;
                var ss = l.Line.Substring(start, length);
                TextRenderer.DrawText(g, ss, font, v.Location, color);
            });
        }

        private void VisitSelection(LogModel.Region r, LogLine l, Action<int, int> callback)
        {
            if (l.Index < r.Start.Line) return;
            if (l.Index > r.End.Line) return;
            var start = 0;
            var length = l.Line.Length;
            if (l.Index == r.Start.Line && l.Index == r.End.Line)
            {
                if (r.Start.Index < 0 && r.End.Index < 0) return;
                if (r.Start.Index >= l.Line.Length && r.End.Index >= l.Line.Length) return;
                start = Math.Max(0, r.Start.Index);
                length = Math.Min(l.Line.Length - 1, r.End.Index) - start + 1;
            }
            else if (l.Index == r.Start.Line)
            {
                if (r.Start.Index >= l.Line.Length) return;
                start = Math.Max(0, r.Start.Index);
                length = l.Line.Length - start;
            }
            else if (l.Index == r.End.Line)
            {
                if (r.End.Index < 0) return;
                start = 0;
                length = Math.Min(l.Line.Length - 1, r.End.Index) + 1;
            }
            callback(start, length);
        }

        private void QueueModelChange(Action action)
        {
            InitializeModel();
            actions.Enqueue(action);
            if (!executing)
            {
                executing = true;
                while (actions.Count > 0)
                {
                    //apply as much as possible
                    while (actions.Count > 0)
                    {
                        var currentAction = actions.Dequeue();
                        currentAction();
                    }
                    //doble loop to ensure processing 
                    //nested updates in a single call
                    SynchronizeModelChange();
                }
                executing = false;
            }
        }

        private void SynchronizeModelChange()
        {
            updates++;
            if (dirty)
            {
                //mouser updates do not require this
                debugger.WriteLine("LogPanel.InputState {0} I:{1}", updates, input);
                model.ProcessInput(input);
                dirty = false;
            }
            debugger.WriteLine("LogPanel.OutputState {0} O:{1}", updates, model.Output);
            var previous = output;
            var next = model.Output;
            var current = new LogModel.OutputState()
            {
                Lines = next.Lines,
                CharSize = next.CharSize,
                Visibles = next.Visibles,
                ViewPort = next.ViewPort,
                Selected = next.Selected,
                Selecting = next.Selecting,
                ScrollSize = next.ScrollSize,
            };
            if (LogModel.NotEqual(previous, current))
            {
                debugger.WriteLine("LogPanel.OutputChanged");
                output = current; //cache new output
                var scrollSizeChanged = LogModel.NotEqual(current.ScrollSize, previous.ScrollSize);
                if (scrollSizeChanged)
                {
                    debugger.WriteLine("LogPanel.ScrollSizeChanged");
                    //wont issue scroll event
                    AutoScrollMinSize = current.ScrollSize;
                    VerticalScroll.Value = VerticalScroll.Maximum;
                    HorizontalScroll.Value = 0;
                    //redraw/repositions vertical scroll bar
                    //issues client size change on first call
                    PerformLayout();
                }
                Invalidate();
            }
        }

        private void QueueMouseEvent(string name, Point point)
        {
            var shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            var control = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            QueueModelChange(() => { model.ProcessMouse(name, point, shift, control); });
        }

        private void QueueViewportUpdate()
        {
            QueueModelChange(() =>
            {
                var viewPort = ViewPort();
                if (output.ViewPort != viewPort)
                {
                    debugger.WriteLine("LogPanel.ViewportUpdate {0}", viewPort);
                    input.ViewPort = viewPort;
                    dirty = true;
                }
            });
        }

        private void InitializeModel()
        {
            if (model == null)
            {
                debugger.WriteLine("LogPanel.InitializeModel");
                model = new LogModel();
                input = new LogModel.InputState();
                output = new LogModel.OutputState();
                selectionBrush = new SolidBrush(SelectionBack);
                selectingBrush = new SolidBrush(SelectingBack);
                color = SelectionFront;
                actions = new Queue<Action>();
                //Monospace font required because MeasureText is expensive
                //Ubiquitous monospace fonts: Consolas, Courier
                //FontFamily.GenericMonospace creates Courier New
                font = new Font(
                    FontFamily.GenericMonospace,
                    FontSize,
                    FontStyle.Regular,
                    GraphicsUnit.Pixel);
                //returns width of an extra character 1=14, 2=21, ...
                var cs = TextRenderer.MeasureText("-", font);
                cs.Width /= 2; //returns 14=2*7 for asigned font size = 12
                input.CharSize = cs;
                input.ViewPort = ViewPort();
                input.Lines = output.Lines;
            }
        }

        private Rectangle ViewPort()
        {
            var offset = AutoScrollPosition;
            offset.X *= -1;
            offset.Y *= -1;
            return new Rectangle(offset, ClientSize);
        }
    }
}
