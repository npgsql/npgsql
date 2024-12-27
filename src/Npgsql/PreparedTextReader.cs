using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql;

sealed class PreparedTextReader : TextReader
{
    string _str = null!;
    NpgsqlReadBuffer.ColumnStream _stream = null!;

    int _position;
    bool _disposed;

    public void Init(string str, NpgsqlReadBuffer.ColumnStream stream)
    {
        _str = str;
        _stream = stream;
        _disposed = false;
        _position = 0;
    }

    public bool IsDisposed => _disposed;

    public override int Peek()
    {
        CheckDisposed();

        return _position < _str.Length
            ? _str[_position]
            : -1;
    }

    public override int Read()
    {
        CheckDisposed();

        return _position < _str.Length
            ? _str[_position++]
            : -1;
    }

    public override int Read(Span<char> buffer)
    {
        CheckDisposed();

        var toRead = Math.Min(buffer.Length, _str.Length - _position);
        if (toRead == 0)
            return 0;

        _str.AsSpan(_position, toRead).CopyTo(buffer);
        _position += toRead;
        return toRead;
    }

    public override int Read(char[] buffer, int index, int count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if (buffer.Length - index < count)
        {
            ThrowHelper.ThrowArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
        }

        return Read(buffer.AsSpan(index, count));
    }

    public override Task<int> ReadAsync(char[] buffer, int index, int count)
        => Task.FromResult(Read(buffer, index, count));

    public override ValueTask<int> ReadAsync(Memory<char> buffer, CancellationToken cancellationToken = default) => new(Read(buffer.Span));

    public override Task<string?> ReadLineAsync() => Task.FromResult<string?>(ReadLine());

    public override string ReadToEnd()
    {
        CheckDisposed();

        if (_position == _str.Length)
            return string.Empty;

        var str = _str.Substring(_position);
        _position = _str.Length;
        return str;
    }

    public override Task<string> ReadToEndAsync() => Task.FromResult(ReadToEnd());

    void CheckDisposed()
        => ObjectDisposedException.ThrowIf(_disposed || _stream.IsDisposed, this);

    public void Restart()
    {
        CheckDisposed();
        _position = 0;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _disposed = true;
            _stream.Dispose();
        }
    }
}
