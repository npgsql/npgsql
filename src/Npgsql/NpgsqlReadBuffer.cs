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
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    public sealed partial class NpgsqlReadBuffer
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

        [CanBeNull]
        ColumnStream _columnStream;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        private readonly INpgsqlStatistics _stats = new NoOpNpgsqlStatistics();

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

            if (connector?.Connection != null && connector.Connection.EnableStatistics)
            {
                _stats = new NpgsqlStatistics();
            }
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
        {
            return count <= ReadBytesLeft ? PGUtil.CompletedTask : EnsureLong();

            async Task EnsureLong()
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

        /// <summary>
        /// Skip a given number of bytes.
        /// </summary>
        public async Task Skip(long len, bool async)
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

        public ValueTask<int> ReadBytes(byte[] output, int outputOffset, int len, bool async)
        {
            var readFromBuffer = Math.Min(ReadBytesLeft, len);
            if (readFromBuffer > 0)
            {
                System.Buffer.BlockCopy(Buffer, ReadPosition, output, outputOffset, readFromBuffer);
                ReadPosition += len;
                return new ValueTask<int>(readFromBuffer);
            }

            return new ValueTask<int>(ReadBytesLong());

            async Task<int> ReadBytesLong()
            {
                Debug.Assert(ReadPosition == 0);
                Clear();
                try
                {
                    var read = async
                        ? await Underlying.ReadAsync(output, outputOffset, len)
                        : Underlying.Read(output, outputOffset, len);
                    if (read == 0)
                        throw new EndOfStreamException();
                    return read;
                }
                catch (Exception e)
                {
                    Connector.Break();
                    throw new NpgsqlException("Exception while reading from stream", e);
                }
            }
        }

        public Stream GetStream(int len, bool canSeek)
        {
            if (_columnStream == null)
                _columnStream = new ColumnStream(this);

            _columnStream.Init(len, canSeek);
            return _columnStream;
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

        #endregion

        #region Misc

        internal void Clear()
        {
            _stats.AddBytesReceived(_filledBytes);
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


        public IDictionary<string, long> RetrieveStatistics()
        {
            return _stats.RetrieveStatistics();
        }

        public void ResetStatistics()
        {
            _stats.ResetStatistics();
        }
    }

    internal sealed class NpgsqlStatistics : INpgsqlStatistics
    {
        private ConcurrentDictionary<string, long> _concurrentDictionary;

        internal NpgsqlStatistics()
        {
            _concurrentDictionary = new ConcurrentDictionary<string, long>
            {
                ["BytesReceived"] = 0
            };
        }

        public void AddBytesReceived(int bytesReceived)
        {
            _concurrentDictionary["BytesReceived"] += bytesReceived;
        }

        public IDictionary<string, long> RetrieveStatistics()
        {
            var dict = new Dictionary<string, long>();

            foreach (var pair in _concurrentDictionary)
            {
                dict.Add(pair.Key, pair.Value);
            }

            return dict;
        }

        public void ResetStatistics()
        {
            _concurrentDictionary = new ConcurrentDictionary<string, long>
            {
                ["BytesReceived"] = 0
            };
        }
    }

    internal interface INpgsqlStatistics
    {
        void AddBytesReceived(int bytesReceived);
        IDictionary<string, long> RetrieveStatistics();
        void ResetStatistics();
    }

    internal sealed class NoOpNpgsqlStatistics : INpgsqlStatistics
    {
        public void AddBytesReceived(int bytesReceived)
        {
            // no op
        }

        public IDictionary<string, long> RetrieveStatistics()
        {
            return new Dictionary<string, long>();
        }

        public void ResetStatistics()
        {
            // no op
        }
    }
}
