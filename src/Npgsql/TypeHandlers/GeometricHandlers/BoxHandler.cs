using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric box type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("box", NpgsqlDbType.Box, typeof(NpgsqlBox))]
    class BoxHandler : NpgsqlSimpleTypeHandler<NpgsqlBox>
    {
        public override NpgsqlBox Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new NpgsqlBox(
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()),
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble())
            );

        public override int ValidateAndGetLength(NpgsqlBox value, NpgsqlParameter parameter)
            => 32;

        public override void Write(NpgsqlBox value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteDouble(value.Right);
            buf.WriteDouble(value.Top);
            buf.WriteDouble(value.Left);
            buf.WriteDouble(value.Bottom);
        }
    }
}
