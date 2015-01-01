using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Npgsql.Messages
{
    class DataRowNonSequentialMessage : DataRowMessage
    {
        List<int> _columnOffsets;
        int _endOffset;

        internal override DataRowMessage Load(NpgsqlBuffer buf)
        {
            NumColumns = buf.ReadInt16();
            Buffer = buf;
            Column = -1;
            ColumnLen = -1;
            PosInColumn = DecodedPosInColumn = 0;
            // TODO: Recycle message objects rather than recreating for each row
            _columnOffsets = new List<int>(NumColumns);
            for (var i = 0; i < NumColumns; i++)
            {
                _columnOffsets.Add(buf.ReadPosition);
                var len = buf.ReadInt32();
                if (len != -1)
                {
                    buf.Seek(len, SeekOrigin.Current);
                }
            }
            _endOffset = buf.ReadPosition;
            return this;
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
            if (posInColumn > ColumnLen) {
                posInColumn = ColumnLen;
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
