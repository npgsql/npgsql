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
            CheckNotStreaming();

            var value = _values[column];
            if (value == null)
            {
                SeekToColumn(column);
                SeekInColumn(0);
                value = _values[column] = new NpgsqlValue();  // TODO: Allocation
                if (ColumnLen == -1) {
                    value.SetToNull();
                } else {
                    var fieldDescription = Description[column];
                    fieldDescription.Handler.Read(Buffer, ColumnLen, fieldDescription, value);
                }
                return value;
            }
            return value;
        }

        #region Seek

        protected override void SeekToColumn(int column)
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

        protected override void SeekInColumn(int posInColumn)
        {
            if (posInColumn >= ColumnLen)
            {
                // TODO: What is the actual required behavior here?
                throw new IndexOutOfRangeException();
            }

            Buffer.Seek(_columnOffsets[Column] + 4 + posInColumn, SeekOrigin.Begin);
            PosInColumn = posInColumn;
        }

        protected override void SeekInColumnText(int textPosInColumn)
        {
            if (textPosInColumn < DecodedPosInColumn)
            {
                // Rewind back to the beginning of the column
                Buffer.Seek(_columnOffsets[Column] + 4, SeekOrigin.Begin);
                PosInColumn = DecodedPosInColumn = 0;
            }

            if (textPosInColumn > DecodedPosInColumn)
            {
                var charsToSkip = textPosInColumn - DecodedPosInColumn;
                int bytesSkipped, charsSkipped;
                Buffer.SkipChars(charsToSkip, ColumnLen - PosInColumn, out bytesSkipped, out charsSkipped);
                PosInColumn += bytesSkipped;
                DecodedPosInColumn += charsSkipped;
                if (charsSkipped < charsToSkip)
                {
                    // TODO: What is the actual required behavior here?
                    throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion

        internal override void Consume()
        {
            Buffer.Seek(_endOffset, SeekOrigin.Begin);
        }
    }
}
