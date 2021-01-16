namespace Npgsql.Replication
{
    /// <summary>
    /// Decides what to do with the snapshot created during logical slot initialization.
    /// </summary>
    public enum LogicalSlotSnapshotInitMode
    {
        /// <summary>
        /// Export the snapshot for use in other sessions. This is the default.
        /// This option can't be used inside a transaction.
        /// </summary>
        Export = 0,

        /// <summary>
        /// Use the snapshot for the current transaction executing the command.
        /// This option must be used in a transaction, and CREATE_REPLICATION_SLOT must be the first command run
        /// in that transaction.
        /// </summary>
        Use = 1,

        /// <summary>
        /// Just use the snapshot for logical decoding as normal but don't do anything else with it.
        /// </summary>
        NoExport = 2
    }
}
