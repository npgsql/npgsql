using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql.Internal;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Npgsql.Json.NET.Internal;

sealed class JsonNetJsonConverter<T>(bool jsonb, Encoding textEncoding, JsonSerializerSettings settings) : PgStreamingConverter<T?>
{
    public override T? Read(PgReader reader)
        => (T?)JsonNetJsonConverter.Read(async: false, jsonb, reader, typeof(T), settings, textEncoding, CancellationToken.None).GetAwaiter().GetResult();
    public override async ValueTask<T?> ReadAsync(PgReader reader, CancellationToken cancellationToken = default)
        => (T?)await JsonNetJsonConverter.Read(async: true, jsonb, reader, typeof(T), settings, textEncoding, cancellationToken).ConfigureAwait(false);

    public override Size GetSize(SizeContext context, T? value, ref object? writeState)
        => JsonNetJsonConverter.GetSize(jsonb, context, typeof(T), settings, textEncoding, value, ref writeState);

    public override void Write(PgWriter writer, T? value)
        => JsonNetJsonConverter.Write(jsonb, async: false, writer, CancellationToken.None).GetAwaiter().GetResult();

    public override ValueTask WriteAsync(PgWriter writer, T? value, CancellationToken cancellationToken = default)
        => JsonNetJsonConverter.Write(jsonb, async: true, writer, cancellationToken);
}

// Split out to avoid unnecessary code duplication.
static class JsonNetJsonConverter
{
    public const byte JsonbProtocolVersion = 1;

    public static async ValueTask<object?> Read(bool async, bool jsonb, PgReader reader, Type type, JsonSerializerSettings settings, Encoding encoding, CancellationToken cancellationToken)
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
            await stream.CopyToAsync(mem, Math.Min((int)mem.Length, 81920), cancellationToken).ConfigureAwait(false);
        else
            stream.CopyTo(mem);
        mem.Position = 0;
        var jsonSerializer = JsonSerializer.CreateDefault(settings);
        using var textReader = new JsonTextReader(new StreamReader(mem, encoding));
        return jsonSerializer.Deserialize(textReader, type);
    }

    public static Size GetSize(bool jsonb, SizeContext context, Type type, JsonSerializerSettings settings, Encoding encoding, object? value, ref object? writeState)
    {
        var jsonSerializer = JsonSerializer.CreateDefault(settings);
        var sb = new StringBuilder(256);
        var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
        using (var jsonWriter = new JsonTextWriter(sw))
        {
            jsonWriter.Formatting = jsonSerializer.Formatting;

            jsonSerializer.Serialize(jsonWriter, value, type);
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
}
