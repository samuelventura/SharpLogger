using System;

namespace SharpLogger
{
    public class PatternLogFormatter : ILogConverter<string>
    {
        public readonly static PatternLogFormatter LINE = new PatternLogFormatter("{MESSAGE}");
        public readonly static PatternLogFormatter TIMEONLY_MESSAGE = new PatternLogFormatter("{TS:HH:mm:ss.fff} {MESSAGE}");
        public readonly static PatternLogFormatter TIMESTAMP_MESSAGE = new PatternLogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {MESSAGE}");
        public readonly static PatternLogFormatter TIMESTAMP_LEVEL_MESSAGE = new PatternLogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {MESSAGE}");
        public readonly static PatternLogFormatter TIMESTAMP_LEVEL_THREAD_MESSAGE = new PatternLogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {THREAD} {MESSAGE}");
        public readonly static PatternLogFormatter TIMESTAMP_LEVEL_SOURCE_MESSAGE = new PatternLogFormatter("{TS:yyyy-MM-dd HH:mm:ss.fff} {LEVEL} {SOURCE} {MESSAGE}");

        private readonly string format;

        public PatternLogFormatter(string pattern)
        {
            pattern = pattern.Replace("TS", "0");
            pattern = pattern.Replace("PID", "1");
            pattern = pattern.Replace("THREAD", "2");
            pattern = pattern.Replace("LEVEL", "3");
            pattern = pattern.Replace("SOURCE", "4");
            pattern = pattern.Replace("MESSAGE", "5");
            this.format = pattern;
        }

        public string Format { get { return format; } }

        public string Convert(LogDto log)
        {
            var args = new object[] { log.Timestamp, log.ProcessId, log.ThreadName, log.Level, log.Source, log.Message };
            return string.Format(format, args);
        }
    }
}
