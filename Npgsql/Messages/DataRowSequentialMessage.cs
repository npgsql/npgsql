using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    class DataRowSequentialMessage : DataRowMessageBase
    {
        readonly NpgsqlValue _value;

        public DataRowSequentialMessage(NpgsqlBufferedStream buf) : base(buf)
        {
            NumColumns = buf.ReadInt16();
            _value = new NpgsqlValue();
        }

        internal override NpgsqlValue Get(int column)
        {
            Read(column, _value);
            return _value;
        }

        internal override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (column < Column) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, Column));
            }

            if (column == Column) {
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
                if (len != -1) {
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

            if (posInColumn > ColumnLen)
            {
                // TODO: What is the actual required behavior here?
                throw new IndexOutOfRangeException();
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
            if (remainingInColumn > 0) {
                Buffer.Skip(remainingInColumn);
            }

            while (Column < NumColumns - 1)
            {
                Buffer.Ensure(4);
                Buffer.Skip(Buffer.ReadInt32());
                Column++;
            }
        }
    }
}
