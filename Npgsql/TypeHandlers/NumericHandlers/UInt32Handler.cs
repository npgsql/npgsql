using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql.BackendMessages;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <remarks>
    /// http://www.postgresql.org/docs/9.3/static/datatype-oid.html
    /// </remarks>
    [TypeMapping("oid", NpgsqlDbType.Oid)]
    [TypeMapping("xid", NpgsqlDbType.Xid)]
    [TypeMapping("cid", NpgsqlDbType.Cid)]
    internal class UInt32Handler : TypeHandler<uint>,
        ISimpleTypeReader<uint>, ISimpleTypeWriter
    {
        public uint Read(NpgsqlBuffer buf, FieldDescription fieldDescription, int len)
        {
            return (uint)buf.ReadInt32();
        }

        public int ValidateAndGetLength(object value) { return 4; }

        public void Write(object value, NpgsqlBuffer buf)
        {
            buf.WriteInt32((int)(uint)value);
        }
    }
}
