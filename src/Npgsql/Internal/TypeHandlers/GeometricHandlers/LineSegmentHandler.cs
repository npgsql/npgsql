using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL lseg data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class LineSegmentHandler : NpgsqlSimpleTypeHandler<NpgsqlLSeg>
    {
        public LineSegmentHandler(PostgresType pgType) : base(pgType) {}

        /// <inheritdoc />
        public override NpgsqlLSeg Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null)
            => new(buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble(), buf.ReadDouble());

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlLSeg value, NpgsqlParameter? parameter)
            => 32;

        /// <inheritdoc />
        public override void Write(NpgsqlLSeg value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter)
        {
            buf.WriteDouble(value.Start.X);
            buf.WriteDouble(value.Start.Y);
            buf.WriteDouble(value.End.X);
            buf.WriteDouble(value.End.Y);
        }
    }
}
