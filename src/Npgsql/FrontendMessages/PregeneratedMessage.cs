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
            Contract.Requires(data.Length < WriteBuffer.MinimumBufferSize);

            _data = data;
            _description = description;
        }

        internal override int Length => _data.Length;

        internal override void WriteFully(WriteBuffer buf)
        {
            buf.WriteBytes(_data, 0, _data.Length);
        }

        public override string ToString()
        {
            return _description ?? "[?]";
        }

        static readonly WriteBuffer _tempBuf;
        static readonly QueryMessage _tempQuery;

        static PregeneratedMessage()
        {
            _tempBuf = new WriteBuffer(null, new MemoryStream(), WriteBuffer.MinimumBufferSize, Encoding.ASCII);
            _tempQuery = new QueryMessage();

            BeginTrans                = BuildQuery("BEGIN;");
            SetTransRepeatableRead    = BuildQuery("SET TRANSACTION ISOLATION LEVEL REPEATABLE READ;");
            SetTransSerializable      = BuildQuery("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;");
            SetTransReadCommitted     = BuildQuery("SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");
            SetTransReadUncommitted   = BuildQuery("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;");
            CommitTransaction         = BuildQuery("COMMIT");
            RollbackTransaction       = BuildQuery("ROLLBACK");
            DiscardSessionState       = BuildQuery("SET SESSION AUTHORIZATION DEFAULT; RESET ALL; CLOSE ALL; UNLISTEN *; SELECT pg_advisory_unlock_all(); DISCARD TEMP;");
            DiscardSequences          = BuildQuery("DISCARD SEQUENCES");
            UnlistenAll               = BuildQuery("UNLISTEN *");
            KeepAlive                 = BuildQuery("SELECT NULL");

            _tempBuf = null;
            _tempQuery = null;
        }

        static PregeneratedMessage BuildQuery(string query)
        {
            Contract.Requires(query != null && query.All(c => c < 128));

            var totalLen = 5 + query.Length;
            var ms = new MemoryStream(totalLen);
            _tempBuf.Underlying = ms;
            _tempQuery.Populate(query);
            _tempQuery.Write(_tempBuf);
            _tempBuf.Flush();
            return new PregeneratedMessage(ms.ToArray(), _tempQuery.ToString());
        }

        internal static readonly PregeneratedMessage BeginTrans;
        internal static readonly PregeneratedMessage SetTransRepeatableRead;
        internal static readonly PregeneratedMessage SetTransSerializable;
        internal static readonly PregeneratedMessage SetTransReadCommitted;
        internal static readonly PregeneratedMessage SetTransReadUncommitted;
        internal static readonly PregeneratedMessage CommitTransaction;
        internal static readonly PregeneratedMessage RollbackTransaction;
        internal static readonly PregeneratedMessage DiscardSessionState;
        internal static readonly PregeneratedMessage DiscardSequences;
        internal static readonly PregeneratedMessage UnlistenAll;
        internal static readonly PregeneratedMessage KeepAlive;
    }
}
