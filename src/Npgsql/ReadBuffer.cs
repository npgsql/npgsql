#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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
    internal partial class ReadBuffer
    {
        #region Fields and Properties

        internal Stream Underlying { get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; }

        internal Encoding TextEncoding { get; }

        internal int ReadPosition { get; private set; }
        internal int ReadBytesLeft => _filledBytes - ReadPosition;

        internal readonly byte[] _buf;
        int _filledBytes;
        readonly Decoder _textDecoder;

        readonly byte[] _workspace;

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        readonly char[] _tempCharBuf;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumBufferSize = 4096;
        internal const int DefaultBufferSize = 8192;

        #endregion

        #region Constructors

        internal ReadBuffer(Stream underlying, int size, Encoding textEncoding)
        {
            if (size < MinimumBufferSize) {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumBufferSize);
            }
            Contract.EndContractBlock();

            Underlying = underlying;
            Size = size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
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
        internal ReadBuffer EnsureOrAllocateTemp(int count)
        {
            if (count <= Size) {
                Ensure(count);
                return this;
            }

            // Worst case: our buffer isn't big enough. For now, allocate a new buffer
            // and copy into it
            // TODO: Optimize with a pool later?
            var tempBuf = new ReadBuffer(Underlying, count, TextEncoding);
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

        internal ushort ReadUInt16()
        {
            Contract.Requires(ReadBytesLeft >= sizeof(short));
            var result = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_buf, ReadPosition));
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

        #region Read PostGIS

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
            ReadPosition = 0;
            _filledBytes = 0;
        }

        internal void CopyTo(ReadBuffer other)
        {
            Contract.Assert(other.Size - other._filledBytes >= ReadBytesLeft);
            Array.Copy(_buf, ReadPosition, other._buf, other._filledBytes, ReadBytesLeft);
            other._filledBytes += ReadBytesLeft;
        }

        internal MemoryStream GetMemoryStream(int len)
        {
            return new MemoryStream(_buf, ReadPosition, len, false, false);
        }

        #endregion
    }
}
