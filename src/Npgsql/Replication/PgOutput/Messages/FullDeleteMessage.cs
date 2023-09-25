using NpgsqlTypes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol delete message for tables with REPLICA IDENTITY REPLICA IDENTITY set to FULL.
/// </summary>
public sealed class FullDeleteMessage : DeleteMessage
{
    readonly ReplicationTuple _tupleEnumerable;

    /// <summary>
    /// Columns representing the deleted row.
    /// </summary>
    public ReplicationTuple OldRow => _tupleEnumerable;

    internal FullDeleteMessage(NpgsqlConnector connector)
        => _tupleEnumerable = new(connector);

    internal FullDeleteMessage Populate(
        NpgsqlLogSequenceNumber walStart, NpgsqlLogSequenceNumber walEnd, DateTime serverClock, uint? transactionXid,
        RelationMessage relation, ushort numColumns)
    {
        base.Populate(walStart, walEnd, serverClock, transactionXid, relation);

        _tupleEnumerable.Reset(numColumns, relation.RowDescription);

        return this;
    }

    internal Task Consume(CancellationToken cancellationToken)
        => _tupleEnumerable.Consume(cancellationToken);
}