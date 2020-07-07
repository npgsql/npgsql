using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using System;
using System.Text.Json;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.SystemJson
{
    public class JsonHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        private readonly JsonSerializerOptions _settings;

        public JsonHandlerFactory(JsonSerializerOptions settings = null)
            => _settings = settings ?? new JsonSerializerOptions();

        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonHandler(postgresType, conn, _settings);
    }

    internal class JsonHandler : TypeHandlers.TextHandler
    {
        private readonly JsonSerializerOptions _settings;

        public JsonHandler(PostgresType postgresType, NpgsqlConnection connection, JsonSerializerOptions settings)
            : base(postgresType, connection) => _settings = settings;

        protected override async ValueTask<T> Read<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var s = await base.Read<string>(buf, len, async, fieldDescription);
            if (typeof(T) == typeof(string))
                return (T)(object)s;

            return JsonSerializer.Deserialize<T>(s, _settings);
        }

        protected override int ValidateAndGetLength<T2>(T2 value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => typeof(T2) == typeof(string)
                ? base.ValidateAndGetLength(value, ref lengthCache, parameter)
                : ValidateObjectAndGetLength(value, ref lengthCache, parameter);

        protected override Task WriteWithLength<T2>(T2 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
            => typeof(T2) == typeof(string)
                ? base.WriteWithLength(value, buf, lengthCache, parameter, async)
                : WriteObjectWithLength(value, buf, lengthCache, parameter, async);

        protected override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
        {
            if (!(value is string s))
            {
                s = JsonSerializer.Serialize(value, _settings);
                if (parameter != null)
                    parameter.ConvertedValue = s;
            }
            return base.ValidateAndGetLength(s, ref lengthCache, parameter);
        }

        protected override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value is DBNull)
                return base.WriteObjectWithLength(DBNull.Value, buf, lengthCache, parameter, async);

            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;
            var s = value as string ?? JsonSerializer.Serialize(value, _settings);
            return base.WriteObjectWithLength(s, buf, lengthCache, parameter, async);
        }
    }
}
