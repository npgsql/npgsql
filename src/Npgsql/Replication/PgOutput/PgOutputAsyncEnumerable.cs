using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.Util;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput
{
    class PgOutputAsyncEnumerable : IAsyncEnumerable<PgOutputReplicationMessage>
    {
        readonly LogicalReplicationConnection _connection;
        readonly PgOutputReplicationSlot _slot;
        readonly PgOutputReplicationOptions _options;
        readonly CancellationToken _baseCancellationToken;
        readonly NpgsqlLogSequenceNumber? _walLocation;
        readonly Dictionary<uint, RelationMessage> _relations = new();

        #region Cached messages

        readonly BeginMessage _beginMessage = new();
        readonly LogicalDecodingMessage _logicalDecodingMessage = new();
        readonly CommitMessage _commitMessage = new();
        readonly OriginMessage _originMessage = new();
        readonly TruncateMessage _truncateMessage = new();
        readonly TypeMessage _typeMessage = new();
        readonly StreamStartMessage _streamStartMessage = new();
        readonly StreamStopMessage _streamStopMessage = new();
        readonly StreamCommitMessage _streamCommitMessage = new();
        readonly StreamAbortMessage _streamAbortMessage = new();
        readonly ReadOnlyArrayBuffer<RelationMessage> _truncateMessageRelations = new();

        readonly InsertMessage _insertMessage;
        readonly DefaultUpdateMessage _defaultUpdateMessage;
        readonly FullUpdateMessage _fullUpdateMessage;
        readonly IndexUpdateMessage _indexUpdateMessage;
        readonly FullDeleteMessage _fullDeleteMessage;
        readonly KeyDeleteMessage _keyDeleteMessage;

        #endregion

        internal PgOutputAsyncEnumerable(
            LogicalReplicationConnection connection,
            PgOutputReplicationSlot slot,
            PgOutputReplicationOptions options,
            CancellationToken cancellationToken,
            NpgsqlLogSequenceNumber? walLocation = null)
        {
            _connection = connection;
            _slot = slot;
            _options = options;
            _baseCancellationToken = cancellationToken;
            _walLocation = walLocation;

            var connector = _connection.Connector;
            _insertMessage = new(connector);
            _defaultUpdateMessage = new(connector);
            _fullUpdateMessage = new(connector);
            _indexUpdateMessage = new(connector);
            _fullDeleteMessage = new(connector);
            _keyDeleteMessage = new(connector);
        }

        public IAsyncEnumerator<PgOutputReplicationMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            using (NoSynchronizationContextScope.Enter())
            {
                return StartReplicationInternal(
                    CancellationTokenSource.CreateLinkedTokenSource(_baseCancellationToken, cancellationToken).Token);
            }
        }

        async IAsyncEnumerator<PgOutputReplicationMessage> StartReplicationInternal(CancellationToken cancellationToken)
        {
            var stream = _connection.StartLogicalReplication(
                _slot, cancellationToken, _walLocation, _options.GetOptionPairs(), bypassingStream: true);
            var buf = _connection.Connector!.ReadBuffer;
            var inStreamingTransaction = false;
            var formatCode = _options.Binary ?? false ? FormatCode.Binary : FormatCode.Text;

            await foreach (var xLogData in stream.WithCancellation(cancellationToken))
            {
                await buf.EnsureAsync(1);
                var messageCode = (BackendReplicationMessageCode)buf.ReadByte();
                switch (messageCode)
                {
                case BackendReplicationMessageCode.Begin:
                {
                    await buf.EnsureAsync(20);
                    yield return _beginMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionFinalLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        transactionCommitTimestamp: DateTimeUtils.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Unspecified),
                        transactionXid: buf.ReadUInt32());
                    continue;
                }
                case BackendReplicationMessageCode.Message:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(14);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(10);
                        transactionXid = null;
                    }

                    var flags = buf.ReadByte();
                    var messageLsn = new NpgsqlLogSequenceNumber(buf.ReadUInt64());
                    var prefix = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    await buf.EnsureAsync(4);
                    var length = buf.ReadUInt32();
                    var data = (NpgsqlReadBuffer.ColumnStream)xLogData.Data;
                    data.Init(checked((int)length), false);
                    yield return _logicalDecodingMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        flags, messageLsn, prefix, data);
                    continue;
                }
                case BackendReplicationMessageCode.Commit:
                {
                    await buf.EnsureAsync(25);
                    yield return _commitMessage.Populate(
                        xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        (CommitMessage.CommitFlags)buf.ReadByte(),
                        commitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        transactionEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        transactionCommitTimestamp: DateTimeUtils.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Unspecified));
                    continue;
                }
                case BackendReplicationMessageCode.Origin:
                {
                    await buf.EnsureAsync(9);
                    yield return _originMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        originCommitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        originName: await buf.ReadNullTerminatedString(async: true, cancellationToken));
                    continue;
                }
                case BackendReplicationMessageCode.Relation:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(10);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(6);
                        transactionXid = null;
                    }

                    var relationId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    var relationName = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    await buf.EnsureAsync(3);
                    var relationReplicaIdentitySetting = (RelationMessage.ReplicaIdentitySetting)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();

                    if (!_relations.TryGetValue(relationId, out var msg))
                        msg = _relations[relationId] = new RelationMessage();

                    msg.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid, relationId, ns, relationName,
                        relationReplicaIdentitySetting);

                    var columns = msg.InternalColumns;
                    columns.Count = numColumns;
                    for (var i = 0; i < numColumns; i++)
                    {
                        await buf.EnsureAsync(2);
                        var flags = (RelationMessage.Column.ColumnFlags)buf.ReadByte();
                        var columnName = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                        await buf.EnsureAsync(8);
                        var dateTypeId = buf.ReadUInt32();
                        var typeModifier = buf.ReadInt32();
                        columns[i] = new RelationMessage.Column(flags, columnName, dateTypeId, typeModifier);
                    }

                    msg.RowDescription = RowDescriptionMessage.CreateForReplication(
                        _connection.Connector.TypeMapper, relationId, formatCode, columns);

                    yield return msg;
                    continue;
                }
                case BackendReplicationMessageCode.Type:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(9);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(5);
                        transactionXid = null;
                    }

                    var typeId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    var name = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    yield return _typeMessage.Populate(
                        xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid, typeId, ns, name);
                    continue;
                }
                case BackendReplicationMessageCode.Insert:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(11);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(7);
                        transactionXid = null;
                    }

                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    Debug.Assert(tupleDataType == TupleType.NewTuple);
                    var numColumns = buf.ReadUInt16();

                    if (!_relations.TryGetValue(relationId, out var relation))
                    {
                        throw new InvalidOperationException(
                            $"Could not find previous Relation message for relation ID {relationId} when processing Insert message");
                    }

                    Debug.Assert(numColumns == relation.RowDescription.Count);

                    yield return _insertMessage.Populate(
                        xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid, relation, numColumns);
                    await _insertMessage.Consume(cancellationToken);

                    continue;
                }
                case BackendReplicationMessageCode.Update:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(11);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(7);
                        transactionXid = null;
                    }

                    var relationId = buf.ReadUInt32();
                    var tupleType = (TupleType)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();

                    if (!_relations.TryGetValue(relationId, out var relation))
                    {
                        throw new InvalidOperationException(
                            $"Could not find previous Relation message for relation ID {relationId} when processing Update message");
                    }

                    Debug.Assert(numColumns == relation.RowDescription.Count);

                    switch (tupleType)
                    {
                    case TupleType.Key:
                        yield return _indexUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relation, numColumns);
                        await _indexUpdateMessage.Consume(cancellationToken);
                        continue;
                    case TupleType.OldTuple:
                        yield return _fullUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relation, numColumns);
                        await _fullUpdateMessage.Consume(cancellationToken);
                        continue;
                    case TupleType.NewTuple:
                        yield return _defaultUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relation, numColumns);
                        await _defaultUpdateMessage.Consume(cancellationToken);
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Delete:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(11);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(7);
                        transactionXid = null;
                    }

                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();

                    if (!_relations.TryGetValue(relationId, out var relation))
                    {
                        throw new InvalidOperationException(
                            $"Could not find previous Relation message for relation ID {relationId} when processing Update message");
                    }

                    Debug.Assert(numColumns == relation.RowDescription.Count);

                    switch (tupleDataType)
                    {
                    case TupleType.Key:
                        yield return _keyDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relation, numColumns);
                        await _keyDeleteMessage.Consume(cancellationToken);
                        continue;
                    case TupleType.OldTuple:
                        yield return _fullDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relation, numColumns);
                        await _fullDeleteMessage.Consume(cancellationToken);
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleDataType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Truncate:
                {
                    uint? transactionXid;
                    if (inStreamingTransaction)
                    {
                        await buf.EnsureAsync(9);
                        transactionXid = buf.ReadUInt32();
                    }
                    else
                    {
                        await buf.EnsureAsync(5);
                        transactionXid = null;
                    }

                    // Don't dare to truncate more than 2147483647 tables at once!
                    var numRels = checked((int)buf.ReadUInt32());
                    var truncateOptions = (TruncateMessage.TruncateOptions)buf.ReadByte();
                    _truncateMessageRelations.Count = numRels;
                    for (var i = 0; i < numRels; i++)
                    {
                        await buf.EnsureAsync(4);

                        var relationId = buf.ReadUInt32();
                        if (!_relations.TryGetValue(relationId, out var relation))
                        {
                            throw new InvalidOperationException(
                                $"Could not find previous Relation message for relation ID {relationId} when processing Update message");
                        }

                        _truncateMessageRelations[i] = relation;
                    }

                    yield return _truncateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        truncateOptions, _truncateMessageRelations);
                    continue;
                }
                case BackendReplicationMessageCode.StreamStart:
                {
                    await buf.EnsureAsync(5);
                    inStreamingTransaction = true;
                    yield return _streamStartMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionXid: buf.ReadUInt32(), streamSegmentIndicator: buf.ReadByte());
                    continue;
                }
                case BackendReplicationMessageCode.StreamStop:
                {
                    inStreamingTransaction = false;
                    yield return _streamStopMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock);
                    continue;
                }
                case BackendReplicationMessageCode.StreamCommit:
                {
                    await buf.EnsureAsync(29);
                    yield return _streamCommitMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionXid: buf.ReadUInt32(), flags: buf.ReadByte(), commitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        transactionEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        transactionCommitTimestamp: DateTimeUtils.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Unspecified));
                    continue;
                }
                case BackendReplicationMessageCode.StreamAbort:
                {
                    await buf.EnsureAsync(8);
                    yield return _streamAbortMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionXid: buf.ReadUInt32(), subtransactionXid: buf.ReadUInt32());
                    continue;
                }
                default:
                    throw new NotSupportedException(
                        $"Invalid message code {messageCode} in Logical Replication Protocol.");
                }
            }

            // We never get here - the above is an endless loop that terminates only with a cancellation exception
        }

        enum BackendReplicationMessageCode : byte
        {
            Begin = (byte)'B',
            Message = (byte)'M',
            Commit = (byte)'C',
            Origin = (byte)'O',
            Relation = (byte)'R',
            Type = (byte)'Y',
            Insert = (byte)'I',
            Update = (byte)'U',
            Delete = (byte)'D',
            Truncate = (byte)'T',
            StreamStart = (byte)'S',
            StreamStop = (byte)'E',
            StreamCommit = (byte)'c',
            StreamAbort = (byte)'A',
        }
    }
}
