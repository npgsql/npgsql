using System.Data;
using Npgsql.BackendMessages;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/current/static/datatype-boolean.html
    /// </remarks>
    [TypeMapping("boolean", NpgsqlDbType.Boolean, DbType.Boolean, typeof(bool))]
    class BoolHandler : NpgsqlSimpleTypeHandler<bool>
    {
        public override bool Read(NpgsqlReadBuffer buf, int len, FieldDescription fieldDescription = null)
            => buf.ReadByte() != 0;

        public override int ValidateAndGetLength(bool value, NpgsqlParameter parameter)
            => 1;

        public override void Write(bool value, NpgsqlWriteBuffer buf, NpgsqlParameter parameter)
            => buf.WriteByte(value ? (byte)1 : (byte)0);
    }
}
