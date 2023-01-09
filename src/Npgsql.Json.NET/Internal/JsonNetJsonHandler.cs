using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql.BackendMessages;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Json.NET.Internal;

class JsonNetJsonHandler : JsonTextHandler
{
    readonly JsonSerializerSettings _settings;

    public JsonNetJsonHandler(PostgresType postgresType, NpgsqlConnector connector, bool isJsonb, JsonSerializerSettings settings)
        : base(postgresType, connector.TextEncoding, isJsonb) => _settings = settings;

    protected override async ValueTask<T> ReadCustom<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription)
    {
        if (IsSupportedAsText<T>())
            return await base.ReadCustom<T>(buf, len, async, fieldDescription);

        // JSON.NET returns null if no JSON content was found. This means null may get returned even if T is a non-nullable reference
        // type (for value types, an exception will be thrown).
        return JsonConvert.DeserializeObject<T>(await base.Read<string>(buf, len, async, fieldDescription), _settings)!;
    }

    protected override int ValidateAndGetLengthCustom<TAny>([DisallowNull] TAny value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
    {
        if (IsSupportedAsText<TAny>())
            return base.ValidateAndGetLengthCustom(value, ref lengthCache, parameter);

        var serialized = JsonConvert.SerializeObject(value, _settings);
        if (parameter != null)
            parameter.ConvertedValue = serialized;
        return base.ValidateAndGetLengthCustom(serialized, ref lengthCache, parameter);
    }

    protected override Task WriteWithLengthCustom<TAny>([DisallowNull] TAny value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
    {
        if (IsSupportedAsText<TAny>())
            return base.WriteWithLengthCustom(value, buf, lengthCache, parameter, async, cancellationToken);

        // User POCO, read serialized representation from the validation phase
        var serialized = parameter?.ConvertedValue != null
            ? (string)parameter.ConvertedValue
            : JsonConvert.SerializeObject(value, _settings);
        return base.WriteWithLengthCustom(serialized, buf, lengthCache, parameter, async, cancellationToken);
    }

    public override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        => IsSupported(value.GetType())
            ? base.ValidateObjectAndGetLength(value, ref lengthCache, parameter)
            : ValidateAndGetLengthCustom(value, ref lengthCache, parameter);

    public override Task WriteObjectWithLength(object? value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache,
        NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        => value is null or DBNull || IsSupported(value.GetType())
            ? base.WriteObjectWithLength(value, buf, lengthCache, parameter, async, cancellationToken)
            : WriteWithLengthCustom(value, buf, lengthCache, parameter, async, cancellationToken);
}