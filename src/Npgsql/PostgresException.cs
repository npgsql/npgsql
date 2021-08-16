using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.Internal;

namespace Npgsql
{
    /// <summary>
    /// The exception that is thrown when the PostgreSQL backend reports errors (e.g. query
    /// SQL issues, constraint violations).
    /// </summary>
    /// <remarks>
    /// This exception only corresponds to a PostgreSQL-delivered error.
    /// Other errors (e.g. network issues) will be raised via <see cref="NpgsqlException" />,
    /// and purely Npgsql-related issues which aren't related to the server will be raised
    /// via the standard CLR exceptions (e.g. <see cref="ArgumentException" />).
    ///
    /// See https://www.postgresql.org/docs/current/static/errcodes-appendix.html,
    /// https://www.postgresql.org/docs/current/static/protocol-error-fields.html
    /// </remarks>
    [Serializable]
    public sealed class PostgresException : NpgsqlException
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PostgresException(string messageText, string severity, string invariantSeverity, string sqlState)
            : this(messageText, severity, invariantSeverity, sqlState, detail: null) {}

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PostgresException(
            string messageText, string severity, string invariantSeverity, string sqlState,
            string? detail = null, string? hint = null, int position = 0, int internalPosition = 0,
            string? internalQuery = null, string? where = null, string? schemaName = null, string? tableName = null,
            string? columnName = null, string? dataTypeName = null, string? constraintName = null, string? file = null,
            string? line = null, string? routine = null)
            : base(GetMessage(sqlState, messageText, position, detail))
        {
            MessageText = messageText;
            Severity = severity;
            InvariantSeverity = invariantSeverity;
            SqlState = sqlState;

            Detail = detail;
            Hint = hint;
            Position = position;
            InternalPosition = internalPosition;
            InternalQuery = internalQuery;
            Where = where;
            SchemaName = schemaName;
            TableName = tableName;
            ColumnName = columnName;
            DataTypeName = dataTypeName;
            ConstraintName = constraintName;
            File = file;
            Line = line;
            Routine = routine;

            AddData(nameof(Severity), Severity);
            AddData(nameof(InvariantSeverity), InvariantSeverity);
            AddData(nameof(SqlState), SqlState);
            AddData(nameof(MessageText), MessageText);
            AddData(nameof(Detail), Detail);
            AddData(nameof(Hint), Hint);
            AddData(nameof(Position), Position);
            AddData(nameof(InternalPosition), InternalPosition);
            AddData(nameof(InternalQuery), InternalQuery);
            AddData(nameof(Where), Where);
            AddData(nameof(SchemaName), SchemaName);
            AddData(nameof(TableName), TableName);
            AddData(nameof(ColumnName), ColumnName);
            AddData(nameof(DataTypeName), DataTypeName);
            AddData(nameof(ConstraintName), ConstraintName);
            AddData(nameof(File), File);
            AddData(nameof(Line), Line);
            AddData(nameof(Routine), Routine);

            void AddData<T>(string key, T value)
            {
                if (!EqualityComparer<T>.Default.Equals(value, default!))
                    Data.Add(key, value);
            }
        }

        static string GetMessage(string sqlState, string messageText, int position, string? detail)
        {
            var baseMessage = sqlState + ": " + messageText;
            var additionalMessage =
                TryAddString("POSITION", position == 0 ? null : position.ToString()) +
                TryAddString("DETAIL", detail);
            return string.IsNullOrEmpty(additionalMessage)
                ? baseMessage
                : baseMessage + Environment.NewLine + additionalMessage;
        }

        static string TryAddString(string text, string? value) => !string.IsNullOrWhiteSpace(value) ? $"{Environment.NewLine}{text}: {value}" : string.Empty;

        PostgresException(ErrorOrNoticeMessage msg)
            : this(
                msg.Message, msg.Severity, msg.InvariantSeverity, msg.SqlState,
                msg.Detail, msg.Hint, msg.Position, msg.InternalPosition, msg.InternalQuery,
                msg.Where, msg.SchemaName, msg.TableName, msg.ColumnName, msg.DataTypeName,
                msg.ConstraintName, msg.File, msg.Line, msg.Routine) {}

        internal static PostgresException Load(NpgsqlReadBuffer buf, bool includeDetail)
            => new(ErrorOrNoticeMessage.Load(buf, includeDetail));

        internal PostgresException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Severity = GetValue<string>(nameof(Severity));
            InvariantSeverity = GetValue<string>(nameof(InvariantSeverity));
            SqlState = GetValue<string>(nameof(SqlState));
            MessageText = GetValue<string>(nameof(MessageText));
            Detail = GetValue<string>(nameof(Detail));
            Hint = GetValue<string>(nameof(Hint));
            Position = GetValue<int>(nameof(Position));
            InternalPosition = GetValue<int>(nameof(InternalPosition));
            InternalQuery = GetValue<string>(nameof(InternalQuery));
            Where = GetValue<string>(nameof(Where));
            SchemaName = GetValue<string>(nameof(SchemaName));
            TableName = GetValue<string>(nameof(TableName));
            ColumnName = GetValue<string>(nameof(ColumnName));
            DataTypeName = GetValue<string>(nameof(DataTypeName));
            ConstraintName = GetValue<string>(nameof(ConstraintName));
            File = GetValue<string>(nameof(File));
            Line = GetValue<string>(nameof(Line));
            Routine = GetValue<string>(nameof(Routine));

