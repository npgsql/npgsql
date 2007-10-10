// created on 4/3/2003 at 19:45

// Npgsql.NpgsqlBinaryRow.cs
//
// Author:
//	Francisco Jr. (fxjrlists@yahoo.com.br)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
using System.IO;
using System.Text;
using System.Net;
using NpgsqlTypes;


namespace Npgsql
{

    /// <summary>
    /// This is the abstract base class for NpgsqlAsciiRow and NpgsqlBinaryRow.
    /// </summary>
    internal abstract class NpgsqlRow
    {
        // Logging related values
        private static readonly String CLASSNAME = "NpgsqlRow";

        protected ArrayList                  data;
        protected NpgsqlRowDescription       row_desc;
        protected ProtocolVersion            protocol_version;

        public NpgsqlRow(NpgsqlRowDescription rowDesc, ProtocolVersion protocolVersion)
        {
            data = new ArrayList();
            row_desc = rowDesc;
            protocol_version = protocolVersion;
        }

        public virtual void ReadFromStream(Stream inputStream, Encoding encoding)
        {
          throw new NotImplementedException("Abstract");
        }

        /// <summary>
        /// Provide access to the fields in this row.
        /// </summary>
        public virtual Object this[Int32 index]
        {
            get
            {
                NpgsqlEventLog.LogIndexerGet(LogLevel.Debug, CLASSNAME, index);
                if ((index < 0) || (index >= row_desc.NumFields)) {
                    throw new IndexOutOfRangeException("this[] index value");
                }

                return data[index];
            }
        }
    }

}
