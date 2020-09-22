using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL circle data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("circle", NpgsqlDbType.Circle, typeof(NpgsqlCircle))]
    public class CircleHandler : NpgsqlSimpleTypeHandler<NpgsqlCircle>
    {
        /// <inheritdoc />
        public CircleHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override NpgsqlCircle Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new NpgsqlCircle(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlCircle value, NpgsqlParameter? parameter)
            => 24;

        /// <inheritdoc />
        public override void Write(NpgsqlCircle value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteDouble(value.X);
            buf.WriteDouble(value.Y);
            buf.WriteDouble(value.Radius);
        }
    }
}
