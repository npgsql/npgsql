using System;
using Npgsql.Logging;

namespace Npgsql.BackendMessages
{
    [Serializable]
    class ErrorOrNoticeMessage
    {
        internal string Severity { get; private set; }
        internal string Code { get; private set; }
        internal string Message { get; private set; }
        internal string Detail { get; private set; }
        internal string Hint { get; private set; }
        internal int Position { get; private set; }
        internal int InternalPosition { get; private set; }
        internal string InternalQuery { get; private set; }
        internal string Where { get; private set; }
        internal string SchemaName { get; private set; }
        internal string TableName { get; private set; }
        internal string ColumnName { get; private set; }
        internal string DataTypeName { get; private set; }
        internal string ConstraintName { get; private set; }
        internal string File { get; private set; }
        internal string Line { get; private set; }
        internal string Routine { get; private set; }

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        // ReSharper disable once FunctionComplexityOverflow
        internal ErrorOrNoticeMessage(NpgsqlReadBuffer buf)
        {
            while (true)
            {
                var code = (ErrorFieldTypeCode)buf.ReadByte();
                switch (code) {
                case ErrorFieldTypeCode.Done:
                    // Null terminator; error message fully consumed.
                    return;
                case ErrorFieldTypeCode.Severity:
                    Severity = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Code:
                    Code = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Message:
                    Message = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Detail:
                    Detail = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Hint:
                    Hint = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Position:
                    var positionStr = buf.ReadNullTerminatedStringRelaxed();
                    if (!int.TryParse(positionStr, out var position)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + positionStr);
                        continue;
                    }
                    Position = position;
                    break;
                case ErrorFieldTypeCode.InternalPosition:
                    var internalPositionStr = buf.ReadNullTerminatedStringRelaxed();
                    if (!int.TryParse(internalPositionStr, out var internalPosition)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + internalPositionStr);
                        continue;
                    }
                    InternalPosition = internalPosition;
                    break;
                case ErrorFieldTypeCode.InternalQuery:
                    InternalQuery = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Where:
                    Where = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.File:
                    File = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Line:
                    Line = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.Routine:
                    Routine = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.SchemaName:
                    SchemaName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.TableName:
                    TableName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.ColumnName:
                    ColumnName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.DataTypeName:
                    DataTypeName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                case ErrorFieldTypeCode.ConstraintName:
                    ConstraintName = buf.ReadNullTerminatedStringRelaxed();
                    break;
                default:
                    // Unknown error field; consume and discard.
                    buf.ReadNullTerminatedStringRelaxed();
                    break;
                }
            }
        }

        /// <summary>
        /// Error and notice message field codes
        /// </summary>
        enum ErrorFieldTypeCode : byte
        {
            Done = 0,
            Severity = (byte)'S',
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
