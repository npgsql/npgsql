using JetBrains.Annotations;
using NpgsqlTypes;
using System;

namespace Npgsql.Replication.Logical.Protocol
{
    /// <summary>
    /// Logical Replication Protocol relation message
    /// </summary>
    [PublicAPI]
    public sealed class RelationMessage : LogicalReplicationProtocolMessage
    {
        internal RelationMessage(NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock,
            uint relationId, string ns, string relationName, char relationReplicaIdentitySetting,
            RelationMessageColumn[] columns) : base(walStart, walEnd, serverClock)
        {
            RelationId = relationId;
            Namespace = ns;
            RelationName = relationName;
            RelationReplicaIdentitySetting = relationReplicaIdentitySetting;
            Columns = columns;
        }

        /// <summary>
        /// ID of the relation.
        /// </summary>
        [PublicAPI]
        public uint RelationId { get; }

        /// <summary>
        /// Namespace (empty string for pg_catalog).
        /// </summary>
        [PublicAPI]
        public string Namespace { get; }

        /// <summary>
        /// Relation name.
        /// </summary>
        [PublicAPI]
        public string RelationName { get; }

        /// <summary>
        /// Replica identity setting for the relation (same as relreplident in pg_class).
        /// </summary>
        [PublicAPI]
        public char RelationReplicaIdentitySetting { get; }

        /// <summary>
        /// Relation columns
        /// </summary>
        [PublicAPI]
        public RelationMessageColumn[] Columns { get; }
    }
}
