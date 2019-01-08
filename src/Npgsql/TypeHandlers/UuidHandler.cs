using System;
using Npgsql.BackendMessages;
using NpgsqlTypes;
using System.Data;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-uuid.html
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    class UuidHandler : NpgsqlSimpleTypeHandler<Guid>
    {
        public override Guid Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var a = buf.ReadInt32();
            var b = buf.ReadInt16();
            var c = buf.ReadInt16();
            var d = new byte[8];
            buf.ReadBytes(d, 0, 8);
            return new Guid(a, b, c, d);
        }

        #region Write

        public override int ValidateAndGetLength(Guid value, NpgsqlParameter parameter)
            => 16;

        public override void Write(Guid value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            // TODO: Allocation... investigate alternatives?
            var bytes = value.ToByteArray();
            buf.WriteInt32(BitConverter.ToInt32(bytes, 0));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 4));
            buf.WriteInt16(BitConverter.ToInt16(bytes, 6));
            buf.WriteBytes(bytes, 8, 8);
        }

        #endregion
    }
}
