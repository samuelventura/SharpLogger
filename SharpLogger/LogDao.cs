using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using LiteDB;

namespace SharpLogger
{
    public class LogDao : ILogAppender, IDisposable
    {
        public static readonly string ASSYPATH = AssyPath();

        private static readonly object locker = new object();

        private readonly LiteDatabase db;
        private readonly LiteCollection<LogDto> logs;

        public LogDao(string path = null)
        {
            db = new LiteDatabase(path ?? ASSYPATH);
            logs = db.GetCollection<LogDto>("Logs");
            logs.EnsureIndex(x => x.Timestamp);
        }

        public void Append(params LogDto[] dtos)
        {
            logs.InsertBulk(dtos);
        }

        public void Dispose()
        {
            db.Dispose();
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

        public static void Append(string path, params LogDto[] dtos)
        {
            lock(locker)
            {
                BsonMapper.Global.EmptyStringToNull = false;
                using (var db = new LiteDatabase(path))
                {
                    var logs = db.GetCollection<LogDto>("Logs");
                    logs.InsertBulk(dtos);
                }
            }
        }

        public static IEnumerable<LogDto> Filter(string path, DateTime from, DateTime to, Func<LogDto, bool> filter = null)
        {
            lock (locker)
            {
                using (var db = new LiteDatabase(path))
                {
                    var logs = db.GetCollection<LogDto>("Logs");
                    logs.EnsureIndex(x => x.Timestamp);
                    return logs.Find(x => x.Timestamp >= from
                        && x.Timestamp <= to
                        && filter == null ? true : filter(x));
                }
            }
        }

        private static string DumpPath(string id)
        {
            var entry = Assembly.GetEntryAssembly().Location;
            return Path.ChangeExtension(entry, $"StackTrace.{id}.txt");
        }

        private static string AssyPath()
        {
            var entry = Assembly.GetEntryAssembly().Location;
            return Path.ChangeExtension(entry, "SharpLogger");
        }
    }
}
