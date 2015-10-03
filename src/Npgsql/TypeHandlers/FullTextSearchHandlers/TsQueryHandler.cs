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

using Npgsql.BackendMessages;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Npgsql.TypeHandlers.FullTextSearchHandlers
{
    /// <summary>
    /// http://www.postgresql.org/docs/current/static/datatype-textsearch.html
    /// </summary>
    [TypeMapping("tsquery", NpgsqlDbType.TsQuery, new[] {
        typeof(NpgsqlTsQuery), typeof(NpgsqlTsQueryAnd), typeof(NpgsqlTsQueryEmpty),
        typeof(NpgsqlTsQueryLexeme), typeof(NpgsqlTsQueryNot), typeof(NpgsqlTsQueryOr), typeof(NpgsqlTsQueryBinOp) })
    ]
    internal class TsQueryHandler : ChunkingTypeHandler<NpgsqlTsQuery>
    {
        // 1 (type) + 1 (weight) + 1 (is prefix search) + 2046 (max str len) + 1 (null terminator)
        const int MaxSingleTokenBytes = 2050;

        NpgsqlBuffer _buf;
        Stack<Tuple<NpgsqlTsQuery, int>> _nodes;
        int _numTokens;
        int _tokenPos;
        int _bytesLeft;
        NpgsqlTsQuery _value;

        Stack<NpgsqlTsQuery> _stack;

        public override void PrepareRead(NpgsqlBuffer buf, int len, FieldDescription fieldDescription)
        {
            _buf = buf;
            _nodes = new Stack<Tuple<NpgsqlTsQuery, int>>();
            _tokenPos = -1;
            _bytesLeft = len;
        }

        public override bool Read([CanBeNull] out NpgsqlTsQuery result)
        {
            result = null;

            if (_tokenPos == -1)
            {
                if (_buf.ReadBytesLeft < 4)
                    return false;
                _numTokens = _buf.ReadInt32();
                _bytesLeft -= 4;
                _tokenPos = 0;
            }

            if (_numTokens == 0)
            {
                result = new NpgsqlTsQueryEmpty();
                _buf = null;
                _nodes = null;
                return true;
            }

            for (; _tokenPos < _numTokens; _tokenPos++)
            {
                if (_buf.ReadBytesLeft < Math.Min(_bytesLeft, MaxSingleTokenBytes))
                    return false;

                int readPos = _buf.ReadPosition;

                bool isOper = _buf.ReadByte() == 2;
                if (isOper)
                {
                    NpgsqlTsQuery.NodeKind operKind = (NpgsqlTsQuery.NodeKind)_buf.ReadByte();
                    if (operKind == NpgsqlTsQuery.NodeKind.Not)
                    {
                        var node = new NpgsqlTsQueryNot(null);
                        InsertInTree(node);
                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 0));
                    }
                    else
                    {
                        NpgsqlTsQuery node = null;

                        if (operKind == NpgsqlTsQuery.NodeKind.And)
                            node = new NpgsqlTsQueryAnd(null, null);
                        else if (operKind == NpgsqlTsQuery.NodeKind.Or)
                            node = new NpgsqlTsQueryOr(null, null);
                        else
                            PGUtil.ThrowIfReached();

                        InsertInTree(node);

                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 2));
                        _nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 1));
                    }
                }
                else
                {
                    NpgsqlTsQueryLexeme.Weight weight = (NpgsqlTsQueryLexeme.Weight)_buf.ReadByte();
                    bool prefix = _buf.ReadByte() != 0;
                    string str = _buf.ReadNullTerminatedString();
                    InsertInTree(new NpgsqlTsQueryLexeme(str, weight, prefix));
                }

                _bytesLeft -= _buf.ReadPosition - readPos;
            }

            if (_nodes.Count != 0)
                PGUtil.ThrowIfReached();

            result = _value;
            _buf = null;
            _nodes = null;
            _value = null;
            return true;
        }

        void InsertInTree(NpgsqlTsQuery node)
        {
            if (_nodes.Count == 0)
            {
                _value = node;
            }
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

        public override int ValidateAndGetLength(object value, ref LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            var vec = value as NpgsqlTsQuery;
            if (vec == null) {
                throw CreateConversionException(value.GetType());
            }

            if (vec.Kind == NpgsqlTsQuery.NodeKind.Empty)
                return 4;

            return 4 + GetNodeLength(vec);
        }

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
                case NpgsqlTsQuery.NodeKind.Not:
                    return 2 + GetNodeLength(((NpgsqlTsQueryNot)node).Child);
                case NpgsqlTsQuery.NodeKind.Empty:
                    throw new InvalidOperationException("Empty tsquery nodes must be top-level");
                default:
                    throw new InvalidOperationException("Illegal node kind: " + node.Kind);
            }
        }

        public override void PrepareWrite(object value, NpgsqlBuffer buf, LengthCache lengthCache, NpgsqlParameter parameter=null)
        {
            _buf = buf;
            _value = (NpgsqlTsQuery)value;
            _numTokens = GetTokenCount(_value);
        }

        int GetTokenCount(NpgsqlTsQuery node)
        {
            switch (node.Kind)
            {
                case NpgsqlTsQuery.NodeKind.Lexeme:
                    return 1;
                case NpgsqlTsQuery.NodeKind.And:
                case NpgsqlTsQuery.NodeKind.Or:
                    return 1 + GetTokenCount(((NpgsqlTsQueryBinOp)node).Left) + GetTokenCount(((NpgsqlTsQueryBinOp)node).Right);
                case NpgsqlTsQuery.NodeKind.Not:
                    return 1 + GetTokenCount(((NpgsqlTsQueryNot)node).Child);
                case NpgsqlTsQuery.NodeKind.Empty:
                    return 0;
            }
            return -1;
        }

        public override bool Write(ref DirectBuffer directBuf)
        {
            if (_stack == null)
            {
                if (_buf.WriteSpaceLeft < 4)
                    return false;
                _buf.WriteInt32(_numTokens);

                if (_numTokens == 0)
                {
                    _buf = null;
                    _value = null;
                    return true;
                }
                _stack = new Stack<NpgsqlTsQuery>();
                _stack.Push(_value);
            }

            while (_stack.Count > 0)
            {
                if (_buf.WriteSpaceLeft < 2)
                    return false;

                if (_stack.Peek().Kind == NpgsqlTsQuery.NodeKind.Lexeme && _buf.WriteSpaceLeft < MaxSingleTokenBytes)
                    return false;

                var node = _stack.Pop();
                _buf.WriteByte(node.Kind == NpgsqlTsQuery.NodeKind.Lexeme ? (byte)1 : (byte)2);
                if (node.Kind != NpgsqlTsQuery.NodeKind.Lexeme)
                {
                    _buf.WriteByte((byte)node.Kind);
                    if (node.Kind == NpgsqlTsQuery.NodeKind.Not)
                        _stack.Push(((NpgsqlTsQueryNot)node).Child);
                    else
                    {
                        _stack.Push(((NpgsqlTsQueryBinOp)node).Right);
                        _stack.Push(((NpgsqlTsQueryBinOp)node).Left);
                    }
                }
                else
                {
                    var lexemeNode = (NpgsqlTsQueryLexeme)node;
                    _buf.WriteByte((byte)lexemeNode.Weights);
                    _buf.WriteByte(lexemeNode.IsPrefixSearch ? (byte)1 : (byte)0);
                    _buf.WriteString(lexemeNode.Text);
                    _buf.WriteByte(0);
                }
            }

            _buf = null;
            _value = null;
            _stack = null;
            return true;
        }
    }
}
