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
using System.Linq;
using System.Text;

namespace Npgsql.FrontendMessages
{
    class ParseMessage : ChunkingFrontendMessage
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
        internal List<uint> ParameterTypeOIDs { get; }

        byte[] _statementNameBytes;
        int _queryLen;
        char[] _queryChars;
        int _charPos;
        int _parameterTypePos;

        State _state;

        const byte Code = (byte)'P';

        internal ParseMessage()
        {
            ParameterTypeOIDs = new List<uint>();
        }

        internal ParseMessage Populate(NpgsqlStatement statement, TypeHandlerRegistry typeHandlerRegistry)
        {
            _state = State.WroteNothing;
            _parameterTypePos = 0;
            ParameterTypeOIDs.Clear();
            Query = statement.SQL;
            Statement = statement.PreparedStatementName ?? "";
            foreach (var inputParam in statement.InputParameters) {
                inputParam.ResolveHandler(typeHandlerRegistry);
                ParameterTypeOIDs.Add(inputParam.Handler.OID);
            }
            return this;
        }

        internal override bool Write(NpgsqlBuffer buf, ref DirectBuffer directBuf)
        {
            Contract.Requires(Statement != null && Statement.All(c => c < 128));

            switch (_state)
            {
                case State.WroteNothing:
                    _statementNameBytes = PGUtil.UTF8Encoding.GetBytes(Statement);
                    _queryLen = PGUtil.UTF8Encoding.GetByteCount(Query);
                    if (buf.WriteSpaceLeft < 1 + 4 + _statementNameBytes.Length + 1) {
                        return false;
                    }

                    var messageLength =
                        1 +                         // Message code
                        4 +                         // Length
                        _statementNameBytes.Length +
                        1 +                         // Null terminator
                        _queryLen +
                        1 +                         // Null terminator
                        2 +                         // Number of parameters
                        ParameterTypeOIDs.Count * 4;

                    buf.WriteByte(Code);
                    buf.WriteInt32(messageLength - 1);
                    buf.WriteBytesNullTerminated(_statementNameBytes);
                    goto case State.WroteHeader;

                case State.WroteHeader:
                    _state = State.WroteHeader;

                    if (_queryLen <= buf.WriteSpaceLeft) {
                        buf.WriteString(Query);
                        goto case State.WroteQuery;
                    }

                    if (_queryLen <= buf.Size) {
                        // String can fit entirely in an empty buffer. Flush and retry rather than
                        // going into the partial writing flow below (which requires ToCharArray())
                        return false;
                    }

                    _queryChars = Query.ToCharArray();
                    _charPos = 0;
                    goto case State.WritingQuery;

                case State.WritingQuery:
                    _state = State.WritingQuery;
                    int charsUsed;
                    bool completed;
                    buf.WriteStringChunked(_queryChars, _charPos, _queryChars.Length - _charPos, true,
                                           out charsUsed, out completed);
                    if (!completed)
                    {
                        _charPos += charsUsed;
                        return false;
                    }
                    goto case State.WroteQuery;

                case State.WroteQuery:
                    _state = State.WroteQuery;
                    if (buf.WriteSpaceLeft < 1 + 2) {
                        return false;
                    }
                    buf.WriteByte(0); // Null terminator for the query
                    buf.WriteInt16((short)ParameterTypeOIDs.Count);
                    goto case State.WritingParameterTypes;

                case State.WritingParameterTypes:
                    _state = State.WritingParameterTypes;
                    for (; _parameterTypePos < ParameterTypeOIDs.Count; _parameterTypePos++)
                    {
                        if (buf.WriteSpaceLeft < 4)
                        {
                            return false;
                        }
                        buf.WriteInt32((int)ParameterTypeOIDs[_parameterTypePos]);
                    }

                    _state = State.WroteAll;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        public override string ToString()
        {
            return $"[Parse(Statement={Statement},NumParams={ParameterTypeOIDs.Count}]";
        }

        private enum State
        {
            WroteNothing,
            WroteHeader,
            WritingQuery,
            WroteQuery,
            WritingParameterTypes,
            WroteAll
        }
    }
}
