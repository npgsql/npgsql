using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Npgsql.Localization;

namespace Npgsql.Messages
{
    class DataRowMessage : DataRowMessageBase
    {
        readonly List<int> _columnOffsets;
        readonly int _endOffset;
        readonly List<NpgsqlValue> _cachedValues;
 
        public DataRowMessage(NpgsqlBufferedStream buf) : base(buf)
        {
            // TODO: Recycle message objects rather than recreating for each row
            NumColumns = buf.ReadInt16();
            _columnOffsets = new List<int>(NumColumns);
            _cachedValues = new List<NpgsqlValue>(NumColumns);
            for (var i = 0; i < NumColumns; i++)
            {
                _cachedValues.Add(null);
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
            var value = _cachedValues[column];
            if (value == null)
            {
                value = new NpgsqlValue();  // TODO: Allocation
                Read(column, value);

                // Don't cache binary since the user may modify it
                if (value.Type != NpgsqlValue.StorageType.Binary) {
                    _cachedValues[column] = value;
                }
            }

            return value;
        }

        internal override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (Column != column)
            {
                Buffer.Seek(_columnOffsets[column], SeekOrigin.Begin);
                Column = column;
                ColumnLen = Buffer.ReadInt32();
                PosInColumn = DecodedPosInColumn = 0;
            }
        }

        internal override void SeekInColumn(int posInColumn)
        {
            if (posInColumn > ColumnLen)
            {
                // TODO: What is the actual required behavior here?
                throw new IndexOutOfRangeException();
            }

            Buffer.Seek(_columnOffsets[Column] + 4 + posInColumn, SeekOrigin.Begin);
            PosInColumn = posInColumn;
        }

        internal override void Consume()
        {
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
        }
    }
}
