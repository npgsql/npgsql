using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    class DataRowSequentialMessage : DataRowMessageBase
    {
        public NpgsqlBufferedStream Buffer { get; private set; }

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
        /// Specifies whether we are currently reading within the column, i.e. streaming it
        /// </summary>
        bool _inColumn;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the length of the column.
        /// Does not include the length prefix.
        /// </summary>
        long _columnLen;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the current position within the column.
        /// Does not include the length prefix.
        /// </summary>
        long _posInColumn;

        NpgsqlValue _value;

        public DataRowSequentialMessage(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            _numColumns = buf.ReadInt16();
            _column = -1;
            _value = new NpgsqlValue();
        }

        public void SeekToColumn(int column)
        {
            if (column < 0 || column >= _numColumns) {
                throw new IndexOutOfRangeException("Column index out of range");
            }

            if (column < _column) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, _column));
            }

            if (_column < column)
            {
                // Skip to end of column if needed
                if (_inColumn && _columnLen > _posInColumn)
                {
                    Buffer.Skip(_columnLen - _posInColumn);
                }

                // Skip over unwanted fields
                for (; _column < column - 1; _column++)
                {
                    Buffer.Ensure(4);
                    var len = Buffer.ReadInt32();
                    if (len != -1) {
                        Buffer.Skip(len);
                    }
                }

                _inColumn = false;
                _column++;
            }
        }

        internal override NpgsqlValue Get(int column)
        {
            if (_column == column)
            {
                if (_posInColumn != 0) {
                    throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, _column));
                }
                return _value;
            }

            SeekToColumn(column);

            Buffer.Ensure(4);
            var len = Buffer.ReadInt32();
            if (len == -1) {
                _value.SetToNull();
            } else {
                // TODO: For primitives only, need to ensure we have the data here...
                Buffer.Ensure(len);
                var fieldDescription = Description[column];
                fieldDescription.Handler.Read(Buffer, len, fieldDescription, _value);
            }
            return _value;
        }

        void EnsureBytea(int column)
        {
            var typeHandler = Description[column].Handler;
            if (!(typeHandler is ByteaHandler)) {
                throw new InvalidCastException(String.Format("Column type must be bytea (was {0})", typeHandler.PgName));
            }            
        }

        internal override long GetBytes(int column, long posInColumn, byte[] output, int ouputOffset, int len)
        {
            EnsureBytea(column);

            if (_column != column)
            {
                SeekToColumn(column);
                Buffer.Ensure(4);
                _posInColumn = 0;
                _columnLen = Buffer.ReadInt32();
                _inColumn = true;
            }

            // Return column length
            if (output == null) {
                return _columnLen;
            }

            if (posInColumn < _posInColumn) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, _column));
            }

            // Need to skip some bytes within the column
            if (posInColumn > _posInColumn) {
                Buffer.Skip(posInColumn - _posInColumn);
                _posInColumn = posInColumn;
            }

            // Attempt to read beyond the end of the column
            if (posInColumn + len > _columnLen) {
                len = (int)(_columnLen - posInColumn);
            }

            Buffer.Read(output, ouputOffset, len);
            _posInColumn += len;

            return len;
        }

        internal override void Consume()
        {
            // Skip to end of column if needed
            if (_inColumn && _columnLen > _posInColumn) {
                Buffer.Skip(_columnLen - _posInColumn);
            }

            while (_column < _numColumns - 1)
            {
                Buffer.Ensure(4);
                Buffer.Skip(Buffer.ReadInt32());
                _column++;
            }
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }
    }
}
