using System;
using System.Threading;
using System.Threading.Tasks;
using Npgsql.BackendMessages;
using Npgsql.Internal.TypeHandling;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Internal.TypeHandlers.GeometricHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL path data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-geometric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    public partial class PathHandler : NpgsqlTypeHandler<NpgsqlPath>
    {
        public PathHandler(PostgresType pgType) : base(pgType) {}

        #region Read

        /// <inheritdoc />
        public override async ValueTask<NpgsqlPath> Read(NpgsqlReadBuffer buf, int len, bool async, FieldDescription? fieldDescription = null)
        {
            await buf.Ensure(5, async);
            var open = buf.ReadByte() switch
            {
                1 => false,
                0 => true,
                _ => throw new Exception("Error decoding binary geometric path: bad open byte")
            };

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

        /// <inheritdoc />
        public override int ValidateAndGetLength(NpgsqlPath value, ref NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter)
            => 5 + value.Count * 16;

        /// <inheritdoc />
        public override async Task Write(NpgsqlPath value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
        {
            if (buf.WriteSpaceLeft < 5)
                await buf.Flush(async, cancellationToken);
            buf.WriteByte((byte)(value.Open ? 0 : 1));
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
