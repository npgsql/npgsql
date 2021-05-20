using NpgsqlTypes;
using System;
using System.Collections.Generic;

namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Logical Replication Protocol relation message
    /// </summary>
    public sealed class RelationMessage : TransactionalMessage
    {
        /// <summary>
        /// ID of the relation.
        /// </summary>
        public uint RelationId { get; private set; }

        /// <summary>
        /// Namespace (empty string for pg_catalog).
        /// </summary>
        public string Namespace { get; private set; } = string.Empty;

        /// <summary>
        /// Relation name.
        /// </summary>
        public string RelationName { get; private set; } = string.Empty;

        /// <summary>
        /// Replica identity setting for the relation (same as relreplident in pg_class).
        /// </summary>
        public char RelationReplicaIdentitySetting { get; private set; }

        /// <summary>
        /// Relation columns
        /// </summary>
        public IReadOnlyList<Column> Columns { get; private set; } = ReadOnlyArrayBuffer<Column>.Empty!;

        internal RelationMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId, string ns,
            string relationName, char relationReplicaIdentitySetting, ReadOnlyArrayBuffer<Column> columns)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);
            RelationId = relationId;
            Namespace = ns;
            RelationName = relationName;
            RelationReplicaIdentitySetting = relationReplicaIdentitySetting;
            Columns = columns;
            return this;
        }

        /// <inheritdoc />
#if NET5_0_OR_GREATER
        public override RelationMessage Clone()
#else
        public override PgOutputReplicationMessage Clone()
#endif
        {
            var clone = new RelationMessage();
            clone.Populate(WalStart, WalEnd, ServerClock, TransactionXid, RelationId, Namespace, RelationName, RelationReplicaIdentitySetting, ((ReadOnlyArrayBuffer<Column>)Columns).Clone());
            return clone;
        }

        /// <summary>
        /// Represents a column in a Logical Replication Protocol relation message
        /// </summary>
        public readonly struct Column
        {
            internal Column(byte flags, string columnName, uint dataTypeId, int typeModifier)
            {
                Flags = flags;
                ColumnName = columnName;
                DataTypeId = dataTypeId;
                TypeModifier = typeModifier;
            }

            /// <summary>
            /// Flags for the column. Currently can be either 0 for no flags or 1 which marks the column as part of the key.
            /// </summary>
            public byte Flags { get; }

            /// <summary>
            /// Name of the column.
            /// </summary>
            public string ColumnName { get; }

            /// <summary>
            /// ID of the column's data type.
            /// </summary>
            public uint DataTypeId { get; }

            /// <summary>
            /// Type modifier of the column (atttypmod).
            /// </summary>
            public int TypeModifier { get; }
        }
    }
}
