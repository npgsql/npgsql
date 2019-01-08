using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using JetBrains.Annotations;
using Npgsql.BackendMessages;

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
        Dictionary<object, object> _data;

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
        /// Constants are defined in <seealso cref="PostgresErrorCodes"/>.
        /// See http://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
        [PublicAPI]
        public string SqlState { get; set; }

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// Constants are defined in <seealso cref="PostgresErrorCodes"/>.
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
                case PostgresErrorCodes.InsufficientResources:
                case PostgresErrorCodes.DiskFull:
                case PostgresErrorCodes.OutOfMemory:
                case PostgresErrorCodes.TooManyConnections:
                case PostgresErrorCodes.ConfigurationLimitExceeded:
                case PostgresErrorCodes.CannotConnectNow:
                case PostgresErrorCodes.SystemError:
                case PostgresErrorCodes.IoError:
                case PostgresErrorCodes.SerializationFailure:
                case PostgresErrorCodes.LockNotAvailable:
                case PostgresErrorCodes.ObjectInUse:
                case PostgresErrorCodes.ObjectNotInPrerequisiteState:
                case PostgresErrorCodes.ConnectionException:
                case PostgresErrorCodes.ConnectionDoesNotExist:
                case PostgresErrorCodes.ConnectionFailure:
                case PostgresErrorCodes.SqlClientUnableToEstablishSqlConnection:
                case PostgresErrorCodes.SqlServerRejectedEstablishmentOfSqlConnection:
                case PostgresErrorCodes.TransactionResolutionUnknown:
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

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());
            builder.AppendLine().Append("  Exception data:");

            void AppendPropertyLine(string propertyName, object propertyValue)
            {
                if (propertyValue == null || propertyValue is int intPropertyValue && intPropertyValue == 0)
                {
                    return;
                }

                builder.AppendLine().Append("    ").Append(propertyName).Append(": ").Append(propertyValue);
            }

            AppendPropertyLine(nameof(Severity), Severity);
            AppendPropertyLine(nameof(SqlState), SqlState);
            AppendPropertyLine(nameof(MessageText), MessageText);
            AppendPropertyLine(nameof(Detail), Detail);
            AppendPropertyLine(nameof(Hint), Hint);
            AppendPropertyLine(nameof(Position), Position);
            AppendPropertyLine(nameof(InternalPosition), InternalPosition);
            AppendPropertyLine(nameof(InternalQuery), InternalQuery);
            AppendPropertyLine(nameof(Where), Where);
            AppendPropertyLine(nameof(SchemaName), SchemaName);
            AppendPropertyLine(nameof(TableName), TableName);
            AppendPropertyLine(nameof(ColumnName), ColumnName);
            AppendPropertyLine(nameof(DataTypeName), DataTypeName);
            AppendPropertyLine(nameof(ConstraintName), ConstraintName);
            AppendPropertyLine(nameof(File), File);
            AppendPropertyLine(nameof(Line), Line);
            AppendPropertyLine(nameof(Routine), Routine);
            return builder.ToString();
        }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional PostgreSQL fields about the exception.
        /// </summary>
        public override IDictionary Data
        {
            get
            {
                // Remarks: return Dictionary with object keys although all our keys are string keys
                // because System.Windows.Threading.Dispatcher relies on that
                return _data ?? (_data = (
                    from p in typeof(PostgresException).GetProperties()
                    let k = p.Name
                    where p.Name != nameof(Data)
                    where p.GetCustomAttribute<PublicAPIAttribute>() != null
                    let v = p.GetValue(this)
                    where v != null
                    where k != nameof(Position) && k != nameof(InternalPosition) || (int)v != 0
                    select new { Key = k, Value = v }
                    ).ToDictionary(kv => (object)kv.Key, kv => kv.Value)
                );
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
