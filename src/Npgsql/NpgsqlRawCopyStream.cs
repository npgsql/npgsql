using System;
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
    public sealed class NpgsqlRawCopyStream : NpgsqlSpanStream, ICancelable
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        NpgsqlReadBuffer _readBuf;
        NpgsqlWriteBuffer _writeBuf;

        int _leftToReadInDataMsg;
        bool _isConsumed;

        internal bool IsBinary { get; private set; }

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

        private protected override void WriteSpan(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length == 0)
                return;

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
            catch
            {
                _connector.Break();
                Cleanup();
                throw;
            }
        }

        private protected override async ValueTask WriteMemory(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
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
                await FlushAsync(default, true);

                if (buffer.Length <= _writeBuf.WriteSpaceLeft)
                {
                    _writeBuf.WriteBytes(buffer.Span);
                    return;
                }

                // Value is too big even after a flush - bypass the buffer and write directly.
                await _writeBuf.DirectWrite(buffer, true);
            }
            catch
            {
                _connector.Break();
                Cleanup();
                throw;
            }
            
        }

        private protected override Task FlushAsync(CancellationToken cancellationToken, bool async)
            => _writeBuf.Flush(async);

        #endregion

        #region Read
        private protected override int ReadSpan(Span<byte> span)
        {
            var count = ReadCore(span.Length, false).GetAwaiter().GetResult();
            if (count > 0)
                _readBuf.ReadBytes(span.Slice(0, count));
            return count;
        }

        private protected override async ValueTask<int> ReadMemory(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var count = await ReadCore(buffer.Length, true);
            if (count > 0)
                _readBuf.ReadBytes(buffer.Slice(0, count).Span);
            return count;
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
                IsDisposed = true;
                _writeBuf.EndCopyMode();
                _writeBuf.Clear();
                await _connector.WriteCopyFail(async);
                await _connector.Flush(async);
                try
                {
                    var msg = await _connector.ReadMessage(async);
                    // The CopyFail should immediately trigger an exception from the read above.
                    _connector.Break();
                    throw new NpgsqlException("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
                }
                catch (PostgresException e)
                {
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

        private protected override async ValueTask DisposeAsync(bool async)
        {
            try
            {
                if (CanWrite)
                {
                    await FlushAsync(default, async);
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
            _connector = null;
            _readBuf = null;
            _writeBuf = null;
            IsDisposed = true;
        }
#pragma warning restore CS8625

        private protected override void CheckDisposed()
        {
            if (IsDisposed) {
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
            }
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
            {
                connector.Break();
                throw new Exception("Can't use a binary copy stream for text writing");
            }
        }

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
        {
            ((NpgsqlRawCopyStream)BaseStream).Cancel();
        }

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
            {
                connector.Break();
                throw new Exception("Can't use a binary copy stream for text reading");
            }
        }

        /// <summary>
        /// Cancels and terminates an ongoing import.
        /// </summary>
        public void Cancel()
        {
            ((NpgsqlRawCopyStream)BaseStream).Cancel();
        }

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
