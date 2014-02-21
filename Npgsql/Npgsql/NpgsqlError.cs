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
    /// This class represents the ErrorResponse and NoticeResponse
    /// message sent from PostgreSQL server.
    /// </summary>
    [Serializable]
    public sealed class NpgsqlError
    {
        /// <summary>
        /// Error and notice message field codes
        /// </summary>
        private enum ErrorFieldTypeCodes : byte
        {
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
            SchemaName =  (byte)'s',

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

        private readonly String _severity = String.Empty;
        private readonly String _code = String.Empty;
        private readonly String _message = String.Empty;
        private readonly String _detail = String.Empty;
        private readonly String _hint = String.Empty;
        private readonly String _position = String.Empty;
        private readonly String _internalPosition = String.Empty;
        private readonly String _internalQuery = String.Empty;
        private readonly String _where = String.Empty;
        private readonly String _file = String.Empty;
        private readonly String _line = String.Empty;
        private readonly String _routine = String.Empty;
        private readonly String _schemaName = String.Empty;
        private readonly String _tableName = String.Empty;
        private readonly String _columnName = String.Empty;
        private readonly String _datatypeName = String.Empty;
        private readonly String _constraintName = String.Empty;
        private String _errorSql = String.Empty;

        /// <summary>
        /// Severity code.  All versions.
        /// </summary>
        public String Severity
        {
            get { return _severity; }
        }

        /// <summary>
        /// Error code.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Terse error message.  All versions.
        /// </summary>
        public String Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Detailed error message.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Detail
        {
            get { return _detail; }
        }

        /// <summary>
        /// Suggestion to help resolve the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Hint
        {
            get { return _hint; }
        }

        /// <summary>
        /// Position (one based) within the query string where the error was encounterd.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Position (one based) within the query string where the error was encounterd.  This position refers to an internal command executed for example inside a PL/pgSQL function. PostgreSQL 7.4 and up.
        /// </summary>
        public String InternalPosition
        {
            get { return _internalPosition; }
        }

        /// <summary>
        /// Internal query string where the error was encounterd.  This position refers to an internal command executed for example inside a PL/pgSQL function. PostgreSQL 7.4 and up.
        /// </summary>
        public String InternalQuery
        {
            get { return _internalQuery; }
        }
        /// <summary>
        /// Trace back information.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Where
        {
            get { return _where; }
        }

        /// <summary>
        /// Source file (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String File
        {
            get { return _file; }
        }

        /// <summary>
        /// Source file line number (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Line
        {
            get { return _line; }
        }

        /// <summary>
        /// Source routine (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Routine
        {
            get { return _routine; }
        }

        /// <summary>
        /// Schema name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String SchemaName
        {
            get { return _schemaName; }
        }

        /// <summary>
        /// Table name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String TableName
        {
            get { return _tableName; }
        }

        /// <summary>
        /// Column name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ColumnName
        {
            get { return _columnName; }
        }

        /// <summary>
        /// Data type of column which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String DataTypeName
        {
            get { return _datatypeName; }
        }

        /// <summary>
        /// Constraint name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ConstraintName
        {
            get { return _constraintName; }
        }

        /// <summary>
        /// String containing the sql sent which produced this error.
        /// </summary>
        public String ErrorSql
        {
            set { _errorSql = value; }
            get { return _errorSql; }
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

        internal NpgsqlError(Stream stream)
        {
            // Check the messageLength value. If it is 1178686529, this would be the
            // "FATA" string, which would mean a protocol 2.0 error string.
            if (PGUtil.ReadInt32(stream) == 1178686529)
            {
                string[] v2Parts = ("FATA" + PGUtil.ReadString(stream)).Split(new char[] {':'}, 2);
                if (v2Parts.Length == 2)
                {
                    _severity = v2Parts[0].Trim();
                    _message = v2Parts[1].Trim();
                }
                else
                {
                    _severity = string.Empty;
                    _message = v2Parts[0].Trim();
                }
            }
            else
            {
                bool done = false;
                int fieldCode;

                while (! done && (fieldCode = stream.ReadByte()) != -1)
                {
                    switch ((byte)fieldCode)
                    {
                        case 0 :
                            // Null terminator; error message fully consumed.
                            done = true;
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Severity :
                            _severity = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Code :
                            _code = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Message :
                            _message = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Detail :
                            _detail = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Hint :
                            _hint = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Position :
                            _position = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.InternalPosition :
                            _internalPosition = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.InternalQuery :
                            _internalQuery = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Where :
                            _where = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.File :
                            _file = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Line :
                            _line = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.Routine :
                            _routine = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.SchemaName :
                            _schemaName = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.TableName :
                            _tableName = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.ColumnName :
                            _columnName = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.DataTypeName :
                            _datatypeName = PGUtil.ReadString(stream);
                            ;
                            break;
                        case (byte)ErrorFieldTypeCodes.ConstraintName :
                            _constraintName = PGUtil.ReadString(stream);
                            ;
                            break;
                        default:
                            // Unknown error field; consume and discard.
                            PGUtil.ReadString(stream);
                            ;
                            break;

                    }
                }
            }
        }

        internal NpgsqlError(String errorMessage)
        {
            _message = errorMessage;
        }

        /// <summary>
        /// Backend protocol version in use.
        /// </summary>
        internal ProtocolVersion BackendProtocolVersion
        {
            get { return ProtocolVersion.Version3; }
        }
    }
}
