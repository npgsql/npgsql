using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

// TODO: Need to work on the nullability here
#nullable disable
#pragma warning disable CS8632

namespace Npgsql.Internal.TypeHandlers.FullTextSearchHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL tsquery data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-textsearch.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class TsQueryHandler : NpgsqlTypeHandler<NpgsqlTsQuery>,
        INpgsqlTypeHandler<NpgsqlTsQueryEmpty>, INpgsqlTypeHandler<NpgsqlTsQueryLexeme>,
        INpgsqlTypeHandler<NpgsqlTsQueryNot>, INpgsqlTypeHandler<NpgsqlTsQueryAnd>,
        INpgsqlTypeHandler<NpgsqlTsQueryOr>, INpgsqlTypeHandler<NpgsqlTsQueryFollowedBy>
    {
        // 1 (type) + 1 (weight) + 1 (is prefix search) + 2046 (max str len) + 1 (null terminator)
        const int MaxSingleTokenBytes = 2050;

        readonly Stack<NpgsqlTsQuery> _stack = new();

        public TsQueryHandler(PostgresType pgType) : base(pgType) {}

        #region Read

        /// <inheritdoc />
        public override async ValueTask<NpgsqlTsQuery> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numTokens = buf.ReadInt32();
            if (numTokens == 0)
                return new NpgsqlTsQueryEmpty();

            NpgsqlTsQuery? value = null;
            var nodes = new Stack<Tuple<NpgsqlTsQuery, int>>();
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
                        InsertInTree(node, nodes, ref value);
                        nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 0));
                    }
                    else
                    {
                        var node = operKind switch
                        {
                            NpgsqlTsQuery.NodeKind.And    => (NpgsqlTsQuery)new NpgsqlTsQueryAnd(null, null),
                            NpgsqlTsQuery.NodeKind.Or     => new NpgsqlTsQueryOr(null, null),
                            NpgsqlTsQuery.NodeKind.Phrase => new NpgsqlTsQueryFollowedBy(null, buf.ReadInt16(), null),
                            _ => throw new InvalidOperationException($"Internal Npgsql bug: unexpected value {operKind} of enum {nameof(NpgsqlTsQuery.NodeKind)}. Please file a bug.")
                        };

                        InsertInTree(node, nodes, ref value);

                        nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 1));
                        nodes.Push(new Tuple<NpgsqlTsQuery, int>(node, 2));
                    }
                }
                else
                {
                    var weight = (NpgsqlTsQueryLexeme.Weight)buf.ReadByte();
                    var prefix = buf.ReadByte() != 0;
                    var str = buf.ReadNullTerminatedString();
                    InsertInTree(new NpgsqlTsQueryLexeme(str, weight, prefix), nodes, ref value);
                }

                len -= buf.ReadPosition - readPos;
            }

            if (nodes.Count != 0)
                throw new InvalidOperationException("Internal Npgsql bug, please report.");

            return value!;

            static void InsertInTree(NpgsqlTsQuery node, Stack<Tuple<NpgsqlTsQuery, int>> nodes, ref NpgsqlTsQuery? value)
            {
                if (nodes.Count == 0)
                    value = node;
                else
                {
                    var parent = nodes.Pop();
                    if (parent.Item2 == 0)
                        ((NpgsqlTsQueryNot)parent.Item1).Child = node;
                    else if (parent.Item2 == 1)
                        ((NpgsqlTsQueryBinOp)parent.Item1).Left = node;
                    else
                        ((NpgsqlTsQueryBinOp)parent.Item1).Right = node;
                }
            }
        }

        async ValueTask<NpgsqlTsQueryEmpty> INpgsqlTypeHandler<NpgsqlTsQueryEmpty>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryEmpty)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryLexeme> INpgsqlTypeHandler<NpgsqlTsQueryLexeme>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryLexeme)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryNot> INpgsqlTypeHandler<NpgsqlTsQueryNot>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryNot)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryAnd> INpgsqlTypeHandler<NpgsqlTsQueryAnd>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryAnd)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryOr> INpgsqlTypeHandler<NpgsqlTsQueryOr>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryOr)await Read(buf, len, async, fieldDescription);

        async ValueTask<NpgsqlTsQueryFollowedBy> INpgsqlTypeHandler<NpgsqlTsQueryFollowedBy>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
            => (NpgsqlTsQueryFollowedBy)await Read(buf, len, async, fieldDescription);

        #endregion Read

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlTsQuery value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => value.Kind == NpgsqlTsQuery.NodeKind.Empty
                ? 4
                : 4 + GetNodeLength(value);

        int GetNodeLength(NpgsqlTsQuery node)
        {
            // TODO: Figure out the nullability strategy here
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

        /// <inheritdoc />
        public override async Task Write(NpgsqlTsQuery query, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            var numTokens = GetTokenCount(query);

            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(numTokens);

            if (numTokens == 0)
                return;

            _stack.Push(query);

            while (_stack.Count > 0)
            {
                if (buf.WriteSpaceLeft < 2)
                    await buf.Flush(async, cancellationToken);

                if (_stack.Peek().Kind == NpgsqlTsQuery.NodeKind.Lexeme && buf.WriteSpaceLeft < MaxSingleTokenBytes)
                    await buf.Flush(async, cancellationToken);

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

                        _stack.Push(((NpgsqlTsQueryBinOp)node).Left);
                        _stack.Push(((NpgsqlTsQueryBinOp)node).Right);
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

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryOr value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryAnd value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryNot value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryLexeme value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryEmpty value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public int ValidateAndGetLength(NpgsqlTsQueryFollowedBy value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => ValidateAndGetLength((NpgsqlTsQuery)value, ref lengthCache, parameter);

        /// <inheritdoc />
        public Task Write(NpgsqlTsQueryOr value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        /// <inheritdoc />
        public Task Write(NpgsqlTsQueryAnd value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        /// <inheritdoc />
        public Task Write(NpgsqlTsQueryNot value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        /// <inheritdoc />
        public Task Write(NpgsqlTsQueryLexeme value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        /// <inheritdoc />
        public Task Write(NpgsqlTsQueryEmpty value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        /// <inheritdoc />
        public Task Write(
            NpgsqlTsQueryFollowedBy value,
            NpgsqlWriteBuffer buf,
            NpgsqlLengthCache? lengthCache,
            NpgsqlParameter? parameter,
            bool async,
            CancellationToken cancellationToken = default)
            => Write((NpgsqlTsQuery)value, buf, lengthCache, parameter, async, cancellationToken);

        #endregion Write
    }
}
