using NpgsqlTypes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput.Messages;

/// <summary>
/// Logical Replication Protocol delete message for tables with REPLICA IDENTITY set to DEFAULT or USING INDEX.
/// </summary>
public sealed class KeyDeleteMessage : DeleteMessage
{
    readonly ReplicationTuple _tupleEnumerable;

    /// <summary>
    /// Columns representing the key.
    /// </summary>
    public ReplicationTuple Key => _tupleEnumerable;

    internal KeyDeleteMessage(NpgsqlConnector connector)
        => _tupleEnumerable = new(connector);

    internal KeyDeleteMessage Populate(
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