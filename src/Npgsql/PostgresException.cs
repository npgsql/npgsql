using System;
using System.Collections;
using System.Collections.Generic;
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
        bool _dataInitialized;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PostgresException(string messageText, string severity, string invariantSeverity, string sqlState)
        {
            MessageText = messageText;
            Severity = severity;
            InvariantSeverity = invariantSeverity;
            SqlState = sqlState;
        }

        PostgresException(ErrorOrNoticeMessage msg)
        {
            Severity = msg.Severity;
            InvariantSeverity = msg.InvariantSeverity;
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

        internal static PostgresException Load(NpgsqlReadBuffer buf)
            => new PostgresException(ErrorOrNoticeMessage.Load(buf));

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
        public NpgsqlStatement? Statement { get; internal set; }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional PostgreSQL fields about the exception.
        /// </summary>
        public override IDictionary Data
        {
            get
            {
                var data = base.Data;
                if (_dataInitialized)
                    return data;

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

                _dataInitialized = true;
                return data;

                void AddData<T>(string key, T value)
                {
                    if (!EqualityComparer<T>.Default.Equals(value, default!))
                        data.Add(key, value);
                }
            }
        }

        #region Message Fields

        /// <summary>
        /// Severity of the error or notice.
        /// Always present.
        /// </summary>
        [PublicAPI]
        public string Severity { get; }

        /// <summary>
        /// Severity of the error or notice, not localized.
        /// Always present since PostgreSQL 9.6.
        /// </summary>
        [PublicAPI]
        public string InvariantSeverity { get; }

        /// <summary>
        /// The SQLSTATE code for the error.
        /// </summary>
        /// <remarks>
        /// Always present.
        /// Constants are defined in <seealso cref="PostgresErrorCodes"/>.
        /// See http://www.postgresql.org/docs/current/static/errcodes-appendix.html
        /// </remarks>
        [PublicAPI]
        public string SqlState { get; }

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
        public string MessageText { get; }

        /// <summary>
        /// An optional secondary error message carrying more detail about the problem.
        /// May run to multiple lines.
        /// </summary>
        [PublicAPI]
        public string? Detail { get; }

        /// <summary>
        /// An optional suggestion what to do about the problem.
        /// This is intended to differ from Detail in that it offers advice (potentially inappropriate) rather than hard facts.
        /// May run to multiple lines.
        /// </summary>
        [PublicAPI]
        public string? Hint { get; }

        /// <summary>
        /// The field value is a decimal ASCII integer, indicating an error cursor position as an index into the original query string.
        /// The first character has index 1, and positions are measured in characters not bytes.
        /// 0 means not provided.
        /// </summary>
        [PublicAPI]
        public int Position { get; }

        /// <summary>
        /// This is defined the same as the <see cref="Position"/> field, but it is used when the cursor position refers to an internally generated command rather than the one submitted by the client.
        /// The <see cref="InternalQuery" /> field will always appear when this field appears.
        /// 0 means not provided.
        /// </summary>
        [PublicAPI]
        public int InternalPosition { get; }

        /// <summary>
        /// The text of a failed internally-generated command.
        /// This could be, for example, a SQL query issued by a PL/pgSQL function.
        /// </summary>
        [PublicAPI]
        public string? InternalQuery { get; }

        /// <summary>
        /// An indication of the context in which the error occurred.
        /// Presently this includes a call stack traceback of active PL functions.
        /// The trace is one entry per line, most recent first.
        /// </summary>
        [PublicAPI]
        public string? Where { get; }

        /// <summary>
        /// If the error was associated with a specific database object, the name of the schema containing that object, if any.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? SchemaName { get; }

        /// <summary>
        /// Table name: if the error was associated with a specific table, the name of the table.
        /// (Refer to the schema name field for the name of the table's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? TableName { get; }

        /// <summary>
        /// If the error was associated with a specific table column, the name of the column.
        /// (Refer to the schema and table name fields to identify the table.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? ColumnName { get; }

        /// <summary>
        /// If the error was associated with a specific data type, the name of the data type.
        /// (Refer to the schema name field for the name of the data type's schema.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? DataTypeName { get; }

        /// <summary>
        /// If the error was associated with a specific constraint, the name of the constraint.
        /// Refer to fields listed above for the associated table or domain.
        /// (For this purpose, indexes are treated as constraints, even if they weren't created with constraint syntax.)
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? ConstraintName { get; }

        /// <summary>
        /// The file name of the source-code location where the error was reported.
        /// </summary>
        /// <remarks>PostgreSQL 9.3 and up.</remarks>
        [PublicAPI]
        public string? File { get; }

        /// <summary>
        /// The line number of the source-code location where the error was reported.
        /// </summary>
        [PublicAPI]
        public string? Line { get; }

        /// <summary>
        /// The name of the source-code routine reporting the error.
        /// </summary>
        [PublicAPI]
        public string? Routine { get; }

        #endregion
    }
}