            T GetValue<T>(string propertyName) => (T)info.GetValue(propertyName, typeof(T))!;
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Severity), Severity);
            info.AddValue(nameof(InvariantSeverity), InvariantSeverity);
            info.AddValue(nameof(SqlState), SqlState);
            info.AddValue(nameof(MessageText), MessageText);
            info.AddValue(nameof(Detail), Detail);
            info.AddValue(nameof(Hint), Hint);
            info.AddValue(nameof(Position), Position);
            info.AddValue(nameof(InternalPosition), InternalPosition);
            info.AddValue(nameof(InternalQuery), InternalQuery);
            info.AddValue(nameof(Where), Where);
            info.AddValue(nameof(SchemaName), SchemaName);
            info.AddValue(nameof(TableName), TableName);
            info.AddValue(nameof(ColumnName), ColumnName);
            info.AddValue(nameof(DataTypeName), DataTypeName);
            info.AddValue(nameof(ConstraintName), ConstraintName);
            info.AddValue(nameof(File), File);
            info.AddValue(nameof(Line), Line);
            info.AddValue(nameof(Routine), Routine);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString())
                .AppendLine().Append("  Exception data:");

            AppendLine(nameof(Severity), Severity);
            AppendLine(nameof(SqlState), SqlState);
            AppendLine(nameof(MessageText), MessageText);
            AppendLine(nameof(Detail), Detail);
            AppendLine(nameof(Hint), Hint);
            AppendLine(nameof(Position), Position);
            AppendLine(nameof(InternalPosition), InternalPosition);
            AppendLine(nameof(InternalQuery), InternalQuery);
            AppendLine(nameof(Where), Where);
            AppendLine(nameof(SchemaName), SchemaName);
            AppendLine(nameof(TableName), TableName);
            AppendLine(nameof(ColumnName), ColumnName);
            AppendLine(nameof(DataTypeName), DataTypeName);
            AppendLine(nameof(ConstraintName), ConstraintName);
            AppendLine(nameof(File), File);
            AppendLine(nameof(Line), Line);
            AppendLine(nameof(Routine), Routine);

            return builder.ToString();

            void AppendLine<T>(string propertyName, T propertyValue)
            {
                if (!EqualityComparer<T>.Default.Equals(propertyValue, default!))
                    builder.AppendLine().Append("    ").Append(propertyName).Append(": ").Append(propertyValue);
            }
        }

        /// <summary>
        /// Specifies whether the exception is considered transient, that is, whether retrying the operation could
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

        #region Message Fields

        /// <summary>
        /// Severity of the error or notice.
        /// Always present.
        /// </summary>
        public string Severity { get; }

        /// <summary>
        /// Severity of the error or notice, not localized.
        /// Always present since PostgreSQL 9.6.
        /// </summary>
        public string InvariantSeverity { get; }

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// Constants are defined in <seealso cref="PostgresErrorCodes"/>.
        /// See https://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
#if NET5_0_OR_GREATER
        public override string SqlState { get; }
#else
        public string SqlState { get; }
#endif

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// Constants are defined in <seealso cref="PostgresErrorCodes"/>.
        /// See https://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
        [Obsolete("Use SqlState instead")]
        public string Code => SqlState;

        /// <summary>
        /// The primary human-readable error message. This should be accurate but terse.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// </remarks>
        public string MessageText { get; }

        /// <summary>
        /// An optional secondary error message carrying more detail about the problem.
        /// May run to multiple lines.
        /// </summary>
        public string? Detail { get; }

        /// <summary>
        /// An optional suggestion what to do about the problem.
        /// This is intended to differ from Detail in that it offers advice (potentially inappropriate) rather than hard facts.
        /// May run to multiple lines.
        /// </summary>
        public string? Hint { get; }

        /// <summary>
        /// The field value is a decimal ASCII integer, indicating an error cursor position as an index into the original query string.
        /// The first character has index 1, and positions are measured in characters not bytes.
        /// 0 means not provided.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// This is defined the same as the <see cref="Position"/> field, but it is used when the cursor position refers to an internally generated command rather than the one submitted by the client.
        /// The <see cref="InternalQuery" /> field will always appear when this field appears.
        /// 0 means not provided.
        /// </summary>
        public int InternalPosition { get; }

        /// <summary>
        /// The text of a failed internally-generated command.
        /// This could be, for example, a SQL query issued by a PL/pgSQL function.
        /// </summary>
        public string? InternalQuery { get; }

        /// <summary>
        /// An indication of the context in which the error occurred.
        /// Presently this includes a call stack traceback of active PL functions.
        /// The trace is one entry per line, most recent first.
        /// </summary>
        public string? Where { get; }

        /// <summary>
        /// If the error was associated with a specific database object, the name of the schema containing that object, if any.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? SchemaName { get; }

        /// <summary>
        /// Table name: if the error was associated with a specific table, the name of the table.
        /// (Refer to the schema name field for the name of the table's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? TableName { get; }

        /// <summary>
        /// If the error was associated with a specific table column, the name of the column.
        /// (Refer to the schema and table name fields to identify the table.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? ColumnName { get; }

        /// <summary>
        /// If the error was associated with a specific data type, the name of the data type.
        /// (Refer to the schema name field for the name of the data type's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? DataTypeName { get; }

        /// <summary>
        /// If the error was associated with a specific constraint, the name of the constraint.
        /// Refer to fields listed above for the associated table or domain.
        /// (For this purpose, indexes are treated as constraints, even if they weren't created with constraint syntax.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? ConstraintName { get; }

        /// <summary>
        /// The file name of the source-code location where the error was reported.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        public string? File { get; }

        /// <summary>
        /// The line number of the source-code location where the error was reported.
        /// </summary>
        public string? Line { get; }

        /// <summary>
        /// The name of the source-code routine reporting the error.
        /// </summary>
        public string? Routine { get; }

        #endregion
    }
}
