using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using NpgsqlTypes;

#pragma warning disable CA2222 // Do not decrease inherited member visibility

namespace Npgsql
{
    /// <summary>
    /// Provides a basic API for a replication operation. Initiated by <see cref="NpgsqlConnection.BeginReplication"/>.
    /// </summary>
    /// <remarks>See <a href="">https://www.postgresql.org/docs/current/static/protocol-replication.html</a>.</remarks>
    public class NpgsqlRawReplicationStream : Stream, ICancelable
    {
        static readonly DateTime ReplicationEraStart = new DateTime(2000, 1, 1);
        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        readonly object _writeSyncObject = new object();

        readonly NpgsqlConnector _connector;
        readonly ReadBuffer _buffer;

        // Backend messages
        readonly WalDataResponseMessage _walDataResponse;
        readonly PrimaryKeepAliveResponseMessage _primaryKeepAliveResponse;

        // Frontend messages
        readonly StandbyStatusUpdateMessage _standbyStatusUpdateRequest;

        int _bytesLeft;
        int _length;

        volatile bool _disposed;

        /// <inheritdoc />
        public bool CancellationRequired => true;

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanSeek => false;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override long Length => _length;

        /// <summary>
        /// The starting point of the WAL data in the last received message.
        /// </summary>
        [PublicAPI]
        public NpgsqlLsn CurrentLsn { get; private set; }

        /// <summary>
        /// The current end of WAL on the server.
        /// </summary>
        [PublicAPI]
        public NpgsqlLsn EndLsn { get; private set; }

        /// <summary>
        /// The server's system clock at the time of the last transmission, as microseconds since midnight on 2000-01-01.
        /// </summary>
        [PublicAPI]
        public long SystemClock { get; private set; }

        /// <inheritdoc />
        public override long Position
        {
            get => _length - _bytesLeft;
            set => Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// Indicates that the stream is reached the end and the server expecting next query.
        /// </summary>
        public bool EndOfStream { get; private set; }

        internal NpgsqlRawReplicationStream([NotNull] NpgsqlConnector connector, string replicationCommand)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            _connector = connector;
            _buffer = _connector.ReadBuffer;
            _walDataResponse = new WalDataResponseMessage();
            _primaryKeepAliveResponse = new PrimaryKeepAliveResponseMessage();
            _standbyStatusUpdateRequest = new StandbyStatusUpdateMessage();

            _connector.StartUserAction(ConnectorState.Replication);
            try
            {
                _connector.CurrentCancelableOperation = this;
                _connector.SendQuery(replicationCommand);
                _connector.ReadExpecting<CopyBothResponseMessage>();
            }
            catch
            {
                _connector.CurrentCancelableOperation = null;
                _connector.EndUserAction();
                throw;
            }
        }

        /// <summary>
        /// Fetches the next message from the underlying connection. Any data which are not read will be skipped.
        /// </summary>
        /// <returns><b>true</b> if there is at least one pending message. Otherwise <b>false</b>.</returns>
        /// <remarks>This method returns <b>false</b> if the <see cref="EndOfStream">end of the stream</see> is reached.</remarks>
        [PublicAPI]
        public bool FetchNext()
        {
            return FetchNext(false, false).Result;
        }

