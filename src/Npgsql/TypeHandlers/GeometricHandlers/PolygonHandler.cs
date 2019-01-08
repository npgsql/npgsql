using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric polygon type.
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("polygon", NpgsqlDbType.Polygon, typeof(NpgsqlPolygon))]
    class PolygonHandler : NpgsqlTypeHandler<NpgsqlPolygon>
    {
        #region Read

        public override async ValueTask<NpgsqlPolygon> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
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

        public override int ValidateAndGetLength(NpgsqlPolygon value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => 4 + value.Count * 16;

        public override async Task Write(NpgsqlPolygon value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 4)
                await buf.Flush(async);
            buf.WriteInt32(value.Count);

            foreach (var p in value)
            {
                if (buf.WriteSpaceLeft < 16)
                    await buf.Flush(async);
                buf.WriteDouble(p.X);
                buf.WriteDouble(p.Y);
            }
        }

        #endregion
    }
}
