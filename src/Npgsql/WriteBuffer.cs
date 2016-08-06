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
                Debug.Assert(value <= Size);
                _usableSize = value;
            }
        }

        int _usableSize;
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
            UsableSize = Size;
            _buf = new byte[Size];
            TextEncoding = textEncoding;
            _textEncoder = TextEncoding.GetEncoder();
        }

        #endregion

        #region I/O

        [RewriteAsync]
        internal void Flush()
        {
            if (_writePosition == 0)
                return;

            try
            {
                Underlying.Write(_buf, 0, _writePosition);
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while writing to stream", e);
            }

            try
            {
                Underlying.Flush();
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while flushing stream", e);
            }

            TotalBytesFlushed += _writePosition;
            _writePosition = 0;
        }

        [RewriteAsync]
        internal void DirectWrite(byte[] buffer, int offset, int count)
        {
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
            int bytesUsed;
            _textEncoder.Convert(chars, charIndex, charCount, _buf, WritePosition, WriteSpaceLeft,
                                 flush, out charsUsed, out bytesUsed, out completed);
            WritePosition += bytesUsed;
        }

#if !NETSTANDARD1_3
        internal unsafe void WriteStringChunked(string s, int charIndex, int charCount,
                                                bool flush, out int charsUsed, out bool completed)
        {
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

        #region Misc

        internal void Clear()
        {
            WritePosition = 0;
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
