using NpgsqlTypes;
using System;

namespace Npgsql.Replication.TestDecoding
{
    /// <summary>
    /// Text representations of PostgreSQL WAL operations decoded by the "test_decoding" plugin. See
    /// https://www.postgresql.org/docs/current/test-decoding.html.
    /// </summary>
    public sealed class TestDecodingData : ReplicationMessage
    {
        /// <summary>
        /// Decoded text representation of the operation performed in this WAL entry
        /// </summary>
        public string Data { get; private set; } = default!;

        internal TestDecodingData Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, string data)
        {
            base.Populate(walStart, walEnd, serverClock);

            Data = data;

            return this;
        }
    }
}
