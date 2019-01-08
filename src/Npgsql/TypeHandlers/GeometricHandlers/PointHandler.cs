using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric point type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("point", NpgsqlDbType.Point, typeof(NpgsqlPoint))]
    class PointHandler : NpgsqlSimpleTypeHandler<NpgsqlPoint>
    {
        public override NpgsqlPoint Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble());

        public override int ValidateAndGetLength(NpgsqlPoint value, NpgsqlParameter parameter)
            => 16;

        public override void Write(NpgsqlPoint value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteDouble(value.X);
            buf.WriteDouble(value.Y);
        }
    }
}
