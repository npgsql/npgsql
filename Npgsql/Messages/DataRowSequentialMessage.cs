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
        /// <summary>
        /// Whether the current column's value has already been parsed in its entirety.
        /// </summary>
        bool _columnParsed;
        NpgsqlValue _value;

        public DataRowSequentialMessage(NpgsqlBufferedStream buf) : base(buf)
        {
            NumColumns = buf.ReadInt16();
            _value = new NpgsqlValue();
        }

        #region Seek

        protected override void SeekToColumn(int column)
        {
            CheckColumnIndex(column);

            if (column < Column) {
                throw new InvalidOperationException(string.Format(L10N.RowSequentialFieldError, column, Column));
            }

            if (column > Column)
            {
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
                _columnParsed = false;
                Column = column;
            }
        }

        protected override void SeekInColumn(int posInColumn)
        {
            if (posInColumn < PosInColumn)
            {
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

        protected override void SeekInColumnText(int textPosInColumn)
        {
            if (textPosInColumn < DecodedPosInColumn)
            {
                throw new InvalidOperationException("Attempt to read a position in the column which has already been read");
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

        internal override NpgsqlValue Get(int column)
        {
            CheckNotStreaming();

            if (_columnParsed) {
                return _value;
            }

            SeekToColumn(column);
            SeekInColumn(0);

            if (ColumnLen == -1) {
                _value.SetToNull();
            } else {
                // TODO: For primitives only, need to ensure we have the data here...
                Buffer.Ensure(ColumnLen);
                var fieldDescription = Description[column];
                fieldDescription.Handler.Read(Buffer, ColumnLen, fieldDescription, _value);
                PosInColumn += ColumnLen;
            }
            _columnParsed = true;
            return _value;
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

        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }
    }
}
