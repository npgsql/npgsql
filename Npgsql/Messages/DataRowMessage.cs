using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;

namespace Npgsql.Messages
{
    class DataRowMessage : DataRowMessageBase
    {
        List<int> _columnOffsets;
        int _endOffset;
        List<NpgsqlValue> _values;
 
        public DataRowMessage(NpgsqlBufferedStream buf) : base(buf)
        {
            var columnCount = buf.ReadInt16();
            _columnOffsets = new List<int>(columnCount);
            _values = new List<NpgsqlValue>(columnCount);
            for (var i = 0; i < columnCount; i++)
            {
                _values.Add(null);
                _columnOffsets.Add(buf.Position);
                var len = buf.ReadInt32();
                if (len != -1) {
                    buf.Seek(len, SeekOrigin.Current);                    
                }
            }
            _endOffset = buf.Position;
        }

        internal override NpgsqlValue Get(int column)
        {
            var value = _values[column];
            if (value == null) {
                SeekToColumn(column);
                var len = Buffer.ReadInt32();
                value = _values[column] = new NpgsqlValue();  // TODO: Allocation
                if (len == -1) {
                    value.SetToNull();
                } else {
                    var fieldDescription = Description[column];
                    fieldDescription.Handler.Read(Buffer, len, fieldDescription, value);
                }
                return value;
            }
            return value;
        }

        #region Seek

        protected override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (Column < column)
            {
                Buffer.Seek(_columnOffsets[column], SeekOrigin.Begin);
                InColumn = false;
                Column++;
            }
        }

        protected override void SeekInColumn(long posInColumn)
        {
            var posInColumnInt = (int) posInColumn;
            if (posInColumnInt != posInColumn) {
                throw new ArgumentException("Position must be int when in non-sequential mode", "posInColumn");
            }

            if (posInColumn >= ColumnLen)
            {
                // TODO: What is the actual required behavior here?
                throw new IndexOutOfRangeException();
            }

            // TODO: Seek currently accepts ints, so we're limited to 2GB blobs
            Buffer.Seek(_columnOffsets[Column] + 4 + posInColumnInt, SeekOrigin.Begin);
        }

        #endregion

        internal override Stream GetStream(int column)
        {
            CheckColumnIndex(column);
            CheckBytea(column);

            SeekToColumn(column);
            Buffer.Ensure(4);
            PosInColumn = 0;
            var len = Buffer.ReadInt32();
            return Buffer.GetMemoryStream(len);
        }

        internal override void Consume()
        {
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
        }
    }
}
