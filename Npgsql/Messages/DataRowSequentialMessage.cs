using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;

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
        int _columnPos;

        NpgsqlValue _value;

        public DataRowSequentialMessage(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            _numColumns = buf.ReadInt16();
            _columnPos = -1;
            _value = new NpgsqlValue();
        }

        public void SeekToColumn(int index)
        {
            if (index < 0 || index >= _numColumns) {
                throw new IndexOutOfRangeException("index must be between 0 and " + (_numColumns-1));
            }

            if (index < _columnPos)
            {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, index, _columnPos));                
            }

            for (; _columnPos < index; _columnPos++)
            {
                Buffer.Ensure(4);
                var len = Buffer.ReadInt32();
                if (len != -1) {
                    Buffer.Skip(len);
                }
            }
        }

        internal override NpgsqlValue Get(int column)
        {
            if (column < 0 || column >= _numColumns) {
                throw new IndexOutOfRangeException("Column index out of range");
            }

            if (column < _columnPos) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, _columnPos));
            }

            if (column == _columnPos) {
                return _value;
            }

            // Skip over unwanted fields
            int len;
            for (; _columnPos < column - 1; _columnPos++)
            {
                Buffer.Ensure(4);
                len = Buffer.ReadInt32();
                if (len != -1) {
                    Buffer.Skip(len);
                }
            }
            
            Buffer.Ensure(4);
            len = Buffer.ReadInt32();
            if (len == -1) {
                _value.SetToNull();
            } else {
                var fieldDescription = Description[column];
                fieldDescription.Handler.Read(Buffer, len, fieldDescription, _value);
            }
            _columnPos++;
            return _value;
        }

        internal override void Consume()
        {
            while (_columnPos < _numColumns - 1)
            {
                Buffer.Ensure(4);
                Buffer.Skip(Buffer.ReadInt32());
                _columnPos++;
            }
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }
    }
}
