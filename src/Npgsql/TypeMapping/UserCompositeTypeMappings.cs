using System;
using Npgsql.Internal;
using Npgsql.Internal.TypeHandlers.CompositeHandlers;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.TypeMapping
{
    interface IUserCompositeTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType { get; }

        public NpgsqlTypeHandler CreateHandler(PostgresCompositeType pgType, NpgsqlConnector conn);
    }

    class UserCompositeTypeMapping<T> : IUserCompositeTypeMapping
    {
        public string PgTypeName { get; }
        public Type ClrType => typeof(T);
        INpgsqlNameTranslator NameTranslator { get; }

        public UserCompositeTypeMapping(string pgTypeName, INpgsqlNameTranslator nameTranslator)
            => (PgTypeName, NameTranslator) = (pgTypeName, nameTranslator);

        public NpgsqlTypeHandler CreateHandler(PostgresCompositeType pgType, NpgsqlConnector conn)
            => new CompositeHandler<T>(pgType, conn.TypeMapper, NameTranslator);
    }
}
