using System;

namespace SharpLogger
{
    public class Logger : ILogger
    {
        private readonly string name;
        private readonly ILogHandler runner;
        private readonly Func<DateTime> timer;

        public Logger(ILogHandler runner, string name = null)
        {
            this.timer = timer ?? Now;
            this.runner = runner;
            this.name = name;
        }

        public void Debug(string format, params object[] args)
        {
            Log(LogDto.DEBUG, format, args);
        }

        public void Info(string format, params object[] args)
        {
            Log(LogDto.INFO, format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Log(LogDto.WARN, format, args);
        }

        public void Error(string format, params object[] args)
        {
            Log(LogDto.ERROR, format, args);
        }

        public void Success(string format, params object[] args)
        {
            Log(LogDto.SUCCESS, format, args);
        }

        public void Log(string level, string format, params object[] args)
        {
            runner.Handle(new LogDto
            {
                Source = name,
                Level = level,
                Message = Format(format, args),
            });
        }

        private string Format(string format, params object[] args)
        {
            if (args.Length == 0) return format;
            return string.Format(format, args);
        }

        private DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
