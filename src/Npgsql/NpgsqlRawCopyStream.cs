using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;

namespace Npgsql
{
    /// <summary>
    /// Provides an API for a raw binary COPY operation, a high-performance data import/export mechanism to
    /// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginRawBinaryCopy"/>
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public class NpgsqlRawCopyStream : Stream
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        NpgsqlBuffer _buf;

        bool _writingDataMsg;
        int _leftToReadInDataMsg;
        bool _isDisposed, _isConsumed;

        readonly bool _canRead;
        readonly bool _canWrite;

        internal bool IsBinary { get; private set; }

        public override bool CanWrite { get { return _canWrite; } }
        public override bool CanRead { get { return _canRead; } }

        /// <summary>
        /// The copy binary format header signature
        /// </summary>
        internal static readonly byte[] BinarySignature =
        {
            (byte)'P',(byte)'G',(byte)'C',(byte)'O',(byte)'P',(byte)'Y',
            (byte)'\n', 255, (byte)'\r', (byte)'\n', 0
        };

        #endregion

        #region Constructor

        internal NpgsqlRawCopyStream(NpgsqlConnector connector, string copyCommand)
        {
            _connector = connector;
            _buf = connector.Buffer;

            _connector.State = ConnectorState.Copy;
            _connector.SendSingleMessage(new QueryMessage(copyCommand));
            var msg = _connector.ReadSingleMessage();
            switch (msg.Code)
            {
                case BackendMessageCode.CopyInResponse:
                    var copyInResponse = (CopyInResponseMessage) msg;
                    IsBinary = copyInResponse.IsBinary;
                    _canWrite = true;
                    break;
                case BackendMessageCode.CopyOutResponse:
                    var copyOutResponse = (CopyOutResponseMessage) msg;
                    IsBinary = copyOutResponse.IsBinary;
                    _canRead = true;
                    break;
                default:
                    throw _connector.UnexpectedMessageReceived(msg.Code);
            }
        }

        #endregion

        #region Write

        public override void Write(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (!CanWrite)
                throw new InvalidOperationException("Stream not open for writing");

            if (count == 0) { return; }

            EnsureDataMessage();

            if (count <= _buf.WriteSpaceLeft)
            {
                _buf.WriteBytesSimple(buffer, offset, count);
                return;
            }

            // Buffer is too big. Write whatever will fit and flush.
            var written = _buf.WriteSpaceLeft;
            _buf.WriteBytesSimple(buffer, offset, _buf.WriteSpaceLeft);
            Flush();

            offset += written;
            count -= written;

            // If the remainder fits in a single buffer, no problem.
            if (count <= _buf.WriteSpaceLeft) {
                EnsureDataMessage();
                _buf.WriteBytesSimple(buffer, offset, count);
                return;
            }

            // Otherwise, write the CopyData header via our buffer and the remaining data directly to the socket
            _buf.WriteByte((byte)BackendMessageCode.CopyData);
            _buf.WriteInt32(count);
            _buf.Flush();
            _buf.Underlying.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            CheckDisposed();
            if (!_writingDataMsg) { return; }

            // Need to update the length for the CopyData about to be sent
            var pos = _buf.WritePosition;
            _buf.WritePosition = 1;
            _buf.WriteInt32(pos - 1);
            _buf.WritePosition = pos;
            _buf.Flush();
            _writingDataMsg = false;
        }

        void EnsureDataMessage()
        {
            if (_writingDataMsg) { return; }

            Contract.Assert(_buf.WritePosition == 0);
            _buf.WriteByte((byte)BackendMessageCode.CopyData);
            // Leave space for the message length
            _buf.WriteInt32(0);
            _writingDataMsg = true;
        }

        #endregion

        #region Read

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (!CanRead)
                throw new InvalidOperationException("Stream not open for reading");

            var totalRead = 0;
            do {
                if (_leftToReadInDataMsg == 0)
                {
                    if (_isConsumed) { return 0; }
                    var msg = _connector.ReadSingleMessage();
                    switch (msg.Code) {
                    case BackendMessageCode.CopyData:
                        _leftToReadInDataMsg = ((CopyDataMessage)msg).Length;
                        break;
                    case BackendMessageCode.CopyDone:
                        _connector.ReadExpecting<CommandCompleteMessage>();
                        _connector.ReadExpecting<ReadyForQueryMessage>();
                        _isConsumed = true;
                        goto done;
                    default:
                        throw _connector.UnexpectedMessageReceived(msg.Code);
                    }
                }

                var len = Math.Min(count, _leftToReadInDataMsg);
                _buf.ReadBytesSimple(buffer, offset, len);
                offset += len;
                count -= len;
                _leftToReadInDataMsg -= len;
                totalRead += len;
            } while (count > 0);

        done:
            return totalRead;
        }

        #endregion

        #region Cancel

        /// <summary>
        /// Cancels and terminates an ongoing operation. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
        {
            CheckDisposed();

            if (CanWrite)
            {
                _isDisposed = true;
                _buf.Clear();
                _connector.SendSingleMessage(new CopyFailMessage());
                try
                {
                    var msg = _connector.ReadSingleMessage();
                    // The CopyFail should immediately trigger an exception from the read above.
                    _connector.Break();
                    throw new Exception("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
                }
                catch (NpgsqlException e)
                {
                    if (e.Code == "57014") { return; }
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

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed || !disposing) { return; }

            if (CanWrite)
            {
                Flush();
                _connector.SendSingleMessage(CopyDoneMessage.Instance);
                _connector.ReadExpecting<CommandCompleteMessage>();
                _connector.ReadExpecting<ReadyForQueryMessage>();
            }
            else
            {
                if (!_isConsumed) {
                    _buf.Skip(_leftToReadInDataMsg);
                    _connector.SkipUntil(BackendMessageCode.ReadyForQuery);
                }
            }

            _connector.State = ConnectorState.Ready;

            _connector = null;
            _buf = null;
            _isDisposed = true;
        }

        void CheckDisposed()
        {
            if (_isDisposed) {
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
            }
        }

        #endregion

        #region Invariants

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(_isDisposed || (_connector != null && _buf != null));
            Contract.Invariant(CanRead || CanWrite);
            Contract.Invariant(_buf == null || _buf == _connector.Buffer);
        }

        #endregion

        #region Unsupported

        public override bool CanSeek { get { return false; } }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }

    /// <summary>
    /// Writer for a text import, initiated by <see cref="NpgsqlConnection.BeginTextImport"/>.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public class NpgsqlCopyTextWriter : StreamWriter
    {
        internal NpgsqlCopyTextWriter(NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw new Exception("Can't use a binary copy stream for text writing");
            Contract.EndContractBlock();
        }

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
        {
            ((NpgsqlRawCopyStream)BaseStream).Cancel();
        }
    }

    /// <summary>
    /// Reader for a text export, initiated by <see cref="NpgsqlConnection.BeginTextExport"/>.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public class NpgsqlCopyTextReader : StreamReader
    {
        internal NpgsqlCopyTextReader(NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw new Exception("Can't use a binary copy stream for text reading");
            Contract.EndContractBlock();
        }

        /// <summary>
        /// Cancels and terminates an ongoing import.
        /// </summary>
        public void Cancel()
        {
            ((NpgsqlRawCopyStream)BaseStream).Cancel();
        }
    }
}
