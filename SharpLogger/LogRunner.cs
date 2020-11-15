using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLogger
{
    public class LogRunner : ILogHandler, IDisposable
    {
        public const int SLEEP = 10; //millis
        public const int FLUSH = 50; //millis

        private readonly List<ILogAppender> appenders;
        private readonly List<ILogAppender> removes;
        private readonly List<LogDto> buffer;
        private readonly LogThread thread;
        private DateTime flushed;

        public LogRunner()
        {
            flushed = DateTime.Now;
            appenders = new List<ILogAppender>();
            removes = new List<ILogAppender>();
            buffer = new List<LogDto>();
            thread = new LogThread(Idle);
        }

        public ILogger Create(string source)
        {
            return new Logger(this, source);
        }

        public void Dispose()
        {
            thread.Dispose();
        }

        public void AddAppender(ILogAppender appender)
        {
            thread.Run(() => appenders.Add(appender));
        }

        public void Handle(LogDto log)
        {
            thread.Run(() =>
            {
                buffer.Add(log);
                TryFlush();
            });
        }

        private void TryFlush()
        {
            var elapsed = DateTime.Now - flushed;
            if (elapsed.TotalMilliseconds > FLUSH)
            {
                var list = buffer.ToArray();
                foreach (var appender in appenders)
                {
                    try
                    {
                        appender.Append(list);
                    }
                    catch (Exception ex)
                    {
                        removes.Add(appender);
                        thread.Dump(ex);
                    }
                }
                //autoremove excepting appenders
                foreach (var appender in removes)
                {
                    appenders.Remove(appender);
                }
                flushed = DateTime.Now;
                removes.Clear();
                buffer.Clear();
            }
        }

        private void Idle()
        {
            TryFlush();
            Thread.Sleep(SLEEP);
        }
    }

    class LogThread : IDisposable
    {
        private readonly Thread thread;
        private readonly Action idle;
        private Queue<Action> queue;

        public LogThread(Action callback)
        {
            idle = callback;
            queue = new Queue<Action>();
            thread = new Thread(Loop);
            thread.IsBackground = true;
            thread.Name = typeof(LogThread).Name;
            thread.Start();
        }

        public void Dispose()
        {
            Run(() => { queue = null; });
            thread.Join();
        }

        public void Run(Action action)
        {
            lock (queue)
            {
                queue.Enqueue(action);
                Monitor.Pulse(queue);
            }
        }

        public void Dump(Exception ex)
        {
            LogDao.Dump(typeof(LogRunner).Name, ex);
        }

        private void Loop()
        {
            while (queue != null)
            {
                var action = idle;

                lock (queue)
                {
                    if (queue.Count == 0)
                    {
                        Monitor.Wait(queue, 0);
                    }
                    if (queue.Count > 0)
                    {
                        action = queue.Dequeue();
                    }
                }

                try { action(); }
                catch (Exception ex) { Dump(ex); }
            }
        }
    }
}
