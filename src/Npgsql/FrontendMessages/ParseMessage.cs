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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.FrontendMessages
{
    class ParseMessage : FrontendMessage
    {
        /// <summary>
        /// The query string to be parsed.
        /// </summary>
        string Query { get; set; }

        /// <summary>
        /// The name of the destination prepared statement (an empty string selects the unnamed prepared statement).
        /// </summary>
        string Statement { get; set; }

        // ReSharper disable once InconsistentNaming
        List<uint> ParameterTypeOIDs { get; }

        readonly Encoding _encoding;

        const byte Code = (byte)'P';

        internal ParseMessage(Encoding encoding)
        {
            _encoding = encoding;
            ParameterTypeOIDs = new List<uint>();
        }

        internal ParseMessage Populate(NpgsqlStatement statement, TypeHandlerRegistry typeHandlerRegistry)
        {
            ParameterTypeOIDs.Clear();
            Query = statement.SQL;
            Statement = statement.PreparedStatementName ?? "";
            foreach (var inputParam in statement.InputParameters) {
                inputParam.ResolveHandler(typeHandlerRegistry);
                ParameterTypeOIDs.Add(inputParam.Handler.PostgresType.OID);
            }
            return this;
        }

        internal override async Task Write(WriteBuffer buf, bool async, CancellationToken cancellationToken)
        {
            Debug.Assert(Statement != null);

            var statementNameBytes = Statement.Length == 0 ? PGUtil.EmptyBuffer : _encoding.GetBytes(Statement);
            var queryByteLen = _encoding.GetByteCount(Query);
            if (buf.WriteSpaceLeft < 1 + 4 + statementNameBytes.Length + 1)
                await buf.Flush(async, cancellationToken);

            var messageLength =
                1 +                         // Message code
                4 +                         // Length
                statementNameBytes.Length +
                1 +                         // Null terminator
                queryByteLen +
                1 +                         // Null terminator
                2 +                         // Number of parameters
                ParameterTypeOIDs.Count * 4;

            buf.WriteByte(Code);
            buf.WriteInt32(messageLength - 1);
            buf.WriteBytesNullTerminated(statementNameBytes);

            await buf.WriteString(Query, queryByteLen, async, cancellationToken);

            if (buf.WriteSpaceLeft < 1 + 2)
                await buf.Flush(async, cancellationToken);
            buf.WriteByte(0); // Null terminator for the query
            buf.WriteInt16((short)ParameterTypeOIDs.Count);

            foreach (uint t in ParameterTypeOIDs)
            {
                if (buf.WriteSpaceLeft < 4)
                    await buf.Flush(async, cancellationToken);
                buf.WriteInt32((int)t);
            }
        }

        public override string ToString()
            => $"[Parse(Statement={Statement},NumParams={ParameterTypeOIDs.Count}]";
    }
}
