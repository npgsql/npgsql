// created on 12/7/2003 at 18:36

// Npgsql.NpgsqlError.cs
//
// Author:
//    Francisco Jr. (fxjrlists@yahoo.com.br)
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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

using System;
using System.IO;
using System.Text;

namespace Npgsql
{
    /// <summary>
    /// This class represents the ErrorResponse and NoticeResponse message sent from PostgreSQL server.
    /// </summary>
    [Serializable]
    public sealed class NpgsqlError
    {
        /// <summary>
        /// Severity code.  All versions.
        /// </summary>
        public String Severity { get; private set; }

        /// <summary>
        /// Error code.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Code { get; private set; }

        /// <summary>
        /// Terse error message.  All versions.
        /// </summary>
        public String Message { get; private set; }

        /// <summary>
        /// Detailed error message.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Detail { get; private set; }

        /// <summary>
        /// Suggestion to help resolve the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Hint { get; private set; }

        /// <summary>
        /// Position (one based) within the query string where the error was encounterd.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Position { get; private set; }

        /// <summary>
        /// Position (one based) within the query string where the error was encounterd.  This position refers to an internal command executed for example inside a PL/pgSQL function. PostgreSQL 7.4 and up.
        /// </summary>
        public String InternalPosition { get; private set; }

        /// <summary>
        /// Internal query string where the error was encounterd.  This position refers to an internal command executed for example inside a PL/pgSQL function. PostgreSQL 7.4 and up.
        /// </summary>
        public String InternalQuery { get; private set; }

        /// <summary>
        /// Trace back information.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Where { get; private set; }

        /// <summary>
        /// Source file (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String File { get; private set; }

        /// <summary>
        /// Source file line number (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Line { get; private set; }

        /// <summary>
        /// Source routine (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Routine { get; private set; }

        /// <summary>
        /// Schema name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String SchemaName { get; private set; }

        /// <summary>
        /// Table name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String TableName { get; private set; }

        /// <summary>
        /// Column name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ColumnName { get; private set; }

        /// <summary>
        /// Data type of column which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String DataTypeName { get; private set; }

        /// <summary>
        /// Constraint name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ConstraintName { get; private set; }

        /// <summary>
        /// String containing the sql sent which produced this error.
        /// </summary>
        public String ErrorSql { get; private set; }

