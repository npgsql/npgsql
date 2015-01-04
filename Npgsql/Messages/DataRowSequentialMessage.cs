using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql.Localization;

namespace Npgsql.Messages
{
    class DataRowSequentialMessage : DataRowMessage
    {
        internal override DataRowMessage Load(NpgsqlBuffer buf)
        {
            buf.Ensure(sizeof(short));
            NumColumns = buf.ReadInt16();
            Buffer = buf;
            Column = -1;
            ColumnLen = -1;
            PosInColumn = DecodedPosInColumn = 0;
            return this;
        }

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        internal override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (column < Column)
            {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, Column));
            }

            if (column == Column)
            {
                return;
            }

            // Skip to end of column if needed
            var remainingInColumn = (ColumnLen == -1 ? 0 : ColumnLen - PosInColumn);
            if (remainingInColumn > 0)
            {
                Buffer.Skip(remainingInColumn);
            }

            // Skip over unwanted fields
            for (; Column < column - 1; Column++)
            {
                Buffer.Ensure(4);
                var len = Buffer.ReadInt32();
                if (len != -1)
                {
                    Buffer.Skip(len);
                }
            }

            Buffer.Ensure(4);
            ColumnLen = Buffer.ReadInt32();
            PosInColumn = DecodedPosInColumn = 0;
            Column = column;
        }

        internal override void SeekInColumn(int posInColumn)
        {
            if (posInColumn < PosInColumn)
            {
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
            }

            if (posInColumn > ColumnLen) {
                posInColumn = ColumnLen;
            }

            if (posInColumn > PosInColumn)
            {
                Buffer.Skip(posInColumn - PosInColumn);
                PosInColumn = posInColumn;
            }
        }

        internal override void Consume()
        {
            // Skip to end of column if needed
            var remainingInColumn = (ColumnLen == -1 ? 0 : ColumnLen - PosInColumn);
            if (remainingInColumn > 0)
            {
                Buffer.Skip(remainingInColumn);
            }

            // Skip over the remaining columns in the row
            for (; Column < NumColumns - 1; Column++)
            {
                Buffer.Ensure(4);
                var len = Buffer.ReadInt32();
                if (len != -1) {
                    Buffer.Skip(len);
                }
            }
        }
    }
}
