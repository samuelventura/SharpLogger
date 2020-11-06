using System;

namespace SharpLogger
{
    public class LogEvent
    {
        public LogLevel Level { get; set; } = LogLevel.DEBUG;
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Level} {Source} {Message}";
        }
    }

    public class LogDto : LogEvent
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string Thread { get; set; } = string.Empty;
        public int Process { get; set; }

        public override string ToString()
        {
            var dt = DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"{dt} {Process} {Thread} {base.ToString()}";
        }
    }
}
