// created on 12/5/2002 at 23:10

// Npgsql.NpgsqlException.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;

namespace Npgsql
{
    /// <summary>
    /// The exception that is thrown when the PostgreSQL backend reports errors.
    /// </summary>
    [Serializable]
    public sealed class NpgsqlException : DbException
    {
        private readonly NpgsqlError[] errors;

        // Logging related values
        //private static readonly String CLASSNAME = MethodBase.GetCurrentMethod().DeclaringType.Name;
        private static readonly ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);

        // To allow deserialization.
        private NpgsqlException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IList<NpgsqlError> l = (IList<NpgsqlError>) info.GetValue("errors", typeof (IList<NpgsqlError>));

            errors = new NpgsqlError[l.Count];
            l.CopyTo(errors, 0);
        }

        /// <summary>
        /// Construct a backend error exception based on a list of one or more
        /// backend errors.  The basic Exception.Message will be built from the
        /// first (usually the only) error in the list.
        /// </summary>
        internal NpgsqlException(IList<NpgsqlError> errors)
            : base(errors[0].ToString())
        {
            NpgsqlEventLog.LogMsg(resman, "Log_ExceptionOccured", LogLevel.Normal, Message);

            this.errors = new NpgsqlError[errors.Count];
            errors.CopyTo(this.errors, 0);
        }

        internal NpgsqlException(String message)
            : this(message, null)
        {
        }

        internal NpgsqlException(String message, Exception innerException)
            : base(message, innerException)
        {
            NpgsqlEventLog.LogMsg(resman, "Log_ExceptionOccured", LogLevel.Normal, Message);

            errors = new NpgsqlError[] { new NpgsqlError(message) };
        }

        /// <summary>
        /// Get object data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            // Add custom data, in this case the list of errors when serializing.
            // Thanks Robert Chartier for info: http://www.15seconds.com/issue/020903.htm

            //use the info object to add the items you want serialized
            info.AddValue("errors", errors, typeof (IList));
        }

        /// <summary>
        /// Provide access to the entire list of errors provided by the PostgreSQL backend.
        /// </summary>
        public NpgsqlError this[Int32 Index]
        {
            get { return (NpgsqlError) errors[Index]; }
        }

        /// <summary>
        /// Severity code.  All versions.
        /// </summary>
        public String Severity
        {
            get { return this[0].Severity; }
        }

        /// <summary>
        /// Error code.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Code
        {
            get { return this[0].Code; }
        }

        /// <summary>
        /// Basic error message.  All versions.
        /// </summary>
        public String BaseMessage
        {
            get { return this[0].Message; }
        }

        /// <summary>
        /// Detailed error message.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Detail
        {
            get { return this[0].Detail; }
        }

        /// <summary>
        /// Suggestion to help resolve the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Hint
        {
            get { return this[0].Hint; }
        }

        /// <summary>
        /// Position (one based) within the query string where the error was encounterd.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Position
        {
            get { return this[0].Position; }
        }

        /// <summary>
        /// Trace back information.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Where
        {
            get { return this[0].Where; }
        }

        /// <summary>
        /// Source file (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String File
        {
            get { return this[0].File; }
        }

        /// <summary>
        /// Source file line number (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Line
        {
            get { return this[0].Line; }
        }

        /// <summary>
        /// Source routine (in backend) reporting the error.  PostgreSQL 7.4 and up.
        /// </summary>
        public String Routine
        {
            get { return this[0].Routine; }
        }

        /// <summary>
        /// Schema name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String SchemaName
        {
            get { return this[0].SchemaName; }
        }

        /// <summary>
        /// Table name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String TableName
        {
            get { return this[0].TableName; }
        }

        /// <summary>
        /// Column name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ColumnName
        {
            get { return this[0].ColumnName; }
        }

        /// <summary>
        /// Data type of column which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String DataTypeName
        {
            get { return this[0].DataTypeName; }
        }

        /// <summary>
        /// Constraint name which relates to the error. PostgreSQL 9.3 and up.
        /// </summary>
        public String ConstraintName
        {
            get { return this[0].ConstraintName; }
        }

        /// <summary>
        /// String containing the sql sent which produced this error.
        /// </summary>
        public String ErrorSql
        {
            get { return this[0].ErrorSql; }
        }

        /// <summary>
        /// Returns the entire list of errors provided by the PostgreSQL backend.
        /// </summary>
        public IList Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Format a .NET style exception string.
        /// Include all errors in the list, including any hints.
        /// </summary>
        public override String ToString()
        {
            if (Errors != null)
            {
                StringWriter S = new StringWriter();

                S.WriteLine("{0}:", this.GetType().FullName);

                foreach (NpgsqlError PgError in Errors)
                {
                    AppendString(S, "{0}", PgError.Message);
                    AppendString(S, "Severity: {0}", PgError.Severity);
                    AppendString(S, "Code: {0}", PgError.Code);
                    AppendString(S, "Hint: {0}", PgError.Hint);
                }

                S.Write(StackTrace);

                return S.ToString();
            }

            return base.ToString();
        }

        /// <summary>
        /// Append a line to the given Stream, first checking for zero-length.
        /// </summary>
        private static void AppendString(StringWriter Stream, string Format, string Str)
        {
            if (Str.Length > 0)
            {
                Stream.WriteLine(Format, Str);
            }
        }
    }
}
