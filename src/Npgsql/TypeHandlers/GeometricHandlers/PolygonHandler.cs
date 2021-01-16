using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL polygon data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("polygon", NpgsqlDbType.Polygon, typeof(NpgsqlPolygon))]
    public class PolygonHandler : NpgsqlTypeHandler<NpgsqlPolygon>
    {
        /// <inheritdoc />
        public PolygonHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override async ValueTask<NpgsqlPolygon> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(4, async);
            var numPoints = buf.ReadInt32();
            var result = new NpgsqlPolygon(numPoints);
            for (var i = 0; i < numPoints; i++)
            {
                await buf.Ensure(16, async);
                result.Add(new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()));
            }
            return result;
        }

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlPolygon value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => 4 + value.Count * 16;

        /// <inheritdoc />
        public override async Task Write(NpgsqlPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async, cancellationToken);
            buf.WriteInt32(value.Count);

            foreach (var p in value)
            {
                if (buf.WriteSpaceLeft < 16)
                    await buf.Flush(async, cancellationToken);
                buf.WriteDouble(p.X);
                buf.WriteDouble(p.Y);
            }
        }

        #endregion
    }
}
