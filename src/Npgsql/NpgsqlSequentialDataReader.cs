﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

namespace Npgsql
{
    /// <summary>
    /// A sequential reader, which does not buffer rows in memory, and requires columns to be
    /// read in-order only. Returned when <see cref="CommandBehavior.SequentialAccess"/> is passed
    /// to <see cref="NpgsqlCommand.ExecuteReader()"/>.
    /// </summary>
    /// <remarks>
    /// This reader is suitable in scenarios where a single row is very large, and holding
    /// it in memory is undesirable.
    /// </remarks>
#pragma warning disable CA1010
    sealed class NpgsqlSequentialDataReader : NpgsqlDataReader
#pragma warning restore CA1010
    {
        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        int _numColumns;

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1
        /// </summary>
        int _column;

        /// <summary>
        /// A stream that has been opened on this colun, and needs to be disposed of when the column is consumed.
        /// </summary>
        [CanBeNull]
        SequentialByteaStream _stream;

        internal NpgsqlSequentialDataReader(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements, Task sendTask)
            : base(command, behavior, statements, sendTask)
        {
            Debug.Assert(!command.Parameters.HasOutputParameters);
            Buffer = Connector.ReadBuffer;
        }

        internal override ValueTask<IBackendMessage> ReadMessage(bool async)
            => Connector.ReadMessage(async, DataRowLoadingMode.Sequential);

        internal override void ProcessDataMessage(DataRowMessage dataMsg)
        {
            _column = -1;
            ColumnLen = -1;
            PosInColumn = 0;
        }

        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return GetFieldValue<T>(ordinal, true).AsTask();
        }

        public override T GetFieldValue<T>(int column)
            => GetFieldValue<T>(column, false).GetAwaiter().GetResult();

        async ValueTask<T> GetFieldValue<T>(int column, bool async)
        {
            CheckRowAndOrdinal(column);

            await SeekToColumn(column, async);
            CheckColumnStart();
            GetValueHandlers<T>(out var read, out var readAsync, out var canHandleNulls);
            if (ColumnLen == -1)
            {
                if (canHandleNulls)
                    return default;
                throw new InvalidCastException("Column is null");
            }

            var fieldDescription = RowDescription[column];
            try
            {
                return ColumnLen <= Buffer.ReadBytesLeft
                    ? read(fieldDescription, Buffer, ColumnLen)
                    : await readAsync(fieldDescription, Buffer, ColumnLen, async);
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="column">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int column)
        {
            CheckRowAndOrdinal(column);

            SeekToColumn(column, false).GetAwaiter().GetResult();
            CheckColumnStart();
            if (ColumnLen == -1)
                return DBNull.Value;

            var fieldDescription = RowDescription[column];
            object result;
            try
            {
                result = fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, false, fieldDescription)
                    .GetAwaiter().GetResult();
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }

            // Used for Entity Framework <= 6 compability
            if (Command.ObjectResultTypes?[column] != null)
            {
                var type = Command.ObjectResultTypes[column];
                result = type == typeof(DateTimeOffset)
                    ? new DateTimeOffset((DateTime)result)
                    : Convert.ChangeType(result, type);
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="column">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int column)
        {
            CheckRowAndOrdinal(column);

            SeekToColumn(column, false).GetAwaiter().GetResult();
            CheckColumnStart();
            if (ColumnLen == -1)
                return DBNull.Value;

            var fieldDescription = RowDescription[column];
            try
            {
                return fieldDescription.Handler.ReadPsvAsObject(Buffer, ColumnLen, false, fieldDescription)
                    .GetAwaiter().GetResult();
            }
            catch (NpgsqlSafeReadException e)
            {
                throw e.InnerException;
            }
            catch
            {
                Connector.Break();
                throw;
            }
            finally
            {
                // Important in case a NpgsqlSafeReadException was thrown, position must still be updated
                PosInColumn += ColumnLen;
            }
        }

        /// <summary>
        /// Seeks to the given column. The 4-byte length is read and stored in <see cref="NpgsqlDataReader.ColumnLen"/>.
        /// </summary>
        internal override async Task SeekToColumn(int column, bool async)
        {
            if (_column == -1)
            {
                await Buffer.Ensure(2, async);
                _numColumns = Buffer.ReadInt16();
            }

            if (column < 0 || column >= _numColumns)
                throw new IndexOutOfRangeException("Column index out of range");

            if (column < _column)
                throw new InvalidOperationException($"Invalid attempt to read from column ordinal '{column}'. With CommandBehavior.SequentialAccess, you may only read from column ordinal '{_column}' or greater.");

            if (column > _column)
            {
                // Skip to end of column if needed
                // TODO: Simplify by better initializing _columnLen/_posInColumn
                var remainingInColumn = ColumnLen == -1 ? 0 : ColumnLen - PosInColumn;
                if (remainingInColumn > 0)
                    await Buffer.Skip(remainingInColumn, async);

                // Shut down any streaming going on on the column
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                // Skip over unwanted fields
                for (; _column < column - 1; _column++)
                {
                    await Buffer.Ensure(4, async);
                    var len = Buffer.ReadInt32();
                    if (len != -1)
                        await Buffer.Skip(len, async);
                }

                await Buffer.Ensure(4, async);
                ColumnLen = Buffer.ReadInt32();
                PosInColumn = 0;
                _column = column;
            }
        }

        internal override async Task SeekInColumn(int posInColumn, bool async)
        {
            Debug.Assert(_column > -1);

            if (posInColumn < PosInColumn)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");

            if (posInColumn > ColumnLen)
                posInColumn = ColumnLen;

            if (posInColumn > PosInColumn)
            {
                await Buffer.Skip(posInColumn - PosInColumn, async);
                PosInColumn = posInColumn;
            }
        }

        internal override async Task ConsumeRow(bool async)
        {
            if (_column == -1)
            {
                await Buffer.Ensure(2, async);
                _numColumns = Buffer.ReadInt16();
            }

            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            // TODO: Potential for code-sharing with ReadColumn above, which also skips
            // Skip to end of column if needed
            var remainingInColumn = ColumnLen == -1 ? 0 : ColumnLen - PosInColumn;
            if (remainingInColumn > 0)
                await Buffer.Skip(remainingInColumn, async);

            // Skip over the remaining columns in the row
            for (; _column < _numColumns - 1; _column++)
            {
                await Buffer.Ensure(4, async);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    await Buffer.Skip(len, async);
            }
        }

        #region IsDBNull

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal) => IsDBNull(ordinal, false).GetAwaiter().GetResult();

