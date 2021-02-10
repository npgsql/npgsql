using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;

namespace Npgsql.NetTopologySuite.Internal
{
    public class NetTopologySuiteHandlerFactory : NpgsqlTypeHandlerFactory<Geometry>
    {
        readonly PostGisReader _reader;
        readonly PostGisWriter _writer;

        internal NetTopologySuiteHandlerFactory(PostGisReader reader, PostGisWriter writer)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public override NpgsqlTypeHandler<Geometry> Create(PostgresType postgresType, NpgsqlConnection conn)
            => new NetTopologySuiteHandler(postgresType, _reader, _writer);
    }
}
