using NpgsqlTypes;

namespace Npgsql.FrontendMessages
{
    // NOTE: see https://www.postgresql.org/docs/current/static/protocol-replication.html for details.

    class StandbyStatusUpdateMessage : SimpleFrontendMessage
    {
        internal override int Length =>
            1    // Message type
            + 8  // LastWrittenLsn
            + 8  // LastFlushedLsn
            + 8  // LastAppliedLsn
            + 8  // SystemClock
            + 1; // ReplyImmediately

        /// <summary>
        /// The location of the last WAL byte + 1 received and written to disk in the standby.
        /// </summary>
        public NpgsqlLsn LastWrittenLsn { get; set; }

        /// <summary>
        /// The location of the last WAL byte + 1 flushed to disk in the standby.
        /// </summary>
        public NpgsqlLsn LastFlushedLsn { get; set; }

        /// <summary>
        /// The location of the last WAL byte + 1 applied in the standby.
        /// </summary>
        public NpgsqlLsn LastAppliedLsn { get; set; }

        /// <summary>
        /// The client's system clock at the time of transmission, as microseconds since midnight on 2000-01-01.
        /// </summary>
        public long SystemClock { get; set; }

        /// <summary>
        /// If true, the client requests the server to reply to this message immediately. This can be used to ping the server, to test if the connection is still healthy.
        /// </summary>
        public bool ReplyImmediately { get; set; }

        internal override void WriteFully(NpgsqlWriteBuffer buf)
        {
            buf.WriteByte((byte)'r');

            buf.WriteUInt32(LastWrittenLsn.Upper);
            buf.WriteUInt32(LastWrittenLsn.Lower);

            buf.WriteUInt32(LastFlushedLsn.Upper);
            buf.WriteUInt32(LastFlushedLsn.Lower);

            buf.WriteUInt32(LastAppliedLsn.Upper);
            buf.WriteUInt32(LastAppliedLsn.Lower);

            buf.WriteInt64(SystemClock);

            buf.WriteByte((byte)(ReplyImmediately ? 1 : 0));
        }
    }
}