        /// <summary>
        /// An asynchronous version of <see cref="IsDBNull(int)"/>, which gets a value that indicates whether the column contains non-existent or missing values.
        /// The <paramref name="cancellationToken"/> parameter is currently ignored.
        /// </summary>
        /// <param name="ordinal">The zero-based column to be retrieved.</param>
        /// <param name="cancellationToken">Currently ignored.</param>
        /// <returns><b>true</b> if the specified column value is equivalent to <see cref="DBNull"/> otherwise <b>false</b>.</returns>
        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (NoSynchronizationContextScope.Enter())
                return IsDBNull(ordinal, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        async Task<bool> IsDBNull(int ordinal, bool async)
        {
            CheckRowAndOrdinal(ordinal);

            await SeekToColumn(ordinal, async);
            return ColumnLen == -1;
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override long GetBytes(int ordinal, long dataOffset, [CanBeNull] byte[] buffer, int bufferOffset, int length)
        {
            var read = base.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
            if (buffer != null)  // If buffer is null we're just getting the length, no change in position
                PosInColumn += (int)read;
            return read;
        }

        #endregion

        internal override async ValueTask<Stream> GetStreamInternal(int column, bool async)
        {
            CheckRowAndOrdinal(column);
            if (_stream != null)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");

            await SeekToColumn(column, false);
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");

            return _stream = new SequentialByteaStream(this);
        }

        void CheckColumnStart()
        {
            if (PosInColumn != 0)
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
        }

        class SequentialByteaStream : Stream
        {
            readonly NpgsqlSequentialDataReader _reader;
            bool _disposed;

            internal SequentialByteaStream(NpgsqlSequentialDataReader reader)
            {
                _reader = reader;
            }

            public override int Read(byte[] buffer, int offset, int count)
                => Read(buffer, offset, count, false).GetAwaiter().GetResult();

            public override Task<int> ReadAsync([NotNull] byte[] buffer, int offset, int count, CancellationToken token)
            {
                token.ThrowIfCancellationRequested();
                using (NoSynchronizationContextScope.Enter())
                    return Read(buffer, offset, count, true);
            }

            async Task<int> Read(byte[] buffer, int offset, int count, bool async)
            {
                CheckDisposed();
                count = Math.Min(count, _reader.ColumnLen - _reader.PosInColumn);
                var read = await _reader.Buffer.ReadAllBytes(buffer, offset, count, true, async);
                _reader.PosInColumn += read;
                return read;
            }

            public override long Length
            {
                get
                {
                    CheckDisposed();
                    return _reader.ColumnLen;
                }
            }

            public override long Position
            {
                get
                {
                    CheckDisposed();
                    return _reader.PosInColumn;
                }
                set => throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing) => _disposed = true;

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;

            void CheckDisposed()
            {
                if (_disposed)
                    throw new ObjectDisposedException(null);
            }

            #region Not Supported

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            #endregion
        }
    }
}
