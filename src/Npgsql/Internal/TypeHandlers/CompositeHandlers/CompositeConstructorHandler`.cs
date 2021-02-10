using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeHandlers.CompositeHandlers
{
    sealed class CompositeConstructorHandler<TComposite, T1, T2, T3, T4, T5, T6, T7, T8> : CompositeConstructorHandler<TComposite>
    {
        delegate TComposite CompositeConstructor(in Arguments args);

        readonly CompositeConstructor _constructor;

        public CompositeConstructorHandler(PostgresType postgresType, ConstructorInfo constructorInfo, CompositeParameterHandler[] parameterHandlers)
            : base(postgresType, constructorInfo, parameterHandlers)
        {
            var parameter = Expression.Parameter(typeof(Arguments).MakeByRefType());
            var fields = Enumerable
                .Range(1, parameterHandlers.Length)
                .Select(i => Expression.Field(parameter, "Argument" + i));

            _constructor = Expression
                .Lambda<CompositeConstructor>(Expression.New(constructorInfo, fields), parameter)
                .Compile();
        }

        public override async ValueTask<TComposite> Read(NpgsqlReadBuffer buffer, bool async)
        {
            await buffer.Ensure(sizeof(int), async);

            var fieldCount = buffer.ReadInt32();
            if (fieldCount != Handlers.Length)
                throw new InvalidOperationException($"pg_attributes contains {Handlers.Length} fields for type {PostgresType.DisplayName}, but {fieldCount} fields were received.");

            var args = default(Arguments);

            foreach (var handler in Handlers)
                switch (handler.ParameterPosition)
                {
                    case 0: args.Argument1 = await handler.Read<T1>(buffer, async); break;
                    case 1: args.Argument2 = await handler.Read<T2>(buffer, async); break;
                    case 2: args.Argument3 = await handler.Read<T3>(buffer, async); break;
                    case 3: args.Argument4 = await handler.Read<T4>(buffer, async); break;
                    case 4: args.Argument5 = await handler.Read<T5>(buffer, async); break;
                    case 5: args.Argument6 = await handler.Read<T6>(buffer, async); break;
                    case 6: args.Argument7 = await handler.Read<T7>(buffer, async); break;
                    case 7: args.Argument8 = await handler.Read<T8>(buffer, async); break;
                }

            return _constructor(args);
        }

        struct Arguments
        {
            public T1 Argument1;
            public T2 Argument2;
            public T3 Argument3;
            public T4 Argument4;
            public T5 Argument5;
            public T6 Argument6;
            public T7 Argument7;
            public T8 Argument8;
        }
    }
}
