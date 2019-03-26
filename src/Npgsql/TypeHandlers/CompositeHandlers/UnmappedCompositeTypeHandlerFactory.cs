using Npgsql.TypeHandling;

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class UnmappedCompositeTypeHandlerFactory : NpgsqlTypeHandlerFactory<object>
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        internal UnmappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            _nameTranslator = nameTranslator;
        }

        protected override NpgsqlTypeHandler<object> Create(NpgsqlConnection conn)
            => new UnmappedCompositeHandler(_nameTranslator, conn.Connector.TypeMapper);
    }
}
