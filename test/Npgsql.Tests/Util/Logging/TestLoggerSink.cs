using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Npgsql.Tests.Util.Logging
{
    public class TestLoggerSink
    {
        public List<LogRecord> Records { get; } = new List<LogRecord>();
        public void Clear() => Records.Clear();

        public event RecordLoggedEventHandler RecordLogged;

        internal void AddLogRecord(LogRecord record)
        {
            Records.Add(record);
            RecordLogged?.Invoke(record);
        }
    }

    public class LogRecord
    {
        public LogLevel LogLevel { get; set; }
        public EventId EventId { get; set; }
        public object State { get; set; }
        public Exception Exception { get; set; }
        public Func<object, Exception, string> Formatter { get; set; }
        public string LoggerName { get; set; }

        public string Format() => Formatter(State, Exception);
        public override string ToString() => $"[{EventId}]";
    }

    public delegate void RecordLoggedEventHandler(LogRecord record);
}