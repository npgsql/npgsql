#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.FrontendMessages;
using Npgsql.Logging;
using NpgsqlTypes;
using static Npgsql.Statics;

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
        NpgsqlWriteBuffer _buf;

        ImporterState _state;

        /// <summary>
        /// The number of columns in the current (not-yet-written) row.
        /// </summary>
        short _column;

        /// <summary>
        /// The number of columns, as returned from the backend in the CopyInResponse.
        /// </summary>
        internal int NumColumns { get; }

        bool InMiddleOfRow => _column != -1 && _column != NumColumns;

        [ItemCanBeNull]
        readonly NpgsqlParameter[] _params;

        static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

        #endregion

        #region Construction / Initialization

        internal NpgsqlBinaryImporter(NpgsqlConnector connector, string copyFromCommand)
        {
            _connector = connector;
            _buf = connector.WriteBuffer;
            _column = -1;

            try
            {
                _connector.SendQuery(copyFromCommand);

                CopyInResponseMessage copyInResponse;
                var msg = _connector.ReadMessage();
                switch (msg.Code)
                {
                case BackendMessageCode.CopyInResponse:
                    copyInResponse = (CopyInResponseMessage)msg;
                    if (!copyInResponse.IsBinary)
                        throw new ArgumentException("copyFromCommand triggered a text transfer, only binary is allowed", nameof(copyFromCommand));
                    break;
                case BackendMessageCode.CompletedResponse:
                    throw new InvalidOperationException(
                        "This API only supports import/export from the client, i.e. COPY commands containing TO/FROM STDIN. " +
                        "To import/export with files on your PostgreSQL machine, simply execute the command with ExecuteNonQuery. " +
                        "Note that your data has been successfully imported/exported.");
                default:
                    throw _connector.UnexpectedMessageReceived(msg.Code);
                }

                NumColumns = copyInResponse.NumColumns;
                _params = new NpgsqlParameter[NumColumns];
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
            CheckReady();

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
            var p = _params[_column];
            if (p == null)
            {
                // First row, create the parameter objects
                _params[_column] = p = typeof(T) == typeof(object)
                    ? new NpgsqlParameter()
                    : new NpgsqlParameter<T>();
            }

            Write(value, p);
        }

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="npgsqlDbType"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="npgsqlDbType">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type. An example is
        /// the JSONB type, for which <typeparamref name="T"/> will be a simple string but for which
        /// <paramref name="npgsqlDbType"/> must be specified as <see cref="NpgsqlDbType.Jsonb"/>.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        public void Write<T>(T value, NpgsqlDbType npgsqlDbType)
        {
            var p = _params[_column];
            if (p == null)
            {
                // First row, create the parameter objects
                _params[_column] = p = typeof(T) == typeof(object)
                    ? new NpgsqlParameter()
                    : new NpgsqlParameter<T>();
                p.NpgsqlDbType = npgsqlDbType;
            }

            if (npgsqlDbType != p.NpgsqlDbType)
                throw new InvalidOperationException($"Can't change {nameof(p.NpgsqlDbType)} from {p.NpgsqlDbType} to {npgsqlDbType}");

            Write(value, p);
        }

        /// <summary>
        /// Writes a single column in the current row as type <paramref name="dataTypeName"/>.
        /// </summary>
        /// <param name="value">The value to be written</param>
        /// <param name="dataTypeName">
        /// In some cases <typeparamref name="T"/> isn't enough to infer the data type to be written to
        /// the database. This parameter and be used to unambiguously specify the type.
        /// </param>
        /// <typeparam name="T">The .NET type of the column to be written.</typeparam>
        public void Write<T>(T value, string dataTypeName)
        {
            var p = _params[_column];
            if (p == null)
            {
                // First row, create the parameter objects
                _params[_column] = p = typeof(T) == typeof(object)
                    ? new NpgsqlParameter()
                    : new NpgsqlParameter<T>();
                p.DataTypeName = dataTypeName;
            }

            //if (dataTypeName!= p.DataTypeName)
            //    throw new InvalidOperationException($"Can't change {nameof(p.DataTypeName)} from {p.DataTypeName} to {dataTypeName}");

            Write(value, p);
        }

        void Write<T>([CanBeNull] T value, NpgsqlParameter param)
        {
            CheckReady();
            if (_column == -1)
                throw new InvalidOperationException("A row hasn't been started");

            if (value == null || value is DBNull)
            {
                WriteNull();
                return;
            }

            if (typeof(T) == typeof(object))
            {
                param.Value = value;
            }
            else
            {
                if (!(param is NpgsqlParameter<T> typedParam))
                {
                    _params[_column] = typedParam = new NpgsqlParameter<T>();
                    typedParam.NpgsqlDbType = param.NpgsqlDbType;
                }
                typedParam.TypedValue = value;
            }
            param.ResolveHandler(_connector.TypeMapper);
            param.ValidateAndGetLength();
            param.LengthCache?.Rewind();
            param.WriteWithLength(_buf, false);
            param.LengthCache?.Clear();
            _column++;
        }

        /// <summary>
        /// Writes a single null column value.
        /// </summary>
        public void WriteNull()
        {
            CheckReady();
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

        #region Commit / Cancel / Close / Dispose

        /// <summary>
        /// Completes the import operation. The writer is unusable after this operation.
        /// </summary>
        public void Complete()
        {
            CheckReady();

            if (InMiddleOfRow)
            {
                Cancel();
                throw new InvalidOperationException("Binary importer closed in the middle of a row, cancelling import.");
            }

            try
            {
                WriteTrailer();
                _buf.Flush();
                _buf.EndCopyMode();

                _connector.SendMessage(CopyDoneMessage.Instance);
                Expect<CommandCompleteMessage>(_connector.ReadMessage());
                Expect<ReadyForQueryMessage>(_connector.ReadMessage());
                _state = ImporterState.Committed;
            }
            catch
            {
                // An exception here will have already broken the connection etc.
                Cleanup();
                throw;
            }
        }

        void ICancelable.Cancel() => Close();

        /// <summary>
        /// Completes that binary import and sets the connection back to idle state
        /// </summary>
        public void Dispose() => Close();

        void Cancel()
        {
            _state = ImporterState.Cancelled;
            _buf.Clear();
            _buf.EndCopyMode();
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
                if (e.SqlState != "57014")
                    throw;
            }
        }

        /// <summary>
        /// Completes the import process and signals to the database to write everything.
        /// </summary>
        [PublicAPI]
        public void Close()
        {
            switch (_state)
            {
            case ImporterState.Disposed:
                return;
            case ImporterState.Ready:
                Cancel();
                break;
            case ImporterState.Cancelled:
            case ImporterState.Committed:
                break;
            default:
                throw new Exception("Invalid state: " + _state);
            }

            var connector = _connector;
            Cleanup();
            connector.EndUserAction();
        }

        void Cleanup()
        {
            Log.Debug("COPY operation ended", _connector.Id);
            _connector.CurrentCopyOperation = null;
            _connector = null;
            _buf = null;
            _state = ImporterState.Disposed;
        }

        void WriteTrailer()
        {
            if (_buf.WriteSpaceLeft < 2)
                _buf.Flush();
            _buf.WriteInt16(-1);
        }

        void CheckReady()
        {
            switch (_state)
            {
            case ImporterState.Ready:
                return;
            case ImporterState.Disposed:
                throw new ObjectDisposedException(GetType().FullName, "The COPY operation has already ended.");
            case ImporterState.Cancelled:
                throw new InvalidOperationException("The COPY operation has already been cancelled.");
            case ImporterState.Committed:
                throw new InvalidOperationException("The COPY operation has already been committed.");
            default:
                throw new Exception("Invalid state: " + _state);
            }
        }

        #endregion

        #region Enums

        enum ImporterState
        {
            Ready,
            Committed,
            Cancelled,
            Disposed
        }

        #endregion Enums
    }
}
