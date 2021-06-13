using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Replication.Internal;
using Npgsql.Replication.PgOutput.Messages;
using Npgsql.TypeHandlers.DateTimeHandlers;
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

        readonly BeginMessage _beginMessage = new BeginMessage();
        readonly CommitMessage _commitMessage = new CommitMessage();
        readonly FullDeleteMessage _fullDeleteMessage = new FullDeleteMessage();
        readonly FullUpdateMessage _fullUpdateMessage = new FullUpdateMessage();
        readonly IndexUpdateMessage _indexUpdateMessage = new IndexUpdateMessage();
        readonly InsertMessage _insertMessage = new InsertMessage();
        readonly KeyDeleteMessage _keyDeleteMessage = new KeyDeleteMessage();
        readonly OriginMessage _originMessage = new OriginMessage();
        readonly RelationMessage _relationMessage = new RelationMessage();
        readonly TruncateMessage _truncateMessage = new TruncateMessage();
        readonly TypeMessage _typeMessage = new TypeMessage();
        readonly UpdateMessage _updateMessage = new UpdateMessage();

        TupleData[] _tupleDataArray1 = Array.Empty<TupleData>();
        TupleData[] _tupleDataArray2 = Array.Empty<TupleData>();
        RelationMessage.Column[] _relationalMessageColumns = Array.Empty<RelationMessage.Column>();

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

        public IAsyncEnumerator<PgOutputReplicationMessage> GetAsyncEnumerator(
            CancellationToken cancellationToken = new CancellationToken())
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

            await foreach (var xLogData in stream.WithCancellation(cancellationToken))
            {
                await buf.EnsureAsync(1);
                var messageCode = (BackendReplicationMessageCode)buf.ReadByte();
                switch (messageCode)
                {
                case BackendReplicationMessageCode.Begin:
                {
                    await buf.EnsureAsync(20);
                    yield return _beginMessage.Populate(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        TimestampHandler.FromPostgresTimestamp(buf.ReadInt64()),
                        buf.ReadUInt32()
                    );
                    continue;
                }
                case BackendReplicationMessageCode.Commit:
                {
                    await buf.EnsureAsync(25);
                    yield return _commitMessage.Populate(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        buf.ReadByte(),
                        new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        TimestampHandler.FromPostgresTimestamp(buf.ReadInt64())
                    );
                    continue;
                }
                case BackendReplicationMessageCode.Origin:
                {
                    await buf.EnsureAsync(9);
                    yield return _originMessage.Populate(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        await buf.ReadNullTerminatedString(async: true, cancellationToken));
                    continue;
                }
                case BackendReplicationMessageCode.Relation:
                {
                    await buf.EnsureAsync(6);
                    var relationId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    var relationName = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    await buf.EnsureAsync(3);
                    var relationReplicaIdentitySetting = (char)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    if (numColumns > _relationalMessageColumns.Length)
                        _relationalMessageColumns = new RelationMessage.Column[numColumns];
                    for (var i = 0; i < numColumns; i++)
                    {
                        await buf.EnsureAsync(2);
                        var flags = buf.ReadByte();
                        var columnName = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                        await buf.EnsureAsync(8);
                        var dateTypeId = buf.ReadUInt32();
                        var typeModifier = buf.ReadInt32();
                        _relationalMessageColumns[i] = new RelationMessage.Column(flags, columnName, dateTypeId, typeModifier);
                    }

                    yield return _relationMessage.Populate(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        relationId,
                        ns,
                        relationName,
                        relationReplicaIdentitySetting,
                        new ReadOnlyMemory<RelationMessage.Column>(_relationalMessageColumns, 0, numColumns)
                    );

                    continue;
                }
                case BackendReplicationMessageCode.Type:
                {
                    await buf.EnsureAsync(5);
                    var typeId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    var name = await buf.ReadNullTerminatedString(async: true, cancellationToken);
                    yield return _typeMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, typeId, ns, name);

                    continue;
                }
                case BackendReplicationMessageCode.Insert:
                {
                    await buf.EnsureAsync(7);
                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    Debug.Assert(tupleDataType == TupleType.NewTuple);
                    var numColumns = buf.ReadUInt16();
                    var newRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                    yield return _insertMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, relationId, newRow);

                    continue;
                }
                case BackendReplicationMessageCode.Update:
                {
                    await buf.EnsureAsync(7);
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
                        yield return _indexUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow, keyRow);
                        continue;
                    case TupleType.OldTuple:
                        var oldRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                        await buf.EnsureAsync(3);
                        tupleType = (TupleType)buf.ReadByte();
                        Debug.Assert(tupleType == TupleType.NewTuple);
                        numColumns = buf.ReadUInt16();
                        newRow = await ReadTupleDataAsync(ref _tupleDataArray2, numColumns);
                        yield return _fullUpdateMessage.Populate(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow, oldRow);
                        continue;
                    case TupleType.NewTuple:
                        newRow = await ReadTupleDataAsync(ref _tupleDataArray1, numColumns);
                        yield return _updateMessage.Populate(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow);
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Delete:
                {
                    await buf.EnsureAsync(7);
                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    switch (tupleDataType)
                    {
                    case TupleType.Key:
                        yield return _keyDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                            relationId, await ReadTupleDataAsync(ref _tupleDataArray1, numColumns));
                        continue;
                    case TupleType.OldTuple:
                        yield return _fullDeleteMessage.Populate(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                            relationId, await ReadTupleDataAsync(ref _tupleDataArray1, numColumns));
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleDataType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Truncate:
                {
                    await buf.EnsureAsync(9);
                    // Don't dare to truncate more than 2147483647 tables at once!
                    var numRels = checked((int)buf.ReadUInt32());
                    var truncateOptions = (TruncateOptions)buf.ReadByte();
                    var relationIds = new uint[numRels];
                    await buf.EnsureAsync(checked(numRels * 4));

                    for (var i = 0; i < numRels; i++)
                        relationIds[i] = buf.ReadUInt32();

                    yield return _truncateMessage.Populate(
                        xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, truncateOptions, relationIds);
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
            Commit = (byte)'C',
            Origin = (byte)'O',
            Relation = (byte)'R',
            Type = (byte)'Y',
            Insert = (byte)'I',
            Update = (byte)'U',
            Delete = (byte)'D',
            Truncate = (byte)'T'
        }

        enum TupleType : byte
        {
            Key = (byte)'K',
            NewTuple = (byte)'N',
            OldTuple = (byte)'O',
        }
    }
}
