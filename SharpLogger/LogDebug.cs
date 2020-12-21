using System;
using System.Reflection;
using System.Diagnostics;

namespace SharpLogger
{
    public static class LogDebug
    {
        public static bool Enabled;

        private static ILogger logger;

        [Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            if (Enabled) 
            {
                if (logger == null)
                {
                    if (IsDesignMode())
                    {
                        logger = new Logger(new NopLogHandler(), typeof(LogDebug).Name);
                    }
                    else
                    {
                        var path = LogFile.MakePath("debug.txt");
                        var dao = new LogFile(path);
                        logger = new Logger(dao, typeof(LogDebug).Name);
                    }
                }
                logger.Debug(format, args);
            }
        }

        private static bool IsDesignMode()
        {
            //running with in VS designer?
            return Assembly.GetEntryAssembly()==null;
        }
    }
}
