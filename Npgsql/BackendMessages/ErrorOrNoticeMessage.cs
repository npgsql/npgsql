using System;
using System.Diagnostics.Contracts;
using Common.Logging;

namespace Npgsql.BackendMessages
{
    class ErrorOrNoticeMessage
    {
        static readonly ILog Log = LogManager.GetCurrentClassLogger();

        internal ErrorSeverity Severity { get; private set; }
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

        // ReSharper disable once FunctionComplexityOverflow
        internal ErrorOrNoticeMessage(NpgsqlBuffer buf)
        {
            while (true)
            {
                var code = (ErrorFieldTypeCode)buf.ReadByte();
                switch (code) {
                case ErrorFieldTypeCode.Done:
                    // Null terminator; error message fully consumed.
                    return;
                case ErrorFieldTypeCode.Severity:
                    var severityStr = buf.ReadNullTerminatedString();
                    ErrorSeverity severity;
                    if (!Enum.TryParse(severityStr, true, out severity)) {
                        Log.Warn("Unrecognized severity level in ErrorResponse: " + severityStr);
                        continue;
                    }
                    Severity = severity;
                    break;
                case ErrorFieldTypeCode.Code:
                    Code = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Message:
                    Message = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Detail:
                    Detail = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Hint:
                    Hint = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Position:
                    var positionStr = buf.ReadNullTerminatedString();
                    int position;
                    if (!Int32.TryParse(positionStr, out position)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + positionStr);
                        continue;
                    }
                    Position = position;
                    break;
                case ErrorFieldTypeCode.InternalPosition:
                    var internalPositionStr = buf.ReadNullTerminatedString();
                    int internalPosition;
                    if (!Int32.TryParse(internalPositionStr, out internalPosition)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + internalPositionStr);
                        continue;
                    }
                    InternalPosition = internalPosition;
                    break;
                case ErrorFieldTypeCode.InternalQuery:
                    InternalQuery = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Where:
                    Where = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.File:
                    File = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Line:
                    Line = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Routine:
                    Routine = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.SchemaName:
                    SchemaName = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.TableName:
                    TableName = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.ColumnName:
                    ColumnName = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.DataTypeName:
                    DataTypeName = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.ConstraintName:
                    ConstraintName = buf.ReadNullTerminatedString();
                    break;
                default:
                    // Unknown error field; consume and discard.
                    buf.ReadNullTerminatedString();
                    break;
                }
            }
        }

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(Code != null);
            Contract.Invariant(Message != null);
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
