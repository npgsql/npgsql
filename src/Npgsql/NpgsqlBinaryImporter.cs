#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
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
    public class NpgsqlBinaryImporter : IDisposable, ICancelable
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
        internal int NumColumns { get; }

        /// <summary>
        /// NpgsqlParameter instance needed in order to pass the <see cref="NpgsqlParameter.ConvertedValue"/> from
        /// the validation phase to the writing phase.
        /// </summary>
        readonly NpgsqlParameter _dummyParam;

        #endregion

        #region Construction / Initialization

        internal NpgsqlBinaryImporter(NpgsqlConnector connector, string copyFromCommand)
        {
            _connector = connector;
            _buf = connector.Buffer;
            _registry = connector.TypeHandlerRegistry;
            _lengthCache = new LengthCache();
            _column = -1;
            _dummyParam = new NpgsqlParameter();

            try
            {
                _connector.SendSingleMessage(new QueryMessage(copyFromCommand));

                // TODO: Failure will break the connection (e.g. if we get CopyOutResponse), handle more gracefully
                var copyInResponse = _connector.ReadExpecting<CopyInResponseMessage>();
                if (!copyInResponse.IsBinary)
                {
                    throw new ArgumentException("copyFromCommand triggered a text transfer, only binary is allowed", nameof(copyFromCommand));
                }
                NumColumns = copyInResponse.NumColumns;
                WriteHeader();

                // We will be sending CopyData messages from now on, deduct the header from the buffer's usable size
                _buf.UsableSize -= 5;
            }
            catch
            {
                _connector.Break();
                throw;
            }
        }

        void WriteHeader()
        {
            EnsureDataMessage();
            _buf.WriteBytes(NpgsqlRawCopyStream.BinarySignature, 0, NpgsqlRawCopyStream.BinarySignature.Length);
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
            CheckDisposed();

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

        void DoWrite<T>(TypeHandler handler, [CanBeNull] T value)
        {
            try
            {
                if (_buf.WriteSpaceLeft < 4)
                {
                    FlushAndStartDataMessage();
                }

                var asObject = (object) value; // TODO: Implement boxless writing in the future
                if (asObject == null)
                {
                    _buf.WriteInt32(-1);
                    _column++;
                    return;
                }

                _dummyParam.ConvertedValue = null;

                var asSimple = handler as ISimpleTypeHandler;
                if (asSimple != null)
                {
                    var len = asSimple.ValidateAndGetLength(asObject, _dummyParam);
                    _buf.WriteInt32(len);
                    if (_buf.WriteSpaceLeft < len)
                    {
                        Contract.Assume(_buf.Size >= len);
                        FlushAndStartDataMessage();
                    }
                    asSimple.Write(asObject, _buf, _dummyParam);
                    _column++;
                    return;
                }

                var asChunking = handler as IChunkingTypeHandler;
                if (asChunking != null)
                {
                    _lengthCache.Clear();
                    var len = asChunking.ValidateAndGetLength(asObject, ref _lengthCache, _dummyParam);
                    _buf.WriteInt32(len);

                    // If the type handler used the length cache, rewind it to skip the first position:
                    // it contains the entire value length which we already have in len.
                    if (_lengthCache.Position > 0)
                    {
                        _lengthCache.Rewind();
                        _lengthCache.Position++;
                    }

                    asChunking.PrepareWrite(asObject, _buf, _lengthCache, _dummyParam);
                    var directBuf = new DirectBuffer();
                    while (!asChunking.Write(ref directBuf))
                    {
                        Flush();

                        // The following is an optimization hack for writing large byte arrays without passing
                        // through our buffer
                        if (directBuf.Buffer != null)
                        {
                            len = directBuf.Size == 0 ? directBuf.Buffer.Length : directBuf.Size;
                            _buf.WritePosition = 1;
                            _buf.WriteInt32(len + 4);
                            _buf.Flush();
                            _writingDataMsg = false;
                            _buf.Underlying.Write(directBuf.Buffer, directBuf.Offset, len);
                            directBuf.Buffer = null;
                            directBuf.Size = 0;
                        }
                        EnsureDataMessage();
                    }
                    _column++;
                    return;
                }

                throw PGUtil.ThrowIfReached();
            }
            catch
            {
                _connector.Break();
                Cleanup();
                throw;
            }
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

            if (_buf.WriteSpaceLeft < 4) { FlushAndStartDataMessage(); }

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
                var msg = _connector.ReadSingleMessage(DataRowLoadingMode.NonSequential);
                // The CopyFail should immediately trigger an exception from the read above.
                _connector.Break();
                throw new Exception("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
            } catch (NpgsqlException e) {
                if (e.Code == "57014") { return; }
                throw;
            }
        }

        /// <summary>
        /// Completes that binary import and sets the connection back to idle state
        /// </summary>
        public void Dispose() { Close(); }

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        [PublicAPI]
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
            _connector.CurrentCopyOperation = null;
            _connector.EndUserAction();

            Cleanup();
        }

        void Cleanup()
        {
            _connector = null;
            _registry = null;
            _buf.UsableSize = _buf.Size;
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

        [ContractInvariantMethod]
        void ObjectInvariants()
        {
            Contract.Invariant(_isDisposed || _writingDataMsg);
        }

        #endregion
    }
}
