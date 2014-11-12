using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Localization;
using Npgsql.TypeHandlers;

namespace Npgsql.Messages
{
    internal abstract class DataRowMessageBase : IServerMessage
    {
        protected NpgsqlBufferedStream Buffer { get; private set; }

        /// <summary>
        /// The number of columns in the current row
        /// </summary>
        protected int NumColumns;

        /// <summary>
        /// The index of the column that we're on, i.e. that has already been parsed, is
        /// is memory and can be retrieved. Initialized to -1
        /// </summary>
        protected int Column;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the current byte position within the column.
        /// Does not include the length prefix.
        /// </summary>
        protected int PosInColumn;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the current "decoded" position within the column.
        /// For text, this is the character index. For text-encoded bytea, this holds the decoded position
        /// (i.e. the 3rd decoded (content) byte in a hex text-encoded bytea will occupy the 7th and 8th
        /// actual bytes).
        /// </summary>
        protected int DecodedPosInColumn;

        /// <summary>
        /// For streaming types (e.g. bytea), holds the byte length of the column.
        /// Does not include the length prefix.
        /// </summary>
        protected int ColumnLen;

        /// <summary>
        /// For streaming types (e.g. bytea, text), holds the decoded length of the column.
        /// </summary>
        protected int DecodedColumnLen;

        ByteaTextFormat ByteaTextFormat;

        /// <summary>
        /// Indicates whether a stream is currently open on a column. No read may occur until the stream is closed.
        /// </summary>
        protected bool IsStreaming;

        internal abstract NpgsqlValue Get(int ordinal);

        /// <summary>
        /// Places our position at the beginning of the given column, after the 4-byte length.
        /// The length is available in ColumnLen.
        /// </summary>
        protected abstract void SeekToColumn(int column);
        protected abstract void SeekInColumn(int posInColumn);
        protected abstract void SeekInColumnText(int textPosInColumn);

        internal RowDescriptionMessage Description { get; set; }
        internal abstract void Consume();
        public BackEndMessageCode Code { get { return BackEndMessageCode.DataRow; } }

        protected DataRowMessageBase(NpgsqlBufferedStream buf)
        {
            Buffer = buf;
            Column = -1;    
        }

        internal bool IsDBNull(int column)
        {
            CheckColumnIndex(column);
            SeekToColumn(column);
            return ColumnLen == -1;
        }

        #region Bytea

        internal long GetBytes(int column, int decodedPosInColumn, byte[] output, int ouputOffset, int decodedLen)
        {
            CheckNotStreaming();
            CheckColumnIndex(column);
            CheckBytea(column);

            SeekToColumn(column);

            if (ColumnLen == -1) {
                // TODO: What is the actual required behavior here?
                throw new Exception("null");
            }

            if (Description[column].IsBinaryFormat)
            {
                if (output == null) {
                    return ColumnLen;
                }

                // In binary format the encoded position is the same as the byte position
                var posInColumn = decodedPosInColumn;
                var len = decodedLen;

                SeekInColumn(posInColumn);

                // Attempt to read beyond the end of the column
                if (posInColumn + len > ColumnLen)
                {
                    // TODO: What is the actual required behavior here?
                    len = ColumnLen - posInColumn;
                }

                Buffer.ReadBytes(output, ouputOffset, len, true);
                PosInColumn += len;

                return len;
            }
            else
            {
                if (PosInColumn == 0) {
                    ParseTextualByteaHeader();
                }

                if (ByteaTextFormat != ByteaTextFormat.Hex) {
                    throw new NotImplementedException("Traditional bytea text escape encoding not (yet) implemented");
                }

                if (output == null) {
                    return DecodedColumnLen;
                }

                // Translate content position to the byte position: 2-byte header, 2 bytes per encoded byte
                var posInColumn = 2 + decodedPosInColumn * 2;
                SeekInColumn(posInColumn);
                DecodedPosInColumn = decodedPosInColumn;

                var len = decodedLen * 2;

                // Attempt to read beyond the end of the column
                if (decodedPosInColumn + decodedLen > DecodedColumnLen)
                {
                    // TODO: What is the actual required behavior here?
                    throw new Exception("Attempt to read beyond end of column");
                }

                var read = Buffer.ReadBytesHex(output, ouputOffset, decodedLen, true);
                Debug.Assert(read == decodedLen);
                DecodedPosInColumn += decodedLen;
                PosInColumn += len;

                return decodedLen;
            }
        }

        void ParseTextualByteaHeader()
        {
            Buffer.Ensure(2);

            // Must start with a backslash
            if (Buffer.ReadByte() != (byte)'\\') {
                throw new Exception("Unrecognized bytea text encoding");
            }
            if (Buffer.ReadByte() != (byte)'x') {
                ByteaTextFormat = ByteaTextFormat.Escape;
                throw new NotImplementedException("Traditional bytea text escape encoding not (yet) implemented");
            }
            ByteaTextFormat = ByteaTextFormat.Hex;
            PosInColumn = 2;
            DecodedPosInColumn = 0;
            DecodedColumnLen = (ColumnLen - 2) / 2;            
        }

