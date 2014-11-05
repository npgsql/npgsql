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
        /// For streaming types (e.g. bytea), holds the length of the column.
        /// Does not include the length prefix.
        /// </summary>
        protected int ColumnLen;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the current position within the column.
        /// Does not include the length prefix.
        /// </summary>
        protected int PosInColumn;

        internal abstract NpgsqlValue Get(int ordinal);
        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="posInColumn"></param>
        protected abstract void SeekToColumn(int column, int posInColumn);

        internal RowDescriptionMessage Description { get; set; }
        internal abstract void Consume();
        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }

        protected DataRowMessageBase(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            Column = -1;    
        }

        internal long GetBytes(int column, int posInColumn, byte[] output, int ouputOffset, int len)
        {
            CheckBytea(column);

            // Return column length
            if (output == null)
            {
                // TODO: What is the required behavior when the column is null?
                if (Column < column)
                {
                    SeekToColumn(column, 0);                    
                }
                return ColumnLen;
            }

            SeekToColumn(column, posInColumn);

            if (ColumnLen == -1)
            {
                // TODO: What is the actual required behavior here?
                throw new Exception("null");
            }

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

        internal Stream GetStream(int column)
        {
            CheckColumnIndex(column);
            CheckBytea(column);
            SeekToColumn(column, 0);
            return Buffer.GetStream(ColumnLen);
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
