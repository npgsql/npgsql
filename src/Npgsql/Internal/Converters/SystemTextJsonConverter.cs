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

sealed class SystemTextJsonConverter<T, TBase> : PgStreamingConverter<T?> where T: TBase?
{
    readonly bool _jsonb;
    readonly Encoding _textEncoding;
    readonly JsonTypeInfo _jsonTypeInfo;
    readonly JsonTypeInfo<object?>? _objectTypeInfo;

    public SystemTextJsonConverter(bool jsonb, Encoding textEncoding, JsonSerializerOptions serializerOptions)
    {
        // We do GetTypeInfo calls directly so we need a resolver.
        if (serializerOptions.TypeInfoResolver is null)
            serializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();

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
        // We always fall back to buffers on older targets due to the absence of transcoding stream.
        if (SystemTextJsonConverter.TryReadStream(_jsonb, _textEncoding, reader, out var byteCount, out var stream))
        {
            using var _ = stream;
            if (_jsonTypeInfo is JsonTypeInfo<T> typeInfoOfT)
                return async
                    ? await JsonSerializer.DeserializeAsync(stream, typeInfoOfT, cancellationToken).ConfigureAwait(false)
                    : JsonSerializer.Deserialize(stream, typeInfoOfT);

            return (T?)(async
                ? await JsonSerializer.DeserializeAsync(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo, cancellationToken).ConfigureAwait(false)
                : JsonSerializer.Deserialize(stream, (JsonTypeInfo<TBase?>)_jsonTypeInfo));
        }
        else
        {
            var (rentedChars, rentedBytes) = await SystemTextJsonConverter.ReadRentedBuffer(async, _textEncoding, byteCount, reader, cancellationToken).ConfigureAwait(false);
            var result = _jsonTypeInfo is JsonTypeInfo<T> typeInfoOfT
                ? JsonSerializer.Deserialize(rentedChars.AsSpan(), typeInfoOfT)
                : (T?)JsonSerializer.Deserialize(rentedChars.AsSpan(), (JsonTypeInfo<TBase?>)_jsonTypeInfo);

            ArrayPool<char>.Shared.Return(rentedChars.Array!);
            if (rentedBytes is not null)
                ArrayPool<byte>.Shared.Return(rentedBytes);

            return result;
        }
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

        return SystemTextJsonConverter.GetSizeCore(_jsonb, stream, _textEncoding, ref writeState);
    }

    public override void Write(PgWriter writer, T? value)
        => SystemTextJsonConverter.Write(_jsonb, async: false, writer, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => SystemTextJsonConverter.Write(_jsonb, async: true, writer, cancellationToken);
}

// Split out to avoid unneccesary code duplication.
static class SystemTextJsonConverter
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
        // We always fall back to buffers on older targets
        if (isUtf8
#if !NETSTANDARD
            || byteCount >= StreamingThreshold
#endif
            )
        {
            stream =
#if !NETSTANDARD
                !isUtf8
                    ? Encoding.CreateTranscodingStream(reader.GetStream(), encoding, Encoding.UTF8)
                    : reader.GetStream();
#else
                reader.GetStream();
            Debug.Assert(isUtf8);
#endif
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
            writer.WriteByte(JsonbProtocolVersion);

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
