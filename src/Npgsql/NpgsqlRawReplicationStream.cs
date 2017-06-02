using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using NpgsqlTypes;

namespace Npgsql
{
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

        [PublicAPI]
        public NpgsqlLsn CurrentLsn { get; private set; }

        [PublicAPI]
        public NpgsqlLsn EndLsn { get; private set; }

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

        public bool FetchNext()
        {
            return FetchNext(false);
        }

        bool FetchNext(bool waitCompletionConfirmation)
        {
            if (_bytesLeft > 0)
                Skip(_bytesLeft);

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
                    var msg = _connector.ReadMessage();
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
                        _connector.ReadExpecting<CommandCompleteMessage>();
                        _connector.ReadExpecting<ReadyForQueryMessage>();
                        
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

                var msgCode = (BackendMessageCode)_buffer.ReadByte();
                switch (msgCode)
                {
                case BackendMessageCode.WalData:
                    _walDataResponse.Load(_buffer);
                    _bytesLeft -= _walDataResponse.MessageLength;
                    EndLsn = _walDataResponse.EndLsn;
                    SystemClock = _walDataResponse.SystemClock;
                    if (copyDone)
                    {
                        // CopyDone was received. Waiting for the completion confirmation.
                        Skip(_bytesLeft);
                        continue;
                    }
                    CurrentLsn = _walDataResponse.StartLsn;
                    return true;

                case BackendMessageCode.PrimaryKeepAlive:
                    _primaryKeepAliveResponse.Load(_buffer);
                    _bytesLeft -= _primaryKeepAliveResponse.MessageLength;
                    EndLsn = _primaryKeepAliveResponse.EndLsn;
                    SystemClock = _primaryKeepAliveResponse.SystemClock;
                    if (_primaryKeepAliveResponse.ReplyImmediately && !copyDone)
                    {
                        if (_standbyStatusUpdateRequest.SystemClock > 0)
                        {
                            // Repeating the last status update message.
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

        public override void Flush()
        {
            var nextLsn = new NpgsqlLsn(CurrentLsn.Value + 1);
            Flush(nextLsn, nextLsn, nextLsn);
        }

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

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_bytesLeft <= 0)
                return 0;

            var bytesRead = _buffer.ReadAllBytes(buffer, offset, Math.Min(_bytesLeft, count), false, false).Result;
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

            Skip((int)(newPosition - position));
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

                while (FetchNext(true))
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
            Log.Debug("REPLICATION operation ended", _connector.Id);
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

        int Skip(int bytes)
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
                    _buffer.ReadMore(false).GetAwaiter().GetResult();
                }
                Debug.Assert(_buffer.ReadBytesLeft > 0);

                var skip = Math.Min(_buffer.ReadBytesLeft, skipBytesLeft);
                _buffer.Skip(skip);
                skipBytesLeft -= skip;
                _bytesLeft -= skip;
            }

            return result;
        }
    }
}
