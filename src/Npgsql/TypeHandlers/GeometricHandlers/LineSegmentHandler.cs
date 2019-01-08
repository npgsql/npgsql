using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric line segment type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("lseg", NpgsqlDbType.LSeg, typeof(NpgsqlLSeg))]
    class LineSegmentHandler : NpgsqlSimpleTypeHandler<NpgsqlLSeg>
    {
        public override NpgsqlLSeg Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new NpgsqlLSeg(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        public override int ValidateAndGetLength(NpgsqlLSeg value, NpgsqlParameter parameter)
            => 32;

        public override void Write(NpgsqlLSeg value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteDouble(value.Start.X);
            buf.WriteDouble(value.Start.Y);
            buf.WriteDouble(value.End.X);
            buf.WriteDouble(value.End.Y);
        }
    }
}
