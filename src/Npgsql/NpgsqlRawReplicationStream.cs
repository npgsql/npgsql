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
using static Npgsql.Statics;

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
        readonly NpgsqlReadBuffer _buffer;

        // Backend messages
        readonly WalDataResponseMessage _walDataResponse;
        readonly PrimaryKeepAliveResponseMessage _primaryKeepAliveResponse;

        // Frontend messages
        readonly StandbyStatusUpdateMessage _standbyStatusUpdateRequest;

        int _bytesLeft;
        int _length;

        // For details about copy-in mode (data transfer to the server) and copy-out mode (data transfer from the server)
        // see https://www.postgresql.org/docs/current/static/protocol-flow.html#protocol-copy
        volatile bool _copyInMode;
        volatile bool _copyOutMode;

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
        /// The starting point of the WAL data in the last received message (or the starting point of the WAL if the message is not yet recieved).
        /// </summary>
        [PublicAPI]
        public NpgsqlLsn StartLsn { get; private set; }

        /// <summary>
        /// The current end of WAL on the server.
        /// </summary>
        [PublicAPI]
        public NpgsqlLsn? EndLsn { get; private set; }

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
        /// Indicates that the stream is reached the end and the server is ready to receive next query.
        /// </summary>
        public bool EndOfStream { get; private set; }

        internal NpgsqlRawReplicationStream([NotNull] NpgsqlConnector connector, string replicationCommand, NpgsqlLsn startLsn)
        {
            _connector = connector;
            _buffer = _connector.ReadBuffer;
            _walDataResponse = new WalDataResponseMessage();
            _primaryKeepAliveResponse = new PrimaryKeepAliveResponseMessage();
            StartLsn = startLsn;
            _standbyStatusUpdateRequest = new StandbyStatusUpdateMessage
            {
                LastAppliedLsn = startLsn,
                LastWrittenLsn = startLsn,
                LastFlushedLsn = startLsn
            };

            _connector.SendQuery(replicationCommand);
            Expect<CopyBothResponseMessage>(_connector.ReadMessage());
            _copyInMode = _copyOutMode = true;
        }

        /// <summary>
        /// Fetches the next message from the underlying connection. Any data which are not read will be skipped.
        /// </summary>
        /// <returns>The operation status code. See <see cref="NpgsqlReplicationStreamFetchStatus"/> for details.</returns>
        [PublicAPI]
        public NpgsqlReplicationStreamFetchStatus FetchNext()
        {
            return FetchNext(false, false).Result;
        }

        /// <inheritdoc cref="FetchNext()"/> 
        [PublicAPI]
        public async ValueTask<NpgsqlReplicationStreamFetchStatus> FetchNextAsync()
        {
            return await FetchNext(false, true);
        }
       
        [SuppressMessage("ReSharper", "InconsistentlySynchronizedField")]
        async ValueTask<NpgsqlReplicationStreamFetchStatus> FetchNext(bool waitForConfirmation, bool async)
        {
            if (_bytesLeft > 0)
                await Skip(_bytesLeft, async);

            if (EndOfStream)
                return NpgsqlReplicationStreamFetchStatus.Closed;

            var dontWait = !waitForConfirmation && _copyOutMode;
            var status = NpgsqlReplicationStreamFetchStatus.None;
            while (true)
            {
                if (_bytesLeft < 0)
                    throw new InvalidOperationException($"Internal Npgsql bug: the state of {nameof(NpgsqlRawReplicationStream)} is corrupted. Please file a bug.");
                if (_bytesLeft == 0)
                {
                    if (dontWait && !_connector.CanReadMore())
                    {
                        // There are no more pending incoming messages.
                        return status;
                    }

                    CheckDisposed();
                    IBackendMessage msg;
                    try
                    {
                        msg = await _connector.ReadMessage(async);
                    }
                    catch (PostgresException)
                    {
                        if (_connector.State != ConnectorState.Ready)
                            throw;
                        
                        if (!_disposed)
                        {
                            lock (_writeSyncObject)
                            {
                                if (!_disposed)
                                    Cleanup();
                            }
                        }
                        EndOfStream = true;
                        throw;
                    }
                    
                    if (msg == null)
                    {
                        if (dontWait)
                            return status;
                        continue;
                    }
                    status = NpgsqlReplicationStreamFetchStatus.None;

                    switch (msg.Code)
                    {
                        case BackendMessageCode.CopyData:
                            _bytesLeft = ((CopyDataMessage)msg).Length;
                            _length = _bytesLeft;
                            break;
                        case BackendMessageCode.CopyDone:
                            if (!_copyOutMode)
                            {
                                // Protocol violation. CopyDone received twice.
                                throw _connector.UnexpectedMessageReceived(msg.Code);
                            }
                            lock (_writeSyncObject)
                            {
                                _copyOutMode = false;
                                if (_copyInMode)
                                {
                                    _connector.SendMessage(CopyDoneMessage.Instance);
                                    _copyInMode = false;
                                }
                            }
                            dontWait = false;
                            continue;
                        case BackendMessageCode.CompletedResponse:
                            Expect<CommandCompleteMessage>(await _connector.ReadMessage(async));
                            Expect<ReadyForQueryMessage>(await _connector.ReadMessage(async));

                            lock (_writeSyncObject)
                            {
                                Debug.Assert(!_copyInMode && !_copyOutMode);

                                if (!_disposed)
                                {
                                    if (_connector.State == ConnectorState.Replication)
                                        _connector.EndUserAction();
                                    Cleanup();
                                }
                            }
                            EndOfStream = true;
                            return NpgsqlReplicationStreamFetchStatus.Closed;
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
                    if (!_copyOutMode)
                    {
                        // Protocol violation. WalData received after CopyDone.
                        throw _connector.UnexpectedMessageReceived(msgCode);
                    }

                    await _buffer.Ensure(_walDataResponse.MessageLength - 1, async);
                    _walDataResponse.Load(_buffer);
                    _bytesLeft -= _walDataResponse.MessageLength;
                    EndLsn = _walDataResponse.EndLsn;
                    SystemClock = _walDataResponse.SystemClock;
                    StartLsn = _walDataResponse.StartLsn;
                    return NpgsqlReplicationStreamFetchStatus.Data;

                case BackendMessageCode.PrimaryKeepAlive:
                    await _buffer.Ensure(_primaryKeepAliveResponse.MessageLength - 1, async);
                    _primaryKeepAliveResponse.Load(_buffer);
                    _bytesLeft -= _primaryKeepAliveResponse.MessageLength;
                    EndLsn = _primaryKeepAliveResponse.EndLsn;
                    SystemClock = _primaryKeepAliveResponse.SystemClock;
                    if (_primaryKeepAliveResponse.ReplyImmediately)
                    {
                        // Repeat the last status update message.
                        Flush(_standbyStatusUpdateRequest.LastWrittenLsn, _standbyStatusUpdateRequest.LastFlushedLsn, _standbyStatusUpdateRequest.LastAppliedLsn, false);
                    }
                    status = NpgsqlReplicationStreamFetchStatus.KeepAlive;
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
            if (Flush(false))
                return;
               
            CheckDisposed();
            throw new NpgsqlException("The stream is in copy-out mode.");
        }

        /// <summary>
        /// Confirms that the last message was recieved and the server may flush the replication slot.
        /// </summary>
        /// <param name="replyImmediately">If true, the client requests the server to reply to this message immediately. This can be used to ping the server, to test if the connection is still healthy.</param>
        /// <returns>Returns <b>false</b> if the stream is either closed or in copy-out mode. Otherwise returns <b>true</b>.</returns>
        [PublicAPI]
        public bool Flush(bool replyImmediately)
        {
            var lsn = StartLsn;
            return Flush(lsn, lsn, lsn, replyImmediately);
        }

        /// <summary>
        /// Confirms that the server may flush the replication slot.
        /// </summary>
        /// <param name="lastWrittenLsn">The location of the last WAL byte + 1 received and written to disk in the standby.</param>
        /// <param name="lastFlushedLsn">The location of the last WAL byte + 1 flushed to disk in the standby.</param>
        /// <param name="lastAppliedLsn">The location of the last WAL byte + 1 applied in the standby.</param>
        /// <param name="replyImmediately">If true, the client requests the server to reply to this message immediately. This can be used to ping the server, to test if the connection is still healthy.</param>
        /// <returns>Returns <b>false</b> if the stream is either closed or in copy-out mode. Otherwise returns <b>true</b>.</returns>
        [PublicAPI]
        public bool Flush(NpgsqlLsn lastWrittenLsn, NpgsqlLsn lastFlushedLsn, NpgsqlLsn lastAppliedLsn, bool replyImmediately)
        {
            if (_disposed || !_copyInMode)
                return false;
            
            lock (_writeSyncObject)
            {
                if (_disposed || !_copyInMode)
                    return false;

                SendStandbyStatusUpdateRequest(lastWrittenLsn, lastFlushedLsn, lastAppliedLsn, replyImmediately);
            }

            return true;
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

            // TODO: review this change, not sure about it
            var bytesRead = await _buffer.ReadBytes(buffer, offset, Math.Min(_bytesLeft, count), async);
            //var bytesRead = await _buffer.ReadAllBytes(buffer, offset, Math.Min(_bytesLeft, count), false, async);

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
        /// This method can be called only if <see cref="_writeSyncObject"/> is locked.
        /// </summary>
        void SendStandbyStatusUpdateRequest(NpgsqlLsn lastWrittenLsn, NpgsqlLsn lastFlushedLsn, NpgsqlLsn lastAppliedLsn, bool replyImmediately)
        {
            _standbyStatusUpdateRequest.LastWrittenLsn = lastWrittenLsn;
            _standbyStatusUpdateRequest.LastFlushedLsn = lastFlushedLsn;
            _standbyStatusUpdateRequest.LastAppliedLsn = lastAppliedLsn;
            _standbyStatusUpdateRequest.ReplyImmediately = replyImmediately;

            var ticks = (DateTime.Now - ReplicationEraStart).Ticks;
            _standbyStatusUpdateRequest.SystemClock = (long)(ticks / (TimeSpan.TicksPerMillisecond / 1000d));

            // CopyData message code...
            _connector.WriteBuffer.WriteByte((byte)BackendMessageCode.CopyData);
            // .. and the length of the message
            _connector.WriteBuffer.WriteInt32(_standbyStatusUpdateRequest.Length + 4);
            _connector.SendMessage(_standbyStatusUpdateRequest);
        }

        /// <summary>
        /// Send CopyDone request and wait for confirmation.
        /// </summary>
        [PublicAPI]
        public override void Close()
        {
            Close(true);
        }

        void Close(bool waitForConfirmation)
        {
            if (_disposed)
                return;

            lock (_writeSyncObject)
            {
                if (_disposed)
                    return;

                if (_connector.IsBroken)
                {
                    Cleanup();
                    return;
                }

                if (_copyInMode)
                {
                    _connector.SendMessage(CopyDoneMessage.Instance);
                    _copyInMode = false;
                }

                if (!waitForConfirmation)
                {
                    if (_connector.State == ConnectorState.Replication)
                        _connector.EndUserAction();
                    Cleanup();
                    return;
                }
            }

            while (FetchNext(true, false).Result != NpgsqlReplicationStreamFetchStatus.Closed)
            {
                // Receiving and skipping all pending messages.
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
            try
            {
                Close(true);
            }
            catch (Exception ex)
            {
                Log.Error("START_REPLICATION operation completed with an error", ex, _connector.Id);
                Cleanup();
            }
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

    /// <summary>
    /// Provides status codes for <see cref="NpgsqlRawReplicationStream.FetchNext()"/> and <see cref="NpgsqlRawReplicationStream.FetchNextAsync"/>.
    /// </summary>
#pragma warning disable CA1717 //CA1717 (only FlagsAttribute enums should have plural names) false positive detection for the word 'Status'.
    public enum NpgsqlReplicationStreamFetchStatus
#pragma warning restore CA1717
    {
        /// <summary>
        /// There are no pending messages in the underlying stream.
        /// </summary>
        None,

        /// <summary>
        /// The header of a message was successfuly fetched from the underlying stream.
        /// The replication stream is ready to read the message's content.
        /// </summary>
        Data,

        /// <summary>
        /// There are no pending WAL messages in the underlying stream, but the 'keep alive' message was received.
        /// Values of properties <see cref="NpgsqlRawReplicationStream.EndLsn"/> and <see cref="NpgsqlRawReplicationStream.SystemClock"/> were updated.
        /// </summary>
        KeepAlive,

        /// <summary>
        /// The underlying stream was closed.
        /// </summary>
        Closed,
    }
}
