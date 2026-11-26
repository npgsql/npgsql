using System;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql.Internal;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Npgsql.Json.NET.Internal;

sealed class JsonNetJsonConverter<T>(bool jsonb, JsonSerializerSettings settings) : PgStreamingConverter<T?>
{
    readonly JsonSerializer _serializer = JsonSerializer.CreateDefault(settings);

    public override T? Read(PgReader reader)
        => (T?)JsonNetJsonConverter.Read(async: false, jsonb, reader, typeof(T), _serializer, reader.ConversionContext.TextEncoding, CancellationToken.None).GetAwaiter().GetResult();
    public override async ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => (T?)await JsonNetJsonConverter.Read(async: true, jsonb, reader, typeof(T), _serializer, reader.ConversionContext.TextEncoding, cancellationToken).ConfigureAwait(false);

    protected override Size BindValue(in BindContext context, T? value, ref object? writeState)
        => JsonNetJsonConverter.BindValue(jsonb, context, typeof(T), _serializer, context.ConversionContext.TextEncoding, value, ref writeState);

    public override void Write(PgWriter writer, T? value)
        => JsonNetJsonConverter.Write(jsonb, async: false, writer, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => JsonNetJsonConverter.Write(jsonb, async: true, writer, cancellationToken);
}

// Split out to avoid unnecessary code duplication.
static class JsonNetJsonConverter
{
    public const byte JsonbProtocolVersion = 1;

    public static async ValueTask<object?> Read(bool async, bool jsonb, PgReader reader, Type type, JsonSerializer serializer, Encoding encoding, CancellationToken cancellationToken)
    {
        if (jsonb)
        {
            if (reader.ShouldBuffer(sizeof(byte)))
            {
                if (async)
                    await reader.BufferAsync(sizeof(byte), cancellationToken).ConfigureAwait(false);
                else
                    reader.Buffer(sizeof(byte));
            }
            var version = reader.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new InvalidCastException($"Unknown jsonb wire format version {version}");
        }

        // Json.NET only reads synchronously, so the value is fully read before deserializing it. When the value is
        // already in the read buffer it is decoded from there, otherwise it is first read into a rented array.
        var byteCount = reader.CurrentRemaining;
        byte[]? rentedBytes = null;
        if (!reader.TryReadBytes(byteCount, out ReadOnlyMemory<byte> bytes))
        {
            rentedBytes = ArrayPool<byte>.Shared.Rent(byteCount);
            if (async)
                await reader.ReadBytesAsync(rentedBytes.AsMemory(0, byteCount), cancellationToken).ConfigureAwait(false);
            else
                reader.ReadBytes(rentedBytes.AsSpan(0, byteCount));
            bytes = rentedBytes.AsMemory(0, byteCount);
        }

        var rentedChars = ArrayPool<char>.Shared.Rent(encoding.GetMaxCharCount(byteCount));
        try
        {
            var charCount = encoding.GetChars(bytes.Span, rentedChars);
            using var textReader = new JsonTextReader(new CharArrayReader(rentedChars, charCount)) { ArrayPool = JsonArrayPool.Instance };
            return serializer.Deserialize(textReader, type);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rentedChars);
            if (rentedBytes is not null)
                ArrayPool<byte>.Shared.Return(rentedBytes);
        }
    }

    public static Size BindValue(bool jsonb, in BindContext context, Type type, JsonSerializer serializer, Encoding encoding, object? value, ref object? writeState)
    {
        var sb = new StringBuilder(256);
        var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
        using (var jsonWriter = new JsonTextWriter(sw))
        {
            jsonWriter.Formatting = serializer.Formatting;

            serializer.Serialize(jsonWriter, value, type);
        }

        var str = sw.ToString();
        var bytes = encoding.GetBytes(str);
        writeState = bytes;
        return bytes.Length + (jsonb ? sizeof(byte) : 0);
    }

    public static async ValueTask Write(bool jsonb, bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (jsonb)
        {
            if (writer.ShouldFlush(sizeof(byte)))
            {
                if (async)
                    await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
                else
                    writer.Flush();
            }
            writer.WriteByte(JsonbProtocolVersion);
        }

        ArraySegment<byte> buffer;
        switch (writer.Current.WriteState)
        {
        case byte[] bytes:
            buffer = new ArraySegment<byte>(bytes);
            break;
        default:
            throw new InvalidCastException($"Invalid state {writer.Current.WriteState?.GetType().FullName}.");
        }

        if (async)
            await writer.WriteBytesAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false);
        else
            writer.WriteBytes(buffer.AsSpan());
    }
    sealed class CharArrayReader(char[] chars, int length) : TextReader
    {
        int _position;

        public override int Peek() => _position < length ? chars[_position] : -1;

        public override int Read() => _position < length ? chars[_position++] : -1;

        public override int Read(char[] buffer, int index, int count) => Read(buffer.AsSpan(index, count));

        public override int Read(Span<char> buffer)
        {
            var count = Math.Min(buffer.Length, length - _position);
            chars.AsSpan(_position, count).CopyTo(buffer);
            _position += count;
            return count;
        }
    }

    sealed class JsonArrayPool : IArrayPool<char>
    {
        public static readonly JsonArrayPool Instance = new();

        public char[] Rent(int minimumLength) => ArrayPool<char>.Shared.Rent(minimumLength);

        public void Return(char[]? array)
        {
            if (array is not null)
                ArrayPool<char>.Shared.Return(array);
        }
    }
}
