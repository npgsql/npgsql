using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Messages;

namespace Npgsql.TypeHandlers
{
    internal class ByteaHandler : TypeHandler
    {
        static readonly string[] _pgNames = { "bytea" };
        internal override string[] PgNames { get { return _pgNames; } }
        internal override bool SupportsBinaryRead { get { return true; } }

        public long GetBytes(DataRowMessageBase row, int dataOffset, byte[] output, int bufferOffset, int len, FieldDescription field)
        {
            if (field.IsBinaryFormat) {
                return GetBytesBinary(row, dataOffset, output, bufferOffset, len);
            }

            if (row.PosInColumn == 0) {
                ParseTextualByteaHeader(row);
            }

            switch (row.CurrentByteaTextFormat)
            {
                case ByteaTextFormat.Hex:
                    return GetBytesHex(row, dataOffset, output, bufferOffset, len);
                case ByteaTextFormat.Escape:
                    throw new NotImplementedException("Traditional bytea text escape encoding not (yet) implemented");
                default:
                    throw new ArgumentOutOfRangeException("Unknown bytea text encoding: " + row.CurrentByteaTextFormat);
            }
        }

        internal long GetBytesBinary(DataRowMessageBase row, int offset, byte[] output, int outputOffset, int len)
        {
            if (output == null) {
                return row.ColumnLen;
            }

            row.SeekInColumn(offset);

            // Attempt to read beyond the end of the column
            if (offset + len > row.ColumnLen) {
                len = row.ColumnLen - offset;
            }

            row.Buffer.ReadBytes(output, outputOffset, len, true);
            row.PosInColumn += len;
            return len;
        }

        internal long GetBytesHex(DataRowMessageBase row, int decodedOffset, byte[] output, int outputOffset, int decodedLen)
        {
            if (output == null) {
                return row.DecodedColumnLen;
            }

            // Translate content position to the byte position: 2-byte header, 2 bytes per encoded byte
            var posInColumn = 2 + decodedOffset * 2;
            row.SeekInColumn(posInColumn);
            row.DecodedPosInColumn = decodedOffset;

            // Attempt to read beyond the end of the column
            if (decodedOffset + decodedLen > row.DecodedColumnLen) {
                decodedLen = row.DecodedColumnLen - decodedOffset;
            }

            var read = row.Buffer.ReadBytesHex(output, outputOffset, decodedLen, true);
            Debug.Assert(read == decodedLen);
            row.DecodedPosInColumn += decodedLen;
            row.PosInColumn += decodedLen * 2;

            return decodedLen;
        }

        internal Stream GetStream(DataRowMessageBase row, FieldDescription field)
        {
            if (field.IsBinaryFormat)
            {
                row.DecodedPosInColumn = 0;
                row.DecodedColumnLen = row.ColumnLen;
                return new ByteaBinaryStream(row);
            }
            else
            {
                ParseTextualByteaHeader(row);
                return new ByteaHexStream(row);
            }
        }

        protected void ParseTextualByteaHeader(DataRowMessageBase row)
        {
            row.Buffer.Ensure(2);

            // Must start with a backslash
            if (row.Buffer.ReadByte() != (byte)'\\') {
                throw new Exception("Unrecognized bytea text encoding");
            }

            if (row.Buffer.ReadByte() != (byte)'x')
            {
                row.CurrentByteaTextFormat = ByteaTextFormat.Escape;
                return;
            }

            row.CurrentByteaTextFormat = ByteaTextFormat.Hex;
            row.PosInColumn = 2;
            row.DecodedPosInColumn = 0;
            row.DecodedColumnLen = (row.ColumnLen - 2) / 2;
        }

#if PREVIOUS_IMPLEMENTATION
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal override void ReadText(NpgsqlBufferedStream buf, int len, FieldDescription field, NpgsqlValue output)
        {
            throw new NotImplementedException();

            Int32 byteAPosition = 0;
#if GO
            if (len >= 2 && BackendData[0] == (byte)ASCIIBytes.BackSlash && BackendData[1] == (byte)ASCIIBytes.x)
            {
                // PostgreSQL 8.5+'s bytea_output=hex format
                byte[] result = new byte[(len - 2) / 2];
#if UNSAFE
                unsafe
                {
                    fixed (byte* pBackendData = &BackendData[2])
                    {
                        fixed (byte* pResult = &result[0])
                        {
                            byte* pBackendData2 = pBackendData;
                            byte* pResult2 = pResult;
                            
                            for (byteAPosition = 2 ; byteAPosition < byteALength ; byteAPosition += 2)
                            {
                                *pResult2 = FastConverter.ToByteHexFormat(pBackendData2);
                                pBackendData2 += 2;
                                pResult2++;
                            }
                        }
                    }
                }
#else
                Int32 k = 0;

                for (byteAPosition = 2; byteAPosition < len; byteAPosition += 2)
                {
                    result[k] = FastConverter.ToByteHexFormat(BackendData, byteAPosition);
                    k++;
                }
#endif

                return result;
            }
            else
            {
                byte octalValue = 0;
                MemoryStream ms = new MemoryStream();

                while (byteAPosition < len)
                {
                    // The IsDigit is necessary in case we receive a \ as the octal value and not
                    // as the indicator of a following octal value in decimal format.
                    // i.e.: \201\301P\A
                    if (BackendData[byteAPosition] == (byte)ASCIIBytes.BackSlash)
                    {
                        if (byteAPosition + 1 == len)
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition++;
                        }
                        else if (BackendData[byteAPosition + 1] >= (byte)ASCIIBytes.b0 && BackendData[byteAPosition + 1] <= (byte)ASCIIBytes.b7)
                        {
                            octalValue = FastConverter.ToByteEscapeFormat(BackendData, byteAPosition + 1);
                            byteAPosition += 4;
                        }
                        else
                        {
                            octalValue = (byte)ASCIIBytes.BackSlash;
                            byteAPosition += 2;
                        }
                    }
                    else
                    {
                        octalValue = BackendData[byteAPosition];
                        byteAPosition++;
                    }

                    ms.WriteByte((Byte)octalValue);
                }

                return ms.ToArray();
            }
#endif
        }
#endif

        internal override void Read(DataRowMessageBase row, FieldDescription field, NpgsqlValue output)
        {
            if (field.IsTextFormat)
            {
                ParseTextualByteaHeader(row);
            }
            else
            {
                // TODO: This sucks
                row.DecodedColumnLen = row.ColumnLen;
            }

            var outputBuffer = new byte[row.DecodedColumnLen];
            var read = GetBytes(row, 0, outputBuffer, 0, outputBuffer.Length, field);
            Debug.Assert(read == outputBuffer.Length);
            output.SetTo(outputBuffer);
        }
    }

    #region Streams

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
        public ByteaBinaryStream(DataRowMessageBase row) : base(row) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, Row.ColumnLen - Row.PosInColumn);
            var read = Row.Buffer.ReadBytes(buffer, offset, count, false);
            Row.PosInColumn += read;
            Row.DecodedPosInColumn += read;
            return read;
        }

#if NET45
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
#else
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }
    }

    class ByteaHexStream : ByteaStream
    {
        public ByteaHexStream(DataRowMessageBase row) : base(row) { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = Math.Min(count, Row.ColumnLen - Row.PosInColumn);
            var decodedRead = Row.Buffer.ReadBytesHex(buffer, offset, count, false);
            Row.DecodedPosInColumn += decodedRead;
            Row.PosInColumn += decodedRead * 2;
            return decodedRead;
        }

#if NET45
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
#else
        public Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
#endif
        {
            throw new NotImplementedException();
        }
    }

    #endregion

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
