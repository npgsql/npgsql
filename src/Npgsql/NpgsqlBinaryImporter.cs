using System;
using System.Runtime.CompilerServices;
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
/// Provides an API for a binary COPY FROM operation, a high-performance data import mechanism to
/// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginBinaryImport(string)"/>
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/static/sql-copy.html.
/// </remarks>
public sealed class NpgsqlBinaryImporter : ICancelable
{
    #region Fields and Properties

    NpgsqlConnector _connector;
    NpgsqlWriteBuffer _buf;

    ImporterState _state;

    /// <summary>
    /// The number of columns in the current (not-yet-written) row.
    /// </summary>
    short _column;
    ulong _rowsImported;

    /// <summary>
    /// The number of columns, as returned from the backend in the CopyInResponse.
    /// </summary>
    int NumColumns => _params.Length;

    bool InMiddleOfRow => _column != -1 && _column != NumColumns;

    NpgsqlParameter?[] _params;

    readonly ILogger _copyLogger;
    PgWriter _pgWriter = null!; // Setup in Init

    /// <summary>
    /// Current timeout
    /// </summary>
    public TimeSpan Timeout
    {
        set
        {
            _buf.Timeout = value;
            _connector.ReadBuffer.Timeout = value;
        }
    }

    #endregion

    #region Construction / Initialization

    internal NpgsqlBinaryImporter(NpgsqlConnector connector)
    {
        _connector = connector;
        _buf = connector.WriteBuffer;
        _column = -1;
        _params = null!;
        _copyLogger = connector.LoggingConfiguration.CopyLogger;
    }

