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
#pragma warning disable 1591

namespace Npgsql.Logging
{
    /// <summary>
    /// A generic interface for logging.
    /// </summary>
    public abstract class NpgsqlLogger
    {
        public abstract bool IsEnabled(NpgsqlLogLevel level);
        public abstract void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null);

        internal void Trace(string msg, int connectionId = 0) { Log(NpgsqlLogLevel.Trace, connectionId, msg); }
        internal void Debug(string msg, int connectionId = 0) { Log(NpgsqlLogLevel.Debug, connectionId, msg); }
        internal void Info(string msg,  int connectionId = 0) { Log(NpgsqlLogLevel.Info,  connectionId, msg); }
        internal void Warn(string msg,  int connectionId = 0) { Log(NpgsqlLogLevel.Warn,  connectionId, msg); }
        internal void Error(string msg, int connectionId = 0) { Log(NpgsqlLogLevel.Error, connectionId, msg); }
        internal void Fatal(string msg, int connectionId = 0) { Log(NpgsqlLogLevel.Fatal, connectionId, msg); }

        /*
        internal void Trace(string msg, int connectionId = 0, params object[] args) { Log(NpgsqlLogLevel.Trace, String.Format(msg, args)); }
        internal void Debug(string msg, params object[] args) { Log(NpgsqlLogLevel.Debug, String.Format(msg, args)); }
        internal void Info(string msg,  params object[] args) { Log(NpgsqlLogLevel.Info,  String.Format(msg, args)); }
        internal void Warn(string msg,  params object[] args) { Log(NpgsqlLogLevel.Warn,  String.Format(msg, args)); }
        internal void Error(string msg, params object[] args) { Log(NpgsqlLogLevel.Error, String.Format(msg, args)); }
        internal void Fatal(string msg, params object[] args) { Log(NpgsqlLogLevel.Fatal, String.Format(msg, args)); }
         */

        internal void Trace(string msg, Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Trace, connectionId, msg, ex); }
        internal void Debug(string msg, Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Debug, connectionId, msg, ex); }
        internal void Info(string msg,  Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Info,  connectionId, msg, ex); }
        internal void Warn(string msg,  Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Warn,  connectionId, msg, ex); }
        internal void Error(string msg, Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Error, connectionId, msg, ex); }
        internal void Fatal(string msg, Exception ex, int connectionId = 0) { Log(NpgsqlLogLevel.Fatal, connectionId, msg, ex); }
    }
}
