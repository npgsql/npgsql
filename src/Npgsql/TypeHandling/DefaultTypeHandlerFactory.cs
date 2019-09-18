using System;
using System.Reflection;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeHandling
{
    /// <summary>
    /// A type handler factory used to instantiate Npgsql's built-in type handlers.
    /// </summary>
    class DefaultTypeHandlerFactory : NpgsqlTypeHandlerFactory
    {
        readonly Type _handlerType;

        internal DefaultTypeHandlerFactory(Type handlerType)
        {
            // Recursively look for the TypeHandler<T> superclass to extract its T as the
            // DefaultValueType
            Type? baseClass = handlerType;
            while (!baseClass.GetTypeInfo().IsGenericType || baseClass.GetGenericTypeDefinition() != typeof(NpgsqlTypeHandler<>))
            {
                baseClass = baseClass.GetTypeInfo().BaseType;
                if (baseClass == null)
                    throw new Exception($"Npgsql type handler {handlerType} doesn't inherit from TypeHandler<>?");
            }

            DefaultValueType = baseClass.GetGenericArguments()[0];
            _handlerType = handlerType;
        }

        public override NpgsqlTypeHandler CreateNonGeneric(PostgresType pgType, NpgsqlConnection conn)
            => (NpgsqlTypeHandler)Activator.CreateInstance(_handlerType, pgType)!;

        public override Type DefaultValueType { get; }
    }
}
