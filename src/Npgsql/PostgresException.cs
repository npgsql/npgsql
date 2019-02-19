#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using System.Runtime.Serialization;

#pragma warning disable CA1032

namespace Npgsql
{
    /// <summary>
    /// The exception that is thrown when the PostgreSQL backend reports errors (e.g. query
    /// SQL issues, constraint violations).
    /// </summary>
    /// <remarks>
    /// This exception only corresponds to a PostgreSQL-delivered error.
    /// Other errors (e.g. network issues) will be raised via <see cref="NpgsqlException"/>,
    /// and purely Npgsql-related issues which aren't related to the server will be raised
    /// via the standard CLR exceptions (e.g. ArgumentException).
    /// 
    /// See http://www.postgresql.org/docs/current/static/errcodes-appendix.html,
    /// http://www.postgresql.org/docs/current/static/protocol-error-fields.html
    /// </remarks>
    [Serializable]
    public sealed class PostgresException : NpgsqlException
    {
        [CanBeNull]
        bool _dataInitialized;

        #region Message Fields

        /// <summary>
        /// Severity of the error or notice.
        /// Always present.
        /// </summary>
        [PublicAPI]
        public string Severity { get; set; }

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// See http://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
        [PublicAPI]
        public string SqlState { get; set; }

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// See http://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
        [PublicAPI, Obsolete("Use SqlState instead")]
        public string Code => SqlState;

        /// <summary>
        /// The primary human-readable error message. This should be accurate but terse.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// </remarks>
        [PublicAPI]
        public string MessageText { get; set; }

        /// <summary>
        /// An optional secondary error message carrying more detail about the problem.
        /// May run to multiple lines.
        /// </summary>
        [PublicAPI]
        public string Detail { get; set; }

        /// <summary>
        /// An optional suggestion what to do about the problem.
        /// This is intended to differ from Detail in that it offers advice (potentially inappropriate) rather than hard facts.
        /// May run to multiple lines.
        /// </summary>
        [PublicAPI]
        public string Hint { get; set; }

        /// <summary>
        /// The field value is a decimal ASCII integer, indicating an error cursor position as an index into the original query string.
        /// The first character has index 1, and positions are measured in characters not bytes.
        /// 0 means not provided.
        /// </summary>
        [PublicAPI]
        public int Position { get; set; }

        /// <summary>
        /// This is defined the same as the <see cref="Position"/> field, but it is used when the cursor position refers to an internally generated command rather than the one submitted by the client.
        /// The <see cref="InternalQuery" /> field will always appear when this field appears.
        /// 0 means not provided.
        /// </summary>
        [PublicAPI]
        public int InternalPosition { get; set; }

        /// <summary>
        /// The text of a failed internally-generated command.
        /// This could be, for example, a SQL query issued by a PL/pgSQL function.
        /// </summary>
        [PublicAPI]
        public string InternalQuery { get; set; }

        /// <summary>
        /// An indication of the context in which the error occurred.
        /// Presently this includes a call stack traceback of active PL functions.
        /// The trace is one entry per line, most recent first.
        /// </summary>
        [PublicAPI]
        public string Where { get; set; }

        /// <summary>
        /// If the error was associated with a specific database object, the name of the schema containing that object, if any.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string SchemaName { get; set; }

        /// <summary>
        /// Table name: if the error was associated with a specific table, the name of the table.
        /// (Refer to the schema name field for the name of the table's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string TableName { get; set; }

        /// <summary>
        /// If the error was associated with a specific table column, the name of the column.
        /// (Refer to the schema and table name fields to identify the table.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string ColumnName { get; set; }

        /// <summary>
        /// If the error was associated with a specific data type, the name of the data type.
        /// (Refer to the schema name field for the name of the data type's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string DataTypeName { get; set; }

        /// <summary>
        /// If the error was associated with a specific constraint, the name of the constraint.
        /// Refer to fields listed above for the associated table or domain.
        /// (For this purpose, indexes are treated as constraints, even if they weren't created with constraint syntax.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string ConstraintName { get; set; }

        /// <summary>
        /// The file name of the source-code location where the error was reported.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string File { get; set; }

        /// <summary>
        /// The line number of the source-code location where the error was reported.
        /// </summary>
        [PublicAPI]
        public string Line { get; set; }

