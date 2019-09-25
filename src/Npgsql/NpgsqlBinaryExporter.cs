﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql
{
    /// <summary>
    /// Provides an API for a binary COPY TO operation, a high-performance data export mechanism from
    /// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginBinaryExport"/>
    /// </summary>
    public sealed class NpgsqlBinaryExporter : ICancelable, IAsyncDisposable
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        NpgsqlReadBuffer _buf;
        ConnectorTypeMapper _typeMapper;
        bool _isConsumed, _isDisposed;
        int _leftToReadInDataMsg, _columnLen;

        short _column;

        /// <summary>
        /// The number of columns, as returned from the backend in the CopyInResponse.
        /// </summary>
        internal int NumColumns { get; }

        readonly NpgsqlTypeHandler?[] _typeHandlerCache;
        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlBinaryExporter));

        #endregion

        #region Construction / Initialization

        internal NpgsqlBinaryExporter(NpgsqlConnector connector, string copyToCommand)
        {
            _connector = connector;
            _buf = connector.ReadBuffer;
            _typeMapper = connector.TypeMapper;
            _columnLen = int.MinValue;   // Mark that the (first) column length hasn't been read yet
            _column = -1;

            _connector.WriteQuery(copyToCommand);
            _connector.Flush();

            CopyOutResponseMessage copyOutResponse;
            var msg = _connector.ReadMessage();
            switch (msg.Code)
            {
            case BackendMessageCode.CopyOutResponse:
                copyOutResponse = (CopyOutResponseMessage)msg;
                if (!copyOutResponse.IsBinary)
                {
                    _connector.Break();
                    throw new ArgumentException("copyToCommand triggered a text transfer, only binary is allowed", nameof(copyToCommand));
                }
                break;
            case BackendMessageCode.CompletedResponse:
                throw new InvalidOperationException(
                    "This API only supports import/export from the client, i.e. COPY commands containing TO/FROM STDIN. " +
                    "To import/export with files on your PostgreSQL machine, simply execute the command with ExecuteNonQuery. " +
                    "Note that your data has been successfully imported/exported.");
            default:
                throw _connector.UnexpectedMessageReceived(msg.Code);
            }

            NumColumns = copyOutResponse.NumColumns;
            _typeHandlerCache = new NpgsqlTypeHandler[NumColumns];
            ReadHeader();
        }

        void ReadHeader()
        {
            _leftToReadInDataMsg = Expect<CopyDataMessage>(_connector.ReadMessage(), _connector).Length;
            var headerLen = NpgsqlRawCopyStream.BinarySignature.Length + 4 + 4;
            _buf.Ensure(headerLen);

            if (NpgsqlRawCopyStream.BinarySignature.Any(t => _buf.ReadByte() != t))
                throw new NpgsqlException("Invalid COPY binary signature at beginning!");

            var flags = _buf.ReadInt32();
            if (flags != 0)
                throw new NotSupportedException("Unsupported flags in COPY operation (OID inclusion?)");

            _buf.ReadInt32();   // Header extensions, currently unused
            _leftToReadInDataMsg -= headerLen;
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
            if (cancellationToken.IsCancellationRequested)
                return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
            using (NoSynchronizationContextScope.Enter())
                return StartRow(true);
        }

        async ValueTask<int> StartRow(bool async)
        {
            CheckDisposed();
            if (_isConsumed)
                return -1;

            // The very first row (i.e. _column == -1) is included in the header's CopyData message.
            // Otherwise we need to read in a new CopyData row (the docs specify that there's a CopyData
            // message per row).
            if (_column == NumColumns)
                _leftToReadInDataMsg = Expect<CopyDataMessage>(await _connector.ReadMessage(async), _connector).Length;
            else if (_column != -1)
                throw new InvalidOperationException("Already in the middle of a row");

            await _buf.Ensure(2, async);
            _leftToReadInDataMsg -= 2;

            var numColumns = _buf.ReadInt16();
            if (numColumns == -1)
            {
                Debug.Assert(_leftToReadInDataMsg == 0);
                Expect<CopyDoneMessage>(await _connector.ReadMessage(async), _connector);
                Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
                Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
                _column = -1;
                _isConsumed = true;
                return -1;
            }

            Debug.Assert(numColumns == NumColumns);

            _column = 0;
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
        public T Read<T>() => Read<T>(false).GetAwaiter().GetResult();

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
            if (cancellationToken.IsCancellationRequested)
                return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
            using (NoSynchronizationContextScope.Enter())
                return Read<T>(true);
        }

        ValueTask<T> Read<T>(bool async)
        {
            CheckDisposed();
            if (_column == -1 || _column == NumColumns)
                throw new InvalidOperationException("Not reading a row");

            var type = typeof(T);
            var handler = _typeHandlerCache[_column];
            if (handler == null)
                handler = _typeHandlerCache[_column] = _typeMapper.GetByClrType(type);

            return DoRead<T>(handler, async);
        }

        /// <summary>
        /// Reads the current column, returns its value according to <paramref name="type"/> and
        /// moves ahead to the next column.
        /// If the column is null an exception is thrown.
        /// </summary>
        /// <param name="type">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type coming in from the
        /// database. This parameter and be used to unambiguously specify the type. An example is the JSONB
        /// type, for which <typeparamref name="T"/> will be a simple string but for which
        /// <paramref name="type"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be read.</typeparam>
        /// <returns>The value of the column</returns>
        public T Read<T>(NpgsqlDbType type) => Read<T>(type, false).GetAwaiter().GetResult();

        /// <summary>
        /// Reads the current column, returns its value according to <paramref name="type"/> and
        /// moves ahead to the next column.
        /// If the column is null an exception is thrown.
        /// </summary>
        /// <param name="type">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type coming in from the
        /// database. This parameter and be used to unambiguously specify the type. An example is the JSONB
        /// type, for which <typeparamref name="T"/> will be a simple string but for which
        /// <paramref name="type"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">The .NET type of the column to be read.</typeparam>
        /// <returns>The value of the column</returns>
        public ValueTask<T> ReadAsync<T>(NpgsqlDbType type, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
                return new ValueTask<T>(Task.FromCanceled<T>(cancellationToken));
            using (NoSynchronizationContextScope.Enter())
                return Read<T>(type, true);
        }

        ValueTask<T> Read<T>(NpgsqlDbType type, bool async)
        {
            CheckDisposed();
            if (_column == -1 || _column == NumColumns)
                throw new InvalidOperationException("Not reading a row");

            var handler = _typeHandlerCache[_column];
            if (handler == null)
                handler = _typeHandlerCache[_column] = _typeMapper.GetByNpgsqlDbType(type);

            return DoRead<T>(handler, async);
        }

        async ValueTask<T> DoRead<T>(NpgsqlTypeHandler handler, bool async)
        {
            try
            {
                await ReadColumnLenIfNeeded(async);

                if (_columnLen == -1)
                {
#pragma warning disable CS8653 // A default expression introduces a null value when 'T' is a non-nullable reference type.
                    // When T is a Nullable<T>, we support returning null
                    if (NullableHandler<T>.Exists)
                        return default;
#pragma warning restore CS8653
                    throw new InvalidCastException("Column is null");
                }

                // If we know the entire column is already in memory, use the code path without async
                var result = NullableHandler<T>.Exists
                    ? _columnLen <= _buf.ReadBytesLeft
                        ? NullableHandler<T>.Read(handler, _buf, _columnLen)
                        : await NullableHandler<T>.ReadAsync(handler, _buf, _columnLen, async)
                    : _columnLen <= _buf.ReadBytesLeft
                        ? handler.Read<T>(_buf, _columnLen)
                        : await handler.Read<T>(_buf, _columnLen, async);

                _leftToReadInDataMsg -= _columnLen;
                _columnLen = int.MinValue;   // Mark that the (next) column length hasn't been read yet
                _column++;
                return result;
            }
            catch
            {
                _connector.Break();
                Cleanup();
                throw;
            }
        }

        /// <summary>
        /// Returns whether the current column is null.
        /// </summary>
        public bool IsNull
        {
            get
            {
                ReadColumnLenIfNeeded(false).GetAwaiter().GetResult();
                return _columnLen == -1;
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
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return Skip(true);
        }

        async Task Skip(bool async)
        {
            await ReadColumnLenIfNeeded(async);
            if (_columnLen != -1)
                await _buf.Skip(_columnLen, async);

            _columnLen = int.MinValue;
            _column++;
        }

        #endregion

        #region Utilities

        async Task ReadColumnLenIfNeeded(bool async)
        {
            if (_columnLen == int.MinValue)
            {
                await _buf.Ensure(4, async);
                _columnLen = _buf.ReadInt32();
                _leftToReadInDataMsg -= 4;
            }
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
        }

        #endregion

        #region Cancel / Close / Dispose

        /// <summary>
        /// Cancels an ongoing export.
        /// </summary>
        public void Cancel() => _connector.CancelRequest();

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

            if (!_isConsumed)
            {
                // Finish the current CopyData message
                _buf.Skip(_leftToReadInDataMsg);
                // Read to the end
                _connector.SkipUntil(BackendMessageCode.CopyDone);
                Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
                Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
            }

            var connector = _connector;
            Cleanup();
            connector.EndUserAction();
        }

#pragma warning disable CS8625
        void Cleanup()
        {
            var connector = _connector;
            Log.Debug("COPY operation ended", connector?.Id ?? -1);

            if (connector != null)
            {
                connector.CurrentCopyOperation = null;
                _connector = null;
            }

            _typeMapper = null;
            _buf = null;
            _isDisposed = true;
        }
#pragma warning restore CS8625

        #endregion
    }
}
