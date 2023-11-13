using NpgsqlTypes;
using System;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Abstract base class for Logical Replication Protocol delete message types.
/// </summary>
public abstract class DeleteMessage : TransactionalMessage
{
    /// <summary>
    /// The relation for this <see cref="InsertMessage" />.
    /// </summary>
    public RelationMessage Relation { get; private set; } = null!;

    private protected DeleteMessage() {}

    private protected DeleteMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid,
        RelationMessage relation)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);

        Relation = relation;

        return this;
    }
}
