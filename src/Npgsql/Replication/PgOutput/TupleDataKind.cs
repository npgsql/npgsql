namespace Npgsql.Replication.PgOutput
{
    /// <summary>
    /// The kind of data transmitted for a tuple in a Logical Replication Protocol message.
    /// </summary>
    public enum TupleDataKind : byte
    {
        /// <summary>
        /// Identifies the data as NULL value.
        /// </summary>
        Null = (byte)'n',

        /// <summary>
        /// Identifies unchanged TOASTed value (the actual value is not sent).
        /// </summary>
        UnchangedToastedValue = (byte)'u',

        /// <summary>
        /// Identifies the data as text formatted value.
        /// </summary>
        TextValue = (byte)'t',

        /// <summary>
        /// Identifies the data as binary value.
        /// </summary>
        /// <remarks>Added in PG14</remarks>
        BinaryValue = (byte)'b'
    }
}
