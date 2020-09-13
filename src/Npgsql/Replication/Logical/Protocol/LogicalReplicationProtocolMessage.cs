using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// The base class of all Logical Replication Protocol Messages
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html for details about the
    /// protocol.
    /// </remarks>
    [PublicAPI]
    public abstract class LogicalReplicationProtocolMessage : INpgsqlReplicationMessage
    {
        private protected LogicalReplicationProtocolMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd,
            DateTime serverClock)
        {
            WalStart = walStart;
            WalEnd = walEnd;
            ServerClock = serverClock;
        }

        /// <inheritdoc />
        [PublicAPI]
        public NpgsqlLogSequenceNumber WalStart { get; }

        /// <inheritdoc />
        [PublicAPI]
        public NpgsqlLogSequenceNumber WalEnd { get; }

        /// <inheritdoc />
        [PublicAPI]
        public DateTime ServerClock { get; }
    }
}
