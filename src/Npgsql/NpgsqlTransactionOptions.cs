using System;

namespace Npgsql;

#pragma warning disable RS0016

/// <summary>
/// Specifies additional, Npgsql-specific options to apply when beginning a transaction, avoiding the need for an additional
/// roundtrip to the server (e.g. via <c>SET TRANSACTION</c>).
/// </summary>
[Flags]
public enum NpgsqlTransactionOptions
{
    /// <summary>
    /// No additional options are set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The transaction is read-only; no data modifications can be made. Corresponds to <c>READ ONLY</c> in <c>BEGIN</c>.
    /// </summary>
    ReadOnly = 1,

    /// <summary>
    /// The transaction can be deferred. This only has an effect when the transaction is both <see cref="ReadOnly"/> and
    /// <see cref="System.Data.IsolationLevel.Serializable"/>, in which case it allows the database to wait for a point in time where
    /// no conflicts can occur before starting the transaction, avoiding the overhead associated with serializable transactions.
    /// Corresponds to <c>DEFERRABLE</c> in <c>BEGIN</c>.
    /// </summary>
    Deferrable = 2,
}
