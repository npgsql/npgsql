#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Npgsql
{
    class ReadBuffer
    {
        #region Fields and Properties

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; }

        internal Encoding TextEncoding { get; }

        internal int ReadPosition { get; private set; }
        internal int ReadBytesLeft => _filledBytes - ReadPosition;

        internal byte[] Buffer { get; }
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
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        internal ReadBuffer([CanBeNull] NpgsqlConnector connector, Stream stream, int size, Encoding textEncoding)
        {
            if (size < MinimumSize) {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumSize);
            }

            Connector = connector;
            Underlying = stream;
            Size = size;
            Buffer = new byte[Size];
            TextEncoding = textEncoding;
            _textDecoder = TextEncoding.GetDecoder();
            _tempCharBuf = new char[1024];
            _workspace = new byte[8];
        }

        #endregion

        #region I/O

        internal async Task Ensure(int count, bool async, bool dontBreakOnTimeouts=false)
        {
            Debug.Assert(count <= Size);
            count -= ReadBytesLeft;
            if (count <= 0) { return; }

            if (ReadPosition == _filledBytes) {
                Clear();
            } else if (count > Size - _filledBytes) {
                Array.Copy(Buffer, ReadPosition, Buffer, 0, ReadBytesLeft);
                _filledBytes = ReadBytesLeft;
                ReadPosition = 0;
            }

            try
            {
                while (count > 0)
                {
                    var toRead = Size - _filledBytes;
                    var read = async
                        ? await Underlying.ReadAsync(Buffer, _filledBytes, toRead)
                        : Underlying.Read(Buffer, _filledBytes, toRead);
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

        internal void Ensure(int count)
            => Ensure(count, false).GetAwaiter().GetResult();

        internal Task ReadMore(bool async) => Ensure(ReadBytesLeft + 1, async);

        internal ReadBuffer AllocateOversize(int count)
        {
            Debug.Assert(count > Size);
            var tempBuf = new ReadBuffer(Connector, Underlying, count, TextEncoding);
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

        internal byte ReadByte()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(byte));
            return Buffer[ReadPosition++];
        }

        internal short ReadInt16()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(short));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Buffer, ReadPosition));
            ReadPosition += 2;
            return result;
        }

        internal ushort ReadUInt16()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(short));
            var result = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(Buffer, ReadPosition));
            ReadPosition += 2;
            return result;
        }

        internal int ReadInt32()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(int));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Buffer, ReadPosition));
            ReadPosition += 4;
            return result;
        }

        internal uint ReadUInt32()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(int));
            var result = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(Buffer, ReadPosition));
            ReadPosition += 4;
            return result;
        }

        internal long ReadInt64()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(long));
            var result = IPAddress.NetworkToHostOrder(BitConverter.ToInt64(Buffer, ReadPosition));
            ReadPosition += 8;
            return result;
        }

        internal float ReadSingle()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(float));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[3] = Buffer[ReadPosition++];
                _workspace[2] = Buffer[ReadPosition++];
                _workspace[1] = Buffer[ReadPosition++];
                _workspace[0] = Buffer[ReadPosition++];
                return BitConverter.ToSingle(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToSingle(Buffer, ReadPosition);
                ReadPosition += 4;
                return result;
            }
        }

        internal double ReadDouble()
        {
            Debug.Assert(ReadBytesLeft >= sizeof(double));
            if (BitConverter.IsLittleEndian)
            {
                _workspace[7] = Buffer[ReadPosition++];
                _workspace[6] = Buffer[ReadPosition++];
                _workspace[5] = Buffer[ReadPosition++];
                _workspace[4] = Buffer[ReadPosition++];
                _workspace[3] = Buffer[ReadPosition++];
                _workspace[2] = Buffer[ReadPosition++];
                _workspace[1] = Buffer[ReadPosition++];
                _workspace[0] = Buffer[ReadPosition++];
                return BitConverter.ToDouble(_workspace, 0);
            }
            else
            {
                var result = BitConverter.ToDouble(Buffer, ReadPosition);
                ReadPosition += 8;
                return result;
            }
        }

        internal string ReadString(int byteLen)
        {
            Debug.Assert(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetString(Buffer, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        internal char[] ReadChars(int byteLen)
        {
            Debug.Assert(byteLen <= ReadBytesLeft);
            var result = TextEncoding.GetChars(Buffer, ReadPosition, byteLen);
            ReadPosition += byteLen;
            return result;
        }

        internal void ReadBytes(byte[] output, int outputOffset, int len)
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
            charsSkipped = bytesSkipped = 0;
            while (charsSkipped < charCount && bytesSkipped < byteCount)
            {
                ReadAllChars(_tempCharBuf, 0, Math.Min(charCount, _tempCharBuf.Length), byteCount, out var bSkipped, out var cSkipped);
                charsSkipped += cSkipped;
                bytesSkipped += bSkipped;
            }
        }

        #endregion

        #region Read PostGIS

        internal int ReadInt32(ByteOrder bo)
        {
            Debug.Assert(ReadBytesLeft >= sizeof(int));
            int result;
            if (BitConverter.IsLittleEndian == (bo == ByteOrder.LSB))
            {
                result = BitConverter.ToInt32(Buffer, ReadPosition);
                ReadPosition += 4;
            }
            else
            {
                _workspace[3] = Buffer[ReadPosition++];
                _workspace[2] = Buffer[ReadPosition++];
                _workspace[1] = Buffer[ReadPosition++];
                _workspace[0] = Buffer[ReadPosition++];
                result = BitConverter.ToInt32(_workspace, 0);
            }
            return result;
        }

        internal uint ReadUInt32(ByteOrder bo)
        {
            Debug.Assert(ReadBytesLeft >= sizeof(int));
            uint result;
            if (BitConverter.IsLittleEndian == (bo == ByteOrder.LSB))
            {
                result = BitConverter.ToUInt32(Buffer, ReadPosition);
                ReadPosition += 4;
            }
            else
            {
                _workspace[3] = Buffer[ReadPosition++];
                _workspace[2] = Buffer[ReadPosition++];
                _workspace[1] = Buffer[ReadPosition++];
                _workspace[0] = Buffer[ReadPosition++];
                result = BitConverter.ToUInt32(_workspace, 0);
            }
            return result;
        }

        internal double ReadDouble(ByteOrder bo)
        {
            Debug.Assert(ReadBytesLeft >= sizeof(double));

            if (BitConverter.IsLittleEndian == (ByteOrder.LSB == bo))
            {
                var result = BitConverter.ToDouble(Buffer, ReadPosition);
                ReadPosition += 8;
                return result;
            }
            else
            {
                _workspace[7] = Buffer[ReadPosition++];
                _workspace[6] = Buffer[ReadPosition++];
                _workspace[5] = Buffer[ReadPosition++];
                _workspace[4] = Buffer[ReadPosition++];
                _workspace[3] = Buffer[ReadPosition++];
                _workspace[2] = Buffer[ReadPosition++];
                _workspace[1] = Buffer[ReadPosition++];
                _workspace[0] = Buffer[ReadPosition++];
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
            Debug.Assert(absoluteOffset >= 0 && absoluteOffset <= _filledBytes);

            ReadPosition = absoluteOffset;
        }

        internal void Clear()
        {
            ReadPosition = 0;
            _filledBytes = 0;
        }

        internal void CopyTo(ReadBuffer other)
        {
            Debug.Assert(other.Size - other._filledBytes >= ReadBytesLeft);
            Array.Copy(Buffer, ReadPosition, other.Buffer, other._filledBytes, ReadBytesLeft);
            other._filledBytes += ReadBytesLeft;
        }

        internal MemoryStream GetMemoryStream(int len)
        {
            return new MemoryStream(Buffer, ReadPosition, len, false, false);
        }

        #endregion
    }
}
