namespace Npgsql.Replication.PgOutput;

/// <summary>
/// Option to enable streaming of in-progress transactions.
/// Minimum protocol version 2 is required to turn it on. Minimum protocol version 4 is required for the "parallel" option.
/// </summary>
public enum PgOutputStreamingMode
{
    /// <summary>
    /// Disable streaming of in-progress transactions
    /// </summary>
    Off,

    /// <summary>
    /// Enable streaming of in-progress transactions
    /// </summary>
    On,

    /// <summary>
    /// Enable streaming of in-progress transactions and enable sending extra information with some messages to be used for parallelisation
    /// </summary>
    Parallel
}
