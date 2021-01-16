using System;
using Npgsql.Logging;

namespace Npgsql.BackendMessages
{
    [Serializable]
    class ErrorOrNoticeMessage
    {
        internal string Severity { get; }
        internal string InvariantSeverity { get; }
        internal string SqlState { get; }
        internal string Message { get; }
        internal string? Detail { get; }
        internal string? Hint { get; }
        internal int Position { get; }
        internal int InternalPosition { get; }
        internal string? InternalQuery { get; }
        internal string? Where { get; }
        internal string? SchemaName { get; }
        internal string? TableName { get; }
        internal string? ColumnName { get; }
        internal string? DataTypeName { get; }
        internal string? ConstraintName { get; }
        internal string? File { get; }
        internal string? Line { get; }
        internal string? Routine { get; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(ErrorOrNoticeMessage));

        // ReSharper disable once FunctionComplexityOverflow
        internal static ErrorOrNoticeMessage Load(NpgsqlReadBuffer buf, bool includeDetail)
        {
            (string? severity, string? invariantSeverity, string? code, string? message, string? detail, string? hint) = (null, null, null, null, null, null);
            var (position, internalPosition) = (0, 0);
            (string? internalQuery, string? where) = (null, null);
            (string? schemaName, string? tableName, string? columnName, string? dataTypeName, string? constraintName) =
                (null, null, null, null, null);
            (string? file, string? line, string? routine) = (null, null, null);

            while (true)
            {
                var fieldCode = (ErrorFieldTypeCode)buf.ReadByte();
                switch (fieldCode) {
                case ErrorFieldTypeCode.Done:
                    // Null terminator; error message fully consumed.
                    goto End;
                case ErrorFieldTypeCode.Severity:
                    severity = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.InvariantSeverity:
                    invariantSeverity = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Code:
                    code = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Message:
                    message = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Detail:
                    detail = buf.ReadNullTerminatedStringRelaxed();
                    if (!includeDetail && !string.IsNullOrEmpty(detail))
                        detail = $"Detail redacted as it may contain sensitive data. Specify '{NpgsqlConnectionStringBuilder.IncludeExceptionDetailDisplayName}' in the connection string to include this information.";
                    break;
                case ErrorFieldTypeCode.Hint:
                    hint = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Position:
                    var positionStr = buf.ReadNullTerminatedStringRelaxed();
                    if (!int.TryParse(positionStr, out var tmpPosition)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + positionStr);
                        continue;
                    }
                    position = tmpPosition;
                    break;
                case ErrorFieldTypeCode.InternalPosition:
                    var internalPositionStr = buf.ReadNullTerminatedStringRelaxed();
                    if (!int.TryParse(internalPositionStr, out var internalPositionTmp)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + internalPositionStr);
                        continue;
                    }
                    internalPosition = internalPositionTmp;
                    break;
                case ErrorFieldTypeCode.InternalQuery:
                    internalQuery = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Where:
                    where = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.File:
                    file = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Line:
                    line = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Routine:
                    routine = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.SchemaName:
                    schemaName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.TableName:
                    tableName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.ColumnName:
                    columnName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.DataTypeName:
                    dataTypeName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.ConstraintName:
                    constraintName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                default:
                    // Unknown error field; consume and discard.
                    buf.ReadNullTerminatedStringRelaxed();
                    break;
                }
            }

            End:
            if (severity == null)
                throw new NpgsqlException("Severity not received in server error message");
            if (code == null)
                throw new NpgsqlException("Code not received in server error message");
            if (message == null)
                throw new NpgsqlException("Message not received in server error message");

            return new ErrorOrNoticeMessage(
                severity, invariantSeverity ?? severity, code, message,
                detail, hint, position, internalPosition, internalQuery, where,
                schemaName, tableName, columnName, dataTypeName, constraintName,
                file, line, routine);

        }

        internal ErrorOrNoticeMessage(
            string severity, string invariantSeverity, string sqlState, string message,
            string? detail = null, string? hint = null, int position = 0, int internalPosition = 0, string? internalQuery = null, string? where = null,
            string? schemaName = null, string? tableName = null, string? columnName = null, string? dataTypeName = null, string? constraintName = null,
            string? file = null, string? line = null, string? routine = null)
        {
            Severity = severity;
            InvariantSeverity = invariantSeverity;
            SqlState = sqlState;
            Message = message;
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
        }

        /// <summary>
        /// Error and notice message field codes
        /// </summary>
        internal enum ErrorFieldTypeCode : byte
        {
            Done = 0,
            Severity = (byte)'S',
            InvariantSeverity = (byte)'V',
            Code = (byte)'C',
            Message = (byte)'M',
            Detail = (byte)'D',
            Hint = (byte)'H',
            Position = (byte)'P',
            InternalPosition = (byte)'p',
            InternalQuery = (byte)'q',
            Where = (byte)'W',
            SchemaName = (byte)'s',
            TableName = (byte)'t',
            ColumnName = (byte)'c',
            DataTypeName = (byte)'d',
            ConstraintName = (byte)'n',
            File = (byte)'F',
            Line = (byte)'L',
            Routine = (byte)'R'
        }
    }
}
