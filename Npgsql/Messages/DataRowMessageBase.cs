using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    internal abstract class DataRowMessageBase : IServerMessage
    {
        protected internal NpgsqlBufferedStream Buffer { get; private set; }

        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        internal int NumColumns;

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1
        /// </summary>
        internal int Column;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the current byte position within the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int PosInColumn;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the current "decoded" position within the column.
        /// For text, this is the character index. For text-encoded bytea, this holds the decoded position
        /// (i.e. the 3rd decoded (content) byte in a hex text-encoded bytea will occupy the 7th and 8th
        /// actual bytes).
        /// </summary>
        internal int DecodedPosInColumn;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the byte length of the column.
        /// Does not include the length prefix.
        /// </summary>
        internal int ColumnLen;

        internal bool IsColumnNull { get { return ColumnLen == -1; } }
        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the decoded length of the column.
        /// </summary>
        internal int DecodedColumnLen;

        internal ByteaTextFormat CurrentByteaTextFormat;

        /// <summary>
        /// Indicates whether a stream is currently open on a column. No read may occur until the stream is closed.
        /// </summary>
        internal bool IsStreaming;

        internal abstract NpgsqlValue Get(int column);

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        internal abstract void SeekToColumn(int column);
        internal abstract void SeekInColumn(int posInColumn);

        internal RowDescriptionMessage Description { get; set; }
        internal abstract void Consume();
        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }

        protected DataRowMessageBase(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            Column = -1;    
        }

        internal void Read(int column, NpgsqlValue value)
        {
            CheckNotStreaming();

            SeekToColumn(column);
            if (IsColumnNull)
            {
                value.SetToNull();
                return;
            }
            if (PosInColumn != 0)
            {
                SeekInColumn(0);
            }

            var fieldDescription = Description[column];
            var handler = fieldDescription.Handler;
            handler.Read(this, fieldDescription, value);
        }

        internal long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            CheckNotStreaming();

            var fieldDescription = Description[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetBytes() not supported for type " + fieldDescription.Name);
            }

            SeekToColumn(ordinal);

            if (IsColumnNull)
            {
                // TODO: What actual exception to throw here? Oracle throws InvalidCast, SqlClient throws its
                // own SqlNullValueException
                throw new InvalidCastException("The column contained NULL");
            }

            return handler.GetBytes(this, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        internal Stream GetStream(int ordinal)
        {
            CheckNotStreaming();

            var fieldDescription = Description[ordinal];
            var handler = fieldDescription.Handler as ByteaHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetStream() not supported for type " + fieldDescription.Name);
            }

            SeekToColumn(ordinal);
            if (IsColumnNull)
            {
                // TODO: What actual exception to throw here? Oracle throws InvalidCast, SqlClient throws its
                // own SqlNullValueException
                throw new InvalidCastException("The column contained NULL");
            }
            SeekInColumn(0);

            IsStreaming = true;
            try
            {
                return handler.GetStream(this, fieldDescription);
            }
            catch
            {
                IsStreaming = false;
                throw;
            }
        }

        internal long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            CheckNotStreaming();

            var fieldDescription = Description[ordinal];
            var handler = fieldDescription.Handler as StringHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetChars() not supported for type " + fieldDescription.Name);
            }
            SeekToColumn(ordinal);

            if (IsColumnNull)
            {
                // TODO: What actual exception to throw here? Oracle throws InvalidCast, SqlClient throws its
                // own SqlNullValueException
                throw new InvalidCastException("The column contained NULL");
            }

            return handler.GetChars(this, (int)dataOffset, buffer, bufferOffset, length, fieldDescription);
        }

        internal TextReader GetTextReader(int ordinal)
        {
            CheckNotStreaming();

            var fieldDescription = Description[ordinal];
            var handler = fieldDescription.Handler as StringHandler;
            if (handler == null)
            {
                throw new InvalidCastException("GetTextReader() not supported for type " + fieldDescription.Name);
            }

            SeekToColumn(ordinal);
            if (IsColumnNull)
            {
                // TODO: What actual exception to throw here? Oracle throws InvalidCast, SqlClient throws its
                // own SqlNullValueException
                throw new InvalidCastException("The column contained NULL");
            }
            SeekInColumn(0);

            IsStreaming = true;
            try
            {
                return new StreamReader(new ByteaBinaryStream(this));
            }
            catch
            {
                IsStreaming = false;
                throw;
            }
        }

        #region Checks

        protected void CheckColumnIndex(int column)
        {
            if (column < 0 || column >= Description.NumFields) {
                throw new IndexOutOfRangeException("Column index out of range");
            }
        }

        internal void CheckNotStreaming()
        {
            if (IsStreaming)
            {
                throw new InvalidOperationException("Column streaming is in progress, close the Stream or TextReader first.");
            }
        }

        #endregion
    }
}
