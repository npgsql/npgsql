using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers;

/// <summary>
/// A type handler for the PostgreSQL json and jsonb data type which uses System.Text.Json.
/// </summary>
/// <remarks>
/// See https://www.postgresql.org/docs/current/datatype-json.html.
///
/// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
/// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
/// Use it at your own risk.
/// </remarks>
public class SystemTextJsonHandler : JsonTextHandler
{
    readonly JsonSerializerOptions _serializerOptions;
    readonly bool _isJsonb;
    readonly int _headerLen;

    /// <summary>
    /// Prepended to the string in the wire encoding
    /// </summary>
    const byte JsonbProtocolVersion = 1;

    static readonly JsonSerializerOptions DefaultSerializerOptions = new();

    /// <inheritdoc />
    public SystemTextJsonHandler(PostgresType postgresType, Encoding encoding, bool isJsonb, JsonSerializerOptions? serializerOptions = null)
        : base(postgresType, encoding, isJsonb)
    {
        _serializerOptions = serializerOptions ?? DefaultSerializerOptions;
        _isJsonb = isJsonb;
        _headerLen = isJsonb ? 1 : 0;
    }

    /// <inheritdoc />
    protected internal override int ValidateAndGetLengthCustom<TAny>([DisallowNull] TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        if (IsSupportedAsText<TAny>())
            return base.ValidateAndGetLengthCustom(value, ref lengthCache, parameter);

        if (typeof(TAny) == typeof(JsonDocument))
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var data = SerializeJsonDocument((JsonDocument)(object)value);
            if (parameter != null)
                parameter.ConvertedValue = data;
            return lengthCache.Set(data.Length + _headerLen);
        }
        
        if (typeof(TAny) == typeof(JsonObject) || typeof(TAny) == typeof(JsonArray))
        {
            lengthCache ??= new NpgsqlLengthCache(1);
            if (lengthCache.IsPopulated)
                return lengthCache.Get();

            var data = SerializeJsonObject((JsonNode)(object)value);
            if (parameter != null)
                parameter.ConvertedValue = data;
            return lengthCache.Set(data.Length + _headerLen);
        }

        // User POCO, need to serialize. At least internally ArrayPool buffers are used...
        var s = JsonSerializer.Serialize(value, _serializerOptions);
        if (parameter != null)
            parameter.ConvertedValue = s;

