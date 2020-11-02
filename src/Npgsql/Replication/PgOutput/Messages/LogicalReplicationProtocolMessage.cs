using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// The base class of all Logical Replication Protocol Messages
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/protocol-logicalrep-message-formats.html for details about the
    /// protocol.
    /// </remarks>
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
        public NpgsqlLogSequenceNumber WalStart { get; }

        /// <inheritdoc />
        public NpgsqlLogSequenceNumber WalEnd { get; }

        /// <inheritdoc />
        public DateTime ServerClock { get; }
    }
}
