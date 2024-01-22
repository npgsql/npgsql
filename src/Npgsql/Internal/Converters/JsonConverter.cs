using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Npgsql.Internal.Converters;

sealed class JsonConverter<T, TBase> : PgStreamingConverter<T?> where T: TBase?
{
    readonly bool _jsonb;
    readonly Encoding _textEncoding;
    readonly JsonTypeInfo _jsonTypeInfo;
    readonly JsonTypeInfo<object?>? _objectTypeInfo;

    public JsonConverter(bool jsonb, Encoding textEncoding, JsonSerializerOptions serializerOptions)
    {
        if (serializerOptions.TypeInfoResolver is null)
            throw new InvalidOperationException("System.Text.Json serialization requires a type info resolver, make sure to set-it up beforehand.");

        _jsonb = jsonb;
        _textEncoding = textEncoding;
        _jsonTypeInfo = typeof(TBase) != typeof(object) && typeof(T) != typeof(TBase)
            ? (JsonTypeInfo<TBase>)serializerOptions.GetTypeInfo(typeof(TBase))
            : (JsonTypeInfo<T>)serializerOptions.GetTypeInfo(typeof(T));
        // Unspecified polymorphism, let STJ handle it.
        _objectTypeInfo = typeof(TBase) == typeof(object)
            ? (JsonTypeInfo<object?>)serializerOptions.GetTypeInfo(typeof(object))
            : null;
    }

    public override T? Read(PgReader reader)
        => Read(async: false, reader, CancellationToken.None).GetAwaiter().GetResult();
    public override ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => Read(async: true, reader, cancellationToken);

    async ValueTask<T?> Read(bool async, PgReader reader, CancellationToken cancellationToken)
    {
        if (_jsonb && reader.ShouldBuffer(sizeof(byte)))
            await reader.Buffer(async, sizeof(byte), cancellationToken).ConfigureAwait(false);

        // We always fall back to buffers on older targets due to the absence of transcoding stream.
        if (JsonConverter.TryReadStream(_jsonb, _textEncoding, reader, out var byteCount, out var stream))
        {
            using var _ = stream;
            return _jsonTypeInfo switch
            {
                JsonTypeInfo<JsonDocument> => (T)(object)(async
                    ? await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false)
                    : JsonDocument.Parse(stream)),

                JsonTypeInfo<T> typeInfoOfT => async
                    ? await JsonSerializer.DeserializeAsync(stream, typeInfoOfT, cancellationToken).ConfigureAwait(false)
                    : JsonSerializer.Deserialize(stream, typeInfoOfT),

                _ => (T?)(async
                    ? await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo, cancellationToken)
                        .ConfigureAwait(false)
                    : JsonSerializer.Deserialize(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo))
            };
        }

        var (rentedChars, rentedBytes) = await JsonConverter.ReadRentedBuffer(async, _textEncoding, byteCount, reader, cancellationToken).ConfigureAwait(false);
        var result = _jsonTypeInfo switch
        {
            JsonTypeInfo<JsonDocument> => (T)(object)JsonDocument.Parse(rentedChars.AsMemory()),
            JsonTypeInfo<T> typeInfoOfT => JsonSerializer.Deserialize(rentedChars.AsSpan(), typeInfoOfT),
            _ => (T?)JsonSerializer.Deserialize(rentedChars.AsSpan(), (JsonTypeInfo<TBase?>)_jsonTypeInfo)
        };

        ArrayPool<char>.Shared.Return(rentedChars.Array!);
        if (rentedBytes is not null)
            ArrayPool<byte>.Shared.Return(rentedBytes);

        return result;
    }

    public override Size GetSize(SizeContext context, T? value, ref object? writeState)
    {
        var capacity = 0;
        if (typeof(T) == typeof(JsonDocument))
            capacity = ((JsonDocument?)(object?)value)?.RootElement.GetRawText().Length ?? 0;
        var stream = new MemoryStream(capacity);

        // Mirroring ASP.NET Core serialization strategy https://github.com/dotnet/aspnetcore/issues/47548
        if (_objectTypeInfo is null)
            JsonSerializer.Serialize(stream, value, (JsonTypeInfo<TBase?>)_jsonTypeInfo);
        else
            JsonSerializer.Serialize(stream, value, _objectTypeInfo);

        return JsonConverter.GetSizeCore(_jsonb, stream, _textEncoding, ref writeState);
    }

    public override void Write(PgWriter writer, T? value)
        => JsonConverter.Write(_jsonb, async: false, writer, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => JsonConverter.Write(_jsonb, async: true, writer, cancellationToken);
}

