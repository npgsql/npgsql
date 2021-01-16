using System;
using System.Threading;
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

            return JsonConvert.DeserializeObject<T>(await base.Read<string>(buf, len, async, fieldDescription), _settings);
        }

        protected override int ValidateAndGetLength<T2>(T2 value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (typeof(T2) == typeof(string) ||
                typeof(T2) == typeof(char[]) ||
                typeof(T2) == typeof(ArraySegment<char>) ||
                typeof(T2) == typeof(char) ||
                typeof(T2) == typeof(byte[]))
            {
                return base.ValidateAndGetLength(value, ref lengthCache, parameter);
            }

            var serialized = JsonConvert.SerializeObject(value, _settings);
            if (parameter != null)
                parameter.ConvertedValue = serialized;
            return base.ValidateAndGetLength(serialized, ref lengthCache, parameter);
        }

        protected override Task WriteWithLength<T2>(T2 value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (typeof(T2) == typeof(string) ||
                typeof(T2) == typeof(char[]) ||
                typeof(T2) == typeof(ArraySegment<char>) ||
                typeof(T2) == typeof(char) ||
                typeof(T2) == typeof(byte[]))
            {
                return base.WriteWithLength(value, buf, lengthCache, parameter, async, cancellationToken);
            }

            // User POCO, read serialized representation from the validation phase
            var serialized = parameter?.ConvertedValue != null
                ? (string)parameter.ConvertedValue
                : JsonConvert.SerializeObject(value, _settings);
            return base.WriteWithLength(serialized, buf, lengthCache, parameter, async, cancellationToken);
        }

        protected override int ValidateObjectAndGetLength(object value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
        {
            if (value is DBNull ||
                value is string ||
                value is char[] ||
                value is ArraySegment<char> ||
                value is char ||
                value is byte[])
            {
                return base.ValidateObjectAndGetLength(value, ref lengthCache, parameter);
            }

            return ValidateAndGetLength(value, ref lengthCache, parameter);
        }

        protected override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (value is DBNull ||
                value is string ||
                value is char[] ||
                value is ArraySegment<char> ||
                value is char ||
                value is byte[])
            {
                return base.WriteObjectWithLength(value, buf, lengthCache, parameter, async, cancellationToken);
            }

            return WriteWithLength(value, buf, lengthCache, parameter, async, cancellationToken);
        }
    }
}
