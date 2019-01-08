﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

namespace Npgsql
{
    /// <summary>
    /// The default, non-sequential reader, which buffers entire rows in memory.
    /// </summary>
#pragma warning disable CA1010
    sealed class NpgsqlDefaultDataReader : NpgsqlDataReader
#pragma warning restore CA1010
    {
        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        int _column;
        readonly List<(int Offset, int Length)> _columns = new List<(int Offset, int Length)>();
        int _dataMsgEnd;

        internal NpgsqlDefaultDataReader(NpgsqlConnector connector) : base(connector) {}

        internal override ValueTask<IBackendMessage> ReadMessage(bool async)
            => Connector.ReadMessage(async);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override Task ConsumeRow(bool async)
        {
            Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);

            if (ColumnStream != null)
            {
                ColumnStream.Dispose();
                ColumnStream = null;
            }
            Buffer.ReadPosition = _dataMsgEnd;
            return PGUtil.CompletedTask;
        }

        internal override void ProcessDataMessage(DataRowMessage dataMsg)
        {
            // The connector's buffer can actually change between DataRows:
            // If a large DataRow exceeding the connector's current read buffer arrives, and we're
            // reading in non-sequential mode, a new oversize buffer is allocated. We thus have to
            // recapture the connector's buffer on each new DataRow.
            Buffer = Connector.ReadBuffer;
            _dataMsgEnd = Buffer.ReadPosition + dataMsg.Length;

            // We currently assume that the row's number of columns is identical to the description's
#if DEBUG
            var numColumns = Buffer.ReadInt16();
            Debug.Assert(RowDescription.NumFields == numColumns);
#else
            Buffer.ReadPosition += 2;
#endif
            _column = -1;
            _columns.Clear();

            // Initialize our columns array with the offset and length of the first column
            var len = Buffer.ReadInt32();
            _columns.Add((Buffer.ReadPosition, len));
        }

        // We know the entire row is buffered in memory (non-sequential reader), so no I/O will be performed
        public override Task<T> GetFieldValueAsync<T>(int column, CancellationToken cancellationToken)
            => Task.FromResult(GetFieldValue<T>(column));

        public override T GetFieldValue<T>(int column)
        {
            CheckRowAndOrdinal(column);

            SeekToColumn(column);
            if (ColumnLen == -1)
            {
                if (NullableHandler<T>.Exists)
                    return default;
                if (typeof(T) == typeof(object))
                    return (T)(object)DBNull.Value;
                throw new InvalidCastException("Column is null");
            }

            var fieldDescription = RowDescription[column];
            try
            {
                if (NullableHandler<T>.Exists)
                    return NullableHandler<T>.Read(Buffer, ColumnLen, fieldDescription);

                return typeof(T) == typeof(object)
                    ? (T)fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, fieldDescription)
                    : fieldDescription.Handler.Read<T>(Buffer, ColumnLen, fieldDescription);
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
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            SeekToColumn(ordinal);
            if (ColumnLen == -1)
                return DBNull.Value;

            var fieldDescription = RowDescription[ordinal];
            object result;
            try
            {
                result = fieldDescription.Handler.ReadAsObject(Buffer, ColumnLen, fieldDescription);
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

            // Used for Entity Framework <= 6 compability
            if (Command.ObjectResultTypes?[ordinal] != null)
            {
                var type = Command.ObjectResultTypes[ordinal];
                result = type == typeof(DateTimeOffset)
                    ? new DateTimeOffset((DateTime)result)
                    : Convert.ChangeType(result, type);
            }

            return result;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of <see cref="object"/>.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetProviderSpecificValue(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            SeekToColumn(ordinal);
            if (ColumnLen == -1)
                return DBNull.Value;

            var fieldDescription = RowDescription[ordinal];
            object result;
            try
            {
                // TODO: Maybe call a non-async method which would allow simple type handlers (and
                // maybe also text) to read without going through async...
                result = fieldDescription.Handler.ReadPsvAsObject(Buffer, ColumnLen, fieldDescription);
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

            return result;
        }

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns><b>true</b> if the specified column is equivalent to <see cref="DBNull"/>; otherwise <b>false</b>.</returns>
        public override bool IsDBNull(int ordinal)
        {
            CheckRowAndOrdinal(ordinal);

            SeekToColumn(ordinal);
            return ColumnLen == -1;
        }

        void SeekToColumn(int column)
        {
            // Shut down any streaming going on on the column
            if (ColumnStream != null)
            {
                ColumnStream.Dispose();
                ColumnStream = null;
            }

            for (var lastColumnRead = _columns.Count; column >= lastColumnRead; lastColumnRead++)
            {
                int lastColumnLen;
                (Buffer.ReadPosition, lastColumnLen) = _columns[lastColumnRead-1];
                if (lastColumnLen != -1)
                    Buffer.ReadPosition += lastColumnLen;
                var len = Buffer.ReadInt32();
                _columns.Add((Buffer.ReadPosition, len));
            }

            (Buffer.ReadPosition, ColumnLen) = _columns[column];
            _column = column;
            PosInColumn = 0;
        }

        internal override Task SeekToColumn(int column, bool async)
        {
            SeekToColumn(column);
            return PGUtil.CompletedTask;
        }

        internal override Task SeekInColumn(int posInColumn, bool async)
        {
            if (posInColumn > ColumnLen)
                posInColumn = ColumnLen;
            Buffer.ReadPosition = _columns[_column].Offset + posInColumn;
            PosInColumn = posInColumn;
            return PGUtil.CompletedTask;
        }
    }
}
