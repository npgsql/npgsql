using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class MappedCompositeTypeHandlerFactory<T> : NpgsqlTypeHandlerFactory<T>, IMappedCompositeTypeHandlerFactory
    {
        public INpgsqlNameTranslator NameTranslator { get; }

        internal MappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
            => NameTranslator = nameTranslator;

        public override NpgsqlTypeHandler<T> Create(PostgresType pgType, NpgsqlConnection conn)
            => new MappedCompositeHandler<T>((PostgresCompositeType)pgType, conn.Connector!.TypeMapper, NameTranslator);
    }
}
