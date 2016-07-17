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
using System.Diagnostics;
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
        /// <param name="responseMessageCount">Returns how many messages PostgreSQL is expected to send in response to this message.</param>
        internal PregeneratedMessage(byte[] data, string description, int responseMessageCount)
        {
            Debug.Assert(data.Length < WriteBuffer.MinimumBufferSize);

            _data = data;
            _description = description;
            ResponseMessageCount = responseMessageCount;
        }

        internal override int Length => _data.Length;

        internal override int ResponseMessageCount { get; }

        internal override void WriteFully(WriteBuffer buf)
        {
            buf.WriteBytes(_data, 0, _data.Length);
        }

        public override string ToString() =>  _description ?? "[?]";

        static readonly WriteBuffer _tempBuf;
        static readonly QueryMessage _tempQuery;

        static PregeneratedMessage()
        {
            _tempBuf = new WriteBuffer(null, new MemoryStream(), WriteBuffer.MinimumBufferSize, Encoding.ASCII);
            _tempQuery = new QueryMessage();

            BeginTrans                = BuildQuery("BEGIN");
            SetTransRepeatableRead    = BuildQuery("SET TRANSACTION ISOLATION LEVEL REPEATABLE READ");
            SetTransSerializable      = BuildQuery("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE");
            SetTransReadCommitted     = BuildQuery("SET TRANSACTION ISOLATION LEVEL READ COMMITTED");
            SetTransReadUncommitted   = BuildQuery("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
            CommitTransaction         = BuildQuery("COMMIT");
            RollbackTransaction       = BuildQuery("ROLLBACK");
            KeepAlive                 = BuildQuery("SELECT NULL");

            ResetSessionAuthorization = BuildQuery("SET SESSION AUTHORIZATION DEFAULT");
            ResetAll                  = BuildQuery("RESET ALL");
            CloseAll                  = BuildQuery("CLOSE ALL");
            UnlistenAll               = BuildQuery("UNLISTEN *");
            AdvisoryUnlockAll         = BuildQuery("SELECT pg_advisory_unlock_all()", 3);
            DiscardTemp               = BuildQuery("DISCARD TEMP");
            DiscardSequences          = BuildQuery("DISCARD SEQUENCES");

            _tempBuf = null;
            _tempQuery = null;
        }

        static PregeneratedMessage BuildQuery(string query, int responseMessageCount=2)
        {
            Debug.Assert(query != null && query.All(c => c < 128));

            var totalLen = 5 + query.Length;
            var ms = new MemoryStream(totalLen);
            _tempBuf.Underlying = ms;
            _tempQuery.Populate(query);
            _tempQuery.Write(_tempBuf);
            _tempBuf.Flush();
            return new PregeneratedMessage(ms.ToArray(), _tempQuery.ToString(), responseMessageCount);
        }

        internal static readonly PregeneratedMessage BeginTrans;
        internal static readonly PregeneratedMessage SetTransRepeatableRead;
        internal static readonly PregeneratedMessage SetTransSerializable;
        internal static readonly PregeneratedMessage SetTransReadCommitted;
        internal static readonly PregeneratedMessage SetTransReadUncommitted;
        internal static readonly PregeneratedMessage CommitTransaction;
        internal static readonly PregeneratedMessage RollbackTransaction;
        internal static readonly PregeneratedMessage KeepAlive;

        internal static readonly PregeneratedMessage ResetSessionAuthorization;
        internal static readonly PregeneratedMessage ResetAll;
        internal static readonly PregeneratedMessage CloseAll;
        internal static readonly PregeneratedMessage UnlistenAll;
        internal static readonly PregeneratedMessage AdvisoryUnlockAll;
        internal static readonly PregeneratedMessage DiscardTemp;
        internal static readonly PregeneratedMessage DiscardSequences;
    }
}
