using System;
using NpgsqlTypes;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol type message
    /// </summary>
    public sealed class TypeMessage : LogicalReplicationProtocolMessage
    {
        internal TypeMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint typeId,
            string ns, string name) : base(walStart, walEnd, serverClock)
        {
            TypeId = typeId;
            Namespace = ns;
            Name = name;
        }

        /// <summary>
        /// ID of the data type.
        /// </summary>
        public uint TypeId { get; }

        /// <summary>
        /// Namespace (empty string for pg_catalog).
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        /// Name of the data type.
        /// </summary>
        public string Name { get; }
    }
}
