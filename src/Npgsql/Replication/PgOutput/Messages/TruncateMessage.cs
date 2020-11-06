using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol truncate message
    /// </summary>
    public sealed class TruncateMessage : LogicalReplicationProtocolMessage
    {
        /// <summary>
        /// Option flags for TRUNCATE
        /// </summary>
        public TruncateOptions Options { get; private set; }

        /// <summary>
        /// IDs of the relations corresponding to the ID in the relation message.
        /// </summary>
        public uint[] RelationIds { get; private set; } = default!;

        internal TruncateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, TruncateOptions options,
            uint[] relationIds)
        {
            base.Populate(walStart, walEnd, serverClock);

            Options = options;
            RelationIds = relationIds;

            return this;
        }
    }
}
