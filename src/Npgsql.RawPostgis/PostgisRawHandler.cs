using Npgsql.TypeHandlers;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.LegacyPostgis
{
    public class PostgisRawHandlerFactory : NpgsqlTypeHandlerFactory<byte[]>
    {
        protected override NpgsqlTypeHandler<byte[]> Create(NpgsqlConnection conn)
            => new PostgisRawHandler();
    }

    class PostgisRawHandler : ByteaHandler {}
}
