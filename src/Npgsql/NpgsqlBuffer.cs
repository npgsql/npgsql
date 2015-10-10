#region License
// The PostgreSQL License
//
// Copyright (C) 2015 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using AsyncRewriter;

namespace Npgsql
{
    internal partial class NpgsqlBuffer
    {
        #region Fields and Properties

        internal Stream Underlying { get; set; }
        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; }

        /// <summary>
        /// During copy operations, the buffer's usable size is smaller than its total size because of the CopyData
        /// message header. This distinction is important since some type handlers check how much space is left
        /// in the buffer in their decision making.
        /// </summary>
        internal int UsableSize
        {
            get { return _usableSize; }
            set
            {
                Contract.Requires(value <= Size);
                _usableSize = value;
            }
        }

        int _usableSize;
        internal Encoding TextEncoding { get; }

        internal int ReadPosition { get; private set; }
        internal int ReadBytesLeft => _filledBytes - ReadPosition;

        internal int WritePosition { get { return _writePosition; } set { _writePosition = value; } }
        internal int WriteSpaceLeft => Size - _writePosition;

        internal long TotalBytesFlushed { get; private set; }

        internal readonly byte[] _buf;
        int _filledBytes;
        readonly Decoder _textDecoder;
        readonly Encoder _textEncoder;

        readonly byte[] _workspace;

        int _writePosition;

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        readonly char[] _tempCharBuf;

        BitConverterUnion _bitConverterUnion;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumBufferSize = 4096;
        internal const int DefaultBufferSize = 8192;

        #endregion

        #region Constructors

        internal NpgsqlBuffer(Stream underlying, int size, Encoding textEncoding)
        {
            if (size < MinimumBufferSize) {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumBufferSize);
            }
            Contract.EndContractBlock();

