using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric circle type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("circle", NpgsqlDbType.Circle, typeof(NpgsqlCircle))]
    class CircleHandler : NpgsqlSimpleTypeHandler<NpgsqlCircle>
    {
        public override NpgsqlCircle Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new NpgsqlCircle(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        public override int ValidateAndGetLength(NpgsqlCircle value, NpgsqlParameter parameter)
            => 24;

        public override void Write(NpgsqlCircle value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteDouble(value.X);
            buf.WriteDouble(value.Y);
            buf.WriteDouble(value.Radius);
        }
    }
}
