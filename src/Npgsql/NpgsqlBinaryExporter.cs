using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.Postgres;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql;

/// <summary>
/// Provides an API for a binary COPY TO operation, a high-performance data export mechanism from
/// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginBinaryExport(string)"/>
/// </summary>
public sealed class NpgsqlBinaryExporter : ICancelable
{
    const int BeforeRow = -2;
    const int BeforeColumn = -1;

    #region Fields and Properties

    NpgsqlConnector _connector;
    NpgsqlReadBuffer _buf;
    ExporterState _state = ExporterState.Uninitialized;
    long _endOfMessagePos;

    short _column;
    ulong _rowsExported;

    PgReader PgReader => _buf.PgReader;

    /// <summary>
    /// The number of columns, as returned from the backend in the CopyInResponse.
    /// </summary>
    int NumColumns { get; set; }

    PgConverterInfo[] _columnInfoCache;

    readonly ILogger _copyLogger;

    /// <summary>
    /// Current timeout
    /// </summary>
    public TimeSpan Timeout
    {
        set => _buf.Timeout = value;
    }

    Activity? CurrentActivity;

    #endregion

    #region Construction / Initialization

    internal NpgsqlBinaryExporter(NpgsqlConnector connector)
    {
        _connector = connector;
        _buf = connector.ReadBuffer;
        _column = BeforeRow;
        _columnInfoCache = null!;
        _copyLogger = connector.LoggingConfiguration.CopyLogger;
    }

    internal async Task Init(string copyToCommand, bool async, CancellationToken cancellationToken = default)
    {
        TraceExportStart(copyToCommand);
        try
        {
            await _connector.WriteQuery(copyToCommand, async, cancellationToken).ConfigureAwait(false);
            await _connector.Flush(async, cancellationToken).ConfigureAwait(false);

            using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

            CopyOutResponseMessage copyOutResponse;
            var msg = await _connector.ReadMessage(async).ConfigureAwait(false);
            switch (msg.Code)
            {
            case BackendMessageCode.CopyOutResponse:
                copyOutResponse = (CopyOutResponseMessage)msg;
                if (!copyOutResponse.IsBinary)
                {
                    throw _connector.Break(
                        new ArgumentException("copyToCommand triggered a text transfer, only binary is allowed",
                            nameof(copyToCommand)));
                }
                break;
            case BackendMessageCode.CommandComplete:
                throw new InvalidOperationException(
                    "This API only supports import/export from the client, i.e. COPY commands containing TO/FROM STDIN. " +
                    "To import/export with files on your PostgreSQL machine, simply execute the command with ExecuteNonQuery. " +
                    "Note that your data has been successfully imported/exported.");
            default:
                throw _connector.UnexpectedMessageReceived(msg.Code);
            }

            _state = ExporterState.Ready;
            NumColumns = copyOutResponse.NumColumns;
            _columnInfoCache = new PgConverterInfo[NumColumns];
            _rowsExported = 0;
            _endOfMessagePos = _buf.CumulativeReadPosition;
            await ReadHeader(async).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            TraceSetException(e);
            throw;
        }
    }

    async Task ReadHeader(bool async)
    {
        var msg = await _connector.ReadMessage(async).ConfigureAwait(false);
        _endOfMessagePos = _buf.CumulativeReadPosition + Expect<CopyDataMessage>(msg, _connector).Length;
        var headerLen = NpgsqlRawCopyStream.BinarySignature.Length + 4 + 4;
        await _buf.Ensure(headerLen, async).ConfigureAwait(false);

        foreach (var t in NpgsqlRawCopyStream.BinarySignature)
            if (_buf.ReadByte() != t)
                throw new NpgsqlException("Invalid COPY binary signature at beginning!");

        var flags = _buf.ReadInt32();
        if (flags != 0)
            throw new NotSupportedException("Unsupported flags in COPY operation (OID inclusion?)");

        _buf.ReadInt32();   // Header extensions, currently unused
    }

    #endregion

    #region Read

    /// <summary>
    /// Starts reading a single row, must be invoked before reading any columns.
    /// </summary>
    /// <returns>
    /// The number of columns in the row. -1 if there are no further rows.
    /// Note: This will currently be the same value for all rows, but this may change in the future.
    /// </returns>
    public int StartRow() => StartRow(false).GetAwaiter().GetResult();

    /// <summary>
    /// Starts reading a single row, must be invoked before reading any columns.
    /// </summary>
    /// <returns>
    /// The number of columns in the row. -1 if there are no further rows.
    /// Note: This will currently be the same value for all rows, but this may change in the future.
    /// </returns>
    public ValueTask<int> StartRowAsync(CancellationToken cancellationToken = default) => StartRow(true, cancellationToken);

