using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpLogger
{
    public partial class LogRunner : ILogHandler, IDisposable
    {
        private readonly List<AppenderRT> appenders;
        private readonly List<AppenderRT> removes;
        private readonly LogThread thread;
        private readonly Logger logger;
        private readonly int sleepMs;

        public LogRunner(int sleepMs = 10)
        {
            this.sleepMs = sleepMs;
            appenders = new List<AppenderRT>();
            removes = new List<AppenderRT>();
            thread = new LogThread(Idle);
            logger = Create(typeof(LogRunner).Name);
        }

        public Logger Create(string source)
        {
            return new Logger(this, source);
        }

        public void Dispose()
        {
            thread.Dispose(() => TryFlush(true));
        }

        public void AddAppender(ILogAppender appender, int millis = 100)
        {
            thread.Run(() => appenders.Add(new AppenderRT() {
                Buffer = new List<LogDto>(),
                Appender = appender,
                Millis = millis,
            }));
        }

        public void Append(LogDto log)
        {
            thread.Run(() =>
            {
                foreach (var rt in appenders)
                {
                    rt.Buffer.Add(log);
                    if (rt.Oldest.HasValue) continue;
                    rt.Oldest = log.Timestamp;
                }
            });
        }

        public void Run(Action action)
        {
            thread.Run(action);
        }

        private void TryFlush(bool force = false)
        {
            foreach (var rt in appenders)
            {
                var now = DateTime.Now;
                var oldest = rt.Oldest ?? now;
                var elapsed = now - oldest;
                var flush = elapsed.TotalMilliseconds > rt.Millis;
                if (force || flush)
                {
                    try
                    {
                        //Should not switch to UI to prevent thread
                        //deathlock when disposing from UI events
                        rt.Appender.Append(rt.Buffer.ToArray());
                        rt.Buffer.Clear();
                    }
                    catch (Exception ex)
                    {
                        removes.Add(rt);
                        thread.Dump(ex);
                    }
                }
            }
            //autoremove excepting appenders
            foreach (var appender in removes)
            {
                appenders.Remove(appender);
            }
            removes.Clear();
        }

        private void Idle()
        {
            TryFlush();
            Thread.Sleep(sleepMs);
        }

        private class AppenderRT
        {
            public ILogAppender Appender;
            public List<LogDto> Buffer;
            public DateTime? Oldest;
            public int Millis;
        }
    }

    public class LogThread
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

        public void Dispose(Action action)
        {
            Run(() => { queue = null; action(); });
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

    public partial class LogRunner : ILogger
    {
        public void Log(string level, string format, params object[] args)
        {
            logger.Log(level, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            logger.Debug(format, args);
        }

        public void Info(string format, params object[] args)
        {
            logger.Info(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            logger.Warn(format, args);
        }

        public void Error(string format, params object[] args)
        {
            logger.Error(format, args);
        }

        public void Success(string format, params object[] args)
        {
            logger.Success(format, args);
        }
    }
}
