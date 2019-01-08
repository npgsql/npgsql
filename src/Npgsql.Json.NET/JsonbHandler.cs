using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.Json.NET
{
    public class JsonbHandlerFactory : NpgsqlTypeHandlerFactory<string>
    {
        readonly JsonSerializerSettings _settings;

        public JsonbHandlerFactory(JsonSerializerSettings settings) => _settings = settings;

        protected override NpgsqlTypeHandler<string> Create(NpgsqlConnection conn)
            => new JsonbHandler(conn, _settings);
    }

    class JsonbHandler : Npgsql.TypeHandlers.JsonbHandler
    {
        readonly JsonSerializerSettings _settings;

        public JsonbHandler(NpgsqlConnection connection, JsonSerializerSettings settings) : base(connection) => _settings = settings;

        protected override async ValueTask<T> Read<T>(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            var s = await base.Read<string>(buf, len, async, fieldDescription);
            if (typeof(T) == typeof(string))
                return (T)(object)s;
            try
            {
                return JsonConvert.DeserializeObject<T>(s, _settings);
            }
            catch (Exception e)
            {
                throw new NpgsqlSafeReadException(e);
            }
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
            var s = value as string;
            if (s == null)
            {
                s = JsonConvert.SerializeObject(value, _settings);
                if (parameter != null)
                    parameter.ConvertedValue = s;
            }
            return base.ValidateObjectAndGetLength(s, ref lengthCache, parameter);
        }

        protected override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (value == null || value is DBNull)
                return base.WriteObjectWithLength(value, buf, lengthCache, parameter, async);

            if (parameter?.ConvertedValue != null)
                value = parameter.ConvertedValue;
            var s = value as string ?? JsonConvert.SerializeObject(value, _settings);
            return base.WriteObjectWithLength(s, buf, lengthCache, parameter, async);
        }
    }
}