            Underlying = underlying;
            Size = size;
            UsableSize = Size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
            _textEncoder = TextEncoding.GetEncoder();
            _tempCharBuf = new char[1024];
            _workspace = new byte[8];
        }

        #endregion

        #region I/O

        [RewriteAsync]
        internal void Ensure(int count)
        {
            Contract.Requires(count <= Size);
            count -= ReadBytesLeft;
            if (count <= 0) { return; }

            if (ReadPosition == _filledBytes) {
                Clear();
            } else if (count > Size - _filledBytes) {
                Array.Copy(_buf, ReadPosition, _buf, 0, ReadBytesLeft);
                _filledBytes = ReadBytesLeft;
                ReadPosition = 0;
            }

            while (count > 0)
            {
                var toRead = Size - _filledBytes;
                var read = Underlying.Read(_buf, _filledBytes, toRead);
                if (read == 0) { throw new EndOfStreamException(); }
                count -= read;
                _filledBytes += read;
            }
        }

        [RewriteAsync]
        internal void ReadMore()
        {
            Ensure(ReadBytesLeft + 1);
        }

        /// <summary>
        /// Reads in the requested bytes into the buffer, or if the buffer isn't big enough, allocates a new
        /// temporary buffer and reads into it. Returns the buffer that contains the data (either itself or the
        /// temp buffer). Used in cases where we absolutely have to have an entire value in memory and cannot
        /// read it in sequentially.
        /// </summary>
        [RewriteAsync]
        internal NpgsqlBuffer EnsureOrAllocateTemp(int count)
        {
            if (count <= Size) {
                Ensure(count);
                return this;
            }

            // Worst case: our buffer isn't big enough. For now, allocate a new buffer
            // and copy into it
            // TODO: Optimize with a pool later?
            var tempBuf = new NpgsqlBuffer(Underlying, count, TextEncoding);
            CopyTo(tempBuf);
            Clear();
            tempBuf.Ensure(count);
            return tempBuf;
        }

        [RewriteAsync]
        internal void Skip(long len)
        {
            Contract.Requires(len >= 0);

            if (len > ReadBytesLeft)
            {
                len -= ReadBytesLeft;
                while (len > Size)
                {
                    Clear();
                    Ensure(Size);
                    len -= Size;
                }
                Clear();
                Ensure((int)len);
            }

            ReadPosition += (int)len;
        }

        [RewriteAsync]
        public void Flush()
        {
            if (_writePosition != 0)
            {
                Contract.Assert(ReadBytesLeft == 0, "There cannot be read bytes buffered while a write operation is going on.");
                Underlying.Write(_buf, 0, _writePosition);
                Underlying.Flush();
                TotalBytesFlushed += _writePosition;
                _writePosition = 0;
            }
        }

        #endregion

        #region Read Simple

        internal byte ReadByte()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(byte));
            return _buf[ReadPosition++];
        }

        internal short ReadInt16()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(short));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_buf, ReadPosition));
            ReadPosition += 2;
            return result;
        }

        internal int ReadInt32()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(int));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_buf, ReadPosition));
            ReadPosition += 4;
            return result;
        }

        internal uint ReadUInt32()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(int));
            var result = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_buf, ReadPosition));
            ReadPosition += 4;
            return result;
        }

        internal long ReadInt64()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(long));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(_buf, ReadPosition));
            ReadPosition += 8;
            return result;
        }

        internal float ReadSingle()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(float));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[3] = _buf[ReadPosition++];
                _workspace[2] = _buf[ReadPosition++];
                _workspace[1] = _buf[ReadPosition++];
                _workspace[0] = _buf[ReadPosition++];
                return BitConverter.ToSingle(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToSingle(_buf, ReadPosition);
                ReadPosition += 4;
                return result;
            }
        }

        internal double ReadDouble()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(double));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[7] = _buf[ReadPosition++];
                _workspace[6] = _buf[ReadPosition++];
                _workspace[5] = _buf[ReadPosition++];
                _workspace[4] = _buf[ReadPosition++];
                _workspace[3] = _buf[ReadPosition++];
                _workspace[2] = _buf[ReadPosition++];
                _workspace[1] = _buf[ReadPosition++];
                _workspace[0] = _buf[ReadPosition++];
                return BitConverter.ToDouble(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToDouble(_buf, ReadPosition);
                ReadPosition += 8;
                return result;
            }
        }

        internal string ReadString(int byteLen)
        {
            Contract.Requires(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetString(_buf, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        internal char[] ReadChars(int byteLen)
        {
            Contract.Requires(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetChars(_buf, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        internal void ReadBytes(byte[] output, int outputOffset, int len)
        {
            Contract.Requires(len <= ReadBytesLeft);
            Buffer.BlockCopy(_buf, ReadPosition, output, outputOffset, len);
            ReadPosition += len;
        }

        #endregion

        #region Read Complex

        [RewriteAsync]
        internal int ReadAllBytes(byte[] output, int outputOffset, int len, bool readOnce)
        {
            if (len <= ReadBytesLeft)
            {
                Array.Copy(_buf, ReadPosition, output, outputOffset, len);
                ReadPosition += len;
                return len;
            }

            Array.Copy(_buf, ReadPosition, output, outputOffset, ReadBytesLeft);
            var offset = outputOffset + ReadBytesLeft;
            var totalRead = ReadBytesLeft;
            Clear();
            while (totalRead < len)
            {
                var read = Underlying.Read(output, offset, len - totalRead);
                if (read == 0) { throw new EndOfStreamException(); }
                totalRead += read;
                if (readOnce) { return totalRead; }
                offset += read;
            }
            return len;
        }

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        internal string ReadNullTerminatedString()
        {
            return ReadNullTerminatedString(TextEncoding);
        }

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        /// <param name="encoding">Decodes the messages with this encoding.</param>
        internal string ReadNullTerminatedString(Encoding encoding)
        {
            int i;
            for (i = ReadPosition; _buf[i] != 0; i++)
            {
                Contract.Assume(i <= ReadPosition + ReadBytesLeft);
            }
            Contract.Assert(i >= ReadPosition);
            var result = encoding.GetString(_buf, ReadPosition, i - ReadPosition);
            ReadPosition = i + 1;
            return result;
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
        /// <param name="bytesRead">The number of bytes actually read.</param>
        /// <param name="charsRead">The number of characters actually read.</param>
        /// <returns>the number of bytes read</returns>
        internal void ReadAllChars(char[] output, int outputOffset, int charCount, int byteCount, out int bytesRead, out int charsRead)
        {
            Contract.Requires(charCount <= output.Length - outputOffset);

            bytesRead = 0;
            charsRead = 0;
            if (charCount == 0) { return; }

            try
            {
                while (true)
                {
                    Ensure(1); // Make sure we have at least some data

                    int bytesUsed, charsUsed;
                    bool completed;
                    var maxBytes = Math.Min(byteCount - bytesRead, ReadBytesLeft);
                    _textDecoder.Convert(_buf, ReadPosition, maxBytes, output, outputOffset, charCount - charsRead, false,
                                         out bytesUsed, out charsUsed, out completed);
                    ReadPosition += bytesUsed;
                    bytesRead += bytesUsed;
                    charsRead += charsUsed;
                    if (charsRead == charCount || bytesRead == byteCount) {
                        return;
                    }
                    outputOffset += charsUsed;
                    Clear();
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
        /// <param name="charCount">the number of characters to skip over.
        /// int.MaxValue means all available characters (limited only by <paramref name="byteCount"/>).
        /// </param>
        /// <param name="byteCount">the maximal number of bytes to process</param>
        /// <param name="bytesSkipped">The number of bytes actually skipped.</param>
        /// <param name="charsSkipped">The number of characters actually skipped.</param>
        /// <returns>the number of bytes read</returns>
        internal void SkipChars(int charCount, int byteCount, out int bytesSkipped, out int charsSkipped)
        {
            charsSkipped = bytesSkipped = 0;
            while (charsSkipped < charCount && bytesSkipped < byteCount)
            {
                int bSkipped, cSkipped;
                ReadAllChars(_tempCharBuf, 0, Math.Min(charCount, _tempCharBuf.Length), byteCount, out bSkipped, out cSkipped);
                charsSkipped += cSkipped;
                bytesSkipped += bSkipped;
            }
        }

        #endregion

        #region Write Simple

        public void WriteByte(byte b)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(byte));
            _buf[_writePosition++] = b;
        }

        public void WriteInt16(int i)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(short));
            _buf[_writePosition++] = (byte)(i >> 8);
            _buf[_writePosition++] = (byte)i;
        }

        public void WriteInt32(int i)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(int));
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;
        }

        internal void WriteUInt32(uint i)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(uint));
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;
        }

        public void WriteInt64(long i)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(long));
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 56);
            _buf[pos++] = (byte)(i >> 48);
            _buf[pos++] = (byte)(i >> 40);
            _buf[pos++] = (byte)(i >> 32);
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;
        }

        public void WriteSingle(float f)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(float));
            _bitConverterUnion.float4 = f;
            var pos = _writePosition;
            if (BitConverter.IsLittleEndian)
            {
                _buf[pos++] = _bitConverterUnion.b3;
                _buf[pos++] = _bitConverterUnion.b2;
                _buf[pos++] = _bitConverterUnion.b1;
                _buf[pos++] = _bitConverterUnion.b0;
            }
            else
            {
                _buf[pos++] = _bitConverterUnion.b0;
                _buf[pos++] = _bitConverterUnion.b1;
                _buf[pos++] = _bitConverterUnion.b2;
                _buf[pos++] = _bitConverterUnion.b3;
            }
            _writePosition = pos;
        }

        public void WriteDouble(double d)
        {
            Contract.Requires(WriteSpaceLeft >= sizeof(double));
            _bitConverterUnion.float8 = d;
            var pos = _writePosition;
            if (BitConverter.IsLittleEndian)
            {
                _buf[pos++] = _bitConverterUnion.b7;
                _buf[pos++] = _bitConverterUnion.b6;
                _buf[pos++] = _bitConverterUnion.b5;
                _buf[pos++] = _bitConverterUnion.b4;
                _buf[pos++] = _bitConverterUnion.b3;
                _buf[pos++] = _bitConverterUnion.b2;
                _buf[pos++] = _bitConverterUnion.b1;
                _buf[pos++] = _bitConverterUnion.b0;
            }
            else
            {
                _buf[pos++] = _bitConverterUnion.b0;
                _buf[pos++] = _bitConverterUnion.b1;
                _buf[pos++] = _bitConverterUnion.b2;
                _buf[pos++] = _bitConverterUnion.b3;
                _buf[pos++] = _bitConverterUnion.b4;
                _buf[pos++] = _bitConverterUnion.b5;
                _buf[pos++] = _bitConverterUnion.b6;
                _buf[pos++] = _bitConverterUnion.b7;
            }
            _writePosition = pos;
        }

        internal void WriteString(string s, int len = 0)
        {
            Contract.Requires(TextEncoding.GetByteCount(s) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(s, 0, len == 0 ? s.Length : len, _buf, WritePosition);
        }

        internal void WriteChars(char[] chars, int len = 0)
        {
            Contract.Requires(TextEncoding.GetByteCount(chars) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(chars, 0, len == 0 ? chars.Length : len, _buf, WritePosition);
        }

        public void WriteBytes(byte[] buf, int offset, int count)
        {
            Contract.Requires(count <= WriteSpaceLeft);
            Buffer.BlockCopy(buf, offset, _buf, WritePosition, count);
            WritePosition += count;
        }

        public void WriteBytesNullTerminated(byte[] buf)
        {
            Contract.Requires(WriteSpaceLeft >= buf.Length + 1);
            WriteBytes(buf, 0, buf.Length);
            WriteByte(0);
        }

        #endregion

        #region Write Complex

        internal void WriteStringChunked(char[] chars, int charIndex, int charCount,
                                         bool flush, out int charsUsed, out bool completed)
        {
            int bytesUsed;
            _textEncoder.Convert(chars, charIndex, charCount, _buf, WritePosition, WriteSpaceLeft,
                                 flush, out charsUsed, out bytesUsed, out completed);
            WritePosition += bytesUsed;
        }

        #endregion

        #region Misc

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
                    absoluteOffset = ReadPosition + offset;
                    break;
                case SeekOrigin.End:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }
            Contract.Assert(absoluteOffset >= 0 && absoluteOffset <= _filledBytes);

            ReadPosition = absoluteOffset;
        }

        internal void Clear()
        {
            WritePosition = 0;
            ReadPosition = 0;
            _filledBytes = 0;
        }

        internal void CopyTo(NpgsqlBuffer other)
        {
            Contract.Assert(other.Size - other._filledBytes >= ReadBytesLeft);
            Array.Copy(_buf, ReadPosition, other._buf, other._filledBytes, ReadBytesLeft);
            other._filledBytes += ReadBytesLeft;
        }

        internal MemoryStream GetMemoryStream(int len)
        {
            return new MemoryStream(_buf, ReadPosition, len, false, false);
        }

        internal void ResetTotalBytesFlushed()
        {
            TotalBytesFlushed = 0;
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct BitConverterUnion
        {
            [FieldOffset(0)] public readonly byte b0;
            [FieldOffset(1)] public readonly byte b1;
            [FieldOffset(2)] public readonly byte b2;
            [FieldOffset(3)] public readonly byte b3;
            [FieldOffset(4)] public readonly byte b4;
            [FieldOffset(5)] public readonly byte b5;
            [FieldOffset(6)] public readonly byte b6;
            [FieldOffset(7)] public readonly byte b7;

            [FieldOffset(0)] public float float4;
            [FieldOffset(0)] public double float8;
        }

        #endregion

        #region Postgis

        internal int ReadInt32(ByteOrder bo)
        {
            Contract.Requires(ReadBytesLeft >= sizeof(int));
            int result;
            if (BitConverter.IsLittleEndian == (bo == ByteOrder.LSB))
            {
                result = BitConverter.ToInt32(_buf, ReadPosition);
                ReadPosition += 4;
            }
            else
            {
                _workspace[3] = _buf[ReadPosition++];
                _workspace[2] = _buf[ReadPosition++];
                _workspace[1] = _buf[ReadPosition++];
                _workspace[0] = _buf[ReadPosition++];
                result = BitConverter.ToInt32(_workspace, 0);
            }
            return result;
        }

        internal uint ReadUInt32(ByteOrder bo)
        {
            Contract.Requires(ReadBytesLeft >= sizeof(int));
            uint result;
            if (BitConverter.IsLittleEndian == (bo == ByteOrder.LSB))
            {
                result = BitConverter.ToUInt32(_buf, ReadPosition);
                ReadPosition += 4;
            }
            else
            {
                _workspace[3] = _buf[ReadPosition++];
                _workspace[2] = _buf[ReadPosition++];
                _workspace[1] = _buf[ReadPosition++];
                _workspace[0] = _buf[ReadPosition++];
                result = BitConverter.ToUInt32(_workspace, 0);
            }
            return result;
        }

        internal double ReadDouble(ByteOrder bo)
        {
            Contract.Requires(ReadBytesLeft >= sizeof(double));

            if (BitConverter.IsLittleEndian == (ByteOrder.LSB == bo))
            {
                var result = BitConverter.ToDouble(_buf, ReadPosition);
                ReadPosition += 8;
                return result;
            }
            else
            {
                _workspace[7] = _buf[ReadPosition++];
                _workspace[6] = _buf[ReadPosition++];
                _workspace[5] = _buf[ReadPosition++];
                _workspace[4] = _buf[ReadPosition++];
                _workspace[3] = _buf[ReadPosition++];
                _workspace[2] = _buf[ReadPosition++];
                _workspace[1] = _buf[ReadPosition++];
                _workspace[0] = _buf[ReadPosition++];
                return BitConverter.ToDouble(_workspace, 0);
            }
        }
        #endregion
    }
}
