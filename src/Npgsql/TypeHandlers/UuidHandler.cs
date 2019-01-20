using System;
using System.Data;
using System.Runtime.CompilerServices;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-uuid.html
    /// </remarks>
    [TypeMapping("uuid", NpgsqlDbType.Uuid, DbType.Guid, typeof(Guid))]
    class UuidHandler : NpgsqlSimpleTypeHandler<Guid>
    {
        public override unsafe Guid Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
        {
            var raw = new GuidRaw();

            raw.A = buf.ReadInt32();
            raw.B = buf.ReadInt16();
            raw.C = buf.ReadInt16();
            buf.ReadBytes(new Span<byte>(raw.D, 8));

            return Unsafe.As<GuidRaw, Guid>(ref raw);
        }

        public override int ValidateAndGetLength(Guid value, NpgsqlParameter parameter)
            => 16;

        public override unsafe void Write(Guid value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
        {
            var raw = Unsafe.As<Guid, GuidRaw>(ref value);
            
            buf.WriteInt32(raw.A);
            buf.WriteInt16(raw.B);
            buf.WriteInt16(raw.C);
            buf.WriteBytes(new ReadOnlySpan<byte>(raw.D, 8));
        }

        unsafe struct GuidRaw
        {
            public int A;
            public short B;
            public short C;
            public fixed byte D[8];
        }
    }
}
