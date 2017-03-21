#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;

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
        ReadBuffer _readBuf;
        WriteBuffer _writeBuf;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Constructor

        internal NpgsqlRawCopyStream(NpgsqlConnector connector, string copyCommand)
        {
            _connector = connector;
            _readBuf = connector.ReadBuffer;
            _writeBuf = connector.WriteBuffer;
            _connector.SendQuery(copyCommand);
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

            if (count <= _writeBuf.WriteSpaceLeft)
            {
                _writeBuf.WriteBytes(buffer, offset, count);
                return;
            }

            try {
                // Value is too big, flush.
                Flush();

                if (count <= _writeBuf.WriteSpaceLeft)
                {
                    _writeBuf.WriteBytes(buffer, offset, count);
                    return;
                }

                // Value is too big even after a flush - bypass the buffer and write directly.
                _writeBuf.DirectWrite(buffer, offset, count);
            } catch {
                _connector.Break();
                Cleanup();
                throw;
            }
        }

        public override void Flush()
        {
            CheckDisposed();
            _writeBuf.Flush();
        }

        #endregion

        #region Read

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            if (!CanRead)
                throw new InvalidOperationException("Stream not open for reading");

            if (_isConsumed) {
                return 0;
            }

            if (_leftToReadInDataMsg == 0)
            {
                // We've consumed the current DataMessage (or haven't yet received the first),
                // read the next message
                var msg = _connector.ReadMessage();
                switch (msg.Code) {
                case BackendMessageCode.CopyData:
                    _leftToReadInDataMsg = ((CopyDataMessage)msg).Length;
                    break;
                case BackendMessageCode.CopyDone:
                    _connector.ReadExpecting<CommandCompleteMessage>();
                    _connector.ReadExpecting<ReadyForQueryMessage>();
                    _isConsumed = true;
                    return 0;
                default:
                    throw _connector.UnexpectedMessageReceived(msg.Code);
                }
            }

            Debug.Assert(_leftToReadInDataMsg > 0);

            // If our buffer is empty, read in more. Otherwise return whatever is there, even if the
            // user asked for more (normal socket behavior)
            if (_readBuf.ReadBytesLeft == 0) {
                _readBuf.ReadMore(false).GetAwaiter().GetResult();
            }

            Debug.Assert(_readBuf.ReadBytesLeft > 0);

            var maxCount = Math.Min(_readBuf.ReadBytesLeft, _leftToReadInDataMsg);
            if (count > maxCount) {
                count = maxCount;
            }

            _leftToReadInDataMsg -= count;
            _readBuf.ReadBytes(buffer, offset, count);
            return count;
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
                _writeBuf.EndCopyMode();
                _writeBuf.Clear();
                _connector.SendMessage(new CopyFailMessage());
                try
                {
                    var msg = _connector.ReadMessage();
                    // The CopyFail should immediately trigger an exception from the read above.
                    _connector.Break();
                    throw new NpgsqlException("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
                }
                catch (PostgresException e)
                {
                    if (e.SqlState == "57014") { return; }
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

            try
            {
                if (CanWrite)
                {
                    Flush();
                    _writeBuf.EndCopyMode();
                    _connector.SendMessage(CopyDoneMessage.Instance);
                    _connector.ReadExpecting<CommandCompleteMessage>();
                    _connector.ReadExpecting<ReadyForQueryMessage>();
                }
                else
                {
                    if (!_isConsumed)
                    {
                        if (_leftToReadInDataMsg > 0)
                        {
                            _readBuf.Skip(_leftToReadInDataMsg);
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

        void Cleanup()
        {
            Log.Debug("COPY operation ended", _connector.Id);
            _connector.CurrentCopyOperation = null;
            _connector = null;
            _readBuf = null;
            _writeBuf = null;
            _isDisposed = true;
        }

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
    public sealed class NpgsqlCopyTextWriter : StreamWriter, ICancelable
    {
        internal NpgsqlCopyTextWriter(NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw new Exception("Can't use a binary copy stream for text writing");
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
    public sealed class NpgsqlCopyTextReader : StreamReader, ICancelable
    {
        internal NpgsqlCopyTextReader(NpgsqlRawCopyStream underlying) : base(underlying)
        {
            if (underlying.IsBinary)
                throw new Exception("Can't use a binary copy stream for text reading");
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
