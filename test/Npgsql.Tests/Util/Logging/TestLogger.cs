using System;
using Microsoft.Extensions.Logging;

namespace Npgsql.Tests.Util.Logging
{
    public class TestLogger : ILogger
    {
        readonly string _name;
        readonly TestLoggerSink _sink;

        public TestLogger(string name, TestLoggerSink sink)
        {
            _name = name;
            _sink = sink;
        }

        public string Name { get; set; }

        public IDisposable BeginScope<TState>(TState state) => TestDisposable.Instance;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _sink.AddLogRecord(new LogRecord
            {
                LogLevel = logLevel,
                EventId = eventId,
                State = state,
                Exception = exception,
                Formatter = (s, e) => formatter((TState)s, e),
                LoggerName = _name
            });
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        class TestDisposable : IDisposable
        {
            public static readonly TestDisposable Instance = new TestDisposable();

            public void Dispose()
            {
                // intentionally does nothing
            }
        }
    }
}