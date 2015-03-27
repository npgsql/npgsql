using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Npgsql.Logging
{
    class NoOpLoggingProvider : INpgsqlLoggingProvider
    {
        public NpgsqlLogger CreateLogger(string name)
        {
            return NoOpLogger.Instance;
        }
    }

    class NoOpLogger : NpgsqlLogger
    {
        internal static NoOpLogger Instance = new NoOpLogger();

        NoOpLogger() {}

        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return false;
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
        }
    }
}
