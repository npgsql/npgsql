﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Logging;
using static Npgsql.Util.Statics;

#pragma warning disable 1591

namespace Npgsql
{
    /// <summary>
    /// Provides an API for a raw binary COPY operation, a high-performance data import/export mechanism to
    /// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginRawBinaryCopy"/>
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public sealed class NpgsqlRawCopyStream : Stream, ICancelable
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        NpgsqlReadBuffer _readBuf;
        NpgsqlWriteBuffer _writeBuf;

        int _leftToReadInDataMsg;
        bool _isDisposed, _isConsumed;

        readonly bool _canRead;
        readonly bool _canWrite;

        internal bool IsBinary { get; private set; }

        public override bool CanWrite => _canWrite;
        public override bool CanRead => _canRead;

        /// <summary>
        /// The copy binary format header signature
        /// </summary>
        internal static readonly byte[] BinarySignature =
        {
            (byte)'P',(byte)'G',(byte)'C',(byte)'O',(byte)'P',(byte)'Y',
            (byte)'\n', 255, (byte)'\r', (byte)'\n', 0
        };

        static readonly NpgsqlLogger Log = NpgsqlLogManager.CreateLogger(nameof(NpgsqlRawCopyStream));

        #endregion

        #region Constructor

        internal NpgsqlRawCopyStream(NpgsqlConnector connector, string copyCommand)
        {
            _connector = connector;
            _readBuf = connector.ReadBuffer;
            _writeBuf = connector.WriteBuffer;

            _connector.WriteQuery(copyCommand);
            _connector.Flush();

            var msg = _connector.ReadMessage();
            switch (msg.Code)
            {
            case BackendMessageCode.CopyInResponse:
                var copyInResponse = (CopyInResponseMessage) msg;
                IsBinary = copyInResponse.IsBinary;
                _canWrite = true;
                _writeBuf.StartCopyMode();
                break;
            case BackendMessageCode.CopyOutResponse:
                var copyOutResponse = (CopyOutResponseMessage) msg;
                IsBinary = copyOutResponse.IsBinary;
                _canRead = true;
                break;
            case BackendMessageCode.CompletedResponse:
                throw new InvalidOperationException(
                    "This API only supports import/export from the client, i.e. COPY commands containing TO/FROM STDIN. " +
                    "To import/export with files on your PostgreSQL machine, simply execute the command with ExecuteNonQuery. " +
                    "Note that your data has been successfully imported/exported.");
            default:
                throw _connector.UnexpectedMessageReceived(msg.Code);
            }
        }

        #endregion

        #region Write

        public override void Write(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            Write(new ReadOnlySpan<byte>(buffer, offset, count));
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            return WriteAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

#if !NET461 && !NETSTANDARD2_0
        public override void Write(ReadOnlySpan<byte> buffer)
#else
        public void Write(ReadOnlySpan<byte> buffer)
#endif
        {
            CheckDisposed();
            if (!CanWrite)
                throw new InvalidOperationException("Stream not open for writing");

            if (buffer.Length == 0) { return; }

            if (buffer.Length <= _writeBuf.WriteSpaceLeft)
            {
                _writeBuf.WriteBytes(buffer);
                return;
            }

            try
            {
                // Value is too big, flush.
                Flush();

                if (buffer.Length <= _writeBuf.WriteSpaceLeft)
                {
                    _writeBuf.WriteBytes(buffer);
                    return;
                }

                // Value is too big even after a flush - bypass the buffer and write directly.
                _writeBuf.DirectWrite(buffer);
            }
            catch (Exception e)
            {
                _connector.Break(e);
                Cleanup();
                throw;
            }
        }

#if !NET461 && !NETSTANDARD2_0
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
#else
        public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
#endif
        {
            CheckDisposed();
            if (!CanWrite)
                throw new InvalidOperationException("Stream not open for writing");
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return WriteAsyncInternal();

            async ValueTask WriteAsyncInternal()
            {
                if (buffer.Length == 0)
                    return;

                if (buffer.Length <= _writeBuf.WriteSpaceLeft)
                {
                    _writeBuf.WriteBytes(buffer.Span);
                    return;
                }

                try
                {
                    // Value is too big, flush.
                    await FlushAsync(true);

                    if (buffer.Length <= _writeBuf.WriteSpaceLeft)
                    {
                        _writeBuf.WriteBytes(buffer.Span);
                        return;
                    }

                    // Value is too big even after a flush - bypass the buffer and write directly.
                    await _writeBuf.DirectWrite(buffer, true);
                }
                catch (Exception e)
                {
                    _connector.Break(e);
                    Cleanup();
                    throw;
                }
            }
        }

        public override void Flush() => FlushAsync(false).GetAwaiter().GetResult();

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled(cancellationToken);
            using (NoSynchronizationContextScope.Enter())
                return FlushAsync(true);
        }

