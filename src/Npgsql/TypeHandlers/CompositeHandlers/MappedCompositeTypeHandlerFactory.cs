using System;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class MappedCompositeTypeHandlerFactory<T> : NpgsqlTypeHandlerFactory<T>, IMappedCompositeTypeHandlerFactory
        where T : new()
    {
        public INpgsqlNameTranslator NameTranslator { get; }

        internal MappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
            => NameTranslator = nameTranslator;

        internal override NpgsqlTypeHandler Create(PostgresType pgType, NpgsqlConnection conn)
            => MappedCompositeHandler<T>.Create((PostgresCompositeType)pgType, conn.Connector.TypeMapper, NameTranslator);

        protected override NpgsqlTypeHandler<T> Create(NpgsqlConnection conn)
            => throw new InvalidOperationException($"Expect {nameof(PostgresType)}");
    }
}
