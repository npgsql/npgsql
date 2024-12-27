using System;
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

namespace Npgsql.Internal;

/// <summary>
/// A buffer used by Npgsql to write data to the socket efficiently.
/// Provides methods which encode different values types and tracks the current position.
/// </summary>
sealed class NpgsqlWriteBuffer : IDisposable
{
    #region Fields and Properties

    internal static readonly UTF8Encoding UTF8Encoding = new(false, true);
    internal static readonly UTF8Encoding RelaxedUTF8Encoding = new(false, false);

    internal readonly NpgsqlConnector Connector;

    internal Stream Underlying { private get; set; }

    readonly Socket? _underlyingSocket;
    internal bool MessageLengthValidation { get; set; } = true;

    readonly ResettableCancellationTokenSource _timeoutCts;
    readonly MetricsReporter? _metricsReporter;

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

    // (Re)init to make sure we'll refetch from the write buffer.
    internal PgWriter GetWriter(NpgsqlDatabaseInfo typeCatalog, FlushMode flushMode = FlushMode.None)
        => _pgWriter.Init(typeCatalog, flushMode);

    internal readonly byte[] Buffer;
    readonly Encoder _textEncoder;

    internal int WritePosition;

    int _messageBytesFlushed;
    int? _messageLength;

    bool _disposed;
    readonly PgWriter _pgWriter;

    /// <summary>
    /// The minimum buffer size possible.
    /// </summary>
    internal const int MinimumSize = 4096;
    internal const int DefaultSize = 8192;

    #endregion

    #region Constructors

