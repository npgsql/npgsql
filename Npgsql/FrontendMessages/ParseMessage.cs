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
        internal List<uint> ParameterTypeOIDs { get; private set; }

        byte[] _statementNameBytes;
        int _queryLen;
        char[] _queryChars;
        int _charPos;

        State _state;

        const byte Code = (byte)'P';

        internal ParseMessage()
        {
            ParameterTypeOIDs = new List<uint>();
        }

        internal ParseMessage Populate(QueryDetails queryDetails, TypeHandlerRegistry typeHandlerRegistry)
        {
            _state = State.WroteNothing;
            ParameterTypeOIDs.Clear();
            Query = queryDetails.Sql;
            Statement = queryDetails.PreparedStatementName ?? "";
            foreach (var inputParam in queryDetails.InputParameters) {
                inputParam.ResolveHandler(typeHandlerRegistry);
                ParameterTypeOIDs.Add(inputParam.Handler.OID);
            }
            return this;
        }

        internal override bool Write(NpgsqlBuffer buf)
        {
            Contract.Requires(Statement != null && Statement.All(c => c < 128));

            switch (_state)
            {
                case State.WroteNothing:
                    _statementNameBytes = PGUtil.UTF8Encoding.GetBytes(Statement);
                    _queryLen = PGUtil.UTF8Encoding.GetByteCount(Query);
                    if (buf.WriteSpaceLeft < 4 + _statementNameBytes.Length + 1) {
                        return false;
                    }

                    var messageLength =
                        4 +                         // Length
                        _statementNameBytes.Length +
                        1 +                         // Null terminator
                        _queryLen +
                        1 +                         // Null terminator
                        2 +                         // Number of parameters
                        ParameterTypeOIDs.Count * 4;

                    buf.WriteByte(Code);
                    buf.WriteInt32(messageLength);
                    buf.WriteBytesNullTerminated(_statementNameBytes);
                    goto case State.WroteHeader;

                case State.WroteHeader:
                    _state = State.WroteHeader;

                    if (_queryLen <= buf.WriteSpaceLeft) {
                        buf.WriteStringSimple(Query);
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
                    if (buf.WriteSpaceLeft < 1 + 2 + ParameterTypeOIDs.Count * 4) {
                        return false;
                    }
                    buf.WriteByte(0); // Null terminator for the query
                    buf.WriteInt16((short)ParameterTypeOIDs.Count);

                    foreach (var t in ParameterTypeOIDs) {
                        buf.WriteInt32((int)t);
                    }

                    _state = State.WroteAll;
                    return true;

                default:
                    throw PGUtil.ThrowIfReached();
            }
        }

        public override string ToString()
        {
            return String.Format("[Parse(Statement={0},NumParams={1}]", Statement, ParameterTypeOIDs.Count);
        }

        private enum State
        {
            WroteNothing,
            WroteHeader,
            WritingQuery,
            WroteQuery,
            WroteAll
        }
    }
}
