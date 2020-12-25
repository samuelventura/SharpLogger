using System;
using System.Threading;
using System.Diagnostics;

namespace SharpLogger
{
    public class LogDto
    {
        public const string DEBUG = "DEBUG";
        public const string INFO = "INFO";
        public const string WARN = "WARN";
        public const string ERROR = "ERROR";
        public const string SUCCESS = "SUCCESS";

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int ProcessId { get; set; } = Process.GetCurrentProcess().Id;
        public string ThreadName { get; set; } = Thread.CurrentThread.Name;
        public string Level { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public override string ToString()
        {
            var dt = Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            return $"|{dt}|{ProcessId}|{ThreadName}|{Level}|{Source}|{Message}|";
        }
    }
}