        internal NpgsqlError(NpgsqlBuffer buf) : this()
        {
            while (true)
            {
                var code = (ErrorFieldTypeCode)buf.ReadByte();
                switch (code)
                {
                    case ErrorFieldTypeCode.Done:
                        // Null terminator; error message fully consumed.
                        return;
                    case ErrorFieldTypeCode.Severity:
                        Severity = buf.ReadNullTerminatedString();
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
                        Position = buf.ReadNullTerminatedString();
                        break;
                    case ErrorFieldTypeCode.InternalPosition:
                        InternalPosition = buf.ReadNullTerminatedString();
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

        internal NpgsqlError(String errorMessage) : this()
        {
            Message = errorMessage;
        }

        NpgsqlError()
        {
            Severity         = String.Empty;
            Code             = String.Empty;
            Message          = String.Empty;
            Detail           = String.Empty;
            Hint             = String.Empty;
            Position         = String.Empty;
            InternalPosition = String.Empty;
            InternalQuery    = String.Empty;
            Where            = String.Empty;
            File             = String.Empty;
            Line             = String.Empty;
            Routine          = String.Empty;
            SchemaName       = String.Empty;
            TableName        = String.Empty;
            ColumnName       = String.Empty;
            DataTypeName     = String.Empty;
            ConstraintName   = String.Empty;
            ErrorSql         = String.Empty;
        }

        /// <summary>
        /// Return a string representation of this error object.
        /// </summary>
        public override String ToString()
        {
            StringBuilder B = new StringBuilder();

            if (Severity.Length > 0)
            {
                B.AppendFormat("{0}: ", Severity);
            }
            if (Code.Length > 0)
            {
                B.AppendFormat("{0}: ", Code);
            }
            B.AppendFormat("{0}", Message);
            // CHECKME - possibly multi-line, that is yucky
            //            if (Hint.Length > 0) {
            //                B.AppendFormat(" ({0})", Hint);
            //            }

            return B.ToString();
        }

        /// <summary>
        /// Backend protocol version in use.
        /// </summary>
        internal ProtocolVersion BackendProtocolVersion
        {
            get { return ProtocolVersion.Version3; }
        }
    }

    /// <summary>
    /// EventArgs class to send Notice parameters, which are just NpgsqlError's in a lighter context.
    /// </summary>
    public class NpgsqlNoticeEventArgs : EventArgs
    {
        /// <summary>
        /// Notice information.
        /// </summary>
        public NpgsqlError Notice = null;

        internal NpgsqlNoticeEventArgs(NpgsqlError eNotice)
        {
            Notice = eNotice;
        }
    }

    /// <summary>
    /// Error and notice message field codes
    /// </summary>
    enum ErrorFieldTypeCode : byte
    {
        /// <summary>
        /// Terminator, marks the end of the error data.
        /// </summary>
        Done = 0,

        /// <summary>
        /// Severity: the field contents are ERROR, FATAL, or PANIC (in an error message),
        /// or WARNING, NOTICE, DEBUG, INFO, or LOG (in a notice message), or a localized
        /// translation of one of these. Always present.
        /// </summary>
        Severity = (byte)'S',

        /// <summary>
        /// Code: the SQLSTATE code for the error (see Appendix A). Not localizable. Always present.
        /// </summary>
        Code = (byte)'C',

        /// <summary>
        /// Message: the primary human-readable error message. This should be accurate
        /// but terse (typically one line). Always present.
        /// </summary>
        Message = (byte)'M',

        /// <summary>
        /// Detail: an optional secondary error message carrying more detail about the problem.
        /// Might run to multiple lines.
        /// </summary>
        Detail = (byte)'D',

        /// <summary>
        /// Hint: an optional suggestion what to do about the problem. This is intended to differ
        /// from Detail in that it offers advice (potentially inappropriate) rather than hard facts.
        /// Might run to multiple lines.
        /// </summary>
        Hint = (byte)'H',

        /// <summary>
        /// Position: the field value is a decimal ASCII integer, indicating an error cursor
        /// position as an index into the original query string. The first character has index 1,
        /// and positions are measured in characters not bytes.
        /// </summary>
        Position = (byte)'P',

        /// <summary>
        /// Internal position: this is defined the same as the P field, but it is used when the
        /// cursor position refers to an internally generated command rather than the one submitted
        /// by the client.
        /// The q field will always appear when this field appears.
        /// </summary>
        InternalPosition = (byte)'p',

        /// <summary>
        /// Internal query: the text of a failed internally-generated command.
        /// This could be, for example, a SQL query issued by a PL/pgSQL function.
        /// </summary>
        InternalQuery = (byte)'q',

        /// <summary>
        /// Where: an indication of the context in which the error occurred.
        /// Presently this includes a call stack traceback of active procedural language functions
        /// and internally-generated queries. The trace is one entry per line, most recent first.
        /// </summary>
        Where = (byte)'W',

        /// <summary>
        /// Schema name: if the error was associated with a specific database object,
        /// the name of the schema containing that object, if any.
        /// </summary>
        SchemaName = (byte)'s',

        /// <summary>
        /// Table name: if the error was associated with a specific table, the name of the table.
        /// (Refer to the schema name field for the name of the table's schema.)
        /// </summary>
        TableName = (byte)'t',

        /// <summary>
        /// Column name: if the error was associated with a specific table column, the name of the column.
        /// (Refer to the schema and table name fields to identify the table.)
        /// </summary>
        ColumnName = (byte)'c',

        /// <summary>
        /// Data type name: if the error was associated with a specific data type, the name of the data type.
        /// (Refer to the schema name field for the name of the data type's schema.)
        /// </summary>
        DataTypeName = (byte)'d',

        /// <summary>
        /// Constraint name: if the error was associated with a specific constraint, the name of the constraint.
        /// Refer to fields listed above for the associated table or domain.
        /// (For this purpose, indexes are treated as constraints, even if they weren't created with constraint syntax.)
        /// </summary>
        ConstraintName = (byte)'n',

        /// <summary>
        /// File: the file name of the source-code location where the error was reported.
        /// </summary>
        File = (byte)'F',

        /// <summary>
        /// Line: the line number of the source-code location where the error was reported.
        /// </summary>
        Line = (byte)'L',

        /// <summary>
        /// Routine: the name of the source-code routine reporting the error.
        /// </summary>
        Routine = (byte)'R'
    }
}
