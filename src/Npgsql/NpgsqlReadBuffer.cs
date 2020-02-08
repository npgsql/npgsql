﻿using System;
using System.Buffers.Binary;
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

        public NpgsqlConnection Connection => Connector.Connection!;

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        /// <summary>
        /// The total byte length of the buffer.
        /// </summary>
        internal int Size { get; }

        internal Encoding TextEncoding { get; }

        /// <summary>
        /// Same as <see cref="TextEncoding"/>, except that it does not throw an exception if an invalid char is
        /// encountered (exception fallback), but rather replaces it with a question mark character (replacement
        /// fallback).
        /// </summary>
        internal Encoding RelaxedTextEncoding { get; }

        internal int ReadPosition { get; set; }
        internal int ReadBytesLeft => FilledBytes - ReadPosition;

        internal readonly byte[] Buffer;
        internal int FilledBytes;

        ColumnStream? _columnStream;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        internal NpgsqlReadBuffer(
            NpgsqlConnector connector,
            Stream stream,
            int size,
            Encoding textEncoding,
            Encoding relaxedTextEncoding)
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
            RelaxedTextEncoding = relaxedTextEncoding;
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
            return count <= ReadBytesLeft ? Task.CompletedTask : EnsureLong();

            async Task EnsureLong()
            {
                Debug.Assert(count <= Size);
                Debug.Assert(count > ReadBytesLeft);
                count -= ReadBytesLeft;
                if (count <= 0) { return; }

                if (ReadPosition == FilledBytes)
                {
                    Clear();
                }
                else if (count > Size - FilledBytes)
                {
                    Array.Copy(Buffer, ReadPosition, Buffer, 0, ReadBytesLeft);
                    FilledBytes = ReadBytesLeft;
                    ReadPosition = 0;
                }

                try
                {
                    var totalRead = 0;
                    while (count > 0)
                    {
                        var toRead = Size - FilledBytes;
                        var read = async
                            ? await Underlying.ReadAsync(Buffer, FilledBytes, toRead)
                            : Underlying.Read(Buffer, FilledBytes, toRead);

                        if (read == 0)
                            throw new EndOfStreamException();
                        count -= read;
                        FilledBytes += read;
                        totalRead += read;
                    }

                    NpgsqlEventSource.Log.BytesRead(totalRead);
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
            var tempBuf = new NpgsqlReadBuffer(Connector, Underlying, count, TextEncoding, RelaxedTextEncoding);
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
        public sbyte ReadSByte() => Read<sbyte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => Read<byte>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16()
            => ReadInt16(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadInt16(bool littleEndian)
        {
            var result = Read<short>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16()
            => ReadUInt16(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16(bool littleEndian)
        {
            var result = Read<ushort>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32()
            => ReadInt32(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32(bool littleEndian)
        {
            var result = Read<int>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32()
            => ReadUInt32(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32(bool littleEndian)
        {
            var result = Read<uint>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64()
            => ReadInt64(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64(bool littleEndian)
        {
            var result = Read<long>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64()
            => ReadUInt64(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64(bool littleEndian)
        {
            var result = Read<ulong>();
            return littleEndian == BitConverter.IsLittleEndian
                ? result : BinaryPrimitives.ReverseEndianness(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle()
            => ReadSingle(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingle(bool littleEndian)
        {
            var result = ReadInt32(littleEndian);
            return Unsafe.As<int, float>(ref result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble()
            => ReadDouble(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDouble(bool littleEndian)
        {
            var result = ReadInt64(littleEndian);
            return Unsafe.As<long, double>(ref result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T Read<T>()
        {
            if (Unsafe.SizeOf<T>() > ReadBytesLeft)
                ThrowNotSpaceLeft();

            var result = Unsafe.ReadUnaligned<T>(ref Buffer[ReadPosition]);
            ReadPosition += Unsafe.SizeOf<T>();
            return result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ThrowNotSpaceLeft()
            => throw new InvalidOperationException("There is not enough space left in the buffer.");

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

        public void ReadBytes(Span<byte> output)
        {
            Debug.Assert(output.Length <= ReadBytesLeft);
            new Span<byte>(Buffer, ReadPosition, output.Length).CopyTo(output);
            ReadPosition += output.Length;
        }

        public void ReadBytes(byte[] output, int outputOffset, int len)
            => ReadBytes(new Span<byte>(output, outputOffset, len));

        public ReadOnlySpan<byte> ReadSpan(int len)
        {
            Debug.Assert(len <= ReadBytesLeft);
            return new ReadOnlySpan<byte>(Buffer, ReadPosition, len);
        }

        public ReadOnlyMemory<byte> ReadMemory(int len)
        {
            Debug.Assert(len <= ReadBytesLeft);
            return new ReadOnlyMemory<byte>(Buffer, ReadPosition, len);
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
        /// contain the entire string and its terminator. If any character could not be decoded, a question
        /// mark character is returned instead of throwing an exception.
        /// </summary>
        public string ReadNullTerminatedStringRelaxed() => ReadNullTerminatedString(RelaxedTextEncoding);

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        /// <param name="encoding">Decodes the messages with this encoding.</param>
        string ReadNullTerminatedString(Encoding encoding)
        {
            int i;
            for (i = ReadPosition; Buffer[i] != 0; i++)
                Debug.Assert(i <= ReadPosition + ReadBytesLeft);
            Debug.Assert(i >= ReadPosition);
            var result = encoding.GetString(Buffer, ReadPosition, i - ReadPosition);
            ReadPosition = i + 1;
            return result;
        }

        public ReadOnlySpan<byte> GetNullTerminatedBytes()
        {
            int i;
            for (i = ReadPosition; Buffer[i] != 0; i++)
                Debug.Assert(i <= ReadPosition + ReadBytesLeft);
            Debug.Assert(i >= ReadPosition);

            var result = new ReadOnlySpan<byte>(Buffer, ReadPosition, i - ReadPosition);
            ReadPosition = i + 1;
            return result;
        }

        #endregion

        #region Misc

        internal void Clear()
        {
            ReadPosition = 0;
            FilledBytes = 0;
        }

        internal void CopyTo(NpgsqlReadBuffer other)
        {
            Debug.Assert(other.Size - other.FilledBytes >= ReadBytesLeft);
            Array.Copy(Buffer, ReadPosition, other.Buffer, other.FilledBytes, ReadBytesLeft);
            other.FilledBytes += ReadBytesLeft;
        }

        #endregion
    }
}
