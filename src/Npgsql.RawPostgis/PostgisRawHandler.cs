using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.RawPostgis
{
    public class PostgisRawHandlerFactory : NpgsqlTypeHandlerFactory<byte[]>
    {
        public override NpgsqlTypeHandler<byte[]> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new PostgisRawHandler(postgresType);
    }

    class PostgisRawHandler : ByteaHandler
    {
        public PostgisRawHandler(PostgresType postgresType) : base(postgresType) {}
    }
}