        /// <inheritdoc cref="FetchNext()"/> 
        [PublicAPI]
        public async ValueTask<bool> FetchNextAsync()
        {
            return await FetchNext(false, true);
        }
       
        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        async ValueTask<bool> FetchNext(bool waitCompletionConfirmation, bool async)
        {
            if (_bytesLeft > 0)
                await Skip(_bytesLeft, async);

            if (EndOfStream)
                return false;

            var dontWait = !waitCompletionConfirmation;
            var copyDone = false;
            while (true)
            {
                if (_bytesLeft < 0)
                    throw new InvalidOperationException($"Internal Npgsql bug: the state of {nameof(NpgsqlRawReplicationStream)} is corrupted. Please file a bug.");
                if (_bytesLeft == 0)
                {
                    if (dontWait && !_connector.CanReadMore())
                    {
                        // There are no more pending incoming messages.
                        return false;
                    }

                    CheckDisposed();
                    var msg = await _connector.ReadMessage(async);
                    if (msg == null)
                    {
                        if (dontWait)
                            return false;
                        continue;
                    }

                    switch (msg.Code)
                    {
                    case BackendMessageCode.CopyData:
                        _bytesLeft = ((CopyDataMessage)msg).Length;
                        _length = _bytesLeft;
                        break;
                    case BackendMessageCode.CopyDone:
                        copyDone = true;
                        dontWait = false;
                        continue;
                    case BackendMessageCode.CompletedResponse:
                        Debug.Assert(copyDone);
                        await _connector.ReadExpecting<CommandCompleteMessage>(async);
                        await _connector.ReadExpecting<ReadyForQueryMessage>(async);
                        
                        Debug.Assert(_connector.State == ConnectorState.Replication);
                        EndOfStream = true;
                        _connector.EndUserAction();
                        return false;
                    default:
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }
                Debug.Assert(_bytesLeft > 0);
                Debug.Assert(_buffer.ReadBytesLeft > 0);

                await _buffer.Ensure(1, async);
                var msgCode = (BackendMessageCode)_buffer.ReadByte();
                switch (msgCode)
                {
                case BackendMessageCode.WalData:
                    await _buffer.Ensure(_walDataResponse.MessageLength - 1, async);
                    _walDataResponse.Load(_buffer);
                    _bytesLeft -= _walDataResponse.MessageLength;
                    EndLsn = _walDataResponse.EndLsn;
                    SystemClock = _walDataResponse.SystemClock;
                    if (copyDone)
                    {
                        // CopyDone was received. Waiting for the completion confirmation.
                        await Skip(_bytesLeft, async);
                        continue;
                    }
                    CurrentLsn = _walDataResponse.StartLsn;
                    return true;

                case BackendMessageCode.PrimaryKeepAlive:
                    await _buffer.Ensure(_primaryKeepAliveResponse.MessageLength - 1, async);
                    _primaryKeepAliveResponse.Load(_buffer);
                    _bytesLeft -= _primaryKeepAliveResponse.MessageLength;
                    EndLsn = _primaryKeepAliveResponse.EndLsn;
                    SystemClock = _primaryKeepAliveResponse.SystemClock;
                    if (_primaryKeepAliveResponse.ReplyImmediately && !copyDone)
                    {
                        if (_standbyStatusUpdateRequest.SystemClock > 0)
                        {
                            // Repeat the last status update message.
                            Flush(_standbyStatusUpdateRequest.LastWrittenLsn, _standbyStatusUpdateRequest.LastFlushedLsn, _standbyStatusUpdateRequest.LastAppliedLsn);
                        }
                        // TODO: autoflush mode
                    }
                    Debug.Assert(_bytesLeft == 0);
                    continue;

                default:
                    throw _connector.UnexpectedMessageReceived(msgCode);
                }
            }
        }

        /// <summary>
        /// Confirms that the last message was recieved and the server may flush the replication slot.
        /// </summary>
        ///<remarks>
        /// The last received message is fully consumed if the <see cref="Position"/> equals to <see cref="Length"/>.
        /// </remarks>
        public override void Flush()
        {
            if (_walDataResponse.SystemClock == 0)
                return;

            var nextLsn = new NpgsqlLsn(CurrentLsn.Value + 1);
            Flush(nextLsn, nextLsn, nextLsn);
        }

        /// <summary>
        /// Confirms that the server may flush the replication slot.
        /// </summary>
        /// <param name="lastWrittenLsn">The location of the last WAL byte + 1 received and written to disk in the standby.</param>
        /// <param name="lastFlushedLsn">The location of the last WAL byte + 1 flushed to disk in the standby.</param>
        /// <param name="lastAppliedLsn">The location of the last WAL byte + 1 applied in the standby.</param>
        [PublicAPI]
        public void Flush(NpgsqlLsn lastWrittenLsn, NpgsqlLsn lastFlushedLsn, NpgsqlLsn lastAppliedLsn)
        {
            if (EndOfStream)
                return;

            CheckDisposed();

            _standbyStatusUpdateRequest.LastWrittenLsn = lastWrittenLsn;
            _standbyStatusUpdateRequest.LastFlushedLsn = lastFlushedLsn;
            _standbyStatusUpdateRequest.LastAppliedLsn = lastAppliedLsn;

            var ticks = (DateTime.Now - ReplicationEraStart).Ticks;
            _standbyStatusUpdateRequest.SystemClock = (long)(ticks / (TimeSpan.TicksPerMillisecond / 1000d));

            lock (_writeSyncObject)
            {
                CheckDisposed();
                _connector.SendMessage(_standbyStatusUpdateRequest);
            }
        }

        /// <inheritdoc />
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return await Read(buffer, offset, count, true);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer, offset, count, false).Result;
        }

