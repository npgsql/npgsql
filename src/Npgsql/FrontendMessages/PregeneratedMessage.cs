#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.IO;
using System.Linq;
using System.Text;

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
        internal PregeneratedMessage(byte[] data, string description=null)
        {
            Contract.Requires(data.Length < NpgsqlBuffer.MinimumBufferSize);

            _data = data;
            _description = description;
        }

        internal override int Length => _data.Length;

        internal override void Write(NpgsqlBuffer buf)
        {
            buf.WriteBytes(_data, 0, _data.Length);
        }

        public override string ToString()
        {
            return _description ?? "[?]";
        }

        static NpgsqlBuffer _tempBuf;

        static PregeneratedMessage()
        {
            _tempBuf = new NpgsqlBuffer(new MemoryStream(), NpgsqlBuffer.MinimumBufferSize, Encoding.ASCII);

            BeginTransRepeatableRead  = BuildQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;");
            BeginTransSerializable    = BuildQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;");
            BeginTransReadCommitted   = BuildQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
            BeginTransReadUncommitted = BuildQuery("BEGIN; SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            CommitTransaction         = BuildQuery("COMMIT");
            RollbackTransaction       = BuildQuery("ROLLBACK");
            DiscardAll                = BuildQuery("DISCARD ALL");
            UnlistenAll               = BuildQuery("UNLISTEN *");
            KeepAlive                 = BuildQuery("SELECT NULL");
            SetStmtTimeout10Sec       = BuildQuery("SET statement_timeout = 10000");
            SetStmtTimeout20Sec       = BuildQuery("SET statement_timeout = 20000");
            SetStmtTimeout30Sec       = BuildQuery("SET statement_timeout = 30000");
            SetStmtTimeout60Sec       = BuildQuery("SET statement_timeout = 60000");
            SetStmtTimeout90Sec       = BuildQuery("SET statement_timeout = 90000");
            SetStmtTimeout120Sec      = BuildQuery("SET statement_timeout = 120000");

            _tempBuf = null;

            BeginTransactionMessages = new[] {
                BeginTransRepeatableRead, BeginTransSerializable, BeginTransReadCommitted, BeginTransReadUncommitted
            };
        }

        static PregeneratedMessage BuildQuery(string query)
        {
            Contract.Requires(query != null && query.All(c => c < 128));

            var totalLen = 5 + query.Length;
            var ms = new MemoryStream(totalLen);
            _tempBuf.Underlying = ms;
            var simpleQuery = new QueryMessage(query);
            simpleQuery.Write(_tempBuf);
            _tempBuf.Flush();
            return new PregeneratedMessage(ms.ToArray(), simpleQuery.ToString());
        }

        internal static readonly PregeneratedMessage BeginTransRepeatableRead;
        internal static readonly PregeneratedMessage BeginTransSerializable;
        internal static readonly PregeneratedMessage BeginTransReadCommitted;
        internal static readonly PregeneratedMessage BeginTransReadUncommitted;
        internal static readonly PregeneratedMessage CommitTransaction;
        internal static readonly PregeneratedMessage RollbackTransaction;
        internal static readonly PregeneratedMessage DiscardAll;
        internal static readonly PregeneratedMessage UnlistenAll;
        internal static readonly PregeneratedMessage KeepAlive;
        internal static readonly PregeneratedMessage SetStmtTimeout10Sec;
        internal static readonly PregeneratedMessage SetStmtTimeout20Sec;
        internal static readonly PregeneratedMessage SetStmtTimeout30Sec;
        internal static readonly PregeneratedMessage SetStmtTimeout60Sec;
        internal static readonly PregeneratedMessage SetStmtTimeout90Sec;
        internal static readonly PregeneratedMessage SetStmtTimeout120Sec;

        internal static readonly PregeneratedMessage[] BeginTransactionMessages;
    }
}