// Split out to avoid unnecessary code duplication.
static class JsonConverter
{
    public const byte JsonbProtocolVersion = 1;
    // We pick a value that is the largest multiple of 4096 that is still smaller than the large object heap threshold (85K).
    const int StreamingThreshold = 81920;

    public static bool TryReadStream(bool jsonb, Encoding encoding, PgReader reader, out int byteCount, [NotNullWhen(true)]out Stream? stream)
    {
        if (jsonb)
        {
            var version = reader.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new InvalidCastException($"Unknown jsonb wire format version {version}");
        }

        var isUtf8 = encoding.CodePage == Encoding.UTF8.CodePage;
        byteCount = reader.CurrentRemaining;
        if (isUtf8 || byteCount >= StreamingThreshold)
        {
            stream = !isUtf8
                ? Encoding.CreateTranscodingStream(reader.GetStream(), encoding, Encoding.UTF8)
                : reader.GetStream();
        }
        else
            stream = null;

        return stream is not null;
    }

    public static async ValueTask<(ArraySegment<char> RentedChars, byte[]? RentedBytes)> ReadRentedBuffer(bool async, Encoding encoding, int byteCount, PgReader reader, CancellationToken cancellationToken)
    {
        // Never utf8, but we may still be able to save a copy.
        byte[]? rentedBuffer = null;
        if (!reader.TryReadBytes(byteCount, out ReadOnlyMemory<byte> buffer))
        {
            rentedBuffer = ArrayPool<byte>.Shared.Rent(byteCount);
            if (async)
                await reader.ReadBytesAsync(rentedBuffer.AsMemory(0, byteCount), cancellationToken).ConfigureAwait(false);
            else
                reader.ReadBytes(rentedBuffer.AsSpan(0, byteCount));
            buffer = rentedBuffer.AsMemory(0, byteCount);
        }

        var charCount = encoding.GetCharCount(buffer.Span);
        var chars = ArrayPool<char>.Shared.Rent(charCount);
        encoding.GetChars(buffer.Span, chars);

        return (new(chars, 0, charCount), rentedBuffer);
    }

    public static Size GetSizeCore(bool jsonb, MemoryStream stream, Encoding encoding, ref object? writeState)
    {
        if (encoding.CodePage == Encoding.UTF8.CodePage)
        {
            writeState = stream;
            return (int)stream.Length + (jsonb ? sizeof(byte) : 0);
        }

        if (!stream.TryGetBuffer(out var buffer))
            throw new InvalidOperationException();

        var bytes = encoding.GetBytes(Encoding.UTF8.GetChars(buffer.Array!, buffer.Offset, buffer.Count));
        writeState = bytes;
        return bytes.Length + (jsonb ? sizeof(byte) : 0);
    }

    public static async ValueTask Write(bool jsonb, bool async, PgWriter writer, CancellationToken cancellationToken)
    {
        if (jsonb)
        {
            if (writer.ShouldFlush(sizeof(byte)))
                await writer.Flush(async, cancellationToken).ConfigureAwait(false);
            writer.WriteByte(JsonbProtocolVersion);
        }

        ArraySegment<byte> buffer;
        switch (writer.Current.WriteState)
        {
        case MemoryStream stream:
            if (!stream.TryGetBuffer(out buffer))
                throw new InvalidOperationException();
            break;
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
}
