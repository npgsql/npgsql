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

namespace Npgsql.Logging
{
    /// <summary>
    /// An logging provider that outputs Npgsql logging messages to standard error.
    /// </summary>
    public class ConsoleLoggingProvider : INpgsqlLoggingProvider
    {
        readonly NpgsqlLogLevel _minLevel;
        readonly bool _printLevel;
        readonly bool _printConnectorId;

        /// <summary>
        /// Constructs a new <see cref="ConsoleLoggingProvider"/>
        /// </summary>
        /// <param name="minLevel">Only messages of this level of higher will be logged</param>
        /// <param name="printLevel">If true, will output the log level (e.g. WARN). Defaults to false.</param>
        /// <param name="printConnectorId">If true, will output the connector ID. Defaults to false.</param>
        public ConsoleLoggingProvider(NpgsqlLogLevel minLevel=NpgsqlLogLevel.Info, bool printLevel=false, bool printConnectorId=false)
        {
            _minLevel = minLevel;
            _printLevel = printLevel;
            _printConnectorId = printConnectorId;
        }

        /// <summary>
        /// Creates a new <see cref="ConsoleLogger"/> instance of the given name.
        /// </summary>
        public NpgsqlLogger CreateLogger(string name)
        {
            return new ConsoleLogger(_minLevel, _printLevel, _printConnectorId);
        }
    }

    class ConsoleLogger : NpgsqlLogger
    {
        readonly NpgsqlLogLevel _minLevel;
        readonly bool _printLevel;
        readonly bool _printConnectorId;

        internal ConsoleLogger(NpgsqlLogLevel minLevel, bool printLevel, bool printConnectorId)
        {
            _minLevel = minLevel;
            _printLevel = printLevel;
            _printConnectorId = printConnectorId;
        }

        public override bool IsEnabled(NpgsqlLogLevel level) => level >= _minLevel;

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            if (!IsEnabled(level))
                return;

            var sb = new StringBuilder();
            if (_printLevel) {
                sb.Append(level.ToString().ToUpper());
                sb.Append(' ');
            }

            if (_printConnectorId && connectorId != 0)
            {
                sb.Append("[");
                sb.Append(connectorId);
                sb.Append("] ");
            }

            sb.AppendLine(msg);

            if (exception != null)
                sb.AppendLine(exception.ToString());

            Console.Error.Write(sb.ToString());
        }
    }
}
