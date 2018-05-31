#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A simple query message.
    /// </summary>
    class QueryMessage : FrontendMessage
    {
        readonly Encoding _encoding;
        string _query;

        const byte Code = (byte)'Q';

        internal QueryMessage(Encoding encoding)
        {
            _encoding = encoding;
        }

        internal QueryMessage Populate(string query)
        {
            Debug.Assert(query != null);

            _query = query;
            return this;
        }

        internal override async Task Write(NpgsqlWriteBuffer buf, bool async)
        {
            if (buf.WriteSpaceLeft < 1 + 4)
                await buf.Flush(async);
            var queryByteLen = _encoding.GetByteCount(_query);

            buf.WriteByte(Code);
            buf.WriteInt32(4 +            // Message length (including self excluding code)
                           queryByteLen + // Query byte length
                           1);            // Null terminator

            await buf.WriteString(_query, queryByteLen, async);
            if (buf.WriteSpaceLeft < 1)
                await buf.Flush(async);
            buf.WriteByte(0);
        }

        public override string ToString() => $"[Query={_query}]";
    }
}
