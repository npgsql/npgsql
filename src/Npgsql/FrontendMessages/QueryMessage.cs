﻿#region License
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
    ///
    /// Note that since this is only used to send some specific control messages (e.g. start transaction)
    /// and never arbitrary-length user-provided queries, this message is treated as simple and not chunking,
    /// and only ASCII is supported.
    /// </summary>
    class QueryMessage : SimpleFrontendMessage
    {
        string Query { get; set; }

        internal const byte Code = (byte)'Q';

        internal QueryMessage(string query)
        {
            Contract.Requires(query != null && query.All(c => c < 128), "Not ASCII");
            Contract.Requires(PGUtil.UTF8Encoding.GetByteCount(query) + 5 < NpgsqlBuffer.MinimumBufferSize, "Too long");

            Query = query;
        }

        internal override int Length => 1 + 4 + (Query.Length + 1);

        internal override void Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Query != null && Query.All(c => c < 128));

            buf.WriteByte(Code);
            buf.WriteInt32(Length - 1);
            buf.WriteBytesNullTerminated(Encoding.ASCII.GetBytes(Query));
        }

        public override string ToString()
        {
            return $"[Query={Query}]";
        }
    }
}
