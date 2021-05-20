using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Npgsql.BackendMessages;

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
        /// Replica identity setting for the relation (same as <c>relreplident</c> in <c>pg_class</c>):
        /// columns used to form “replica identity” for rows.
        /// </summary>
        public ReplicaIdentitySetting ReplicaIdentity { get; private set; }

        /// <summary>
        /// Relation columns
        /// </summary>
        public IReadOnlyList<Column> Columns => InternalColumns;

        internal ReadOnlyArrayBuffer<Column> InternalColumns { get; private set; } = ReadOnlyArrayBuffer<Column>.Empty;

        internal RowDescriptionMessage RowDescription { get; set; } = null!;

        internal RelationMessage() {}

        internal RelationMessage Populate(
            NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid, uint relationId, string ns,
            string relationName, ReplicaIdentitySetting relationReplicaIdentitySetting)
        {
            base.Populate(walStart, walEnd, serverClock, transactionXid);

            RelationId = relationId;
            Namespace = ns;
            RelationName = relationName;
            ReplicaIdentity = relationReplicaIdentitySetting;

            return this;
        }

        /// <summary>
        /// Represents a column in a Logical Replication Protocol relation message
        /// </summary>
        public readonly struct Column
        {
            internal Column(ColumnFlags flags, string columnName, uint dataTypeId, int typeModifier)
            {
                Flags = flags;
                ColumnName = columnName;
                DataTypeId = dataTypeId;
                TypeModifier = typeModifier;
            }

            /// <summary>
            /// Flags for the column.
            /// </summary>
            public ColumnFlags Flags { get; }

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

            /// <summary>
            /// Flags for the column.
            /// </summary>
            [Flags]
            public enum ColumnFlags
            {
                /// <summary>
                /// No flags.
                /// </summary>
                None = 0,

                /// <summary>
                /// Marks the column as part of the key.
                /// </summary>
                PartOfKey = 1
            }
        }

        /// <summary>
        /// Replica identity setting for the relation (same as <c>relreplident</c> in <c>pg_class</c>).
        /// </summary>
        /// <remarks>
        /// See <see href="https://www.postgresql.org/docs/current/catalog-pg-class.html" />
        /// </remarks>
        public enum ReplicaIdentitySetting : byte
        {
            /// <summary>
            /// Default (primary key, if any).
            /// </summary>
            Default = (byte)'d',

            /// <summary>
            /// Nothing.
            /// </summary>
            Nothing = (byte)'n',

            /// <summary>
            /// All columns.
            /// </summary>
            AllColumns = (byte)'f',

            /// <summary>
            /// Index with <c>indisreplident</c> set (same as nothing if the index used has been dropped)
            /// </summary>
            IndexWithIndIsReplIdent = (byte)'i'
        }
    }
}
