using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// A type handler for PostgreSQL character data types (text, char, varchar, xml...).
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/datatype-character.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public partial class TextHandler : NpgsqlTypeHandler<string>, INpgsqlTypeHandler<char[]>, INpgsqlTypeHandler<ArraySegment<char>>,
    INpgsqlTypeHandler<char>, INpgsqlTypeHandler<byte[]>, INpgsqlTypeHandler<ReadOnlyMemory<byte>>, ITextReaderHandler
{
    // Text types are handled a bit more efficiently when sent as text than as binary
    // see https://github.com/npgsql/npgsql/issues/1210#issuecomment-235641670
    internal override bool PreferTextWrite => true;

    readonly Encoding _encoding;

    /// <inheritdoc />
    protected internal TextHandler(PostgresType postgresType, Encoding encoding)
        : base(postgresType)
        => _encoding = encoding;

    #region Read

    /// <inheritdoc />
    public override ValueTask<string> Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription = null)
    {
        return buf.ReadBytesLeft >= byteLen
            ? new ValueTask<string>(buf.ReadString(byteLen))
            : ReadLong(_encoding, buf, byteLen, async);

        static async ValueTask<string> ReadLong(Encoding encoding, NpgsqlReadBuffer buf, int byteLen, bool async)
        {
            if (byteLen <= buf.Size)
            {
                // The string's byte representation can fit in our read buffer, read it.
                await buf.Ensure(byteLen, async);
                return buf.ReadString(byteLen);
            }

            // Bad case: the string's byte representation doesn't fit in our buffer.
            // This is rare - will only happen in CommandBehavior.Sequential mode (otherwise the
            // entire row is in memory). Tweaking the buffer length via the connection string can

            var tempBuf = ArrayPool<byte>.Shared.Rent(byteLen);

            try
            {
                var pos = 0;
                while (true)
                {
                    var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                    buf.ReadBytes(tempBuf, pos, len);
                    pos += len;
                    if (pos < byteLen)
                    {
                        await buf.ReadMore(async);
                        continue;
                    }
                    break;
                }
                return encoding.GetString(tempBuf, 0, byteLen);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(tempBuf);
            }
        }
    }

    async ValueTask<char[]> INpgsqlTypeHandler<char[]>.Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription)
    {
        if (byteLen <= buf.Size)
        {
            // The string's byte representation can fit in our read buffer, read it.
            await buf.Ensure(byteLen, async);
            return buf.ReadChars(byteLen);
        }

        var tempBuf = ArrayPool<byte>.Shared.Rent(byteLen);

        try
        {
            var pos = 0;
            while (true)
            {
                var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                buf.ReadBytes(tempBuf, pos, len);
                pos += len;
                if (pos < byteLen)
                {
                    await buf.ReadMore(async);
                    continue;
                }
                break;
            }
            return _encoding.GetChars(tempBuf, 0, byteLen);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(tempBuf);
        }
    }

    async ValueTask<char> INpgsqlTypeHandler<char>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
    {
        // Make sure we have enough bytes in the buffer for a single character
        // We can get here a much bigger length in case it's a string
        // while we want to read only its first character
        var maxBytes = Math.Min(_encoding.GetMaxByteCount(1), len);
        await buf.Ensure(maxBytes, async);

        var character = ReadCharCore();

        // We've been requested to read 'len' bytes, which is why we're going to skip them
        // This is important for NpgsqlDataReader with CommandBehavior.SequentialAccess
        // which tracks how many bytes it has to skip for the next column
        await buf.Skip(len, async);
        return character;

        char ReadCharCore()
        {
            var charSpan = buf.Buffer.AsSpan(buf.ReadPosition, maxBytes);
            var chars = _encoding.GetCharCount(charSpan);
            if (chars < 1)
                throw new NpgsqlException("Could not read char - string was empty");

            Span<char> destination = stackalloc char[chars];
            _encoding.GetChars(charSpan, destination);
            return destination[0];
        }
    }

    ValueTask<ArraySegment<char>> INpgsqlTypeHandler<ArraySegment<char>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException("Only writing ArraySegment<char> to PostgreSQL text is supported, no reading.");

    ValueTask<byte[]> INpgsqlTypeHandler<byte[]>.Read(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription)
    {
        var bytes = new byte[byteLen];
        if (buf.ReadBytesLeft >= byteLen)
        {
            buf.ReadBytes(bytes, 0, byteLen);
            return new ValueTask<byte[]>(bytes);
        }
        return ReadLong(buf, bytes, byteLen, async);

        static async ValueTask<byte[]> ReadLong(NpgsqlReadBuffer buf, byte[] bytes, int byteLen, bool async)
        {
            if (byteLen <= buf.Size)
            {
                // The bytes can fit in our read buffer, read it.
                await buf.Ensure(byteLen, async);
                buf.ReadBytes(bytes, 0, byteLen);
                return bytes;
            }

            // Bad case: the bytes don't fit in our buffer.
            // This is rare - will only happen in CommandBehavior.Sequential mode (otherwise the
            // entire row is in memory). Tweaking the buffer length via the connection string can
            // help avoid this.

            var pos = 0;
            while (true)
            {
                var len = Math.Min(buf.ReadBytesLeft, byteLen - pos);
                buf.ReadBytes(bytes, pos, len);
                pos += len;
                if (pos < byteLen)
                {
                    await buf.ReadMore(async);
                    continue;
                }
                break;
            }
            return bytes;
        }
    }

    ValueTask<ReadOnlyMemory<byte>> INpgsqlTypeHandler<ReadOnlyMemory<byte>>.Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
        => throw new NotSupportedException("Only writing ReadOnlyMemory<byte> to PostgreSQL text is supported, no reading.");

    #endregion

    #region Write

    /// <inheritdoc />
    public override unsafe int ValidateAndGetLength(string value, [NotNullIfNotNull("lengthCache")] ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        lengthCache ??= new NpgsqlLengthCache(1);
        if (lengthCache.IsPopulated)
            return lengthCache.Get();

        if (parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length)
            return lengthCache.Set(_encoding.GetByteCount(value));
        fixed (char* p = value)
            return lengthCache.Set(_encoding.GetByteCount(p, parameter.Size));
    }

    /// <inheritdoc />
    public virtual int ValidateAndGetLength(char[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        lengthCache ??= new NpgsqlLengthCache(1);
        if (lengthCache.IsPopulated)
            return lengthCache.Get();

        return lengthCache.Set(
            parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
                ? _encoding.GetByteCount(value)
                : _encoding.GetByteCount(value, 0, parameter.Size)
        );
    }

    /// <inheritdoc />
    public virtual int ValidateAndGetLength(ArraySegment<char> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        lengthCache ??= new NpgsqlLengthCache(1);
        if (lengthCache.IsPopulated)
            return lengthCache.Get();

        if (parameter?.Size > 0)
            throw new ArgumentException($"Parameter {parameter.ParameterName} is of type ArraySegment<char> and should not have its Size set", parameter.ParameterName);

        return lengthCache.Set(value.Array is null ? 0 : _encoding.GetByteCount(value.Array, value.Offset, value.Count));
    }

    /// <inheritdoc />
    public int ValidateAndGetLength(char value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        Span<char> singleCharArray = stackalloc char[1];
        singleCharArray[0] = value;
        return _encoding.GetByteCount(singleCharArray);
    }

    /// <inheritdoc />
    public int ValidateAndGetLength(byte[] value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => value.Length;

    /// <inheritdoc />
    public int ValidateAndGetLength(ReadOnlyMemory<byte> value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => value.Length;

    /// <inheritdoc />
    public override Task Write(string value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => WriteString(value, buf, lengthCache!, parameter, async, cancellationToken);

    /// <inheritdoc />
    public virtual Task Write(char[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= value.Length
            ? value.Length
            : parameter.Size;
        return buf.WriteChars(value, 0, charLen, lengthCache!.GetLast(), async, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task Write(ArraySegment<char> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value.Array is null ? Task.CompletedTask : buf.WriteChars(value.Array, value.Offset, value.Count, lengthCache!.GetLast(), async, cancellationToken);

    Task WriteString(string str, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        var charLen = parameter == null || parameter.Size <= 0 || parameter.Size >= str.Length
            ? str.Length
            : parameter.Size;
        return buf.WriteString(str, charLen, lengthCache.GetLast(), async, cancellationToken);
    }

    /// <inheritdoc />
    public async Task Write(char value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        if (buf.WriteSpaceLeft < _encoding.GetMaxByteCount(1))
            await buf.Flush(async, cancellationToken);
        WriteCharCore(value, buf);

        static unsafe void WriteCharCore(char value, NpgsqlWriteBuffer buf)
        {
            Span<char> singleCharArray = stackalloc char[1];
            singleCharArray[0] = value;
            buf.WriteChars(singleCharArray);
        }
    }


    public Task Write(byte[] value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken = default)
        => buf.WriteBytesRaw(value, async, cancellationToken);

    public Task Write(ReadOnlyMemory<byte> value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async,
        CancellationToken cancellationToken = default)
        => buf.WriteBytesRaw(value, async, cancellationToken);

    #endregion

    /// <inheritdoc />
    public virtual TextReader GetTextReader(Stream stream, NpgsqlReadBuffer buffer)
    {
        var byteLength = (int)(stream.Length - stream.Position);
        return buffer.ReadBytesLeft >= byteLength
            ? buffer.GetPreparedTextReader(_encoding.GetString(buffer.Buffer, buffer.ReadPosition, byteLength), stream)
            : new StreamReader(stream, _encoding);
    }
}
