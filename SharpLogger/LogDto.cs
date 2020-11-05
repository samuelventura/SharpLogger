using System;

namespace SharpLogger
{
    public class LogEvent
    {
        public LogLevel Level { get; set; } = LogLevel.DEBUG;
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class LogDto : LogEvent
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Thread { get; set; } = string.Empty;
        public int Process { get; set; }
    }
}
