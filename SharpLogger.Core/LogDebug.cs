using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;

namespace SharpLogger
{
    public class LogDebug
    {
        private static HashSet<string> enabled = new HashSet<string>();

        private static LogRunner runner;

        private static void InitRunner()
        {
            if (runner == null) runner = new LogRunner();
        }

        public static void AddDefaultFile()
        {
            InitRunner();
            var path = LogFile.MakePath("debug.txt");
            var dao = new LogFile(path);
            runner.AddAppender(dao);
        }

        public static void AddAppender(ILogAppender appender, int flushMs = 100)
        {
            InitRunner();
            runner.AddAppender(appender, flushMs);
        }

        public static void Enable(string name)
        {
            enabled.Add(name);
        }

        private readonly ILogger logger;

        public LogDebug(string name)
        {
            if (IsDesignMode()) return;
            if (!enabled.Contains(name)) return;
            InitRunner();
            logger = new Logger(runner, name);
        }

        [Conditional("DEBUG")]
        public void WriteLine(string format, params object[] args)
        {
            logger?.Debug(format, args);
        }

        private static bool IsDesignMode()
        {
            //running within VS designer?
            return Assembly.GetEntryAssembly()==null;
        }
    }
}
