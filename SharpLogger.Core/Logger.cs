using System;

namespace SharpLogger
{
    public class Logger : ILogger
    {
        private readonly string source;
        private readonly ILogHandler handler;

        public Logger(ILogHandler handler, string source = null)
        {
            this.handler = handler;
            this.source = source;
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
            handler.HandleLog(new LogDto
            {
                Level = level,
                Source = source,
                Message = Format(format, args),
            });
        }

        private string Format(string format, params object[] args)
        {
            if (args.Length == 0) return format;
            return string.Format(format, args);
        }
    }
}
