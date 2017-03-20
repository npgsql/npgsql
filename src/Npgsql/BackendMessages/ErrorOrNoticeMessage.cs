#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using Npgsql.Logging;
#if NET45 || NET451
using System.Runtime.Serialization;
#endif

namespace Npgsql.BackendMessages
{
#if NET45 || NET451
    [Serializable]
#endif
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
        internal ErrorOrNoticeMessage(ReadBuffer buf)
        {
            while (true)
            {
                var code = (ErrorFieldTypeCode)buf.ReadByte();
                switch (code) {
                case ErrorFieldTypeCode.Done:
                    // Null terminator; error message fully consumed.
                    return;
                case ErrorFieldTypeCode.Severity:
                    Severity = buf.ReadNullTerminatedString(PGUtil.RelaxedUTF8Encoding);
                    break;
                case ErrorFieldTypeCode.Code:
                    Code = buf.ReadNullTerminatedString();
                    break;
                case ErrorFieldTypeCode.Message:
                    Message = buf.ReadNullTerminatedString(PGUtil.RelaxedUTF8Encoding);
                    break;
                case ErrorFieldTypeCode.Detail:
                    Detail = buf.ReadNullTerminatedString(PGUtil.RelaxedUTF8Encoding);
                    break;
                case ErrorFieldTypeCode.Hint:
                    Hint = buf.ReadNullTerminatedString(PGUtil.RelaxedUTF8Encoding);
                    break;
                case ErrorFieldTypeCode.Position:
                    var positionStr = buf.ReadNullTerminatedString();
                    if (!int.TryParse(positionStr, out var position)) {
                        Log.Warn("Non-numeric position in ErrorResponse: " + positionStr);
                        continue;
                    }
                    Position = position;
                    break;
                case ErrorFieldTypeCode.InternalPosition:
                    var internalPositionStr = buf.ReadNullTerminatedString();
                    if (!Int32.TryParse(internalPositionStr, out var internalPosition)) {
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
                    File = buf.ReadNullTerminatedString(PGUtil.RelaxedUTF8Encoding);
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
