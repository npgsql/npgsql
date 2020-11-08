namespace Npgsql.Replication.PgOutput.Messages
{
    /// <summary>
    /// Represents a column in a Logical Replication Protocol relation message
    /// </summary>
    public readonly struct RelationMessageColumn
    {
        internal RelationMessageColumn(byte flags, string columnName, uint dataTypeId, int typeModifier)
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
