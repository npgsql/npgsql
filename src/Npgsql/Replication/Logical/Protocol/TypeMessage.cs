using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol type message
    /// </summary>
    [PublicAPI]
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
        [PublicAPI]
        public uint TypeId { get; }

        /// <summary>
        /// Namespace (empty string for pg_catalog).
        /// </summary>
        [PublicAPI]
        public string Namespace { get; }

        /// <summary>
        /// Name of the data type.
        /// </summary>
        [PublicAPI]
        public string Name { get; }
    }
}