    async ValueTask<int> StartRow(bool async, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_state == ExporterState.Consumed)
            return -1;

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken);

        // Consume and advance any active column.
        if (_column >= 0)
        {
            if (async)
                await PgReader.CommitAsync().ConfigureAwait(false);
            else
                PgReader.Commit();
            _column++;
        }

        // The very first row (i.e. _column == -1) is included in the header's CopyData message.
        // Otherwise we need to read in a new CopyData row (the docs specify that there's a CopyData
        // message per row).
        if (_column == NumColumns)
        {
            var msg = Expect<CopyDataMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            _endOfMessagePos = _buf.CumulativeReadPosition + msg.Length;
        }
        else if (_column != BeforeRow)
            ThrowHelper.ThrowInvalidOperationException("Already in the middle of a row");

        await _buf.Ensure(2, async).ConfigureAwait(false);

        var numColumns = _buf.ReadInt16();
        if (numColumns == -1)
        {
            Expect<CopyDoneMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            Expect<CommandCompleteMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            _column = BeforeRow;
            _state = ExporterState.Consumed;
            return -1;
        }

        Debug.Assert(numColumns == NumColumns);

        _column = BeforeColumn;
        _rowsExported++;
        return NumColumns;
    }

    /// <summary>
    /// Reads the current column, returns its value and moves ahead to the next column.
    /// If the column is null an exception is thrown.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the column to be read. This must correspond to the actual type or data
    /// corruption will occur. If in doubt, use <see cref="Read{T}(NpgsqlDbType)"/> to manually
    /// specify the type.
    /// </typeparam>
    /// <returns>The value of the column</returns>
    public T Read<T>()
        => Read<T>(null);

    /// <summary>
    /// Reads the current column, returns its value and moves ahead to the next column.
    /// If the column is null an exception is thrown.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the column to be read. This must correspond to the actual type or data
    /// corruption will occur. If in doubt, use <see cref="Read{T}(NpgsqlDbType)"/> to manually
    /// specify the type.
    /// </typeparam>
    /// <returns>The value of the column</returns>
    public ValueTask<T> ReadAsync<T>(CancellationToken cancellationToken = default)
        => ReadAsync<T>(null, cancellationToken);

    /// <summary>
    /// Reads the current column, returns its value according to <paramref name="type"/> and
    /// moves ahead to the next column.
    /// If the column is null an exception is thrown.
    /// </summary>
    /// <param name="type">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type coming in from the
    /// database. This parameter can be used to unambiguously specify the type. An example is the JSONB
    /// type, for which <typeparamref name="T"/> will be a simple string but for which
    /// <paramref name="type"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be read.</typeparam>
    /// <returns>The value of the column</returns>
    public T Read<T>(NpgsqlDbType type)
        => Read<T>((NpgsqlDbType?)type);

    /// <summary>
    /// Reads the current column, returns its value according to <paramref name="type"/> and
    /// moves ahead to the next column.
    /// If the column is null an exception is thrown.
    /// </summary>
    /// <param name="type">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type coming in from the
    /// database. This parameter can be used to unambiguously specify the type. An example is the JSONB
    /// type, for which <typeparamref name="T"/> will be a simple string but for which
    /// <paramref name="type"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be read.</typeparam>
    /// <returns>The value of the column</returns>
    public ValueTask<T> ReadAsync<T>(NpgsqlDbType type, CancellationToken cancellationToken = default)
        => ReadAsync<T>((NpgsqlDbType?)type, cancellationToken);

    T Read<T>(NpgsqlDbType? type)
    {
        ThrowIfNotOnRow();

        if (!IsInitializedAndAtStart)
            MoveNextColumn(resumableOp: false);

        var reader = PgReader;
        try
        {
            if (reader.FieldIsDbNull)
                return DbNullOrThrow<T>();

            var info = GetInfo(typeof(T), type, out var asObject);

            reader.StartRead(info.BufferRequirement);
            var result = asObject
                ? (T)info.Converter.ReadAsObject(reader)
                : info.Converter.UnsafeDowncast<T>().Read(reader);
            reader.EndRead();

            return result;
        }
        finally
        {
            // Don't delay committing the current column, just do it immediately (as opposed to on the next action: Read, IsNull, Skip).
            // Zero length columns would otherwise create an edge-case where we'd have to immediately commit as we won't know whether we're at the end.
            // To guarantee the commit happens in that case we would still need this try finally, at which point it's just better to be consistent.
            reader.Commit();
        }
    }

    async ValueTask<T> ReadAsync<T>(NpgsqlDbType? type, CancellationToken cancellationToken)
    {
        ThrowIfNotOnRow();

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        if (!IsInitializedAndAtStart)
            await MoveNextColumnAsync(resumableOp: false).ConfigureAwait(false);

        var reader = PgReader;
        try
        {
            if (reader.FieldIsDbNull)
                return DbNullOrThrow<T>();

            var info = GetInfo(typeof(T), type, out var asObject);

            await reader.StartReadAsync(info.BufferRequirement, cancellationToken).ConfigureAwait(false);
            var result = asObject
                ? (T)await info.Converter.ReadAsObjectAsync(reader, cancellationToken).ConfigureAwait(false)
                : await info.Converter.UnsafeDowncast<T>().ReadAsync(reader, cancellationToken).ConfigureAwait(false);
            await reader.EndReadAsync().ConfigureAwait(false);

            return result;
        }
        finally
        {
            // Don't delay committing the current column, just do it immediately (as opposed to on the next action: Read, IsNull, Skip).
            // Zero length columns would otherwise create an edge-case where we'd have to immediately commit as we won't know whether we're at the end.
            // To guarantee the commit happens in that case we would still need this try finally, at which point it's just better to be consistent.
            await reader.CommitAsync().ConfigureAwait(false);
        }
    }

    static T DbNullOrThrow<T>()
    {
        // When T is a Nullable<T>, we support returning null
        if (default(T) is null && typeof(T).IsValueType)
            return default!;
        throw new InvalidCastException("Column is null");
    }

    PgConverterInfo GetInfo(Type type, NpgsqlDbType? npgsqlDbType, out bool asObject)
    {
        ref var cachedInfo = ref _columnInfoCache[_column];
        var converterInfo = cachedInfo.IsDefault ? cachedInfo = CreateConverterInfo(type, npgsqlDbType) : cachedInfo;
        asObject = converterInfo.IsBoxingConverter;
        return converterInfo;
    }

    PgConverterInfo CreateConverterInfo(Type type, NpgsqlDbType? npgsqlDbType = null)
    {
        var options = _connector.SerializerOptions;
        PgTypeId? pgTypeId = null;
        if (npgsqlDbType.HasValue)
        {
            pgTypeId = npgsqlDbType.Value.ToDataTypeName() is { } name
                ? options.GetCanonicalTypeId(name)
                // Handle plugin types via lookup.
                : GetRepresentationalOrDefault(npgsqlDbType.Value.ToUnqualifiedDataTypeNameOrThrow());
        }
        var info = options.GetTypeInfoInternal(type, pgTypeId)
                   ?? throw new NotSupportedException($"Reading is not supported for type '{type}'{(npgsqlDbType is null ? "" : $" and NpgsqlDbType '{npgsqlDbType}'")}");

        // Binary export has no type info so we only do caller-directed interpretation of data.
        return info.Bind(new Field("?",
            info.PgTypeId ?? ((PgResolverTypeInfo)info).GetDefaultResolution(null).PgTypeId, -1), DataFormat.Binary);

        PgTypeId GetRepresentationalOrDefault(string dataTypeName)
        {
            var type = options.DatabaseInfo.GetPostgresType(dataTypeName);
            return options.ToCanonicalTypeId(type.GetRepresentationalType());
        }
    }

    /// <summary>
    /// Returns whether the current column is null.
    /// </summary>
    public bool IsNull
    {
        get
        {
            ThrowIfNotOnRow();
            if (!IsInitializedAndAtStart)
                MoveNextColumn(resumableOp: true);

            return PgReader.FieldIsDbNull;
        }
    }

    /// <summary>
    /// Skips the current column without interpreting its value.
    /// </summary>
    public void Skip()
    {
        ThrowIfNotOnRow();

        if (!IsInitializedAndAtStart)
            MoveNextColumn(resumableOp: false);

        PgReader.Commit();
    }

    /// <summary>
    /// Skips the current column without interpreting its value.
    /// </summary>
    public async Task SkipAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotOnRow();

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken);

        if (!IsInitializedAndAtStart)
            await MoveNextColumnAsync(resumableOp: false).ConfigureAwait(false);

        await PgReader.CommitAsync().ConfigureAwait(false);
    }

    #endregion

    #region Utilities

    bool IsInitializedAndAtStart => PgReader.Initialized && (PgReader.FieldIsDbNull || PgReader.FieldAtStart);

    void MoveNextColumn(bool resumableOp)
    {
        PgReader.Commit();

        if (_column + 1 == NumColumns)
            ThrowHelper.ThrowInvalidOperationException("No more columns left in the current row");
        _column++;
        _buf.Ensure(sizeof(int));
        var columnLen = _buf.ReadInt32();
        PgReader.Init(columnLen, DataFormat.Binary, resumableOp);
    }

    async ValueTask MoveNextColumnAsync(bool resumableOp)
    {
        await PgReader.CommitAsync().ConfigureAwait(false);

        if (_column + 1 == NumColumns)
            ThrowHelper.ThrowInvalidOperationException("No more columns left in the current row");
        _column++;
        await _buf.Ensure(sizeof(int), async: true).ConfigureAwait(false);
        var columnLen = _buf.ReadInt32();
        PgReader.Init(columnLen, DataFormat.Binary, resumableOp);
    }

    void ThrowIfNotOnRow()
    {
        ThrowIfDisposed();
        if (_column is BeforeRow)
            ThrowHelper.ThrowInvalidOperationException("Not reading a row");
    }

    void ThrowIfDisposed()
    {
        if (_state == ExporterState.Disposed)
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlBinaryExporter), "The COPY operation has already ended.");
    }

    #endregion

    #region Cancel / Close / Dispose

    /// <summary>
    /// Cancels an ongoing export.
    /// </summary>
    public void Cancel() => _connector.PerformImmediateUserCancellation();

    /// <summary>
    /// Async cancels an ongoing export.
    /// </summary>
    public Task CancelAsync()
    {
        Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Completes that binary export and sets the connection back to idle state
    /// </summary>
    public void Dispose() => DisposeAsync(async: false).GetAwaiter().GetResult();

    /// <summary>
    /// Async completes that binary export and sets the connection back to idle state
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync() => DisposeAsync(async: true);

    async ValueTask DisposeAsync(bool async)
    {
        if (_state == ExporterState.Disposed)
            return;

        try
        {
            if (_state is ExporterState.Consumed or ExporterState.Uninitialized)
            {
                LogMessages.BinaryCopyOperationCompleted(_copyLogger, _rowsExported, _connector.Id);
                TraceExportStop();
            }
            else if (!_connector.IsBroken)
            {
                try
                {
                    using var registration = _connector.StartNestedCancellableOperation(attemptPgCancellation: false);
                    // Be sure to commit the reader.
                    if (async)
                         await PgReader.CommitAsync().ConfigureAwait(false);
                    else
                        PgReader.Commit();
                    // Finish the current CopyData message
                    await _buf.Skip(async, checked((int)(_endOfMessagePos - _buf.CumulativeReadPosition))).ConfigureAwait(false);
                    // Read to the end
                    _connector.SkipUntil(BackendMessageCode.CopyDone);
                    // We intentionally do not pass a CancellationToken since we don't want to cancel cleanup
                    Expect<CommandCompleteMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
                    Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);

                    if (_connector.UserCancellationRequested)
                    {
                        LogMessages.CopyOperationCancelled(_copyLogger, _connector.Id);
                        TraceSetCancelled();
                    }
                    else
                    {
                        TraceExportStop();
                    }
                }
                catch (OperationCanceledException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.QueryCanceled })
                {
                    LogMessages.CopyOperationCancelled(_copyLogger, _connector.Id);
                    TraceSetCancelled();
                }
                catch (Exception e)
                {
                    LogMessages.ExceptionWhenDisposingCopyOperation(_copyLogger, _connector.Id, e);
                    TraceSetException(e);
                }
            }
        }
        finally
        {
            _connector.EndUserAction();
            Cleanup();
        }

        void Cleanup()
        {
            Debug.Assert(_state != ExporterState.Disposed);
            var connector = _connector;

            if (!ReferenceEquals(connector, null))
            {
                connector.CurrentCopyOperation = null;
                _connector.Connection?.EndBindingScope(ConnectorBindingScope.Copy);
                _connector = null!;
            }

            _buf = null!;
            _state = ExporterState.Disposed;
        }
    }

    #endregion

    #region Tracing

    void TraceExportStart(string copyToCommand)
    {
        Debug.Assert(CurrentActivity is null);
        if (NpgsqlActivitySource.IsEnabled)
        {
            var tracingOptions = _connector.DataSource.Configuration.TracingOptions;
            var copyOperationType = CopyOperationType.BinaryExport;

            if (tracingOptions.CopyOperationFilter?.Invoke(copyToCommand, copyOperationType) ?? true)
            {
                var spanName = tracingOptions.CopyOperationSpanNameProvider?.Invoke(copyToCommand, copyOperationType);
                CurrentActivity = NpgsqlActivitySource.ExportStart(copyToCommand, _connector, spanName);

                if (CurrentActivity != null)
                {
                    tracingOptions.CopyOperationEnrichmentCallback?.Invoke(CurrentActivity, copyToCommand, copyOperationType);
                }
            }
        }
    }

    void TraceExportStop()
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.ExportStop(CurrentActivity, _rowsExported);
            CurrentActivity = null;
        }
    }

    private void TraceSetCancelled()
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.SetCancelled(CurrentActivity);
            CurrentActivity = null;
        }
    }

    void TraceSetException(Exception exception)
    {
        if (CurrentActivity is not null)
        {
            NpgsqlActivitySource.SetException(CurrentActivity, exception);
            CurrentActivity = null;
        }
    }

    #endregion Tracing

    #region Enums

    enum ExporterState
    {
        Uninitialized,
        Ready,
        Consumed,
        Disposed
    }

    #endregion Enums
}
