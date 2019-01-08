using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-oid.html
    /// </remarks>
    [TypeMapping("oid", NpgsqlDbType.Oid)]
    [TypeMapping("xid", NpgsqlDbType.Xid)]
    [TypeMapping("cid", NpgsqlDbType.Cid)]
    [TypeMapping("regtype", NpgsqlDbType.Regtype)]
    [TypeMapping("regconfig", NpgsqlDbType.Regconfig)]
    class UInt32Handler : NpgsqlSimpleTypeHandler<uint>
    {
        public override uint Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => (uint)buf.ReadInt32();

        public override int ValidateAndGetLength(uint value, NpgsqlParameter parameter) => 4;

        public override void Write(uint value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteInt32((int)value);
    }
}
