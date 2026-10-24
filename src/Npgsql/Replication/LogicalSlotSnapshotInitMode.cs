namespace Npgsql.Replication;

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
    /// <remarks>
    /// The transaction has to be started with <c>BEGIN READ ONLY ISOLATION LEVEL REPEATABLE READ</c> (READ ONLY is required
    /// since PostgreSQL 16) and can't be a subtransaction. Npgsql currently has no way to start a transaction on a
    /// replication connection, so this option can't be used yet.
    /// </remarks>
    Use = 1,

    /// <summary>
    /// Just use the snapshot for logical decoding as normal but don't do anything else with it.
    /// </summary>
    NoExport = 2
}