        return TextHandler.ValidateAndGetLength(s, ref lengthCache, parameter) + _headerLen;
    }

    /// <inheritdoc />
    protected override async Task WriteWithLengthCustom<TAny>([DisallowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken)
    {
        if (IsSupportedAsText<TAny>())
        {
            await base.WriteWithLengthCustom(value, buf, lengthCache, parameter, async, cancellationToken);
            return;
        }

        var spaceRequired = _isJsonb ? 5 : 4;

        if (buf.WriteSpaceLeft < spaceRequired)
            await buf.Flush(async, cancellationToken);

        buf.WriteInt32(ValidateAndGetLength(value, ref lengthCache, parameter));

        if (_isJsonb)
            buf.WriteByte(JsonbProtocolVersion);

        if (typeof(TAny) == typeof(JsonDocument))
        {
            var data = parameter?.ConvertedValue != null
                ? (byte[])parameter.ConvertedValue
                : SerializeJsonDocument((JsonDocument)(object)value);
            await buf.WriteBytesRaw(data, async, cancellationToken);
        }
        else if (typeof(TAny) == typeof(JsonObject) || typeof(TAny) == typeof(JsonArray))
        {
            var data = parameter?.ConvertedValue != null
                ? (byte[])parameter.ConvertedValue
                : SerializeJsonObject((JsonNode)(object)value);
            await buf.WriteBytesRaw(data, async, cancellationToken);
        }
        else
        {
            // User POCO, read serialized representation from the validation phase
            var s = parameter?.ConvertedValue != null
                ? (string)parameter.ConvertedValue
                : JsonSerializer.Serialize(value, value.GetType(), _serializerOptions);

            await TextHandler.Write(s, buf, lengthCache, parameter, async, cancellationToken);
        }
    }

    /// <inheritdoc />
    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => IsSupported(value.GetType())
            ? base.ValidateObjectAndGetLength(value, ref lengthCache, parameter)
            : value switch
            {
                JsonDocument jsonDocument => ValidateAndGetLengthCustom(jsonDocument, ref lengthCache, parameter),
                JsonObject jsonObject => ValidateAndGetLengthCustom(jsonObject, ref lengthCache, parameter),
                JsonArray jsonArray => ValidateAndGetLengthCustom(jsonArray, ref lengthCache, parameter),
                _ => ValidateAndGetLengthCustom(value, ref lengthCache, parameter)
            };

    /// <inheritdoc />
    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value is null or DBNull || IsSupported(value.GetType())
            ? base.WriteObjectWithLength(value, buf, lengthCache, parameter, async, cancellationToken)
            : value switch
            {
                JsonDocument jsonDocument => WriteWithLengthCustom(jsonDocument, buf, lengthCache, parameter, async, cancellationToken),
                JsonObject jsonObject => WriteWithLengthCustom(jsonObject, buf, lengthCache, parameter, async, cancellationToken),
                JsonArray jsonArray => WriteWithLengthCustom(jsonArray, buf, lengthCache, parameter, async, cancellationToken),
                _ => WriteWithLengthCustom(value, buf, lengthCache, parameter, async, cancellationToken),
            };

    /// <inheritdoc />
    protected internal override async ValueTask<T> ReadCustom<T>(NpgsqlReadBuffer buf, int byteLen, bool async, FieldDescription? fieldDescription)
    {
        if (IsSupportedAsText<T>())
        {
            return await base.ReadCustom<T>(buf, byteLen, async, fieldDescription);
        }

        if (_isJsonb)
        {
            await buf.Ensure(1, async);
            var version = buf.ReadByte();
            if (version != JsonbProtocolVersion)
                throw new NotSupportedException($"Don't know how to decode JSONB with wire format {version}, your connection is now broken");
            byteLen--;
        }

        // JsonDocument is a view over its provided buffer, so we can't return one over our internal buffer (see #2811), so we deserialize
        // a string and get a JsonDocument from that. #2818 tracks improving this.
        if (typeof(T) == typeof(JsonDocument))
            return (T)(object)JsonDocument.Parse(await TextHandler.Read<string>(buf, byteLen, async, fieldDescription));

        // User POCO
        if (buf.ReadBytesLeft >= byteLen)
            return JsonSerializer.Deserialize<T>(buf.ReadSpan(byteLen), _serializerOptions)!;

#if NET6_0_OR_GREATER
        return (async
            ? await JsonSerializer.DeserializeAsync<T>(buf.GetStream(byteLen, canSeek: false), _serializerOptions)
            : JsonSerializer.Deserialize<T>(buf.GetStream(byteLen, canSeek: false), _serializerOptions))!;
#else
        return JsonSerializer.Deserialize<T>(await TextHandler.Read<string>(buf, byteLen, async, fieldDescription), _serializerOptions)!;
#endif
    }

    byte[] SerializeJsonDocument(JsonDocument document)
    {
        // TODO: Writing is currently really inefficient - please don't criticize :)
        // We need to implement one-pass writing to serialize directly to the buffer (or just switch to pipelines).
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        document.WriteTo(writer);
        writer.Flush();
        return stream.ToArray();
    }

    byte[] SerializeJsonObject(JsonNode jsonObject)
    {
        // TODO: Writing is currently really inefficient - please don't criticize :)
        // We need to implement one-pass writing to serialize directly to the buffer (or just switch to pipelines).
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);
        jsonObject.WriteTo(writer);
        writer.Flush();
        return stream.ToArray();
    }
}