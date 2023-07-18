using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.Converters;
using Npgsql.PostgresTypes;
using Npgsql.Schema;
using Npgsql.Util;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

#pragma warning disable CA2222 // Do not decrease inherited member visibility
namespace Npgsql;

/// <summary>
/// Reads a forward-only stream of rows from a data source.
/// </summary>
#pragma warning disable CA1010
public sealed class NpgsqlDataReader : DbDataReader, IDbColumnSchemaGenerator
#pragma warning restore CA1010
{
    internal NpgsqlCommand Command { get; private set; } = default!;
    internal NpgsqlConnector Connector { get; }
    NpgsqlConnection? _connection;

    /// <summary>
    /// The behavior of the command with which this reader was executed.
    /// </summary>
    CommandBehavior _behavior;

    /// <summary>
    /// In multiplexing, this is <see langword="null" /> as the sending is managed in the write multiplexing loop,
    /// and does not need to be awaited by the reader.
    /// </summary>
    Task? _sendTask;

    internal ReaderState State = ReaderState.Disposed;

    internal NpgsqlReadBuffer Buffer = default!;
    PgReader PgReader => Buffer.PgReader;

    /// <summary>
    /// Holds the list of statements being executed by this reader.
    /// </summary>
    List<NpgsqlBatchCommand> _statements = default!;

    /// <summary>
    /// The index of the current query resultset we're processing (within a multiquery)
    /// </summary>
    internal int StatementIndex { get; private set; }

    /// <summary>
    /// The number of columns in the current row
    /// </summary>
    int _numColumns;

    /// <summary>
    /// Records, for each column, its starting offset and length in the current row.
    /// Used only in non-sequential mode.
    /// </summary>
    readonly List<(int Offset, int Length)> _columns = new();

    /// <summary>
    /// The index of the column that we're on, i.e. that has already been parsed, is
    /// is memory and can be retrieved. Initialized to -1, which means we're on the column
    /// count (which comes before the first column).
    /// </summary>
    int _column;

    /// <summary>
    /// The position in the buffer at which the current data row message ends.
    /// Used only when the row is consumed non-sequentially.
    /// </summary>
    int _dataMsgEnd;

    /// <summary>
    /// Determines, if we can consume the row non-sequentially.
    /// Mostly useful for a sequential mode, when the row is already in the buffer.
    /// Should always be true for the non-sequential mode.
    /// </summary>
    bool _canConsumeRowNonSequentially;

    /// <summary>
    /// The RowDescription message for the current resultset being processed
    /// </summary>
    internal RowDescriptionMessage? RowDescription;

    ulong? _recordsAffected;

    /// <summary>
    /// Whether the current result set has rows
    /// </summary>
    bool _hasRows;

    /// <summary>
    /// Is raised whenever Close() is called.
    /// </summary>
    public event EventHandler? ReaderClosed;

    bool _isSchemaOnly;
    bool _isSequential;

    /// <summary>
    /// Used to keep track of every unique row this reader object ever traverses.
    /// This is used to detect whether nested DbDataReaders are still valid.
    /// </summary>
    internal ulong UniqueRowId;

    internal NpgsqlNestedDataReader? CachedFreeNestedDataReader;

    long _startTimestamp;
    readonly ILogger _commandLogger;

    internal NpgsqlDataReader(NpgsqlConnector connector)
    {
        Connector = connector;
        _commandLogger = connector.CommandLogger;
    }

    internal void Init(
        NpgsqlCommand command,
        CommandBehavior behavior,
        List<NpgsqlBatchCommand> statements,
        long startTimestamp = 0,
        Task? sendTask = null)
    {
        Command = command;
        _connection = command.InternalConnection;
        _behavior = behavior;
        _isSchemaOnly = _behavior.HasFlag(CommandBehavior.SchemaOnly);
        _isSequential = _behavior.HasFlag(CommandBehavior.SequentialAccess);
        _statements = statements;
        StatementIndex = -1;
        _sendTask = sendTask;
        State = ReaderState.BetweenResults;
        _recordsAffected = null;
        _startTimestamp = startTimestamp;
    }

    #region Read

    /// <summary>
    /// Advances the reader to the next record in a result set.
    /// </summary>
    /// <returns><b>true</b> if there are more rows; otherwise <b>false</b>.</returns>
    /// <remarks>
    /// The default position of a data reader is before the first record. Therefore, you must call Read to begin accessing data.
    /// </remarks>
    public override bool Read()
    {
        CheckClosedOrDisposed();

        UniqueRowId++;
        var fastRead = TryFastRead();
        return fastRead.HasValue
            ? fastRead.Value
            : Read(false).GetAwaiter().GetResult();
    }

    /// <summary>
    /// This is the asynchronous version of <see cref="Read()"/>
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        CheckClosedOrDisposed();

        UniqueRowId++;
        var fastRead = TryFastRead();
        if (fastRead.HasValue)
            return fastRead.Value ? PGUtil.TrueTask : PGUtil.FalseTask;

        using (NoSynchronizationContextScope.Enter())
            return Read(true, cancellationToken);
    }

    bool? TryFastRead()
    {
        // This is an optimized execution path that avoids calling any async methods for the (usual)
        // case where the next row (or CommandComplete) is already in memory.

        if (_behavior.HasFlag(CommandBehavior.SingleRow))
            return null;

        switch (State)
        {
        case ReaderState.BeforeResult:
            // First Read() after NextResult. Data row has already been processed.
            State = ReaderState.InResult;
            return true;
        case ReaderState.InResult:
            if (!_canConsumeRowNonSequentially)
                return null;
            // We get here, if we're in a non-sequential mode (or the row is already in the buffer)
            ConsumeRowNonSequential(userOp: true);
            break;
        case ReaderState.BetweenResults:
        case ReaderState.Consumed:
        case ReaderState.Closed:
        case ReaderState.Disposed:
            return false;
        }

        var readBuf = Connector.ReadBuffer;
        if (readBuf.ReadBytesLeft < 5)
            return null;
        var messageCode = (BackendMessageCode)readBuf.ReadByte();
        var len = readBuf.ReadInt32() - 4;  // Transmitted length includes itself
        if (messageCode != BackendMessageCode.DataRow || readBuf.ReadBytesLeft < len)
        {
            readBuf.ReadPosition -= 5;
            return null;
        }

        var msg = Connector.ParseServerMessage(readBuf, BackendMessageCode.DataRow, len, false)!;
        Debug.Assert(msg.Code == BackendMessageCode.DataRow);
        ProcessMessage(msg);
        return true;
    }

    async Task<bool> Read(bool async, CancellationToken cancellationToken = default)
    {
        using var registration = Connector.StartNestedCancellableOperation(cancellationToken);
        try
        {
            switch (State)
            {
            case ReaderState.BeforeResult:
                // First Read() after NextResult. Data row has already been processed.
                State = ReaderState.InResult;
                return true;

            case ReaderState.InResult:
                await ConsumeRow(async, userOp: true);
                if (_behavior.HasFlag(CommandBehavior.SingleRow))
                {
                    // TODO: See optimization proposal in #410
                    await Consume(async);
                    return false;
                }
                break;

            case ReaderState.BetweenResults:
            case ReaderState.Consumed:
            case ReaderState.Closed:
            case ReaderState.Disposed:
                return false;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException();
                return false;
            }

            var msg = await ReadMessage(async);

            switch (msg.Code)
            {
            case BackendMessageCode.DataRow:
                ProcessMessage(msg);
                return true;

            case BackendMessageCode.CommandComplete:
            case BackendMessageCode.EmptyQueryResponse:
                ProcessMessage(msg);
                if (_statements[StatementIndex].AppendErrorBarrier ?? Command.EnableErrorBarriers)
                    Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);
                return false;

            default:
                throw Connector.UnexpectedMessageReceived(msg.Code);
            }
        }
        catch
        {
            // Break may have progressed the reader already.
            if (State is not ReaderState.Closed)
                State = ReaderState.Consumed;
            throw;
        }
    }

    ValueTask<IBackendMessage> ReadMessage(bool async)
    {
        return _isSequential ? ReadMessageSequential(Connector, async) : Connector.ReadMessage(async);

        static async ValueTask<IBackendMessage> ReadMessageSequential(NpgsqlConnector connector, bool async)
        {
            var msg = await connector.ReadMessage(async, DataRowLoadingMode.Sequential);
            if (msg.Code == BackendMessageCode.DataRow)
            {
                // Make sure that the datarow's column count is already buffered
                await connector.ReadBuffer.Ensure(2, async);
                return msg;
            }
            return msg;
        }
    }

    #endregion

    #region NextResult

    /// <summary>
    /// Advances the reader to the next result when reading the results of a batch of statements.
    /// </summary>
    /// <returns></returns>
    public override bool NextResult() => (_isSchemaOnly ? NextResultSchemaOnly(false) : NextResult(false))
        .GetAwaiter().GetResult();

    /// <summary>
    /// This is the asynchronous version of NextResult.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
        using var _ = NoSynchronizationContextScope.Enter();

        return _isSchemaOnly
            ? NextResultSchemaOnly(async: true, cancellationToken: cancellationToken)
            : NextResult(async: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Internal implementation of NextResult
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    async Task<bool> NextResult(bool async, bool isConsuming = false, CancellationToken cancellationToken = default)
    {
        CheckClosedOrDisposed();

        IBackendMessage msg;
        Debug.Assert(!_isSchemaOnly);

        using var registration = isConsuming ? default : Connector.StartNestedCancellableOperation(cancellationToken);

        try
        {
            // If we're in the middle of a resultset, consume it
            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
                await ConsumeRow(async, userOp: !isConsuming);
                while (true)
                {
                    var completedMsg = await Connector.ReadMessage(async, DataRowLoadingMode.Skip);
                    switch (completedMsg.Code)
                    {
                    case BackendMessageCode.CommandComplete:
                    case BackendMessageCode.EmptyQueryResponse:
                        ProcessMessage(completedMsg);

                        if (_statements[StatementIndex].AppendErrorBarrier ?? Command.EnableErrorBarriers)
                            Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);

                        break;

                    default:
                        continue;
                    }

                    break;
                }

                break;

            case ReaderState.BetweenResults:
                break;

            case ReaderState.Consumed:
            case ReaderState.Closed:
            case ReaderState.Disposed:
                return false;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException();
                return false;
            }

            Debug.Assert(State == ReaderState.BetweenResults);
            _hasRows = false;

            if (_behavior.HasFlag(CommandBehavior.SingleResult) && StatementIndex == 0 && !isConsuming)
            {
                await Consume(async);
                return false;
            }

            // We are now at the end of the previous result set. Read up to the next result set, if any.
            // Non-prepared statements receive ParseComplete, BindComplete, DescriptionRow/NoData,
            // prepared statements receive only BindComplete
            for (StatementIndex++; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];

                if (statement.TryGetPrepared(out var preparedStatement))
                {
                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    RowDescription = preparedStatement.Description;
                }
                else // Non-prepared/preparing flow
                {
                    preparedStatement = statement.PreparedStatement;
                    if (preparedStatement != null)
                    {
                        Debug.Assert(!preparedStatement.IsPrepared);
                        if (preparedStatement.StatementBeingReplaced != null)
                        {
                            Expect<CloseCompletedMessage>(await Connector.ReadMessage(async), Connector);
                            preparedStatement.StatementBeingReplaced.CompleteUnprepare();
                            preparedStatement.StatementBeingReplaced = null;
                        }
                    }

                    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async), Connector);

                    if (statement.IsPreparing)
                    {
                        preparedStatement!.State = PreparedState.Prepared;
                        Connector.PreparedStatementManager.NumPrepared++;
                        statement.IsPreparing = false;
                    }

                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async), Connector);
                    msg = await Connector.ReadMessage(async);

                    RowDescription = statement.Description = msg.Code switch
                    {
                        BackendMessageCode.NoData => null,

                        // RowDescription messages are cached on the connector, but if we're auto-preparing, we need to
                        // clone our own copy which will last beyond the lifetime of this invocation.
                        BackendMessageCode.RowDescription => preparedStatement == null
                            ? (RowDescriptionMessage)msg
                            : ((RowDescriptionMessage)msg).Clone(),

                        _ => throw Connector.UnexpectedMessageReceived(msg.Code)
                    };
                }

                if (RowDescription == null)
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next statement
                    // No need to read sequentially as it's not a DataRow
                    msg = await Connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CommandComplete:
                    case BackendMessageCode.EmptyQueryResponse:
                        break;
                    case BackendMessageCode.CopyInResponse:
                        throw Connector.Break(new NotSupportedException(
                            "COPY isn't supported in regular command execution - see https://www.npgsql.org/doc/copy.html for documentation on COPY with Npgsql. " +
                            "If you are trying to execute a SQL script created by pg_dump, pass the '--inserts' switch to disable generating COPY statements."));
                    case BackendMessageCode.CopyOutResponse:
                        throw Connector.Break(new NotSupportedException(
                            "COPY isn't supported in regular command execution - see https://www.npgsql.org/doc/copy.html for documentation on COPY with Npgsql."));
                    default:
                        throw Connector.UnexpectedMessageReceived(msg.Code);
                    }

                    ProcessMessage(msg);

                    if (statement.AppendErrorBarrier ?? Command.EnableErrorBarriers)
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);

                    continue;
                }

                if (!Command.IsWrappedByBatch && StatementIndex == 0 && Command.Parameters.HasOutputParameters)
                {
                    // If output parameters are present and this is the first row of the first resultset,
                    // we must always read it in non-sequential mode because it will be traversed twice (once
                    // here for the parameters, then as a regular row).
                    msg = await Connector.ReadMessage(async);
                    ProcessMessage(msg);
                    if (msg.Code == BackendMessageCode.DataRow)
                        PopulateOutputParameters();
                }
                else
                {
                    msg = await ReadMessage(async);
                    ProcessMessage(msg);
                }

                switch (msg.Code)
                {
                case BackendMessageCode.DataRow:
                    Connector.State = ConnectorState.Fetching;
                    return true;
                case BackendMessageCode.CommandComplete:
                    if (statement.AppendErrorBarrier ?? Command.EnableErrorBarriers)
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);
                    return true;
                default:
                    throw Connector.UnexpectedMessageReceived(msg.Code);
                }
            }

            // There are no more queries, we're done. Read the RFQ.
            if (_statements.Count == 0 || !(_statements[_statements.Count - 1].AppendErrorBarrier ?? Command.EnableErrorBarriers))
                Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);

            State = ReaderState.Consumed;
            RowDescription = null;
            return false;
        }
        catch (Exception e)
        {
            if (e is PostgresException postgresException && StatementIndex >= 0 && StatementIndex < _statements.Count)
            {
                var statement = _statements[StatementIndex];

                // Reference the triggering statement from the exception
                postgresException.BatchCommand = statement;

                // Prevent the command or batch from being recycled (by the connection) when it's disposed. This is important since
                // the exception is very likely to escape the using statement of the command, and by that time some other user may
                // already be using the recycled instance.
                Command.IsCacheable = false;

                // If the schema of a table changes after a statement is prepared on that table, PostgreSQL errors with
                // 0A000: cached plan must not change result type. 0A000 seems like a non-specific code, but it's very unlikely the
                // statement would successfully execute anyway, so invalidate the prepared statement.
                if (postgresException.SqlState == PostgresErrorCodes.FeatureNotSupported &&
                    statement.PreparedStatement is { } preparedStatement)
                {
                    preparedStatement.State = PreparedState.Invalidated;
                    Command.ResetPreparation();
                    foreach (var s in Command.InternalBatchCommands)
                        s.ResetPreparation();
                }
            }

            // For the statement that errored, if it was being prepared we need to update our bookkeeping to put them back in unprepared
            // state.
            for (; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];
                if (statement.IsPreparing)
                {
                    statement.IsPreparing = false;
                    statement.PreparedStatement!.AbortPrepare();
                }

                // In normal, non-isolated batching, we've consumed the result set and are done.
                // However, if the command has error barrier, we now have to consume results from the commands after it (unless it's the
                // last one).
                // Note that Consume calls NextResult (this method) recursively, the isConsuming flag tells us we're in this mode.
                if ((statement.AppendErrorBarrier ?? Command.EnableErrorBarriers) && StatementIndex < _statements.Count - 1)
                {
                    if (isConsuming)
                        throw;
                    switch (State)
                    {
                    case ReaderState.Consumed:
                    case ReaderState.Closed:
                    case ReaderState.Disposed:
                        // The exception may have caused the connector to break (e.g. I/O), and so the reader is already closed.
                        break;
                    default:
                        // We provide Consume with the first exception which we've just caught.
                        // If it encounters other exceptions while consuming the rest of the result set, it will raise an AggregateException,
                        // otherwise it will rethrow this first exception.
                        await Consume(async, firstException: e);
                        break; // Never reached, Consume always throws above
                    }
                }
            }

            // Break may have progressed the reader already.
            if (State is not ReaderState.Closed)
                State = ReaderState.Consumed;
            throw;
        }
    }

    void PopulateOutputParameters()
    {
        // The first row in a stored procedure command that has output parameters needs to be traversed twice -
        // once for populating the output parameters and once for the actual result set traversal. So in this
        // case we can't be sequential.
        Debug.Assert(Command.Parameters.Any(p => p.IsOutputDirection));
        Debug.Assert(StatementIndex == 0);
        Debug.Assert(RowDescription != null);
        Debug.Assert(State == ReaderState.BeforeResult);

        var currentPosition = Buffer.ReadPosition;

        // Temporarily set our state to InResult to allow us to read the values
        State = ReaderState.InResult;

        var pending = new Queue<object>();
        var taken = new List<NpgsqlParameter>();
        for (var i = 0; i < FieldCount; i++)
        {
            if (Command.Parameters.TryGetValue(GetName(i), out var p) && p.IsOutputDirection)
            {
                p.Value = GetValue(i);
                taken.Add(p);
            }
            else
                pending.Enqueue(GetValue(i));
        }

        // Not sure where this odd behavior comes from: all output parameters which did not get matched by
        // name now get populated with column values which weren't matched. Keeping this for backwards compat,
        // opened #2252 for investigation.
        foreach (var p in Command.Parameters.Where(p => p.IsOutputDirection && !taken.Contains(p)))
        {
            if (pending.Count == 0)
                break;
            p.Value = pending.Dequeue();
        }

        PgReader.Commit(async: false, resuming: false).GetAwaiter().GetResult();
        State = ReaderState.BeforeResult; // Set the state back
        Buffer.ReadPosition = currentPosition; // Restore position

        _column = -1;
    }

    /// <summary>
    /// Note that in SchemaOnly mode there are no resultsets, and we read nothing from the backend (all
    /// RowDescriptions have already been processed and are available)
    /// </summary>
    async Task<bool> NextResultSchemaOnly(bool async, bool isConsuming = false, CancellationToken cancellationToken = default)
    {
        Debug.Assert(_isSchemaOnly);

        using var registration = isConsuming ? default : Connector.StartNestedCancellableOperation(cancellationToken);

        try
        {
            switch (State)
            {
            case ReaderState.BeforeResult:
            case ReaderState.InResult:
            case ReaderState.BetweenResults:
                break;
            case ReaderState.Consumed:
            case ReaderState.Closed:
            case ReaderState.Disposed:
                return false;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException();
                return false;
            }

            for (StatementIndex++; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];
                if (statement.TryGetPrepared(out var preparedStatement))
                {
                    // Row descriptions have already been populated in the statement objects at the
                    // Prepare phase
                    RowDescription = preparedStatement.Description;
                }
                else
                {
                    var pStatement = statement.PreparedStatement;
                    if (pStatement != null)
                    {
                        Debug.Assert(!pStatement.IsPrepared);
                        if (pStatement.StatementBeingReplaced != null)
                        {
                            Expect<CloseCompletedMessage>(await Connector.ReadMessage(async), Connector);
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }
                    }

                    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async), Connector);

                    if (statement.IsPreparing)
                    {
                        pStatement!.State = PreparedState.Prepared;
                        Connector.PreparedStatementManager.NumPrepared++;
                        statement.IsPreparing = false;
                    }

                    Expect<ParameterDescriptionMessage>(await Connector.ReadMessage(async), Connector);
                    var msg = await Connector.ReadMessage(async);
                    switch (msg.Code)
                    {
                    case BackendMessageCode.NoData:
                        RowDescription = _statements[StatementIndex].Description = null;
                        break;
                    case BackendMessageCode.RowDescription:
                        // We have a resultset
                        RowDescription = _statements[StatementIndex].Description = (RowDescriptionMessage)msg;
                        Command.FixupRowDescription(RowDescription, StatementIndex == 0);
                        break;
                    default:
                        throw Connector.UnexpectedMessageReceived(msg.Code);
                    }

                    if (_statements.Skip(StatementIndex + 1).All(x => x.IsPrepared))
                    {
                        // There are no more queries, we're done. Read to the RFQ.
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async), Connector);
                    }
                }

                // Found a resultset
                if (RowDescription != null)
                    return true;
            }

            RowDescription = null;
            State = ReaderState.Consumed;

            return false;
        }
        catch (Exception e)
        {
            // Break may have progressed the reader already.
            if (State is not ReaderState.Closed)
                State = ReaderState.Consumed;

            // Reference the triggering statement from the exception
            if (e is PostgresException postgresException && StatementIndex >= 0 && StatementIndex < _statements.Count)
            {
                postgresException.BatchCommand = _statements[StatementIndex];

                // Prevent the command or batch from being recycled (by the connection) when it's disposed. This is important since
                // the exception is very likely to escape the using statement of the command, and by that time some other user may
                // already be using the recycled instance.
                Command.IsCacheable = false;
            }

            // An error means all subsequent statements were skipped by PostgreSQL.
            // If any of them were being prepared, we need to update our bookkeeping to put
            // them back in unprepared state.
            for (; StatementIndex < _statements.Count; StatementIndex++)
            {
                var statement = _statements[StatementIndex];
                if (statement.IsPreparing)
                {
                    statement.IsPreparing = false;
                    statement.PreparedStatement!.AbortPrepare();
                }
            }

            throw;
        }
    }

    #endregion

    #region ProcessMessage

    internal void ProcessMessage(IBackendMessage msg)
    {
        switch (msg.Code)
        {
        case BackendMessageCode.DataRow:
            ProcessDataRowMessage((DataRowMessage)msg);
            return;

        case BackendMessageCode.CommandComplete:
            var completed = (CommandCompleteMessage)msg;
            switch (completed.StatementType)
            {
            case StatementType.Update:
            case StatementType.Insert:
            case StatementType.Delete:
            case StatementType.Copy:
            case StatementType.Move:
            case StatementType.Merge:
                if (!_recordsAffected.HasValue)
                    _recordsAffected = 0;
                _recordsAffected += completed.Rows;
                break;
            }

            _statements[StatementIndex].ApplyCommandComplete(completed);
            goto case BackendMessageCode.EmptyQueryResponse;

        case BackendMessageCode.EmptyQueryResponse:
            State = ReaderState.BetweenResults;
            return;

        default:
            ThrowUnexpectedBackendMessage(msg.Code);
            return;
        }

        static void ThrowUnexpectedBackendMessage(BackendMessageCode code)
            => throw new Exception("Received unexpected backend message of type " + code);
    }

    void ProcessDataRowMessage(DataRowMessage msg)
    {
        // The connector's buffer can actually change between DataRows:
        // If a large DataRow exceeding the connector's current read buffer arrives, and we're
        // reading in non-sequential mode, a new oversize buffer is allocated. We thus have to
        // recapture the connector's buffer on each new DataRow.
        // Note that this can happen even in sequential mode, if the row description message is big
        // (see #2003)
        if (!ReferenceEquals(Buffer, Connector.ReadBuffer))
            Buffer = Connector.ReadBuffer;

        _hasRows = true;
        _column = -1;

        // We assume that the row's number of columns is identical to the description's
        _numColumns = Buffer.ReadInt16();
        Debug.Assert(_numColumns == RowDescription!.Count,
            $"Row's number of columns ({_numColumns}) differs from the row description's ({RowDescription.Count})");

        _dataMsgEnd = Buffer.ReadPosition + msg.Length - 2;
        _canConsumeRowNonSequentially = Buffer.ReadBytesLeft >= msg.Length - 2;

        if (!_isSequential)
        {
            Debug.Assert(_canConsumeRowNonSequentially);
            // Initialize our columns array with the offset and length of the first column
            _columns.Clear();
            var len = Buffer.ReadInt32();
            _columns.Add((Buffer.ReadPosition, len));
        }

        switch (State)
        {
        case ReaderState.BetweenResults:
            State = ReaderState.BeforeResult;
            break;
        case ReaderState.BeforeResult:
            State = ReaderState.InResult;
            break;
        case ReaderState.InResult:
            break;
        default:
            throw Connector.UnexpectedMessageReceived(BackendMessageCode.DataRow);
        }
    }

    #endregion

    /// <summary>
    /// Gets a value indicating the depth of nesting for the current row.  Always returns zero.
    /// </summary>
    public override int Depth => 0;

    /// <summary>
    /// Gets a value indicating whether the data reader is closed.
    /// </summary>
    public override bool IsClosed => State == ReaderState.Closed || State == ReaderState.Disposed;

    /// <summary>
    /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
    /// </summary>
    /// <value>
    /// The number of rows changed, inserted, or deleted. -1 for SELECT statements; 0 if no rows were affected or the statement failed.
    /// </value>
    public override int RecordsAffected
        => !_recordsAffected.HasValue
            ? -1
            : _recordsAffected > int.MaxValue
                ? throw new OverflowException(
                    $"The number of records affected exceeds int.MaxValue. Use {nameof(Rows)}.")
                : (int)_recordsAffected;

    /// <summary>
    /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
    /// </summary>
    /// <value>
    /// The number of rows changed, inserted, or deleted. 0 for SELECT statements, if no rows were affected or the statement failed.
    /// </value>
    public ulong Rows => _recordsAffected ?? 0;

    /// <summary>
    /// Returns details about each statement that this reader will or has executed.
    /// </summary>
    /// <remarks>
    /// Note that some fields (i.e. rows and oid) are only populated as the reader
    /// traverses the result.
    ///
    /// For commands with multiple queries, this exposes the number of rows affected on
    /// a statement-by-statement basis, unlike <see cref="NpgsqlDataReader.RecordsAffected"/>
    /// which exposes an aggregation across all statements.
    /// </remarks>
    [Obsolete("Use the new DbBatch API")]
    public IReadOnlyList<NpgsqlBatchCommand> Statements => _statements.AsReadOnly();

    /// <summary>
    /// Gets a value that indicates whether this DbDataReader contains one or more rows.
    /// </summary>
    public override bool HasRows
        => State switch
        {
            ReaderState.Closed => throw new InvalidOperationException("Invalid attempt to call HasRows when reader is closed."),
            ReaderState.Disposed => throw new ObjectDisposedException(nameof(NpgsqlDataReader)),
            _ => _hasRows
        };

    /// <summary>
    /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
    /// column is possible.
    /// This property is different from <see cref="HasRows"/> in that <see cref="HasRows"/> will
    /// return true even if attempting to read a column will fail, e.g. before <see cref="Read()"/>
    /// has been called
    /// </summary>
    public bool IsOnRow => State == ReaderState.InResult;

    /// <summary>
    /// Gets the name of the column, given the zero-based column ordinal.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The name of the specified column.</returns>
    public override string GetName(int ordinal) => GetField(ordinal).Name;

    /// <summary>
    /// Gets the number of columns in the current row.
    /// </summary>
    public override int FieldCount
    {
        get
        {
            CheckClosedOrDisposed();
            return RowDescription?.Count ?? 0;
        }
    }

    #region Cleanup / Dispose

    /// <summary>
    /// Consumes all result sets for this reader, leaving the connector ready for sending and processing further
    /// queries
    /// </summary>
    async Task Consume(bool async, Exception? firstException = null)
    {
        var exceptions = firstException is null ? null : new List<Exception> { firstException };

        // Skip over the other result sets. Note that this does tally records affected from CommandComplete messages, and properly sets
        // state for auto-prepared statements
        while (true)
        {
            try
            {
                if (!(_isSchemaOnly
                        ? await NextResultSchemaOnly(async, isConsuming: true)
                        : await NextResult(async, isConsuming: true)))
                {
                    break;
                }
            }
            catch (Exception e)
            {
                exceptions ??= new();
                exceptions.Add(e);
            }
        }

        Debug.Assert(exceptions?.Count != 0);

        switch (exceptions?.Count)
        {
        case null:
            return;
        case 1:
            ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            return;
        default:
            throw new NpgsqlException(
                "Multiple exceptions occurred when consuming the result set",
                new AggregateException(exceptions));
        }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="NpgsqlDataReader"/>.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        try
        {
            Close(connectionClosing: false, async: false, isDisposing: true).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // In the case of a PostgresException (or multiple ones, if we have error barriers), the reader's state has already been set
            // to Disposed in Close above; in multiplexing, we also unbind the connector (with its reader), and at that point it can be used
            // by other consumers. Therefore, we only set the state fo Disposed if the exception *wasn't* a PostgresException.
            if (!(ex is PostgresException ||
                  ex is NpgsqlException { InnerException: AggregateException aggregateException } &&
                  aggregateException.InnerExceptions.All(e => e is PostgresException)))
            {
                State = ReaderState.Disposed;
            }

            throw;
        }
        finally
        {
            Command.TraceCommandStop();
        }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="NpgsqlDataReader"/>.
    /// </summary>
#if NETSTANDARD2_0
    public ValueTask DisposeAsync()
#else
    public override ValueTask DisposeAsync()
#endif
    {
        using (NoSynchronizationContextScope.Enter())
            return DisposeAsyncCore();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async ValueTask DisposeAsyncCore()
        {
            try
            {
                await Close(connectionClosing: false, async: true, isDisposing: true);
            }
            catch (Exception ex)
            {
                // In the case of a PostgresException (or multiple ones, if we have error barriers), the reader's state has already been set
                // to Disposed in Close above; in multiplexing, we also unbind the connector (with its reader), and at that point it can be used
                // by other consumers. Therefore, we only set the state fo Disposed if the exception *wasn't* a PostgresException.
                if (!(ex is PostgresException ||
                      ex is NpgsqlException { InnerException: AggregateException aggregateException } &&
                      aggregateException.InnerExceptions.All(e => e is PostgresException)))
                {
                    State = ReaderState.Disposed;
                }

                throw;
            }
            finally
            {
                Command.TraceCommandStop();
            }
        }
    }

    /// <summary>
    /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
    /// </summary>
    public override void Close() => Close(connectionClosing: false, async: false, isDisposing: false).GetAwaiter().GetResult();

    /// <summary>
    /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
    /// </summary>
#if NETSTANDARD2_0
    public Task CloseAsync()
#else
    public override Task CloseAsync()
#endif
    {
        using (NoSynchronizationContextScope.Enter())
            return Close(connectionClosing: false, async: true, isDisposing: false);
    }

    internal async Task Close(bool connectionClosing, bool async, bool isDisposing)
    {
        if (State is ReaderState.Closed or ReaderState.Disposed)
        {
            if (isDisposing)
                State = ReaderState.Disposed;
            return;
        }

        // Whenever a connector is broken, it also closes the current reader.
        Connector.CurrentReader = null;

        switch (Connector.State)
        {
        case ConnectorState.Ready:
        case ConnectorState.Fetching:
        case ConnectorState.Executing:
        case ConnectorState.Connecting:
            if (State != ReaderState.Consumed)
            {
                try
                {
                    await Consume(async);
                }
                catch (Exception ex) when (ex is OperationCanceledException or NpgsqlException { InnerException: TimeoutException })
                {
                    // Timeout/cancellation - completely normal, consume has basically completed.
                }
                catch (Exception ex) when (
                    ex is PostgresException ||
                    ex is NpgsqlException { InnerException: AggregateException aggregateException } &&
                    aggregateException.InnerExceptions.All(e => e is PostgresException))
                {
                    // In the case of a PostgresException (or multiple ones, if we have error barriers), the connection is fine and consume
                    // has basically completed. Defer throwing the exception until Cleanup is complete.
                    await Cleanup(async, connectionClosing, isDisposing);
                    throw;
                }
                catch
                {
                    Debug.Assert(Connector.IsBroken);
                    throw;
                }
            }
            break;
        case ConnectorState.Closed:
        case ConnectorState.Broken:
            break;
        case ConnectorState.Waiting:
        case ConnectorState.Copy:
        case ConnectorState.Replication:
            Debug.Fail("Bad connector state when closing reader: " + Connector.State);
            break;
        default:
            throw new ArgumentOutOfRangeException();
        }

        await Cleanup(async, connectionClosing, isDisposing);
    }

    internal async Task Cleanup(bool async, bool connectionClosing = false, bool isDisposing = false)
    {
        LogMessages.ReaderCleanup(_commandLogger, Connector.Id);

        // If multiplexing isn't on, _sendTask contains the task for the writing of this command.
        // Make sure that this task, which may have executed asynchronously and in parallel with the reading,
        // has completed, throwing any exceptions it generated. If we don't do this, there's the possibility of a race condition where the
        // user executes a new command after reader.Dispose() returns, but some additional write stuff is still finishing up from the last
        // command.
        if (_sendTask != null)
        {
            // If the connector is broken, we have no reason to wait for the sendTask to complete
            // as we're not going to send anything else over it
            // and that can lead to deadlocks (concurrent write and read failure, see #4804)
            if (Connector.IsBroken)
            {
                // Prevent unobserved Task notifications by observing the failed Task exception.
                _ = _sendTask.ContinueWith(t => _ = t.Exception, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted, TaskScheduler.Current);
            }
            else
            {
                try
                {
                    if (async)
                        await _sendTask;
                    else
                        _sendTask.GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    // TODO: think of a better way to handle exceptions, see #1323 and #3163
                    _commandLogger.LogDebug(e, "Exception caught while sending the request", Connector.Id);
                }
            }
        }

        State = ReaderState.Closed;
        Command.State = CommandState.Idle;
        Connector.CurrentReader = null;
        if (_commandLogger.IsEnabled(LogLevel.Information))
            Command.LogExecutingCompleted(Connector, executing: false);
        NpgsqlEventSource.Log.CommandStop();
        Connector.DataSource.MetricsReporter.ReportCommandStop(_startTimestamp);
        Connector.EndUserAction();

        // The reader shouldn't be unbound, if we're disposing - so the state is set prematurely
        if (isDisposing)
            State = ReaderState.Disposed;

        if (_connection?.ConnectorBindingScope == ConnectorBindingScope.Reader)
        {
            UnbindIfNecessary();

            // TODO: Refactor... Use proper scope
            _connection.Connector = null;
            Connector.Connection = null;
            _connection.ConnectorBindingScope = ConnectorBindingScope.None;

            // If the reader is being closed as part of the connection closing, we don't apply
            // the reader's CommandBehavior.CloseConnection
            if (_behavior.HasFlag(CommandBehavior.CloseConnection) && !connectionClosing)
                _connection.Close();

            Connector.ReaderCompleted.SetResult(null);
        }
        else if (_behavior.HasFlag(CommandBehavior.CloseConnection) && !connectionClosing)
        {
            Debug.Assert(_connection is not null);
            _connection.Close();
        }

        if (ReaderClosed != null)
        {
            ReaderClosed(this, EventArgs.Empty);
            ReaderClosed = null;
        }
    }

    #endregion

    #region Simple value getters

    /// <summary>
    /// Gets the value of the specified column as a Boolean.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override bool GetBoolean(int ordinal) => GetFieldValue<bool>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a byte.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override byte GetByte(int ordinal) => GetFieldValue<byte>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a single character.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override char GetChar(int ordinal) => GetFieldValue<char>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 16-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override short GetInt16(int ordinal) => GetFieldValue<short>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 32-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override int GetInt32(int ordinal) => GetFieldValue<int>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 64-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override long GetInt64(int ordinal) => GetFieldValue<long>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override DateTime GetDateTime(int ordinal) => GetFieldValue<DateTime>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="string"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override string GetString(int ordinal) => GetFieldValue<string>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a <see cref="decimal"/> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override decimal GetDecimal(int ordinal) => GetFieldValue<decimal>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a double-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override double GetDouble(int ordinal) => GetFieldValue<double>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a single-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override float GetFloat(int ordinal) => GetFieldValue<float>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a globally-unique identifier (GUID).
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override Guid GetGuid(int ordinal) => GetFieldValue<Guid>(ordinal);

    /// <summary>
    /// Populates an array of objects with the column values of the current row.
    /// </summary>
    /// <param name="values">An array of Object into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
    public override int GetValues(object[] values)
    {
        if (values == null)
            throw new ArgumentNullException(nameof(values));
        CheckResultSet();

        var count = Math.Min(FieldCount, values.Length);
        for (var i = 0; i < count; i++)
            values[i] = GetValue(i);
        return count;
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[int ordinal] => GetValue(ordinal);

    #endregion

    #region Provider-specific simple type getters

    /// <summary>
    /// Gets the value of the specified column as a TimeSpan,
    /// </summary>
    /// <remarks>
    /// PostgreSQL's interval type has has a resolution of 1 microsecond and ranges from
    /// -178000000 to 178000000 years, while .NET's TimeSpan has a resolution of 100 nanoseconds
    /// and ranges from roughly -29247 to 29247 years.
    /// See https://www.postgresql.org/docs/current/static/datatype-datetime.html
    /// </remarks>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public TimeSpan GetTimeSpan(int ordinal) => GetFieldValue<TimeSpan>(ordinal);

    /// <inheritdoc />
    protected override DbDataReader GetDbDataReader(int ordinal) => GetData(ordinal);

    /// <summary>
    /// Returns a nested data reader for the requested column.
    /// The column type must be a record or a to Npgsql known composite type, or an array thereof.
    /// Currently only supported in non-sequential mode.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>A data reader.</returns>
    public new NpgsqlNestedDataReader GetData(int ordinal)
    {
        if (_isSequential)
            throw new NotSupportedException("GetData() not supported in sequential mode.");

        var field = CheckRowAndGetField(ordinal);
        var type = field.PostgresType;
        var isArray = type is PostgresArrayType;
        var elementType = isArray ? ((PostgresArrayType)type).Element : type;
        var compositeType = elementType as PostgresCompositeType;
        if (field.DataFormat is DataFormat.Text || (elementType.InternalName != "record" && compositeType == null))
            throw new InvalidCastException("GetData() not supported for type " + field.TypeDisplayName);

        var columnLength = SeekToColumn(async: false, ordinal, field).GetAwaiter().GetResult();
        if (columnLength == -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(field);

        var reader = CachedFreeNestedDataReader;
        if (reader != null)
        {
            CachedFreeNestedDataReader = null;
            reader.Init(UniqueRowId, compositeType);
        }
        else
        {
            reader = new NpgsqlNestedDataReader(this, null, UniqueRowId, 1, compositeType);
        }
        if (isArray)
            reader.InitArray();
        else
            reader.InitSingleRow();
        return reader;
    }

    #endregion

    #region Special binary getters

    /// <summary>
    /// Reads a stream of bytes from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    /// <returns>The actual number of bytes read.</returns>
    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
    {
        if (dataOffset is < 0 or > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between {0} and {int.MaxValue}");
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            throw new IndexOutOfRangeException($"bufferOffset must be between 0 and {buffer.Length}");
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            throw new IndexOutOfRangeException($"length must be between 0 and {buffer.Length - bufferOffset}");

        // Check whether we can do byte[] reads.
        var info = GetInfo(ordinal, typeof(Stream), out var field);
        Debug.Assert(info.BufferRequirement is { Kind: SizeKind.Exact, Value: 0 });

        var columnLength = SeekToColumn(async: false, ordinal, field, resumableOp: true).GetAwaiter().GetResult();
        if (columnLength == -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(field);
        //
        // if (buffer is null)
        //     return ColumnLen;
        //
        // var dataOffset2 = checked((int)dataOffset);
        // if (_isSequential && dataOffset2 < PgReader.CurrentOffset)
        //     ThrowHelper.ThrowInvalidOperationException("Attempt to read a position in the column which has already been read");
        //
        // PgReader.Seek(async: false, dataOffset2).GetAwaiter().GetResult();
        //
        // var reader = PgReader.Init(new ArraySegment<byte>(buffer, bufferOffset, length), ColumnLen, field.DataFormat);
        // // TODO actually make this work in the byte[] converter.
        // var result = info.AsObject
        //     ? (Stream)info.Converter.ReadAsObject(reader)
        //     : info.GetConverter<Stream>().Read(reader);
        // Debug.Assert(ReferenceEquals(buffer, result));
        // PosInColumn += length;
        return length;
    }

    /// <summary>
    /// Retrieves data as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The returned object.</returns>
    public override Stream GetStream(int ordinal)
        => GetFieldValue<Stream>(ordinal);

    /// <summary>
    /// Retrieves data as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The returned object.</returns>
    public Task<Stream> GetStreamAsync(int ordinal, CancellationToken cancellationToken = default)
        => GetFieldValueAsync<Stream>(ordinal, cancellationToken);

    #endregion

    #region Special text getters

    /// <summary>
    /// Reads a stream of characters from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
    /// <param name="buffer">The buffer into which to copy the data.</param>
    /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
    /// <param name="length">The maximum number of characters to read.</param>
    /// <returns>The actual number of characters read.</returns>
    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
    {
        if (dataOffset is < 0 or > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(dataOffset), dataOffset, $"dataOffset must be between 0 and {int.MaxValue}");
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            throw new IndexOutOfRangeException($"bufferOffset must be between 0 and {buffer.Length}");
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            throw new IndexOutOfRangeException($"length must be between 0 and {buffer.Length - bufferOffset}");

        // Check whether we can do resumable reads.
        var info = GetInfo(ordinal, typeof(GetChars), out var field);
        if (info.Converter is not IResumableRead { Supported: true })
            throw new NotSupportedException("The GetChars method is not supported for this column type");

        Debug.Assert(info.BufferRequirement == Size.Zero);

        var columnLength = SeekToColumn(async: false, ordinal, field, resumableOp: true).GetAwaiter().GetResult();
        if (columnLength == -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(field);

        dataOffset = buffer is null ? 0 : dataOffset;
        PgReader.InitCharsRead(checked((int)dataOffset),
            buffer is not null ? new ArraySegment<char>(buffer, bufferOffset, length) : (ArraySegment<char>?)null,
            out var previousDataOffset);

        if (_isSequential && previousDataOffset > dataOffset)
            ThrowHelper.ThrowInvalidOperationException("Attempt to read a position in the column which has already been read");

        var result = info.AsObject
            ? (GetChars)info.Converter.ReadAsObject(PgReader)
            : info.GetConverter<GetChars>().Read(PgReader);
        PgReader.AdvanceCharsRead(result.Read);
        return result.Read;
    }

    /// <summary>
    /// Retrieves data as a <see cref="TextReader"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The returned object.</returns>
    public override TextReader GetTextReader(int ordinal)
        => GetFieldValue<TextReader>(ordinal);

    /// <summary>
    /// Retrieves data as a <see cref="TextReader"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>The returned object.</returns>
    public Task<TextReader> GetTextReaderAsync(int ordinal, CancellationToken cancellationToken = default)
        => GetFieldValueAsync<TextReader>(ordinal, cancellationToken);

    #endregion

    #region GetFieldValue

    /// <summary>
    /// Asynchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">The type of the value to be returned.</typeparam>
    /// <param name="ordinal">The type of the value to be returned.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns></returns>
    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
    {
        // In non-sequential, we know that the column is already buffered - no I/O will take place
        if (!_isSequential)
            return Task.FromResult(GetFieldValue<T>(ordinal));

        using (NoSynchronizationContextScope.Enter())
            return Core(ordinal, cancellationToken).AsTask();

        async ValueTask<T> Core(int ordinal, CancellationToken cancellationToken)
        {
            await using var registration = Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);
            var isStream = typeof(T) == typeof(Stream);
            var info = GetInfo(ordinal, isStream ? null : typeof(T), out var field);

            var columnLength = await SeekToColumnSequential(async: true, ordinal, field);
            if (columnLength == -1)
                return DbNullValueOrThrow<T>(field);

            if (isStream || typeof(T) == typeof(TextReader))
            {
                PgReader.ThrowIfStreamActive();

                // The only statically mapped converter, it always exists.
                if (isStream)
                    return (T)(object)PgReader.GetStream();
            }

            await PgReader.BufferDataAsync(info.BufferRequirement, cancellationToken);
            return info.AsObject
                ? (T)await info.Converter.ReadAsObjectAsync(PgReader, cancellationToken)
                : await info.GetConverter<T>().ReadAsync(PgReader, cancellationToken);
        }
    }

    /// <summary>
    /// Synchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
    /// <param name="ordinal">The column to be retrieved.</param>
    /// <returns>The column to be retrieved.</returns>
    public override T GetFieldValue<T>(int ordinal)
    {
        var isStream = typeof(T) == typeof(Stream);
        var info = GetInfo(ordinal, isStream ? null : typeof(T), out var field);

        if (isStream || typeof(T) == typeof(TextReader))
            PgReader.ThrowIfStreamActive();

        var columnLength = SeekToColumn(async: false, ordinal, field).GetAwaiter().GetResult();
        if (columnLength == -1)
            return DbNullValueOrThrow<T>(field);

        // The only statically mapped converter, it always exists.
        if (isStream)
            return (T)(object)PgReader.GetStream();

        PgReader.BufferData(info.BufferRequirement);
        return info.AsObject
            ? (T)info.Converter.ReadAsObject(PgReader)
            : info.GetConverter<T>().Read(PgReader);
    }

    #endregion

    #region GetValue

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override object GetValue(int ordinal)
    {
        var info = GetInfo(ordinal, null, out var field);
        var columnLength = SeekToColumn(async: false, ordinal, field).GetAwaiter().GetResult();
        if (columnLength == -1)
            return DBNull.Value;

        PgReader.BufferData(info.BufferRequirement);
        var result = info.Converter.ReadAsObject(PgReader);

        return result;
    }

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="object"/>.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The value of the specified column.</returns>
    public override object this[string name] => GetValue(GetOrdinal(name));

    #endregion

    #region IsDBNull

    /// <summary>
    /// Gets a value that indicates whether the column contains nonexistent or missing values.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
    public override bool IsDBNull(int ordinal)
        => SeekToColumn(async: false, ordinal, CheckRowAndGetField(ordinal), resumableOp: true).GetAwaiter().GetResult() is -1;

    /// <summary>
    /// An asynchronous version of <see cref="IsDBNull(int)"/>, which gets a value that indicates whether the column contains non-existent or missing values.
    /// The <paramref name="cancellationToken"/> parameter is currently ignored.
    /// </summary>
    /// <param name="ordinal">The zero-based column to be retrieved.</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
    {
        if (!_isSequential)
            return IsDBNull(ordinal) ? PGUtil.TrueTask : PGUtil.FalseTask;

        using (NoSynchronizationContextScope.Enter())
            return Core(ordinal, cancellationToken);

        async Task<bool> Core(int ordinal, CancellationToken cancellationToken)
        {
            await using var registration = Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);
            return await SeekToColumn(async: true, ordinal, CheckRowAndGetField(ordinal), resumableOp: true) is -1;
        }
    }

    #endregion

    #region Other public accessors

    /// <summary>
    /// Gets the column ordinal given the name of the column.
    /// </summary>
    /// <param name="name">The name of the column.</param>
    /// <returns>The zero-based column ordinal.</returns>
    public override int GetOrdinal(string name)
    {
        if (string.IsNullOrEmpty(name))
            ThrowHelper.ThrowArgumentException($"{nameof(name)} cannot be empty", nameof(name));
        CheckClosedOrDisposed();
        if (RowDescription is null)
            ThrowHelper.ThrowInvalidOperationException("No resultset is currently being traversed");
        return RowDescription.GetFieldIndex(name);
    }

    /// <summary>
    /// Gets a representation of the PostgreSQL data type for the specified field.
    /// The returned representation can be used to access various information about the field.
    /// </summary>
    /// <param name="ordinal">The zero-based column index.</param>
    public PostgresType GetPostgresType(int ordinal) => GetField(ordinal).PostgresType;

    /// <summary>
    /// Gets the data type information for the specified field.
    /// This is the PostgreSQL type name (e.g. double precision), not the .NET type
    /// (see <see cref="GetFieldType"/> for that).
    /// </summary>
    /// <param name="ordinal">The zero-based column index.</param>
    public override string GetDataTypeName(int ordinal) => GetField(ordinal).TypeDisplayName;

    /// <summary>
    /// Gets the OID for the PostgreSQL type for the specified field, as it appears in the pg_type table.
    /// </summary>
    /// <remarks>
    /// This is a PostgreSQL-internal value that should not be relied upon and should only be used for
    /// debugging purposes.
    /// </remarks>
    /// <param name="ordinal">The zero-based column index.</param>
    public uint GetDataTypeOID(int ordinal) => GetField(ordinal).TypeOID;

    /// <summary>
    /// Gets the data type of the specified column.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The data type of the specified column.</returns>
    public override Type GetFieldType(int ordinal)
        => GetField(ordinal).FieldType;

    /// <summary>
    /// Returns an <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.
    /// </summary>
    /// <returns>An <see cref="IEnumerator"/> that can be used to iterate through the rows in the data reader.</returns>
    public override IEnumerator GetEnumerator()
        => new DbEnumerator(this);

    /// <summary>
    /// Returns schema information for the columns in the current resultset.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<NpgsqlDbColumn> GetColumnSchema()
        => GetColumnSchema(async: false).GetAwaiter().GetResult();

    ReadOnlyCollection<DbColumn> IDbColumnSchemaGenerator.GetColumnSchema()
        => new(GetColumnSchema().Select(c => (DbColumn)c).ToList());

    /// <summary>
    /// Asynchronously returns schema information for the columns in the current resultset.
    /// </summary>
    /// <returns></returns>
#if NET5_0_OR_GREATER
    public new Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
#else
    public Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
#endif
    {
        using (NoSynchronizationContextScope.Enter())
            return GetColumnSchema(async: true, cancellationToken);
    }

    Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchema(bool async, CancellationToken cancellationToken = default)
        => RowDescription == null || RowDescription.Count == 0
            ? Task.FromResult(new List<NpgsqlDbColumn>().AsReadOnly())
            : new DbColumnSchemaGenerator(_connection!, RowDescription, _behavior.HasFlag(CommandBehavior.KeyInfo))
                .GetColumnSchema(async, cancellationToken);

    #endregion

    #region Schema metadata table

    /// <summary>
    /// Returns a System.Data.DataTable that describes the column metadata of the DataReader.
    /// </summary>
    [UnconditionalSuppressMessage(
        "Composite type mapping currently isn't trimming-safe, and warnings are generated at the MapComposite level.", "IL2026")]
    public override DataTable? GetSchemaTable()
        => GetSchemaTable(async: false).GetAwaiter().GetResult();

    /// <summary>
    /// Asynchronously returns a System.Data.DataTable that describes the column metadata of the DataReader.
    /// </summary>
    [UnconditionalSuppressMessage(
        "Composite type mapping currently isn't trimming-safe, and warnings are generated at the MapComposite level.", "IL2026")]
#if NET5_0_OR_GREATER
    public override Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default)
#else
    public Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default)
#endif
    {
        using (NoSynchronizationContextScope.Enter())
            return GetSchemaTable(async: true, cancellationToken);
    }

    [UnconditionalSuppressMessage(
        "Composite type mapping currently isn't trimming-safe, and warnings are generated at the MapComposite level.", "IL2026")]
    async Task<DataTable?> GetSchemaTable(bool async, CancellationToken cancellationToken = default)
    {
        if (FieldCount == 0) // No resultset
            return null;

        var table = new DataTable("SchemaTable");

        // Note: column order is important to match SqlClient's, some ADO.NET users appear
        // to assume ordering (see #1671)
        table.Columns.Add("ColumnName", typeof(string));
        table.Columns.Add("ColumnOrdinal", typeof(int));
        table.Columns.Add("ColumnSize", typeof(int));
        table.Columns.Add("NumericPrecision", typeof(int));
        table.Columns.Add("NumericScale", typeof(int));
        table.Columns.Add("IsUnique", typeof(bool));
        table.Columns.Add("IsKey", typeof(bool));
        table.Columns.Add("BaseServerName", typeof(string));
        table.Columns.Add("BaseCatalogName", typeof(string));
        table.Columns.Add("BaseColumnName", typeof(string));
        table.Columns.Add("BaseSchemaName", typeof(string));
        table.Columns.Add("BaseTableName", typeof(string));
        table.Columns.Add("DataType", typeof(Type));
        table.Columns.Add("AllowDBNull", typeof(bool));
        table.Columns.Add("ProviderType", typeof(int));
        table.Columns.Add("IsAliased", typeof(bool));
        table.Columns.Add("IsExpression", typeof(bool));
        table.Columns.Add("IsIdentity", typeof(bool));
        table.Columns.Add("IsAutoIncrement", typeof(bool));
        table.Columns.Add("IsRowVersion", typeof(bool));
        table.Columns.Add("IsHidden", typeof(bool));
        table.Columns.Add("IsLong", typeof(bool));
        table.Columns.Add("IsReadOnly", typeof(bool));
        table.Columns.Add("ProviderSpecificDataType", typeof(Type));
        table.Columns.Add("DataTypeName", typeof(string));

        foreach (var column in await GetColumnSchema(async, cancellationToken))
        {
            var row = table.NewRow();

            row["ColumnName"] = column.ColumnName;
            row["ColumnOrdinal"] = column.ColumnOrdinal ?? -1;
            row["ColumnSize"] = column.ColumnSize ?? -1;
            row["NumericPrecision"] = column.NumericPrecision ?? 0;
            row["NumericScale"] = column.NumericScale ?? 0;
            row["IsUnique"] = column.IsUnique == true;
            row["IsKey"] = column.IsKey == true;
            row["BaseServerName"] = "";
            row["BaseCatalogName"] = column.BaseCatalogName;
            row["BaseColumnName"] = column.BaseColumnName;
            row["BaseSchemaName"] = column.BaseSchemaName;
            row["BaseTableName"] = column.BaseTableName;
            row["DataType"] = column.DataType;
            row["AllowDBNull"] = (object?)column.AllowDBNull ?? DBNull.Value;
            row["ProviderType"] = column.NpgsqlDbType ?? NpgsqlDbType.Unknown;
            row["IsAliased"] = column.IsAliased == true;
            row["IsExpression"] = column.IsExpression == true;
            row["IsIdentity"] = column.IsIdentity == true;
            row["IsAutoIncrement"] = column.IsAutoIncrement == true;
            row["IsRowVersion"] = false;
            row["IsHidden"] = column.IsHidden == true;
            row["IsLong"] = column.IsLong == true;
            row["DataTypeName"] = column.DataTypeName;

            table.Rows.Add(row);
        }

        return table;
    }

    #endregion Schema metadata table

    #region Seeking

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ValueTask<int> SeekToColumn(bool async, int ordinal, FieldDescription field, bool resumableOp = false)
        => _isSequential
            ? SeekToColumnSequential(async, ordinal, field, resumableOp)
            : new(SeekToColumnNonSequential(ordinal, field, resumableOp));

    int SeekToColumnNonSequential(int ordinal, FieldDescription field, bool resumableOp = false)
    {
        PgReader.Commit(async: false, _column == ordinal && resumableOp).GetAwaiter().GetResult();

        for (var lastColumnRead = _columns.Count; ordinal >= lastColumnRead; lastColumnRead++)
        {
            (Buffer.ReadPosition, var lastColumnLen) = _columns[lastColumnRead - 1];
            if (lastColumnLen != -1)
                Buffer.ReadPosition += lastColumnLen;
            var len = Buffer.ReadInt32();
            _columns.Add((Buffer.ReadPosition, len));
        }

        (Buffer.ReadPosition, var columnLength) = _columns[ordinal];
        PgReader.Init(columnLength, field.DataFormat, resumableOp);
        if (ordinal != _column)
            _column = ordinal;

        return columnLength;
    }

    /// <summary>
    /// Seeks to the given column. The 4-byte length is read and returned/>.
    /// </summary>
    ValueTask<int> SeekToColumnSequential(bool async, int ordinal, FieldDescription field, bool resumableOp = false)
    {
        var resuming = _column == ordinal;
        if (ordinal < _column || (resuming && !resumableOp && PgReader.FieldSize != -1))
            ThrowHelper.ThrowInvalidOperationException(
                $"Invalid attempt to read from column ordinal '{ordinal}'. With CommandBehavior.SequentialAccess, " +
                $"you may only read from column ordinal '{_column}' or greater.");

        var committed = false;
        if (!PgReader.CommitHasIO(resuming))
        {
            var task = PgReader.Commit(async, resuming);
            task.GetAwaiter().GetResult();
            Debug.Assert(task.IsCompleted);
            committed = true;
            if (TrySeekBuffered(ordinal, out var columnLength))
            {
                if (!resuming)
                    PgReader.Init(columnLength, field.DataFormat, resumableOp);
                return new(columnLength);
            }

            // If we couldn't consume the column TrySeekBuffered had to stop at, do so now.
            if (columnLength > -1)
            {
                PgReader.Init(columnLength, field.DataFormat, resumableOp);
                committed = false;
            }
        }

        return Core(async, !committed, ordinal, field.DataFormat, resumableOp);

#if NET6_0_OR_GREATER
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
#endif
        async ValueTask<int> Core(bool async, bool commit, int ordinal, DataFormat dataFormat, bool resumableOp)
        {
            if (commit)
            {
                Debug.Assert(ordinal != _column);
                await PgReader.Commit(async, resuming);
            }

            if (ordinal == _column)
                return PgReader.FieldSize;

            // Seek to the requested column
            var buffer = Buffer;
            for (; _column < ordinal - 1; _column++)
            {
                await buffer.Ensure(4, async);
                var len = buffer.ReadInt32();
                if (len != -1)
                    await buffer.Skip(len, async);
            }

            await buffer.Ensure(4, async);
            var columnLength = buffer.ReadInt32();
            _column = ordinal;

            PgReader.Init(columnLength, dataFormat, resumableOp);
            return columnLength;
        }

        bool TrySeekBuffered(int ordinal, out int columnLength)
        {
            if (ordinal == _column)
            {
                columnLength = PgReader.FieldSize;
                return true;
            }

            // Skip over unwanted fields
            columnLength = -1;
            var buffer = Buffer;
            for (; _column < ordinal - 1; _column++)
            {
                if (buffer.ReadBytesLeft < 4)
                    return false;
                columnLength = buffer.ReadInt32();
                if (columnLength > 0)
                {
                    if (buffer.ReadBytesLeft < columnLength)
                        return false;
                    buffer.Skip(columnLength);
                }
            }

            if (buffer.ReadBytesLeft < 4)
            {
                columnLength = -1;
                return false;
            }

            columnLength = buffer.ReadInt32();
            _column = ordinal;
            return true;
        }
    }

    #endregion

    #region ConsumeRow

    Task ConsumeRow(bool async, bool userOp = false)
    {
        Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);

        UniqueRowId++;

        if (!_canConsumeRowNonSequentially)
            return ConsumeRowSequential(async, userOp);

        // We get here, if we're in a non-sequential mode (or the row is already in the buffer)
        ConsumeRowNonSequential(userOp);
        return Task.CompletedTask;

        async Task ConsumeRowSequential(bool async, bool userOp)
        {
            if (!userOp)
            {
                if (async)
                    await PgReader.ConsumeAsync();
                else
                    PgReader.Consume();
            }

            await PgReader.Commit(async, resuming: false);

            // Skip over the remaining columns in the row
            for (; _column < _numColumns - 1; _column++)
            {
                await Buffer.Ensure(4, async);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.Skip(len, async);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ConsumeRowNonSequential(bool userOp)
    {
        Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);
        if (!userOp)
            PgReader.Consume();
        PgReader.Commit(async: false, resuming: false).GetAwaiter().GetResult();
        Buffer.ReadPosition = _dataMsgEnd;
    }

    #endregion

    #region Checks

    void CheckResultSet()
    {
        switch (State)
        {
        case ReaderState.BeforeResult:
        case ReaderState.InResult:
            return;
        case ReaderState.Closed:
            ThrowHelper.ThrowInvalidOperationException("The reader is closed");
            return;
        case ReaderState.Disposed:
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlDataReader));
            return;
        default:
            ThrowHelper.ThrowInvalidOperationException("No resultset is currently being traversed");
            return;
        }
    }

    static T DbNullValueOrThrow<T>(FieldDescription field)
    {
        // When T is a Nullable<T> (and only in that case), we support returning null
        if (default(T) is null && typeof(T).IsValueType)
            return default!;

        if (typeof(T) == typeof(object))
            return (T)(object)DBNull.Value;

        ThrowHelper.ThrowInvalidCastException_NoValue(field);
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    PgConverterInfo GetInfo(int ordinal, Type? type, out FieldDescription field)
    {
        field = CheckRowAndGetField(ordinal);
        return type is null ? field.ObjectOrDefaultInfo : field.GetConverterInfo(type);
    }

    FieldDescription CheckRowAndGetField(int column)
    {
        switch (State)
        {
        case ReaderState.InResult:
            break;
        case ReaderState.Closed:
            ThrowHelper.ThrowInvalidOperationException("The reader is closed");
            break;
        case ReaderState.Disposed:
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlDataReader));
            break;
        default:
            ThrowHelper.ThrowInvalidOperationException("No row is available");
            break;
        }

        var columns = RowDescription;
        if (column < 0 || column >= columns!.Count)
            ThrowColumnOutOfRange(columns!.Count);

        return columns[column];
    }

    /// <summary>
    /// Checks that we have a RowDescription, but not necessary an actual resultset
    /// (for operations which work in SchemaOnly mode.
    /// </summary>
    FieldDescription GetField(int column)
    {
        if (RowDescription is null)
            ThrowHelper.ThrowInvalidOperationException("No resultset is currently being traversed");

        var columns = RowDescription;
        if (column < 0 || column >= columns.Count)
            ThrowColumnOutOfRange(columns.Count);

        return columns[column];
    }

    void CheckClosedOrDisposed()
    {
        switch (State)
        {
        case ReaderState.Closed:
            ThrowHelper.ThrowInvalidOperationException("The reader is closed");
            return;
        case ReaderState.Disposed:
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlDataReader));
            return;
        }
    }

    static void ThrowColumnOutOfRange(int maxIndex) =>
        throw new IndexOutOfRangeException($"Column must be between {0} and {maxIndex - 1}");

    #endregion

    #region Misc

    /// <summary>
    /// Unbinds reader from the connector.
    /// Should be called before the connector is returned to the pool.
    /// </summary>
    internal void UnbindIfNecessary()
    {
        // We're closing the connection, but reader is not yet disposed
        // We have to unbind the reader from the connector, otherwise there could be a concurrency issues
        // See #3126 and #3290
        if (State != ReaderState.Disposed)
        {
            Connector.DataReader = Connector.UnboundDataReader is { State: ReaderState.Disposed } previousReader
                ? previousReader
                : new NpgsqlDataReader(Connector);
            Connector.UnboundDataReader = this;
        }
    }

    #endregion
}

enum ReaderState
{
    BeforeResult,
    InResult,
    BetweenResults,
    Consumed,
    Closed,
    Disposed,
}
