using System;
using Npgsql.Internal.TypeHandlers.CompositeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping;

sealed class UserCompositeTypeMapping<T> : IUserTypeMapping
{
    INpgsqlNameTranslator NameTranslator { get; }
    public string PgTypeName { get; }
    public Type ClrType => typeof(T);

    public UserCompositeTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
        => (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);

    public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector)
        => new CompositeHandler<T>((PostgresCompositeType)pgType, connector.TypeMapper, NameTranslator);
}