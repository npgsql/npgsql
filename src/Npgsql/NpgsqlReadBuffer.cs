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

        /// <summary>
        /// Timeout for sync and async reads
        /// </summary>
        internal TimeSpan Timeout
        {
            get => Cts.Timeout;
            set
            {
                if (Cts.Timeout != value)
                {
                    Debug.Assert(_underlyingSocket != null);

                    if (value > TimeSpan.Zero)
                    {
                        _underlyingSocket.ReceiveTimeout = (int)value.TotalMilliseconds;
                        Cts.Timeout = value;
                    }
                    else
                    {
                        _underlyingSocket.ReceiveTimeout = -1;
                        Cts.Timeout = InfiniteTimeSpan;
                    }
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

        public Task Ensure(int count, bool async, CancellationToken cancellationToken = default)
            => Ensure(count, async, readingNotifications: false, attemptPostgresCancellation: false, cancellationToken);

        public Task EnsureAsync(int count, CancellationToken cancellationToken = default)
            => Ensure(count, async: true, readingNotifications: false, attemptPostgresCancellation: false, cancellationToken);

        /// <summary>
        /// Ensures that <paramref name="count"/> bytes are available in the buffer, and if
        /// not, reads from the socket until enough is available.
        /// </summary>
        internal Task Ensure(int count, bool async, bool readingNotifications, bool attemptPostgresCancellation,
            CancellationToken cancellationToken = default)
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

                var finalCt = cancellationToken;
                if (async)
                {
                    finalCt = Timeout > TimeSpan.Zero
                        ? Cts.Start(cancellationToken)
                        : Cts.Reset(cancellationToken);
                }

                var totalRead = 0;
                var wasCancellationRequested = false;
                while (count > 0)
                {
                    try
                    {
                        var toRead = Size - FilledBytes;
                        var read = async
                            ? await Underlying.ReadAsync(Buffer, FilledBytes, toRead, finalCt)
                            : Underlying.Read(Buffer, FilledBytes, toRead);

                        if (read == 0)
                            throw new EndOfStreamException();
                        count -= read;
                        FilledBytes += read;
                        totalRead += read;

                        // Most of the time, it should be fine to reset cancellation token source, so we can use it again
                        // It's still possible for cancellation token to cancel between reading and resetting (although highly improbable)
                        // In this case, we consider it as timed out and fail with OperationCancelledException on next ReadAsync
                        // Or we consider it not timed out if we have already read everything (count == 0)
                        // In which case we reinitialize it on the next call to EnsureLong()
                        if (async)
                            Cts.RestartTimeoutWithoutReset();
                    }
                    catch (Exception e)
                    {
                        // Stopping twice (in case the previous Stop() call succeeded) doesn't hurt.
                        // Not stopping will cause an assertion failure in debug mode when we call Start() the next time.
                        // We can't stop in a finally block because Connector.Break() will dispose the buffer and the contained
                        // _timeoutCts
                        Cts.Stop();

                        switch (e)
                        {
                        // User requested the cancellation (at this moment, it is COPY operations, WaitAsync, Reader's sequential methods, authentication)
                        case OperationCanceledException _ when cancellationToken.IsCancellationRequested:
                            throw readingNotifications ? e : Connector.Break(e);

                        // Read timeout
                        case OperationCanceledException _:
                        // Note that mono throws SocketException with the wrong error (see #1330)
                        case IOException _ when (e.InnerException as SocketException)?.SocketErrorCode ==
                                                (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock):
                        {
                            Debug.Assert(e is OperationCanceledException ? async : !async);

                            if (readingNotifications)
                                throw NpgsqlTimeoutException();

                            // Note that if PG cancellation fails, the exception for that is already logged internally by CancelRequest.
                            // We simply continue and throw the timeout one.
                            // TODO: As an optimization, we can still attempt to send a cancellation request, but after that immediately break the connection
                            if (attemptPostgresCancellation && !wasCancellationRequested && Connector.CancelRequest(requestedByUser: false))
                            {
                                // If the cancellation timeout is negative, we break the connection immediately
                                var cancellationTimeout = Connector.Settings.CancellationTimeout;
                                if (cancellationTimeout >= 0)
                                {
                                    wasCancellationRequested = true;

                                    if (cancellationTimeout > 0)
                                        Timeout = TimeSpan.FromMilliseconds(cancellationTimeout);

                                    if (async)
                                        finalCt = Cts.Start(cancellationToken);

                                    continue;
                                }
                            }

                            // There is a case, when we might call a cancellable method (NpgsqlDataReader.NextResult)
                            // but it times out on a sequential read (NpgsqlDataReader.ConsumeRow)
                            if (Connector.UserCancellationRequested)
                            {
                                // User requested the cancellation and it timed out (or we didn't send it)
                                throw Connector.Break(new OperationCanceledException("Query was cancelled", TimeoutException(),
                                    Connector.UserCancellationToken));
                            }

                            throw Connector.Break(NpgsqlTimeoutException());
                        }

                        default:
                            throw Connector.Break(new NpgsqlException("Exception while reading from stream", e));
                        }
                    }
                }

                Cts.Stop();
                NpgsqlEventSource.Log.BytesRead(totalRead);

                static Exception NpgsqlTimeoutException() => new NpgsqlException("Exception while reading from stream", TimeoutException());

                static Exception TimeoutException() => new TimeoutException("Timeout during reading attempt");
            }
        }

        internal void ReadMore() => ReadMore(false).GetAwaiter().GetResult();

        internal Task ReadMore(bool async, CancellationToken cancellationToken = default) => Ensure(ReadBytesLeft + 1, async, cancellationToken);

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
        public async Task Skip(long len, bool async, CancellationToken cancellationToken = default)
        {
            Debug.Assert(len >= 0);

            if (len > ReadBytesLeft)
            {
                len -= ReadBytesLeft;
                while (len > Size)
                {
                    Clear();
                    await Ensure(Size, async, cancellationToken);
                    len -= Size;
                }
                Clear();
                await Ensure((int)len, async, cancellationToken);
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

            Debug.Assert(ReadPosition == 0);
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

            return ReadAsyncLong();

            async ValueTask<int> ReadAsyncLong()
            {
                Debug.Assert(ReadBytesLeft == 0);
                Clear();
                try
                {
                    var read = await Underlying.ReadAsync(output, cancellationToken);
                    if (read == 0)
                        throw new EndOfStreamException();
                    return read;
                }
                catch (Exception e)
                {
                    throw Connector.Break(new NpgsqlException("Exception while reading from stream", e));
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
            return ReadFromBuffer(out var s)
                ? new ValueTask<string>(s)
                : ReadLong(s);

            bool ReadFromBuffer(out string s)
            {
                var start = ReadPosition;
                while (ReadPosition < FilledBytes)
                {
                    if (Buffer[ReadPosition++] == 0)
                    {
                        s = encoding.GetString(Buffer, start, ReadPosition - start - 1);
                        return true;
                    }
                }

                s = encoding.GetString(Buffer, start, ReadPosition - start);
                return false;
            }

            async ValueTask<string> ReadLong(string s)
            {
                var builder = new StringBuilder(s);
                bool complete;
                do
                {
                    await ReadMore(async, cancellationToken);
                    complete = ReadFromBuffer(out s);
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
