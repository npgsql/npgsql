using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
    /// A buffer used by Npgsql to write data to the socket efficiently.
    /// Provides methods which encode different values types and tracks the current position.
    /// </summary>
    public sealed partial class NpgsqlWriteBuffer : IDisposable
    {
        #region Fields and Properties

        internal readonly NpgsqlConnector Connector;

        internal Stream Underlying { private get; set; }

        readonly Socket? _underlyingSocket;

        readonly ResettableCancellationTokenSource _timeoutCts;

        /// <summary>
        /// Timeout for sync and async writes
        /// </summary>
        internal TimeSpan Timeout
        {
            get => _timeoutCts.Timeout;
            set
            {
                if (_timeoutCts.Timeout != value)
                {
                    Debug.Assert(_underlyingSocket != null);

                    if (value > TimeSpan.Zero)
                    {
                        _underlyingSocket.SendTimeout = (int)value.TotalMilliseconds;
                        _timeoutCts.Timeout = value;
                    }
                    else
                    {
                        _underlyingSocket.SendTimeout = -1;
                        _timeoutCts.Timeout = InfiniteTimeSpan;
                    }
                }
            }
        }

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

        ParameterStream? _parameterStream;

        bool _disposed;

        /// <summary>
        /// The minimum buffer size possible.
        /// </summary>
        internal const int MinimumSize = 4096;
        internal const int DefaultSize = 8192;

        #endregion

        #region Constructors

        internal NpgsqlWriteBuffer(NpgsqlConnector connector, Stream stream, Socket? socket, int size, Encoding textEncoding)
        {
            if (size < MinimumSize)
                throw new ArgumentOutOfRangeException(nameof(size), size, "Buffer size must be at least " + MinimumSize);

            Connector = connector;
            Underlying = stream;
            _underlyingSocket = socket;
            _timeoutCts = new ResettableCancellationTokenSource();
            Size = size;
            Buffer = ArrayPool<byte>.Shared.Rent(size);
            TextEncoding = textEncoding;
            _textEncoder = TextEncoding.GetEncoder();
        }

        #endregion

        #region I/O

        public async Task Flush(bool async, CancellationToken cancellationToken = default)
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

            var finalCt = cancellationToken;
            if (async && Timeout > TimeSpan.Zero)
                finalCt = _timeoutCts.Start(cancellationToken);

            try
            {
                if (async)
                {
                    await Underlying.WriteAsync(Buffer, 0, WritePosition, finalCt);
                    await Underlying.FlushAsync(finalCt);
                    _timeoutCts.Stop();
                }
                else
                {
                    Underlying.Write(Buffer, 0, WritePosition);
                    Underlying.Flush();
                }
            }
            catch (Exception e)
            {
                // Stopping twice (in case the previous Stop() call succeeded) doesn't hurt.
                // Not stopping will cause an assertion failure in debug mode when we call Start() the next time.
                // We can't stop in a finally block because Connector.Break() will dispose the buffer and the contained
                // _timeoutCts
                _timeoutCts.Stop();
                switch (e)
                {
                // User requested the cancellation
                case OperationCanceledException _ when (cancellationToken.IsCancellationRequested):
                    throw Connector.Break(e);
                // Read timeout
                case OperationCanceledException _:
                // Note that mono throws SocketException with the wrong error (see #1330)
                case IOException _ when (e.InnerException as SocketException)?.SocketErrorCode ==
                                            (Type.GetType("Mono.Runtime") == null ? SocketError.TimedOut : SocketError.WouldBlock):
                    Debug.Assert(e is OperationCanceledException ? async : !async);
                    throw Connector.Break(new NpgsqlException("Exception while writing to stream", new TimeoutException("Timeout during writing attempt")));
                }

                throw Connector.Break(new NpgsqlException("Exception while writing to stream", e));
            }
            NpgsqlEventSource.Log.BytesWritten(WritePosition);
            //NpgsqlEventSource.Log.RequestFailed();

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

        internal NpgsqlCommand? CurrentCommand { get; set; }

        #endregion

        #region Direct write

        internal void DirectWrite(ReadOnlySpan<byte> buffer)
        {
            Flush();

            if (_copyMode)
            {
                // Flush has already written the CopyData header for us, but write the CopyData
                // header to the socket with the write length before we can start writing the data directly.
                Debug.Assert(WritePosition == 5);

                WritePosition = 1;
                WriteInt32(buffer.Length + 4);
                WritePosition = 5;
                _copyMode = false;
                Flush();
                _copyMode = true;
                WriteCopyDataHeader();  // And ready the buffer after the direct write completes
            }
            else
                Debug.Assert(WritePosition == 0);

            try
            {
                Underlying.Write(buffer);
            }
            catch (Exception e)
            {
                throw Connector.Break(new NpgsqlException("Exception while writing to stream", e));
            }
        }

        internal async Task DirectWrite(ReadOnlyMemory<byte> memory, bool async, CancellationToken cancellationToken = default)
        {
            await Flush(async, cancellationToken);

            if (_copyMode)
            {
                // Flush has already written the CopyData header for us, but write the CopyData
                // header to the socket with the write length before we can start writing the data directly.
                Debug.Assert(WritePosition == 5);

                WritePosition = 1;
                WriteInt32(memory.Length + 4);
                WritePosition = 5;
                _copyMode = false;
                await Flush(async, cancellationToken);
                _copyMode = true;
                WriteCopyDataHeader();  // And ready the buffer after the direct write completes
            }
            else
                Debug.Assert(WritePosition == 0);

            try
            {
                if (async)
                    await Underlying.WriteAsync(memory, cancellationToken);
                else
                    Underlying.Write(memory.Span);
            }
            catch (Exception e)
            {
                throw Connector.Break(new NpgsqlException("Exception while writing to stream", e));
            }
        }

        #endregion Direct write

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
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value)
            => WriteUInt16(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16(ushort value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value)
            => WriteInt32(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32(int value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value)
            => WriteUInt32(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32(uint value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value)
            => WriteInt64(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64(long value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value)
            => WriteUInt64(value, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64(ulong value, bool littleEndian)
            => Write(littleEndian == BitConverter.IsLittleEndian ? value : BinaryPrimitives.ReverseEndianness(value));

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

        public Task WriteString(string s, int byteLen, bool async, CancellationToken cancellationToken = default)
            => WriteString(s, s.Length, byteLen, async, cancellationToken);

        public Task WriteString(string s, int charLen, int byteLen, bool async, CancellationToken cancellationToken = default)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteString(s, charLen);
                return Task.CompletedTask;
            }
            return WriteStringLong(this, async, s, charLen, byteLen, cancellationToken);

            static async Task WriteStringLong(NpgsqlWriteBuffer buffer, bool async, string s, int charLen, int byteLen, CancellationToken cancellationToken)
            {
                Debug.Assert(byteLen > buffer.WriteSpaceLeft);
                if (byteLen <= buffer.Size)
                {
                    // String can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below (which requires ToCharArray())
                    await buffer.Flush(async, cancellationToken);
                    buffer.WriteString(s, charLen);
                }
                else
                {
                    var charPos = 0;
                    while (true)
                    {
                        buffer.WriteStringChunked(s, charPos, charLen - charPos, true, out var charsUsed, out var completed);
                        if (completed)
                            break;
                        await buffer.Flush(async, cancellationToken);
                        charPos += charsUsed;
                    }
                }
            }
        }

        internal Task WriteChars(char[] chars, int offset, int charLen, int byteLen, bool async, CancellationToken cancellationToken = default)
        {
            if (byteLen <= WriteSpaceLeft)
            {
                WriteChars(chars, offset, charLen);
                return Task.CompletedTask;
            }
            return WriteCharsLong(this, async, chars, offset, charLen, byteLen, cancellationToken);

            static async Task WriteCharsLong(NpgsqlWriteBuffer buffer, bool async, char[] chars, int offset, int charLen, int byteLen, CancellationToken cancellationToken)
            {
                Debug.Assert(byteLen > buffer.WriteSpaceLeft);
                if (byteLen <= buffer.Size)
                {
                    // String can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below (which requires ToCharArray())
                    await buffer.Flush(async, cancellationToken);
                    buffer.WriteChars(chars, offset, charLen);
                }
                else
                {
                    var charPos = 0;

                    while (true)
                    {
                        buffer.WriteStringChunked(chars, charPos + offset, charLen - charPos, true, out var charsUsed, out var completed);
                        if (completed)
                            break;
                        await buffer.Flush(async, cancellationToken);
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

        public Task WriteBytesRaw(byte[] bytes, bool async, CancellationToken cancellationToken = default)
        {
            if (bytes.Length <= WriteSpaceLeft)
            {
                WriteBytes(bytes);
                return Task.CompletedTask;
            }
            return WriteBytesLong(this, async, bytes, cancellationToken);

            static async Task WriteBytesLong(NpgsqlWriteBuffer buffer, bool async, byte[] bytes, CancellationToken cancellationToken)
            {
                if (bytes.Length <= buffer.Size)
                {
                    // value can fit entirely in an empty buffer. Flush and retry rather than
                    // going into the partial writing flow below
                    await buffer.Flush(async, cancellationToken);
                    buffer.WriteBytes(bytes);
                }
                else
                {
                    var remaining = bytes.Length;
                    do
                    {
                        if (buffer.WriteSpaceLeft == 0)
                            await buffer.Flush(async, cancellationToken);
                        var writeLen = Math.Min(remaining, buffer.WriteSpaceLeft);
                        var offset = bytes.Length - remaining;
                        buffer.WriteBytes(bytes, offset, writeLen);
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
            if (WriteSpaceLeft < _textEncoder.GetByteCount(chars, charIndex, char.IsHighSurrogate(chars[charIndex]) ? 2 : 1, flush: false))
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
            int bytesUsed;

            fixed (char* sPtr = s)
            fixed (byte* bufPtr = Buffer)
            {
                if (WriteSpaceLeft < _textEncoder.GetByteCount(sPtr + charIndex, char.IsHighSurrogate(*(sPtr + charIndex)) ? 2 : 1, flush: false))
                {
                    charsUsed = 0;
                    completed = false;
                    return;
                }

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
            WriteByte(FrontendMessageCode.CopyData);
            // Leave space for the message length
            WriteInt32(0);
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (_disposed)
                return;

            ArrayPool<byte>.Shared.Return(Buffer);

            _timeoutCts.Dispose();
            _disposed = true;
        }

        #endregion

        #region Misc

        internal void Clear()
        {
            WritePosition = 0;
        }

        /// <summary>
        /// Returns all contents currently written to the buffer (but not flushed).
        /// Useful for pre-generating messages.
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