        async ValueTask<int> Read(byte[] buffer, int offset, int count, bool async)
        {
            if (_bytesLeft <= 0)
                return 0;

            var bytesRead = await _buffer.ReadAllBytes(buffer, offset, Math.Min(_bytesLeft, count), false, async);
            _bytesLeft -= bytesRead;

            return bytesRead;
        }

        /// <summary>
        /// This stream can seek only forward.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            var position = Position;
            long newPosition;
            switch (origin)
            {
            case SeekOrigin.Begin:
                newPosition = offset;
                break;
            case SeekOrigin.Current:
                newPosition = position + offset;
                break;
            case SeekOrigin.End:
                newPosition = Length + offset;
                break;
            default:
                throw new ArgumentException($"The origin {origin} not supported.", nameof(origin));
            }
            if (newPosition < position)
                throw new NotSupportedException("The stream does not support backward seeking.");

            Skip((int)(newPosition - position), false).GetAwaiter().GetResult();
            return Position;
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("The stream is read-only.");
        }

        /// <summary>
        /// This method is not supported.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream is read-only.");
        }

        /// <summary>
        /// Send CopyDone request and wait for confirmation.
        /// </summary>
#if NET45 || NET451
        public override void Close()
#else
        [PublicAPI]
        public void Close()
#endif
        {
            Close(true);
        }

        void Close(bool waitForConfirmation)
        {
            if (_disposed)
                return;

            try
            {
                lock (_writeSyncObject)
                {
                    if (_disposed)
                        return;

                    _connector.SendMessage(CopyDoneMessage.Instance);
                    if (!waitForConfirmation)
                    {
                        if (_connector.State == ConnectorState.Replication)
                            _connector.EndUserAction();
                        Cleanup();
                        return;
                    }
                }

                while (FetchNext(true, false).Result)
                {
                    // Receiving and skipping all pending messages.
                }
                Cleanup();
            }
            catch
            {
                Cleanup();
            }
        }

        void Cleanup()
        {
            Log.Debug("START_REPLICATION operation ended", _connector.Id);
            _bytesLeft = 0;
            _length = 0;
            _disposed = true;
            _connector.CurrentCancelableOperation = null;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            Close(disposing);
        }

        void ICancelable.Cancel()
        {
            Close(false);
        }

        void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(NpgsqlRawReplicationStream));
        }

        async ValueTask<int> Skip(int bytes, bool async)
        {
            if (bytes < 0)
                throw new ArgumentException("The number of bytes is negative.", nameof(bytes));

            var result = Math.Min(bytes, _bytesLeft);
            var skipBytesLeft = result;
            while (skipBytesLeft > 0)
            {
                if (_buffer.ReadBytesLeft == 0)
                {
                    CheckDisposed();
                    await _buffer.ReadMore(async);
                }
                Debug.Assert(_buffer.ReadBytesLeft > 0);

                var skip = Math.Min(_buffer.ReadBytesLeft, skipBytesLeft);
                await _buffer.Skip(skip, async);
                skipBytesLeft -= skip;
                _bytesLeft -= skip;
            }

            return result;
        }
    }
}
