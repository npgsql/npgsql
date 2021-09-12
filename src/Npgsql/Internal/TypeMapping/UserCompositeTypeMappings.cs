using System;
using Npgsql.Internal.TypeHandlers.CompositeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping
{
    public interface IUserCompositeTypeMapping : IUserTypeMapping
    {
        INpgsqlNameTranslator NameTranslator { get; }
    }

    class UserCompositeTypeMapping<T> : IUserCompositeTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType => typeof(T);
        public INpgsqlNameTranslator NameTranslator { get; }

        public UserCompositeTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            => (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);

        public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector)
            => new CompositeHandler<T>((PostgresCompositeType)pgType, connector.TypeMapper, NameTranslator);
    }
}
