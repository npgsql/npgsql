using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL line data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class LineHandler : NpgsqlSimpleTypeHandler<NpgsqlLine>
    {
        public LineHandler(PostgresType pgType) : base(pgType) {}

        /// <inheritdoc />
        public override NpgsqlLine Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

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