        Task FlushAsync(bool async)
        {
            CheckDisposed();
            return _writeBuf.Flush(async);
        }

        #endregion

        #region Read

        public override int Read(byte[] buffer, int offset, int count)
        {
            ValidateArguments(buffer, offset, count);
            return Read(new Span<byte>(buffer, offset, count));
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            ValidateArguments(buffer, offset, count);
            return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
        }

#if !NET461 && !NETSTANDARD2_0
        public override int Read(Span<byte> span)
#else
        public int Read(Span<byte> span)
#endif
        {
            CheckDisposed();
            if (!CanRead)
                throw new InvalidOperationException("Stream not open for reading");

            var count = ReadCore(span.Length, false).GetAwaiter().GetResult();
            if (count > 0)
                _readBuf.ReadBytes(span.Slice(0, count));
            return count;
        }

#if !NET461 && !NETSTANDARD2_0
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#else
        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken)
#endif
        {
            CheckDisposed();
            if (!CanRead)
                throw new InvalidOperationException("Stream not open for reading");
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return ReadAsyncInternal();

            async ValueTask<int> ReadAsyncInternal()
            {
                var count = await ReadCore(buffer.Length, true);
                if (count > 0)
                    _readBuf.ReadBytes(buffer.Slice(0, count).Span);
                return count;
            }
        }

        async ValueTask<int> ReadCore(int count, bool async)
        {
            if (_isConsumed)
                return 0;

            if (_leftToReadInDataMsg == 0)
            {
                IBackendMessage msg;
                try
                {
                    // We've consumed the current DataMessage (or haven't yet received the first),
                    // read the next message
                    msg = await _connector.ReadMessage(async);
                }
                catch
                {
                    Cleanup();
                    throw;
                }

                switch (msg.Code)
                {
                case BackendMessageCode.CopyData:
                    _leftToReadInDataMsg = ((CopyDataMessage)msg).Length;
                    break;
                case BackendMessageCode.CopyDone:
                    Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
                    Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
                    _isConsumed = true;
                    return 0;
                default:
                    throw _connector.UnexpectedMessageReceived(msg.Code);
                }
            }

            Debug.Assert(_leftToReadInDataMsg > 0);

            // If our buffer is empty, read in more. Otherwise return whatever is there, even if the
            // user asked for more (normal socket behavior)
            if (_readBuf.ReadBytesLeft == 0)
                await _readBuf.ReadMore(async);

            Debug.Assert(_readBuf.ReadBytesLeft > 0);

            var maxCount = Math.Min(_readBuf.ReadBytesLeft, _leftToReadInDataMsg);
            if (count > maxCount)
                count = maxCount;

            _leftToReadInDataMsg -= count;
            return count;
        }

        #endregion

        #region Cancel

        /// <summary>
        /// Cancels and terminates an ongoing operation. Any data already written will be discarded.
        /// </summary>
        public void Cancel() => Cancel(false).GetAwaiter().GetResult();

        /// <summary>
        /// Cancels and terminates an ongoing operation. Any data already written will be discarded.
        /// </summary>
        public Task CancelAsync()
        {
            using (NoSynchronizationContextScope.Enter())
               return Cancel(true);
        }

        async Task Cancel(bool async)
        {
            CheckDisposed();

            if (CanWrite)
            {
                _writeBuf.EndCopyMode();
                _writeBuf.Clear();
                await _connector.WriteCopyFail(async);
                await _connector.Flush(async);
                try
                {
                    var msg = await _connector.ReadMessage(async);
                    // The CopyFail should immediately trigger an exception from the read above.
                    throw _connector.Break(
                        new NpgsqlException("Expected ErrorResponse when cancelling COPY but got: " + msg.Code));
                }
                catch (PostgresException e)
                {
                    var connector = _connector;
                    Cleanup();
                    connector.EndUserAction();

                    if (e.SqlState == PostgresErrorCodes.QueryCanceled)
                        return;
                    throw;
                }
            }
            else
            {
                _connector.CancelRequest();
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing) => DisposeAsync(disposing, false).GetAwaiter().GetResult();

        async ValueTask DisposeAsync(bool disposing, bool async)
        {
            if (_isDisposed || !disposing) { return; }

            try
            {
                if (CanWrite)
                {
                    await FlushAsync(async);
                    _writeBuf.EndCopyMode();
                    await _connector.WriteCopyDone(async);
                    await _connector.Flush(async);
                    Expect<CommandCompleteMessage>(await _connector.ReadMessage(async), _connector);
                    Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async), _connector);
                }
                else
                {
                    if (!_isConsumed)
                    {
                        if (_leftToReadInDataMsg > 0)
                        {
                            await _readBuf.Skip(_leftToReadInDataMsg, async);
                        }
                        _connector.SkipUntil(BackendMessageCode.ReadyForQuery);
                    }
                }
            }
            finally
            {
                var connector = _connector;
                Cleanup();
                connector.EndUserAction();
            }
        }

