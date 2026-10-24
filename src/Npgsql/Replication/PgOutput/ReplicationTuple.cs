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
    readonly NpgsqlConnector _connector;
    private protected NpgsqlReadBuffer ReadBuffer = null!;
    readonly TupleEnumerator _tupleEnumerator;

    internal RowState State;

    /// <summary>
    /// The number of columns in the tuple.
    /// </summary>
    public ushort NumColumns { get; private set; }

    RowDescriptionMessage _rowDescription = null!;

    internal ReplicationTuple(NpgsqlConnector connector)
        => (_connector, _tupleEnumerator) = (connector, new(this));

    internal void Reset(ushort numColumns, RowDescriptionMessage rowDescription)
    {
        // The connector switches to a bigger buffer after an oversize message (e.g. a big notice). We take the buffer of the
        // current message here and pass it on, since the connector no longer has one once it's broken.
        ReadBuffer = _connector.ReadBuffer;
        State = RowState.NotRead;
        (NumColumns, _rowDescription) = (numColumns, rowDescription);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerator<ReplicationValue> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        switch (State)
        {
        case RowState.NotRead:
            _tupleEnumerator.Reset(NumColumns, _rowDescription, ReadBuffer, cancellationToken);
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
            _tupleEnumerator.Reset(NumColumns, _rowDescription, ReadBuffer, cancellationToken);
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
