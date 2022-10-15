using System;

namespace Npgsql;

/// <summary>
/// Error codes set on <see cref="NpgsqlException.Code" /> for exceptions thrown by Npgsql.
/// </summary>
public enum NpgsqlErrorCode
{
    /// <summary>
    ///
    /// </summary>
    Other = 0,

    /// <summary>
    /// An error was reported by PostgreSQL. The inner exception on <see cref="NpgsqlException" /> is a <see cref="PostgresException" />
    /// which provides more information about the precise error (see notably <see cref="PostgresException.SqlState" />.
    /// </summary>
    PostgresError = 1,

    /// <summary>
    /// The connection pool was exhausted, reaching its maximum pool size, and a connection could not be obtained within the allotted
    /// timeout. The inner exception on <see cref="NpgsqlException" /> is a <see cref="TimeoutException" />.
    /// </summary>
    ConnectionPoolExhausted = 2
}
