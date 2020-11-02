using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol truncate message
    /// </summary>
    public sealed class TruncateMessage : LogicalReplicationProtocolMessage
    {
        internal TruncateMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            TruncateOptions options, uint[] relationIds) : base(walStart, walEnd, serverClock)
        {
            Options = options;
            RelationIds = relationIds;
        }

        /// <summary>
        /// Option flags for TRUNCATE
        /// </summary>
        public TruncateOptions Options { get; }

        /// <summary>
        /// IDs of the relations corresponding to the ID in the relation message.
        /// </summary>
        public uint[] RelationIds { get; }
    }
}
