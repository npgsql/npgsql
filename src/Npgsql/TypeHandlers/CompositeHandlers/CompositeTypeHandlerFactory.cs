using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class CompositeTypeHandlerFactory<T> : NpgsqlTypeHandlerFactory<T>, ICompositeTypeHandlerFactory
    {
        public INpgsqlNameTranslator NameTranslator { get; }

        internal CompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
            => NameTranslator = nameTranslator;

        public override NpgsqlTypeHandler<T> Create(PostgresType pgType, NpgsqlConnection conn)
            => new CompositeHandler<T>((PostgresCompositeType)pgType, conn.Connector!.TypeMapper, NameTranslator);
    }
}
