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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncRewriter;
using JetBrains.Annotations;

namespace Npgsql
{
    partial class WriteBuffer
    {
        #region Fields and Properties

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; private set; }

        bool _copyMode;
        internal Encoding TextEncoding { get; }

        internal int WritePosition { get { return _writePosition; } set { _writePosition = value; } }
        internal int WriteSpaceLeft => Size - _writePosition;

        internal long TotalBytesFlushed { get; private set; }

        readonly byte[] _buf;
        readonly Encoder _textEncoder;

        int _writePosition;

        BitConverterUnion _bitConverterUnion;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumBufferSize = ReadBuffer.MinimumBufferSize;
        internal const int DefaultBufferSize = ReadBuffer.DefaultBufferSize;

        #endregion

        #region Constructors

        internal WriteBuffer([CanBeNull] NpgsqlConnector connector, Stream stream, int size, Encoding textEncoding)
        {
            if (size < MinimumBufferSize)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumBufferSize);

            Connector = connector;
            Underlying = stream;
            Size = size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textEncoder = TextEncoding.GetEncoder();
        }

        #endregion

        #region I/O

        internal async Task Flush(bool async, CancellationToken cancellationToken)
        {
            if (_copyMode)
            {
                // In copy mode, we write CopyData messages. The message code has already been
                // written to the beginning of the buffer, but we need to go back and write the
                // length.
                if (_writePosition == 1)
                    return;
                var pos = WritePosition;
                _writePosition = 1;
                WriteInt32(pos - 1);
                _writePosition = pos;
            } else if (_writePosition == 0)
                return;

            try
            {
                if (async)
                    await Underlying.WriteAsync(_buf, 0, _writePosition, cancellationToken);
                else
                    Underlying.Write(_buf, 0, _writePosition);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }

            try
            {
                if (async)
                    await Underlying.FlushAsync(cancellationToken);
                else
                    Underlying.Flush();
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while flushing stream", e);
            }

            TotalBytesFlushed += _writePosition;
            _writePosition = 0;
            if (CurrentCommand != null)
                CurrentCommand.FlushOccurred = true;
            if (_copyMode)
                WriteCopyDataHeader();
        }

        internal void Flush() => Flush(false, CancellationToken.None).GetAwaiter().GetResult();

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

        public void WriteByte(byte b)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(byte));
            _buf[_writePosition++] = b;
        }

        public void WriteInt16(int i)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(short));
            _buf[_writePosition++] = (byte)(i >> 8);
            _buf[_writePosition++] = (byte)i;
        }

        public void WriteUInt16(int i)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(ushort));
            _buf[_writePosition++] = (byte)(i >> 8);
            _buf[_writePosition++] = (byte)i;
        }

        public void WriteInt32(int i)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(int));
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;
        }

        internal void WriteUInt32(uint i)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(uint));
            var pos = _writePosition;
            _buf[pos++] = (byte)(i >> 24);
            _buf[pos++] = (byte)(i >> 16);
            _buf[pos++] = (byte)(i >> 8);
            _buf[pos++] = (byte)i;
            _writePosition = pos;
        }

        public void WriteInt64(long i)
        {
            Debug.Assert(WriteSpaceLeft >= sizeof(long));
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
            Debug.Assert(WriteSpaceLeft >= sizeof(float));
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
            Debug.Assert(WriteSpaceLeft >= sizeof(double));
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

        internal Task WriteString(string s, int byteLen, bool async, CancellationToken cancellationToken)
            => WriteString(s, s.Length, byteLen, async, cancellationToken);

        internal async Task WriteString(string s, int charLen, int byteLen, bool async, CancellationToken cancellationToken)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteString(s, charLen);
            }
            else if (byteLen <= Size)
            {
                // String can fit entirely in an empty buffer. Flush and retry rather than
                // going into the partial writing flow below (which requires ToCharArray())
                await Flush(async, cancellationToken);
                WriteString(s, charLen);
            }
            else
            {
#if NETSTANDARD1_3
                await WriteChars(s.ToCharArray(), charLen, byteLen, async, cancellationToken);
                return;
#else
                var charPos = 0;
                while (true)
                {
                    int charsUsed;
                    bool completed;
                    WriteStringChunked(s, charPos, charLen - charPos, true, out charsUsed, out completed);
                    if (completed)
                        break;
                    await Flush(async, cancellationToken);
                    charPos += charsUsed;
                }
#endif
            }
        }

        internal async Task WriteChars(char[] chars, int charLen, int byteLen, bool async, CancellationToken cancellationToken)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteChars(chars, charLen);
            }
            else if (byteLen <= Size)
            {
                // String can fit entirely in an empty buffer. Flush and retry rather than
                // going into the partial writing flow below (which requires ToCharArray())
                await Flush(async, cancellationToken);
                WriteChars(chars, charLen);
            }
            else
            {
                var charPos = 0;

                while (true)
                {
                    int charsUsed;
                    bool completed;
                    WriteStringChunked(chars, charPos, charLen - charPos, true, out charsUsed, out completed);
                    if (completed)
                        break;
                    await Flush(async, cancellationToken);
                    charPos += charsUsed;
                }
            }
        }

        internal void WriteString(string s, int len = 0)
        {
            Debug.Assert(TextEncoding.GetByteCount(s) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(s, 0, len == 0 ? s.Length : len, _buf, WritePosition);
        }

        internal void WriteChars(char[] chars, int len = 0)
        {
            Debug.Assert(TextEncoding.GetByteCount(chars) <= WriteSpaceLeft);
            WritePosition += TextEncoding.GetBytes(chars, 0, len == 0 ? chars.Length : len, _buf, WritePosition);
        }

        public void WriteBytes(byte[] buf, int offset, int count)
        {
            Debug.Assert(count <= WriteSpaceLeft);
            Buffer.BlockCopy(buf, offset, _buf, WritePosition, count);
            WritePosition += count;
        }

        public void WriteBytesNullTerminated(byte[] buf)
        {
            Debug.Assert(WriteSpaceLeft >= buf.Length + 1);
            WriteBytes(buf, 0, buf.Length);
            WriteByte(0);
        }

        #endregion

        #region Write Complex

        internal void WriteStringChunked(char[] chars, int charIndex, int charCount,
                                         bool flush, out int charsUsed, out bool completed)
        {
            if (WriteSpaceLeft == 0)
            {
                charsUsed = 0;
                completed = false;
                return;
            }

            int bytesUsed;
            _textEncoder.Convert(chars, charIndex, charCount, _buf, WritePosition, WriteSpaceLeft,
                                 flush, out charsUsed, out bytesUsed, out completed);
            WritePosition += bytesUsed;
        }

#if !NETSTANDARD1_3
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
            fixed (byte* bufPtr = _buf)
            {
                _textEncoder.Convert(sPtr + charIndex, charCount, bufPtr + WritePosition, WriteSpaceLeft,
                                     flush, out charsUsed, out bytesUsed, out completed);
            }

            WritePosition += bytesUsed;
        }
#endif

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
            var buf = new byte[_writePosition];
            Array.Copy(_buf, buf, _writePosition);
            return buf;
        }

        internal void ResetTotalBytesFlushed()
        {
            TotalBytesFlushed = 0;
        }

#pragma warning disable CA1051 // Do not declare visible instance fields
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
#pragma warning restore CA1051 // Do not declare visible instance fields

        #endregion
    }
}
