using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Util;

// Adapted from https://github.com/dotnet/runtime/blob/83adfae6a6273d8fb4c69554aa3b1cc7cbf01c71/src/libraries/System.IO.Compression/src/System/IO/Compression/ZipCustomStreams.cs#L221
sealed class SubReadStream : Stream
{
    readonly long _start;
    long _position;
    readonly long _end;
    readonly Stream? _stream;
    readonly ArraySegment<byte> _buffer;
    readonly bool _canSeek;
    bool _isDisposed;
    internal bool IsDisposed => _isDisposed;

    public SubReadStream(Stream source, long maxLength)
    {
        _start = -1;
        _position = 0;
        _end = maxLength;
        _stream = source;
        _canSeek = false;
    }

    public SubReadStream(Stream source, long startPosition, long maxLength)
    {
        _start = startPosition;
        _position = startPosition;
        _end = startPosition + maxLength;
        _stream = source;
        _canSeek = source.CanSeek;
    }

    public SubReadStream(byte[] buffer, int offset, int count)
    {
        _buffer = new ArraySegment<byte>(buffer, offset, count);
        _start = 0;
        _position = 0;
        _end = count;
        _canSeek = true;
    }

    public override long Length
    {
        get
        {
            ThrowIfDisposed();

            if (!_canSeek)
                throw new NotSupportedException();

            return _end - _start;
        }
    }

    public override long Position
    {
        get
        {
            ThrowIfDisposed();

            if (!_canSeek)
                throw new NotSupportedException();

            return _position - _start;
        }
        set
        {
            ThrowIfDisposed();

            if (!_canSeek)
                throw new NotSupportedException();

            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _position = _start + value;
        }
    }

    public override bool CanRead => _buffer.Array is not null || _stream!.CanRead;

    public override bool CanSeek => _canSeek;

    public override bool CanWrite => false;

    void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_isDisposed, this);

    void ThrowIfCantRead()
    {
        if (!CanRead)
            throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        ValidateBufferArguments(buffer, offset, count);
        return Read(new Span<byte>(buffer, offset, count));
    }

    public override int Read(Span<byte> destination)
    {
        ThrowIfDisposed();

        var count = destination.Length;
        if (_position + count > _end)
            count = (int)(_end - _position);

        if (count <= 0)
            return 0;

        if (_buffer.Array is not null)
        {
            _buffer.AsSpan((int)_position, count).CopyTo(destination);
            _position += count;
            return count;
        }

        ThrowIfCantRead();
        if (_canSeek && _stream!.Position != _position)
            _stream.Seek(_position, SeekOrigin.Begin);

        var ret = _stream!.Read(destination.Slice(0, count));
        _position += ret;
        return ret;
    }

    public override int ReadByte()
    {
        Span<byte> b = stackalloc byte[1];
        return Read(b) == 1 ? b[0] : -1;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        ValidateBufferArguments(buffer, offset, count);
        return ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_buffer.Array is not null)
            return new(Read(buffer.Span));

        ThrowIfCantRead();
        if (_canSeek && _stream!.Position != _position)
            _stream.Seek(_position, SeekOrigin.Begin);

        if (_position > _end - buffer.Length)
            buffer = buffer.Slice(0, (int)(_end - _position));

        return Core(buffer, cancellationToken);

        async ValueTask<int> Core(Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var ret = await _stream!.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            _position += ret;
            return ret;
        }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        ThrowIfDisposed();

        if (!_canSeek)
            throw new NotSupportedException();

        var newPosition = origin switch
        {
            SeekOrigin.Begin => _start + offset,
            SeekOrigin.Current => _position + offset,
            SeekOrigin.End => _end + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        if (newPosition < _start)
            throw new IOException("An attempt was made to move the position before the beginning of the stream.");

        _position = newPosition;
        return _position - _start;
    }

    public override void SetLength(long value)
    {
        ThrowIfDisposed();
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        ThrowIfDisposed();
        throw new NotSupportedException();
    }

    public override void Flush() => ThrowIfDisposed();

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_isDisposed)
        {
            _isDisposed = true;
        }
        base.Dispose(disposing);
    }
}
