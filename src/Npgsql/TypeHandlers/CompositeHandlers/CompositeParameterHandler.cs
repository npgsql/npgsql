﻿using System.Reflection;
using System.Threading.Tasks;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    abstract class CompositeParameterHandler
    {
        public NpgsqlTypeHandler Handler { get; }
        public ParameterInfo ParameterInfo { get; }
        public int Position { get; }

        public CompositeParameterHandler(NpgsqlTypeHandler handler, ParameterInfo parameterInfo)
        {
            Handler = handler;
            ParameterInfo = parameterInfo;
            Position = parameterInfo.Position;
        }

        public async ValueTask<T> Read<T>(NpgsqlReadBuffer buffer, bool async)
        {
            await buffer.Ensure(sizeof(uint) + sizeof(int), async);

            var oid = buffer.ReadUInt32();
            var length = buffer.ReadInt32();
            if (length == -1)
                return default!;

            return NullableHandler<T>.Exists
                ? await NullableHandler<T>.ReadAsync(Handler, buffer, length, async)
                : await Handler.Read<T>(buffer, length, async);
        }

        public abstract ValueTask<object?> Read(NpgsqlReadBuffer buffer, bool async);
    }
}
