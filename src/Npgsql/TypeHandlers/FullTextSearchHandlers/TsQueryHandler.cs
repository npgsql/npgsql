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

using Npgsql.BackendMessages;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers.FullTextSearchHandlers
{
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-textsearch.html
    /// </summary>
    [TypeMapping("tsquery", NpgsqlDbType.TsQuery, new[] {
        typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty), typeof(NpgsqlTsQueryFollowedBy),
        typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp) })
    ]
    class TsQueryHandler : NpgsqlTypeHandler<NpgsqlTsQuery>,
        INpgsqlTypeHandler<NpgsqlTsQueryEmpty>, INpgsqlTypeHandler<NpgsqlTsQueryLexeme>,
        INpgsqlTypeHandler<NpgsqlTsQueryNot>, INpgsqlTypeHandler<NpgsqlTsQueryAnd>,
        INpgsqlTypeHandler<NpgsqlTsQueryOr>, INpgsqlTypeHandler<NpgsqlTsQueryFollowedBy>
    {
        // 1 (type) + 1 (weight) + 1 (is prefix search) + 2046 (max str len) + 1 (null terminator)
        const int MaxSingleTokenBytes = 2050;

        Stack<Tuple<NpgsqlTsQuery, int>> _nodes;
        NpgsqlTsQuery _value;

        readonly Stack<NpgsqlTsQuery> _stack = new Stack<NpgsqlTsQuery>();

        #region Read

        public override async ValueTask<NpgsqlTsQuery> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numTokens = buf.ReadInt32();
            if (numTokens == 0)
                return new NpgsqlTsQueryEmpty();

            _nodes = new Stack<Tuple<NpgsqlTsQuery, int>>();
            len -= 4;

            for (var tokenPos = 0; tokenPos < numTokens; tokenPos++)
            {
                await buf.Ensure(Math.Min(len, MaxSingleTokenBytes), async);
                var readPos = buf.ReadPosition;

                var isOper = buf.ReadByte() == 2;
                if (isOper)
                {
                    var operKind = (NpgsqlTsQuery.NodeKind)buf.ReadByte();
                    if (operKind == NpgsqlTsQuery.NodeKind.Not)
                    {
                        var node = new NpgsqlTsQueryNot(null);
                        InsertInTree(node);
                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 0));
                    }
                    else
                    {
                        NpgsqlTsQuery node;
                        switch (operKind)
                        {
                        case NpgsqlTsQuery.NodeKind.And:
                            node = new NpgsqlTsQueryAnd(null, null);
                            break;
                        case NpgsqlTsQuery.NodeKind.Or:
                            node = new NpgsqlTsQueryOr(null, null);
                            break;
                        case NpgsqlTsQuery.NodeKind.Phrase:
                            var distance = buf.ReadInt16();
                            node = new NpgsqlTsQueryFollowedBy(null, distance, null);
                            break;
                        default:
                            throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {operKind} of enum {nameof(NpgsqlTsQuery.NodeKind)}. Please file a bug.");
                        }

                        InsertInTree(node);

                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 2));
                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 1));
                    }
                }
                else
                {
                    var weight = (NpgsqlTsQueryLexeme.Weight)buf.ReadByte();
                    var prefix = buf.ReadByte() != 0;
                    var str = buf.ReadNullTerminatedString();
                    InsertInTree(new NpgsqlTsQueryLexeme(str, weight, prefix));
                }

                len -= buf.ReadPosition - readPos;
            }

            if (_nodes.Count != 0)
                throw new InvalidOperationException("Internal Npgsql bug, please report.");

            return _value;
        }

        async ValueTask<NpgsqlTsQueryEmpty> INpgsqlTypeHandler<NpgsqlTsQueryEmpty>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryEmpty)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryLexeme> INpgsqlTypeHandler<NpgsqlTsQueryLexeme>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryLexeme)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryNot> INpgsqlTypeHandler<NpgsqlTsQueryNot>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryNot)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryAnd> INpgsqlTypeHandler<NpgsqlTsQueryAnd>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryAnd)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryOr> INpgsqlTypeHandler<NpgsqlTsQueryOr>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryOr)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryFollowedBy> INpgsqlTypeHandler<NpgsqlTsQueryFollowedBy>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription)
            => (NpgsqlTsQueryFollowedBy)await Read(buf, len, async, fieldDescription);

        #endregion Read

        #region Write

        void InsertInTree([CanBeNull] NpgsqlTsQuery node)
        {
            if (_nodes.Count == 0)
                _value = node;
            else
            {
                var parent = _nodes.Pop();
                if (parent.Item2 == 0)
                    ((NpgsqlTsQueryNot)parent.Item1).Child = node;
                else if (parent.Item2 == 1)
                    ((NpgsqlTsQueryBinOp)parent.Item1).Left = node;
                else
                    ((NpgsqlTsQueryBinOp)parent.Item1).Right = node;
            }
        }

        public override int ValidateAndGetLength(NpgsqlTsQuery value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => value.Kind == NpgsqlTsQuery.NodeKind.Empty
                ? 4
                : 4 + GetNodeLength(value);

        int GetNodeLength(NpgsqlTsQuery node)
        {
            switch (node.Kind)
            {
            case NpgsqlTsQuery.NodeKind.Lexeme:
                var strLen = Encoding.UTF8.GetByteCount(((NpgsqlTsQueryLexeme)node).Text);
                if (strLen > 2046)
                    throw new InvalidCastException("Lexeme text too long. Must be at most 2046 bytes in UTF8.");
                return 4 + strLen;
            case NpgsqlTsQuery.NodeKind.And:
            case NpgsqlTsQuery.NodeKind.Or:
                return 2 + GetNodeLength(((NpgsqlTsQueryBinOp)node).Left) + GetNodeLength(((NpgsqlTsQueryBinOp)node).Right);
            case NpgsqlTsQuery.NodeKind.Phrase:
                // 2 additional bytes for uint16 phrase operator "distance" field.
                return 4 + GetNodeLength(((NpgsqlTsQueryBinOp)node).Left) + GetNodeLength(((NpgsqlTsQueryBinOp)node).Right);
            case NpgsqlTsQuery.NodeKind.Not:
                return 2 + GetNodeLength(((NpgsqlTsQueryNot)node).Child);
            case NpgsqlTsQuery.NodeKind.Empty:
                throw new InvalidOperationException("Empty tsquery nodes must be top-level");
            default:
                throw new InvalidOperationException("Illegal node kind: " + node.Kind);
            }
        }

        public override async Task Write(NpgsqlTsQuery query, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            var numTokens = GetTokenCount(query);

            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(numTokens);

            if (numTokens == 0)
                return;

            _stack.Push(query);

            while (_stack.Count > 0)
            {
                if (buf.WriteSpaceLeft < 2)
                    await buf.Flush(async);

                if (_stack.Peek().Kind == NpgsqlTsQuery.NodeKind.Lexeme && buf.WriteSpaceLeft < MaxSingleTokenBytes)
                    await buf.Flush(async);

                var node = _stack.Pop();
                buf.WriteByte(node.Kind == NpgsqlTsQuery.NodeKind.Lexeme ? (byte)1 : (byte)2);
                if (node.Kind != NpgsqlTsQuery.NodeKind.Lexeme)
                {
                    buf.WriteByte((byte)node.Kind);
                    if (node.Kind == NpgsqlTsQuery.NodeKind.Not)
                        _stack.Push(((NpgsqlTsQueryNot)node).Child);
                    else
                    {
                        if (node.Kind == NpgsqlTsQuery.NodeKind.Phrase)
                            buf.WriteInt16(((NpgsqlTsQueryFollowedBy)node).Distance);

                        _stack.Push(((NpgsqlTsQueryBinOp)node).Right);
                        _stack.Push(((NpgsqlTsQueryBinOp)node).Left);
                    }
                }
                else
                {
                    var lexemeNode = (NpgsqlTsQueryLexeme)node;
                    buf.WriteByte((byte)lexemeNode.Weights);
                    buf.WriteByte(lexemeNode.IsPrefixSearch ? (byte)1 : (byte)0);
                    buf.WriteString(lexemeNode.Text);
                    buf.WriteByte(0);
                }
            }

            _stack.Clear();
        }

        int GetTokenCount(NpgsqlTsQuery node)
        {
            switch (node.Kind)
            {
            case NpgsqlTsQuery.NodeKind.Lexeme:
                return 1;
            case NpgsqlTsQuery.NodeKind.And:
            case NpgsqlTsQuery.NodeKind.Or:
            case NpgsqlTsQuery.NodeKind.Phrase:
                return 1 + GetTokenCount(((NpgsqlTsQueryBinOp)node).Left) + GetTokenCount(((NpgsqlTsQueryBinOp)node).Right);
            case NpgsqlTsQuery.NodeKind.Not:
                return 1 + GetTokenCount(((NpgsqlTsQueryNot)node).Child);
            case NpgsqlTsQuery.NodeKind.Empty:
                return 0;
            }
            return -1;
        }

        public int ValidateAndGetLength(NpgsqlTsQueryOr value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlTsQueryAnd value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlTsQueryNot value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlTsQueryLexeme value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlTsQueryEmpty value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public int ValidateAndGetLength(NpgsqlTsQueryFollowedBy value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        public Task Write(NpgsqlTsQueryOr value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        public Task Write(NpgsqlTsQueryAnd value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        public Task Write(NpgsqlTsQueryNot value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        public Task Write(NpgsqlTsQueryLexeme value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        public Task Write(NpgsqlTsQueryEmpty value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        public Task Write(
            NpgsqlTsQueryFollowedBy value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache lengthCache,
            NpgsqlParameter parameter,
            bool async)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async);

        #endregion Write
    }
}
