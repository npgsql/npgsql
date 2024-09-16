namespace Npgsql.Replication.PgOutput;

/// <summary>
/// The Logical Streaming Replication Protocol version.
/// </summary>
public enum PgOutputProtocolVersion : ulong
{
    /// <summary>
    /// Version 1 is supported for server version 10 and above.
    /// </summary>
    V1 = 1UL,

    /// <summary>
    /// Version 2 is supported only for server version 14 and above, and it allows
    /// streaming of large in-progress transactions.
    /// </summary>
    V2 = 2UL,

    /// <summary>
    /// Version 3 is supported only for server version 15 and above, and it allows
    /// streaming of two-phase commits.
    /// </summary>
    V3 = 3UL,

    /// <summary>
    /// Version 4 is supported only for server version 16 and above, and it allows
    /// streams of large in-progress transactions to be applied in parallel.
    /// </summary>
    V4 = 4UL
}
