using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.BackendMessages;
using Npgsql.TypeHandlers;
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
        int _numColumns;

        [CanBeNull]
        List<int> _columnOffsets;
        int _endOffset;

        int _column;

        /// <summary>
        /// List of all streams that have been opened on the current row, and need to be disposed of
        /// when the row is consumed.
        /// </summary>
        [CanBeNull]
        List<IDisposable> _streams;

        internal NpgsqlDefaultDataReader(NpgsqlCommand command, CommandBehavior behavior, List<NpgsqlStatement> statements, Task sendTask)
            : base(command, behavior, statements, sendTask) {}

        internal override ValueTask<IBackendMessage> ReadMessage(bool async)
            => Connector.ReadMessage(async);

        protected override Task<bool> Read(bool async)
        {
            // This is an optimized execution path that avoids calling any async methods for the (usual)
            // case where the next row (or CommandComplete) is already in memory.
            if (State != ReaderState.InResult || (Behavior & CommandBehavior.SingleRow) != 0)
                return base.Read(async);

            ConsumeRow(false);

            var readBuf = Connector.ReadBuffer;
            if (readBuf.ReadBytesLeft < 5)
                return base.Read(async);
            var messageCode = (BackendMessageCode)readBuf.ReadByte();
            var len = readBuf.ReadInt32() - 4;  // Transmitted length includes itself
            if (messageCode != BackendMessageCode.DataRow || readBuf.ReadBytesLeft < len)
            {
                readBuf.Seek(-5, SeekOrigin.Current);
                return base.Read(async);
            }

            var msg = Connector.ParseServerMessage(readBuf, messageCode, len, false);
            ProcessMessage(msg);
            return msg.Code == BackendMessageCode.DataRow
                ? PGUtil.TrueTask : PGUtil.FalseTask;
        }

        protected override Task<bool> NextResult(bool async, bool isConsuming=false)
        {
            var task = base.NextResult(async, isConsuming);

            if (Command.Parameters.HasOutputParameters && StatementIndex == 0)
            {
                // Populate the output parameters from the first row of the first resultset
                return task.ContinueWith((t, o) =>
                {
                    if (HasRows)
                        PopulateOutputParameters();
                    return t.Result;
                }, null);
            }

            return task;
        }

        /// <summary>
        /// The first row in a stored procedure command that has output parameters needs to be traversed twice -
        /// once for populating the output parameters and once for the actual result set traversal. So in this
        /// case we can't be sequential.
        /// </summary>
        void PopulateOutputParameters()
        {
            Debug.Assert(Command.Parameters.Any(p => p.IsOutputDirection));
            Debug.Assert(StatementIndex == 0);
            Debug.Assert(RowDescription != null);
            Debug.Assert(State == ReaderState.BeforeResult);

            // Temporarily set our state to InResult to allow us to read the values
            State = ReaderState.InResult;

            var pending = new Queue<NpgsqlParameter>();
            var taken = new List<int>();
            foreach (var p in Command.Parameters.Where(p => p.IsOutputDirection))
            {
                if (RowDescription.TryGetFieldIndex(p.CleanName, out var idx))
                {
                    // TODO: Provider-specific check?
                    p.Value = GetValue(idx);
                    taken.Add(idx);
                }
                else
                    pending.Enqueue(p);
            }
            for (var i = 0; pending.Count != 0 && i != _numColumns; ++i)
            {
                // TODO: Need to get the provider-specific value based on the out param's type
                if (!taken.Contains(i))
                    pending.Dequeue().Value = GetValue(i);
            }

            State = ReaderState.BeforeResult;  // Set the state back
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override Task ConsumeRow(bool async)
        {
            Debug.Assert(State == ReaderState.InResult || State == ReaderState.BeforeResult);
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
            if (_streams != null)
            {
                foreach (var stream in _streams)
                    stream.Dispose();
                _streams.Clear();
            }
            return PGUtil.CompletedTask;
        }

        internal override void ProcessDataMessage(DataRowMessage dataMsg)
        {
            // The connector's buffer can actually change between DataRows:
            // If a large DataRow exceeding the connector's current read buffer arrives, and we're
            // reading in non-sequential mode, a new oversize buffer is allocated. We thus have to
            // recapture the connector's buffer on each new DataRow.
            Buffer = Connector.ReadBuffer;

            _numColumns = Buffer.ReadInt16();
            _column = -1;
            Debug.Assert(RowDescription.NumFields == _numColumns);
            if (_columnOffsets == null)
                _columnOffsets = new List<int>(_numColumns);
            else
                _columnOffsets.Clear();

            for (var i = 0; i < _numColumns; i++)
            {
                _columnOffsets.Add(Buffer.ReadPosition);
                var len = Buffer.ReadInt32();
                if (len != -1)
                    Buffer.Seek(len, SeekOrigin.Current);
            }
            _endOffset = Buffer.ReadPosition;
        }

        // We know the entire row is buffered in memory (non-sequential reader), so no I/O will be performed
        public override Task<T> GetFieldValueAsync<T>(int column, CancellationToken cancellationToken)
            => Task.FromResult(GetFieldValue<T>(column));

        public override T GetFieldValue<T>(int column)
        {
            CheckRowAndOrdinal(column);

            SeekToColumn(column);
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");

            var fieldDescription = RowDescription[column];
            try
            {
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

        internal override ValueTask<Stream> GetStreamInternal(int column, bool async)
        {
            SeekToColumn(column);
            if (ColumnLen == -1)
                throw new InvalidCastException("Column is null");

            var s = new MemoryStream(Buffer.Buffer, Buffer.ReadPosition, ColumnLen, false, false);
            if (_streams == null)
                _streams = new List<IDisposable>();
            _streams.Add(s);
            return new ValueTask<Stream>(s);
        }

        void SeekToColumn(int column) => SeekToColumn(column, false);

        internal override Task SeekToColumn(int column, bool async)
        {
            Buffer.Seek(_columnOffsets[column], SeekOrigin.Begin);
            ColumnLen = Buffer.ReadInt32();
            _column = column;
            PosInColumn = 0;
            return PGUtil.CompletedTask;
        }

        internal override Task SeekInColumn(int posInColumn, bool async)
        {
            if (posInColumn > ColumnLen)
                posInColumn = ColumnLen;
            Buffer.Seek(_columnOffsets[_column] + 4 + posInColumn, SeekOrigin.Begin);
            PosInColumn = posInColumn;
            return PGUtil.CompletedTask;
        }
    }
}
