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

        public override NpgsqlTypeHandler<T> Create(PostgresType pgType, NpgsqlConnection conn)
            => new MappedCompositeHandler<T>((PostgresCompositeType)pgType, conn.Connector!.TypeMapper, NameTranslator);
    }
}
