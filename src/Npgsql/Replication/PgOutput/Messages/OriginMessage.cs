using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol origin message
    /// </summary>
    public sealed class OriginMessage : LogicalReplicationProtocolMessage
    {
        internal OriginMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            NpgsqlLogSequenceNumber originCommitLsn, string originName) : base(walStart, walEnd, serverClock)
        {
            OriginCommitLsn = originCommitLsn;
            OriginName = originName;
        }

        /// <summary>
        /// The LSN of the commit on the origin server.
        /// </summary>
        public NpgsqlLogSequenceNumber OriginCommitLsn { get; }

        /// <summary>
        /// Name of the origin.
        /// </summary>
        public string OriginName { get; }
    }
}
