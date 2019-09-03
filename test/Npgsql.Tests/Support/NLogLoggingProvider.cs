using System;
using NLog;
using Npgsql.Logging;

namespace Npgsql.Tests.Support
{
    class NLogLoggingProvider : INpgsqlLoggingProvider
    {
        public NpgsqlLogger CreateLogger(string name)
        {
            return new NLogLogger(name);
        }
    }

    class NLogLogger : NpgsqlLogger
    {
        readonly Logger _log;

        internal NLogLogger(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return _log.IsEnabled(ToNLogLogLevel(level));
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception? exception = null)
        {
            var ev = new LogEventInfo(ToNLogLogLevel(level), "", msg);
            if (exception != null)
                ev.Exception = exception;
            if (connectorId != 0)
                ev.Properties["ConnectorId"] = connectorId;
            _log.Log(ev);
        }

        static LogLevel ToNLogLogLevel(NpgsqlLogLevel level)
            => level switch
            {
                NpgsqlLogLevel.Trace => LogLevel.Trace,
                NpgsqlLogLevel.Debug => LogLevel.Debug,
                NpgsqlLogLevel.Info  => LogLevel.Info,
                NpgsqlLogLevel.Warn  => LogLevel.Warn,
                NpgsqlLogLevel.Error => LogLevel.Error,
                NpgsqlLogLevel.Fatal => LogLevel.Fatal,
                _                    => throw new ArgumentOutOfRangeException(nameof(level))
            };
    }
}
