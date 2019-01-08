using System;

namespace Npgsql.Logging
{
    class NoOpLoggingProvider : INpgsqlLoggingProvider
    {
        public NpgsqlLogger CreateLogger(string name) => NoOpLogger.Instance;
    }

    class NoOpLogger : NpgsqlLogger
    {
        internal static NoOpLogger Instance = new NoOpLogger();

        NoOpLogger() {}
        public override bool IsEnabled(NpgsqlLogLevel level) => false;
        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
        }
    }
}
