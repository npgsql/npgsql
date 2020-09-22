using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL point data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("point", NpgsqlDbType.Point, typeof(NpgsqlPoint))]
    public class PointHandler : NpgsqlSimpleTypeHandler<NpgsqlPoint>
    {
        /// <inheritdoc />
        public PointHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override NpgsqlPoint Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble());

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlPoint value, NpgsqlParameter? parameter)
            => 16;

        /// <inheritdoc />
        public override void Write(NpgsqlPoint value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteDouble(value.X);
            buf.WriteDouble(value.Y);
        }
    }
}
