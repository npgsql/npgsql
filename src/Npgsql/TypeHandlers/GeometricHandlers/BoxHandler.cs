using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL box data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("box", NpgsqlDbType.Box, typeof(NpgsqlBox))]
    public class BoxHandler : NpgsqlSimpleTypeHandler<NpgsqlBox>
    {
        /// <inheritdoc />
        public BoxHandler(PostgresType postgresType) : base(postgresType) {}

        /// <inheritdoc />
        public override NpgsqlBox Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new NpgsqlBox(
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()),
                new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble())
            );

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlBox value, NpgsqlParameter? parameter)
            => 32;

        /// <inheritdoc />
        public override void Write(NpgsqlBox value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteDouble(value.Right);
            buf.WriteDouble(value.Top);
            buf.WriteDouble(value.Left);
            buf.WriteDouble(value.Bottom);
        }
    }
}