    internal async Task Init(string copyFromCommand, bool async, CancellationToken cancellationToken = default)
    {
        await _connector.WriteQuery(copyFromCommand, async, cancellationToken).ConfigureAwait(false);
        await _connector.Flush(async, cancellationToken).ConfigureAwait(false);

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        CopyInResponseMessage copyInResponse;
        var msg = await _connector.ReadMessage(async).ConfigureAwait(false);
        switch (msg.Code)
        {
        case BackendMessageCode.CopyInResponse:
            copyInResponse = (CopyInResponseMessage)msg;
            if (!copyInResponse.IsBinary)
            {
                throw _connector.Break(
                    new ArgumentException("copyFromCommand triggered a text transfer, only binary is allowed",
                        nameof(copyFromCommand)));
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

        _params = new NpgsqlParameter[copyInResponse.NumColumns];
        _rowsImported = 0;
        _buf.StartCopyMode();
        WriteHeader();
        // Only init after header.
        _pgWriter = _buf.GetWriter(_connector.DatabaseInfo);
    }

    void WriteHeader()
    {
        _buf.WriteBytes(NpgsqlRawCopyStream.BinarySignature, 0, NpgsqlRawCopyStream.BinarySignature.Length);
        _buf.WriteInt32(0);   // Flags field. OID inclusion not supported at the moment.
        _buf.WriteInt32(0);   // Header extension area length
    }

    #endregion

    #region Write

    /// <summary>
    /// Starts writing a single row, must be invoked before writing any columns.
    /// </summary>
    public void StartRow() => StartRow(false).GetAwaiter().GetResult();

    /// <summary>
    /// Starts writing a single row, must be invoked before writing any columns.
    /// </summary>
    public Task StartRowAsync(CancellationToken cancellationToken = default) => StartRow(async: true, cancellationToken);

    async Task StartRow(bool async, CancellationToken cancellationToken = default)
    {
        CheckReady();
        cancellationToken.ThrowIfCancellationRequested();
        if (_column is not -1 && _column != NumColumns)
            ThrowColumnMismatch();

        if (_buf.WriteSpaceLeft < 2)
            await _buf.Flush(async, cancellationToken).ConfigureAwait(false);
        _buf.WriteInt16((short)NumColumns);

        _pgWriter.RefreshBuffer();
        _column = 0;
        _rowsImported++;
    }

    /// <summary>
    /// Writes a single column in the current row.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <typeparam name="T">
    /// The type of the column to be written. This must correspond to the actual type or data
    /// corruption will occur. If in doubt, use <see cref="Write{T}(T, NpgsqlDbType)"/> to manually
    /// specify the type.
    /// </typeparam>
    public void Write<T>(T value) => Write(async: false, value).GetAwaiter().GetResult();

    /// <summary>
    /// Writes a single column in the current row.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <typeparam name="T">
    /// The type of the column to be written. This must correspond to the actual type or data
    /// corruption will occur. If in doubt, use <see cref="Write{T}(T, NpgsqlDbType)"/> to manually
    /// specify the type.
    /// </typeparam>
    public Task WriteAsync<T>(T value, CancellationToken cancellationToken = default) => Write(async: true, value, cancellationToken);

    Task Write<T>(bool async, T value, CancellationToken cancellationToken = default)
    {
        CheckColumnIndex();
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        // First row, create the parameter object
        ref var p = ref _params[_column];
        if (p is not NpgsqlParameter<T> typedParam)
            typedParam = new NpgsqlParameter<T>();

        // We only report previous values if anything actually changed, this saves some checks during the write.
        // For object typed parameters when we don't have any other data we always have to pass the previousParam.
        // In such cases the runtime type will define the entire postgres type lookup.
        PgTypeInfo? previousTypeInfo = null;
        PgConverter? previousConverter = null;
        PgTypeId previousTypeId = default;
        if (p is not null && (typeof(T) == typeof(object) || p._npgsqlDbType is not null || p._dataTypeName is not null))
        {
            p.GetResolutionInfo(out previousTypeInfo, out previousConverter, out previousTypeId);
            if (ReferenceEquals(p, typedParam))
                p.ResetDbType();
        }

        if (!ReferenceEquals(p, typedParam))
            p = typedParam;

        return Write(async, value, typedParam, previousTypeInfo, previousConverter, previousTypeId, cancellationToken);
    }

    /// <summary>
    /// Writes a single column in the current row as type <paramref name="npgsqlDbType"/>.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <param name="npgsqlDbType">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
    /// the database. This parameter can be used to unambiguously specify the type. An example is
    /// the JSONB type, for which <typeparamref name="T"/> will be a simple string but for which
    /// <paramref name="npgsqlDbType"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
    public void Write<T>(T value, NpgsqlDbType npgsqlDbType) =>
        Write(async: false, value, npgsqlDbType).GetAwaiter().GetResult();

    /// <summary>
    /// Writes a single column in the current row as type <paramref name="npgsqlDbType"/>.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <param name="npgsqlDbType">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
    /// the database. This parameter can be used to unambiguously specify the type. An example is
    /// the JSONB type, for which <typeparamref name="T"/> will be a simple string but for which
    /// <paramref name="npgsqlDbType"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
    public Task WriteAsync<T>(T value, NpgsqlDbType npgsqlDbType, CancellationToken cancellationToken = default)
        => Write(async: true, value, npgsqlDbType, cancellationToken);

    Task Write<T>(bool async, T value, NpgsqlDbType npgsqlDbType, CancellationToken cancellationToken = default)
    {
        CheckColumnIndex();
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        // First row, create the parameter objects
        ref var p = ref _params[_column];
        if (p is not NpgsqlParameter<T> typedParam)
            typedParam = new NpgsqlParameter<T> { NpgsqlDbType = npgsqlDbType };

        // We only report previous values if anything actually changed, this saves some checks during the write.
        PgTypeInfo? previousTypeInfo = null;
        PgConverter? previousConverter = null;
        PgTypeId previousTypeId = default;
        if (p is not null && (p._npgsqlDbType != npgsqlDbType || p._dataTypeName is not null))
        {
            p.GetResolutionInfo(out previousTypeInfo, out previousConverter, out previousTypeId);
            if (ReferenceEquals(p, typedParam))
            {
                p.ResetDbType();
                p.NpgsqlDbType = npgsqlDbType;
            }
        }

        if (!ReferenceEquals(p, typedParam))
            p = typedParam;

        return Write(async, value, typedParam, previousTypeInfo, previousConverter, previousTypeId, cancellationToken);
    }

    /// <summary>
    /// Writes a single column in the current row as type <paramref name="dataTypeName"/>.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <param name="dataTypeName">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
    /// the database. This parameter and be used to unambiguously specify the type.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
    public void Write<T>(T value, string dataTypeName) =>
        Write(async: false, value, dataTypeName).GetAwaiter().GetResult();

    /// <summary>
    /// Writes a single column in the current row as type <paramref name="dataTypeName"/>.
    /// </summary>
    /// <param name="value">The value to be written</param>
    /// <param name="dataTypeName">
    /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
    /// the database. This parameter and be used to unambiguously specify the type.
    /// </param>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
    public Task WriteAsync<T>(T value, string dataTypeName, CancellationToken cancellationToken = default)
        => Write(async: true, value, dataTypeName, cancellationToken);

    Task Write<T>(bool async, T value, string dataTypeName, CancellationToken cancellationToken = default)
    {
        CheckColumnIndex();
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        // First row, create the parameter objects
        ref var p = ref _params[_column];
        if (p is not NpgsqlParameter<T> typedParam)
            typedParam = new NpgsqlParameter<T> { DataTypeName = dataTypeName };

        // We only report previous values if anything actually changed, this saves some checks during the write.
        PgTypeInfo? previousTypeInfo = null;
        PgConverter? previousConverter = null;
        PgTypeId previousTypeId = default;
        if (p is not null && (p._npgsqlDbType is not null || p._dataTypeName != dataTypeName))
        {
            p.GetResolutionInfo(out previousTypeInfo, out previousConverter, out previousTypeId);
            if (ReferenceEquals(p, typedParam))
            {
                p.ResetDbType();
                p.DataTypeName = dataTypeName;
            }
        }

        if (!ReferenceEquals(p, typedParam))
            p = typedParam;

        return Write(async, value, typedParam, previousTypeInfo, previousConverter, previousTypeId, cancellationToken);
    }

    async Task Write<T>(bool async, T value, NpgsqlParameter<T> param, PgTypeInfo? previousTypeInfo, PgConverter? previousConverter, PgTypeId previousTypeId, CancellationToken cancellationToken = default)
    {
        CheckReady();
        if (_column == -1)
            throw new InvalidOperationException("A row hasn't been started");

        // Statically map any DBNull value during importing, generic parameters when T = DBNull normally won't find any mapping.
        // Also allow null values for object typed parameters, parameters exclusively accept DBNull.Value when T = object.
        if (typeof(T) == typeof(DBNull) || (typeof(T) == typeof(object) && (value == null || value is DBNull)))
        {
            await WriteNull(async, cancellationToken).ConfigureAwait(false);
            return;
        }

        param.TypedValue = value;
        param.ResolveTypeInfo(_connector.SerializerOptions);

        if (previousTypeInfo is not null && previousConverter is not null && param.PgTypeId != previousTypeId)
        {
            var currentPgTypeId = param.PgTypeId;
            param.SetResolutionInfo(previousTypeInfo, previousConverter, previousTypeId);
            throw new InvalidOperationException($"Write for column {_column} resolves to a different PostgreSQL type: {currentPgTypeId} than the first row resolved to ({previousTypeId}). " +
                                                $"Please make sure to use clr types that resolve to the same PostgreSQL type across rows. " +
                                                $"Alternatively pass the same NpgsqlDbType or DataTypeName to ensure the PostgreSQL type ends up to be identical." );
        }

        param.Bind(out _, out _);
        try
        {
            await param.Write(async, _pgWriter.WithFlushMode(async ? FlushMode.NonBlocking : FlushMode.Blocking), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _connector.Break(ex);
            throw;
        }
        _column++;
    }

    /// <summary>
    /// Writes a single null column value.
    /// </summary>
    public void WriteNull() => WriteNull(false).GetAwaiter().GetResult();

    /// <summary>
    /// Writes a single null column value.
    /// </summary>
    public Task WriteNullAsync(CancellationToken cancellationToken = default) => WriteNull(async: true, cancellationToken);

    async Task WriteNull(bool async, CancellationToken cancellationToken = default)
    {
        CheckReady();
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();
        CheckColumnIndex();

        if (_buf.WriteSpaceLeft < 4)
            await _buf.Flush(async, cancellationToken).ConfigureAwait(false);

        _buf.WriteInt32(-1);
        _pgWriter.RefreshBuffer();
        _column++;
    }

    /// <summary>
    /// Writes an entire row of columns.
    /// Equivalent to calling <see cref="StartRow()"/>, followed by multiple <see cref="Write{T}(T)"/>
    /// on each value.
    /// </summary>
    /// <param name="values">An array of column values to be written as a single row</param>
    public void WriteRow(params object?[] values) => WriteRow(false, CancellationToken.None, values).GetAwaiter().GetResult();

    /// <summary>
    /// Writes an entire row of columns.
    /// Equivalent to calling <see cref="StartRow()"/>, followed by multiple <see cref="Write{T}(T)"/>
    /// on each value.
    /// </summary>
    /// <param name="cancellationToken">
    /// An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <param name="values">An array of column values to be written as a single row</param>
    public Task WriteRowAsync(CancellationToken cancellationToken = default, params object?[] values)
        => WriteRow(async: true, cancellationToken, values);

    async Task WriteRow(bool async, CancellationToken cancellationToken = default, params object?[] values)
    {
        await StartRow(async, cancellationToken).ConfigureAwait(false);
        foreach (var value in values)
            await Write(async, value, cancellationToken).ConfigureAwait(false);
    }

    void CheckColumnIndex()
    {
        if (_column is -1 || _column >= NumColumns)
            Throw();

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Throw()
        {
            if (_column is -1)
                throw new InvalidOperationException("A row hasn't been started");

            if (_column >= NumColumns)
                ThrowColumnMismatch();
        }
    }

    #endregion

    #region Commit / Cancel / Close / Dispose

    /// <summary>
    /// Completes the import operation. The writer is unusable after this operation.
    /// </summary>
    public ulong Complete() => Complete(false).GetAwaiter().GetResult();

    /// <summary>
    /// Completes the import operation. The writer is unusable after this operation.
    /// </summary>
    public ValueTask<ulong> CompleteAsync(CancellationToken cancellationToken = default) => Complete(async: true, cancellationToken);

    async ValueTask<ulong> Complete(bool async, CancellationToken cancellationToken = default)
    {
        CheckReady();

        using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);

        if (InMiddleOfRow)
        {
            await Cancel(async, cancellationToken).ConfigureAwait(false);
            throw new InvalidOperationException("Binary importer closed in the middle of a row, cancelling import.");
        }

        try
        {
            await WriteTrailer(async, cancellationToken).ConfigureAwait(false);
            await _buf.Flush(async, cancellationToken).ConfigureAwait(false);
            _buf.EndCopyMode();
            await _connector.WriteCopyDone(async, cancellationToken).ConfigureAwait(false);
            await _connector.Flush(async, cancellationToken).ConfigureAwait(false);
            var cmdComplete = Expect<CommandCompleteMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async).ConfigureAwait(false), _connector);
            _state = ImporterState.Committed;
            return cmdComplete.Rows;
        }
        catch
        {
            Cleanup();
            throw;
        }
    }

