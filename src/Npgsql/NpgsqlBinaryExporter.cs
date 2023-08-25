using System;
using System.Diagnostics;
using System.Linq;
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

    #region Fields and Properties

    NpgsqlConnector _connector;
    NpgsqlReadBuffer _buf;
    bool _isConsumed, _isDisposed;
    long _endOfMessagePos;

    short _column;
    ulong _rowsExported;

    PgReader PgReader => _buf.PgReader;

    /// <summary>
    /// The number of columns, as returned from the backend in the CopyInResponse.
    /// </summary>
    internal int NumColumns { get; private set; }

    PgConverterInfo[] _columnInfoCache;

    readonly ILogger _copyLogger;

    /// <summary>
    /// Current timeout
    /// </summary>
    public TimeSpan Timeout
    {
        set
        {
            _buf.Timeout = value;
            // While calling Complete(), we're using the connector, which overwrites the buffer's timeout with it's own
            _connector.UserTimeout = (int)value.TotalMilliseconds;
        }
    }

    #endregion

    #region Construction / Initialization

    internal NpgsqlBinaryExporter(NpgsqlConnector connector)
    {
        _connector = connector;
        _buf = connector.ReadBuffer;
        _column = -2;
        _columnInfoCache = null!;
        _copyLogger = connector.LoggingConfiguration.CopyLogger;
    }

    internal async Task Init(string copyToCommand, bool async, CancellationToken cancellationToken = default)
    {
        await _connector.WriteQuery(copyToCommand, async, cancellationToken);
        await _connector.Flush(async, cancellationToken);

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        CopyOutResponseMessage copyOutResponse;
        var msg = await _connector.ReadMessage(async);
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

        NumColumns = copyOutResponse.NumColumns;
        _columnInfoCache = new PgConverterInfo[NumColumns];
        _rowsExported = 0;
        _endOfMessagePos = _buf.CumulativeReadPosition;
        await ReadHeader(async);
    }

    async Task ReadHeader(bool async)
    {
        var msg = await _connector.ReadMessage(async);
        _endOfMessagePos = _buf.CumulativeReadPosition + Expect<CopyDataMessage>(msg, _connector).Length;
        var headerLen = NpgsqlRawCopyStream.BinarySignature.Length + 4 + 4;
        await _buf.Ensure(headerLen, async);

        if (NpgsqlRawCopyStream.BinarySignature.Any(t => _buf.ReadByte() != t))
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
    public ValueTask<int> StartRowAsync(CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return StartRow(true, cancellationToken);
    }

    async ValueTask<int> StartRow(bool async, CancellationToken cancellationToken = default)
    {
        CheckDisposed();
        if (_isConsumed)
            return -1;

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken);

        // Consume and advance any active column.
        if (_column >= 0)
            await Commit(async, resumableOp: false);

        // The very first row (i.e. _column == -1) is included in the header's CopyData message.
        // Otherwise we need to read in a new CopyData row (the docs specify that there's a CopyData
        // message per row).
        if (_column == NumColumns)
        {
            var msg = Expect<CopyDataMessage>(await _connector.ReadMessage(async), _connector);
            _endOfMessagePos = _buf.CumulativeReadPosition + msg.Length;
        }
        else if (_column != -2)
            ThrowHelper.ThrowInvalidOperationException("Already in the middle of a row");

        await _buf.Ensure(2, async);

        var numColumns = _buf.ReadInt16();
        if (numColumns == -1)
        {
            Expect<CopyDoneMessage>(await _connector.ReadMessage(async), _connector);
            Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
            Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
            _column = -2;
            _isConsumed = true;
            return -1;
        }

        Debug.Assert(numColumns == NumColumns);

        _column = -1;
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
    public T Read<T>() => Read<T>(async: false).GetAwaiter().GetResult();

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
    {
        using (NoSynchronizationContextScope.Enter())
            return Read<T>(async: true, cancellationToken);
    }

    ValueTask<T> Read<T>(bool async, CancellationToken cancellationToken = default)
        => Read<T>(async, null, cancellationToken);

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
        var info = options.GetTypeInfo(type, pgTypeId)
                   ?? throw new NotSupportedException($"Reading is not supported for type '{type}'{(npgsqlDbType is null ? "" : $" and NpgsqlDbType '{npgsqlDbType}'")}");
        // Binary export has no type info so we only do caller-directed interpretation of data.
        return info.Bind(new Field("?", info.PgTypeId!.Value, -1), DataFormat.Binary);

        PgTypeId GetRepresentationalOrDefault(string dataTypeName)
        {
            var type = options.TypeCatalog.GetPostgresTypeByName(dataTypeName);
            return options.ToCanonicalTypeId(type.GetRepresentationalType());
        }
    }

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
    public T Read<T>(NpgsqlDbType type) => Read<T>(async: false, type, CancellationToken.None).GetAwaiter().GetResult();

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
    {
        using (NoSynchronizationContextScope.Enter())
            return Read<T>(async: true, type, cancellationToken);
    }

    async ValueTask<T> Read<T>(bool async, NpgsqlDbType? type, CancellationToken cancellationToken)
    {
        CheckDisposed();
        if (_column == -2)
            ThrowHelper.ThrowInvalidOperationException("Not reading a row");

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        // Allow one more read if the field is a db null.
        // We cannot allow endless rereads otherwise it becomes quite unclear when a column advance happens.
        if (PgReader is { Resumable: true, FieldSize: -1 })
        {
            await Commit(async, resumableOp: false);
            return DbNullOrThrow();
        }

        // We must commit the current column before reading the next one unless it was an IsNull call.
        PgConverterInfo info;
        if (!PgReader.Resumable || PgReader.CurrentRemaining != PgReader.FieldSize)
        {
            await Commit(async, resumableOp: false);
            info = GetInfo();

            // We need to get info after potential I/O as we don't know beforehand at what column we're at.
            var columnLen = await ReadColumnLenIfNeeded(async, resumableOp: false);
            if (_column == NumColumns)
                ThrowHelper.ThrowInvalidOperationException("No more columns left in the current row");

            if (columnLen is -1)
                return DbNullOrThrow();

        }
        else
            info = GetInfo();

        T result;
        if (async)
        {
            await PgReader.StartReadAsync(info.BufferRequirement, cancellationToken);
            result = info.AsObject
                ? (T)await info.Converter.ReadAsObjectAsync(PgReader, cancellationToken)
                : await info.GetConverter<T>().ReadAsync(PgReader, cancellationToken);
            await PgReader.EndReadAsync();
        }
        else
        {
            PgReader.StartRead(info.BufferRequirement);
            result = info.AsObject
                ? (T)info.Converter.ReadAsObject(PgReader)
                : info.GetConverter<T>().Read(PgReader);
            PgReader.EndRead();
        }

        return result;

        PgConverterInfo GetInfo()
        {
            ref var cachedInfo = ref _columnInfoCache[_column];
            return cachedInfo.IsDefault ? cachedInfo = CreateConverterInfo(typeof(T), type) : cachedInfo;
        }

        T DbNullOrThrow()
        {
            // When T is a Nullable<T>, we support returning null
            if (default(T) is null && typeof(T).IsValueType)
                return default!;
            throw new InvalidCastException("Column is null");
        }
    }

    /// <summary>
    /// Returns whether the current column is null.
    /// </summary>
    public bool IsNull
    {
        get
        {
            Commit(async: false, resumableOp: true);
            return ReadColumnLenIfNeeded(async: false, resumableOp: true).GetAwaiter().GetResult() is -1;
        }
    }

    /// <summary>
    /// Skips the current column without interpreting its value.
    /// </summary>
    public void Skip() => Skip(false).GetAwaiter().GetResult();

    /// <summary>
    /// Skips the current column without interpreting its value.
    /// </summary>
    public Task SkipAsync(CancellationToken cancellationToken = default)
    {
        using (NoSynchronizationContextScope.Enter())
            return Skip(true, cancellationToken);
    }

    async Task Skip(bool async, CancellationToken cancellationToken = default)
    {
        CheckDisposed();

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken);

        // We allow IsNull to have been called before skip.
        if (PgReader.Initialized && PgReader is not { Resumable: true, FieldSize: -1 })
            await Commit(async, resumableOp: false);
        await ReadColumnLenIfNeeded(async, resumableOp: false);
        await PgReader.Consume(async, cancellationToken: cancellationToken);
    }

    #endregion

    #region Utilities

    ValueTask Commit(bool async, bool resumableOp)
    {
        var resuming = PgReader is { Initialized: true, Resumable: true } && resumableOp;
        if (!resuming)
            _column++;
        return PgReader.Commit(async, resuming);
    }

    async ValueTask<int> ReadColumnLenIfNeeded(bool async, bool resumableOp)
    {
        if (PgReader is { Resumable: true, FieldSize: -1 })
            return -1;

        await _buf.Ensure(4, async);
        var columnLen = _buf.ReadInt32();
        PgReader.Init(columnLen, DataFormat.Binary, resumableOp);
        return PgReader.FieldSize;
    }

    void CheckDisposed()
    {
        if (_isDisposed)
            ThrowHelper.ThrowObjectDisposedException(nameof(NpgsqlBinaryExporter), "The COPY operation has already ended.");
    }

    #endregion

    #region Cancel / Close / Dispose

    /// <summary>
    /// Cancels an ongoing export.
    /// </summary>
    public void Cancel() => _connector.PerformUserCancellation();

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
    public void Dispose() => DisposeAsync(false).GetAwaiter().GetResult();

    /// <summary>
    /// Async completes that binary export and sets the connection back to idle state
    /// </summary>
    /// <returns></returns>
    public ValueTask DisposeAsync()
    {
        using (NoSynchronizationContextScope.Enter())
            return DisposeAsync(true);
    }

    async ValueTask DisposeAsync(bool async)
    {
        if (_isDisposed)
            return;

        if (_isConsumed)
        {
            LogMessages.BinaryCopyOperationCompleted(_copyLogger, _rowsExported, _connector.Id);
        }
        else if (!_connector.IsBroken)
        {
            try
            {
                using var registration = _connector.StartNestedCancellableOperation(attemptPgCancellation: false);
                // Be sure to commit the reader.
                await PgReader.Commit(async, resuming: false);
                // Finish the current CopyData message
                await _buf.Skip(checked((int)(_endOfMessagePos - _buf.CumulativeReadPosition)), async);
                // Read to the end
                _connector.SkipUntil(BackendMessageCode.CopyDone);
                // We intentionally do not pass a CancellationToken since we don't want to cancel cleanup
                Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
                Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
            }
            catch (OperationCanceledException e) when (e.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.QueryCanceled)
            {
                LogMessages.CopyOperationCancelled(_copyLogger, _connector.Id);
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhenDisposingCopyOperation(_copyLogger, _connector.Id, e);
            }
        }

        _connector.EndUserAction();
        Cleanup();
    }

#pragma warning disable CS8625
    void Cleanup()
    {
        Debug.Assert(!_isDisposed);
        var connector = _connector;

        if (connector != null)
        {
            connector.CurrentCopyOperation = null;
            _connector.Connection?.EndBindingScope(ConnectorBindingScope.Copy);
            _connector = null;
        }

        _buf = null;
        _isDisposed = true;
    }
#pragma warning restore CS8625

    #endregion
}