        internal Stream GetStream(int column)
        {
            CheckNotStreaming();
            CheckColumnIndex(column);
            CheckBytea(column);
            SeekToColumn(column);
            SeekInColumn(0);
            IsStreaming = true;
            try
            {
                if (Description[column].IsBinaryFormat)
                {
                    DecodedColumnLen = ColumnLen;
                    DecodedPosInColumn = PosInColumn;
                    return new ByteaBinaryStream(this);
                }
                else
                {
                    ParseTextualByteaHeader();
                    return new ByteaHexStream(this);
                }
            }
            catch
            {
                IsStreaming = false;
                throw;
            }
        }

        #endregion

        #region Text

        internal long GetChars(int column, int textPosInColumn, char[] output, int outputOffset, int len)
        {
            CheckNotStreaming();
            CheckColumnIndex(column);
            CheckText(column);

            SeekToColumn(column);

            if (ColumnLen == -1)
            {
                // TODO: What is the actual required behavior here?
                throw new Exception("null");
            }

            if (output == null) {
                // TODO: In non-sequential mode, getting the length can be implemented by decoding the entire
                // field - very inefficient. In sequential mode doing this prevents subsequent reading of the column.
                throw new NotSupportedException("Cannot get length of a text field");
            }

            SeekInColumnText(textPosInColumn);
            int bytesRead, charsRead;
            Buffer.ReadChars(output, outputOffset, len, ColumnLen - PosInColumn, out bytesRead, out charsRead);
            PosInColumn += bytesRead;
            DecodedPosInColumn += charsRead;
            return charsRead;
        }

        internal TextReader GetTextReader(int column)
        {
            CheckNotStreaming();
            CheckColumnIndex(column);
            CheckText(column);
            SeekToColumn(column);
            SeekInColumn(0);
            IsStreaming = true;
            try
            {
                return new StreamReader(new ByteaBinaryStream(this));
            }
            catch
            {
                IsStreaming = false;
                throw;
            }
        }

        #endregion

        #region Checks

        protected void CheckNotStreaming()
        {
            if (IsStreaming)
            {
                throw new InvalidOperationException("A stream is open on a column, close it first");
            }
        }

        protected void CheckColumnIndex(int column)
        {
            if (column < 0 || column >= Description.NumFields) {
                throw new IndexOutOfRangeException("Column index out of range");
            }
        }

        protected void CheckBytea(int column)
        {
            var typeHandler = Description[column].Handler;
            if (!(typeHandler is ByteaHandler))
            {
                throw new InvalidCastException(String.Format("Column type must be bytea (was {0})", typeHandler.PgName));
            }
        }

        protected void CheckText(int column)
        {
            var typeHandler = Description[column].Handler;
            if (!(typeHandler is StringHandler))
            {
                throw new InvalidCastException(String.Format("Column type must be string (was {0})", typeHandler.PgName));
            }
        }

        #endregion

        abstract class ByteaStream : Stream
        {
            protected readonly DataRowMessageBase Row;

            protected ByteaStream(DataRowMessageBase row)
            {
                Row = row;
            }

            public override void Close()
            {
                Row.IsStreaming = false;
            }

            public override long Length
            {
                get { return Row.DecodedColumnLen; }
            }

            public override long Position
            {
                get { return Row.DecodedPosInColumn; }
                set { throw new NotSupportedException(); }
            }

            public override bool CanRead { get { return true; } }
            public override bool CanSeek { get { return false; } }
            public override bool CanWrite { get { return false; } }

            #region Not Supported

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        class ByteaBinaryStream : ByteaStream
        {
            public ByteaBinaryStream(DataRowMessageBase row) : base(row) {}

            public override int Read(byte[] buffer, int offset, int count)
            {
                count = Math.Min(count, Row.ColumnLen - Row.PosInColumn);
                var read = Row.Buffer.ReadBytes(buffer, offset, count, false);
                Row.PosInColumn += read;
                Row.DecodedPosInColumn += read;
                return read;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        class ByteaHexStream : ByteaStream
        {
            public ByteaHexStream(DataRowMessageBase row) : base(row) {}

            public override int Read(byte[] buffer, int offset, int count)
            {
                count = Math.Min(count, Row.ColumnLen - Row.PosInColumn);
                var decodedRead = Row.Buffer.ReadBytesHex(buffer, offset, count, false);
                Row.DecodedPosInColumn += decodedRead;
                Row.PosInColumn += decodedRead * 2;
                return decodedRead;
            }

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Indicates whether bytea text encoding uses the traditional escape format or the newer hex format.
    /// http://www.postgresql.org/docs/current/static/datatype-binary.html
    /// </summary>
    enum ByteaTextFormat
    {
        /// <summary>
        /// The newer hex format (the default since Postgresql 9.0)
        /// </summary>
        Hex,
        /// <summary>
        /// The traditional escape format
        /// </summary>
        Escape
    }
}
