using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.Json.NET
{
    public class JsonbHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerSettings _settings;

        public JsonbHandlerFactory(JsonSerializerSettings? settings = null)
            => _settings = settings ?? new JsonSerializerSettings();

        public override NpgsqlTypeHandler<string> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new JsonbHandler(postgresType, conn, _settings);
    }

    class JsonbHandler : Npgsql.TypeHandlers.JsonHandler
    {
        readonly JsonSerializerSettings _settings;

        public JsonbHandler(PostgresType postgresType, NpgsqlConnection connection, JsonSerializerSettings settings)
            : base(postgresType, connection, isJsonb: true) => _settings = settings;

        protected override async ValueTask<T> Read<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            if (typeof(T) == typeof(string)             ||
                typeof(T) == typeof(char[])             ||
                typeof(T) == typeof(ArraySegment<char>) ||
                typeof(T) == typeof(char)               ||
                typeof(T) == typeof(byte[]))
            {
                return await base.Read<T>(buf, len, async, fieldDescription);
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(await base.Read<string>(buf, len, async, fieldDescription), _settings);
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
        }

        protected override int ValidateAndGetLength<T2>(T2 value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => typeof(T2) == typeof(string)
                ? base.ValidateAndGetLength(value, ref lengthCache, parameter)
                : ValidateObjectAndGetLength(value!, ref lengthCache, parameter);

        protected override Task WriteWithLength<T2>(T2 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
            => typeof(T2) == typeof(string)
                ? base.WriteWithLength(value, buf, lengthCache, parameter, async)
                : WriteObjectWithLength(value!, buf, lengthCache, parameter, async);

        protected override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            switch (value)
            {
            case string _:
            case char[] _:
            case ArraySegment<char> _:
            case char _:
            case byte[] _:
                return base.ValidateObjectAndGetLength(value, ref lengthCache, parameter);
            default:
                var serialized = JsonConvert.SerializeObject(value, _settings);
                if (parameter != null)
                    parameter.ConvertedValue = serialized;
                return base.ValidateObjectAndGetLength(serialized, ref lengthCache, parameter);
            }
        }

        protected override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async)
        {
            if (value is DBNull)
                return base.WriteObjectWithLength(DBNull.Value, buf, lengthCache, parameter, async);

            switch (value)
            {
            case string _:
            case char[] _:
            case ArraySegment<char> _:
            case char _:
            case byte[] _:
                return base.WriteObjectWithLength(value, buf, lengthCache, parameter, async);
            default:
                // User POCO, read serialized representation from the validation phase
                var serialized = parameter?.ConvertedValue != null
                    ? (string)parameter.ConvertedValue
                    : JsonConvert.SerializeObject(value, _settings);
                return base.WriteObjectWithLength(serialized, buf, lengthCache, parameter, async);
            }
        }
    }
}
