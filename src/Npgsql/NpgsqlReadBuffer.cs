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

using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql
{
    /// <summary>
    /// A buffer used by Npgsql to read data from the socket efficiently.
    /// Provides methods which decode different values types and tracks the current position.
    /// </summary>
    public sealed class NpgsqlReadBuffer
    {
        #region Fields and Properties

        public NpgsqlConnection Connection => Connector.Connection;

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        /// <summary>
        /// Wraps SocketAsyncEventArgs for better async I/O as long as we're not doing SSL.
        /// </summary>
        [CanBeNull]
        internal AwaitableSocket AwaitableSocket { private get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; }

        internal Encoding TextEncoding { get; }

        internal int ReadPosition { get; set; }
        internal int ReadBytesLeft => _filledBytes - ReadPosition;

        internal readonly byte[] Buffer;
        int _filledBytes;
        readonly Decoder _textDecoder;

        readonly byte[] _workspace;

        /// <summary>
        /// Used for internal temporary purposes
        /// </summary>
        [CanBeNull]
        char[] _tempCharBuf;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        internal NpgsqlReadBuffer([CanBeNull] NpgsqlConnector connector, Stream stream, int size, Encoding textEncoding)
        {
            if (size < MinimumSize)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumSize);
            }

            Connector = connector;
            Underlying = stream;
            Size = size;
            Buffer = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
            _workspace = new byte[8];
        }

        #endregion

        #region I/O

        /// <summary>
        /// Ensures that <paramref name="count"/> bytes are available in the buffer, and if
        /// not, reads from the socket until enough is available.
        /// </summary>
        public Task Ensure(int count, bool async) => Ensure(count, async, false);

        internal void Ensure(int count)
        {
            if (count <= ReadBytesLeft)
                return;
            Ensure(count, false).GetAwaiter().GetResult();
        }

        internal Task Ensure(int count, bool async, bool dontBreakOnTimeouts)
            => count <= ReadBytesLeft ? PGUtil.CompletedTask : EnsureLong(count, async, dontBreakOnTimeouts);

        async Task EnsureLong(int count, bool async, bool dontBreakOnTimeouts = false)
        {
            Debug.Assert(count <= Size);
            Debug.Assert(count > ReadBytesLeft);
            count -= ReadBytesLeft;
            if (count <= 0) { return; }

            if (ReadPosition == _filledBytes)
            {
                Clear();
            }
            else if (count > Size - _filledBytes)
            {
                Array.Copy(Buffer, ReadPosition, Buffer, 0, ReadBytesLeft);
                _filledBytes = ReadBytesLeft;
                ReadPosition = 0;
            }

            try
            {
                while (count > 0)
                {
                    var toRead = Size - _filledBytes;

                    int read;
                    if (async)
                    {
                        if (AwaitableSocket == null)  // SSL
                            read = await Underlying.ReadAsync(Buffer, _filledBytes, toRead);
                        else  // Non-SSL async I/O, optimized
                        {
                            AwaitableSocket.SetBuffer(Buffer, _filledBytes, toRead);
                            await AwaitableSocket.ReceiveAsync();
                            read = AwaitableSocket.BytesTransferred;
                        }
                    } else  // Sync I/O
                        read = Underlying.Read(Buffer, _filledBytes, toRead);

                    if (read == 0)
                        throw new EndOfStreamException();
                    count -= read;
                    _filledBytes += read;
                }
            }
            // We have a special case when reading async notifications - a timeout may be normal
            // shouldn't be fatal
            // Note that mono throws SocketException with the wrong error (see #1330)
            catch (IOException e) when (
                dontBreakOnTimeouts && (e.InnerException as SocketException)?.SocketErrorCode ==
                   (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock)
            )
            {
                throw new TimeoutException("Timeout while reading from stream");
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while reading from stream", e);
            }
        }

        internal Task ReadMore(bool async) => Ensure(ReadBytesLeft + 1, async);

        internal NpgsqlReadBuffer AllocateOversize(int count)
        {
            Debug.Assert(count > Size);
            var tempBuf = new NpgsqlReadBuffer(Connector, Underlying, count, TextEncoding);
            CopyTo(tempBuf);
            Clear();
            return tempBuf;
        }

        /// <summary>
        /// Does not perform any I/O - assuming that the bytes to be skipped are in the memory buffer.
        /// </summary>
        /// <param name="len"></param>
        internal void Skip(long len)
        {
            Debug.Assert(ReadBytesLeft >= len);
            ReadPosition += (int)len;
        }

        internal async Task Skip(long len, bool async)
        {
            Debug.Assert(len >= 0);

            if (len > ReadBytesLeft)
            {
                len -= ReadBytesLeft;
                while (len > Size)
                {
                    Clear();
                    await Ensure(Size, async);
                    len -= Size;
                }
                Clear();
                await Ensure((int)len, async);
            }

            ReadPosition += (int)len;
        }

        #endregion

        #region Read Simple

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte()
        {
            Debug.Assert(sizeof(sbyte) <= ReadBytesLeft);
            return (sbyte)Buffer[ReadPosition++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            Debug.Assert(sizeof(byte) <= ReadBytesLeft);
            return Buffer[ReadPosition++];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
            => ReadInt16(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16(bool littleEndian)
        {
            var result = Read<short>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
            => ReadUInt16(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16(bool littleEndian)
        {
            var result = Read<ushort>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
            => ReadInt32(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(bool littleEndian)
        {
            var result = Read<int>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
            => ReadUInt32(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32(bool littleEndian)
        {
            var result = Read<uint>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
            => ReadInt64(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64(bool littleEndian)
        {
            var result = Read<long>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
            => ReadUInt64(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64(bool littleEndian)
        {
            var result = Read<ulong>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
            => ReadSingle(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle(bool littleEndian)
        {
            var result = Read<float>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
            => ReadDouble(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble(bool littleEndian)
        {
            var result = Read<double>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : PGUtil.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Read<T>()
        {
            Debug.Assert(Unsafe.SizeOf<T>() <= ReadBytesLeft);
            var result = Unsafe.ReadUnaligned<T>(ref Buffer[ReadPosition]);
            ReadPosition += Unsafe.SizeOf<T>();
            return result;
        }

        public string ReadString(int byteLen)
        {
            Debug.Assert(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetString(Buffer, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        public char[] ReadChars(int byteLen)
        {
            Debug.Assert(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetChars(Buffer, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        public void ReadBytes(byte[] output, int outputOffset, int len)
        {
            Debug.Assert(len <= ReadBytesLeft);
            System.Buffer.BlockCopy(Buffer, ReadPosition, output, outputOffset, len);
            ReadPosition += len;
        }

        #endregion

        #region Read Complex

        internal async ValueTask<int> ReadAllBytes(byte[] output, int outputOffset, int len, bool readOnce, bool async)
        {
            if (len <= ReadBytesLeft)
            {
                Array.Copy(Buffer, ReadPosition, output, outputOffset, len);
                ReadPosition += len;
                return len;
            }

            Array.Copy(Buffer, ReadPosition, output, outputOffset, ReadBytesLeft);
            var offset = outputOffset + ReadBytesLeft;
            var totalRead = ReadBytesLeft;
            Clear();
            try
            {
                while (totalRead < len)
                {
                    var read = async
                        ? await Underlying.ReadAsync(output, offset, len - totalRead)
                        : Underlying.Read(output, offset, len - totalRead);
                    if (read == 0)
                        throw new EndOfStreamException();
                    totalRead += read;
                    if (readOnce)
                        return totalRead;
                    offset += read;
                }
            }
            catch (Exception e)
            {
                Connector.Break();
                throw new NpgsqlException("Exception while reading from stream", e);
            }
            return len;
        }

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        public string ReadNullTerminatedString() => ReadNullTerminatedString(TextEncoding);

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        /// <param name="encoding">Decodes the messages with this encoding.</param>
        internal string ReadNullTerminatedString(Encoding encoding)
        {
            int i;
            for (i = ReadPosition; Buffer[i] != 0; i++)
            {
                Debug.Assert(i <= ReadPosition + ReadBytesLeft);
            }
            Debug.Assert(i >= ReadPosition);
            var result = encoding.GetString(Buffer, ReadPosition, i - ReadPosition);
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
            Debug.Assert(charCount <= output.Length - outputOffset);

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
                    _textDecoder.Convert(Buffer, ReadPosition, maxBytes, output, outputOffset, charCount - charsRead, false,
                                         out bytesUsed, out charsUsed, out completed);
                    ReadPosition += bytesUsed;
                    bytesRead += bytesUsed;
                    charsRead += charsUsed;
                    if (charsRead == charCount || bytesRead == byteCount)
                        return;
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
            if (_tempCharBuf == null)
                _tempCharBuf = new char[1024];
            charsSkipped = bytesSkipped = 0;
            while (charsSkipped < charCount && bytesSkipped < byteCount)
            {
                ReadAllChars(_tempCharBuf, 0, Math.Min(charCount, _tempCharBuf.Length), byteCount, out var bSkipped, out var cSkipped);
                charsSkipped += cSkipped;
                bytesSkipped += bSkipped;
            }
        }

        #endregion

        #region Misc

        internal void Clear()
        {
            ReadPosition = 0;
            _filledBytes = 0;
        }

        internal void CopyTo(NpgsqlReadBuffer other)
        {
            Debug.Assert(other.Size - other._filledBytes >= ReadBytesLeft);
            Array.Copy(Buffer, ReadPosition, other.Buffer, other._filledBytes, ReadBytesLeft);
            other._filledBytes += ReadBytesLeft;
        }

        #endregion
    }
}
