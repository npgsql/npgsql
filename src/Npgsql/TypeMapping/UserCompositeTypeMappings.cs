using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.CompositeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping
{
    class UserCompositeTypeMapping<T> : IUserTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType => typeof(T);
        INpgsqlNameTranslator NameTranslator { get; }

        public UserCompositeTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            => (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);

        public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector)
            => new CompositeHandler<T>((PostgresCompositeType)pgType, connector.TypeMapper, NameTranslator);
    }
}
