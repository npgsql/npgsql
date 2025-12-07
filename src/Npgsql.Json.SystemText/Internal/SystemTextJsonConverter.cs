using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.Internal;

namespace Npgsql.Json.Internal;

sealed class SystemTextJsonConverter<T>(bool jsonb, Encoding textEncoding, JsonSerializerOptions options) : PgStreamingConverter<T?>
{
    public override T? Read(PgReader reader)
        => (T?)SystemTextJsonConverter.Read(async: false, jsonb, reader, typeof(T), options, textEncoding, CancellationToken.None).GetAwaiter().GetResult();
    public override async ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => (T?)await SystemTextJsonConverter.Read(async: true, jsonb, reader, typeof(T), options, textEncoding, cancellationToken).ConfigureAwait(false);

    public override Size GetSize(SizeContext context, T? value, ref object? writeState)
        => SystemTextJsonConverter.GetSize(jsonb, context, typeof(T), options, textEncoding, value, ref writeState);

    public override void Write(PgWriter writer, T? value)
        => SystemTextJsonConverter.Write(jsonb, async: false, writer, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => SystemTextJsonConverter.Write(jsonb, async: true, writer, cancellationToken);
}

// Split out to avoid unnecessary code duplication.
static class SystemTextJsonConverter
{
    public const byte JsonbProtocolVersion = 1;

    public static async ValueTask<object?> Read(bool async, bool jsonb, PgReader reader, Type type, JsonSerializerOptions options,
        Encoding encoding, CancellationToken cancellationToken)
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

        using var stream = reader.GetStream();
        var mem = new MemoryStream();
        if (async)
            await stream.CopyToAsync(mem, Math.Min((int)stream.Length, 81920), cancellationToken).ConfigureAwait(false);
        else
            stream.CopyTo(mem);
        mem.Position = 0;

        if (async)
            return await JsonSerializer.DeserializeAsync(mem, type, options, cancellationToken).ConfigureAwait(false);
        else
            return JsonSerializer.Deserialize(mem, type, options);
    }

    public static Size GetSize(bool jsonb, SizeContext context, Type type, JsonSerializerOptions options, Encoding encoding, object? value, ref object? writeState)
    {
        var json = JsonSerializer.Serialize(value, type, options);
        var bytes = encoding.GetBytes(json);
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
}
