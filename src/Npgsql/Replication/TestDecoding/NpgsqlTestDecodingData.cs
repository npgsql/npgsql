using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.TestDecoding
{
    /// <summary>
    /// Text representations of PostgreSQL WAL operations decoded by the "test_decoding" plugin. See
    /// https://www.postgresql.org/docs/current/test-decoding.html.
    /// </summary>
    public sealed class NpgsqlTestDecodingData : INpgsqlReplicationMessage
    {
        internal NpgsqlTestDecodingData(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, string data)
        {
            WalStart = walStart;
            WalEnd = walEnd;
            ServerClock = serverClock;
            Data = data;
        }

        /// <inheritdoc />
        public NpgsqlLogSequenceNumber WalStart { get; }

        /// <inheritdoc />
        public NpgsqlLogSequenceNumber WalEnd { get; }

        /// <inheritdoc />
        public DateTime ServerClock { get; }

        /// <summary>
        /// Decoded text representation of the operation performed in this WAL entry
        /// </summary>
        public string Data { get; }
    }
}
