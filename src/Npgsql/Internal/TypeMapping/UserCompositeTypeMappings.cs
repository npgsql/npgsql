using System;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping;

public interface IUserCompositeTypeMapping : IUserTypeMapping
{
    INpgsqlNameTranslator NameTranslator { get; }
}

sealed class UserCompositeTypeMapping<T> : IUserCompositeTypeMapping
{
    public string PgTypeName { get; }
    public Type ClrType => typeof(T);
    public INpgsqlNameTranslator NameTranslator { get; }

    public UserCompositeTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
        => (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);

    public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector)
        => throw new NotImplementedException();
}
