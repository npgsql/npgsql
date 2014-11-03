using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    class DataRowMessage : DataRowMessageBase
    {
        public NpgsqlBufferedStream Buffer { get; private set; }
        List<int> _columnOffsets;
        int _endOffset;
        List<NpgsqlValue> _values;
 
        public DataRowMessage(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
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
            if (column < 0 || column >= _columnOffsets.Count) {
                throw new IndexOutOfRangeException("Column index out of range");
            }
            var value = _values[column];
            if (value == null) {
                Buffer.Seek(_columnOffsets[column], SeekOrigin.Begin);
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

        internal override void Consume()
        {
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
        }
    }
}
