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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Npgsql.FrontendMessages
{
    /// <summary>
    /// A frontend message of an arbitrary type that has been pregenerated for efficiency - it is kept
    /// in byte[] form and doesn't have to be serialized for each send.
    /// </summary>
    class PregeneratedMessage : SimpleFrontendMessage
    {
        readonly byte[] _data;
        readonly string _description;

        /// <summary>
        /// Constructs a new pregenerated message.
        /// </summary>
        /// <param name="data">The data to be sent for this message, not including the 4-byte length.</param>
        /// <param name="description">Optional string form/description for debugging</param>
        /// <param name="responseMessageCount">Returns how many messages PostgreSQL is expected to send in response to this message.</param>
        internal PregeneratedMessage(byte[] data, string description, int responseMessageCount)
        {
            Debug.Assert(data.Length < NpgsqlWriteBuffer.MinimumSize);

            _data = data;
            _description = description;
            ResponseMessageCount = responseMessageCount;
        }

        internal override int Length => _data.Length;

        internal override int ResponseMessageCount { get; }

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteBytes(_data, 0, _data.Length);
        }

        public override string ToString() =>  _description ?? "[?]";

        static PregeneratedMessage()
        {
            var buf = new NpgsqlWriteBuffer(null, new MemoryStream(), NpgsqlWriteBuffer.MinimumSize, Encoding.ASCII);
            var message = new QueryMessage(PGUtil.UTF8Encoding);

            BeginTrans                = Generate(buf, message, "BEGIN");
            SetTransRepeatableRead    = Generate(buf, message, "SET TRANSACTION ISOLATION LEVEL REPEATABLE READ");
            SetTransSerializable      = Generate(buf, message, "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE");
            SetTransReadCommitted     = Generate(buf, message, "SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
            SetTransReadUncommitted   = Generate(buf, message, "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            CommitTransaction         = Generate(buf, message, "COMMIT");
            RollbackTransaction       = Generate(buf, message, "ROLLBACK");
            KeepAlive                 = Generate(buf, message, "SELECT NULL");

            DiscardAll                = Generate(buf, message, "DISCARD ALL");
        }

        internal static PregeneratedMessage Generate(NpgsqlWriteBuffer buf, QueryMessage queryMessage, string query, int responseMessageCount=2)
        {
            Debug.Assert(query != null && query.All(c => c < 128));
            queryMessage.Populate(query);
            var description = queryMessage.ToString();
            queryMessage.Write(buf, false).Wait();
            var bytes = buf.GetContents();
            buf.Clear();
            return new PregeneratedMessage(bytes, description, responseMessageCount);
        }

        internal static readonly PregeneratedMessage BeginTrans;
        internal static readonly PregeneratedMessage SetTransRepeatableRead;
        internal static readonly PregeneratedMessage SetTransSerializable;
        internal static readonly PregeneratedMessage SetTransReadCommitted;
        internal static readonly PregeneratedMessage SetTransReadUncommitted;
        internal static readonly PregeneratedMessage CommitTransaction;
        internal static readonly PregeneratedMessage RollbackTransaction;
        internal static readonly PregeneratedMessage KeepAlive;

        internal static readonly PregeneratedMessage DiscardAll;
    }
}