    internal NpgsqlWriteBuffer(
        NpgsqlConnector? connector,
        Stream stream,
        Socket? socket,
        int size,
        Encoding textEncoding)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, MinimumSize);

        Connector = connector!; // TODO: Clean this up; only null when used from PregeneratedMessages, where we don't care.
        Underlying = stream;
        _underlyingSocket = socket;
        _metricsReporter = connector?.DataSource.MetricsReporter!;
        _timeoutCts = new ResettableCancellationTokenSource();
        Buffer = new byte[size];
        Size = size;

        TextEncoding = textEncoding;
        _textEncoder = TextEncoding.GetEncoder();
        _pgWriter = new PgWriter(new NpgsqlBufferWriter(this));
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
        else
            AdvanceMessageBytesFlushed(WritePosition);

        var finalCt = async && Timeout > TimeSpan.Zero
            ? _timeoutCts.Start(cancellationToken)
            : cancellationToken;

        try
        {
            if (async)
            {
                await Underlying.WriteAsync(Buffer, 0, WritePosition, finalCt).ConfigureAwait(false);
                await Underlying.FlushAsync(finalCt).ConfigureAwait(false);
                if (Timeout > TimeSpan.Zero)
                    _timeoutCts.Stop();
            }
            else
            {
                Underlying.Write(Buffer, 0, WritePosition);
                Underlying.Flush();
            }
        }
        catch (Exception ex)
        {
            // Stopping twice (in case the previous Stop() call succeeded) doesn't hurt.
            // Not stopping will cause an assertion failure in debug mode when we call Start() the next time.
            // We can't stop in a finally block because Connector.Break() will dispose the buffer and the contained
            // _timeoutCts
            _timeoutCts.Stop();
            switch (ex)
            {
            // User requested the cancellation
            case OperationCanceledException when cancellationToken.IsCancellationRequested:
                throw Connector.Break(ex);
            // Read timeout
            case OperationCanceledException:
            case IOException { InnerException: SocketException { SocketErrorCode: SocketError.TimedOut } }:
                Debug.Assert(ex is OperationCanceledException ? async : !async);
                throw Connector.Break(new NpgsqlException("Exception while writing to stream", new TimeoutException("Timeout during writing attempt")));
            }

            throw Connector.Break(new NpgsqlException("Exception while writing to stream", ex));
        }
        NpgsqlEventSource.Log.BytesWritten(WritePosition);
        _metricsReporter?.ReportBytesWritten(WritePosition);

        WritePosition = 0;
        if (_copyMode)
            WriteCopyDataHeader();
    }

    internal void Flush() => Flush(false).GetAwaiter().GetResult();

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
            WriteInt32(checked(buffer.Length + 4));
            WritePosition = 5;
            _copyMode = false;
            StartMessage(5);
            Flush();
            _copyMode = true;
            WriteCopyDataHeader();  // And ready the buffer after the direct write completes
        }
        else
        {
            Debug.Assert(WritePosition == 0);
            AdvanceMessageBytesFlushed(buffer.Length);
        }

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
        await Flush(async, cancellationToken).ConfigureAwait(false);

        if (_copyMode)
        {
            // Flush has already written the CopyData header for us, but write the CopyData
            // header to the socket with the write length before we can start writing the data directly.
            Debug.Assert(WritePosition == 5);

            WritePosition = 1;
            WriteInt32(checked(memory.Length + 4));
            WritePosition = 5;
            _copyMode = false;
            StartMessage(5);
            await Flush(async, cancellationToken).ConfigureAwait(false);
            _copyMode = true;
            WriteCopyDataHeader();  // And ready the buffer after the direct write completes
        }
        else
        {
            Debug.Assert(WritePosition == 0);
            AdvanceMessageBytesFlushed(memory.Length);
        }

        try
        {
            if (async)
                await Underlying.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
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
    public void WriteByte(byte value)
    {
        CheckBounds<byte>();
        Buffer[WritePosition] = value;
        WritePosition += sizeof(byte);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short value)
    {
        CheckBounds<short>();
        Unsafe.WriteUnaligned(ref Buffer[WritePosition], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
        WritePosition += sizeof(short);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort value)
    {
        CheckBounds<ushort>();
        Unsafe.WriteUnaligned(ref Buffer[WritePosition], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
        WritePosition += sizeof(ushort);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int value)
    {
        CheckBounds<int>();
        Unsafe.WriteUnaligned(ref Buffer[WritePosition], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
        WritePosition += sizeof(int);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint value)
    {
        CheckBounds<uint>();
        Unsafe.WriteUnaligned(ref Buffer[WritePosition], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
        WritePosition += sizeof(uint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long value)
    {
        CheckBounds<long>();
        Unsafe.WriteUnaligned(ref Buffer[WritePosition], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value);
        WritePosition += sizeof(long);
    }

    [Conditional("DEBUG")]
    unsafe void CheckBounds<T>() where T : unmanaged
    {
        if (sizeof(T) > WriteSpaceLeft)
            ThrowNotSpaceLeft();
    }

    static void ThrowNotSpaceLeft()
        => ThrowHelper.ThrowInvalidOperationException("There is not enough space left in the buffer.");

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
                await buffer.Flush(async, cancellationToken).ConfigureAwait(false);
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
                    await buffer.Flush(async, cancellationToken).ConfigureAwait(false);
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

    public void WriteBytes(ReadOnlySpan<byte> buf)
    {
        Debug.Assert(buf.Length <= WriteSpaceLeft);
        buf.CopyTo(new Span<byte>(Buffer, WritePosition, Buffer.Length - WritePosition));
        WritePosition += buf.Length;
    }

    public void WriteBytes(ReadOnlyMemory<byte> buf)
        => WriteBytes(buf.Span);

    public void WriteBytes(byte[] buf) => WriteBytes(buf.AsSpan());

    public void WriteBytes(byte[] buf, int offset, int count)
        => WriteBytes(new ReadOnlySpan<byte>(buf, offset, count));

    public Task WriteBytesRaw(ReadOnlyMemory<byte> bytes, bool async, CancellationToken cancellationToken = default)
    {
        if (bytes.Length <= WriteSpaceLeft)
        {
            WriteBytes(bytes);
            return Task.CompletedTask;
        }
        return WriteBytesLong(this, async, bytes, cancellationToken);

        static async Task WriteBytesLong(NpgsqlWriteBuffer buffer, bool async, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
        {
            if (bytes.Length <= buffer.Size)
            {
                // value can fit entirely in an empty buffer. Flush and retry rather than
                // going into the partial writing flow below
                await buffer.Flush(async, cancellationToken).ConfigureAwait(false);
                buffer.WriteBytes(bytes);
            }
            else
            {
                var remaining = bytes.Length;
                do
                {
                    if (buffer.WriteSpaceLeft == 0)
                        await buffer.Flush(async, cancellationToken).ConfigureAwait(false);
                    var writeLen = Math.Min(remaining, buffer.WriteSpaceLeft);
                    var offset = bytes.Length - remaining;
                    buffer.WriteBytes(bytes.Slice(offset, writeLen));
                    remaining -= writeLen;
                }
                while (remaining > 0);
            }
        }
    }

    public async Task WriteStreamRaw(Stream stream, int count, bool async, CancellationToken cancellationToken = default)
    {
        while (count > 0)
        {
            if (WriteSpaceLeft == 0)
                await Flush(async, cancellationToken).ConfigureAwait(false);
            try
            {
                var read = async
                    ? await stream.ReadAsync(Buffer, WritePosition, Math.Min(WriteSpaceLeft, count), cancellationToken).ConfigureAwait(false)
                    : stream.Read(Buffer, WritePosition, Math.Min(WriteSpaceLeft, count));
                if (read == 0)
                    throw new EndOfStreamException();
                WritePosition += read;
                count -= read;
            }
            catch (Exception e)
            {
                throw Connector.Break(new NpgsqlException("Exception while writing to stream", e));
            }
        }
        Debug.Assert(count == 0);
    }

    public void WriteNullTerminatedString(string s)
    {
        AssertASCIIOnly(s);
        Debug.Assert(WriteSpaceLeft >= s.Length + 1);
        WritePosition += Encoding.ASCII.GetBytes(s, 0, s.Length, Buffer, WritePosition);
        WriteByte(0);
    }

    public void WriteNullTerminatedString(byte[] s)
    {
        AssertASCIIOnly(s);
        Debug.Assert(WriteSpaceLeft >= s.Length + 1);
        WriteBytes(s);
        WriteByte(0);
    }

    #endregion

    #region Write Complex

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

        _timeoutCts.Dispose();
        _disposed = true;
    }

    #endregion

    #region Misc

    internal void StartMessage(int messageLength)
    {
        if (!MessageLengthValidation)
            return;

        if (_messageLength is not null && _messageBytesFlushed != _messageLength && WritePosition != -_messageBytesFlushed + _messageLength)
            Throw();

        // Add negative WritePosition to compensate for previous message(s) written without flushing.
        _messageBytesFlushed = -WritePosition;
        _messageLength = messageLength;

        void Throw()
        {
            throw Connector.Break(new OverflowException("Did not write the amount of bytes the message length specified"));
        }
    }

    void AdvanceMessageBytesFlushed(int count)
    {
        if (!MessageLengthValidation)
            return;

        if (count < 0 || _messageLength is null || (long)_messageBytesFlushed + count > _messageLength)
            Throw();

        _messageBytesFlushed += count;

        void Throw()
        {
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            if (_messageLength is null)
                throw Connector.Break(new InvalidOperationException("No message was started"));

            if ((long)_messageBytesFlushed + count > _messageLength)
                throw Connector.Break(new OverflowException("Tried to write more bytes than the message length specified"));
        }
    }

    internal void Clear()
    {
        WritePosition = 0;
        _messageLength = null;
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

    [Conditional("DEBUG")]
    internal static void AssertASCIIOnly(string s)
    {
        foreach (var c in s)
            if (c >= 128)
                Debug.Fail("Method only supports ASCII strings");
    }

    [Conditional("DEBUG")]
    internal static void AssertASCIIOnly(byte[] s)
    {
        foreach (var c in s)
            if (c >= 128)
                Debug.Fail("Method only supports ASCII strings");
    }

    #endregion
}
