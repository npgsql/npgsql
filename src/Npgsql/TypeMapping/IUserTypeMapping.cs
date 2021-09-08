using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping
{
    interface IUserTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType { get; }

        public NpgsqlTypeHandler CreateHandler(PostgresType pgType, NpgsqlConnector connector);
    }
}
