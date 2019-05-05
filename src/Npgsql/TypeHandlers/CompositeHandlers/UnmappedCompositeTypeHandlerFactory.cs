using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#nullable disable // About to be removed

namespace Npgsql.TypeHandlers.CompositeHandlers
{
    class UnmappedCompositeTypeHandlerFactory : NpgsqlTypeHandlerFactory<object>
    {
        readonly INpgsqlNameTranslator _nameTranslator;

        internal UnmappedCompositeTypeHandlerFactory(INpgsqlNameTranslator nameTranslator)
        {
            _nameTranslator = nameTranslator;
        }

        public override NpgsqlTypeHandler<object> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new UnmappedCompositeHandler(postgresType, _nameTranslator, conn.Connector.TypeMapper);
    }
}
