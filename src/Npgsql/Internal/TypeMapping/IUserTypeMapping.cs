using System;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.Internal.TypeMapping
{
    public interface IUserTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType { get; }

        public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector);
    }
}
