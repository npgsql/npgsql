using System;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using NpgsqlTypes;

namespace Npgsql
{
    public class NpgsqlRawReplicationStream : Stream
    {
        readonly NpgsqlConnector _connector;
        readonly ReadBuffer _buffer;

        // Backend messages
        readonly WalDataResponseMessage _walDataResponse;

        readonly PrimaryKeepAliveResponseMessage _primaryKeepAliveResponse;

        // Frontend messages
        readonly StandbyStatusUpdateMessage _standbyStatusUpdateRequest;

        int _bytesLeft;
        int _length;

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
            _walDataResponse = new WalDataResponseMessage(connector.Settings.ReplicationMode);
            _primaryKeepAliveResponse = new PrimaryKeepAliveResponseMessage();
            _standbyStatusUpdateRequest = new StandbyStatusUpdateMessage();

            _connector.StartUserAction(ConnectorState.Replication);
            try
            {
                _connector.SendQuery(replicationCommand);
                _connector.ReadExpecting<CopyBothResponseMessage>();
            }
            catch
            {
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
                        // CopyDone was recieved. Waiting for the completion confirmation.
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
                        Flush();
                    Debug.Assert(_bytesLeft == 0);
                    continue;

                default:
                    throw _connector.UnexpectedMessageReceived(msgCode);
                }
            }
        }

        public override void Flush()
        {
            if (EndOfStream)
                return;

            // TODO: send keep alive
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


#if NET45 || NET451
        /// <inheritdoc />
        public override void Close()
#else
        protected override void Dispose(bool disposing)
        {
            Close();
            base.Dispose(disposing);
        }
        
        public void Close()
#endif
        {
            if (_connector.State != ConnectorState.Replication)
                return;

            _connector.SendMessage(CopyDoneMessage.Instance);
            while (FetchNext(true))
            {
                // Recieving and skipping all pending messages.
            }


#if NET45 || NET451
            base.Close();
#endif
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