#pragma warning disable CS8625
        void Cleanup()
        {
            Log.Debug("COPY operation ended", _connector.Id);
            _connector.CurrentCopyOperation = null;
            _connector.Connection!.EndBindingScope(ConnectorBindingScope.Copy);
            _connector = null;
            _readBuf = null;
            _writeBuf = null;
            _isDisposed = true;
        }
#pragma warning restore CS8625

        void CheckDisposed()
        {
            if (_isDisposed) {
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
            }
        }

        #endregion

        #region Unsupported

        public override bool CanSeek => false;

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        #endregion

        #region Input validation
        static void ValidateArguments(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (offset < 0)
                throw new ArgumentNullException(nameof(offset));
            if (count < 0)
                throw new ArgumentNullException(nameof(count));
            if (buffer.Length - offset < count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
        }
        #endregion
    }

    /// <summary>
    /// Writer for a text import, initiated by <see cref="NpgsqlConnection.BeginTextImport"/>.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public sealed class NpgsqlCopyTextWriter : StreamWriter, ICancelable
    {
        internal NpgsqlCopyTextWriter(NpgsqlConnector connector, NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw connector.Break(new Exception("Can't use a binary copy stream for text writing"));
        }

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
            => ((NpgsqlRawCopyStream)BaseStream).Cancel();

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public Task CancelAsync()
        {
            using (NoSynchronizationContextScope.Enter())
                return ((NpgsqlRawCopyStream)BaseStream).CancelAsync();
        }
    }

    /// <summary>
    /// Reader for a text export, initiated by <see cref="NpgsqlConnection.BeginTextExport"/>.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public sealed class NpgsqlCopyTextReader : StreamReader, ICancelable
    {
        internal NpgsqlCopyTextReader(NpgsqlConnector connector, NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw connector.Break(new Exception("Can't use a binary copy stream for text reading"));
        }

        /// <summary>
        /// Cancels and terminates an ongoing import.
        /// </summary>
        public void Cancel()
            => ((NpgsqlRawCopyStream)BaseStream).Cancel();

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public Task CancelAsync()
        {
            using (NoSynchronizationContextScope.Enter())
                return ((NpgsqlRawCopyStream)BaseStream).CancelAsync();
        }
    }
}
