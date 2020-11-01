using JetBrains.Annotations;
using Npgsql.Replication.Logical.Internal;
using Npgsql.TypeHandlers.DateTimeHandlers;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Extension methods to use <see cref="NpgsqlLogicalReplicationConnection"/> with the
    /// logical streaming replication protocol.
    /// See <a href="https://www.postgresql.org/docs/current/protocol-logical-replication.html">https://www.postgresql.org/docs/current/protocol-logical-replication.html</a>
    /// and <a href="https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html">https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html</a>.
    /// </summary>
    public static class NpgsqlLogicalReplicationConnectionExtensions
    {
        /// <summary>
        /// Creates a <see cref="NpgsqlPgOutputReplicationSlot"/> class that wraps a replication slot using the
        /// "pgoutput" logical decoding plugin and can be used to start streaming replication via the logical
        /// streaming replication protocol.
        /// </summary>
        /// <remarks>
        /// See <a href="https://www.postgresql.org/docs/current/protocol-logical-replication.html">https://www.postgresql.org/docs/current/protocol-logical-replication.html</a>
        /// and <a href="https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html">https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html</a>
        /// for more information.
        /// </remarks>
        /// <param name="connection">The <see cref="NpgsqlLogicalReplicationConnection"/> to use for creating the
        /// replication slot</param>
        /// <param name="slotName">The name of the slot to create. Must be a valid replication slot name (see
        /// <a href="https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION">https://www.postgresql.org/docs/current/warm-standby.html#STREAMING-REPLICATION-SLOTS-MANIPULATION</a>).
        /// </param>
        /// <param name="temporarySlot">
        /// <see langword="true"/> if this replication slot shall be temporary one; otherwise <see langword="false"/>.
        /// Temporary slots are not saved to disk and are automatically dropped on error or when the session has finished.
        /// </param>
        /// <param name="slotSnapshotInitMode">
        /// A <see cref="SlotSnapshotInitMode"/> to specify what to do with the snapshot created during logical slot
        /// initialization. <see cref="SlotSnapshotInitMode.Export"/>, which is also the default, will export the
        /// snapshot for use in other sessions. This option can't be used inside a transaction.
        /// <see cref="SlotSnapshotInitMode.Use"/> will use the snapshot for the current transaction executing the
        /// command. This option must be used in a transaction, and <see cref="SlotSnapshotInitMode.Use"/> must be the
        /// first command run in that transaction. Finally, <see cref="SlotSnapshotInitMode.NoExport"/> will just use
        /// the snapshot for logical decoding as normal but won't do anything else with it.
        /// </param>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// The default value is <see cref="CancellationToken.None"/>.
        /// </param>
        /// <returns>
        /// A <see cref="NpgsqlPgOutputReplicationSlot"/> that wraps the newly-created replication slot.
        /// </returns>
        public static async Task<NpgsqlPgOutputReplicationSlot> CreateReplicationSlot(
            this NpgsqlLogicalReplicationConnection connection,
            string slotName,
            bool temporarySlot = false,
            SlotSnapshotInitMode? slotSnapshotInitMode = null,
            CancellationToken cancellationToken = default)
        {
            // We don't enter NoSynchronizationContextScope here since we (have to) do it in CreateReplicationSlotForPlugin, because
            // otherwise it couldn't be set for external plugins.
            var options =
                await connection.CreateReplicationSlotForPlugin(slotName, "pgoutput", temporarySlot, slotSnapshotInitMode, cancellationToken);
            return new NpgsqlPgOutputReplicationSlot(options);
        }

        /// <summary>
        /// Instructs the server to start the Logical Streaming Replication Protocol (pgoutput logical decoding plugin),
        /// starting at WAL location <paramref name="walLocation"/> or at the slot's consistent point if <paramref name="walLocation"/>
        /// isn't specified.
        /// The server can reply with an error, for example if the requested section of the WAL has already been recycled.
        /// </summary>
        /// <param name="connection">The <see cref="NpgsqlLogicalReplicationConnection"/> to use for starting replication</param>
        /// <param name="slot">The replication slot that will be updated as replication progresses so that the server
        /// knows which WAL segments are still needed by the standby.
        /// </param>
        /// <param name="options">The collection of options passed to the slot's logical decoding plugin.</param>
        /// <param name="cancellationToken">The token to monitor for stopping the replication.</param>
        /// <param name="walLocation">The WAL location to begin streaming at.</param>
        /// <returns>A <see cref="Task{T}"/> representing an <see cref="IAsyncEnumerable{T}"/> that
        /// can be used to stream WAL entries in form of <see cref="LogicalReplicationProtocolMessage"/> instances.</returns>
        public static IAsyncEnumerable<LogicalReplicationProtocolMessage> StartReplication(
            this NpgsqlLogicalReplicationConnection connection, NpgsqlPgOutputReplicationSlot slot,
            NpgsqlPgOutputPluginOptions options, CancellationToken cancellationToken, NpgsqlLogSequenceNumber? walLocation = null)
        {
            using (NoSynchronizationContextScope.Enter())
                return StartReplicationInternal(connection, slot, walLocation, options, cancellationToken);
        }

        static async IAsyncEnumerable<LogicalReplicationProtocolMessage> StartReplicationInternal(
            NpgsqlLogicalReplicationConnection connection, NpgsqlPgOutputReplicationSlot slot, NpgsqlLogSequenceNumber? walLocation,
            NpgsqlPgOutputPluginOptions options, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var stream = connection.StartReplicationInternal(commandBuilder =>
            {
                commandBuilder.Append("SLOT ").Append(slot.SlotName).Append(' ');
                Internal.NpgsqlLogicalReplicationConnectionExtensions.AppendCommon(
                    commandBuilder, walLocation, options.GetOptionPairs(), slot.ConsistentPoint);
            }, bypassingStream: true, cancellationToken);

            // Hack: NAMEDATALEN is a constant in PostgreSQL which can be changed at compile time.// It's probably saner to query
            // max_identifier_length on connection startup.
            // See https://www.postgresql.org/docs/current/runtime-config-preset.html
            const int NAMEDATALEN = 64;
            var buf = connection.Connector!.ReadBuffer;
            await foreach (var xLogData in stream.WithCancellation(cancellationToken))
            {
                await buf.EnsureAsync(1, cancellationToken);
                var messageCode = (BackendReplicationMessageCode)buf.ReadByte();
                switch (messageCode)
                {
                case BackendReplicationMessageCode.Begin:
                {
                    await buf.EnsureAsync(20, cancellationToken);
                    yield return new BeginMessage(
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
                    await buf.EnsureAsync(25, cancellationToken);
                    yield return new CommitMessage(
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
                    await buf.EnsureAsync(9, cancellationToken);
                    yield return new OriginMessage(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        new NpgsqlLogSequenceNumber(buf.ReadUInt64()),
                        await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken));
                    continue;
                }
                case BackendReplicationMessageCode.Relation:
                {
                    await buf.EnsureAsync(6, cancellationToken);
                    var relationId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken);
                    var relationName = await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken);
                    await buf.EnsureAsync(3, cancellationToken);
                    var relationReplicaIdentitySetting = (char)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    var columns = new RelationMessageColumn[numColumns];
                    for (var i = 0; i < numColumns; i++)
                    {
                        await buf.EnsureAsync(2, cancellationToken);
                        var flags = buf.ReadByte();
                        var columnName = await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken);
                        await buf.EnsureAsync(8, cancellationToken);
                        var dateTypeId = buf.ReadUInt32();
                        var typeModifier = buf.ReadInt32();
                        columns[i] = new RelationMessageColumn(flags, columnName, dateTypeId,
                            typeModifier);
                    }

                    yield return new RelationMessage(
                        xLogData.WalStart,
                        xLogData.WalEnd,
                        xLogData.ServerClock,
                        relationId,
                        ns,
                        relationName,
                        relationReplicaIdentitySetting,
                        columns
                    );

                    continue;
                }
                case BackendReplicationMessageCode.Type:
                {
                    await buf.EnsureAsync(5, cancellationToken);
                    var typeId = buf.ReadUInt32();
                    var ns = await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken);
                    var name = await buf.ReadNullTerminatedStringDefensive(NAMEDATALEN, cancellationToken);
                    yield return new TypeMessage(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock, typeId,
                        ns, name);

                    continue;
                }
                case BackendReplicationMessageCode.Insert:
                {
                    await buf.EnsureAsync(7, cancellationToken);
                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    Debug.Assert(tupleDataType == TupleType.NewTuple);
                    var numColumns = buf.ReadUInt16();
                    var newRow = await ReadTupleDataAsync(numColumns);
                    yield return new InsertMessage(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        relationId, newRow);

                    continue;
                }
                case BackendReplicationMessageCode.Update:
                {
                    await buf.EnsureAsync(7, cancellationToken);
                    var relationId = buf.ReadUInt32();
                    var tupleType = (TupleType)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    switch (tupleType)
                    {
                    case TupleType.Key:
                        var keyRow = await ReadTupleDataAsync(numColumns);
                        await buf.EnsureAsync(3, cancellationToken);
                        tupleType = (TupleType)buf.ReadByte();
                        Debug.Assert(tupleType == TupleType.NewTuple);
                        numColumns = buf.ReadUInt16();
                        var newRow = await ReadTupleDataAsync(numColumns);
                        yield return new IndexUpdateMessage(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow, keyRow);
                        continue;
                    case TupleType.OldTuple:
                        var oldRow = await ReadTupleDataAsync(numColumns);
                        await buf.EnsureAsync(3, cancellationToken);
                        tupleType = (TupleType)buf.ReadByte();
                        Debug.Assert(tupleType == TupleType.NewTuple);
                        numColumns = buf.ReadUInt16();
                        newRow = await ReadTupleDataAsync(numColumns);
                        yield return new FullUpdateMessage(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow, oldRow);
                        continue;
                    case TupleType.NewTuple:
                        newRow = await ReadTupleDataAsync(numColumns);
                        yield return new UpdateMessage(xLogData.WalStart, xLogData.WalEnd,
                            xLogData.ServerClock, relationId, newRow);
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Delete:
                {
                    await buf.EnsureAsync(7, cancellationToken);
                    var relationId = buf.ReadUInt32();
                    var tupleDataType = (TupleType)buf.ReadByte();
                    var numColumns = buf.ReadUInt16();
                    switch (tupleDataType)
                    {
                    case TupleType.Key:
                        yield return new KeyDeleteMessage(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                            relationId, await ReadTupleDataAsync(numColumns));
                        continue;
                    case TupleType.OldTuple:
                        yield return new FullDeleteMessage(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                            relationId, await ReadTupleDataAsync(numColumns));
                        continue;
                    default:
                        throw new NotSupportedException($"The tuple type '{tupleDataType}' is not supported.");
                    }
                }
                case BackendReplicationMessageCode.Truncate:
                {
                    await buf.EnsureAsync(9, cancellationToken);
                    // Don't dare to truncate more than 2147483647 tables at once!
                    var numRels = checked((int)buf.ReadUInt32());
                    var truncateOptions = (TruncateOptions)buf.ReadByte();
                    var relationIds = new uint[numRels];
                    await buf.EnsureAsync(checked(numRels * 4), cancellationToken);

                    for (var i = 0; i < numRels; i++)
                        relationIds[i] = buf.ReadUInt32();

                    yield return new TruncateMessage(xLogData.WalStart, xLogData.WalEnd, xLogData.ServerClock,
                        truncateOptions, relationIds);
                    continue;
                }
                default:
                    throw new NotSupportedException(
                        $"Invalid message code {messageCode} in Logical Replication Protocol.");
                }

                async Task<ITupleData[]> ReadTupleDataAsync(ushort numberOfColumns)
                {
                    var ret = new ITupleData[numberOfColumns];
                    for (var i = 0; i < numberOfColumns; i++)
                    {
                        await buf.EnsureAsync(1, cancellationToken);
                        var subMessageKind = (TupleDataKind)buf.ReadByte();
                        switch (subMessageKind)
                        {
                        case TupleDataKind.Null:
                        case TupleDataKind.UnchangedToastedValue:
                            ret[i] = new TupleData<string>(subMessageKind);
                            break;
                        case TupleDataKind.TextValue:
                            await buf.EnsureAsync(4, cancellationToken);
                            var len = buf.ReadInt32();
                            await buf.EnsureAsync(len, cancellationToken);
                            ret[i] = new TupleData<string>(TupleDataKind.TextValue, buf.ReadString(len));
                            break;
                        default:
                            throw new NotSupportedException(
                                $"The tuple data kind '{subMessageKind}' is not supported.");
                        }
                    }

                    return ret;
                }
            }

            throw new NotImplementedException();
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