    void ICancelable.Cancel() => Close();

    async Task ICancelable.CancelAsync() => await CloseAsync().ConfigureAwait(false);

    /// <summary>
    /// <para>
    /// Terminates the ongoing binary import and puts the connection back into the idle state, where regular commands can be executed.
    /// </para>
    /// <para>
    /// Note that if <see cref="Complete()" /> hasn't been invoked before calling this, the import will be cancelled and all changes will
    /// be reverted.
    /// </para>
    /// </summary>
    public void Dispose() => Close();

    /// <summary>
    /// <para>
    /// Async terminates the ongoing binary import and puts the connection back into the idle state, where regular commands can be executed.
    /// </para>
    /// <para>
    /// Note that if <see cref="CompleteAsync" /> hasn't been invoked before calling this, the import will be cancelled and all changes will
    /// be reverted.
    /// </para>
    /// </summary>
    public ValueTask DisposeAsync() => CloseAsync(true);

    async Task Cancel(bool async, CancellationToken cancellationToken = default)
    {
        _state = ImporterState.Cancelled;
        _buf.Clear();
        _buf.EndCopyMode();
        await _connector.WriteCopyFail(async, cancellationToken).ConfigureAwait(false);
        await _connector.Flush(async, cancellationToken).ConfigureAwait(false);
        try
        {
            using var registration = _connector.StartNestedCancellableOperation(cancellationToken, attemptPgCancellation: false);
            var msg = await _connector.ReadMessage(async).ConfigureAwait(false);
            // The CopyFail should immediately trigger an exception from the read above.
            throw _connector.Break(
                new NpgsqlException("Expected ErrorResponse when cancelling COPY but got: " + msg.Code));
        }
        catch (PostgresException e)
        {
            if (e.SqlState != PostgresErrorCodes.QueryCanceled)
                throw;
        }
    }

