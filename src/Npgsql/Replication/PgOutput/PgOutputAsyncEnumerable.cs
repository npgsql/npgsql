using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput;

sealed class PgOutputAsyncEnumerable : IAsyncEnumerable<PgOutputReplicationMessage>
{
    readonly PgOutputProtocolVersion _protocolVersion;
    readonly LogicalReplicationConnection _connection;
    readonly PgOutputReplicationSlot _slot;
    readonly PgOutputReplicationOptions _options;
    readonly CancellationToken _baseCancellationToken;
    readonly NpgsqlLogSequenceNumber? _walLocation;

    #region Cached logical streaming replication protocol messages

    // V1
    readonly BeginMessage _beginMessage = new();
    readonly LogicalDecodingMessage _logicalDecodingMessage = new();
    readonly CommitMessage _commitMessage = new();
    readonly OriginMessage _originMessage = new();
    readonly Dictionary<uint, RelationMessage> _relations = new();
    readonly TypeMessage _typeMessage = new();
    readonly InsertMessage _insertMessage;
    readonly DefaultUpdateMessage _defaultUpdateMessage;
    readonly FullUpdateMessage _fullUpdateMessage;
    readonly IndexUpdateMessage _indexUpdateMessage;
    readonly FullDeleteMessage _fullDeleteMessage;
    readonly KeyDeleteMessage _keyDeleteMessage;
    readonly TruncateMessage _truncateMessage = new();
    readonly ReadOnlyArrayBuffer<RelationMessage> _truncateMessageRelations = new();

    // V2
    readonly StreamStartMessage _streamStartMessage = null!;
    readonly StreamStopMessage _streamStopMessage = null!;
    readonly StreamCommitMessage _streamCommitMessage = null!;
    readonly StreamAbortMessage _streamAbortMessage = null!;

    // V3
    readonly BeginPrepareMessage _beginPrepareMessage = null!;
    readonly PrepareMessage _prepareMessage = null!;
    readonly CommitPreparedMessage _commitPreparedMessage = null!;
    readonly RollbackPreparedMessage _rollbackPreparedMessage = null!;
    readonly StreamPrepareMessage _streamPrepareMessage = null!;

    // V4
    readonly ParallelStreamAbortMessage _parallelStreamAbortMessage = null!;

    #endregion

    internal PgOutputAsyncEnumerable(
        LogicalReplicationConnection connection,
        PgOutputReplicationSlot slot,
        PgOutputReplicationOptions options,
        CancellationToken cancellationToken,
        NpgsqlLogSequenceNumber? walLocation = null)
    {
        _protocolVersion = options.ProtocolVersion;
        _connection = connection;
        _slot = slot;
        _options = options;
        _baseCancellationToken = cancellationToken;
        _walLocation = walLocation;


        if (_protocolVersion >= PgOutputProtocolVersion.V2)
        {
            _streamStartMessage = new();
            _streamStopMessage = new();
            _streamCommitMessage = new();
        }
        if (_protocolVersion >= PgOutputProtocolVersion.V3)
        {
            _beginPrepareMessage = new();
            _prepareMessage = new();
            _commitPreparedMessage = new();
            _rollbackPreparedMessage = new();
            _streamPrepareMessage = new();
        }

        if (_protocolVersion >= PgOutputProtocolVersion.V4)
        {
            _parallelStreamAbortMessage = new();
        }
        else if (_protocolVersion >= PgOutputProtocolVersion.V2)
        {
            _streamAbortMessage = new();
        }

        var connector = _connection.Connector;
        _insertMessage = new(connector);
        _defaultUpdateMessage = new(connector);
        _fullUpdateMessage = new(connector);
        _indexUpdateMessage = new(connector);
        _fullDeleteMessage = new(connector);
        _keyDeleteMessage = new(connector);
    }

    public IAsyncEnumerator<PgOutputReplicationMessage> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => StartReplicationInternal(CancellationTokenSource.CreateLinkedTokenSource(_baseCancellationToken, cancellationToken).Token);

