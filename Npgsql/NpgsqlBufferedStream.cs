using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    internal class NpgsqlBufferedStream
    {
        internal Stream Underlying { get; private set; }
        internal int Size { get; private set; }
        internal Encoding TextEncoding { get; private set; }
        internal int Position { get; private set; }
        internal int BytesLeft { get { return _filledBytes - Position; } }

        internal readonly byte[] _buf;
        int _filledBytes;
        readonly Decoder _textDecoder;

        readonly byte[] _workspace = new byte[8];

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        char[] _tempCharBuf = new char[1024];

        internal const int MinimumBufferSize = 1024;
        internal const int DefaultBufferSize = 8192;

        internal NpgsqlBufferedStream(Stream underlying)
            : this(underlying, DefaultBufferSize, Encoding.UTF8) {}

        internal NpgsqlBufferedStream(Stream underlying, int size, Encoding textEncoding)
        {
            if (size < MinimumBufferSize) {
                throw new ArgumentOutOfRangeException("size", size, "Buffer size must be at least " + MinimumBufferSize);
            }

            Underlying = underlying;
            Size = size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
        }

        internal void Ensure(int count, bool readBeyondCount=true)
        {
            Debug.Assert(count <= Size);
            count -= BytesLeft;
            if (count <= 0) { return; }

            if (Position == _filledBytes) {
                Clear();
            } else if (count > Size - _filledBytes) {
                Array.Copy(_buf, Position, _buf, 0, BytesLeft);
                _filledBytes = BytesLeft;
                Position = 0;
            }

            while (count > 0)
            {
                var toRead = readBeyondCount ? Size - _filledBytes : count;
                var read = Underlying.Read(_buf, _filledBytes, toRead);
                if (read == 0) { throw new EndOfStreamException(); }
                count -= read;
                _filledBytes += read;
            }
        }

        internal void CopyTo(NpgsqlBufferedStream other)
        {
            Debug.Assert(other.Size - other._filledBytes >= BytesLeft);
            Array.Copy(_buf, Position, other._buf, other._filledBytes, BytesLeft);
            other._filledBytes += BytesLeft;
        }

        internal void Clear()
        {
            Position = 0;
            _filledBytes = 0;
        }

        /// <summary>
        /// Seeks within the current in-memory data. Does not read any data from the underlying.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        internal void Seek(int offset, SeekOrigin origin)
        {
            int absoluteOffset;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    absoluteOffset = offset;
                    break;
                case SeekOrigin.Current:
                    absoluteOffset = Position + offset;
                    break;
                case SeekOrigin.End:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            Debug.Assert(absoluteOffset >= 0 && absoluteOffset <= _filledBytes);

            Position = absoluteOffset;
        }

        #region Read

        internal byte ReadByte()
        {
            Debug.Assert(BytesLeft >= sizeof(byte));
            return _buf[Position++];
        }

        internal short ReadInt16()
        {
            Debug.Assert(BytesLeft >= sizeof(short));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_buf, Position));
            Position += 2;
            return result;
        }

        internal int ReadInt32()
        {
            Debug.Assert(BytesLeft >= sizeof(int));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_buf, Position));
            Position += 4;
            return result;
        }

        internal long ReadInt64()
        {
            Debug.Assert(BytesLeft >= sizeof(long));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_buf, Position));
            Position += 8;
            return result;
        }

        internal float ReadSingle()
        {
            Debug.Assert(BytesLeft >= sizeof(float));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[3] = _buf[Position++];
                _workspace[2] = _buf[Position++];
                _workspace[1] = _buf[Position++];
                _workspace[0] = _buf[Position++];
                return BitConverter.ToSingle(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToSingle(_buf, Position);
                Position += 4;
                return result;
            }
        }

        internal double ReadDouble()
        {
            Debug.Assert(BytesLeft >= sizeof(double));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[7] = _buf[Position++];
                _workspace[6] = _buf[Position++];
                _workspace[5] = _buf[Position++];
                _workspace[4] = _buf[Position++];
                _workspace[3] = _buf[Position++];
                _workspace[2] = _buf[Position++];
                _workspace[1] = _buf[Position++];
                _workspace[0] = _buf[Position++];
                return BitConverter.ToDouble(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToDouble(_buf, Position);
                Position += 8;
                return result;
            }
        }

        internal string ReadString(int len)
        {
            Debug.Assert(BytesLeft >= len);
            var result = TextEncoding.GetString(_buf, Position, len);
            Position += len;
            return result;
        }

        internal string ReadNullTerminatedString()
        {
            int i;
            for (i = Position; _buf[i] != 0; i++) {
                Debug.Assert(i <= Position + BytesLeft);
            }
            Debug.Assert(i > Position);
            var result = TextEncoding.GetString(_buf, Position, i - Position);
            Position = i + 1;
            return result;
        }

        /// <summary>
        /// </summary>
        /// and reading directly from the underlying stream
        /// <param name="readAll">whether to loop internally until all bytes are read,
        /// or return after a single read to the underlying stream</param>
        internal int ReadBytes(byte[] output, int outputOffset, int len, bool readAll)
        {
            if (len <= BytesLeft)
            {
                Array.Copy(_buf, Position, output, outputOffset, len);
                Position += len;
                return len;
            }

            Array.Copy(_buf, Position, output, outputOffset, BytesLeft);
            var offset = outputOffset + BytesLeft;
            var totalRead = BytesLeft;
            Clear();
            while (totalRead < len)
            {
                var read = Underlying.Read(output, offset, len - totalRead);
                if (read == 0) { throw new EndOfStreamException(); }
                totalRead += read;
                if (!readAll) { return totalRead; }
                offset += read;
            }
            return len;
        }

        /// <summary>
        /// Note that unlike the primitive readers, this reader can read any length, looping internally
        /// and reading directly from the underlying stream
        /// </summary>
        /// <param name="output"></param>
        /// <param name="outputOffset"></param>
        /// <param name="len">(decoded) number of bytes to fill in <paramref name="output"/></param>
        /// <param name="readAll">whether to loop internally until all bytes are read,
        /// or return after a single read to the underlying stream</param>
        internal int ReadBytesHex(byte[] output, int outputOffset, int len, bool readAll)
        {
            var encodedLen = len * 2;
            var pass = 0;
            var totalRead = 0;
            while (true)
            {
                var bytesToProcess = Math.Min(encodedLen - totalRead, BytesLeft % 2 == 0 ? BytesLeft : BytesLeft - 1);
                for (var i = 0; i < bytesToProcess; i += 2) {
                    output[outputOffset++] = (byte)((DecodeHex(_buf[Position++]) << 4) | DecodeHex(_buf[Position++]));
                }
                totalRead += bytesToProcess;
                if (totalRead == encodedLen || (!readAll && ++pass == 2))
                {
                    return totalRead / 2;
                }
                Ensure(1);
            }
        }

        /// <summary>
        /// Note that unlike the primitive readers, this reader can read any length, looping internally
        /// and reading directly from the underlying stream.
        /// </summary>
        /// <param name="output">output buffer to fill</param>
        /// <param name="outputOffset">offset in the output buffer in which to start writing</param>
        /// <param name="charCount">number of character to be read into the output buffer</param>
        /// <param name="byteCount">number of bytes left in the field. This method will not read bytes
        /// beyond this count</param>
        /// <returns>the number of bytes read</returns>
        internal void ReadChars(char[] output, int outputOffset, int charCount, int byteCount, out int bytesRead, out int charsRead)
        {
            Debug.Assert(charCount <= output.Length - outputOffset);

            bytesRead = 0;
            charsRead = 0;
            if (charCount == 0) { return; }
            if (BytesLeft == 0) {
                // If there are no bytes in the buffer, read some before starting the loop
                Ensure(1);
            }

            try
            {
                while (true)
                {
                    int bytesUsed, charsUsed;
                    bool completed;
                    var maxBytes = Math.Min(byteCount - bytesRead, BytesLeft);
                    _textDecoder.Convert(_buf, Position, maxBytes, output, outputOffset, charCount - charsRead, false,
                                         out bytesUsed, out charsUsed, out completed);
                    Position += bytesUsed;
                    bytesRead += bytesUsed;
                    charsRead += charsUsed;
                    if (charsRead == charCount || bytesRead == byteCount) {
                        return;
                    }
                    outputOffset += charsUsed;
                    Clear();
                    Ensure(1); // Read in more data
                }
            }
            finally
            {
                _textDecoder.Reset();
            }
        }

        /// <summary>
        /// Skips over characters in the buffer, reading from the underlying stream as necessary.
        /// </summary>
        /// <param name="charCount">the number of characters to skip over</param>
        /// <param name="byteCount">the maximal number of bytes to process</param>
        /// <returns>the number of bytes read</returns>
        internal void SkipChars(int charCount, int byteCount, out int bytesSkipped, out int charsSkipped)
        {
            ReadChars(_tempCharBuf, 0, charCount, byteCount, out bytesSkipped, out charsSkipped);
        }

        #endregion

        internal void Skip(long len)
        {
            if (len > BytesLeft)
            {
                len -= BytesLeft;
                while (len > Size)
                {
                    Clear();
                    Ensure(Size);
                    len -= Size;
                }
                Clear();
                Ensure((int)len);
            }

            Position += (int)len;
        }

        #region Utilities

        /// <summary>
        /// Decodes a byte in bytea hex format. Each byte contains a single ASCII hex digit.
        /// </summary>
        static byte DecodeHex(byte a)
        {
            switch (a)
            {
                case (byte)'0':
                    return 0x0;
                case (byte)'1':
                    return 0x1;
                case (byte)'2':
                    return 0x2;
                case (byte)'3':
                    return 0x3;
                case (byte)'4':
                    return 0x4;
                case (byte)'5':
                    return 0x5;
                case (byte)'6':
                    return 0x6;
                case (byte)'7':
                    return 0x7;
                case (byte)'8':
                    return 0x8;
                case (byte)'9':
                    return 0x9;
                case (byte)'a':
                    return 0xa;
                case (byte)'b':
                    return 0xb;
                case (byte)'c':
                    return 0xc;
                case (byte)'d':
                    return 0xd;
                case (byte)'e':
                    return 0xe;
                case (byte)'f':
                    return 0xf;
                default:
                    throw new ArgumentOutOfRangeException("Invalid hex byte");
            }
        }

        #endregion
    }
}
