#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            var ev = new LogEventInfo(ToNLogLogLevel(level), "", msg);
            if (exception != null)
                ev.Exception = exception;
            if (connectorId != 0)
                ev.Properties["ConnectorId"] = connectorId;
            _log.Log(ev);
        }

        static LogLevel ToNLogLogLevel(NpgsqlLogLevel level)
        {
            switch (level)
            {
            case NpgsqlLogLevel.Trace:
                return LogLevel.Trace;
            case NpgsqlLogLevel.Debug:
                return LogLevel.Debug;
            case NpgsqlLogLevel.Info:
                return LogLevel.Info;
            case NpgsqlLogLevel.Warn:
                return LogLevel.Warn;
            case NpgsqlLogLevel.Error:
                return LogLevel.Error;
            case NpgsqlLogLevel.Fatal:
                return LogLevel.Fatal;
            default:
                throw new ArgumentOutOfRangeException("level");
            }
        }
    }
}
