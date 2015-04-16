using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using NpgsqlTypes;

namespace Npgsql
{
    /// <summary>
    /// Provides an API for a binary COPY FROM operation, a high-performance data import mechanism to
    /// a PostgreSQL table. Initiated by <see cref="NpgsqlConnection.BeginBinaryImport"/>
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/sql-copy.html.
    /// </remarks>
    public class NpgsqlBinaryImporter : IDisposable
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        NpgsqlBuffer _buf;
        TypeHandlerRegistry _registry;
        LengthCache _lengthCache;
        bool _isDisposed;
        bool _writingDataMsg;

        /// <summary>
        /// The number of columns in the current (not-yet-written) row.
        /// </summary>
        short _column;

        /// <summary>
        /// The number of columns, as returned from the backend in the CopyInResponse.
        /// </summary>
        internal int NumColumns { get; private set; }

        #endregion

        #region Construction / Initialization

        internal NpgsqlBinaryImporter(NpgsqlConnector connector, string copyFromCommand)
        {
            _connector = connector;
            _buf = connector.Buffer;
            _registry = connector.TypeHandlerRegistry;
            _lengthCache = new LengthCache();
            _column = -1;

            _connector.State = ConnectorState.Copy;
            _connector.SendSingleMessage(new QueryMessage(copyFromCommand));

            // TODO: Failure will break the connection (e.g. if we get CopyOutResponse), handle more gracefully
            var copyInResponse = _connector.ReadExpecting<CopyInResponseMessage>();
            if (!copyInResponse.IsBinary) {
                _connector.Break();
                throw new ArgumentException("copyFromCommand triggered a text transfer, only binary is allowed", "copyFromCommand");
            }
            NumColumns = copyInResponse.NumColumns;
            WriteHeader();
        }

        void WriteHeader()
        {
            EnsureDataMessage();
            _buf.WriteBytesSimple(NpgsqlRawCopyStream.BinarySignature);
            _buf.WriteInt32(0);   // Flags field. OID inclusion not supported at the moment.
            _buf.WriteInt32(0);   // Header extension area length
        }

        #endregion

        #region Write

        /// <summary>
        /// Starts writing a single row, must be invoked before writing any columns.
        /// </summary>
        public void StartRow()
        {
            if (_column != -1 && _column != NumColumns) {
                throw new InvalidOperationException("Row has already been started and must be finished");
            }

            if (_buf.WriteSpaceLeft < 2) { FlushAndStartDataMessage(); }
            _buf.WriteInt16(NumColumns);

            _column = 0;
        }

        /// <summary>
        /// Writes a single column in the current row.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <typeparam name="T">
        /// The type of the column to be written. This must correspond to the actual type or data
        /// corruption will occur. If in doubt, use <see cref="Write{T}(T, NpgsqlDbType)"/> to manually
        /// specify the type.
        /// </typeparam>
        public void Write<T>(T value)
        {
            CheckDisposed();
            if (_column == -1) {
                throw new InvalidOperationException("A row hasn't been started");
            }

            var handler = _registry[value];
            DoWrite(handler, value);
        }

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="type"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="type">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type. An example is
        /// the JSONB type, for which <typeparamref name="T"/> will be a simple string but for which
        /// <paramref name="type"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        public void Write<T>(T value, NpgsqlDbType type)
        {
            CheckDisposed();
            if (_column == -1) {
                throw new InvalidOperationException("A row hasn't been started");
            }

            var handler = _registry[type];
            DoWrite(handler, value);
        }

        void DoWrite<T>(TypeHandler handler, T value)
        {
            if (_buf.WriteSpaceLeft < 4) { Flush(); }
            EnsureDataMessage();

            var asObject = (object)value;   // TODO: Implement boxless writing in the future
            if (asObject == null) {
                _buf.WriteInt32(-1);
                _column++;
                return;
            }

            var asSimple = handler as ISimpleTypeWriter;
            if (asSimple != null) {
                var len = asSimple.ValidateAndGetLength(asObject);
                _buf.WriteInt32(len);
                if (_buf.WriteSpaceLeft < len) {
                    Contract.Assume(_buf.Size >= len);
                    FlushAndStartDataMessage();
                }
                asSimple.Write(asObject, _buf);
                _column++;
                return;
            }

            var asChunking = handler as IChunkingTypeWriter;
            if (asChunking != null) {
                _lengthCache.Clear();
                var len = asChunking.ValidateAndGetLength(asObject, ref _lengthCache);
                _buf.WriteInt32(len);
                _lengthCache.Rewind();
                _lengthCache.Get();  // Hack
                asChunking.PrepareWrite(asObject, _buf, _lengthCache);
                var directBuf = new DirectBuffer();
                while (!asChunking.Write(ref directBuf)) {
                    FlushAndStartDataMessage();

                    // The following is an optimization hack for writing large byte arrays without passing
                    // through our buffer
                    if (directBuf.Buffer != null) {
                        len = directBuf.Size == 0 ? directBuf.Buffer.Length : directBuf.Size;
                        _buf.WriteInt32(len);
                        Flush();
                        _buf.Underlying.Write(directBuf.Buffer, directBuf.Offset, len);
                        directBuf.Buffer = null;
                        directBuf.Size = 0;
                    }
                }
                _column++;
                return;
            }

            throw PGUtil.ThrowIfReached();
        }

        /// <summary>
        /// Writes a single null column value.
        /// </summary>
        public void WriteNull()
        {
            CheckDisposed();
            if (_column == -1) {
                throw new InvalidOperationException("A row hasn't been started");
            }

            if (_buf.WriteSpaceLeft < 4) { Flush(); }
            EnsureDataMessage();

            _buf.WriteInt32(-1);
            _column++;
        }

        /// <summary>
        /// Writes an entire row of columns.
        /// Equivalent to calling <see cref="StartRow"/>, followed by multiple <see cref="Write{T}(T)"/>
        /// on each value.
        /// </summary>
        /// <param name="values">An array of column values to be written as a single row</param>
        public void WriteRow(params object[] values)
        {
            StartRow();
            foreach (var value in values) {
                Write(value);
            }
        }

        void FlushAndStartDataMessage()
        {
            Flush();
            EnsureDataMessage();
        }

        void Flush()
        {
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

        #region Cancel / Close / Dispose

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
        {
            _isDisposed = true;
            _buf.Clear();
            _connector.SendSingleMessage(new CopyFailMessage());
            try {
                var msg = _connector.ReadSingleMessage();
                // The CopyFail should immediately trigger an exception from the read above.
                _connector.Break();
                throw new Exception("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
            } catch (NpgsqlException e) {
                if (e.Code == "57014") { return; }
                throw;
            }
        }

        public void Dispose() { Close(); }

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        public void Close()
        {
            if (_isDisposed) { return; }

            if (_column != -1 && _column != NumColumns) {
                throw new InvalidOperationException("Can't close writer, a row is still in progress, end it first");
            }
            WriteTrailer();

            _connector.SendSingleMessage(CopyDoneMessage.Instance);
            _connector.ReadExpecting<CommandCompleteMessage>();
            _connector.ReadExpecting<ReadyForQueryMessage>();

            _connector.State = ConnectorState.Ready;

            _connector = null;
            _registry = null;
            _buf = null;
            _isDisposed = true;
        }

        void WriteTrailer()
        {
            if (_buf.WriteSpaceLeft < 2) { FlushAndStartDataMessage(); }
            _buf.WriteInt16(-1);
            Flush();
        }

        void CheckDisposed()
        {
            if (_isDisposed) {
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
            }
        }

        #endregion
    }
}
