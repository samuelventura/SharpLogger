using System;

namespace SharpLogger
{
    public class LogFormatter : ILogConverter<string>
    {
        public readonly static LogFormatter LINE = new LogFormatter("{MESSAGE}");
        public readonly static LogFormatter TIMEONLY_MESSAGE = new LogFormatter("{TS:HH:mm:ss.fff} {MESSAGE}");
        public readonly static LogFormatter TIMESTAMP_MESSAGE = new LogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {MESSAGE}");
        public readonly static LogFormatter TIMESTAMP_LEVEL_MESSAGE = new LogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {MESSAGE}");
        public readonly static LogFormatter TIMESTAMP_LEVEL_THREAD_MESSAGE = new LogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {THREAD} {MESSAGE}");
        public readonly static LogFormatter TIMESTAMP_LEVEL_SOURCE_MESSAGE = new LogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {SOURCE} {MESSAGE}");

        private readonly string process;
        private readonly string format;

        public LogFormatter(string format)
        {
            this.format = format;
            format = format.Replace("TS", "0");
            format = format.Replace("PID", "1");
            format = format.Replace("THREAD", "2");
            format = format.Replace("LEVEL", "3");
            format = format.Replace("SOURCE", "4");
            format = format.Replace("MESSAGE", "5");
            this.process = format;
        }

        public string Format { get { return format; } }

        public string Convert(LogDto log)
        {
            var args = new object[] { log.Timestamp, log.ProcessId, log.ThreadName, log.Level, log.Source, log.Message };
            return string.Format(process, args);
        }
    }
}