        /// <summary>
        /// The name of the source-code routine reporting the error.
        /// </summary>
        [PublicAPI]
        public string Routine { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PostgresException() {}

        internal PostgresException(NpgsqlReadBuffer buf)
        {
            var msg = new ErrorOrNoticeMessage(buf);
            Severity = msg.Severity;
            SqlState = msg.Code;
            MessageText = msg.Message;
            Detail = msg.Detail;
            Hint = msg.Hint;
            Position = msg.Position;
            InternalPosition = msg.InternalPosition;
            InternalQuery = msg.InternalQuery;
            Where = msg.Where;
            SchemaName = msg.SchemaName;
            TableName = msg.TableName;
            ColumnName = msg.ColumnName;
            DataTypeName = msg.DataTypeName;
            ConstraintName = msg.ConstraintName;
            File = msg.File;
            Line = msg.Line;
            Routine = msg.Routine;
        }

        /// <summary>
        /// Gets a the PostgreSQL error message and code.
        /// </summary>
        public override string Message => SqlState + ": " + MessageText;

        /// <summary>
        /// Specifies whether the exception is considered transient, that is, whether retrying to operation could
        /// succeed (e.g. a network error). Check <see cref="SqlState"/>.
        /// </summary>
        public override bool IsTransient
        {
            get
            {
                switch (SqlState)
                {
                case "53000":   //insufficient_resources
                case "53100":   //disk_full
                case "53200":   //out_of_memory
                case "53300":   //too_many_connections
                case "53400":   //configuration_limit_exceeded
                case "57P03":   //cannot_connect_now
                case "58000":   //system_error
                case "58030":   //io_error
                case "40001":   //serialization_error
                case "55P03":   //lock_not_available
                case "55006":   //object_in_use
                case "55000":   //object_not_in_prerequisite_state
                case "08000":   //connection_exception
                case "08003":   //connection_does_not_exist
                case "08006":   //connection_failure
                case "08001":   //sqlclient_unable_to_establish_sqlconnection
                case "08004":   //sqlserver_rejected_establishment_of_sqlconnection
                case "08007":   //transaction_resolution_unknown
                    return true;
                default:
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the statement which triggered this exception.
        /// </summary>
        public NpgsqlStatement Statement { get; internal set; }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional PostgreSQL fields about the exception.
        /// </summary>
        public override IDictionary Data
        {
            get
            {
                if (_dataInitialized)
                    return base.Data;

                var data = base.Data;
                foreach (var pair in
                    from p in typeof(PostgresException).GetProperties()
                    let k = p.Name
                    where p.Name != nameof(Data)
                    where p.GetCustomAttribute<PublicAPIAttribute>() != null
                    let v = p.GetValue(this)
                    where v != null
                    where k != nameof(Position) && k != nameof(InternalPosition) || (int)v != 0
                    select new KeyValuePair<string, object>(k, v))
                {
                    data.Add(pair.Key, pair.Value);
                }

                _dataInitialized = true;
                return data;
            }
        }

        #region Serialization

        PostgresException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Severity         = (string)info.GetValue("Severity",         typeof(string));
            SqlState         = (string)info.GetValue("SqlState",         typeof(string));
            MessageText      = (string)info.GetValue("MessageText",      typeof(string));
            Detail           = (string)info.GetValue("Detail",           typeof(string));
            Hint             = (string)info.GetValue("Hint",             typeof(string));
            Position         = (int)   info.GetValue("Position",         typeof(int));
            InternalPosition = (int)   info.GetValue("InternalPosition", typeof(int));
            InternalQuery    = (string)info.GetValue("InternalQuery",    typeof(string));
            Where            = (string)info.GetValue("Where",            typeof(string));
            SchemaName       = (string)info.GetValue("SchemaName",       typeof(string));
            TableName        = (string)info.GetValue("TableName",        typeof(string));
            ColumnName       = (string)info.GetValue("ColumnName",       typeof(string));
            DataTypeName     = (string)info.GetValue("DataTypeName",     typeof(string));
            ConstraintName   = (string)info.GetValue("ConstraintName",   typeof(string));
            File             = (string)info.GetValue("File",             typeof(string));
            Line             = (string)info.GetValue("Line",             typeof(string));
            Routine          = (string)info.GetValue("Routine",          typeof(string));
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Severity", Severity);
            info.AddValue("SqlState", SqlState);
            info.AddValue("MessageText", MessageText);
            info.AddValue("Detail", Detail);
            info.AddValue("Hint", Hint);
            info.AddValue("Position", Position);
            info.AddValue("InternalPosition", InternalPosition);
            info.AddValue("InternalQuery", InternalQuery);
            info.AddValue("Where", Where);
            info.AddValue("SchemaName", SchemaName);
            info.AddValue("TableName", TableName);
            info.AddValue("ColumnName", ColumnName);
            info.AddValue("DataTypeName", DataTypeName);
            info.AddValue("ConstraintName", ConstraintName);
            info.AddValue("File", File);
            info.AddValue("Line", Line);
            info.AddValue("Routine", Routine);
        }
      
        #endregion
    }
}
