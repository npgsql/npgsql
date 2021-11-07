using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Abstract base class for Logical Replication Protocol delete message types.
    /// </summary>
    public abstract class UpdateMessage : TransactionalMessage
    {
        /// <summary>
        /// The relation for this <see cref="InsertMessage" />.
        /// </summary>
        public RelationMessage Relation { get; private set; } = null!;

        /// <summary>
        /// ID of the relation corresponding to the ID in the relation message.
        /// </summary>
        [Obsolete("Use Relation.RelationId")]
        public uint RelationId => Relation.RelationId;

        /// <summary>
        /// Columns representing the new row.
        /// </summary>
        public abstract ReplicationTuple NewRow { get; }

        internal UpdateMessage() {}

        internal UpdateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid,
            RelationMessage relation)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);

            Relation = relation;

            return this;
        }

        private protected class SecondRowTupleEnumerable : ReplicationTuple
        {
            readonly ReplicationTuple _oldRowTupleEnumerable;

            internal SecondRowTupleEnumerable(NpgsqlConnector connector, ReplicationTuple oldRowTupleEnumerable)
                : base(connector)
                => _oldRowTupleEnumerable = oldRowTupleEnumerable;

            public override async IAsyncEnumerator<ReplicationValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                // This will throw if we're already reading (or consumed) the second row
                var enumerator = base.GetAsyncEnumerator(cancellationToken);

                await _oldRowTupleEnumerable.Consume(cancellationToken);
                await ReadBuffer.EnsureAsync(3);
                var tupleType = (TupleType)ReadBuffer.ReadByte();
                Debug.Assert(tupleType == TupleType.NewTuple);
                _ = ReadBuffer.ReadUInt16(); // numColumns,

                while (await enumerator.MoveNextAsync())
                    yield return enumerator.Current;
            }

            internal new async Task Consume(CancellationToken cancellationToken)
            {
                if (State == RowState.NotRead)
                {
                    await _oldRowTupleEnumerable.Consume(cancellationToken);
                    await ReadBuffer.EnsureAsync(3);
                    var tupleType = (TupleType)ReadBuffer.ReadByte();
                    Debug.Assert(tupleType == TupleType.NewTuple);
                    _ = ReadBuffer.ReadUInt16(); // numColumns,
                }
                await base.Consume(cancellationToken);
            }
        }
    }
}
