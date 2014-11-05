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
        NpgsqlValue _value;

        public DataRowSequentialMessage(NpgsqlBufferedStream buf) : base(buf)
        {
            NumColumns = buf.ReadInt16();
            _value = new NpgsqlValue();
        }

        protected override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (column < Column) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, Column));
            }

            if (Column < column)
            {
                // Skip to end of column if needed
                if (InColumn && ColumnLen > PosInColumn)
                {
                    Buffer.Skip(ColumnLen - PosInColumn);
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

                InColumn = false;
                Column++;
            }
        }

        protected override void SeekInColumn(long posInColumn)
        {
            if (posInColumn < PosInColumn) {
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
            }

            if (posInColumn >= ColumnLen)
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

        internal override NpgsqlValue Get(int column)
        {
            if (Column == column)
            {
                if (PosInColumn != 0) {
                    throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, Column));
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

        internal override void Consume()
        {
            // Skip to end of column if needed
            if (InColumn && ColumnLen > PosInColumn) {
                Buffer.Skip(ColumnLen - PosInColumn);
            }

            while (Column < NumColumns - 1)
            {
                Buffer.Ensure(4);
                Buffer.Skip(Buffer.ReadInt32());
                Column++;
            }
        }

        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }
    }
}
