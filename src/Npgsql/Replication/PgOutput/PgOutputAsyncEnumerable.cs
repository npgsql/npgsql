using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.DateTimeHandlers;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput.Messages;
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

        #region Cached messages

        readonly BeginMessage _beginMessage = new();
        readonly LogicalDecodingMessage _logicalDecodingMessage = new();
        readonly CommitMessage _commitMessage = new();
        readonly FullDeleteMessage _fullDeleteMessage = new();
        readonly FullUpdateMessage _fullUpdateMessage = new();
        readonly IndexUpdateMessage _indexUpdateMessage = new();
        readonly InsertMessage _insertMessage = new();
        readonly KeyDeleteMessage _keyDeleteMessage = new();
        readonly OriginMessage _originMessage = new();
        readonly RelationMessage _relationMessage = new();
        readonly TruncateMessage _truncateMessage = new();
        readonly TypeMessage _typeMessage = new();
        readonly UpdateMessage _updateMessage = new();
        readonly StreamStartMessage _streamStartMessage = new();
        readonly StreamStopMessage _streamStopMessage = new();
        readonly StreamCommitMessage _streamCommitMessage = new();
        readonly StreamAbortMessage _streamAbortMessage = new();
        readonly ReadOnlyArrayBuffer<RelationMessage.Column> _relationMessageColumns = new();
        readonly ReadOnlyArrayBuffer<uint> _truncateMessageRelationIds = new();

        TupleData[] _tupleDataArray1 = Array.Empty<TupleData>();
        TupleData[] _tupleDataArray2 = Array.Empty<TupleData>();

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
                    yield return _commitMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, buf.ReadByte(),
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
                    var relationReplicaIdentitySetting = (char)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    _relationMessageColumns.Count = numColumns;
                    for (var i = 0; i < numColumns; i++)
                    {
                        await buf.EnsureAsync(2);
                        var flags = buf.ReadByte();
                        var columnName = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                        await buf.EnsureAsync(8);
                        var dateTypeId = buf.ReadUInt32();
                        var typeModifier = buf.ReadInt32();
                        _relationMessageColumns[i] = new RelationMessage.Column(flags, columnName, dateTypeId, typeModifier);
                    }

                    yield return _relationMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        relationId, ns, relationName, relationReplicaIdentitySetting,
                        columns: _relationMessageColumns);
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
                    yield return _typeMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid, typeId, ns,
                        name);
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
                    var newRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                    yield return _insertMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        relationId, newRow);
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
                    switch (tupleType)
                    {
                    case TupleType.Key:
                        var keyRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                        await buf.EnsureAsync(3);
                        tupleType = (TupleType)buf.ReadByte();
                        Debug.Assert(tupleType == TupleType.NewTuple);
                        numColumns = buf.ReadUInt16();
                        var newRow = await ReadTupleDataAsync(ref _tupleDataArray2, numColumns);
                        yield return _indexUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relationId, newRow, keyRow);
                        continue;
                    case TupleType.OldTuple:
                        var oldRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                        await buf.EnsureAsync(3);
                        tupleType = (TupleType)buf.ReadByte();
                        Debug.Assert(tupleType == TupleType.NewTuple);
                        numColumns = buf.ReadUInt16();
                        newRow = await ReadTupleDataAsync(ref _tupleDataArray2, numColumns);
                        yield return _fullUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relationId, newRow, oldRow);
                        continue;
                    case TupleType.NewTuple:
                        newRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                        yield return _updateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relationId, newRow);
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
                    switch (tupleDataType)
                    {
                    case TupleType.Key:
                        yield return _keyDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relationId, keyRow: await ReadTupleDataAsync(ref _tupleDataArray1, numColumns));
                        continue;
                    case TupleType.OldTuple:
                        yield return _fullDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                            relationId, oldRow: await ReadTupleDataAsync(ref _tupleDataArray1, numColumns));
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
                    var truncateOptions = (TruncateOptions)buf.ReadByte();
                    _truncateMessageRelationIds.Count = numRels;
                    for (var i = 0; i < numRels; i++)
                    {
                        await buf.EnsureAsync(4);
                        _truncateMessageRelationIds[i] = buf.ReadUInt32();
                    }

                    yield return _truncateMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, transactionXid,
                        truncateOptions, relationIds: _truncateMessageRelationIds);
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

            ValueTask<ReadOnlyMemory<TupleData>> ReadTupleDataAsync(ref TupleData[] array, ushort numberOfColumns)
            {
                if (array.Length < numberOfColumns)
                    array = new TupleData[numberOfColumns];
                var nonRefArray = array;
                return ReadTupleDataAsync2();

                async ValueTask<ReadOnlyMemory<TupleData>> ReadTupleDataAsync2()
                {
                    for (var i = 0; i < numberOfColumns; i++)
                    {
                        await buf.EnsureAsync(1);
                        var subMessageKind = (TupleDataKind)buf.ReadByte();
                        switch (subMessageKind)
                        {
                        case TupleDataKind.Null:
                        case TupleDataKind.UnchangedToastedValue:
                            nonRefArray[i] = new TupleData(subMessageKind);
                            continue;
                        case TupleDataKind.TextValue:
                            await buf.EnsureAsync(4);
                            var len = buf.ReadInt32();
                            await buf.EnsureAsync(len);
                            nonRefArray![i] = new TupleData(buf.ReadString(len));
                            continue;
                        default:
                            throw new NotSupportedException($"The tuple data kind '{subMessageKind}' is not supported.");
                        }
                    }

                    return new ReadOnlyMemory<TupleData>(nonRefArray, 0, numberOfColumns);
                }
            }
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

        enum TupleType : byte
        {
            Key = (byte)'K',
            NewTuple = (byte)'N',
            OldTuple = (byte)'O',
        }
    }
}
