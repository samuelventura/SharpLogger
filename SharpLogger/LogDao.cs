using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace SharpLogger
{
    public class LogDao : ILogAppender, IDisposable
    {
        private readonly StreamWriter writer;

        public LogDao(string path = null)
        {
            writer = File.AppendText(path ?? AssyPath());
        }

        public void Append(params LogDto[] dtos)
        {
            foreach(var dto in dtos)
            {
                writer.WriteLine(dto.ToString());
            }
            writer.Flush();
        }

        public void Dispose()
        {
            writer.Dispose();
        }

        public static void Dump(string source, Exception ex, bool show = false)
        {
            try 
            {
                var now = DateTime.Now;
                var fmt1 = "yyyyMMdd_HHmmss_fff";
                var path = DumpPath(now.ToString(fmt1));
                var nl = Environment.NewLine;
                var fmt2 = "yyyy-MM-dd HH:mm:ss.fff";
                var dt = now.ToString(fmt2);
                using (var sw = File.AppendText(path))
                {
                    sw.WriteLine($"{dt} {source}");
                    sw.WriteLine(ex.ToString());
                }
                if (show) Process.Start(path);
            }
            catch (Exception) { }
        }

        private static string DumpPath(string id)
        {
            var entry = Assembly.GetEntryAssembly().Location;
            return Path.ChangeExtension(entry, $"StackTrace.{id}.txt");
        }

        private static string AssyPath()
        {
            var entry = Assembly.GetEntryAssembly().Location;
            return Path.ChangeExtension(entry, "log.txt");
        }
    }
}
