using System;
using System.Reflection;
using System.Threading.Tasks;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class CompositeConstructorHandler<TComposite>
    {
        public PostgresType PostgresType { get; }
        public ConstructorInfo ConstructorInfo { get; }
        public CompositeParameterHandler[] Handlers { get; }

        protected CompositeConstructorHandler(PostgresType postgresType, ConstructorInfo constructorInfo, CompositeParameterHandler[] handlers)
        {
            PostgresType = postgresType;
            ConstructorInfo = constructorInfo;
            Handlers = handlers;
        }

        public virtual async ValueTask<TComposite> Read(NpgsqlReadBuffer buffer, bool async)
        {
            await buffer.Ensure(sizeof(int), async);

            var fieldCount = buffer.ReadInt32();
            if (fieldCount != Handlers.Length)
                throw new InvalidOperationException($"pg_attributes contains {Handlers.Length} fields for type {PostgresType.DisplayName}, but {fieldCount} fields were received.");

            var args = new object?[Handlers.Length];
            foreach (var handler in Handlers)
                args[handler.Position] = await handler.Read(buffer, async);

            return (TComposite)ConstructorInfo.Invoke(args);
        }

        public static CompositeConstructorHandler<TComposite> Create(PostgresType postgresType, ConstructorInfo constructorInfo, CompositeParameterHandler[] parameterHandlers)
        {
            const int maxGenericParameters = 8;

            if (parameterHandlers.Length > maxGenericParameters)
                return new CompositeConstructorHandler<TComposite>(postgresType, constructorInfo, parameterHandlers);

            var parameterTypes = new Type[maxGenericParameters + 1];
            for (var parameterIndex = 0; parameterIndex < maxGenericParameters; ++parameterIndex)
                parameterTypes[parameterIndex + 1] = parameterIndex < parameterHandlers.Length
                    ? parameterHandlers[parameterIndex].ParameterInfo.ParameterType
                    : typeof(Unused);

            parameterTypes[0] = typeof(TComposite);
            return (CompositeConstructorHandler<TComposite>)Activator.CreateInstance(
                typeof(CompositeConstructorHandler<,,,,,,,,>).MakeGenericType(parameterTypes),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { postgresType, constructorInfo, parameterHandlers },
                culture: null)!;
        }

        readonly struct Unused
        {
        }
    }
}
