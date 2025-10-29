using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql;

/// <summary>
/// Reads a forward-only stream of rows from a data source.
/// </summary>
#pragma warning disable CA1010
public sealed class NpgsqlDataReader : DbDataReader, IDbColumnSchemaGenerator
#pragma warning restore CA1010
{
    static readonly Task<bool> TrueTask = Task.FromResult(true);
    static readonly Task<bool> FalseTask = Task.FromResult(false);

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
    /// Records, for each column, its starting offset and length in the current row.
    /// Used only in non-sequential mode.
    /// </summary>
    readonly List<(int Offset, int Length)> _columns = [];
    int _columnsStartPos;

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
    bool _isRowBuffered;

    /// <summary>
    /// The RowDescription message for the current resultset being processed
    /// </summary>
    internal RowDescriptionMessage? RowDescription;

    int ColumnCount => RowDescription!.Count;

    /// <summary>
    /// Stores the last converter info resolved by column, to speed up repeated reading.
    /// </summary>
    ColumnInfo[]? ColumnInfoCache { get; set; }

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
        Debug.Assert(ColumnInfoCache is null);
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
        ThrowIfClosedOrDisposed();
        return TryRead()?.Result ?? Read(false).GetAwaiter().GetResult();
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
        ThrowIfClosedOrDisposed();
        return TryRead() ?? Read(async: true, cancellationToken);
    }

    // This is an optimized execution path that avoids calling any async methods for the (usual)
    // case where the next row (or CommandComplete) is already in memory.
    Task<bool>? TryRead()
    {
        switch (State)
        {
        case ReaderState.BeforeResult:
            // First Read() after NextResult. Data row has already been processed.
            State = ReaderState.InResult;
            return TrueTask;
        case ReaderState.InResult:
            break;
        default:
            return FalseTask;
        }

        // We have a special case path for SingleRow.
        if (_behavior.HasFlag(CommandBehavior.SingleRow) || !_isRowBuffered)
            return null;

        ConsumeBufferedRow();

        const int headerSize = sizeof(byte) + sizeof(int);
        var buffer = Buffer;
        var readPosition = buffer.ReadPosition;
        var bytesLeft = buffer.FilledBytes - readPosition;
        if (bytesLeft < headerSize)
            return null;
        var messageCode = (BackendMessageCode)buffer.ReadByte();
        var len = buffer.ReadInt32() - sizeof(int); // Transmitted length includes itself
        var isDataRow = messageCode is BackendMessageCode.DataRow;
        // sizeof(short) is for the number of columns
        var sufficientBytes = isDataRow && _isSequential ? headerSize + sizeof(short) : headerSize + len;
        if (bytesLeft < sufficientBytes
            || !isDataRow && (_statements[StatementIndex].AppendErrorBarrier ?? Command.EnableErrorBarriers)
            // Could be an error, let main read handle it.
            || Connector.ParseResultSetMessage(buffer, messageCode, len) is not { } msg)
        {
            buffer.ReadPosition = readPosition;
            return null;
        }
        ProcessMessage(msg);
        return isDataRow ? TrueTask : FalseTask;
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
                await ConsumeRow(async).ConfigureAwait(false);
                if (_behavior.HasFlag(CommandBehavior.SingleRow))
                {
                    // TODO: See optimization proposal in #410
                    await Consume(async).ConfigureAwait(false);
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

            var msg = await ReadMessage(async).ConfigureAwait(false);

            switch (msg.Code)
            {
            case BackendMessageCode.DataRow:
                ProcessMessage(msg);
                return true;

            case BackendMessageCode.CommandComplete:
            case BackendMessageCode.EmptyQueryResponse:
                ProcessMessage(msg);
                if (_statements[StatementIndex].AppendErrorBarrier ?? Command.EnableErrorBarriers)
                    Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
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
            var msg = await connector.ReadMessage(async, DataRowLoadingMode.Sequential).ConfigureAwait(false);
            if (msg.Code == BackendMessageCode.DataRow)
            {
                // Make sure that the datarow's column count is already buffered
                await connector.ReadBuffer.Ensure(2, async).ConfigureAwait(false);
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
    public override bool NextResult()
    {
        ThrowIfClosedOrDisposed();
        return (_isSchemaOnly ? NextResultSchemaOnly(false) : NextResult(false))
            .GetAwaiter().GetResult();
    }

    /// <summary>
    /// This is the asynchronous version of NextResult.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override Task<bool> NextResultAsync(CancellationToken cancellationToken)
    {
        ThrowIfClosedOrDisposed();
        return _isSchemaOnly
            ? NextResultSchemaOnly(async: true, cancellationToken: cancellationToken)
            : NextResult(async: true, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Internal implementation of NextResult
    /// </summary>
    async Task<bool> NextResult(bool async, bool isConsuming = false, CancellationToken cancellationToken = default)
    {
        Debug.Assert(!_isSchemaOnly);
        if (State is ReaderState.Consumed)
            return false;

        try
        {
            using var registration = isConsuming ? default : Connector.StartNestedCancellableOperation(cancellationToken);
            // If we're in the middle of a resultset, consume it
            if (State is ReaderState.BeforeResult or ReaderState.InResult)
                await ConsumeResultSet(async).ConfigureAwait(false);

            Debug.Assert(State is ReaderState.BetweenResults);

            _hasRows = false;

            var statements = _statements;
            var statementIndex = StatementIndex;
            if (statementIndex >= 0)
            {
                if (RowDescription is { } description && statements[statementIndex].IsPrepared && ColumnInfoCache is { } cache)
                    description.SetColumnInfoCache(new(cache, 0, ColumnCount));

                if (statementIndex is 0 && _behavior.HasFlag(CommandBehavior.SingleResult) && !isConsuming)
                {
                    await Consume(async).ConfigureAwait(false);
                    return false;
                }
            }

            // We are now at the end of the previous result set. Read up to the next result set, if any.
            // Non-prepared statements receive ParseComplete, BindComplete, DescriptionRow/NoData,
            // prepared statements receive only BindComplete
            for (statementIndex = ++StatementIndex; statementIndex < statements.Count; statementIndex = ++StatementIndex)
            {
                var statement = statements[statementIndex];

                IBackendMessage msg;
                if (statement.TryGetPrepared(out var preparedStatement))
                {
                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
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
                            Expect<CloseCompletedMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                            preparedStatement.StatementBeingReplaced.CompleteUnprepare();
                            preparedStatement.StatementBeingReplaced = null;
                        }
                    }

                    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);

                    if (statement.IsPreparing)
                    {
                        preparedStatement!.State = PreparedState.Prepared;
                        Connector.PreparedStatementManager.NumPrepared++;
                        statement.IsPreparing = false;
                    }

                    Expect<BindCompleteMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                    msg = await Connector.ReadMessage(async).ConfigureAwait(false);

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

                if (RowDescription is not null)
                {
                    if (ColumnInfoCache?.Length >= ColumnCount)
                        Array.Clear(ColumnInfoCache, 0, ColumnCount);
                    else
                    {
                        if (ColumnInfoCache is { } cache)
                            ArrayPool<ColumnInfo>.Shared.Return(cache, clearArray: true);
                        ColumnInfoCache = ArrayPool<ColumnInfo>.Shared.Rent(ColumnCount);
                    }
                    if (statement.IsPrepared)
                        RowDescription.LoadColumnInfoCache(Connector.SerializerOptions, ColumnInfoCache);
                }
                else
                {
                    // Statement did not generate a resultset (e.g. INSERT)
                    // Read and process its completion message and move on to the next statement
                    // No need to read sequentially as it's not a DataRow
                    msg = await Connector.ReadMessage(async).ConfigureAwait(false);
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
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);

                    continue;
                }

                if ((Command.WrappingBatch is not null || StatementIndex is 0) && Command.InternalBatchCommands[StatementIndex]._parameters?.HasOutputParameters == true)
                {
                    // If output parameters are present and this is the first row of the resultset,
                    // we must always read it in non-sequential mode because it will be traversed twice (once
                    // here for the parameters, then as a regular row).
                    msg = await Connector.ReadMessage(async).ConfigureAwait(false);
                    ProcessMessage(msg);
                    if (msg.Code == BackendMessageCode.DataRow)
                    {
                        try
                        {
                            PopulateOutputParameters(Command.InternalBatchCommands[StatementIndex]._parameters!);
                        }
                        catch (Exception e)
                        {
                            // TODO: ideally we should flow down to global exception filter and consume there
                            await Consume(async, firstException: e).ConfigureAwait(false);
                            throw;
                        }
                    }
                }
                else
                {
                    msg = await ReadMessage(async).ConfigureAwait(false);
                    ProcessMessage(msg);
                }

                switch (msg.Code)
                {
                case BackendMessageCode.DataRow:
                    Connector.State = ConnectorState.Fetching;
                    return true;
                case BackendMessageCode.CommandComplete:
                    if (statement.AppendErrorBarrier ?? Command.EnableErrorBarriers)
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                    return true;
                default:
                    Connector.UnexpectedMessageReceived(msg.Code);
                    break;
                }
            }

            // There are no more queries, we're done. Read the RFQ.
            if (_statements.Count is 0 || !(_statements[^1].AppendErrorBarrier ?? Command.EnableErrorBarriers))
                Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);

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
                // TODO: we probably should do than even if it's not PostgresException (error from PopulateOutputParameters)
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
                // TODO: We might as well call Consume on every command (even the last one) to make sure we do read every single message until RFQ
                // in case we get an exception in the middle of NextResult
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
                        await Consume(async, firstException: e).ConfigureAwait(false);
                        break; // Never reached, Consume always throws above
                    }
                }
            }

            // Break may have progressed the reader already.
            if (State is not ReaderState.Closed)
                State = ReaderState.Consumed;
            throw;
        }

        async ValueTask ConsumeResultSet(bool async)
        {
            await ConsumeRow(async).ConfigureAwait(false);
            while (true)
            {
                var completedMsg = await Connector.ReadMessage(async, DataRowLoadingMode.Skip).ConfigureAwait(false);
                switch (completedMsg.Code)
                {
                case BackendMessageCode.CommandComplete:
                case BackendMessageCode.EmptyQueryResponse:
                    ProcessMessage(completedMsg);

                    var statement = _statements[StatementIndex];
                    if (statement.IsPrepared && ColumnInfoCache is not null)
                        RowDescription!.SetColumnInfoCache(new(ColumnInfoCache, 0, ColumnCount));

                    if (statement.AppendErrorBarrier ?? Command.EnableErrorBarriers)
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);

                    break;
                default:
                    // TODO if we hit an ErrorResponse here (PG doesn't do this *today*) we should probably throw.
                    continue;
                }

                break;
            }
        }
    }


    void PopulateOutputParameters(NpgsqlParameterCollection parameters)
    {
        // The first row in a stored procedure command that has output parameters needs to be traversed twice -
        // once for populating the output parameters and once for the actual result set traversal. So in this
        // case we can't be sequential.
        Debug.Assert(RowDescription != null);
        Debug.Assert(State == ReaderState.BeforeResult);

        var currentPosition = Buffer.ReadPosition;

        // Temporarily set our state to InResult to allow us to read the values
        State = ReaderState.InResult;

        var pending = new Queue<object>();
        var taken = new List<NpgsqlParameter>();
        for (var i = 0; i < ColumnCount; i++)
        {
            if (parameters.TryGetValue(GetName(i), out var p) && p.IsOutputDirection)
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
        foreach (var p in (IEnumerable<NpgsqlParameter>)parameters)
        {
            if (!p.IsOutputDirection || taken.Contains(p))
                continue;

            if (pending.Count == 0)
                break;
            p.Value = pending.Dequeue();
        }

        PgReader.Commit();
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
        if (State is ReaderState.Consumed)
            return false;

        using var registration = isConsuming ? default : Connector.StartNestedCancellableOperation(cancellationToken);

        try
        {
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
                            Expect<CloseCompletedMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                            pStatement.StatementBeingReplaced.CompleteUnprepare();
                            pStatement.StatementBeingReplaced = null;
                        }
                    }

                    Expect<ParseCompleteMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);

                    if (statement.IsPreparing)
                    {
                        pStatement!.State = PreparedState.Prepared;
                        Connector.PreparedStatementManager.NumPrepared++;
                        statement.IsPreparing = false;
                    }

                    Expect<ParameterDescriptionMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                    var msg = await Connector.ReadMessage(async).ConfigureAwait(false);
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

                    var forall = true;
                    for (var i = StatementIndex + 1; i < _statements.Count; i++)
                        if (!_statements[i].IsPrepared)
                        {
                            forall = false;
                            break;
                        }
                    // There are no more queries, we're done. Read to the RFQ.
                    if (forall)
                        Expect<ReadyForQueryMessage>(await Connector.ReadMessage(async).ConfigureAwait(false), Connector);
                }

                // Found a resultset
                if (RowDescription is not null)
                {
                    if (ColumnInfoCache?.Length >= ColumnCount)
                        Array.Clear(ColumnInfoCache, 0, ColumnCount);
                    else
                    {
                        if (ColumnInfoCache is { } cache)
                            ArrayPool<ColumnInfo>.Shared.Return(cache, clearArray: true);
                        ColumnInfoCache = ArrayPool<ColumnInfo>.Shared.Rent(ColumnCount);
                    }
                    return true;
                }
            }

            State = ReaderState.Consumed;
            RowDescription = null;
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
        if (msg.Code is not BackendMessageCode.DataRow)
        {
            HandleUncommon(msg);
            return;
        }

        var dataRow = (DataRowMessage)msg;
        // The connector's buffer can actually change between DataRows:
        // If a large DataRow exceeding the connector's current read buffer arrives, and we're
        // reading in non-sequential mode, a new oversize buffer is allocated. We thus have to
        // recapture the connector's buffer on each new DataRow.
        // Note that this can happen even in sequential mode, if the row description message is big
        // (see #2003)
        if (!ReferenceEquals(Buffer, Connector.ReadBuffer))
            Buffer = Connector.ReadBuffer;
        // We assume that the row's number of columns is identical to the description's
        var numColumns = Buffer.ReadInt16();
        if (ColumnCount != numColumns)
            ThrowHelper.ThrowArgumentException($"Row's number of columns ({numColumns}) differs from the row description's ({ColumnCount})");

        var readPosition = Buffer.ReadPosition;
        var msgRemainder = dataRow.Length - sizeof(short);
        _dataMsgEnd = readPosition + msgRemainder;
        _columnsStartPos = readPosition;
        _isRowBuffered = msgRemainder <= Buffer.FilledBytes - readPosition;
        Debug.Assert(_isRowBuffered || _isSequential);
        _column = -1;

        if (_columns.Count > 0)
            _columns.Clear();

        switch (State)
        {
        case ReaderState.BetweenResults:
            _hasRows = true;
            State = ReaderState.BeforeResult;
            break;
        case ReaderState.BeforeResult:
            State = ReaderState.InResult;
            break;
        case ReaderState.InResult:
            break;
        default:
            Connector.UnexpectedMessageReceived(BackendMessageCode.DataRow);
            break;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        void HandleUncommon(IBackendMessage msg)
        {
            switch (msg.Code)
            {
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
                    _recordsAffected ??= 0;
                    _recordsAffected += completed.Rows;
                    break;
                }

                _statements[StatementIndex].ApplyCommandComplete(completed);
                State = ReaderState.BetweenResults;
                break;
            case BackendMessageCode.EmptyQueryResponse:
                State = ReaderState.BetweenResults;
                break;
            default:
                Connector.UnexpectedMessageReceived(msg.Code);
                break;
            }
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
    public override bool IsClosed => State is ReaderState.Closed or ReaderState.Disposed;

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
    public IReadOnlyList<NpgsqlBatchCommand> Statements
    {
        get
        {
            ThrowIfClosedOrDisposed();
            return _statements.AsReadOnly();
        }
    }

    /// <summary>
    /// Gets a value that indicates whether this DbDataReader contains one or more rows.
    /// </summary>
    public override bool HasRows
    {
        get
        {
            ThrowIfClosedOrDisposed();
            return _hasRows;
        }
    }

    /// <summary>
    /// Indicates whether the reader is currently positioned on a row, i.e. whether reading a
    /// column is possible.
    /// This property is different from <see cref="HasRows"/> in that <see cref="HasRows"/> will
    /// return true even if attempting to read a column will fail, e.g. before <see cref="Read()"/>
    /// has been called
    /// </summary>
    public bool IsOnRow
    {
        get
        {
            ThrowIfClosedOrDisposed();
            return State is ReaderState.InResult;
        }
    }

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
            ThrowIfClosedOrDisposed();
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
                        ? await NextResultSchemaOnly(async, isConsuming: true).ConfigureAwait(false)
                        : await NextResult(async, isConsuming: true).ConfigureAwait(false)))
                {
                    break;
                }
            }
            catch (Exception e)
            {
                exceptions ??= [];
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
                  AllPostgresExceptions(aggregateException.InnerExceptions)))
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
    public override async ValueTask DisposeAsync()
    {
        try
        {
            await Close(connectionClosing: false, async: true, isDisposing: true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // In the case of a PostgresException (or multiple ones, if we have error barriers), the reader's state has already been set
            // to Disposed in Close above; in multiplexing, we also unbind the connector (with its reader), and at that point it can be used
            // by other consumers. Therefore, we only set the state to Disposed if the exception *wasn't* a PostgresException.
            if (!(ex is PostgresException ||
                  ex is NpgsqlException { InnerException: AggregateException aggregateException } &&
                  AllPostgresExceptions(aggregateException.InnerExceptions)))
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

    static bool AllPostgresExceptions(ReadOnlyCollection<Exception> collection)
    {
        foreach (var exception in collection)
            if (exception is not PostgresException)
                return false;
        return true;
    }

    /// <summary>
    /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
    /// </summary>
    public override void Close() => Close(connectionClosing: false, async: false, isDisposing: false).GetAwaiter().GetResult();

    /// <summary>
    /// Closes the <see cref="NpgsqlDataReader"/> reader, allowing a new command to be executed.
    /// </summary>
    public override Task CloseAsync()
        => Close(async: true, connectionClosing: false, isDisposing: false);

    internal async Task Close(bool async, bool connectionClosing, bool isDisposing)
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
                    await Consume(async).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is OperationCanceledException or NpgsqlException { InnerException: TimeoutException })
                {
                    // Timeout/cancellation - completely normal, consume has basically completed.
                }
                catch (Exception ex) when (
                    ex is PostgresException ||
                    ex is NpgsqlException { InnerException: AggregateException aggregateException } &&
                    AllPostgresExceptions(aggregateException.InnerExceptions))
                {
                    // In the case of a PostgresException (or multiple ones, if we have error barriers), the connection is fine and consume
                    // has basically completed. Defer throwing the exception until Cleanup is complete.
                    await Cleanup(async, connectionClosing, isDisposing).ConfigureAwait(false);
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

        await Cleanup(async, connectionClosing, isDisposing).ConfigureAwait(false);
    }

    internal async Task Cleanup(bool async, bool connectionClosing = false, bool isDisposing = false)
    {
        LogMessages.ReaderCleanup(_commandLogger, Connector.Id);

        // If multiplexing isn't on, _sendTask contains the task for the writing of this command.
        // Make sure that this task, which may have executed asynchronously and in parallel with the reading,
        // has completed, throwing any exceptions it generated. If we don't do this, there's the possibility of a race condition where the
        // user executes a new command after reader.Dispose() returns, but some additional write stuff is still finishing up from the last
        // command.
        if (_sendTask is { Status: not TaskStatus.RanToCompletion })
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
                        await _sendTask.ConfigureAwait(false);
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

        if (ColumnInfoCache is { } cache)
        {
            ColumnInfoCache = null;
            ArrayPool<ColumnInfo>.Shared.Return(cache, clearArray: true);
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
    public override bool GetBoolean(int ordinal) => GetFieldValueCore<bool>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a byte.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override byte GetByte(int ordinal) => GetFieldValueCore<byte>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a single character.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override char GetChar(int ordinal) => GetFieldValueCore<char>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 16-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override short GetInt16(int ordinal) => GetFieldValueCore<short>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 32-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override int GetInt32(int ordinal) => GetFieldValueCore<int>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a 64-bit signed integer.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override long GetInt64(int ordinal) => GetFieldValueCore<long>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a <see cref="DateTime"/> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override DateTime GetDateTime(int ordinal) => GetFieldValueCore<DateTime>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as an instance of <see cref="string"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override string GetString(int ordinal) => GetFieldValueCore<string>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a <see cref="decimal"/> object.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override decimal GetDecimal(int ordinal) => GetFieldValueCore<decimal>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a double-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override double GetDouble(int ordinal) => GetFieldValueCore<double>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a single-precision floating point number.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override float GetFloat(int ordinal) => GetFieldValueCore<float>(ordinal);

    /// <summary>
    /// Gets the value of the specified column as a globally-unique identifier (GUID).
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The value of the specified column.</returns>
    public override Guid GetGuid(int ordinal) => GetFieldValueCore<Guid>(ordinal);

    /// <summary>
    /// Populates an array of objects with the column values of the current row.
    /// </summary>
    /// <param name="values">An array of Object into which to copy the attribute columns.</param>
    /// <returns>The number of instances of <see cref="object"/> in the array.</returns>
    public override int GetValues(object[] values)
    {
        ThrowIfNotInResult();
        ArgumentNullException.ThrowIfNull(values);

        var count = Math.Min(ColumnCount, values.Length);
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
    public TimeSpan GetTimeSpan(int ordinal) => GetFieldValueCore<TimeSpan>(ordinal);

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
        ThrowIfNotInResult();
        var field = RowDescription[ordinal];
        if (_isSequential)
            ThrowHelper.ThrowNotSupportedException("GetData() not supported in sequential mode.");

        var type = field.PostgresType;
        var isArray = type is PostgresArrayType;
        var elementType = isArray ? ((PostgresArrayType)type).Element : type;
        var compositeType = elementType as PostgresCompositeType;
        if (field.DataFormat is DataFormat.Text || (elementType.InternalName != "record" && compositeType == null))
            ThrowHelper.ThrowInvalidCastException("GetData() not supported for type " + field.TypeDisplayName);

        if (SeekToColumn(ordinal, field.DataFormat, resumableOp: true) is -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(field);

        Debug.Assert(!PgReader.NestedInitialized, "Unexpected nested read active, Seek(0) would seek to the start of the nested data.");
        PgReader.Seek(0);

        var reader = CachedFreeNestedDataReader;
        if (reader != null)
        {
            CachedFreeNestedDataReader = null;
            reader.Init(compositeType);
        }
        else
        {
            reader = new NpgsqlNestedDataReader(this, null, 1, compositeType);
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
        ThrowIfNotInResult();
        var field = RowDescription[ordinal];

        if (dataOffset is < 0 or > int.MaxValue)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(dataOffset), "dataOffset must be between 0 and {0}", int.MaxValue);
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            ThrowHelper.ThrowIndexOutOfRangeException("bufferOffset must be between 0 and {0}", buffer.Length);
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            ThrowHelper.ThrowIndexOutOfRangeException("bufferOffset must be between 0 and {0}", buffer.Length - bufferOffset);

        if (SeekToColumn(ordinal, field.DataFormat, resumableOp: true) is var columnLength && columnLength is -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(field);

        if (buffer is null)
            return columnLength;

        // Check whether any sequential seek is contractually sound (even though we might be able to satisfy rewinds we make sure we won't).
        if (_isSequential && PgReader.IsFieldConsumed((int)dataOffset))
            ThrowHelper.ThrowInvalidOperationException("Attempt to read a position in the column which has already been read");

        // Move to offset
        Debug.Assert(!PgReader.NestedInitialized, "Unexpected nested read active, Seek(0) would seek to the start of the nested data.");
        var remaining = PgReader.Seek((int)dataOffset);

        // At offset, read into buffer.
        length = Math.Min(length, remaining);
        PgReader.ReadBytes(new Span<byte>(buffer, bufferOffset, length));
        return length;
    }

    /// <summary>
    /// Retrieves data as a <see cref="Stream"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The returned object.</returns>
    public override Stream GetStream(int ordinal)
        => GetFieldValueCore<Stream>(ordinal);

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
        ThrowIfNotInResult();

        // Check whether we have a GetChars implementation for this column type.
        var field = GetInfo(ordinal, typeof(GetChars), out var converter, out var bufferRequirement, out var asObject);

        if (dataOffset is < 0 or > int.MaxValue)
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(dataOffset), "dataOffset must be between 0 and {0}", int.MaxValue);
        if (buffer != null && (bufferOffset < 0 || bufferOffset >= buffer.Length + 1))
            ThrowHelper.ThrowIndexOutOfRangeException("bufferOffset must be between 0 and {0}", buffer.Length);
        if (buffer != null && (length < 0 || length > buffer.Length - bufferOffset))
            ThrowHelper.ThrowIndexOutOfRangeException("bufferOffset must be between 0 and {0}", buffer.Length - bufferOffset);

        if (SeekToColumn(ordinal, field, resumableOp: true) is -1)
            ThrowHelper.ThrowInvalidCastException_NoValue(RowDescription[ordinal]);

        var reader = PgReader;
        dataOffset = buffer is null ? 0 : dataOffset;
        if (_isSequential && reader.CharsRead > dataOffset)
            ThrowHelper.ThrowInvalidOperationException("Attempt to read a position in the column which has already been read");

        reader.StartCharsRead(checked((int)dataOffset),
            buffer is not null ? new ArraySegment<char>(buffer, bufferOffset, length) : (ArraySegment<char>?)null);

        reader.StartRead(bufferRequirement);
        var result = asObject
            ? (GetChars)converter.ReadAsObject(reader)
            : ((PgConverter<GetChars>)converter).Read(reader);
        reader.EndRead();

        reader.EndCharsRead();
        return result.Read;
    }

    /// <summary>
    /// Retrieves data as a <see cref="TextReader"/>.
    /// </summary>
    /// <param name="ordinal">The zero-based column ordinal.</param>
    /// <returns>The returned object.</returns>
    public override TextReader GetTextReader(int ordinal)
        => GetFieldValueCore<TextReader>(ordinal);

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
        // As the row is buffered we know the column is too - no I/O will take place
        if (_isRowBuffered)
            return Task.FromResult(GetFieldValueCore<T>(ordinal));

        // The only statically mapped converter, it always exists.
        if (typeof(T) == typeof(Stream))
            return GetStream(ordinal, cancellationToken);

        return Core(ordinal, cancellationToken).AsTask();

        async ValueTask<T> Core(int ordinal, CancellationToken cancellationToken)
        {
            ThrowIfNotInResult();
            var field = GetInfo(ordinal, typeof(T), out var converter, out var bufferRequirement, out var asObject);

            using var registration = Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);
            if (await SeekToColumnAsync(ordinal, field).ConfigureAwait(false) is -1)
                return DbNullValueOrThrow<T>(ordinal);

            if (typeof(T) == typeof(TextReader))
                PgReader.ThrowIfStreamActive();

            Debug.Assert(asObject || converter is PgConverter<T>);
            await PgReader.StartReadAsync(bufferRequirement, cancellationToken).ConfigureAwait(false);
            var result = asObject
                ? (T)await converter.ReadAsObjectAsync(PgReader, cancellationToken).ConfigureAwait(false)
                : await converter.UnsafeDowncast<T>().ReadAsync(PgReader, cancellationToken).ConfigureAwait(false);
            await PgReader.EndReadAsync().ConfigureAwait(false);
            return result;
        }

        async Task<T> GetStream(int ordinal, CancellationToken cancellationToken)
        {
            using var registration = Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

            var field = GetDefaultInfo(ordinal, out _, out _);
            PgReader.ThrowIfStreamActive();

            if (await SeekToColumnAsync(ordinal, field).ConfigureAwait(false) is -1)
                return DbNullValueOrThrow<T>(ordinal);

            return (T)(object)PgReader.GetStream(canSeek: !_isSequential);
        }
    }

    /// <summary>
    /// Synchronously gets the value of the specified column as a type.
    /// </summary>
    /// <typeparam name="T">Synchronously gets the value of the specified column as a type.</typeparam>
    /// <param name="ordinal">The column to be retrieved.</param>
    /// <returns>The column to be retrieved.</returns>
    public override T GetFieldValue<T>(int ordinal) => GetFieldValueCore<T>(ordinal);

    T GetFieldValueCore<T>(int ordinal)
    {
        ThrowIfNotInResult();

        // The only statically mapped converter, it always exists.
        if (typeof(T) == typeof(Stream))
            return GetStream(ordinal);

        var field = GetInfo(ordinal, typeof(T), out var converter, out var bufferRequirement, out var asObject);

        if (typeof(T) == typeof(TextReader))
            PgReader.ThrowIfStreamActive();

        if (SeekToColumn(ordinal, field) is -1)
            return DbNullValueOrThrow<T>(ordinal);

        Debug.Assert(asObject || converter is PgConverter<T>);
        PgReader.StartRead(bufferRequirement);
        var result = asObject
            ? (T)converter.ReadAsObject(PgReader)
            : converter.UnsafeDowncast<T>().Read(PgReader);
        PgReader.EndRead();
        return result;

        [MethodImpl(MethodImplOptions.NoInlining)]
        T GetStream(int ordinal)
        {
            var field = GetDefaultInfo(ordinal, out _, out _);
            PgReader.ThrowIfStreamActive();

            if (SeekToColumn(ordinal, field) is -1)
                return DbNullValueOrThrow<T>(ordinal);

            return (T)(object)PgReader.GetStream(canSeek: !_isSequential);
        }
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
        ThrowIfNotInResult();
        var field = GetDefaultInfo(ordinal, out var converter, out var bufferRequirement);
        if (SeekToColumn(ordinal, field) is -1)
            return DBNull.Value;

        PgReader.StartRead(bufferRequirement);
        var result = converter.ReadAsObject(PgReader);
        PgReader.EndRead();

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
    {
        ThrowIfNotInResult();
        return SeekToColumn(ordinal, RowDescription[ordinal].DataFormat, resumableOp: true) is -1;
    }

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
        if (_isRowBuffered)
            return IsDBNull(ordinal) ? TrueTask : FalseTask;

        return Core(ordinal, cancellationToken);

        async Task<bool> Core(int ordinal, CancellationToken cancellationToken)
        {
            ThrowIfNotInResult();
            using var registration = Connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);
            return await SeekToColumnAsync(ordinal, RowDescription[ordinal].DataFormat, resumableOp: true).ConfigureAwait(false) is -1;
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
        ThrowIfClosedOrDisposed();
        if (string.IsNullOrEmpty(name))
            ThrowHelper.ThrowArgumentException($"{nameof(name)} cannot be empty", nameof(name));
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
    [UnconditionalSuppressMessage("ILLink", "IL2093",
        Justification = "Members are only dynamically accessed by Npgsql via GetFieldType by GetSchema, and only in certain cases. " +
                        "Holding PublicFields and PublicProperties metadata on all our mapped types just for that case is the wrong tradeoff.")]
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
    {
        var columns = GetColumnSchema();
        var result = new DbColumn[columns.Count];
        var i = 0;
        foreach (var column in columns)
            result[i++] = column;

        return new ReadOnlyCollection<DbColumn>(result);
    }

    /// <summary>
    /// Asynchronously returns schema information for the columns in the current resultset.
    /// </summary>
    /// <returns></returns>
    public new Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchemaAsync(CancellationToken cancellationToken = default)
        => GetColumnSchema(async: true, cancellationToken);

    Task<ReadOnlyCollection<NpgsqlDbColumn>> GetColumnSchema(bool async, CancellationToken cancellationToken = default)
        => RowDescription == null || ColumnCount == 0
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
    public override Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default)
        => GetSchemaTable(async: true, cancellationToken);

    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "typeof(Type).TypeInitializer is not used.")]
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

        foreach (var column in await GetColumnSchema(async, cancellationToken).ConfigureAwait(false))
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
            row["IsReadOnly"] = column.IsReadOnly == true;
            row["DataTypeName"] = column.DataTypeName;

            table.Rows.Add(row);
        }

        return table;
    }

    #endregion Schema metadata table

    #region Seeking

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int SeekToColumn(int ordinal, DataFormat dataFormat, bool resumableOp = false)
    {
        Debug.Assert(_isRowBuffered || _isSequential);
        var reader = PgReader;
        var column = _column;

        // Column rereading rules for sequential mode:
        // * We never allow rereading if the column didn't get initialized as resumable the previous time
        // * If it did get initialized as resumable we only allow rereading when either of the following is true:
        //  - The op is a resumable one again
        //  - The op isn't resumable but the field is still entirely unconsumed
        if (_isSequential && (column > ordinal || (column == ordinal && (!reader.Resumable || (!resumableOp && !reader.FieldAtStart)))))
            ThrowInvalidSequentialSeek(column, ordinal);

        if (column == ordinal)
            return reader.Restart(resumableOp);

        reader.Commit();
        var columnLength = BufferSeekToColumn(column, ordinal, !_isRowBuffered);
        reader.Init(columnLength, dataFormat, resumableOp);
        return columnLength;

        static void ThrowInvalidSequentialSeek(int column, int ordinal)
            => ThrowHelper.ThrowInvalidOperationException(
                $"Invalid attempt to read from column ordinal '{ordinal}'. With CommandBehavior.SequentialAccess, " +
                $"you may only read from column ordinal '{column}' or greater.");
    }

    ValueTask<int> SeekToColumnAsync(int ordinal, DataFormat dataFormat, bool resumableOp = false)
    {
        // When the row is buffered or we're rereading previous data no IO will be done.
        if (_isRowBuffered || _column >= ordinal)
            return new(SeekToColumn(ordinal, dataFormat, resumableOp));

        return Core(ordinal, dataFormat, resumableOp);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<int> Core(int ordinal, DataFormat dataFormat, bool resumableOp)
        {
            Debug.Assert(!_isRowBuffered && _column < ordinal);

            var reader = PgReader;
            await reader.CommitAsync().ConfigureAwait(false);
            var columnLength = await BufferSeekToColumnAsync(_column, ordinal, !_isRowBuffered).ConfigureAwait(false);
            reader.Init(columnLength, dataFormat, resumableOp);
            return columnLength;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    int BufferSeekToColumn(int column, int ordinal, bool allowIO)
    {
        Debug.Assert(column < ordinal || !allowIO);

        if (column >= ordinal)
        {
            _column = ordinal;
            return SeekBackwards(ordinal);
        }

        // We know we need at least one iteration, a do while also helps with optimal codegen.
        var buffer = Buffer;
        var columnLength = 0;
        do
        {
            if (columnLength > 0)
                buffer.Skip(columnLength, allowIO);

            if (allowIO)
                buffer.Ensure(sizeof(int));
            columnLength = buffer.ReadInt32();
            Debug.Assert(columnLength >= -1);
        } while (++_column < ordinal);

        return columnLength;

        // On the first call to SeekBackwards we'll fill up the columns list as we may need seek positions more than once.
        [MethodImpl(MethodImplOptions.NoInlining)]
        int SeekBackwards(int ordinal)
        {
            var buffer = Buffer;
            var columns = _columns;

            (buffer.ReadPosition, var columnLength) = columns.Count is 0
                ? (_columnsStartPos, 0)
                : columns[Math.Min(columns.Count -1, ordinal)];

            while (columns.Count <= ordinal)
            {
                if (columnLength > 0)
                    buffer.Skip(columnLength);
                columnLength = buffer.ReadInt32();
                columns.Add((buffer.ReadPosition, columnLength));
            }

            return columnLength;
        }
    }

    ValueTask<int> BufferSeekToColumnAsync(int column, int ordinal, bool allowIO)
    {
        return !allowIO || column >= ordinal ? new(BufferSeekToColumn(column, ordinal, allowIO)) : Core(ordinal);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<int> Core(int ordinal)
        {
            // We know we need at least one iteration, a do while also helps with optimal codegen.
            var buffer = Buffer;
            var columnLength = 0;
            do
            {
                if (columnLength > 0)
                    await buffer.Skip(async: true, columnLength).ConfigureAwait(false);

                await buffer.EnsureAsync(sizeof(int)).ConfigureAwait(false);
                columnLength = buffer.ReadInt32();
                Debug.Assert(columnLength >= -1);
            } while (++_column < ordinal);

            return columnLength;
        }
    }

    #endregion

    #region ConsumeRow

    Task ConsumeRow(bool async)
    {
        Debug.Assert(State is ReaderState.InResult or ReaderState.BeforeResult);

        if (!_isRowBuffered)
            return ConsumeRowSequential(async);

        // We get here, if we're in a non-sequential mode (or the row is already in the buffer)
        ConsumeBufferedRow();
        return Task.CompletedTask;

        async Task ConsumeRowSequential(bool async)
        {
            if (async)
                await PgReader.CommitAsync().ConfigureAwait(false);
            else
                PgReader.Commit();

            // Skip over the remaining columns in the row
            var buffer = Buffer;
            // Written as a while to be able to increment _column directly after reading into it.
            while (_column < ColumnCount - 1)
            {
                await buffer.Ensure(4, async).ConfigureAwait(false);
                var columnLength = buffer.ReadInt32();
                _column++;
                Debug.Assert(columnLength >= -1);
                if (columnLength > 0)
                    await buffer.Skip(async, columnLength).ConfigureAwait(false);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ConsumeBufferedRow()
    {
        Debug.Assert(State is ReaderState.InResult or ReaderState.BeforeResult);
        PgReader.Commit();
        Buffer.ReadPosition = _dataMsgEnd;
    }

    #endregion

    #region Checks

    [MethodImpl(MethodImplOptions.NoInlining)]
    T DbNullValueOrThrow<T>(int ordinal)
    {
        // When T is a Nullable<T> (and only in that case), we support returning null
        if (default(T) is null && typeof(T).IsValueType)
            return default!;

        if (typeof(T) == typeof(object))
            return (T)(object)DBNull.Value;

        ThrowHelper.ThrowInvalidCastException_NoValue(RowDescription![ordinal]);
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    DataFormat GetInfo(int ordinal, Type type, out PgConverter converter, out Size bufferRequirement, out bool asObject)
    {
        if ((uint)ordinal > (uint)ColumnCount)
            ThrowHelper.ThrowIndexOutOfRangeException("Ordinal must be between 0 and " + (ColumnCount - 1));

        ref var info = ref ColumnInfoCache![ordinal];

        Debug.Assert(info.ConverterInfo.IsDefault || ReferenceEquals(Connector.SerializerOptions, info.ConverterInfo.TypeInfo.Options), "Cache is bleeding over");

        if (info.ConverterInfo.TypeToConvert == type)
        {
            converter = info.ConverterInfo.Converter;
            bufferRequirement = info.ConverterInfo.BufferRequirement;
            asObject = info.AsObject;
            return info.DataFormat;
        }

        return Slow(ref info, out converter, out bufferRequirement, out asObject);

        [MethodImpl(MethodImplOptions.NoInlining)]
        DataFormat Slow(ref ColumnInfo info, out PgConverter converter, out Size bufferRequirement, out bool asObject)
        {
            var field = RowDescription![ordinal];
            field.GetInfo(type, ref info);
            converter = info.ConverterInfo.Converter;
            bufferRequirement = info.ConverterInfo.BufferRequirement;
            asObject = info.AsObject;
            return field.DataFormat;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    DataFormat GetDefaultInfo(int ordinal, out PgConverter converter, out Size bufferRequirement)
    {
        var field = RowDescription![ordinal];

        converter = field.ObjectInfo.Converter;
        bufferRequirement = field.ObjectInfo.BufferRequirement;
        return field.DataFormat;
    }

    /// <summary>
    /// Checks that we have a RowDescription, but not necessary an actual resultset
    /// (for operations which work in SchemaOnly mode.
    /// </summary>
    FieldDescription GetField(int ordinal)
    {
        ThrowIfClosedOrDisposed();
        if (RowDescription is { } columns)
            return columns[ordinal];

        ThrowHelper.ThrowInvalidOperationException("No resultset is currently being traversed");
        return default!;
    }

    void ThrowIfClosedOrDisposed()
    {
        if (State is (ReaderState.Closed or ReaderState.Disposed) and var state)
            ThrowInvalidState(state);
    }

    [MemberNotNull(nameof(RowDescription))]
    void ThrowIfNotInResult()
    {
        if (State is not ReaderState.InResult and var state)
            ThrowInvalidState(state);

        Debug.Assert(RowDescription is not null);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void ThrowInvalidState(ReaderState state)
    {
        switch (state)
        {
        case ReaderState.Closed:
            ThrowHelper.ThrowInvalidOperationException("The reader is closed");
            break;
        case ReaderState.Disposed:
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlDataReader));
            break;
        default:
            ThrowHelper.ThrowInvalidOperationException("No resultset is currently being traversed");
            break;
        }
    }

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
