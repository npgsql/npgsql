using NpgsqlTypes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol insert message
/// </summary>
public sealed class InsertMessage : TransactionalMessage
{
    readonly ReplicationTuple _tupleEnumerable;

    /// <summary>
    /// The relation for this <see cref="InsertMessage" />.
    /// </summary>
    public RelationMessage Relation { get; private set; } = null!;

    /// <summary>
    /// Columns representing the new row.
    /// </summary>
    public ReplicationTuple NewRow => _tupleEnumerable;

    internal InsertMessage(NpgsqlConnector connector)
        => _tupleEnumerable = new(connector);

    internal InsertMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid,
        RelationMessage relation, ushort numColumns)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid);

        Relation = relation;
        _tupleEnumerable.Reset(numColumns, relation.RowDescription);

        return this;
    }

    internal Task Consume(CancellationToken cancellationToken)
        => _tupleEnumerable.Consume(cancellationToken);
}
