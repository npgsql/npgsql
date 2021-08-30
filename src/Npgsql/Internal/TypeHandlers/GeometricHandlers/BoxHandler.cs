using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL box data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class BoxHandler : NpgsqlSimpleTypeHandler<NpgsqlBox>
    {
        public BoxHandler(PostgresType pgType) : base(pgType) {}

        /// <inheritdoc />
        public override NpgsqlBox Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new(
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
