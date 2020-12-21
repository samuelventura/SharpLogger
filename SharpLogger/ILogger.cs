using System;

namespace SharpLogger
{
    public interface ILogger
    {
        void Log(string level, string format, params object[] args);

        void Debug(string format, params object[] args);

        void Info(string format, params object[] args);

        void Warn(string format, params object[] args);

        void Error(string format, params object[] args);

        void Success(string format, params object[] args);
    }

    public interface ILogAppender
    {
        void AppendLog(params LogDto[] log);
    }

    public interface ILogHandler
    {
        void HandleLog(LogDto log);
    }

    public interface ILogConverter<T>
    {
        T ConvertLog(LogDto log);
    }

    public class NopLogHandler : ILogHandler
    {
        public void HandleLog(LogDto log)
        {
        }
    }
}
