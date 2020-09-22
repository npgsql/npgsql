using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL line data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("line", NpgsqlDbType.Line, typeof(NpgsqlLine))]
    public class LineHandler : NpgsqlSimpleTypeHandler<NpgsqlLine>
    {
        /// <inheritdoc />
        public LineHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override NpgsqlLine Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new NpgsqlLine(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlLine value, NpgsqlParameter? parameter)
            => 24;

        /// <inheritdoc />
        public override void Write(NpgsqlLine value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteDouble(value.A);
            buf.WriteDouble(value.B);
            buf.WriteDouble(value.C);
        }
    }
}
