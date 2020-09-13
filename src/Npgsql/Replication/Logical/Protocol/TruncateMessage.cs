using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol truncate message
    /// </summary>
    [PublicAPI]
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
        [PublicAPI]
        public TruncateOptions Options { get; }

        /// <summary>
        /// IDs of the relations corresponding to the ID in the relation message.
        /// </summary>
        [PublicAPI]
        public uint[] RelationIds { get; }
    }
}
