using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal;

namespace Npgsql.Replication.PgOutput;

/// <summary>
/// Represents a streaming tuple containing <see cref="ReplicationValue"/>.
/// </summary>
public class ReplicationTuple : IAsyncEnumerable<ReplicationValue>
{
    private protected readonly NpgsqlReadBuffer ReadBuffer;
    readonly TupleEnumerator _tupleEnumerator;

    internal RowState State;

    /// <summary>
    /// The number of columns in the tuple.
    /// </summary>
    public ushort NumColumns { get; private set; }

    RowDescriptionMessage _rowDescription = null!;

    internal ReplicationTuple(NpgsqlConnector connector)
        => (ReadBuffer, _tupleEnumerator) = (connector.ReadBuffer, new(this, connector));

    internal void Reset(ushort numColumns, RowDescriptionMessage rowDescription)
    {
        State = RowState.NotRead;
        (NumColumns, _rowDescription) = (numColumns, rowDescription);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerator<ReplicationValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        switch (State)
        {
        case RowState.NotRead:
            _tupleEnumerator.Reset(NumColumns, _rowDescription, cancellationToken);
            State = RowState.Reading;
            return _tupleEnumerator;
        case RowState.Reading:
            throw new InvalidOperationException("The row is already been read.");
        case RowState.Consumed:
            throw new InvalidOperationException("The row has already been consumed.");
        default:
            throw new ArgumentOutOfRangeException();
        }
    }

    internal async Task Consume(CancellationToken cancellationToken)
    {
        switch (State)
        {
        case RowState.NotRead:
            State = RowState.Reading;
            _tupleEnumerator.Reset(NumColumns, _rowDescription, cancellationToken);
            while (await _tupleEnumerator.MoveNextAsync().ConfigureAwait(false)) { }
            break;
        case RowState.Reading:
            while (await _tupleEnumerator.MoveNextAsync().ConfigureAwait(false)) { }
            break;
        case RowState.Consumed:
            return;
        default:
            throw new ArgumentOutOfRangeException();
        }
    }
}

enum RowState
{
    NotRead,
    Reading,
    Consumed
}
