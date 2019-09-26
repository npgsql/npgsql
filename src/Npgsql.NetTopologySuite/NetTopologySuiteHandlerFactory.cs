using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Npgsql.NetTopologySuite
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
