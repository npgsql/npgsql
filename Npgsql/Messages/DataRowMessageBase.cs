using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    internal abstract class DataRowMessageBase : IServerMessage
    {
        protected NpgsqlBufferedStream Buffer { get; private set; }

        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        protected int NumColumns;

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1
        /// </summary>
        protected int Column;

        /// <summary>
        /// Specifies whether we are currently reading within the column, i.e. streaming it
        /// </summary>
        protected bool InColumn;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the length of the column.
        /// Does not include the length prefix.
        /// </summary>
        protected long ColumnLen;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the current position within the column.
        /// Does not include the length prefix.
        /// </summary>
        protected long PosInColumn;

        internal abstract NpgsqlValue Get(int ordinal);
        protected abstract void SeekToColumn(int column);
        protected abstract void SeekInColumn(long posInColumn);
        internal abstract Stream GetStream(int ordinal);

        internal RowDescriptionMessage Description { get; set; }
        internal abstract void Consume();
        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }

        protected DataRowMessageBase(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            Column = -1;    
        }

        internal long GetBytes(int column, long posInColumn, byte[] output, int ouputOffset, int len)
        {
            CheckBytea(column);

            if (Column != column)
            {
                SeekToColumn(column);
                Buffer.Ensure(4);
                PosInColumn = 0;
                ColumnLen = Buffer.ReadInt32();
                InColumn = true;
            }

            if (ColumnLen == -1)
            {
                // TODO: What is the actual required behavior here?
                throw new Exception("null");
            }

            // Return column length
            if (output == null)
            {
                return ColumnLen;
            }

            SeekInColumn(posInColumn);

            // Attempt to read beyond the end of the column
            if (posInColumn + len > ColumnLen)
            {
                // TODO: What is the actual required behavior here?
                len = (int)(ColumnLen - posInColumn);
            }

            Buffer.ReadBytes(output, ouputOffset, len);
            PosInColumn += len;

            return len;
        }


        protected void CheckColumnIndex(int column)
        {
            if (column < 0 || column >= Description.NumFields) {
                throw new IndexOutOfRangeException("Column index out of range");
            }
        }

        protected void CheckBytea(int column)
        {
            var typeHandler = Description[column].Handler;
            if (!(typeHandler is ByteaHandler))
            {
                throw new InvalidCastException(String.Format("Column type must be bytea (was {0})", typeHandler.PgName));
            }
        }
    }
}
