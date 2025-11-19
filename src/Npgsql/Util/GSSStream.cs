using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Util;

// For more detailed explanation of communication protocol
// See https://www.postgresql.org/docs/current/protocol-flow.html#PROTOCOL-FLOW-GSSAPI
sealed class GSSStream : Stream
{
    // At most, postgres supports GSS messages up to 16kb
    // We use the recommended value of 8kb for the write buffer
    // Which will result in messages of slightly larger than 8kb
    const int MaxWriteMessageSizeLimit = 8 * 1024;
    const int MaxReadMessageSizeLimit = 16 * 1024;

    readonly Stream _stream;
    readonly NegotiateAuthentication _authentication;

    readonly ArrayBufferWriter<byte> _writeBuffer;
    readonly byte[] _writeLengthBuffer;

    readonly byte[] _readBuffer;
    int _readPosition;
    int _leftToRead;

    internal GSSStream(Stream stream, NegotiateAuthentication authentication)
    {
        _stream = stream;
        _authentication = authentication;
        // While we guarantee that unencrypted messages are at most 8kb
        // Encrypting them will result in messages slightly larger than the original size
        // Which is why the initial capacity has an additional 2kb of free space
        _writeBuffer = new ArrayBufferWriter<byte>(MaxWriteMessageSizeLimit + 2048);
        _writeLengthBuffer = new byte[4];
        _readBuffer = new byte[MaxReadMessageSizeLimit];
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        var start = 0;
        while (start != buffer.Length)
        {
            var lengthToWrite = Math.Min(buffer.Length - start, MaxWriteMessageSizeLimit);
            var result = _authentication.Wrap(
                buffer.Slice(start, lengthToWrite),
                _writeBuffer,
                _authentication.IsEncrypted,
                out _);
            if (result != NegotiateAuthenticationStatusCode.Completed)
                throw new NpgsqlException($"Error while encrypting buffer: {result}");

            var written = _writeBuffer.WrittenMemory;
            Unsafe.WriteUnaligned(ref _writeLengthBuffer[0], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(written.Length) : written.Length);

            _stream.Write(_writeLengthBuffer);
            _stream.Write(_writeBuffer.WrittenMemory.Span);

            _writeBuffer.ResetWrittenCount();
            start += lengthToWrite;
        }
    }

    public override void Write(byte[] buffer, int offset, int count)
        => Write(buffer.AsSpan(offset, count));

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var start = 0;
        while (start != buffer.Length)
        {
            var lengthToWrite = Math.Min(buffer.Length - start, MaxWriteMessageSizeLimit);
            var result = _authentication.Wrap(
                buffer.Slice(start, lengthToWrite).Span,
                _writeBuffer,
                _authentication.IsEncrypted,
                out _);
            if (result != NegotiateAuthenticationStatusCode.Completed)
                throw new NpgsqlException($"Error while encrypting buffer: {result}");

            var written = _writeBuffer.WrittenMemory;
            Unsafe.WriteUnaligned(ref _writeLengthBuffer[0], BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(written.Length) : written.Length);

            await _stream.WriteAsync(_writeLengthBuffer, cancellationToken).ConfigureAwait(false);
            await _stream.WriteAsync(_writeBuffer.WrittenMemory, cancellationToken).ConfigureAwait(false);

            _writeBuffer.ResetWrittenCount();
            start += lengthToWrite;
        }
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await WriteAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);

    public override void Flush() => _stream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => _stream.FlushAsync(cancellationToken);

    public override int Read(Span<byte> buffer)
    {
        if (_leftToRead == 0)
        {
            _stream.ReadExactly(_readBuffer.AsSpan(0, 4));
            var messageLength = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref _readBuffer[0]))
                : Unsafe.ReadUnaligned<int>(ref _readBuffer[0]);
            var messageBuffer = _readBuffer.AsSpan(0, messageLength);
            _stream.ReadExactly(messageBuffer);
            var result = _authentication.UnwrapInPlace(messageBuffer, out _readPosition, out _leftToRead, out _);
            if (result != NegotiateAuthenticationStatusCode.Completed)
                throw new NpgsqlException($"Error while decrypting buffer: {result}");
        }

        var maxRead = Math.Min(_leftToRead, buffer.Length);
        _readBuffer.AsSpan(_readPosition, maxRead).CopyTo(buffer);
        _readPosition += maxRead;
        _leftToRead -= maxRead;
        return maxRead;
    }

    public override int Read(byte[] buffer, int offset, int count)
        => Read(buffer.AsSpan(offset, count));

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_leftToRead == 0)
        {
            await _stream.ReadExactlyAsync(_readBuffer.AsMemory(0, 4), cancellationToken).ConfigureAwait(false);
            var messageLength = BitConverter.IsLittleEndian
                ? BinaryPrimitives.ReverseEndianness(Unsafe.ReadUnaligned<int>(ref _readBuffer[0]))
                : Unsafe.ReadUnaligned<int>(ref _readBuffer[0]);
            var messageBuffer = _readBuffer.AsMemory(0, messageLength);
            await _stream.ReadExactlyAsync(messageBuffer, cancellationToken).ConfigureAwait(false);
            var result = _authentication.UnwrapInPlace(messageBuffer.Span, out _readPosition, out _leftToRead, out _);
            if (result != NegotiateAuthenticationStatusCode.Completed)
                throw new NpgsqlException($"Error while decrypting buffer: {result}");
        }

        var maxRead = Math.Min(_leftToRead, buffer.Length);
        _readBuffer.AsMemory(_readPosition, maxRead).CopyTo(buffer);
        _readPosition += maxRead;
        _leftToRead -= maxRead;
        return maxRead;
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await ReadAsync(buffer.AsMemory(offset, count), cancellationToken).ConfigureAwait(false);

    public override void Close() => _stream.Close();

    protected override void Dispose(bool disposing)
    {
        _authentication.Dispose();
        _stream.Dispose();
    }

    public override ValueTask DisposeAsync() => _stream.DisposeAsync();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();

    public override bool CanRead => true;
    public override bool CanWrite => true;
    public override bool CanSeek => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
}
