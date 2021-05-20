using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol stream stop message
    /// </summary>
    public sealed class StreamStopMessage : PgOutputReplicationMessage
    {
        internal new StreamStopMessage Populate(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock)
        {
            base.Populate(walStart, walEnd, serverClock);
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override StreamStopMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new StreamStopMessage();
            clone.Populate(WalStart, WalEnd, ServerClock);
            return clone;
        }
    }
}
