using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using static Npgsql.Statics;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Constructor

        private async Task SendQueryAsync(NpgsqlConnector connector, NpgsqlStatement statement, bool async)
        {
            await connector.ParseMessage
                .Populate(statement.SQL, statement.StatementName, statement.InputParameters, connector.TypeMapper)
                .Write(_writeBuf, async);
            var bind = connector.BindMessage;
            bind.Populate(statement.InputParameters, "", statement.StatementName);
            await connector.BindMessage.Write(_writeBuf, async);
            await connector.DescribeMessage
                .Populate(StatementOrPortal.Portal)
                .Write(_writeBuf, async);
            await ExecuteMessage.DefaultExecute.Write(_writeBuf, async);
            await SyncMessage.Instance.Write(_writeBuf, async);
            await _writeBuf.Flush(async);
        }

        private NpgsqlStatement ParseRawQuery(string copyCommand, NpgsqlParameterCollection parameters)
        {
            var statements = new List<NpgsqlStatement>();
            _connector.SqlParser.ParseRawQuery(copyCommand, _connector.UseConformantStrings, parameters, statements, deriveParameters: false);

            if (statements.Count < 0)
            {
                throw new ArgumentException("TODO - better message");
            }

            var copyStatement = statements[0];
            // TODO: Should we do this?
            //copyStatement.StatementType = StatementType.Copy;
            return copyStatement;
        }

        void ValidateParameters(NpgsqlParameterCollection parameters)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                var p = parameters[i];
                if (!p.IsInputDirection)
                    continue;
                p.Bind(_connector.TypeMapper);
                p.LengthCache?.Clear();
                p.ValidateAndGetLength();
            }
        }

        internal NpgsqlRawCopyStream(NpgsqlConnector connector, string copyCommand, NpgsqlParameterCollection parameters = null)
        {
            _connector = connector;
            _readBuf = connector.ReadBuffer;
            _writeBuf = connector.WriteBuffer;

            // TODO: Better API for managing the command + parameters.
            if (parameters == null)
            {
                parameters = new NpgsqlParameterCollection();
            }

            ValidateParameters(parameters);
            // TODO...
            var copyStatement = ParseRawQuery(copyCommand, parameters);

            // TODO: Could this be setup to be Async?
            var sendTask = SendQueryAsync(connector, copyStatement, async: false);

            // TODO: Copied from NpgsqlCommand.ExecuteDbDataReader
            // The following is a hack. It raises an exception if one was thrown in the first phases
            // of the send (i.e. in parts of the send that executed synchronously). Exceptions may
            // still happen later and aren't properly handled. See #1323.
            if (sendTask.IsFaulted)
                sendTask.GetAwaiter().GetResult();

            Expect<ParseCompleteMessage>(_connector.ReadMessage());
            Expect<BindCompleteMessage>(_connector.ReadMessage());

            // TODO: I believe this is because we sent the DescribeMessage... it's OK because it indicates we don't
            // have row information... makes sense when we're executing a COPY vs a normal DataReader.
            Expect<NoDataMessage>(_connector.ReadMessage());

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
                    // TODO: Anything we have to finish up here?
                    Expect<CommandCompleteMessage>(_connector.ReadMessage());
                    Expect<ReadyForQueryMessage>(_connector.ReadMessage());
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
                    Expect<CommandCompleteMessage>(_connector.ReadMessage());
                    Expect<ReadyForQueryMessage>(_connector.ReadMessage());
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

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
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
