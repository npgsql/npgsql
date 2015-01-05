using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql
{
    internal partial class NpgsqlBuffer
    {
        internal Stream Underlying { get; private set; }
        internal int Size { get; private set; }
        internal Encoding TextEncoding { get; private set; }

        internal int ReadPosition { get; private set; }
        internal int ReadBytesLeft { get { return _filledBytes - ReadPosition; } }

        internal int WritePosition { get { return _writePosition; } set { _writePosition = value; } }
        internal int WriteSpaceLeft { get { return Size - _writePosition; } }

        internal long TotalBytesFlushed { get; private set; }

        internal byte[] _buf;
        int _filledBytes;
        readonly Decoder _textDecoder;
        readonly Encoder _textEncoder;

        readonly byte[] _workspace;

        int _writePosition;

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        char[] _tempCharBuf;

        BitConverterUnion _bitConverterUnion = new BitConverterUnion();

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumBufferSize = 4096;
        internal const int DefaultBufferSize = 8192;

        #region Constructors

        internal NpgsqlBuffer(Stream underlying)
            : this(underlying, DefaultBufferSize, Encoding.UTF8) {}

        internal NpgsqlBuffer(Stream underlying, int size, Encoding textEncoding)
        {
            if (size < MinimumBufferSize) {
                throw new ArgumentOutOfRangeException("size", size, "Buffer size must be at least " + MinimumBufferSize);
            }
            Contract.EndContractBlock();

            Underlying = underlying;
            Size = size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
            _textEncoder = TextEncoding.GetEncoder();
            _tempCharBuf = new char[1024];
            _workspace = new byte[8];
        }

        /// <summary>
        /// A total hack for reading elements of text-encoded arrays
        /// </summary>
        internal NpgsqlBuffer(string content, Encoding textEncoding)
        {
            TextEncoding = textEncoding;
            _buf = TextEncoding.GetBytes(content);
            Size = _filledBytes = _buf.Length;
        }

        #endregion

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

        /// <summary>
        /// Reads in the requested bytes into the buffer, or if the buffer isn't big enough, allocates a new
        /// temporary buffer and reads into it. Returns the buffer that contains the data (either itself or the
        /// temp buffer). Used in cases where we absolutely have to have an entire value in memory and cannot
        /// read it in sequentially.
        /// </summary>
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

        internal void CopyTo(NpgsqlBuffer other)
        {
            Contract.Assert(other.Size - other._filledBytes >= ReadBytesLeft);
            Array.Copy(_buf, ReadPosition, other._buf, other._filledBytes, ReadBytesLeft);
            other._filledBytes += ReadBytesLeft;
        }

        internal void Clear()
        {
            ReadPosition = 0;
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
                    absoluteOffset = ReadPosition + offset;
                    break;
                case SeekOrigin.End:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            Debug.Assert(absoluteOffset >= 0 && absoluteOffset <= _filledBytes);

            ReadPosition = absoluteOffset;
        }

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

        #endregion

        /// <summary>
        /// Converts a given number of bytes into a char array and returns it. Expects the required bytes
        /// to already be in the buffer
        /// </summary>
        internal char[] ReadChars(int byteCount)
        {
            if (byteCount <= Size)
            {
                Ensure(byteCount);
                var result = TextEncoding.GetChars(_buf, ReadPosition, byteCount);
                ReadPosition += byteCount;
                return result;
            }

            // Worst case: our buffer isn't big enough. For now, pessimistically allocate a char buffer
            // that will hold the maximum number of characters for the column length
            // TODO: Optimize
            var pessimisticNumChars = TextEncoding.GetMaxCharCount(byteCount);
            var pessimisticOutput = new char[pessimisticNumChars];
            var actualNumChars = PopulateCharArray(pessimisticOutput, byteCount);
            if (actualNumChars == pessimisticNumChars)
                return pessimisticOutput;
            var output = new char[actualNumChars];
            Array.Copy(pessimisticOutput, 0, output, 0, actualNumChars);
            return output;
        }

        /// <summary>
        /// Note that unlike the primitive readers, this reader can read any length, looping internally
        /// and reading directly from the underlying stream
        /// </summary>
        internal string ReadString(int byteCount)
        {
            if (byteCount <= Size)
            {
                Ensure(byteCount);
                var result = TextEncoding.GetString(_buf, ReadPosition, byteCount);
                ReadPosition += byteCount;
                return result;
            }

            // Worst case: our buffer isn't big enough. For now, pessimistically allocate a char buffer
            // that will hold the maximum number of characters for the column length
            // TODO: Optimize
            var pessimisticNumChars = TextEncoding.GetMaxCharCount(byteCount);
            var pessimisticOutput = new char[pessimisticNumChars];
            var actualNumChars = PopulateCharArray(pessimisticOutput, byteCount);
            return new string(pessimisticOutput, 0, actualNumChars);
        }

        int PopulateCharArray(char[] output, int byteCount)
        {
            try
            {
                var totalBytesRead = 0;
                var totalCharsRead = 0;
                var outputOffset = 0;
                while (true)
                {
                    int bytesRead, charsRead;
                    bool completed;
                    var maxBytes = Math.Min(byteCount - totalBytesRead, ReadBytesLeft);
                    _textDecoder.Convert(_buf, ReadPosition, maxBytes, output, outputOffset, output.Length - totalCharsRead, false,
                                         out bytesRead, out charsRead, out completed);
                    ReadPosition += bytesRead;
                    totalBytesRead += bytesRead;
                    totalCharsRead += charsRead;
                    if (totalBytesRead == byteCount)
                    {
                        return totalCharsRead;
                    }
                    outputOffset += charsRead;
                    Clear();
                    Ensure(1); // Read in more data
                }
            }
            finally
            {
                _textDecoder.Reset();
            }
        }

        internal string ReadNullTerminatedString()
        {
            int i;
            for (i = ReadPosition; _buf[i] != 0; i++) {
                Contract.Assume(i <= ReadPosition + ReadBytesLeft);
            }
            Contract.Assert(i >= ReadPosition);
            var result = TextEncoding.GetString(_buf, ReadPosition, i - ReadPosition);
            ReadPosition = i + 1;
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="readAll">whether to loop internally until all bytes are read,
        /// or return after a single read to the underlying stream</param>
        internal int ReadBytes(byte[] output, int outputOffset, int len, bool readAll=false)
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
                // TODO: Use lookup tables as in FastConverter.cs
                var bytesToProcess = Math.Min(encodedLen - totalRead, ReadBytesLeft % 2 == 0 ? ReadBytesLeft : ReadBytesLeft - 1);
                for (var i = 0; i < bytesToProcess; i += 2) {
                    output[outputOffset++] = (byte)((DecodeHex(_buf[ReadPosition++]) << 4) | DecodeHex(_buf[ReadPosition++]));
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
            Contract.Requires(charCount <= output.Length - outputOffset);

            bytesRead = 0;
            charsRead = 0;
            if (charCount == 0) { return; }
            if (ReadBytesLeft == 0) {
                // If there are no bytes in the buffer, read some before starting the loop
                Ensure(1);
            }

            try
            {
                while (true)
                {
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
        /// <param name="charCount">the number of characters to skip over.
        /// int.MaxValue means all available characters (limited only by <paramref name="byteCount"/>).
        /// </param>
        /// <param name="byteCount">the maximal number of bytes to process</param>
        /// <returns>the number of bytes read</returns>
        internal void SkipChars(int charCount, int byteCount, out int bytesSkipped, out int charsSkipped)
        {
            charsSkipped = bytesSkipped = 0;
            while (charsSkipped < charCount && bytesSkipped < byteCount)
            {
                int bSkipped, cSkipped;
                ReadChars(_tempCharBuf, 0, Math.Min(charCount, _tempCharBuf.Length), byteCount, out bSkipped, out cSkipped);
                charsSkipped += cSkipped;
                bytesSkipped += bSkipped;
            }
        }

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

        public void WriteBytesSimple(byte[] buf, int offset, int count)
        {
            Contract.Requires(count <= WriteSpaceLeft);
            Buffer.BlockCopy(buf, offset, _buf, _writePosition, count);
            _writePosition += count;
        }

        [GenerateAsync]
        public void Write(byte[] buf, int offset, int count)
        {
            if (count <= WriteSpaceLeft)
            {
                Buffer.BlockCopy(buf, offset, _buf, _writePosition, count);
                _writePosition += count;
                return;
            }

            if (_writePosition != 0)
            {
                Buffer.BlockCopy(buf, offset, _buf, _writePosition, WriteSpaceLeft);
                offset += WriteSpaceLeft;
                count -= WriteSpaceLeft;

                Underlying.Write(_buf, 0, Size);
                _writePosition = 0;
            }

            if (count >= Size)
            {
                Underlying.Write(buf, offset, count);
            }
            else
            {
                Buffer.BlockCopy(buf, offset, _buf, 0, count);
                _writePosition = count;
            }
        }
        
        [GenerateAsync]
        public void Flush()
        {
            if (_writePosition != 0)
            {
                Contract.Assert(ReadBytesLeft == 0, "There cannot be read bytes buffered while a write operation is going on.");
                Underlying.Write(_buf, 0, _writePosition);
                TotalBytesFlushed += _writePosition;
                _writePosition = 0;
            }
        }

        internal void ResetTotalBytesFlushed()
        {
            TotalBytesFlushed = 0;
        }

        [GenerateAsync]
        public NpgsqlBuffer EnsureWrite(int bytesToWrite)
        {
            Contract.Requires(bytesToWrite <= Size, "Requested write length larger than buffer size");
            if (bytesToWrite > WriteSpaceLeft)
            {
                Flush();
            }
            return this;
        }

        [GenerateAsync]
        public NpgsqlBuffer WriteBytes(byte[] buf)
        {
            Write(buf, 0, buf.Length);
            return this;
        }

        public NpgsqlBuffer WriteBytesNullTerminated(byte[] buf)
        {
            WriteBytes(buf);
            WriteByte(0);

            return this;
        }

        public NpgsqlBuffer WriteByte(byte b)
        {
            Contract.Requires(WriteSpaceLeft > 0);
            _buf[_writePosition++] = b;

            return this;
        }

        public NpgsqlBuffer WriteInt64(long i)
        {
            Contract.Requires(WriteSpaceLeft > 8);
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

            return this;
        }

        public NpgsqlBuffer WriteInt32(int i)
        {
            Contract.Requires(WriteSpaceLeft > 4);
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;

            return this;
        }

        public NpgsqlBuffer WriteInt16(int i)
        {
            Contract.Requires(WriteSpaceLeft > 2);
            _buf[_writePosition++] = (byte)(i >> 8);
            _buf[_writePosition++] = (byte)i;

            return this;
        }

        public NpgsqlBuffer WriteSingle(float f)
        {
            Contract.Requires(WriteSpaceLeft > 0);
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

            return this;
        }

        public NpgsqlBuffer WriteDouble(double d)
        {
            Contract.Requires(WriteSpaceLeft > 0);
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

            return this;
        }

        [GenerateAsync]
        public NpgsqlBuffer EnsuredWriteByte(byte b)
        {
            if (WriteSpaceLeft == 0) Flush();
            _buf[_writePosition++] = b;

            return this;
        }

        [GenerateAsync]
        public NpgsqlBuffer EnsuredWriteInt32(int i)
        {
            if (WriteSpaceLeft < 4) Flush();
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;

            return this;
        }

        [GenerateAsync]
        public NpgsqlBuffer EnsuredWriteInt16(int i)
        {
            if (WriteSpaceLeft < 2) Flush();
            _buf[_writePosition++] = (byte)(i >> 8);
            _buf[_writePosition++] = (byte)i;

            return this;
        }

        internal void WriteStringSimple(string s)
        {
            Contract.Requires(TextEncoding.GetByteCount(s) <= WriteSpaceLeft);

            WritePosition += TextEncoding.GetBytes(s, 0, s.Length, _buf, WritePosition);
        }

        internal void WriteStringPartial(char[] chars, int charIndex, int charCount,
                                         bool flush, out int charsUsed, out bool completed)
        {
            int bytesUsed;
            _textEncoder.Convert(chars, charIndex, charCount, _buf, WritePosition, WriteSpaceLeft,
                                 flush, out charsUsed, out bytesUsed, out completed);
            WritePosition += bytesUsed;
        }

        [GenerateAsync]
        public NpgsqlBuffer WriteString(string s, int byteLen)
        {
            Contract.Assume(TextEncoding == Encoding.UTF8, "WriteString assumes UTF8-encoding");

            int charPos = 0;

            for (; ; )
            {
                if (byteLen <= WriteSpaceLeft)
                {
                    _writePosition += TextEncoding.GetBytes(s, charPos, s.Length - charPos, _buf, _writePosition);
                    return this;
                }

                int numCharsCanBeWritten = Math.Max(WriteSpaceLeft / 3, WriteSpaceLeft - (byteLen - (s.Length - charPos)));
                if (numCharsCanBeWritten >= 20) // Don't do this if the buffer is almost full
                {
                    char lastChar = s[charPos + numCharsCanBeWritten - 1];
                    if (lastChar >= 0xD800 && lastChar <= 0xDBFF)
                    {
                        --numCharsCanBeWritten; // Don't use high/lead surrogate pair as last char in block
                    }
                    int wrote = TextEncoding.GetBytes(s, charPos, numCharsCanBeWritten, _buf, _writePosition);
                    _writePosition += wrote;
                    byteLen -= wrote;
                    charPos += numCharsCanBeWritten;
                }
                else
                {
                    Underlying.Write(_buf, 0, _writePosition);
                    _writePosition = 0;
                }
            }
        }

        // Exactly the same code as WriteString
        [GenerateAsync]
        public NpgsqlBuffer WriteCharArray(char[] s, int byteLen)
        {
            Contract.Assume(TextEncoding == Encoding.UTF8, "WriteString assumes UTF8-encoding");

            int charPos = 0;

            for (; ; )
            {
                if (byteLen <= WriteSpaceLeft)
                {
                    _writePosition += TextEncoding.GetBytes(s, charPos, s.Length - charPos, _buf, _writePosition);
                    return this;
                }

                int numCharsCanBeWritten = Math.Max(WriteSpaceLeft / 3, WriteSpaceLeft - (byteLen - (s.Length - charPos)));
                if (numCharsCanBeWritten >= 20) // Don't do this if the buffer is almost full
                {
                    char lastChar = s[charPos + numCharsCanBeWritten - 1];
                    if (lastChar >= 0xD800 && lastChar <= 0xDBFF)
                    {
                        --numCharsCanBeWritten; // Don't use high/lead surrogate pair as last char in block
                    }
                    int wrote = TextEncoding.GetBytes(s, charPos, numCharsCanBeWritten, _buf, _writePosition);
                    _writePosition += wrote;
                    byteLen -= wrote;
                    charPos += numCharsCanBeWritten;
                }
                else
                {
                    Underlying.Write(_buf, 0, _writePosition);
                    _writePosition = 0;
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 8)]
        struct BitConverterUnion
        {
            [FieldOffset(0)] public byte b0;
            [FieldOffset(1)] public byte b1;
            [FieldOffset(2)] public byte b2;
            [FieldOffset(3)] public byte b3;
            [FieldOffset(4)] public byte b4;
            [FieldOffset(5)] public byte b5;
            [FieldOffset(6)] public byte b6;
            [FieldOffset(7)] public byte b7;

            [FieldOffset(0)] public float float4;
            [FieldOffset(0)] public double float8;
        }
    }
}
