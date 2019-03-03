#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql
{
    /// <summary>
    /// A buffer used by Npgsql to write data to the socket efficiently.
    /// Provides methods which encode different values types and tracks the current position.
    /// </summary>
    public sealed partial class NpgsqlWriteBuffer
    {
        #region Fields and Properties

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        /// <summary>
        /// Wraps SocketAsyncEventArgs for better async I/O as long as we're not doing SSL.
        /// </summary>
        [CanBeNull]
        internal AwaitableSocket AwaitableSocket { get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; private set; }

        bool _copyMode;
        internal Encoding TextEncoding { get; }

        public int WriteSpaceLeft => Size - WritePosition;

        internal readonly byte[] Buffer;
        readonly Encoder _textEncoder;

        internal int WritePosition;

        [CanBeNull]
        ParameterStream _parameterStream;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        internal NpgsqlWriteBuffer([CanBeNull] NpgsqlConnector connector, Stream stream, int size, Encoding textEncoding)
        {
            if (size < MinimumSize)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumSize);

            Connector = connector;
            Underlying = stream;
            Size = size;
            Buffer = new byte[Size];
            TextEncoding = textEncoding;
            _textEncoder = TextEncoding.GetEncoder();
        }

        #endregion

        #region I/O

        public async Task Flush(bool async)
        {
            if (_copyMode)
            {
                // In copy mode, we write CopyData messages. The message code has already been
                // written to the beginning of the buffer, but we need to go back and write the
                // length.
                if (WritePosition == 1)
                    return;
                var pos = WritePosition;
                WritePosition = 1;
                WriteInt32(pos - 1);
                WritePosition = pos;
            } else if (WritePosition == 0)
                return;

            try
            {
                if (async)
                {
                    if (AwaitableSocket == null)  // SSL
                        await Underlying.WriteAsync(Buffer, 0, WritePosition);
                    else  // Non-SSL async I/O, optimized
                    {
                        AwaitableSocket.SetBuffer(Buffer, 0, WritePosition);
                        await AwaitableSocket.SendAsync();
                    }
                }
                else  // Sync I/O
                    Underlying.Write(Buffer, 0, WritePosition);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }

            try
            {
                if (async)
                    await Underlying.FlushAsync();
                else
                    Underlying.Flush();
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while flushing stream", e);
            }

            WritePosition = 0;
            if (CurrentCommand != null)
            {
                CurrentCommand.FlushOccurred = true;
                CurrentCommand = null;
            }
            if (_copyMode)
                WriteCopyDataHeader();
        }

        internal void Flush() => Flush(false).GetAwaiter().GetResult();

        [CanBeNull]
        internal NpgsqlCommand CurrentCommand { get; set; }

        internal void DirectWrite(byte[] buffer, int offset, int count)
        {
            if (_copyMode)
            {
                // Flush has already written the CopyData header, need to update the length
                Debug.Assert(WritePosition == 5);

                WritePosition = 1;
                WriteInt32(count + 4);
                WritePosition = 5;
                _copyMode = false;
                Flush();
                _copyMode = true;
                WriteCopyDataHeader();
            }
            else
                Debug.Assert(WritePosition == 0);

            try
            {
                Underlying.Write(buffer, offset, count);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }
        }

        #endregion

        #region Write Simple

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value) => Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value) => Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteInt16(int value)
            => WriteInt16((short)value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value)
            => WriteInt16(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt16(short value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
            => WriteUInt16(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value)
            => WriteInt32(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value)
            => WriteUInt32(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value)
            => WriteInt64(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value)
            => WriteUInt64(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : PGUtil.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(float value)
            => WriteSingle(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingle(float value, bool littleEndian)
            => WriteInt32(Unsafe.As<float, int>(ref value), littleEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value)
            => WriteDouble(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDouble(double value, bool littleEndian)
            => WriteInt64(Unsafe.As<double, long>(ref value), littleEndian);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Write<T>(T value)
        {
            if (Unsafe.SizeOf<T>() > WriteSpaceLeft)
                ThrowNotSpaceLeft();

            Unsafe.WriteUnaligned(ref Buffer[WritePosition], value);
            WritePosition += Unsafe.SizeOf<T>();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowNotSpaceLeft()
            => throw new InvalidOperationException("There is not enough space left in the buffer.");

        public Task WriteString(string s, int byteLen, bool async)
            => WriteString(s, s.Length, byteLen, async);

        public Task WriteString(string s, int charLen, int byteLen, bool async)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteString(s, charLen);
                return PGUtil.CompletedTask;
            }
            return WriteStringLong();

            async Task WriteStringLong()
            {
                Debug.Assert(byteLen > WriteSpaceLeft);
                if (byteLen <= Size)
                {
                    // String can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below (which requires ToCharArray())
                    await Flush(async);
                    WriteString(s, charLen);
                }
                else
                {
                    var charPos = 0;
                    while (true)
                    {
                        WriteStringChunked(s, charPos, charLen - charPos, true, out var charsUsed, out var completed);
                        if (completed)
                            break;
                        await Flush(async);
                        charPos += charsUsed;
                    }
                }
            }
        }

        internal Task WriteChars(char[] chars, int offset, int charLen, int byteLen, bool async)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteChars(chars, offset, charLen);
                return PGUtil.CompletedTask;
            }
            return WriteCharsLong();

            async Task WriteCharsLong()
            {
                Debug.Assert(byteLen > WriteSpaceLeft);
                if (byteLen <= Size)
                {
                    // String can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below (which requires ToCharArray())
                    await Flush(async);
                    WriteChars(chars, offset, charLen);
                }
                else
                {
                    var charPos = 0;

                    while (true)
                    {
                        int charsUsed;
                        bool completed;
                        WriteStringChunked(chars, charPos + offset, charLen - charPos, true, out charsUsed, out completed);
                        if (completed)
                            break;
                        await Flush(async);
                        charPos += charsUsed;
                    }
                }
            }
        }

        public void WriteString(string s, int len = 0)
        {
            Debug.Assert(TextEncoding.GetByteCount(s) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(s, 0, len == 0 ? s.Length : len, Buffer, WritePosition);
        }

        internal void WriteChars(char[] chars, int offset, int len)
        {
            var charCount = len == 0 ? chars.Length : len;
            Debug.Assert(TextEncoding.GetByteCount(chars, 0, charCount) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(chars, offset, charCount, Buffer, WritePosition);
        }

        public void WriteBytes(ReadOnlySpan<byte> buf)
        {
            Debug.Assert(buf.Length <= WriteSpaceLeft);
            buf.CopyTo(new Span<byte>(Buffer, WritePosition, Buffer.Length - WritePosition));
            WritePosition += buf.Length;
        }

        public void WriteBytes(byte[] buf, int offset, int count)
            => WriteBytes(new ReadOnlySpan<byte>(buf, offset, count));

        public Task WriteBytesRaw(byte[] bytes, bool async)
        {
            if (bytes.Length <= WriteSpaceLeft)
            {
                WriteBytes(bytes);
                return PGUtil.CompletedTask;
            }
            return WriteBytesLong();

            async Task WriteBytesLong()
            {
                if (bytes.Length <= Size)
                {
                    // value can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below
                    await Flush(async);
                    WriteBytes(bytes);
                }
                else
                {
                    var remaining = bytes.Length;
                    do
                    {
                        if (WriteSpaceLeft == 0)
                            await Flush(async);
                        var writeLen = Math.Min(remaining, WriteSpaceLeft);
                        var offset = bytes.Length - remaining;
                        WriteBytes(bytes, offset, writeLen);
                        remaining -= writeLen;
                    }
                    while (remaining > 0);
                }
            }
        }

        public void WriteNullTerminatedString(string s)
        {
            Debug.Assert(s.All(c => c < 128), "Method only supports ASCII strings");
            Debug.Assert(WriteSpaceLeft >= s.Length + 1);
            WritePosition += Encoding.ASCII.GetBytes(s, 0, s.Length, Buffer, WritePosition);
            WriteByte(0);
        }

        #endregion

        #region Write Complex

        public Stream GetStream()
        {
            if (_parameterStream == null)
                _parameterStream = new ParameterStream(this);

            _parameterStream.Init();
            return _parameterStream;
        }

        internal void WriteStringChunked(char[] chars, int charIndex, int charCount,
                                         bool flush, out int charsUsed, out bool completed)
        {
            if (WriteSpaceLeft == 0)
            {
                charsUsed = 0;
                completed = false;
                return;
            }

            _textEncoder.Convert(chars, charIndex, charCount, Buffer, WritePosition, WriteSpaceLeft,
                                 flush, out charsUsed, out var bytesUsed, out completed);
            WritePosition += bytesUsed;
        }

        internal unsafe void WriteStringChunked(string s, int charIndex, int charCount,
                                                bool flush, out int charsUsed, out bool completed)
        {
            if (WriteSpaceLeft == 0)
            {
                charsUsed = 0;
                completed = false;
                return;
            }

            int bytesUsed;

            fixed (char* sPtr = s)
            fixed (byte* bufPtr = Buffer)
            {
                _textEncoder.Convert(sPtr + charIndex, charCount, bufPtr + WritePosition, WriteSpaceLeft,
                                     flush, out charsUsed, out bytesUsed, out completed);
            }

            WritePosition += bytesUsed;
        }

        #endregion

        #region Copy

        internal void StartCopyMode()
        {
            _copyMode = true;
            Size -= 5;
            WriteCopyDataHeader();
        }

        internal void EndCopyMode()
        {
            // EndCopyMode is usually called after a Flush which ended the last CopyData message.
            // That Flush also wrote the header for another CopyData which we clear here.
            _copyMode = false;
            Size += 5;
            Clear();
        }

        void WriteCopyDataHeader()
        {
            Debug.Assert(_copyMode);
            Debug.Assert(WritePosition == 0);
            WriteByte((byte)BackendMessageCode.CopyData);
            // Leave space for the message length
            WriteInt32(0);
        }

        #endregion

        #region Misc

        internal void Clear()
        {
            WritePosition = 0;
        }

        /// <summary>
        /// Returns all contents currently written to the buffer (but not flushed).
        /// Useful for pregenerating messages.
        /// </summary>
        internal byte[] GetContents()
        {
            var buf = new byte[WritePosition];
            Array.Copy(Buffer, buf, WritePosition);
            return buf;
        }

        #endregion
    }
}
