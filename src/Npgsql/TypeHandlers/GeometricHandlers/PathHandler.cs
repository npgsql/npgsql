using System;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// Type handler for the PostgreSQL geometric path segment type (open or closed).
    /// </summary>
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-geometric.html
    /// </remarks>
    [TypeMapping("path", NpgsqlDbType.Path, typeof(NpgsqlPath))]
    class PathHandler : NpgsqlTypeHandler<NpgsqlPath>
    {
        #region Read

        public override async ValueTask<NpgsqlPath> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription fieldDescription = null)
        {
            await buf.Ensure(5, async);
            bool open;
            var openByte = buf.ReadByte();
            switch (openByte)
            {
            case 1:
                open = false;
                break;
            case 0:
                open = true;
                break;
            default:
                throw new Exception("Error decoding binary geometric path: bad open byte");
            }

            var numPoints = buf.ReadInt32();
            var result = new NpgsqlPath(numPoints, open);
            for (var i = 0; i < numPoints; i++)
            {
                await buf.Ensure(16, async);
                result.Add(new NpgsqlPoint(buf.ReadDouble(), buf.ReadDouble()));
            }
            return result;
        }

        #endregion

        #region Write

        public override int ValidateAndGetLength(NpgsqlPath value, ref NpgsqlLengthCache lengthCache, NpgsqlParameter parameter)
            => 5 + value.Count * 16;

        public override async Task Write(NpgsqlPath value, NpgsqlWriteBuffer buf, NpgsqlLengthCache lengthCache, NpgsqlParameter parameter, bool async)
        {
            if (buf.WriteSpaceLeft < 5)
                await buf.Flush(async);
            buf.WriteByte((byte)(value.Open ? 0 : 1));
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
