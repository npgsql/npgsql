using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Util;
using static System.Threading.Timeout;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql
{
    /// <summary>
    /// A buffer used by Npgsql to read data from the socket efficiently.
    /// Provides methods which decode different values types and tracks the current position.
    /// </summary>
    public sealed partial class NpgsqlReadBuffer : IDisposable
    {
        #region Fields and Properties

        public NpgsqlConnection Connection => Connector.Connection!;

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        readonly Socket? _underlyingSocket;

        internal ResettableCancellationTokenSource Cts { get; }

        TimeSpan _preTranslatedTimeout = TimeSpan.Zero;

        /// <summary>
        /// Timeout for sync and async reads
        /// </summary>
        internal TimeSpan Timeout
        {
            get => _preTranslatedTimeout;
            set
            {
                if (_preTranslatedTimeout != value)
                {
                    _preTranslatedTimeout = value;

                    if (value == TimeSpan.Zero)
                        value = InfiniteTimeSpan;
                    else if (value < TimeSpan.Zero)
                        value = TimeSpan.Zero;

                    Debug.Assert(_underlyingSocket != null);

                    _underlyingSocket.ReceiveTimeout = (int)value.TotalMilliseconds;
                    Cts.Timeout = value;
                }
            }
        }

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

        bool _disposed;

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
            Socket? socket,
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
            _underlyingSocket = socket;
            Cts = new ResettableCancellationTokenSource();
            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size);
            TextEncoding = textEncoding;
            RelaxedTextEncoding = relaxedTextEncoding;
        }

        #endregion

        #region I/O

        internal void Ensure(int count)
        {
            if (count <= ReadBytesLeft)
                return;
            Ensure(count, false).GetAwaiter().GetResult();
        }

        public Task Ensure(int count, bool async)
            => Ensure(count, async, readingNotifications: false);

        public Task EnsureAsync(int count)
            => Ensure(count, async: true, readingNotifications: false);

        /// <summary>
        /// Ensures that <paramref name="count"/> bytes are available in the buffer, and if
        /// not, reads from the socket until enough is available.
        /// </summary>
        internal Task Ensure(int count, bool async, bool readingNotifications)
        {
            return count <= ReadBytesLeft ? Task.CompletedTask : EnsureLong(this, count, async, readingNotifications);

            static async Task EnsureLong(
                NpgsqlReadBuffer buffer,
                int count,
                bool async,
                bool readingNotifications)
            {
                Debug.Assert(count <= buffer.Size);
                Debug.Assert(count > buffer.ReadBytesLeft);
                count -= buffer.ReadBytesLeft;
                if (count <= 0) { return; }

                if (buffer.ReadPosition == buffer.FilledBytes)
                {
                    buffer.Clear();
                }
                else if (count > buffer.Size - buffer.FilledBytes)
                {
                    Array.Copy(buffer.Buffer, buffer.ReadPosition, buffer.Buffer, 0, buffer.ReadBytesLeft);
                    buffer.FilledBytes = buffer.ReadBytesLeft;
                    buffer.ReadPosition = 0;
                }

                var finalCt = async && buffer.Timeout != TimeSpan.Zero
                    ? buffer.Cts.Start()
                    : buffer.Cts.Reset();

                var totalRead = 0;
                while (count > 0)
                {
                    try
                    {
                        var toRead = buffer.Size - buffer.FilledBytes;
                        var read = async
                            ? await buffer.Underlying.ReadAsync(buffer.Buffer, buffer.FilledBytes, toRead, finalCt)
                            : buffer.Underlying.Read(buffer.Buffer, buffer.FilledBytes, toRead);

                        if (read == 0)
                            throw new EndOfStreamException();
                        count -= read;
                        buffer.FilledBytes += read;
                        totalRead += read;

                        // Most of the time, it should be fine to reset cancellation token source, so we can use it again
                        // It's still possible for cancellation token to cancel between reading and resetting (although highly improbable)
                        // In this case, we consider it as timed out and fail with OperationCancelledException on next ReadAsync
                        // Or we consider it not timed out if we have already read everything (count == 0)
                        // In which case we reinitialize it on the next call to EnsureLong()
                        if (async)
                            buffer.Cts.RestartTimeoutWithoutReset();
                    }
                    catch (Exception e)
                    {
                        var connector = buffer.Connector;

                        // Stopping twice (in case the previous Stop() call succeeded) doesn't hurt.
                        // Not stopping will cause an assertion failure in debug mode when we call Start() the next time.
                        // We can't stop in a finally block because Connector.Break() will dispose the buffer and the contained
                        // _timeoutCts
                        buffer.Cts.Stop();

                        switch (e)
                        {
                        // Read timeout
                        case OperationCanceledException _:
                        // Note that mono throws SocketException with the wrong error (see #1330)
                        case IOException _ when (e.InnerException as SocketException)?.SocketErrorCode ==
                                                (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock):
                        {
                            Debug.Assert(e is OperationCanceledException ? async : !async);

                            // When reading notifications (Wait), just throw TimeoutException or OperationCanceledException immediately.
                            // Nothing to cancel, and no breaking of the connection.
                            if (readingNotifications)
                            {
                                if (connector.UserCancellationRequested)
                                    throw;
                                throw NpgsqlTimeoutException();
                            }

                            // If we should attempt PostgreSQL cancellation, do it the first time we get a timeout.
                            // TODO: As an optimization, we can still attempt to send a cancellation request, but after that immediately break the connection
                            if (connector.AttemptPostgresCancellation &&
                                !connector.PostgresCancellationPerformed &&
                                connector.PerformPostgresCancellation())
                            {
                                // Note that if the cancellation timeout is negative, we flow down and break the connection immediately
                                var cancellationTimeout = connector.Settings.CancellationTimeout;
                                if (cancellationTimeout >= 0)
                                {
                                    if (cancellationTimeout > 0)
                                        buffer.Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);

                                    if (async)
                                        finalCt = buffer.Cts.Start();

                                    continue;
                                }
                            }

                            // If we're here, the PostgreSQL cancellation either failed or skipped entirely.
                            // Break the connection, bubbling up the correct exception type (cancellation or timeout)
                            throw connector.Break(!buffer.Connector.UserCancellationRequested
                                ? NpgsqlTimeoutException()
                                : connector.PostgresCancellationPerformed
                                    ? new OperationCanceledException("Query was cancelled", TimeoutException(), connector.UserCancellationToken)
                                    : new OperationCanceledException("Query was cancelled", connector.UserCancellationToken));
                        }

                        default:
                            throw connector.Break(new NpgsqlException("Exception while reading from stream", e));
                        }
                    }
                }

                buffer.Cts.Stop();
                NpgsqlEventSource.Log.BytesRead(totalRead);

                static Exception NpgsqlTimeoutException() => new NpgsqlException("Exception while reading from stream", TimeoutException());

                static Exception TimeoutException() => new TimeoutException("Timeout during reading attempt");
            }
        }

        internal void ReadMore() => ReadMore(false).GetAwaiter().GetResult();

        internal Task ReadMore(bool async) => Ensure(ReadBytesLeft + 1, async);

        internal NpgsqlReadBuffer AllocateOversize(int count)
        {
            Debug.Assert(count > Size);
            var tempBuf = new NpgsqlReadBuffer(Connector, Underlying, _underlyingSocket, count, TextEncoding, RelaxedTextEncoding);
            if (_underlyingSocket != null)
                tempBuf.Timeout = Timeout;
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

        public int Read(Span<byte> output)
        {
            var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
            if (readFromBuffer > 0)
            {
                new Span<byte>(Buffer, ReadPosition, readFromBuffer).CopyTo(output);
                ReadPosition += readFromBuffer;
                return readFromBuffer;
            }

            if (output.Length == 0)
                return 0;

            Debug.Assert(ReadBytesLeft == 0);
            Clear();
            try
            {
                var read = Underlying.Read(output);
                if (read == 0)
                    throw new EndOfStreamException();
                return read;
            }
            catch (Exception e)
            {
                throw Connector.Break(new NpgsqlException("Exception while reading from stream", e));
            }
        }

        public ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default)
        {
            if (output.Length == 0)
                return new ValueTask<int>(0);

            var readFromBuffer = Math.Min(ReadBytesLeft, output.Length);
            if (readFromBuffer > 0)
            {
                new Span<byte>(Buffer, ReadPosition, readFromBuffer).CopyTo(output.Span);
                ReadPosition += readFromBuffer;
                return new ValueTask<int>(readFromBuffer);
            }

            return ReadAsyncLong(this, output, cancellationToken);

            static async ValueTask<int> ReadAsyncLong(NpgsqlReadBuffer buffer, Memory<byte> output, CancellationToken cancellationToken)
            {
                Debug.Assert(buffer.ReadBytesLeft == 0);
                buffer.Clear();
                try
                {
                    var read = await buffer.Underlying.ReadAsync(output, cancellationToken);
                    if (read == 0)
                        throw new EndOfStreamException();
                    return read;
                }
                catch (Exception e)
                {
                    throw buffer.Connector.Break(new NpgsqlException("Exception while reading from stream", e));
                }
            }
        }

        public Stream GetStream(int len, bool canSeek)
        {
            if (_columnStream == null)
                _columnStream = new ColumnStream(Connector);

            _columnStream.Init(len, canSeek);
            return _columnStream;
        }

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator.
        /// </summary>
        public string ReadNullTerminatedString()
            => ReadNullTerminatedString(TextEncoding, async: false).GetAwaiter().GetResult();

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. The buffer must already
        /// contain the entire string and its terminator. If any character could not be decoded, a question
        /// mark character is returned instead of throwing an exception.
        /// </summary>
        public string ReadNullTerminatedStringRelaxed()
            => ReadNullTerminatedString(RelaxedTextEncoding, async: false).GetAwaiter().GetResult();

        public ValueTask<string> ReadNullTerminatedString(bool async, CancellationToken cancellationToken = default)
            => ReadNullTerminatedString(TextEncoding, async, cancellationToken);

        /// <summary>
        /// Seeks the first null terminator (\0) and returns the string up to it. Reads additional data from the network if a null
        /// terminator isn't found in the buffered data.
        /// </summary>
        ValueTask<string> ReadNullTerminatedString(Encoding encoding, bool async, CancellationToken cancellationToken = default)
        {
            return ReadFromBuffer(this, encoding, out var s)
                ? new ValueTask<string>(s)
                : ReadLong(this, async, encoding, s);

            static bool ReadFromBuffer(NpgsqlReadBuffer buffer, Encoding encoding, out string s)
            {
                var start = buffer.ReadPosition;
                while (buffer.ReadPosition < buffer.FilledBytes)
                {
                    if (buffer.Buffer[buffer.ReadPosition++] == 0)
                    {
                        s = encoding.GetString(buffer.Buffer, start, buffer.ReadPosition - start - 1);
                        return true;
                    }
                }

                s = encoding.GetString(buffer.Buffer, start, buffer.ReadPosition - start);
                return false;
            }

            static async ValueTask<string> ReadLong(NpgsqlReadBuffer buffer, bool async, Encoding encoding, string s)
            {
                var builder = new StringBuilder(s);
                bool complete;
                do
                {
                    await buffer.ReadMore(async);
                    complete = ReadFromBuffer(buffer, encoding, out s);
                    builder.Append(s);
                }
                while (!complete);

                return builder.ToString();
            }
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

        #region Dispose

        public void Dispose()
        {
            if (_disposed)
                return;

            ArrayPool<byte>.Shared.Return(Buffer);

            Cts.Dispose();
            _disposed = true;
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
