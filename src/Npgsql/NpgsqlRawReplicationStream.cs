using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql
{
    public class NpgsqlRawReplicationStream : Stream
    {
        readonly NpgsqlConnector _connector;
        readonly ReadBuffer _buffer;
        readonly WalDataResponseMessage _dataMessage;
        readonly PrimaryKeepAliveResponseMessage _keepAlive;

        int _bytesLeft;
        int _position;
        bool _isConsumed;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException("The length of the stream is unknown.");

        [PublicAPI]
        public NpgsqlLsn CurrentLsn { get; private set; }

        [PublicAPI]
        public NpgsqlLsn EndLsn { get; private set; }

        [PublicAPI]
        public long SystemClock { get; private set; }

        public override long Position
        {
            get => throw new NotSupportedException("The stream does not support seeking.");
            set => Seek(value, SeekOrigin.Begin);
        }

        internal NpgsqlRawReplicationStream([NotNull] NpgsqlConnector connector, string replicationCommand)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            _connector = connector;
            _buffer = _connector.ReadBuffer;
            _dataMessage = new WalDataResponseMessage(connector.Settings.ReplicationMode);
            _keepAlive = new PrimaryKeepAliveResponseMessage();

            _connector.SendQuery(replicationCommand);
            _isConsumed = true;
        }

        public bool FetchNext()
        {
            while (_bytesLeft > 0)
            {
                if (_buffer.ReadBytesLeft == 0)
                {
                    _buffer.ReadMore(false).GetAwaiter().GetResult();
                }
                Debug.Assert(_buffer.ReadBytesLeft > 0);

                var bytesSkip = Math.Min(_buffer.ReadBytesLeft, _bytesLeft);
                _buffer.Skip(bytesSkip);
                _bytesLeft -= bytesSkip;
            }

            if (_isConsumed)
            {
                _connector.ReadExpecting<CopyBothResponseMessage>();
                _isConsumed = false;
            }

            while (true)
            {
                if (_bytesLeft < 0)
                    throw new InvalidOperationException($"Internal Npgsql bug: the state of {nameof(NpgsqlRawReplicationStream)} is corrupted. Please file a bug.");
                if (_bytesLeft == 0)
                {
                    //if (_buffer.ReadBytesLeft == 0)
                    //{
                    //    var realStream = _buffer.Underlying;
                    //    _buffer.ReadMore(false).GetAwaiter().GetResult();
                    //    if (_buffer.ReadBytesLeft == 0)
                    //        return false;
                    //}

                    var msg = _connector.ReadMessage();
                    switch (msg.Code)
                    {
                    case BackendMessageCode.CopyData:
                        _bytesLeft = ((CopyDataMessage)msg).Length;
                        break;
                    case BackendMessageCode.CopyDone:
                        _isConsumed = true;
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
                    _dataMessage.Load(_buffer);
                    _bytesLeft -= _dataMessage.MessageLength;
                    _position = 0;
                    return true;
                case BackendMessageCode.PrimaryKeepAlive:
                    _keepAlive.Load(_buffer);
                    _bytesLeft -= _keepAlive.MessageLength;
                    Debug.Assert(_bytesLeft == 0);
                    continue;
                default:
                    throw _connector.UnexpectedMessageReceived(msgCode);
                }
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_bytesLeft <= 0)
                return 0;

            var bytesRead = _buffer.ReadAllBytes(buffer, offset, Math.Min(_bytesLeft, count), false, false).Result;
            _position += bytesRead;
            _bytesLeft -= bytesRead;

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("The stream does not support seeking.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("The stream is read-only.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("The stream is read-only.");
        }
    }
}
