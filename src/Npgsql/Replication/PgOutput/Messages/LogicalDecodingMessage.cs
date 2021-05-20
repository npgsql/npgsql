using System;
using System.IO;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol logical decoding message
    /// </summary>
    public sealed class LogicalDecodingMessage : TransactionalMessage
    {
        /// <summary>
        /// Flags; Either 0 for no flags or 1 if the logical decoding message is transactional.
        /// </summary>
        public byte Flags { get; private set; }

        /// <summary>
        /// The LSN of the logical decoding message.
        /// </summary>
        public NpgsqlLogSequenceNumber MessageLsn { get; private set; }

        /// <summary>
        /// The prefix of the logical decoding message.
        /// </summary>
        public string Prefix { get; private set; } = default!;

        /// <summary>
        /// The content of the logical decoding message.
        /// </summary>
        public Stream Data { get; private set; } = default!;

        internal LogicalDecodingMessage() {}

        internal LogicalDecodingMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint? transactionXid, byte flags, NpgsqlLogSequenceNumber messageLsn, string prefix, Stream data)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            Flags = flags;
            MessageLsn = messageLsn;
            Prefix = prefix;
            Data = data;
            return this;
        }
    }
}
