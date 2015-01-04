using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    abstract class DataRowMessage : BackendMessage
    {
        protected internal NpgsqlBuffer Buffer { get; protected set; }

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

        internal override BackendMessageCode Code { get { return BackendMessageCode.DataRow; } }

        internal abstract DataRowMessage Load(NpgsqlBuffer buf);

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        internal abstract void SeekToColumn(int column);
        internal abstract void SeekInColumn(int posInColumn);
        /// <summary>
        /// Consumes the current row, allowing the reader to read in the next one.
        /// </summary>
        internal abstract void Consume();

        internal void SeekToColumnStart(int column)
        {
            CheckNotStreaming();
            SeekToColumn(column);
            if (PosInColumn != 0) {
                SeekInColumn(0);
            }
        }

        #region Checks

        protected void CheckColumnIndex(int column)
        {
            if (column < 0 || column >= NumColumns)
            {
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

        internal void CheckNotNull()
        {
            if (IsColumnNull)
            {
                // TODO: What actual exception to throw here? Oracle throws InvalidCast, SqlClient throws its
                // own SqlNullValueException
                throw new InvalidCastException("Column is null");
            }
        }

        #endregion
    }
}
