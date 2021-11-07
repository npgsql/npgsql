using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol truncate message
    /// </summary>
    public sealed class TruncateMessage : TransactionalMessage
    {
        /// <summary>
        /// Option flags for TRUNCATE
        /// </summary>
        public TruncateOptions Options { get; private set; }

        /// <summary>
        /// The relations being truncated.
        /// </summary>
        public IReadOnlyList<RelationMessage> Relations { get; private set; } = ReadOnlyArrayBuffer<RelationMessage>.Empty;

        internal TruncateMessage() {}

        internal TruncateMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, TruncateOptions options,
            ReadOnlyArrayBuffer<RelationMessage> relations)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            Options = options;
            Relations = relations;
            return this;
        }

        /// <summary>
        /// Enum representing the additional options for the TRUNCATE command as flags
        /// </summary>
        [Flags]
        public enum TruncateOptions : byte
        {
            /// <summary>
            /// No additional option was specified
            /// </summary>
            None = 0,

            /// <summary>
            /// CASCADE was specified
            /// </summary>
            Cascade = 1,

            /// <summary>
            /// RESTART IDENTITY was specified
            /// </summary>
            RestartIdentity = 2
        }
    }
}