    async IAsyncEnumerator<PgOutputReplicationMessage> StartReplicationInternal(CancellationToken cancellationToken)
    {
        var stream = _connection.StartLogicalReplication(
            _slot, cancellationToken, _walLocation, _options.GetOptionPairs(), bypassingStream: true);
        var buf = _connection.Connector!.ReadBuffer;
        var inStreamingTransaction = false;
        var dataFormat = _options.Binary ?? false ? DataFormat.Binary : DataFormat.Text;

        await foreach (var xLogData in stream.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            await buf.EnsureAsync(1).ConfigureAwait(false);
            var messageCode = (BackendReplicationMessageCode)buf.ReadByte();
            switch (messageCode)
            {
            case BackendReplicationMessageCode.Begin:
            {
                await buf.EnsureAsync(20).ConfigureAwait(false);
                yield return _beginMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    transactionFinalLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionCommitTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32());
                continue;
            }
            case BackendReplicationMessageCode.Message:
            {
                uint? transactionXid;
                if (inStreamingTransaction)
                {
                    await buf.EnsureAsync(14).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(10).ConfigureAwait(false);
                    transactionXid = null;
                }

                var flags = buf.ReadByte();
                var messageLsn = new NpgsqlLogSequenceNumber(buf.ReadUInt64());
                var prefix = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                await buf.EnsureAsync(4).ConfigureAwait(false);
                var length = buf.ReadUInt32();
                var data = (NpgsqlReadBuffer.ColumnStream)xLogData.Data;
                data.Init(checked((int)length), canSeek: false, commandScoped: false);
                yield return _logicalDecodingMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                    flags, messageLsn, prefix, data);
                await data.DisposeAsync().ConfigureAwait(false);
                continue;
            }
            case BackendReplicationMessageCode.Commit:
            {
                await buf.EnsureAsync(25).ConfigureAwait(false);
                yield return _commitMessage.Populate(
                    xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    (CommitMessage.CommitFlags)buf.ReadByte(),
                    commitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionCommitTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc));
                continue;
            }
            case BackendReplicationMessageCode.Origin:
            {
                await buf.EnsureAsync(9).ConfigureAwait(false);
                yield return _originMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    originCommitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    originName: await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false));
                continue;
            }
            case BackendReplicationMessageCode.Relation:
            {
                uint? transactionXid;
                if (inStreamingTransaction)
                {
                    await buf.EnsureAsync(10).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(6).ConfigureAwait(false);
                    transactionXid = null;
                }

                var relationId = buf.ReadUInt32();
                var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                var relationName = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                await buf.EnsureAsync(3).ConfigureAwait(false);
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
                    await buf.EnsureAsync(2).ConfigureAwait(false);
                    var flags = (RelationMessage.Column.ColumnFlags)buf.ReadByte();
                    var columnName = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                    await buf.EnsureAsync(8).ConfigureAwait(false);
                    var dateTypeId = buf.ReadUInt32();
                    var typeModifier = buf.ReadInt32();
                    columns[i] = new RelationMessage.Column(flags, columnName, dateTypeId, typeModifier);
                }

                msg.RowDescription = RowDescriptionMessage.CreateForReplication(
                    _connection.Connector.SerializerOptions, relationId, dataFormat, columns);

                yield return msg;
                continue;
            }
            case BackendReplicationMessageCode.Type:
            {
                uint? transactionXid;
                if (inStreamingTransaction)
                {
                    await buf.EnsureAsync(9).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(5).ConfigureAwait(false);
                    transactionXid = null;
                }

                var typeId = buf.ReadUInt32();
                var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                var name = await buf.ReadNullTerminatedString(async: true, cancellationToken).ConfigureAwait(false);
                yield return _typeMessage.Populate(
                    xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid, typeId, ns, name);
                continue;
            }
            case BackendReplicationMessageCode.Insert:
            {
                uint? transactionXid;
                if (inStreamingTransaction)
                {
                    await buf.EnsureAsync(11).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(7).ConfigureAwait(false);
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
                await _insertMessage.Consume(cancellationToken).ConfigureAwait(false);

                continue;
            }
            case BackendReplicationMessageCode.Update:
            {
                uint? transactionXid;
                if (inStreamingTransaction)
                {
                    await buf.EnsureAsync(11).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(7).ConfigureAwait(false);
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
                    await _indexUpdateMessage.Consume(cancellationToken).ConfigureAwait(false);
                    continue;
                case TupleType.OldTuple:
                    yield return _fullUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        relation, numColumns);
                    await _fullUpdateMessage.Consume(cancellationToken).ConfigureAwait(false);
                    continue;
                case TupleType.NewTuple:
                    yield return _defaultUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        relation, numColumns);
                    await _defaultUpdateMessage.Consume(cancellationToken).ConfigureAwait(false);
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
                    await buf.EnsureAsync(11).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(7).ConfigureAwait(false);
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
                    await _keyDeleteMessage.Consume(cancellationToken).ConfigureAwait(false);
                    continue;
                case TupleType.OldTuple:
                    yield return _fullDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        relation, numColumns);
                    await _fullDeleteMessage.Consume(cancellationToken).ConfigureAwait(false);
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
                    await buf.EnsureAsync(9).ConfigureAwait(false);
                    transactionXid = buf.ReadUInt32();
                }
                else
                {
                    await buf.EnsureAsync(5).ConfigureAwait(false);
                    transactionXid = null;
                }

                // Don't dare to truncate more than 2147483647 tables at once!
                var numRels = checked((int)buf.ReadUInt32());
                var truncateOptions = (TruncateMessage.TruncateOptions)buf.ReadByte();
                _truncateMessageRelations.Count = numRels;
                for (var i = 0; i < numRels; i++)
                {
                    await buf.EnsureAsync(4).ConfigureAwait(false);

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
                await buf.EnsureAsync(5).ConfigureAwait(false);
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
                await buf.EnsureAsync(29).ConfigureAwait(false);
                yield return _streamCommitMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    transactionXid: buf.ReadUInt32(), flags: buf.ReadByte(), commitLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionCommitTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc));
                continue;
            }
            case BackendReplicationMessageCode.StreamAbort:
            {
                if (_protocolVersion >= PgOutputProtocolVersion.V4)
                {
                    await buf.EnsureAsync(24).ConfigureAwait(false);
                    yield return _parallelStreamAbortMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionXid: buf.ReadUInt32(),
                        subtransactionXid: buf.ReadUInt32(),
                        abortLsn: new(buf.ReadUInt64()),
                        abortTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc));

                }
                else
                {
                    await buf.EnsureAsync(8).ConfigureAwait(false);
                    yield return _streamAbortMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        transactionXid: buf.ReadUInt32(), subtransactionXid: buf.ReadUInt32());

                }
                continue;
            }
            case BackendReplicationMessageCode.BeginPrepare:
            {
                await buf.EnsureAsync(29).ConfigureAwait(false);
                yield return _beginPrepareMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    prepareLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    prepareEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionPrepareTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32(),
                    transactionGid: buf.ReadNullTerminatedString());
                continue;
            }
            case BackendReplicationMessageCode.Prepare:
            {
                await buf.EnsureAsync(30).ConfigureAwait(false);
                yield return _prepareMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    flags: (PrepareMessage.PrepareFlags)buf.ReadByte(),
                    prepareLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    prepareEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionPrepareTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32(),
                    transactionGid: buf.ReadNullTerminatedString());
                continue;
            }
            case BackendReplicationMessageCode.CommitPrepared:
            {
                await buf.EnsureAsync(30).ConfigureAwait(false);
                yield return _commitPreparedMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    flags: (CommitPreparedMessage.CommitPreparedFlags)buf.ReadByte(),
                    commitPreparedLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    commitPreparedEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionCommitTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32(),
                    transactionGid: buf.ReadNullTerminatedString());
                continue;
            }
            case BackendReplicationMessageCode.RollbackPrepared:
            {
                await buf.EnsureAsync(38).ConfigureAwait(false);
                yield return _rollbackPreparedMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    flags: (RollbackPreparedMessage.RollbackPreparedFlags)buf.ReadByte(),
                    preparedTransactionEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    rollbackPreparedEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionPrepareTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionRollbackTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32(),
                    transactionGid: buf.ReadNullTerminatedString());
                continue;
            }
            case BackendReplicationMessageCode.StreamPrepare:
            {
                await buf.EnsureAsync(30).ConfigureAwait(false);
                yield return _streamPrepareMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                    flags: (StreamPrepareMessage.StreamPrepareFlags)buf.ReadByte(),
                    prepareLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    prepareEndLsn: new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                    transactionPrepareTimestamp: PgDateTime.DecodeTimestamp(buf.ReadInt64(), DateTimeKind.Utc),
                    transactionXid: buf.ReadUInt32(),
                    transactionGid: buf.ReadNullTerminatedString());
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
        BeginPrepare = (byte)'b',
        Prepare = (byte)'P',
        CommitPrepared = (byte)'K',
        RollbackPrepared = (byte)'r',
        StreamPrepare = (byte)'p',
    }
}
