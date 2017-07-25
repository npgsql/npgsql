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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
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
    public sealed class NpgsqlBinaryImporter : ICancelable
    {
        #region Fields and Properties

        NpgsqlConnector _connector;
        WriteBuffer _buf;
        TypeHandlerRegistry _registry;
        LengthCache _lengthCache;
        bool _isDisposed;

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

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Construction / Initialization

        internal NpgsqlBinaryImporter(NpgsqlConnector connector, string copyFromCommand)
        {
            _connector = connector;
            _buf = connector.WriteBuffer;
            _registry = connector.TypeHandlerRegistry;
            _lengthCache = new LengthCache();
            _column = -1;
            _dummyParam = new NpgsqlParameter();

            try
            {
                _connector.SendQuery(copyFromCommand);

                // TODO: Failure will break the connection (e.g. if we get CopyOutResponse), handle more gracefully
                var copyInResponse = _connector.ReadExpecting<CopyInResponseMessage>();
                if (!copyInResponse.IsBinary)
                    throw new ArgumentException("copyFromCommand triggered a text transfer, only binary is allowed", nameof(copyFromCommand));
                NumColumns = copyInResponse.NumColumns;
                _buf.StartCopyMode();
                WriteHeader();
            }
            catch
            {
                _connector.Break();
                throw;
            }
        }

        void WriteHeader()
        {
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

            if (_column != -1 && _column != NumColumns)
                throw new InvalidOperationException("Row has already been started and must be finished");

            if (_buf.WriteSpaceLeft < 2)
                _buf.Flush();
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
            if (_column == -1)
                throw new InvalidOperationException("A row hasn't been started");

            if (value == null || typeof(T) == typeof(DBNull))
            {
                WriteNull();
                return;
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
            if (_column == -1)
                throw new InvalidOperationException("A row hasn't been started");

            if (value == null || typeof(T) == typeof(DBNull))
            {
                WriteNull();
                return;
            }

            var handler = _registry[type];
            DoWrite(handler, value);
        }

        void DoWrite<T>(TypeHandler handler, [CanBeNull] T value)
        {
            try
            {
                // We simulate the regular writing process with a validation/length calculation pass,
                // followed by a write pass
                _dummyParam.ConvertedValue = null;
                _lengthCache.Clear();
                handler.ValidateAndGetLength(value, ref _lengthCache, _dummyParam);
                _lengthCache.Rewind();
                handler.WriteWithLength(value, _buf, _lengthCache, _dummyParam, false, CancellationToken.None);
                _column++;
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
            if (_column == -1)
                throw new InvalidOperationException("A row hasn't been started");

            if (_buf.WriteSpaceLeft < 4)
                _buf.Flush();

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
            foreach (var value in values)
                Write(value);
        }

        #endregion

        #region Cancel / Close / Dispose

        /// <summary>
        /// Cancels and terminates an ongoing import. Any data already written will be discarded.
        /// </summary>
        public void Cancel()
        {
            _isDisposed = true;
            _buf.EndCopyMode();
            _connector.SendMessage(new CopyFailMessage());
            try {
                var msg = _connector.ReadMessage();
                // The CopyFail should immediately trigger an exception from the read above.
                _connector.Break();
                throw new NpgsqlException("Expected ErrorResponse when cancelling COPY but got: " + msg.Code);
            }
            catch (PostgresException e)
            {
                if (e.SqlState != "57014")
                    throw;
            }
            // Note that the exception has already ended the user action for us
            Cleanup();
        }

        /// <summary>
        /// Completes that binary import and sets the connection back to idle state
        /// </summary>
        public void Dispose() => Close();

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        [PublicAPI]
        public void Close()
        {
            if (_isDisposed)
                return;

            if (_column != -1 && _column != NumColumns)
            {
                Log.Error("Binary importer closed in the middle of a row, cancelling import.");
                _buf.Clear();
                Cancel();
                return;
            }

            WriteTrailer();
            _buf.Flush();
            _buf.EndCopyMode();

            var connector = _connector;
            connector.SendMessage(CopyDoneMessage.Instance);
            try
            {
                connector.ReadExpecting<CommandCompleteMessage>();
                connector.ReadExpecting<ReadyForQueryMessage>();
            }
            finally
            {
                Cleanup();
                connector.EndUserAction();
            }
        }

        void Cleanup()
        {
            Log.Debug("COPY operation ended", _connector.Id);
            _connector.CurrentCopyOperation = null;
            _connector = null;
            _registry = null;
            _buf = null;
            _isDisposed = true;
        }

        void WriteTrailer()
        {
            if (_buf.WriteSpaceLeft < 2)
                _buf.Flush();
            _buf.WriteInt16(-1);
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
        }

        #endregion
    }
}
