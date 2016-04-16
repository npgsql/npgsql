#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A simple query message.
    /// </summary>
    class QueryMessage : FrontendMessage
    {
        string _query;
        char[] _queryChars;
        int _charPos;

        internal const byte Code = (byte)'Q';

        internal QueryMessage Populate(string query)
        {
            Contract.Requires(query != null);

            _query = query;
            _charPos = -1;
            return this;
        }

        internal override bool Write(NpgsqlBuffer buf)
        {
            if (_charPos == -1)
            {
                // Start new query
                if (buf.WriteSpaceLeft < 1 + 4)
                    return false;
                _charPos = 0;
                var queryByteLen = PGUtil.UTF8Encoding.GetByteCount(_query);
                _queryChars = _query.ToCharArray();
                buf.WriteByte(Code);
                buf.WriteInt32(4 +            // Message length (including self excluding code)
                               queryByteLen + // Query byte length
                               1);            // Null terminator
            }

            if (_charPos < _queryChars.Length)
            {
                int charsUsed;
                bool completed;
                buf.WriteStringChunked(_queryChars, _charPos, _queryChars.Length - _charPos, true,
                                       out charsUsed, out completed);
                _charPos += charsUsed;
                if (!completed)
                    return false;
            }

            if (buf.WriteSpaceLeft < 1)
                return false;
            buf.WriteByte(0);

            _charPos = -1;
            return true;
        }

        public override string ToString()
        {
            return $"[Query={_query}]";
        }
    }
}
