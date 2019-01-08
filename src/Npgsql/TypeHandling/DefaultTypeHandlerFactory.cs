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
            var baseClass = handlerType;
            while (!baseClass.GetTypeInfo().IsGenericType || baseClass.GetGenericTypeDefinition() != typeof(NpgsqlTypeHandler<>))
            {
                baseClass = baseClass.GetTypeInfo().BaseType;
                if (baseClass == null)
                    throw new Exception($"Npgsql type handler {handlerType} doesn't inherit from TypeHandler<>?");
            }

            DefaultValueType = baseClass.GetGenericArguments()[0];
            _handlerType = handlerType;
        }

        internal override NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn)
        {
            var handler = (NpgsqlTypeHandler)Activator.CreateInstance(_handlerType);
            handler.PostgresType = pgType;
            return handler;
        }

        internal override Type DefaultValueType { get; }
    }
}
