using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol truncate message
    /// </summary>
    public sealed class TruncateMessage : TransactionalMessage
    {
        /// <summary>
        /// Option flags for TRUNCATE
        /// </summary>
        public TruncateOptions Options { get; private set; }

        /// <summary>
        /// IDs of the relations corresponding to the ID in the relation message.
        /// </summary>
        public IReadOnlyList<uint> RelationIds { get; private set; } = ReadonlyArrayBuffer<uint>.Empty;

        internal TruncateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, TruncateOptions options,
            ReadonlyArrayBuffer<uint> relationIds)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            Options = options;
            RelationIds = relationIds;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override TruncateMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new TruncateMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, Options, ((ReadonlyArrayBuffer<uint>)RelationIds).Clone());
            return clone;
        }
    }
}