    /// <summary>
    /// <para>
    /// Terminates the ongoing binary import and puts the connection back into the idle state, where regular commands can be executed.
    /// </para>
    /// <para>
    /// Note that if <see cref="Complete()" /> hasn't been invoked before calling this, the import will be cancelled and all changes will
    /// be reverted.
    /// </para>
    /// </summary>
    public void Close() => CloseAsync(async: false).GetAwaiter().GetResult();

    /// <summary>
    /// <para>
    /// Async terminates the ongoing binary import and puts the connection back into the idle state, where regular commands can be executed.
    /// </para>
    /// <para>
    /// Note that if <see cref="CompleteAsync" /> hasn't been invoked before calling this, the import will be cancelled and all changes will
    /// be reverted.
    /// </para>
    /// </summary>
    public ValueTask CloseAsync(CancellationToken cancellationToken = default) => CloseAsync(async: true, cancellationToken);

    async ValueTask CloseAsync(bool async, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        switch (_state)
        {
        case ImporterState.Disposed:
            return;
        case ImporterState.Ready:
            await Cancel(async, cancellationToken).ConfigureAwait(false);
            break;
        case ImporterState.Cancelled:
        case ImporterState.Committed:
            break;
        default:
            throw new Exception("Invalid state: " + _state);
        }

        Cleanup();
    }

#pragma warning disable CS8625
    void Cleanup()
    {
        if (_state == ImporterState.Disposed)
            return;
        var connector = _connector;

        LogMessages.BinaryCopyOperationCompleted(_copyLogger, _rowsImported, connector?.Id ?? -1);

        if (connector != null)
        {
            connector.EndUserAction();
            connector.CurrentCopyOperation = null;
            connector.Connection?.EndBindingScope(ConnectorBindingScope.Copy);
            _connector = null;
        }

        _buf = null;
        _state = ImporterState.Disposed;
    }
#pragma warning restore CS8625

    async Task WriteTrailer(bool async, CancellationToken cancellationToken = default)
    {
        if (_buf.WriteSpaceLeft < 2)
            await _buf.Flush(async, cancellationToken).ConfigureAwait(false);
        _buf.WriteInt16(-1);
    }

    void CheckReady()
    {
        if (_state is not ImporterState.Ready and var state)
            Throw(state);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Throw(ImporterState state)
        {
            switch (state)
            {
            case ImporterState.Disposed:
                throw new ObjectDisposedException(typeof(NpgsqlBinaryImporter).FullName, "The COPY operation has already ended.");
            case ImporterState.Cancelled:
                throw new InvalidOperationException("The COPY operation has already been cancelled.");
            case ImporterState.Committed:
                throw new InvalidOperationException("The COPY operation has already been committed.");
            default:
                throw new Exception("Invalid state: " + state);
            }
        }
    }

    #endregion

    #region Enums

    enum ImporterState
    {
        Ready,
        Committed,
        Cancelled,
        Disposed
    }

    #endregion Enums

    void ThrowColumnMismatch()
        => throw new InvalidOperationException($"The binary import operation was started with {NumColumns} column(s), but {_column + 1} value(s) were provided.");
}
