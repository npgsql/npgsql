using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric line type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("line", NpgsqlDbType.Line, typeof(NpgsqlLine))]
    class LineHandler : NpgsqlSimpleTypeHandler<NpgsqlLine>
    {
        public override NpgsqlLine Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => new NpgsqlLine(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        public override int ValidateAndGetLength(NpgsqlLine value, NpgsqlParameter parameter)
            => 24;

        public override void Write(NpgsqlLine value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            buf.WriteDouble(value.A);
            buf.WriteDouble(value.B);
            buf.WriteDouble(value.C);
        }
    }
}
